using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using daigou.infrastructure;

namespace daigou.modules.Welcome
{
    public class WelcomeModule : IModule
    {
        private IRegionViewRegistry registry;
        
        public WelcomeModule(IRegionViewRegistry registry)
        {
            this.registry = registry;
        }

        public void Initialize()
        {
            this.registry.RegisterViewWithRegion(RegionNames.MainContentRegion,typeof(WelcomeView));
            this.registry.RegisterViewWithRegion(RegionNames.NaviRegion, typeof(WelcomeNavi));
        }
    }
}
