using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;

namespace daigou.services
{
    public interface IWaybillPrinter
    {

        void PrintWayBill(List<PrintWayBillParam> recipient, IWaybillInfoExtractor CreateInfoExtractor);
    }

    public class PrintWayBillParam
    {
        public Recipient Recipient { get; set; }
        public Order Order { get; set; }
        public string NameSpecifier { get; set; }
    }
}
