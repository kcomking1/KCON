using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using KCSystem.Core.Entities;
using Newtonsoft.Json;

namespace KCSystem.Web.Extensions
{
    /// <summary>
    /// 邮件帮助类
    /// </summary>
    public class MessageSender
    {
       
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="toAddress">收件人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        //public static bool SendMail(string toAddress, string title, string content)
        //{
        //    SmtpClient _smtpClient = new SmtpClient();
        //    _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
        //    _smtpClient.Host = SmtpConfig.SmtpServer; //指定SMTP服务器
        //    _smtpClient.Port = SmtpConfig.EmailPort;
        //    _smtpClient.UseDefaultCredentials = true;
        //    _smtpClient.Credentials = new System.Net.NetworkCredential(SmtpConfig.Email, SmtpConfig.EmailPass);//用户名和密码

        //    MailAddress senduser = new MailAddress(SmtpConfig.Email, SmtpConfig.EmailNickname, Encoding.UTF8);
        //    MailAddress touser = new MailAddress(toAddress);

        //    MailMessage _mailMessage = new MailMessage(senduser, touser);
        //    _mailMessage.Subject = title;//主题
        //    _mailMessage.Body = content;//内容
        //    _mailMessage.HeadersEncoding = System.Text.Encoding.UTF8;
        //    _mailMessage.BodyEncoding = System.Text.Encoding.UTF8;//正文编码
        //    _mailMessage.IsBodyHtml = true;//设置为HTML格式
        //    _mailMessage.Priority = MailPriority.High;//优先级

        //    try
        //    {
        //        _smtpClient.Send(_mailMessage);
        //        return true;
        //    }
        //    catch(Exception ex )
        //    {
        //        return false;
        //    }
        //}


        /// <summary>
        /// 动态生成邮箱验证码 base64返回
        /// </summary>
        /// <returns></returns>
        public static string GenerateEmailValidateCode()
        {
            string str = "";
            string[] codes = "a b c d e f g h i j k l m n o p q r s t u v w x y z 0 1 2 3 4 5 6 7 8 9 A B C D E F G H I J K L M N O P Q R S T U V W X Y Z".Split(' ');
            Random rnd = new Random();
            int count = rnd.Next(30, 100);
            for (int i = 0; i < count; i++)
            {
                str += codes[rnd.Next(codes.Length - 1)];
            }
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="toMobile">手机号,最多可以一次提交50000个手机号码</param>
        /// <param name="cashType">消费类型</param>
        /// <param name="content">内容,短信内容，短信内容长度不能超过585个字符。使用URL方式编码为UTF-8格式。短信内容超过70个字符（企信通是60个字符）时，会被拆分成多条，然后以长短信的格式发送。</param>
        /// <returns></returns>
        public static bool SendSms(string toMobile, string content, ILogger _logger, IConfiguration _configuration)
        {
          
            try
            {
                                            
                content = content.Replace("<p>", "").Replace("</p>", "");
                string sendUrl = _configuration.GetSection("SMSConfig")["SMS_SEND_URL"];
                string account = _configuration.GetSection("SMSConfig")["SMS_ACCOUNT"];
                string password = _configuration.GetSection("SMSConfig")["SMS_PWD"];

                string postJsonTpl = "\"account\":\"{0}\",\"password\":\"{1}\",\"phone\":\"{2}\",\"report\":\"false\",\"msg\":\"{3}\"";
                string jsonBody = string.Format(postJsonTpl, account, password, toMobile, HttpUtility.UrlEncode(content));
                string result = doPostMethodToObj(sendUrl, "{" + jsonBody + "}");
                string code = "1";
                if (result.Contains("\"code\":\"0\""))
                {
                    code = "0";
                }

                _logger.LogInformation($"发送短信,手机号码:{toMobile},resultCode={result},postjson={jsonBody}");

                if (code == "0")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                
                _logger.LogInformation($"发送短信失败，Mobile = {toMobile} ,Content = {content} ,Exp = {ex.Message}");
                return false;
            }


        }
        
        public static string doPostMethodToObj(string url, string jsonBody)
        {
            string result = String.Empty;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";            
            httpWebRequest.Method = "POST";

            // Create NetworkCredential Object 
            NetworkCredential admin_auth = new NetworkCredential("username", "password");

            // Set your HTTP credentials in your request header
            httpWebRequest.Credentials = admin_auth;

            // callback for handling server certificates
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {                               
                streamWriter.Write(jsonBody);                
                streamWriter.Flush();
                streamWriter.Close();
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            return result;
        }

        /// <summary>
        /// POST数据
        /// </summary>
        /// <param name="Url">地址</param>
        /// <param name="postDataStr">Post内容</param>
        /// <returns></returns>
        public static string HttpPost(string Url, NameValueCollection postData)
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    byte[] response = web.UploadValues(Url, postData);
                    string result = System.Text.Encoding.UTF8.GetString(response);
                    return result;
                }
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }



    }
}
