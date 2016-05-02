using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using daigou.services.EMS;

namespace daigou.services
{
    public interface IDhlWaybillEmailBuilderFactory
    {
        IDhlWaybillEmailBuilder CreateBuilder(ProcessModeInfo type);
    }


    public interface IDhlWaybillExcelBuilderFactory
    {
        IDhlWaybillExcelBuilder CreateBuilder(ProcessModeInfo type);
    }



    public class DhlWaybillEmailBuilderFactory : IDhlWaybillEmailBuilderFactory
    {
        private IUnityContainer container;
        public DhlWaybillEmailBuilderFactory(IUnityContainer container)
        {
            this.container = container;
        }

        public IDhlWaybillEmailBuilder CreateBuilder(ProcessModeInfo type)
        {
            if (type.Name == "诚信安全")
                return this.container.Resolve<CxDhlWaybillEmailBuilder>("CxDhlWaybillEmailBuilder");
            else if (type.Name == "中德快递Bpost")
                return this.container.Resolve<ZdBpostWaybillEmailBuilder>(new ParameterOverride("modeInfo", type));
            else if (type.Name == "EMS")
                return this.container.Resolve<EMSApplyEmailBuilder>(new ParameterOverride("modeInfo", type));
            if (type.Type == "下家客户")
                return this.container.Resolve<ZdBpostWaybillEmailBuilderToClient>(new ParameterOverride("modeInfo", type));
            
            return null;
        }
    }

    public class DhlWaybillExcelBuilderFactory : IDhlWaybillExcelBuilderFactory
    {
         private IUnityContainer container;
         public DhlWaybillExcelBuilderFactory(IUnityContainer container)
        {
            this.container = container;
        }

         public IDhlWaybillExcelBuilder CreateBuilder(ProcessModeInfo type)
         {
             if (type.Name == "诚信安全")
                 return this.container.Resolve<CxDhlWaybillExcelBuilder>("CxDhlWaybillExcelBuilder");
             else if (type.Name == "中德快递Bpost")
                 return this.container.Resolve<ZdBpostWaybillExcelBuilder>(new ParameterOverride("modeInfo", type));
             else if (type.Name == "EMS")
                 return this.container.Resolve<EmsExcelBuilder>(new ParameterOverride("modeInfo", type));

             else if (type.Type == "下家客户")
                 return this.container.Resolve<ZdBpostWaybillExcelBuilder>(new ParameterOverride("modeInfo", type));
            return null;
        }
    }



}
