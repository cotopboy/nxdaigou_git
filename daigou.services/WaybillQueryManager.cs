using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;

namespace daigou.services
{
    public class WaybillQueryManager
    {
        private EMSLocalBarcodeService eMSLocalBarcodeService;
        private DirectoryService DirectoryService;
        private OrderService orderService;

        public WaybillQueryManager(EMSLocalBarcodeService eMSLocalBarcodeService,
            DirectoryService DirectoryService,
               OrderService orderService
            )
        {
            this.orderService = orderService;
            this.DirectoryService = DirectoryService;
            this.eMSLocalBarcodeService = eMSLocalBarcodeService;
        }


        
        public string GetEMSCodeByBpostCode(string bpostCode)
        {
            return eMSLocalBarcodeService.GetEMSLocalBarcode(bpostCode);
        }

        public void SeperateCode(string compositedSn, out string dhlCode, out string bpostCode, out string emsCode)
        {
            dhlCode = bpostCode = emsCode = string.Empty;

            var array = compositedSn.Split(';');

            if (array.Length >= 1) dhlCode = array[0];
            if (array.Length >= 2) bpostCode = array[1];
            if (array.Length >= 3) emsCode = array[2]; 

        
        
        }

        public string GetQueryMsgComplete(PrintWayBillParam param, string dhlWaybillSn, string bpostWaybillsn,string emsCode = "")
        {
            StringBuilder msg = new StringBuilder(100);

            msg.AppendLine("==========包裹追踪===========");
            msg.AppendLine(param.Recipient.ToString());
            msg.AppendLine(string.Join(";",param.Order.Content.ToLines()));
            msg.AppendLine();
            msg.AppendLine("德国境内运单号：{0}".FormatAs(dhlWaybillSn));
            string dhlTemplate = "http://nolp.dhl.de/nextt-online-public/set_identcodes.do?lang=en&idc={0}&rfn=&extendedSearch=true";
            string dhlSearchLink = string.Format("查询链接   " + dhlTemplate, dhlWaybillSn);
            msg.AppendLine(dhlSearchLink);

            msg.AppendLine();
            msg.AppendLine("国际运单号：{0}".FormatAs(bpostWaybillsn));
            string bpostTempalte = "http://www.bpost2.be/bpi/track_trace/find.php?search=s&lng=en&trackcode={0}";
            string bpostSearchLink = string.Format("查询链接  " + bpostTempalte, bpostWaybillsn);
            msg.AppendLine(bpostSearchLink);

            string queryEms;

            queryEms = (!emsCode.IsNullOrEmpty()) ? emsCode : this.GetEMSCodeByBpostCode(bpostWaybillsn);

            if (emsCode.IsNullOrEmpty() && !queryEms.IsNullOrEmpty())
            {
                param.Order.DhlSn += ";" + queryEms;

                this.orderService.Save(param.Order);
            }

            if (queryEms.IsNullOrEmpty())
            {              
                msg.AppendLine();
                msg.AppendLine("待包裹转交给中国EMS之后，国际运单查询里会生成 中国EMS转单号【Local Barcode】");
                msg.AppendLine("可以通过中国EMS网站进行更进一步的查询：http://www.ems.com.cn");
            }
            else
            {
                msg.AppendLine();
                msg.AppendLine("中国EMS,转单号: {0}".FormatAs(queryEms));
                msg.AppendLine("可以通过中国EMS网站进行更进一步的查询：http://www.ems.com.cn");
            }

            msg.AppendLine();
            msg.AppendLine("本包裹于发自德国吕贝克");

            msg.AppendLine("==============================");

            return msg.ToString();
        }

    }
}
