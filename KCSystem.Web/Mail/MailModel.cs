using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; 

namespace KCSystem.Web.Mail
{
    /// <summary>
    /// 邮件类 
    /// </summary>
    public class MailModel
    {
        /// <summary>
        /// 发送人
        /// </summary>
        public string FromPerson { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string FromPersonName { get; set; }
        /// <summary>
        /// 收件人地址(多人)
        /// </summary>
        public string[] RecipientArray { get; set; }

        /// <summary>
        /// 抄送地址(多人)
        /// </summary>
        public string[] MailCcArray { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string MailTitle { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string MailBody { get; set; }

        /// <summary>
        /// 客户端授权码(可存在配置文件中)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// SMTP邮件服务器
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 正文是否是html格式
        /// </summary>
        public bool IsBodyHtml { get; set; }
        /// <summary>
        /// 接收文件
        /// </summary>
        public List<IFormFile> Files { get; set; }


        public async Task PostEmails()
        { 
            
            //将发件人邮箱带入MailAddress中初始化
            MailAddress mailAddress = new MailAddress(FromPerson, FromPersonName);
            //创建Email的Message对象
            MailMessage mailMessage = new MailMessage();

            //判断收件人数组中是否有数据
            if (RecipientArray.Any())
            {
                //循环添加收件人地址
                foreach (var item in RecipientArray)
                {
                    if (!string.IsNullOrEmpty(item))
                        mailMessage.To.Add(item);
                }
            }

            //判断抄送地址数组是否有数据
            if (MailCcArray.Any())
            {
                //循环添加抄送地址
                foreach (var item in MailCcArray)
                {
                    if (!string.IsNullOrEmpty(item))
                        mailMessage.To.Add(item);
                }
            }
            //发件人邮箱
            mailMessage.From = mailAddress;
            //标题
            mailMessage.Subject = MailTitle;
            //编码
            mailMessage.SubjectEncoding = Encoding.UTF8;
            //正文
            mailMessage.Body = MailBody;
            //正文编码
            mailMessage.BodyEncoding = Encoding.Default;
            //邮件优先级
            mailMessage.Priority = MailPriority.High;
            //正文是否是html格式
            mailMessage.IsBodyHtml = IsBodyHtml;
           
            //添加附件
            if (Files != null && Files.Any())
            { 
                foreach (IFormFile item in Files)
                {
                     
                    //添加至附件中
                    //mailMessage.Attachments.Add(new Attachment(stream, item.FileName));
                    mailMessage.Attachments.Add(new Attachment(item.OpenReadStream(), item.FileName));
                } 
            }

            //实例化一个Smtp客户端
            //将发件人的邮件地址和客户端授权码带入以验证发件人身份
            //指定SMTP邮件服务器
            var smtp = new SmtpClient
            {
                Credentials = new System.Net.NetworkCredential(FromPerson, Code), Host = Host
            };
           

            //邮件发送到SMTP服务器
            await smtp.SendMailAsync(mailMessage);
            
        }
    }
}