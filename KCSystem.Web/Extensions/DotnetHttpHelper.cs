using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace KCSystem.Web.Extensions
{

    #region SOCKET访问方式
    public static class RequestHeader
    {
        public const string Host = "Host";
        public const string AcceptEncoding = "Accept-Encoding";
        public const string AcceptLanguage = "Accept-Language";
        public const string Accept = "Accept";
        public const string Connection = "Connection";
        public const string Cookie = "Cookie";
        public const string UserAgent = "User-Agent";
        public const string ContentType = "Content-Type";
        public const string ContentLength = "Content-Length";
        public const string Origin = "Origin";

    }
    public static class ResponseHeader
    {
        public static string ContentLength = "Content-Length";
        public static string ContentType = "Content-Type";
        public static string ContentEncoding = "Content-Encoding";
        public static string SetCookie = "Set-Cookie";

    }
    public static class Connection
    {
        public static string KeepAlive = "Keep-Alive";
        public static string Close = "Close";
    }
    public enum HttpMethod
    {
        GET, POST
    }

    public class HttpException : Exception
    {
        public HttpException(string message) : base(message) { }
        public HttpException(string message, Exception innerException) : base(message, innerException) { }
    }
    public class HttpRequest
    {
        internal HttpRequest()
        {
        }
        public string Host { set; get; }

        public string IP { get; set; }

        public X509CertificateCollection Certificates { get; set; }

        public string Schema { get; set; } = "http";
        private int port = 80;

        /// <summary>
        /// 错误消息
        /// </summary>
        public Exception Exception { get; set; }
        public int Port
        {
            set
            {
                if (port > 0)
                {
                    port = value;
                }
            }
            get
            {
                return port;
            }
        }
        private HttpMethod method = HttpMethod.GET;
        public HttpMethod Method
        {
            set
            {
                method = value;
            }
            get
            {
                return method;
            }
        }
        private string path = "/";
        public string Path
        {
            set
            {
                path = value;
            }
            get
            {
                return path;
            }
        }
        private NameValueCollection headers = new NameValueCollection();
        public NameValueCollection Headers
        {
            set
            {
                headers = value;
            }
            get
            {
                return headers;
            }
        }        
        public void AddHeader(string name, string value)
        {
            headers[name] = value;
        }
        public string Body { set; get; }
        /// <summary>
        /// Millseconds to wait response
        /// </summary>
        private int timeout = -1;//Never time out
        public int Timeout
        {
            set
            {
                if (timeout < -1)
                {
                    throw new ArgumentOutOfRangeException("Timeout is less than -1");
                }
                timeout = value;
            }
            get
            {
                return timeout;
            }
        }
        private void CheckReqiredParameters()
        {
            if (string.IsNullOrEmpty(Host))
            {
                throw new ArgumentException("Host is blank");
            }
        }
        public string BuilSocketRequest()
        {
            StringBuilder requestBuilder = new StringBuilder();
            FillHeader();
            BuildRequestLine(requestBuilder);
            BuildRequestHeader(requestBuilder);
            BuildRequestBody(requestBuilder);
            return requestBuilder.ToString();
        }
        private void FillHeader()
        {
            if (Method.Equals(HttpMethod.POST))
            {
                if (string.IsNullOrEmpty(Headers[RequestHeader.ContentType]))
                {
                    Headers[RequestHeader.ContentType] = "application/x-www-form-urlencoded";
                }
                if (!string.IsNullOrEmpty(Body) && string.IsNullOrEmpty(Headers[RequestHeader.ContentLength]))
                {
                    Headers[RequestHeader.ContentLength] = Encoding.Default.GetBytes(Body).Length.ToString();
                }
            }
            if (!string.IsNullOrEmpty(Headers[RequestHeader.Connection]))
            {
                Headers[RequestHeader.Connection] = Connection.Close;
            }
            if (string.IsNullOrEmpty(Headers[RequestHeader.Accept]))
            {
                Headers[RequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            }
            if (string.IsNullOrEmpty(Headers[RequestHeader.UserAgent]))
            {
                Headers[RequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; IE 9.0)";
            }
            if (string.IsNullOrEmpty(Headers[RequestHeader.AcceptEncoding]))
            {
                Headers[RequestHeader.AcceptEncoding] = "gzip, deflate";
            }
            if (string.IsNullOrEmpty(Headers[RequestHeader.Host]))
            {
                Headers[RequestHeader.Host] = Host;
            }
        }
        private void BuildRequestLine(StringBuilder requestBuilder)
        {
            if (Method.Equals(HttpMethod.POST))
            {
                requestBuilder.AppendLine(string.Format("POST {0} HTTP/1.1", Path));
            }
            else
            {
                requestBuilder.AppendLine(string.Format("GET {0} HTTP/1.1", Path));
            }
        }
        private void BuildRequestHeader(StringBuilder requestBuilder)
        {
            foreach (string name in Headers)
            {
                requestBuilder.AppendLine(string.Format("{0}: {1}", name, Headers[name]));
            }
        }
        private void BuildRequestBody(StringBuilder requestBuilder)
        {
            requestBuilder.AppendLine();
            if (!string.IsNullOrEmpty(Body))
            {
                requestBuilder.Append(Body);
            }
        }

        /// <summary>
        /// ssl访问
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private static bool ValidateServerCertificate(
         object sender,
         X509Certificate certificate,
         X509Chain chain,
         SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public HttpResponse GetResponse()
        {
            CheckReqiredParameters();
            HttpResponse httpResponse = new HttpResponse();
            string socketRequest = BuilSocketRequest();
            byte[] requestBytes = Encoding.ASCII.GetBytes(socketRequest);
            if (this.Schema.ToLower() == "http")
            {
                try
                {
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        socket.ReceiveTimeout = Timeout;
                        if (string.IsNullOrEmpty(IP))
                        {
                            socket.Connect(Host, Port);
                        }
                        else
                        {
                            socket.Connect(IP, Port);
                        }
                        if (socket.Connected)
                        {
                            socket.Send(requestBytes);
                            ParseResponseLine(socket, httpResponse);
                            ParseResponseHeader(socket, httpResponse);
                            ParseResponseBody(socket, httpResponse);
                            socket.Close();
                            socket.Dispose();
                        }
                    }
                }
                catch (Exception e)
                {
                    this.Exception = e;
                    return null;
                }
            }
            else if (this.Schema.ToLower() == "https")
            {
                try
                {
                    TcpClient tcp = new TcpClient();
                    if (String.IsNullOrEmpty(IP))
                    {
                        IP = Dns.GetHostAddresses(this.Host)[0].ToString();
                    }
                    tcp.Connect(new System.Net.IPEndPoint(IPAddress.Parse(IP), port));
                    if (tcp.Connected)
                    {
                        using (SslStream ssl = new SslStream(tcp.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null))
                        {
                            if (Certificates == null)
                                Certificates = new X509CertificateCollection();
                            ssl.AuthenticateAsClient("ServerName",
                                Certificates,
                                SslProtocols.Tls,
                                false);
                            if (ssl.IsAuthenticated)
                            {
                                ssl.Write(requestBytes);
                                ssl.Flush();
                                ParseResponseLine(ssl, httpResponse);
                                ParseResponseHeader(ssl, httpResponse);
                                ParseResponseBody(ssl, httpResponse);
                                ssl.Close();
                                ssl.Dispose();
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    this.Exception = exp;
                    return null;
                }

            }
            return httpResponse;
        }


        private void ParseResponseLine(SslStream socket, HttpResponse response)
        {
            string responseLine = ReceiveCharBytes(socket, "\r\n");
            responseLine = responseLine.Replace("\r\n", "");
            string[] fields = responseLine.Split(' ');
            if (fields.Length >= 3)
            {
                response.StatusCode = fields[1];
                response.StatusDescription = responseLine.Substring(responseLine.IndexOf(fields[1]) + fields[1].Length + 1);
            }
            else
            {
                throw new HttpException("The response line:'" + responseLine + "' has the wrong format.");
            }
        }
        private void ParseResponseLine(Socket socket, HttpResponse response)
        {
            string responseLine = ReceiveCharBytes(socket, "\r\n");
            responseLine = responseLine.Replace("\r\n", "");
            string[] fields = responseLine.Split(' ');
            if (fields.Length >= 3)
            {
                response.StatusCode = fields[1];
                response.StatusDescription = responseLine.Substring(responseLine.IndexOf(fields[1]) + fields[1].Length + 1);
            }
            else
            {
                throw new HttpException("The response line:'" + responseLine + "' has the wrong format.");
            }
        }
        private void ParseResponseHeader(Socket socket, HttpResponse response)
        {
            string responseHeader = ReceiveCharBytes(socket, "\r\n\r\n");
            string[] headerArry = Regex.Split(responseHeader, "\r\n");
            if (headerArry != null)
            {
                foreach (string header in headerArry)
                {
                    if (!string.IsNullOrEmpty(header))
                    {
                        int start = header.IndexOf(":");
                        if (start > 0)
                        {
                            string name = header.Substring(0, start);
                            string value = "";
                            if (header.Length > start + 2)
                            {
                                value = header.Substring(start + 2);
                            }
                            response.AddHeader(name, value);
                        }
                    }
                }
            }
        }

        private void ParseResponseHeader(SslStream socket, HttpResponse response)
        {
            string responseHeader = ReceiveCharBytes(socket, "\r\n\r\n");
            string[] headerArry = Regex.Split(responseHeader, "\r\n");
            if (headerArry != null)
            {
                foreach (string header in headerArry)
                {
                    if (!string.IsNullOrEmpty(header))
                    {
                        int start = header.IndexOf(":");
                        if (start > 0)
                        {
                            string name = header.Substring(0, start);
                            string value = "";
                            if (header.Length > start + 2)
                            {
                                value = header.Substring(start + 2);
                            }
                            response.AddHeader(name, value);
                        }
                    }
                }
            }
        }
        private string ReceiveCharBytes(Socket socket, string breakFlag)
        {
            StringBuilder builder = new StringBuilder();
            while (true)
            {
                byte[] buff = new byte[1];
                int read = socket.Receive(buff, SocketFlags.None);
                if (read > 0)
                {
                    builder.Append((char)buff[0]);
                }
                if (builder.ToString().EndsWith(breakFlag))
                {
                    break;
                }
            }
            return builder.ToString();
        }
        private string ReceiveCharBytes(SslStream socket, string breakFlag)
        {
            StringBuilder builder = new StringBuilder();
            while (true)
            {
                byte[] buff = new byte[1];
                int read = socket.Read(buff, 0, 1);
                if (read > 0)
                {
                    builder.Append((char)buff[0]);
                }
                if (builder.ToString().EndsWith(breakFlag))
                {
                    break;
                }
            }
            return builder.ToString();
        }

        private byte[] ReceiveBytes(SslStream socket, string breakFlag)
        {
            StringBuilder builder = new StringBuilder();
            List<byte> buffs = new List<byte>();
            while (true)
            {
                byte[] buff = new byte[1];
                int read = socket.Read(buff, 0, 1);
                if (read > 0)
                {
                    builder.Append((char)buff[0]);
                    buffs.Add(buff[0]);
                }
                if (builder.ToString().EndsWith(breakFlag))
                {
                    break;
                }
            }
            return buffs.ToArray();
        }
        private byte[] ReceiveBytes(Socket socket, string breakFlag)
        {
            StringBuilder builder = new StringBuilder();
            List<byte> buffs = new List<byte>();
            while (true)
            {
                byte[] buff = new byte[1];
                int read = socket.Receive(buff);
                if (read > 0)
                {
                    builder.Append((char)buff[0]);
                    buffs.Add(buff[0]);
                }
                if (builder.ToString().EndsWith(breakFlag))
                {
                    break;
                }
            }
            return buffs.ToArray();
        }

        private void ParseResponseBody(SslStream socket, HttpResponse response)
        {
            string contentLen = response.GetHeader(ResponseHeader.ContentLength);
            bool bodyDone = false;
            if (!string.IsNullOrEmpty(contentLen))
            {
                int len = Convert.ToInt32(contentLen);
                if (len > 0)
                {
                    byte[] dataBuffer = new byte[len];
                    int readLen = 0;
                    int readByte = 0;
                    byte[] buffer = new byte[4096];
                    do
                    {
                        readByte = socket.Read(buffer, 0, buffer.Length);
                        if (readByte > 0)
                        {
                            Array.Copy(buffer, 0, dataBuffer, readLen, readByte);
                            Array.Clear(buffer, 0, buffer.Length);
                            readLen += readByte;
                        }
                        else
                        {
                            break;
                        }
                    } while (readLen < len);
                    response.Body = dataBuffer;
                    bodyDone = true;
                }
            }
            if (!bodyDone)
            {
                response.Body = ReceiveBytes(socket, "0\r\n\r\n");
            }
        }


        private void ParseResponseBody(Socket socket, HttpResponse response)
        {
            string contentLen = response.GetHeader(ResponseHeader.ContentLength);
            bool bodyDone = false;
            if (!string.IsNullOrEmpty(contentLen))
            {
                int len = Convert.ToInt32(contentLen);
                if (len > 0)
                {
                    byte[] dataBuffer = new byte[len];
                    int readLen = 0;
                    int readByte = 0;
                    byte[] buffer = new byte[4096];
                    do
                    {
                        readByte = socket.Receive(buffer, SocketFlags.None);
                        if (readByte > 0)
                        {
                            Array.Copy(buffer, 0, dataBuffer, readLen, readByte);
                            Array.Clear(buffer, 0, buffer.Length);
                            readLen += readByte;
                        }
                        else
                        {
                            break;
                        }
                    } while (readLen < len);
                    response.Body = dataBuffer;
                    bodyDone = true;
                }
            }
            if (!bodyDone)
            {
                response.Body = ReceiveBytes(socket, "0\r\n\r\n");
            }
        }
        private string GetResponseHeader(Socket socket)
        {
            StringBuilder builder = new StringBuilder();
            while (true)
            {
                byte[] buff = new byte[1];
                int read = socket.Receive(buff, SocketFlags.None);
                if (read > 0)
                {
                    builder.Append((char)buff[0]);
                }
                if (builder.ToString().Contains("\r\n\r\n"))
                {
                    break;
                }
            }
            return builder.ToString();
        }
        public static HttpRequest Create(string url, string ip = "")
        {
            Uri uri = new Uri(url);
            HttpRequest request = new HttpRequest();
            request.Host = uri.Host;
            request.Port = uri.Port;
            request.Path = uri.PathAndQuery;
            request.Schema = uri.Scheme;
            request.IP = ip;
            return request;
        }

    }

    public class HttpCookie
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public string Expires { get; set; }

        public string MaxAge { get; set; }
        public bool HttpOnly { get; set; }

        public bool Secure { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Name + "=" + this.Value + ";");
            if (!string.IsNullOrEmpty(this.MaxAge))
            {
                sb.Append("Max-Age=" + this.MaxAge + ";");
            }
            if (!string.IsNullOrEmpty(this.Expires))
            {
                sb.Append("expires=" + this.Expires + ";");
            }
            if (!string.IsNullOrEmpty(this.Domain))
            {
                sb.Append("Domain=" + this.Domain + ";");
            }
            if (!string.IsNullOrEmpty(this.Path))
            {
                sb.Append("Path=" + this.Path + ";");
            }
            if (this.HttpOnly)
            {
                sb.Append("HttpOnly;");
            }
            if (this.Secure)
            {
                sb.Append("Secure;");
            }
            string cookie = sb.ToString();
            cookie = cookie.Substring(0, cookie.Length - 1);
            return cookie;

        }
    }
    public class HttpResponse
    {
        internal HttpResponse()
        {
        }
        #region Response Line
        public string StatusCode { internal set; get; }
        public string StatusDescription { internal set; get; }
        #endregion

        #region Response Headers
        private NameValueCollection headers = new NameValueCollection();
        public NameValueCollection Headers { get { return headers; } }
        internal void AddHeader(string name, string value)
        {
            headers[name] = value;
        }
        public string GetHeader(string name)
        {
            return headers[name];
        }
        public long? ContentLength
        {
            get
            {
                if (!string.IsNullOrEmpty(GetHeader(ResponseHeader.ContentLength)))
                {
                    return Convert.ToInt64(GetHeader(ResponseHeader.ContentLength));
                }
                return null;
            }
        }
        public string ContentEncoding
        {
            get
            {
                return GetHeader(ResponseHeader.ContentEncoding);
            }
        }

        public List<HttpCookie> Cookies
        {
            get
            {
                string cookie = this.GetHeader("Set-Cookie");
                if (string.IsNullOrEmpty(cookie))
                {
                    return null;
                }
                else
                {
                    List<HttpCookie> cs = new List<HttpCookie>();
                    string[] value = cookie.Split(';');
                    for (int i = 0; i < value.Length; i++)
                    {
                        int oldIndex = i;
                        HttpCookie c = new HttpCookie();
                        string[] data = value[i].Split('=');
                        c.Name = data[0];
                        c.Value = data[1];
                        c.Secure = false;
                        c.HttpOnly = false;
                        do
                        {
                            oldIndex++;
                            if (oldIndex < value.Length)
                            {
                                data = value[oldIndex].Split('=');
                                if (data[0].ToLower().Trim() == "max-age")
                                {
                                    c.MaxAge = data[1];
                                    i++;
                                }
                                else if (data[0].ToLower().Trim() == "expires")
                                {
                                    c.Expires = data[1];
                                    i++;
                                }
                                else if (data[0].ToLower().Trim() == "domain")
                                {
                                    c.Domain = data[1];
                                    i++;
                                }
                                else if (data[0].ToLower().Trim() == "path")
                                {
                                    c.Path = data[1];
                                    i++;
                                }
                                else if (data[0].ToLower().Trim() == "secure")
                                {
                                    c.Secure = true;
                                    i++;
                                }
                                else if (data[0].ToLower().Trim() == "httponly")
                                {
                                    c.HttpOnly = true;
                                    i++;
                                }
                            }

                        } while (oldIndex < value.Length);
                        cs.Add(c);
                    }
                    return cs;
                }

            }
        }
        #endregion
        public byte[] Body { internal set; get; }

        public Stream GetBodyStream()
        {
            if (Body != null)
            {
                return new MemoryStream(Body);
            }
            return null;
        }
    }
    #endregion

    #region socket http helper
    public class SocketHttpHelper
    {

        public HttpResponse Response { get; set; }
        public string Get(HttpRequest request)
        {
            request.Method = HttpMethod.GET;
            request.Timeout = 20000;

            HttpResponse resp = request.GetResponse();
            if (resp == null)
            {
                return request.Exception.Message;
            }
            this.Response = resp;
            string xoiErrorCode = resp.GetHeader("X-XOI-ErrorCode");
            if (!string.IsNullOrEmpty(xoiErrorCode))
            {
                return string.Empty;
            }
            if (resp.StatusCode == "302" || resp.StatusCode == "303")
            {
                request = HttpRequest.Create(resp.GetHeader("Location"));
                return Get(request);
            }
            return ReadContent(resp);
        }

        public string Post(HttpRequest request)
        {
            request.Method = HttpMethod.POST;
            request.Timeout = 20000;

            HttpResponse resp = request.GetResponse();
            if (resp == null)
            {
                return request.Exception.Message;
            }
            this.Response = resp;
            string xoiErrorCode = resp.GetHeader("X-XOI-ErrorCode");
            if (!string.IsNullOrEmpty(xoiErrorCode))
            {
                return string.Empty;
            }
            if (resp.StatusCode == "302" || resp.StatusCode == "303")
            {
                request = HttpRequest.Create(resp.GetHeader("Location"));
                return Get(request);
            }
            return ReadContent(resp);
        }



        protected string ReadContent(HttpResponse resp)
        {
            StreamReader reader = null;
            try
            {
                switch (resp.ContentEncoding)
                {
                    case "gzip":
                        if (resp.GetHeader("Transfer-Encoding") == "chunked")
                        {
                            StringBuilder len = new StringBuilder();
                            List<byte> bytes = new List<byte>();
                            for (int i = 0; i < resp.Body.Length; i++)
                            {
                                if (resp.Body[i] != 0x0d && resp.Body[i + 1] != 0x0a)
                                {
                                    len.Append((char)resp.Body[i]);
                                }
                                else if (resp.Body[i] == 0x0d && resp.Body[i + 1] == 0x0a)
                                {
                                    i += 2;
                                    if (len.Length == 0)
                                    {
                                        len.Append((char)resp.Body[i]);
                                    }
                                    else
                                    {
                                        int iLen = Convert.ToInt32("0x" + len.ToString(), 16);
                                        if (iLen == 0)
                                            break;
                                        byte[] buff = new byte[iLen];
                                        Array.Copy(resp.Body, i, buff, 0, buff.Length);
                                        i += iLen - 1;
                                        bytes.AddRange(buff);
                                        len.Clear();
                                    }
                                }
                            }
                            reader = new StreamReader(new GZipStream(new MemoryStream(bytes.ToArray()), CompressionMode.Decompress));
                        }
                        else
                        {
                            reader = new StreamReader(new GZipStream(resp.GetBodyStream(), CompressionMode.Decompress));
                        }
                        break;
                    case "deflate":
                        reader = new StreamReader(new DeflateStream(resp.GetBodyStream(), CompressionMode.Decompress));
                        break;
                    default:
                        reader = new StreamReader(resp.GetBodyStream());
                        break;
                }
                return reader.ReadToEnd();
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
    #endregion

    #region 常规HTTPHELPER
    /// <summary>
    /// 封装后的.net header
    /// </summary>
    public class DotnetReuqetHeader
    {
        /// <summary>
        /// 访问网址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 提交时为 application/x-www-form-urlencoded
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 引用网址
        /// </summary>
        public string Refer { get; set; }

        /// <summary>
        /// 接收的内容类型
        /// </summary>
        public string Accept { get; set; } = "*/*";

        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 访问的浏览器类型
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 访问的源网址
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 编码方式
        /// </summary>
        public string Encoding { get; set; } = "GB2312";

        /// <summary>
        /// 默认值
        /// </summary>
        public int TimeOut { get; set; } = 20000;

        /// <summary>
        /// 代理信息
        /// </summary>
        public DotnetHttpProxy Proxy;
    }

    /// <summary>
    /// 代理信息
    /// </summary>
    public class DotnetHttpProxy
    {
        /// <summary>
        /// 代理地址
        /// </summary>
        public string ProxyIP { get; set; }

        /// <summary>
        /// 代理端口
        /// </summary>
        public string ProxyPort { get; set; }

        /// <summary>
        /// 代理用户名
        /// </summary>
        public string ProxyUser { get; set; }

        /// <summary>
        /// 代理密码
        /// </summary>
        public string ProxyPass { get; set; }
    }

    /// <summary>
    /// dotnet http helper
    /// </summary>
    public class DotnetHttpHelper
    {
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="header">HTTP头</param>
        /// <param name="postData">待发送的数据</param>
        /// <param name="cookie">Cookie</param>
        /// <returns></returns>
        public static string Post(DotnetReuqetHeader header, NameValueCollection postData, CookieContainer cookie)
        {
            try
            {
                HttpWebRequest req = buildRequest(header);
                req.Method = "POST";
                req.CookieContainer = cookie;
                req.AllowAutoRedirect = true;
                Encoding encoding = Encoding.GetEncoding(header.Encoding);
                if (string.IsNullOrEmpty(req.ContentType))
                {
                    req.ContentType = "application/x-www-form-urlencoded";
                }
                string body = string.Empty;
                if (postData != null)
                {
                    foreach (string key in postData.AllKeys)
                    {
                        body += "&" + key + "=" + postData[key];
                    }
                    body = body.Substring(1);
                    byte[] btBodys = encoding.GetBytes(body);
                    req.ContentLength = btBodys.Length;
                    req.GetRequestStream().Write(btBodys, 0, btBodys.Length);
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader streamReader = new StreamReader(resp.GetResponseStream(), encoding);
                string responseContent = streamReader.ReadToEnd();
                req.Abort();
                resp.Close();
                streamReader.Close();
                return responseContent;
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        public static string PostImage(string url, IDictionary<object, object> param, byte[] fileByte)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.UserAgent = "TT_C# 2.0";
            wr.Method = "POST";

            //wr.Timeout = 150000;
            //wr.KeepAlive = true;

            //wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            Stream rs = null;
            try
            {
                rs = wr.GetRequestStream();
            }
            catch { return "无法连接.请检查网络."; }
            string responseStr = null;

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in param.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, param[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "image", "i.gif", "image/gif");//image/jpeg
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            rs.Write(fileByte, 0, fileByte.Length);

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();

                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                responseStr = reader2.ReadToEnd();

            }
            catch
            {
                //throw;
            }
            finally
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
                wr.Abort();
                wr = null;

            }
            return responseStr;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="header">HTTP头</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static string PostFile(DotnetReuqetHeader header, string filePath, CookieContainer cookie)
        {
            try
            {
                byte[] data = null;
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        data = br.ReadBytes((int)br.BaseStream.Length);
                    }
                }
                if (data != null)
                {
                    HttpWebRequest req = buildRequest(header);
                    req.Method = "POST";
                    req.CookieContainer = cookie;
                    req.AllowAutoRedirect = true;
                    req.ContentType = "application/octet-stream";
                    req.ContentLength = data.Length;
                    Encoding encoding = Encoding.GetEncoding(header.Encoding);
                    Stream outstream = req.GetRequestStream();
                    outstream.Write(data, 0, data.Length);
                    outstream.Close();
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                    StreamReader streamReader = new StreamReader(resp.GetResponseStream(), encoding);
                    string responseContent = streamReader.ReadToEnd();
                    req.Abort();
                    resp.Close();
                    streamReader.Close();
                    return responseContent;
                }
                return string.Empty;
            }
            catch (Exception exp)
            {
                return "错误" + exp.Message;
            }
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="header">Http头</param>
        /// <param name="cookie">Cookie</param>
        /// <returns></returns>
        public static string Get(DotnetReuqetHeader header, CookieContainer cookie)
        {
            try
            {
                HttpWebRequest req = buildRequest(header);
                req.Method = "GET";
                req.CookieContainer = cookie;
                req.AllowAutoRedirect = true;
                Encoding encoding = Encoding.GetEncoding(header.Encoding);
                if (string.IsNullOrEmpty(req.ContentType))
                {
                    req.ContentType = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader streamReader = new StreamReader(resp.GetResponseStream(), encoding);
                string responseContent = streamReader.ReadToEnd();
                req.Abort();
                resp.Close();
                streamReader.Close();
                return responseContent;
            }
            catch (Exception exp)
            {
                return "错误:" + exp.Message;
            }
        }

        /// <summary>
        /// 获取图片信息
        /// </summary>
        /// <param name="header">HTTP 头</param>
        /// <param name="cookie">Cookie</param>
        /// <returns></returns>
        public static Bitmap GetImage(DotnetReuqetHeader header, CookieContainer cookie)
        {
            try
            {
                HttpWebRequest req = buildRequest(header);
                req.Method = "GET";
                req.CookieContainer = cookie;
                req.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Bitmap bitmap = Bitmap.FromStream(resp.GetResponseStream()) as Bitmap;
                req.Abort();
                resp.Close();
                return bitmap;
            }
            catch
            {
                return null;
            }

        }


        /// <summary>
        /// 将图片转为二进制
        /// </summary>
        /// <param name="img">图片</param>
        /// <returns></returns>
        public static byte[] GetByteImage(Bitmap img)
        {

            byte[] bt = null;
            if (!img.Equals(null))
            {
                using (MemoryStream mostream = new MemoryStream())
                {
                    Bitmap bmp = new Bitmap(img);

                    bmp.Save(mostream, System.Drawing.Imaging.ImageFormat.Jpeg);//将图像以指定的格式存入缓存内存流

                    bt = new byte[mostream.Length];

                    mostream.Position = 0;//设置留的初始位置

                    mostream.Read(bt, 0, Convert.ToInt32(bt.Length));

                }
            }
            return bt;
        }

        /// <summary>
        /// 创建REQUEST
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private static HttpWebRequest buildRequest(DotnetReuqetHeader header)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(header.Url);
            req.Method = "GET";
            req.Timeout = 20000;

            if (!string.IsNullOrEmpty(header.Accept))
            {
                req.Accept = header.Accept;
            }

            if (!string.IsNullOrEmpty(header.ContentType))
            {
                req.ContentType = header.ContentType;
            }

            if (!string.IsNullOrEmpty(header.Host))
            {
                req.Host = header.Host;
            }

            if (!string.IsNullOrEmpty(header.Origin))
            {
                req.Headers.Add("Origin", header.Origin);
            }

            if (!string.IsNullOrEmpty(header.Refer))
            {
                req.Referer = header.Refer;
            }


            req.UserAgent = UserAgent.RandomUserAget();


            req.Timeout = header.TimeOut;
            //否则有就设置
            if (header.Proxy != null)
            {
                WebProxy webProxy = new WebProxy(header.Proxy.ProxyIP, int.Parse(header.Proxy.ProxyPort));
                if (!string.IsNullOrEmpty(header.Proxy.ProxyUser))
                {
                    webProxy.Credentials = new NetworkCredential(header.Proxy.ProxyUser, header.Proxy.ProxyPass);
                }
                req.Proxy = webProxy;
            }

            return req;

        }


    }
    #endregion

    /// <summary>
    /// 浏览器Agent
    /// </summary>
    public class BrowserAgent
    {
        public static string safari_5_Mac = "苹果-Safari";
        public static string safari_5_Windows = "Safari";
        public static string IE_11 = "IE(11)浏览器";
        public static string IE_9 = "IE(9)浏览器";
        public static string Firefox = "火狐浏览器";
        public static string Opera_11_Mac = "苹果-Opera";
        public static string Opera_11_Windows = "Opera";
        public static string Chrome_17_MAC = "苹果Google";
        public static string Chrome_85_Windows = "Google";
        public static string Maxthon = "傲游浏览器";
        public static string IPAD = "IPad浏览器";
        public static string Sogou = "搜狗浏览器";
        public static string QiHu360 = "360浏览器";
        public static string UC = "UC浏览器";
        public static string QQ = "QQ浏览器";

    }
    /// <summary>
    /// 浏览器Agent
    /// </summary>
    public class UserAgent
    {
        public static NameValueCollection UserAgents = new NameValueCollection()
        {
            { BrowserAgent.safari_5_Mac,"Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_8; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50" },
            { BrowserAgent.safari_5_Windows,"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50" },
            { BrowserAgent.Firefox,"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0" },
            { BrowserAgent.IE_11,"Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729; InfoPath.3; rv:11.0) like Gecko" },
            { BrowserAgent.IE_9,"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0;" },
            { BrowserAgent.Opera_11_Mac,"Opera/9.80 (Macintosh; Intel Mac OS X 10.6.8; U; en) Presto/2.8.131 Version/11.11" },
            { BrowserAgent.Opera_11_Windows,"Opera/9.80 (Windows NT 6.1; U; en) Presto/2.8.131 Version/11.11" },
            { BrowserAgent.Chrome_17_MAC,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_0) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11" },
            { BrowserAgent.Maxthon,"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/5.1.0.4000 Chrome/55.0.2883.75 Safari/537.36" },
            { BrowserAgent.IPAD,"Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A403 Safari/8536.25" },
            { BrowserAgent.Sogou,"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.221 Safari/537.36 SE 2.X MetaSr 1.0" },
            { BrowserAgent.QiHu360,"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36" },
            { BrowserAgent.UC, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 UBrowser/6.1.3228.1 Safari/537.36"},
            { BrowserAgent.QQ,"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.25 Safari/537.36 Core/1.70.3741.400 QQBrowser/10.5.3863.400" },
            { BrowserAgent.Chrome_85_Windows,"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36" }
        };

        /// <summary>
        /// 默认的USERAGENT
        /// </summary>
        private static string DefaultUserAgent = UserAgents[BrowserAgent.IE_11];

        /// <summary>
        /// 默认的USERAGENT
        /// </summary>
        public static string Default
        {
            get { return DefaultUserAgent; }
            set { DefaultUserAgent = value; }
        }

        /// <summary>
        /// 随机返回USER AGENT
        /// </summary>
        /// <returns></returns>
        public static string RandomUserAget()
        {
            Random rnd = new Random();
            string key = UserAgents.Keys[rnd.Next(0, UserAgents.Keys.Count)];
            return UserAgents[key];
        }

        /// <summary>
        /// 根据指定键值获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetUserAgent(string key)
        {
            return UserAgents[key];
        }
    }


}