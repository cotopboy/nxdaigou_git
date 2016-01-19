using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;
using System.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;

namespace daigou.dal.DaigouDataFile
{
    public class FileOrderRepository : FileRepositoryBase, OrderRepository
    {
        private List<Order> orderList;

        public FileOrderRepository(FileDBMgr fileDbmgr):base(fileDbmgr)
        {
            this.orderList = FileDB.OrderList;
        }

        public  IEnumerable<Order> GetAllOrders()
        {
            return this.orderList;
        }

        public  void Delete(int id)
        {
            var target = this.orderList.SingleOrDefault(x => x.ID == id);
            if (target != null)
            {
                this.orderList.Remove(target);
            }
            this.fileDbmgr.Save();
        }

        public  void Save(Order target)
        {
            this.fileDbmgr.Save();
        }

        public  void Add(Order target)
        {
            int maxid = this.orderList.Max(x => x.ID);

            target.ID = maxid + 1;

            this.orderList.Add(target);

            this.fileDbmgr.Save();
        }
    }
}
