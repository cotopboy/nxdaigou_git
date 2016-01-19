using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.services
{
    public interface IWaybillInfoExtractor
    {
        void ExtractWaybillInfo(List<PrintWayBillParam> list);

        Dictionary<string, string> NameToWaybillSnDict { get; set; }
    }
}
