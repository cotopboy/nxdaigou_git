using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;
using Microsoft.Practices.Unity;

namespace daigou.services
{
    public class WaybillSettingFactory
    {
        
        private ConfigurationRepository configurationRepository;
        private IUnityContainer container;

        public WaybillSettingFactory(ConfigurationRepository configurationRepository,
            IUnityContainer container)
        {
            this.configurationRepository = configurationRepository;
            this.container = container;
        }


        public WaybillSetting Create()
        {
            var configList = this.configurationRepository.GetAllConfiguration();
            Dictionary<string, string> configDict = configList.ToDictionary(v => v.Key, var => var.Value);

            WaybillSetting target = this.container.Resolve<WaybillSetting>();

            target.SenderCity        = configDict["SenderCity"];
            target.SenderCountry     = configDict["SenderCountry"];
            target.SenderHouseNumber = configDict["SenderHouseNumber"];
            target.SenderName        = configDict["SenderName"];
            target.SenderPostCode    = configDict["SenderPostCode"];
            target.SenderStreet      = configDict["SenderStreet"];
            target.SenderTel         = configDict["SenderTel"];
            target.WanwanName        = configDict["WanwanName"];
            target.WaybillEmail      = configDict["WaybillEmail"];
            target.CxEmail = configDict["CxEmail"];
            target.ZdEmail = configDict["ZdEmail"];
            target.CcEmail = configDict["CcEmail"];
            return target;

        }
    }
}
