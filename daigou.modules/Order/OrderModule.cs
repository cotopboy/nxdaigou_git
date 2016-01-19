using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using daigou.infrastructure;


namespace daigou.modules.Order
{
    public class OrderModule : IModule
    {
        private IRegionViewRegistry registry;

        

        public OrderModule(IRegionViewRegistry registry)
	    {
            this.registry = registry;
	    }

        public void Initialize()
        {
            this.registry.RegisterViewWithRegion(RegionNames.NaviRegion, typeof(OrderOperationNavi));
            this.registry.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(OrderOperationView));
            
            this.registry.RegisterViewWithRegion(RegionNames.NaviRegion, typeof(OrderPrintNavi));
            this.registry.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(OrderPrintView));

        }


    }
}
