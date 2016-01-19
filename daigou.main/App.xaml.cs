using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Microsoft.Practices.Unity;
using Utilities.IO.Logging.Interfaces;
using UtilitiesGui;
using Utilities.IO;
using Utilities.IO.Logging;
using System.IO;
using System.Diagnostics;

namespace daigou.main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IUnityContainer container;
        public App()
        {
            string uri = "/Images/logo.png";
            Uri ImageUri1 = new Uri(uri, UriKind.Relative);

            CbbSplash.ShowSplashWindow(
                    "1.0.0.0",
                    "Nx-Daigou",
                    "Nx-Daigou",
                    string.Format("COPYRIGHT © 2010-{0} cbb engineering", DateTime.Today.Year),
                    ImageUri1);


            this.Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(Dispatcher_UnhandledException);
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.toStringEx());
            HtmlFileLog HtmlFileLog = new  HtmlFileLog(Path.Combine(DirectoryHelper.CurrentExeDirectory, "Log"));
            HtmlFileLog.LogException(e.Exception,6,"Default");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();

        
        }



        

    }
}
