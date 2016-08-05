using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using daigou.domain;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using daigou.domain.Services;

namespace daigou.modules.Configuration
{
    public class ConfigurationViewModel : NotificationObject
    {
        private ObservableCollection<domain.Configuration> configurationList;

        private ConfigurationService configurationService;
        private readonly DelegateCommand loadListCommand;
        private readonly DelegateCommand saveCommand;

        private readonly DelegateCommand deleteOldDataCommand;

        public DelegateCommand DeleteOldDataCommand
        {
            get { return deleteOldDataCommand; }
        } 


        public DelegateCommand SaveCommand
        {
            get { return saveCommand; }
        } 


        public DelegateCommand LoadListCommand
        {
            get { return loadListCommand; }
        } 


        public ObservableCollection<domain.Configuration> ConfigurationList
        {
            get { return configurationList; }
            set 
            {
                configurationList = value;
                RaisePropertyChanged("ConfigurationList");
            }
        }

        private readonly BillService billService;

        private readonly OrderService orderService;

        public ConfigurationViewModel
            (
                ConfigurationService configurationService,
                BillService billService,
                OrderService orderService
            )
        {
            this.billService = billService;
            this.orderService = orderService;

            this.configurationService = configurationService;
            this.loadListCommand = new DelegateCommand(LoadList);
            this.saveCommand = new DelegateCommand(Save);
            this.deleteOldDataCommand = new DelegateCommand(DeleteOldData);
        }

        private void DeleteOldData()
        {
            this.billService.DeleteOldData(30);
            this.orderService.DeleteOldData(30);
        }

        private void Save()
        {
            this.configurationService.SaveConfigurations(this.ConfigurationList);
        }

        private void LoadList()
        {
            this.ConfigurationList = new ObservableCollection<domain.Configuration> (this.configurationService.GetAllConfiguration());

        }
 
    }
}
