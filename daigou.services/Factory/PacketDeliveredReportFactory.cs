using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using daigou.services.Interfaces;
using daigou.services.Bpost;
using daigou.services.EMS;

namespace daigou.services.Factory
{
    public class PacketDeliveredReportFactory
    {
        
        private IUnityContainer container;
        public PacketDeliveredReportFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public IPacketDeliveredReportService CreateBuilder(string AgentType)
        {
            if (AgentType == "中德快递Bpost")
                return this.container.Resolve<BpostPacketDeliveredReportService>();
            else if (AgentType == "EMS")
                return this.container.Resolve<EmsPacketDeliveredReportService>();
            else
                return null;
        }

    }
}
