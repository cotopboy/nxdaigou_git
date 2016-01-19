using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using daigou.infrastructure;

namespace daigou.modules.Configuration
{
    public class ConfigurationModule: IModule
    {
        private IRegionViewRegistry registry;
        
        public ConfigurationModule(IRegionViewRegistry registry)
        {
            this.registry = registry;
        }

        public void Initialize()
        {
            this.registry.RegisterViewWithRegion(RegionNames.MainContentRegion,typeof(ConfigurationView));
            this.registry.RegisterViewWithRegion(RegionNames.NaviRegion, typeof(ConfigurationNavi));
        }
 
    }
}
