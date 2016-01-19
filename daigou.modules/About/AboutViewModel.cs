using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace daigou.modules.About
{
    public class AboutViewModel : NotificationObject
    {


        private string changeLog = string.Empty;

        public string ChangeLog
        {
            get 
            {
                return changeLog; 
            }
            set { changeLog = value; }
        }


        public AboutViewModel()
        {
            this.ChangeLog = Properties.Resources.ChangeLog;
        }
    }
}
