using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.modules.Recipient
{
    [Serializable]
    public class Agent
    {
        public string QQNumberOrEmail { get; set; }

        public string Name { get; set; }
    }
}
