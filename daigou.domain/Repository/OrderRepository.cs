using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain
{
    public interface OrderRepository
    {
        IEnumerable<Order> GetAllOrders();
        void Delete(int id);
        void Save(Order target);
        void Add(Order target);
    }
}
