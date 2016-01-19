using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace daigou.infrastructure.Events
{
    public class AddAgentEvent : CompositePresentationEvent<AddAgentPayLoad>
    {

    }

    public class AddAgentPayLoad
    {
        public AddAgentPayLoad(string agentName,string agentQQorEmail)
        {
            this.AgentName = agentName;
            this.AgentQQOrEmail = agentQQorEmail;
        }

        public string AgentName { get; set; }
        public string AgentQQOrEmail { get; set; }
    }
}
