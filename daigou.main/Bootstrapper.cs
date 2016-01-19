/******************************************************************
 * Author:Ruifeng Zhang
 * CreateTime：9/3/2013 Tuesday 3:36:06 PM
 * Remark:
 * E-mail: ruifeng.zhang@cbb.de 
 *******************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.UnityExtensions;
using System.Windows;
using daigou.domain;
using daigou.dal;
using daigou.services;
using Utilities.IO.Logging;
using Utilities.IO.Logging.Interfaces;
using Utilities.IO;
using System.IO;
using daigou.dal.DaigouDataFile;
using daigou.modules.Order;
using daigou.domain.Repository;
using Utilities.Configuration;

namespace daigou.main
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<Shell>();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

          
            RegisterTypeIfMissing(typeof(RecipientRepository), typeof(FileRecipientRepository), true);
            RegisterTypeIfMissing(typeof(OrderRepository), typeof(FileOrderRepository), true);
            RegisterTypeIfMissing(typeof(ConfigurationRepository), typeof(FileConfigurationRepository), false);
            RegisterTypeIfMissing(typeof(ProductRepository), typeof(FileProductRepository), false);
            RegisterTypeIfMissing(typeof(BillRepository), typeof(FileBillRepository), false);
            


            RegisterTypeIfMissing(typeof(IWaybillInfoExtractorFactory), typeof(WaybillInfoExtractorFactory), false);
            RegisterTypeIfMissing(typeof(IWaybillPrinterFactory), typeof(ＷaybillPrinterFactory), false);
            RegisterTypeIfMissing(typeof(IDhlWaybillEmailBuilderFactory), typeof(DhlWaybillEmailBuilderFactory),true);
            RegisterTypeIfMissing(typeof(IDhlWaybillExcelBuilderFactory), typeof(DhlWaybillExcelBuilderFactory), true);
            RegisterTypeIfMissing(typeof(IManualAddressLineCounter), typeof(ManualAddressCounterDialog), false);
            RegisterTypeIfMissing(typeof(FileDBMgr), typeof(FileDBMgr), true);
            
            this.Container.RegisterInstance(new AppStatusManager(DirectoryHelper.CombineWithCurrentExeDir("settting"))); 


            App.container = this.Container;
        }

      

        protected override void InitializeShell()
        {
            base.InitializeShell();

            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

        

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(daigou.modules.DefaultContentModule), InitializationMode.WhenAvailable);
            moduleCatalog.AddModule(typeof(daigou.modules.Header.HeaderModule));
            moduleCatalog.AddModule(typeof(daigou.modules.Welcome.WelcomeModule));
            moduleCatalog.AddModule(typeof(daigou.modules.Recipient.RecipientModule));
            moduleCatalog.AddModule(typeof(daigou.modules.Order.OrderModule));
            moduleCatalog.AddModule(typeof(daigou.modules.Product.ProductModule));
            moduleCatalog.AddModule(typeof(daigou.modules.Bill.BillModule));     
            moduleCatalog.AddModule(typeof(daigou.modules.Configuration.ConfigurationModule));
            moduleCatalog.AddModule(typeof(daigou.modules.About.AboutModule));

        }
    }
}
