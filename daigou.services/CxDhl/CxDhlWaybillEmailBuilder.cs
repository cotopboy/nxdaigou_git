using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;

namespace daigou.services
{
    public class CxDhlWaybillEmailBuilder : daigou.services.IDhlWaybillEmailBuilder
    {
        private WaybillSettingFactory waybillSettingFactory;

        private string receiverEmail = string.Empty;


        public string ReceiverEmail
        {
            get
            {
                var waybillSetting = this.waybillSettingFactory.Create();
                return waybillSetting.CxEmail;
            }
        }

        public CxDhlWaybillEmailBuilder(WaybillSettingFactory waybillSettingFactory)
        {
            this.waybillSettingFactory = waybillSettingFactory;

        }


        public string GetEmailHeader(List<DhlWaybillParam> dhlWaybillParamList,string taoBaoOrderSn = "")
        {
            var waybillSetting = this.waybillSettingFactory.Create();

            Dictionary<uint, int> dict = new Dictionary<uint, int>();

            foreach (var item in dhlWaybillParamList)
            {
                if (dict.ContainsKey(item.Order.PacketWeight))
                {
                    dict[item.Order.PacketWeight]++;
                }
                else
                {
                    dict.Add(item.Order.PacketWeight, 1);
                }
            }

            List<string> list = new List<string>();

            foreach (var dictItem in dict)
            {
                list.Add(string.Format("{0}kg×{1}", dictItem.Key, dictItem.Value));
            }

            string dynanicName = string.Join(" ", list);
            List<string> NameList = dhlWaybillParamList.Select(x => x.Recipient.Name).ToList();
            string NameListTxt = string.Join(",", NameList);

            return string.Format("{0} {1} {2} 单 [{3}] [4]", waybillSetting.WanwanName, DateTime.Now.ToString("MM.dd"), dynanicName, NameListTxt, taoBaoOrderSn);
        }

        public string GetEmailBody(List<DhlWaybillParam> dhlWaybillParamList, string taoBaoOrderSn = "")
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("淘宝订单编号 : [ " + taoBaoOrderSn + " ]");

            builder.AppendLine("");
            builder.AppendLine("");

            foreach (var item in dhlWaybillParamList)
            {
                builder.AppendLine(
                                    String.Format("{0} {1} {2}KG 包裹单",
                                    item.Recipient.Name, 
                                    item.Recipient.Name.NameToPinyin(), 
                                    item.Order.PacketWeight
                                        )
                                    );
            }

            builder.AppendLine("");
            builder.AppendLine("");
            builder.AppendLine("");
            builder.AppendLine("");
            builder.AppendLine("");

            foreach (var item in dhlWaybillParamList)
            {
                builder.AppendLine(
                                     String.Format("{0}==>{1}[{2}]",
                                     item.Recipient.ProviceCity,
                                     item.Recipient.Name,
                                     item.Recipient.Name.NameToPinyin()
                                      )
                                     );   
            }

            return builder.ToString();
        }

    }
}
