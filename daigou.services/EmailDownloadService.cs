using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LumiSoft.Net.POP3.Client;
using LumiSoft.Net.Mail;
using LumiSoft.Net.MIME;
using System.IO;
using daigou.domain;
using System.Diagnostics;

namespace daigou.services
{
    public class EmailDownloadService
    {

        public event Action<string> OnMsgChange;



        private string emailAccount;
        private string password;
        private string savePath;
        private string qqEmailServer = "pop.gmail.com";
        private DirectoryService directoryService;
        private ConfigurationService configurationService;

        public EmailDownloadService(ConfigurationService configurationService,DirectoryService directoryService)
        {
            this.directoryService = directoryService;
            this.configurationService = configurationService;
            this.savePath = directoryService.BaseDir;
            this.emailAccount = "nxdaigou@gmail.com";
            this.password = "980203root";

        }

        public void StartDownload()
        {
            StartEmailDownload();

            Process.Start(this.savePath);
        }

        private void StartEmailDownload()
        {
            
            using (POP3_Client pop3 = new POP3_Client())
            {
                pop3.Connect(this.qqEmailServer, 995, true);

                pop3.Login(this.emailAccount, this.password);
                
                POP3_ClientMessageCollection messages = pop3.Messages;

                Console.WriteLine("EmailCount:{0}", messages.Count);

                for (int i = 0; i < messages.Count; i++)
                {
                    POP3_ClientMessage message = messages[i];   //转化为POP3  

                    Console.WriteLine("\r\nChecking Email :{0} ...", i + 1);

                    if (message != null)
                    {
                        byte[] messageBytes = message.MessageToByte();
                        Mail_Message mime_message = Mail_Message.ParseFromByte(messageBytes);

                        string senderAddress = mime_message.From == null ? "<NULL>" : mime_message.From[0].Address;
                        string subject = mime_message.Subject ?? "<NULL>";

                        Console.WriteLine(subject);
                        
                        DirectoryInfo dir = new DirectoryInfo(this.savePath);
                        if (!dir.Exists) dir.Create();

                        MIME_Entity[] attachments = mime_message.GetAttachments(true);

                        foreach (MIME_Entity entity in attachments)
                        {
                            if (entity.ContentDisposition != null)
                            {
                                string fileName = entity.ContentDisposition.Param_FileName;
                                string extension = new FileInfo(fileName).Extension;

                                if (!string.IsNullOrEmpty(fileName) && extension == ".pdf")
                                {
                                    string path = Path.Combine(dir.FullName, fileName);
                                    MIME_b_SinglepartBase byteObj = (MIME_b_SinglepartBase)entity.Body;
                                    Stream decodedDataStream = byteObj.GetDataStream();
                        
                                    using (FileStream fs = new FileStream(path, FileMode.Create))
                                    {
                                        LumiSoft.Net.Net_Utils.StreamCopy(decodedDataStream, fs, 4000);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
