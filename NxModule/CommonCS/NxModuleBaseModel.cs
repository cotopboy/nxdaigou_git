using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Infrasturcture.MVC;
using Microsoft.Practices.Unity;
using WebServer.Infrasturcture;
using System.Reflection;

namespace NxModule.CommonCS
{
    public class NxModuleBaseModel : ModelBase
    {
        private string  moduleEtag;

        public NxModuleBaseModel()
        {
            moduleEtag = typeof(NxModuleBaseModel).Assembly.GetName().Version.ToString();
        }

        private WebServerConfig webServerConfig;


        [Dependency]
        public WebServerConfig WebServerConfig
        {
            get { return webServerConfig; }
            set { webServerConfig = value; }
        }

        public override string BaseUrl
        {
            get
            {
                return webServerConfig.BaseUrl;
            }
        }




        public override string ModuleEtag
        {
            get
            {
                return this.moduleEtag;
            }
            set 
            {
                this.moduleEtag = value;
            }            
        }

        public override IDictionary<string, string> TextDict
        {
            get { throw new NotImplementedException(); }
        }
    }
}
