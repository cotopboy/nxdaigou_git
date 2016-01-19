using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Infrasturcture.MVC;
using Utilities.IO.Logging.Interfaces;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using WebServer.Infrasturcture;
using Microsoft.Practices.Unity;
using NxModule.CommonCS;
using WebServer.Infrasturcture.Status;
using System.IO;
using WebServer.Infrasturcture.IO;
using Utilities.IO;
using WebServer.Infrasturcture.Validation;


namespace NxModule.Controller
{
    public class AuthenController : NxModuleControllerBase<AuthenModel,object>
    {
        private IUnityContainer container;

  
        public AuthenController(IUnityContainer container)
        {
            this.container = container;
        }

        [AllowAnonymous]
        public ActionRet Welcome()
        {
            Model.IsLoginned = (this.Session.LoginStatus == LoginStatus.Loginned);
            Model.Username = this.Session.GetStringValue("AuthenModel.Username");


            return View("Welcome", Model);
        }

     

    
    }

    public class AuthenModel : NxModuleBaseModel
    {
        public string Index { get { return DateTime.Now.Second.ToString(); } }        
        public bool IsLoginned { get; set; }
        public string Username { get; set; }
    }

   
}

