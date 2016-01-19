using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain.Repository
{
    public interface ProductRepository
    {
        IEnumerable<Product> GetAllOrders();
        void Delete(int id);
        void Save(Product target);
        void Add(Product target);
    }
}
