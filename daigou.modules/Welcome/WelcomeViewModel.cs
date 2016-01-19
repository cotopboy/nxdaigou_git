using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using daigou.domain;

namespace daigou.modules.Welcome
{
    public class WelcomeViewModel : NotificationObject
    {
        private OrderService orderService;

        public WelcomeViewModel(OrderService orderService)
        {
            this.orderService = orderService;            
        }


        public int Last30DaysOrderCount
        {
            get 
            {
                var targetTime = DateTime.Today.AddMonths(-1);
                return this.orderService.GetAllOrders().Count(x => x.CreatedTime >= targetTime);
            }
        }

        public long Last30DaysTotalWeight
        {
            get
            {
                var targetTime = DateTime.Today.AddMonths(-1);
                return this.orderService.GetAllOrders()
                                        .Where(x => x.CreatedTime >= targetTime)
                                        .Sum(x => x.PacketWeight);
            }
        }

        public int CurrentMonthOrderCount
        {
            get
            {
                var now = DateTime.Now;
                var targetTime = new DateTime(now.Year, now.Month, 1);
                return this.orderService.GetAllOrders().Count(x => x.CreatedTime >= targetTime);
            }
        }

        public long CurrentMonthTotalWeight
        {
            get
            {
                var now = DateTime.Now;
                var targetTime = new DateTime(now.Year, now.Month, 1);
                return this.orderService.GetAllOrders()
                                        .Where(x => x.CreatedTime >= targetTime)
                                        .Sum(x => x.PacketWeight);
            }
        }



    }
}
