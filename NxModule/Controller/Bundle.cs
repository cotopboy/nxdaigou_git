using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Infrasturcture;
using WebServer.Infrasturcture.MVC;
using NxModule.CommonCS;
using WebServer.Infrasturcture.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;

namespace NxModule.Controller
{
    public class Bundle : BundleBase<BundleModel,object>
    {
        private WebServerConfig webServerConfig;

        public Bundle(WebServerConfig webServerConfig)
        {
            this.webServerConfig = webServerConfig;        
        }

        [AllowAnonymous]
        public ActionRet CommonCssBundle()
        {
            List<string> bundleList = new List<string>() 
            {
                "/css/templatemo_style.css",
                "/css/slimbox2.css",
                "/css/bootstrap.min.css"
            };

            string key = "CommonCssBundle";
            return GetCssBunlde(bundleList, key);
        }

        /*..*/


        [AllowAnonymous]
        public ActionRet CommonJsBundle()
        {
            List<string> bundleList = new List<string>() 
            {
                "/js/jquery-1.10.2.min.js",
                "/js/knockout-3.1.0.min.js",
                "/js/jquery.timer.js",
                "/js/bootstrap.min.js",
                "/js/test1.js"
            };
           
            string key = "CommonJsBundle";
           
            return GetJsBundle(bundleList, key);
        }




        protected override string ModuleName
        {
            get { return "Nx"; }
        }

        protected override bool IsDevelopMode
        {
            get  { return this.webServerConfig.IsDevelopMode; }           
        }
    }

    public class BundleModel
    {
    
    }
}
