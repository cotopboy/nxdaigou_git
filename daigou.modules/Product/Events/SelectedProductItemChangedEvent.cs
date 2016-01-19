using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace daigou.modules.Product.Events
{
    public class SelectedProductItemChangedEvent
        :
         CompositePresentationEvent<SelectedProductItemChangedEventPayLoad>
    {


    }

    public class SelectedProductItemChangedEventPayLoad
    {
        public SelectedProductItemChangedEventPayLoad(ProductItemViewModel ProductItemViewModel)
        {
            this.ProductItemViewModel = ProductItemViewModel;
        }
        public ProductItemViewModel ProductItemViewModel { get; set; }
    }


}
