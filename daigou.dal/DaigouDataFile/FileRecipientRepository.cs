using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;

namespace daigou.dal.DaigouDataFile
{
    public class FileRecipientRepository : FileRepositoryBase, RecipientRepository
    {
        private List<Recipient> recipientList;
        
        public FileRecipientRepository(FileDBMgr fileDbmgr) : base(fileDbmgr)
        {           
            this.recipientList = FileDB.RecipientList;
        }

        public  IEnumerable<Recipient> GetAllRecipient()
        {
            return this.recipientList;
        }

        public  Recipient GetRecipient(string recipientName)
        {
            return this.recipientList.First(x => x.Name == recipientName);
        }

        public  Recipient GetRecipient(int id)
        {
            var ret = this.recipientList.First(x => x.ID == id);
            return ret;
        }

        public  void DeleteRecipient(int id)
        {
            var target = this.recipientList.SingleOrDefault(x => x.ID == id);
            if (target != null)
            {
                this.recipientList.Remove(target);
            }
            this.fileDbmgr.Save();
        }

        public  void SaveRecipient(Recipient target)
        {
            this.fileDbmgr.Save();
        }

        public void AddRecipient(Recipient target)
        {

            int maxid = this.recipientList.Max(x => x.ID);

            target.ID = maxid + 1;

            this.recipientList.Add(target);

            this.fileDbmgr.Save();
        }
    }
}
