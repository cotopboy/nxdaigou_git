using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using daigou.infrastructure.ExtensionMethods;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;


namespace daigou.services.EMS
{
    public class EMSWaybillAnalyser
    {

        private DirectoryService directoryService;

        public EMSWaybillAnalyser(DirectoryService directoryService)
        {
            this.directoryService = directoryService;
        }


        internal string GetEmsFileName(domain.Recipient recipient, string NameSpecifier)
        {
            var targetFile = GetEmsFileInfo(recipient, NameSpecifier);

            return targetFile.Name;
        }

        internal FileInfo GetEmsFileInfo(domain.Recipient recipient, string NameSpecifier)
        {
            if (NameSpecifier.Length <= 3) NameSpecifier = string.Empty;

            List<string> recipientPdfFileList = new List<string>();
            DirectoryInfo fdir = new DirectoryInfo(this.directoryService.GetOrCreateBaseDir());
            FileInfo[] file = fdir.GetFiles();

            string pyName = recipient.Name.ToPinyin().Replace(" ", "");
            var targetFile = file.Where(x => x.Name.StartsWith(pyName) && x.Name.Contains(NameSpecifier)).FirstOrDefault();

            return targetFile;
        }


        internal List<string> GetRecipientPdfFileList(domain.Recipient recipient, string NameSpecifier)
        {            
            FileInfo emsFile = GetEmsFileInfo(recipient, NameSpecifier);

            string dhlFileName = emsFile.Name.ToLower().SubStringAfter("+",2);
            string dhlFilePath = Path.Combine(emsFile.DirectoryName,dhlFileName);

            return new List<string> { emsFile.FullName, dhlFilePath };

        }
    }
}
