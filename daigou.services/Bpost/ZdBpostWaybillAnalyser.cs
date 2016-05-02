using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;

namespace daigou.services
{
    public class ZdBpostWaybillAnalyser
    {
        private DirectoryService directoryService;

        public ZdBpostWaybillAnalyser(DirectoryService directoryService)
        {
            this.directoryService = directoryService;
        }

        public string GetDHLFile(domain.Recipient recipient,uint nameIndex)
        {
            List<string> allfile = this.GetRecipientPdfFileList(recipient, nameIndex);

            return allfile.First(x => x.Contains('0'));
        }

        internal string GetBpostCN23File(domain.Recipient recipient, uint nameIndex)
        {
            List<string> allfile = this.GetRecipientPdfFileList(recipient, nameIndex);

            return allfile.Where(x => x.Contains('2')).First();
        }

        public string GetBpostFile(domain.Recipient recipient, uint nameIndex)
        {
            List<string> allfile = this.GetRecipientPdfFileList(recipient, nameIndex);

            return allfile.Where(x => x.Contains('1')).First();
        }

        public List<string> GetRecipientPdfFileList(domain.Recipient recipient, uint nameIndex)
        {
            List<string> recipientPdfFileList = new List<string>();
            DirectoryInfo fdir = new DirectoryInfo(this.directoryService.GetOrCreateBaseDir());
            FileInfo[] file = fdir.GetFiles();
            HashSet<string> possibleKey = this.GetPossibleKeySet(recipient);

            foreach (var item in file)
            {
                string fileName = item.Name;
                FileInfo info = new FileInfo(fileName);
                if (info.Extension != ".pdf") continue;

                if (this.IsFileBelongToRecipient(fileName, possibleKey))
                {
                    recipientPdfFileList.Add(item.FullName);
                }
            }

            recipientPdfFileList = NameIndexFilter(recipientPdfFileList,nameIndex);

            recipientPdfFileList.Sort();
            recipientPdfFileList.Reverse();
            
            var temp = recipientPdfFileList[1];
            recipientPdfFileList[1] = recipientPdfFileList[2] ;
            recipientPdfFileList[2] = temp;

            return recipientPdfFileList;
        }

        private List<string> NameIndexFilter(List<string> recipientPdfFileList, uint nameIndex)
        {
            List<string> ret = new List<string>();

            foreach (var item in recipientPdfFileList)
            {
                if (item.Count(x => x == '0') == nameIndex) ret.Add(item);
                else if (item.Count(x => x == '1') == nameIndex) ret.Add(item);
                else if (item.Count(x => x == '2') == nameIndex) ret.Add(item);
                else continue;
            }

            return ret;
        }

        private bool IsFileBelongToRecipient(string fileName, HashSet<string> possibleKey)
        {

            foreach (var key in possibleKey)
            {
                if (fileName.ToLower().Contains(key.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        private HashSet<string> GetPossibleKeySet(domain.Recipient recipient)
        {
            string name = recipient.Name;
            string xin = name.Substring(0, 1);
            string min = name.Substring(1);

            HashSet<string> set = new HashSet<string>();
            set.Add(string.Format("{0} {1}0", xin.ToPinyin(), min.ToPinyin().Replace(" ", "")));
            set.Add(string.Format("{0} {1}0", xin.ToPinyin(), min.ToPinyin()));
            set.Add(string.Format("{0}", name.NameToPinyin()));
            set.Add(string.Format("{0}", name.ToPinyin()).Replace(" ", ""));
            set.Add(string.Format("{0}", name.ToPinyin()));
            return set;

        }


       
    }
}
