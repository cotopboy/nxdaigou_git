using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.IO;
using daigou.domain;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Encryption.ExtensionMethods;
using Utilities.IO;
using System.Net;

namespace daigou.services
{
    public class EmailService
    {


        private ConfigurationService confiService;

        private JokeProvide jokeProvider;

        public EmailService(ConfigurationService confiService,JokeProvide jokeProvider )
        {
            this.jokeProvider = jokeProvider;
            this.confiService = confiService;
        }

        public void SendMail(string title, string content, List<string> recipientList, List<string> ccList, MailPriority mailPriority,bool IsHTML = false)
        {
            SendMail(title, content, recipientList, ccList, mailPriority, string.Empty, IsHTML);
        }

        public void SendMail(string title, string content, List<string> emailList, List<string> ccList, MailPriority mailPriority, string AttachmentFilePath,bool IsHTML = false)
        {

            if (emailList.Count == 0) return;

            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            foreach (string email in emailList)
            {
                if (!string.IsNullOrEmpty(email.Trim()))
                {
                    msg.To.Add(email);
                }
            }

            msg.From = new MailAddress("nxdaigou@gmail.com", "Nx-Daigou Luebeck", System.Text.Encoding.UTF8);

            foreach (var item in ccList)
            {
                if (!string.IsNullOrEmpty(item.Trim()))
                {
                    msg.CC.Add(item);
                }
            }

            msg.Subject = title;
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Body += content;

            if (AttachmentFilePath.Length != 0 && File.Exists(AttachmentFilePath))
            {

                msg.Attachments.Add(new Attachment(AttachmentFilePath));
            }

            if (IsHTML)
            {
                if (!emailList.Contains("396316082@qq.com"))
                {
                    try
                    {
                        FileInfo htmlAppendix = new FileInfo(DirectoryHelper.CombineWithCurrentExeDir("email_appendix.html"));
                        msg.Body += htmlAppendix.Read();
                    }
                    catch { }
                }
                
                msg.Body += "<br/><hr/><strong>最后附上一短笑话，祝你天天开心  ‵（*∩_∩*）′ Yeah~~</strong><br/>" + jokeProvider.GiveMeAJoke();
                msg.Body += "<br/><hr/><br/><br/><strong>包裹发自 德国 吕贝克 - Luebeck Deutschland 23560 </strong><br/>";
                msg.Body += "<a href=\"http://baike.baidu.com/view/206158.htm?fr=aladdin\">德国 吕贝克 百度百科介绍 </a><br/>";
                msg.Body += "http://baike.baidu.com/view/206158.htm?fr=aladdin";
            }
            else
            {
                msg.Body += "\n \n 最后附上一短笑话，祝你天天开心  ‵（*∩_∩*）′ Yeah~~ \n" + jokeProvider.GiveMeAJoke();
                msg.Body += "\n \n德国 吕贝克 ";
            }

            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = IsHTML;
            msg.Priority = mailPriority;


            //"98.....r..t"
           
            byte[] a = "ODozMzIwc21sdQ==".FromBase64().Decrypt(new byte[] { 1, 2, 3 }, false);

            var str = System.Text.Encoding.Default.GetString(a);

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("nxdaigou@gmail.com", str),
                EnableSsl = true
            };
            client.Send(msg);
        }
    }
}
