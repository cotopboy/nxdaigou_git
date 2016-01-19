using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Infrasturcture.MVC;

namespace NxModule.CommonCS
{
    public abstract class NxModuleControllerBase<T,S> : ControllerBase<T,S>
        where T : class, new()        
        where S : class ,new ()
    {

        protected override string ModuleName
        {
            get { return "Nx"; }
        }

        public override ActionRet ErrorHappenedCallback(Exception exp, string type)
        {
            return ActionRet;
        }
    }
}
