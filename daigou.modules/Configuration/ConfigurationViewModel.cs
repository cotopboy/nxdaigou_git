using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using daigou.domain;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace daigou.modules.Configuration
{
    public class ConfigurationViewModel : NotificationObject
    {
        private ObservableCollection<domain.Configuration> configurationList;

        private ConfigurationService configurationService;
        private readonly DelegateCommand loadListCommand;
        private readonly DelegateCommand saveCommand;

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

        public ConfigurationViewModel(ConfigurationService configurationService )
        {
            this.configurationService = configurationService;
            this.loadListCommand = new DelegateCommand(LoadList);
            this.saveCommand = new DelegateCommand(Save);
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
