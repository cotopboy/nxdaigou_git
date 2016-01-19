using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using daigou.infrastructure;

namespace daigou.modules.Product
{
    public class ProductModule : IModule
    {

        private IRegionViewRegistry registry;

        public ProductModule(IRegionViewRegistry registry)
        {
            this.registry = registry;
        }

        public void Initialize()
        {
            this.registry.RegisterViewWithRegion(RegionNames.NaviRegion, typeof(ProductNavi));
            this.registry.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(ProductView));
            this.registry.RegisterViewWithRegion(RegionNames.ProductAddEditRegion, typeof(AddEditProductView));
            this.registry.RegisterViewWithRegion(RegionNames.ProductListRegion, typeof(ProductListView));

        }
    }
}
