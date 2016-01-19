using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace daigou.modules.Product.Events
{    
    public class ProductSellPriceChangedEvent
   :
    CompositePresentationEvent<ProductSellPriceChangedEventPayLoad>
    {

    }

    public class ProductSellPriceChangedEventPayLoad
    {
        public ProductSellPriceChangedEventPayLoad()
        {
            
        }

        public decimal Euro2Cny { get; set; }

        public decimal SecondServiceRate { get; set; }
        
    }

}
