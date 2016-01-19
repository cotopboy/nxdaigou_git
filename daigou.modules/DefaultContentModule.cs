/******************************************************************
 * Author:Ruifeng Zhang
 * CreateTime：9/4/2013 Wednesday 5:57:43 PM
 * Remark:
 * E-mail: ruifeng.zhang@cbb.de 
 *******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using daigou.infrastructure;
using daigou.modules.Command;

namespace daigou.modules
{
    public class DefaultContentModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry;
        private readonly IUnityContainer container;

        public DefaultContentModule(IRegionViewRegistry registry, IUnityContainer container)
        {
            this.regionViewRegistry = registry;
            this.container = container;
        }

        public void Initialize()
        {
            container.RegisterType<ProductVMContainer>(new ContainerControlledLifetimeManager());
        }
    }
}
