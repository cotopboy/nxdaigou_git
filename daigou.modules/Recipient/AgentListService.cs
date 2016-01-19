using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using daigou.infrastructure.Events;
using System.Collections.ObjectModel;
using Utilities.Serialization;
using System.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;
using Utilities.IO;


namespace daigou.modules.Recipient
{
    public class AgentListService
    {
        private IEventAggregator eventAggregator;

        private static ObservableCollection<Agent> agentList = null;

        public AgentListService(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.eventAggregator.GetEvent<AddAgentEvent>().Subscribe(OnAgentAdded);

            LoadAgentList();
        }

        private void LoadAgentList()
        {
            if(agentList == null)
            {
                FileInfo fileInfo = new FileInfo(FilePath);
                var bytes = fileInfo.ReadBinary();
                agentList = FormatterMg.XMLDerObjectFromBytes(typeof( ObservableCollection<Agent>), bytes) as  ObservableCollection<Agent>;
            }


        }

        public ObservableCollection<Agent> AgentList
        {
            get { return agentList; }
        }

        public string FilePath
        {
            get { return DirectoryHelper.CombineWithCurrentExeDir("AgentList.xml");}
        }
       
        public void OnAgentAdded(AddAgentPayLoad payload)
        {
            agentList.Add(new Agent() { Name = payload.AgentName, QQNumberOrEmail = payload.AgentQQOrEmail });

            string x = FormatterMg.XMLSerObjectToString(agentList);

            FileInfo xf = new FileInfo(FilePath);

            xf.Save(x, Encoding.UTF8);

        }










    }
}
