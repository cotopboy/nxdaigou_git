using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using daigou.infrastructure;

namespace daigou.modules.Bill
{
    public class BillModule : IModule
    {
        private IRegionViewRegistry registry;

        public BillModule(IRegionViewRegistry registry)
        {
            this.registry = registry;  
        }

        public void Initialize()
        {
            this.registry.RegisterViewWithRegion(RegionNames.NaviRegion, typeof(BillNavi));
            this.registry.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(BillViewLayout));            
            this.registry.RegisterViewWithRegion(RegionNames.BillListRegion, typeof(BillListView));

            this.registry.RegisterViewWithRegion(RegionNames.SearchProductRegion, typeof(SearchProductView));


            this.registry.RegisterViewWithRegion(RegionNames.BillQuickActionRegion, typeof(BillQuickActionView));
        }
    }
}
