using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using daigou.domain;

namespace daigou.services.test
{
    [TestClass]
    public class ImapTest
    {
        [TestMethod]
        public void ImapTestGetAttachment()
        {
            ConfigurationService configService = new ConfigurationService(null);

            QQImapService qqImapservice = new QQImapService();

            qqImapservice.SavePath = @"T:\";
            qqImapservice.EmailAccount = "2777888@qq.com";
            qqImapservice.Password = "8765354615156750";

            qqImapservice.DownloadEmailAttachment();
        }
    }
}
