using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using WebServer.Infrasturcture.IO;
using WebServer.Infrasturcture.Status;
using WebServer;
using Utilities.IO.Logging;
using WebServer.Infrasturcture.Rounter;
using Utilities.IO.Logging.Interfaces;
using Microsoft.Practices.Unity;
using WebServer.Infrasturcture;
using WebServer.Infrasturcture.MVC;
using System.Diagnostics;
using Utilities.FileFormats.INI;
using Utilities.IO;
using Utilities.Environment.StartupHelper;
using WebServerConfig;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using System.IO;
using WebServer.Infrasturcture.DataModel.Exceptions;
using daigou.domain.Repository;
using daigou.dal.DaigouDataFile;


namespace daigou.web
{
    public class DaigouWebStarter
    {
        private HttpWebServer httpWebserver;

        private MainLogger mainLogger = new MainLogger();

        public virtual void Start()
        {
            InnerStart();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private void InnerStart()
        {
            cbb_http_web_server config = null;
            try
            {
                config = MainConfig.Instance.cbb_http_web_server;
            }
            catch (Exception exp)
            {
                var settingFile = new FileInfo(DirectoryHelper.CombineWithCurrentExeDir("default_settings.ini"));
                settingFile.Save(WebServerConfig.Properties.Resources.settings);
                Process.Start(settingFile.FullName);

                var ErrorFile = new FileInfo(DirectoryHelper.CombineWithCurrentExeDir("settings_error.log"));
                ErrorFile.Save(exp.GetDetailErrorText());
                Process.Start(ErrorFile.FullName);
                Environment.Exit(-1);
            }

            if (config.IsConsoleLogEnabled) mainLogger.AddLogger(new ConsoleLog());
            if (config.IsHtmlLogEnabled) mainLogger.AddLogger(new HtmlFileLog(DirectoryHelper.CombineWithCurrentExeDir("LogData"), "WebServer"));

            try
            {
                IniFile_Ex inifile = IniFile_Ex.ParseIniFile(DirectoryHelper.CombineWithCurrentExeDir("settings.ini"));

                UnityContainer container = new UnityContainer();
                container.RegisterInstance<ILog>(mainLogger);

                container.RegisterType<MainConfig>(new ContainerControlledLifetimeManager());
                container.RegisterType<CacheMgr>(new ContainerControlledLifetimeManager());
                container.RegisterType<ResourceAssemblyMgr>(new ContainerControlledLifetimeManager());
                container.RegisterType<StatusMgr>(new ContainerControlledLifetimeManager());

                container.RegisterInstance(typeof(IniFile_Ex), inifile);
                container.RegisterInstance(MainConfig.Instance);
                container.RegisterInstance<IUnityContainer>(container);

                container.RegisterType<ProductRepository, FileProductRepository>();

                httpWebserver = container.Resolve<HttpWebServer>();

                httpWebserver.Start();

            }
            catch (NotFreePortException exp)
            {
                mainLogger.LogMessage(exp.GetDetailErrorText(), LogType.Error, "WebServerStarter");
            }
            catch (Exception exp)
            {
                mainLogger.LogMessage(exp.GetDetailErrorText(true), LogType.Error, "WebServerStarter");
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null && e.ExceptionObject != null)
            {
                Exception exp = e.ExceptionObject as Exception;

                if (exp != null)
                {
                    this.mainLogger.LogException(exp, LogType.Error, "UnhandledException");
                    Environment.Exit(0);
                }
            }
        }

        public virtual void Stop()
        {
            httpWebserver.Stop();
        }
    }
}
