using System;
using System.IO;

namespace NPOI
{
    public class SysLog
    {
        private static string ErrorDir = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Error\\";

        private static string InfoDir = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Info\\";

        public static void ClearLog()
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(InfoDir);
                DirectoryInfo error = new DirectoryInfo(ErrorDir);
                DateTime dt = DateTime.Now;
                //删除30天以前的日志
                foreach (FileInfo file in info.GetFiles())
                {

                    if ((dt - file.CreationTime).Days >= 30)
                    {
                        try
                        {
                            File.Delete(file.FullName);
                        }
                        catch { }
                    }
                }
                foreach (FileInfo file in error.GetFiles())
                {
                    if ((dt - file.CreationTime).Days >= 30)
                    {
                        try
                        {
                            File.Delete(file.FullName);
                        }
                        catch { }
                    }
                }


            }
            catch { }
        }
        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="mesasge"></param>
        /// <param name="exp"></param>
        public static void Error(string mesasge, Exception exp = null)
        {
            if (!Directory.Exists(ErrorDir))
                Directory.CreateDirectory(ErrorDir);
            try
            {
                string file = ErrorDir + DateTime.Now.ToString("yyyyMMdd") + ".log";
                using (FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default))
                    {
                        sw.WriteLine("--------------------------------------------------------------------------------------");
                        sw.WriteLine("\t" + DateTime.Now.ToString());
                        sw.WriteLine("\t" + mesasge);
                        if (exp != null)
                        {
                            sw.WriteLine("\t Message:" + exp.Message);
                            sw.WriteLine("\t StackTrace:" + exp.StackTrace);
                            sw.WriteLine("\t InnerException:" + exp.InnerException);
                        }
                        sw.WriteLine();
                        sw.Flush();
                        sw.Close();
                        sw.Dispose();
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
            catch { }
            ClearLog();
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            if (!Directory.Exists(InfoDir))
                Directory.CreateDirectory(InfoDir);
            string file = InfoDir + DateTime.Now.ToString("yyyyMMdd") + ".log";
            using (FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default))
                {
                    sw.WriteLine("--------------------------------------------------------------------------------------");
                    sw.WriteLine("\t" + DateTime.Now.ToString());
                    sw.WriteLine("\t" + message);
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                    fs.Close();
                    fs.Dispose();
                }
            }
            ClearLog();
        }
    }
}
