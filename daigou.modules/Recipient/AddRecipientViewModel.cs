using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using daigou.services;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using daigou.infrastructure.Events;
using daigou.domain.Base;
using daigou.domain;

namespace daigou.modules.Recipient
{
    public class AddRecipientViewModel : NotificationObject
    {
        private CnRecipientParser cnRecipientParser;
        private IEventAggregator eventAggregator;
        private ConfigurationService configurationService;

        private string agentQQEmail = "";
        private string agentName = "";

        public string AgentQQEmail
        {
            get { return agentQQEmail; }
            set { agentQQEmail = value; RaisePropertyChanged("AgentQQEmail"); }
        }

        public string AgentName
        {
            get { return agentName; }
            set { agentName = value; RaisePropertyChanged("AgentName"); }
        }

        public AddRecipientViewModel(CnRecipientParser cnRecipientParser, 
                                     IEventAggregator eventAggregator, 
                                     ConfigurationService configurationService
                                    )
        {
            this.addRecipientCommand = new DelegateCommand(AddRecipient);
            this.cleanContentCommand = new DelegateCommand(CleanRawContent);
            this.removeSpaceCommand = new DelegateCommand(RemoveSpace);
            this.addAgentCommand = new DelegateCommand(AddAgent);
            this.cnRecipientParser = cnRecipientParser;
            this.eventAggregator = eventAggregator;
            this.configurationService = configurationService;

            string value = this.configurationService.ConfigDict.GetValue_safe("LatestNewAddedRecipients", "");

            this.AddRecipientRawContent = value;
        }

        private DelegateCommand addRecipientCommand = null;
        private DelegateCommand cleanContentCommand = null;
        private DelegateCommand removeSpaceCommand = null;
        private DelegateCommand addAgentCommand = null;

        public DelegateCommand AddAgentCommand
        {
            get { return addAgentCommand; }
            set { addAgentCommand = value; }
        }

        public DelegateCommand RemoveSpaceCommand
        {
            get { return removeSpaceCommand; }
        }

        private string addRecipientRawContent = string.Empty;

        public string AddRecipientRawContent
        {
            get { return this.addRecipientRawContent; }
            set 
            {
                this.addRecipientRawContent = value;
                RaisePropertyChanged("AddRecipientRawContent");
            }

        }

        public void AddAgent()
        {
            if (!this.AgentName.IsNullOrEmpty() && !this.AgentQQEmail.IsNullOrEmpty())
            {
                this.eventAggregator.GetEvent<AddAgentEvent>().Publish(new AddAgentPayLoad(
                    this.AgentName,
                    this.AgentQQEmail
                    ));
            }

        }

        private void AddRecipient()
        {
            this.configurationService.AddOrUpdate("LatestNewAddedRecipients", this.AddRecipientRawContent);
            string input = this.AddRecipientRawContent;
            if (input.Trim().Length == 0) return;
            var lines = input.ToLines();
            List<string> NotOkLines = new List<string>();

            List<CnRecipientInfo> cnRecipientInfoList = new List<CnRecipientInfo>();

            foreach (var item in lines)
            {
                if (item.Trim().IsNullOrEmpty()) continue;
                try
                {
                    var ret = this.cnRecipientParser.Parse(item);
                    cnRecipientInfoList.Add(ret);
                }
                catch
                {
                    NotOkLines.Add(item.Trim());
                    MessageBox.Show("Error Format ==> {0}".FormatAs(item));
                    continue;
                }
                
            }
            var payLoad = new ParseRawTextRecipientFinishedPayload(cnRecipientInfoList);
            this.eventAggregator.GetEvent<ParseRawTextRecipientFinishedEvent>().Publish(payLoad);

            this.AddRecipientRawContent = string.Join(Environment.NewLine, NotOkLines);

        }

        private void RemoveSpace()
        {
            var lines = this.addRecipientRawContent.ToLines();
            StringBuilder content = new StringBuilder();
            foreach (string line in lines)
            {
                if (line.Trim().IsNullOrEmpty()) continue;

                string afterProcess = line.Replace(" ", "").Replace(","," ").Replace("，"," ");

                if (afterProcess.Contains("【")) 
                  afterProcess = afterProcess.Substring(0, afterProcess.IndexOf("【"));

                  content.AppendLine(afterProcess);

            }

            AddRecipientRawContent = content.ToString();

        }

        private void CleanRawContent()
        {
            this.AddRecipientRawContent = string.Empty;
        }

        public DelegateCommand AddRecipientCommand
        {
            get 
            {
                return this.addRecipientCommand;
            }
        }

        public DelegateCommand CleanContentCommand
        {
            get
            {
                return this.cleanContentCommand;
            }
        }



        
    }
}
