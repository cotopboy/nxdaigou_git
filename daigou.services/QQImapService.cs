using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;
using ImapX;
using System.Net.Mail;

namespace daigou.services
{
    public class QQImapService
    {
        string host = "imap.qq.com";

        public event Action<string> OnMsgChange;

        private string emailAccount;

        public string EmailAccount
        {
          get { return emailAccount; }
          set { emailAccount = value; }
        }
                private string password;

        public string Password
        {
          get { return password; }
          set { password = value; }
        }

        private string savePath;

        public string SavePath
        {
            get { return savePath; }
            set { savePath = value; }
        }

        private DirectoryService directoryService;
        private ConfigurationService configurationService;

        public QQImapService ()
	    {

	    }

        public QQImapService(ConfigurationService configurationService,DirectoryService directoryService)
        {
         
            this.directoryService = directoryService;
            this.configurationService = configurationService;
            this.savePath = directoryService.BaseDir;
            this.emailAccount = configurationService.ConfigDict["SenderQQEmail"];
            this.password = configurationService.ConfigDict["SenderPw"];        
        }

        public void DownloadEmailAttachment()
        {
            ImapClient imapClient = new ImapClient(host);

            imapClient.IsDebug = true;

            if (imapClient.Connect())
            {
                Console.WriteLine("Connect to {0} OK", host);
            }
            else
            {
                Console.WriteLine("Connect to {0} FAILED", host);
                Console.ReadKey();
                return;
            }


            if (imapClient.Login(this.EmailAccount, this.password))
            {
                Console.WriteLine("Login OK");
            }
            else
            {
                Console.ReadKey();
                return;
            }

            Folder testFolder = imapClient.Folders["其他文件夹"].SubFolders["比利时"];

            testFolder.Messages.Download("ALL", ImapX.Enums.MessageFetchMode.Full);

            Console.WriteLine("Messages downloaded! Messages count:{0}", testFolder.Messages.Count());


            List<Message> list = new List<Message>();

            foreach (var message in testFolder.Messages)
            {
                Console.WriteLine("===Message Start===");

                Console.WriteLine("Message Title : {0}", message.Subject);

                Console.WriteLine("Message Header : {0}", string.Join(",", message.Headers.Select(x => x.Key + "->" + x.Value)));

                Console.WriteLine("Message Body : ");

                Console.WriteLine(message.Body.Text);

                Console.WriteLine("Message Attachments");

                foreach (var file in message.Attachments)
                {
                    Console.WriteLine(file.FileName);
                    Console.WriteLine(file.FileSize);
                    Console.WriteLine("--");

                    file.Save(@"T:\");
                }


                Console.WriteLine("===Message End===");

                list.Add(message);
            }


            foreach (var message in list)
            {
                if (message.MoveTo(imapClient.Folders["其他文件夹"].SubFolders["比利时完成"]))
                {
                    Console.WriteLine("Message moved OK");
                }
                else
                {
                    Console.WriteLine("Message moved FAILED");
                }
            }

            var keyInfo = Console.ReadKey(true);


            


            imapClient.Logout();
        }
    }
}
