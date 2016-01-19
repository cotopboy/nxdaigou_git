using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain
{
    public class OrderService
    {
        private OrderRepository repository;

        public OrderService(OrderRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<Order> GetAllOrders()
        {
           return this.repository.GetAllOrders();
        }
        
        public void Delete(int id)
        {
            this.repository.Delete(id);
        }

        public void Save(Order target)
        {
            this.repository.Save(target);
        }

        public Order CreateOrder(int recipientID)
        {
            Order newOrder = new Order();
            newOrder.ID = -1;
            newOrder.CreatedTime = DateTime.Now;
            newOrder.RecipientId = recipientID;
            return newOrder;
        }
        
        public void Add(Order target)
        {
            this.repository.Add(target);
        }

    }
}
