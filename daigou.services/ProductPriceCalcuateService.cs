using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.services
{
    public class ProductPriceCalcuateService
    {

        public decimal GetPrice(domain.Product o, decimal Euro2Cny)
        {
              return Math.Round(o.ImportPrice * Euro2Cny * o.ServiceRate + o.ServiceStaticCost * Euro2Cny + o.PackingCost * Euro2Cny + o.PriceAdaption, 2);
        }
    }
}
