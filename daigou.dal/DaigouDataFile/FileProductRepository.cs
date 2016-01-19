using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain.Repository;
using daigou.domain;

namespace daigou.dal.DaigouDataFile
{
    public class FileProductRepository : FileRepositoryBase, ProductRepository
    {
        private List<Product> productList;
       
        public FileProductRepository(FileDBMgr fileDbmgr) : base(fileDbmgr)
        {            
            this.productList = FileDB.ProductList;
        }

        public  IEnumerable<domain.Product> GetAllOrders()
        {
            return this.productList;
        }

        public  void Delete(int id)
        {
            var target = this.productList.SingleOrDefault(x => x.ID == id);
            if (target != null)
            {
                this.productList.Remove(target);
            }
            this.fileDbmgr.Save();
        }

        public  void Save(domain.Product target)
        {
            this.fileDbmgr.Save();
        }

        public void Add(domain.Product target)
        {
            int ? maxid = this.productList.Max(x => (int?)x.ID);
            

            target.ID = (maxid ?? 0) + 1;

            this.productList.Add(target);

            this.fileDbmgr.Save();
        }
    }
}
