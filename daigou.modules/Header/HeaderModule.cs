using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using daigou.infrastructure;

namespace daigou.modules.Header
{
    public class HeaderModule : IModule
    {
        private IRegionViewRegistry registry;
        public HeaderModule(IRegionViewRegistry registry)
        {
            this.registry = registry;
        }


        public void Initialize()
        {
            this.registry.RegisterViewWithRegion(RegionNames.HeaderRegion, typeof(HeaderView));
        }
    }
}
