using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;

namespace daigou.dal.DaigouDataFile
{
    public class FileBillRepository : FileRepositoryBase, BillRepository
    {
        private List<Bill> billList;

        public FileBillRepository(FileDBMgr fileDbmgr): base(fileDbmgr)
        {
            this.billList = FileDB.BillList;

        }

        public IEnumerable<Bill> GetAllBills()
        {
            return this.billList;
        }

        public  void Delete(int id)
        {
            var target = this.billList.SingleOrDefault(x => x.Id == id);
            if (target != null)
            {
                this.billList.Remove(target);
            }
            this.fileDbmgr.Save();
        }

        public  void Save()
        {
            this.fileDbmgr.Save();
        }

        public  void Add(Bill target)
        {
            int? maxid = this.billList.Max(x => (int?)x.Id);


            target.Id = (maxid ?? 0) + 1;

            this.billList.Add(target);

            this.fileDbmgr.Save();
        }
    }
}
