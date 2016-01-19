using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain.Repository;

namespace daigou.domain.Services
{
    public class ProductService
    {
        private ProductRepository repository;

        public ProductService(ProductRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<Product> GetAllProduct()
        {
            return this.repository.GetAllOrders();
        }


        public void Delete(int targetId)
        {
            this.repository.Delete(targetId);
        }

        public void Save(domain.Product target)
        {
            this.repository.Save(target);
        }

        public void Add(domain.Product target)
        {
            this.repository.Add(target);
        }
    }
}
