using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using daigou.infrastructure;

namespace daigou.modules.About
{
    public class AboutModule : IModule
    {
        private IRegionViewRegistry registry;

        public AboutModule(IRegionViewRegistry registry)
        {
            this.registry = registry;
        }

        public void Initialize()
        {
            this.registry.RegisterViewWithRegion(RegionNames.MainContentRegion,typeof(AboutView));
            this.registry.RegisterViewWithRegion(RegionNames.NaviRegion, typeof(AboutNavi));
        }
    }
}
