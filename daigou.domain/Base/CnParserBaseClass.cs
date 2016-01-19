using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain.Base
{
    public class CnRecipientInfo
    {
        public string Remark { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string ProviceCity { get; set; }
        public List<string> TelList { get; set; }
        public string TelListText { get { return string.Join(",", this.TelList); } }
    }

    public class CnRecipientItem
    {
        public CnRecipientItem(string value)
        {
            this.Value = value;
            this.IsProcessed = false;
        }

        public string Value { get; set; }

        public bool IsProcessed { get; set; }
    }
}
