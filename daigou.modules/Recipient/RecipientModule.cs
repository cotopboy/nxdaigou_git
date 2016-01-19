using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using daigou.infrastructure;

namespace daigou.modules.Recipient
{
    public class RecipientModule : IModule
    {
        private IRegionViewRegistry registry;

        public RecipientModule(IRegionViewRegistry registry)
        {
            this.registry = registry;
        }

        public void Initialize()
        {
            this.registry.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(RecipientView));
            this.registry.RegisterViewWithRegion(RegionNames.NaviRegion, typeof(RecipientNavi));
            this.registry.RegisterViewWithRegion(RegionNames.RecipientAddRegion, typeof(AddRecipientView));

        }
    }
}
