using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;


namespace daigou.services
{
    public interface IWaybillPrinterFactory
    {
        IWaybillPrinter CreatePrinter(string type);
    }

    public interface IWaybillInfoExtractorFactory
    {
        IWaybillInfoExtractor CreateInfoExtractor(string type);
    }



    public class ＷaybillPrinterFactory : IWaybillPrinterFactory
    {
        private IUnityContainer container;
        public ＷaybillPrinterFactory(IUnityContainer container)
        {
            this.container = container;
        }


        public IWaybillPrinter CreatePrinter(string type)
        {
            if (type == "诚信安全")
                return null;
            else if (type == "中德快递Bpost")
                return this.container.Resolve<ZdBpostWaybillPrinter>("ZdBpostWaybillPrinter");                
            return null;
        }
    }

    public class WaybillInfoExtractorFactory : IWaybillInfoExtractorFactory
    {

         private IUnityContainer container;
         public WaybillInfoExtractorFactory(IUnityContainer container)
        {
            this.container = container;
        }


         public IWaybillInfoExtractor CreateInfoExtractor(string type)
        {
            if (type == "诚信安全")
                return null;
            else if (type == "中德快递Bpost")
                return this.container.Resolve<ZdBpostWaybillInfoExtractor>("ZdBpostWaybillInfoExtractor");
            return null;
        }
    }

}
