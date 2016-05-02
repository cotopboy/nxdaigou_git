using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;


namespace daigou.services
{
    public class ZdBpostWaybillEmailBuilder : daigou.services.IDhlWaybillEmailBuilder
    {
        protected ProcessModeInfo processModeInfo;

        public ZdBpostWaybillEmailBuilder(ProcessModeInfo modeInfo)
        {
            this.processModeInfo = modeInfo;
            
        }

        public virtual string ReceiverEmail
        {
            get
            {
                return processModeInfo.PacketExcelEmail;
            }
        }

        protected virtual string Remark
        {
            get { return this.processModeInfo.EmailRemark; }
        }

        protected virtual string WanwanName
        {
            get { return processModeInfo.WanwanName; }
        }

        public virtual string GetEmailBody(List<DhlWaybillParam> dhlWaybillParamList, string taoBaoOrderSn = "")
        {
            StringBuilder builder = new StringBuilder();

            
            builder.AppendLine("");
            builder.AppendLine("");
            builder.AppendLine("淘宝订单编号：" + taoBaoOrderSn);
            builder.AppendLine("");
            builder.AppendLine("");
            builder.AppendLine(Remark);

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

            
            builder.AppendLine("[祝生意兴隆，合家美满]");


            return builder.ToString();
        }

        public string GetEmailHeader(List<DhlWaybillParam> dhlWaybillParamList, string taoBaoOrderSn = "")
        {

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

            return string.Format("[{0}]的BPOST单 {1} {2} 单 [{3}] [{4}]", WanwanName, DateTime.Now.ToString("MM.dd"), dynanicName, NameListTxt,taoBaoOrderSn);
       
        }

    }

    public class ZdBpostWaybillEmailBuilderToClient : ZdBpostWaybillEmailBuilder
    {
        public ZdBpostWaybillEmailBuilderToClient(ProcessModeInfo modeInfo) : base( modeInfo)
        {

        }

        public override string GetEmailBody(List<DhlWaybillParam> dhlWaybillParamList, string taoBaoOrderSn = "")
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("");
            builder.AppendLine(Remark);
            builder.AppendLine("");
            builder.AppendLine("");


            builder.AppendLine("===================================================");

            foreach (var item in dhlWaybillParamList)
            {
                builder.AppendLine(
                                    String.Format("{0} {1} {2}KG 包裹单",
                                    item.Recipient.Name,
                                    item.Recipient.Name.NameToPinyin(),
                                    item.Order.PacketWeight
                                        )
                                    );

                builder.AppendLine("订单内容：");
                builder.AppendLine(item.Order.Content);

                builder.AppendLine("{0}   {1}   {2}   {3}   {4}".FormatAs(
                    item.Recipient.Name,
                    item.Recipient.CnAddress,
                    item.Recipient.PostCode,
                    item.Recipient.MainTel,
                    item.Recipient.OtherTels));

                builder.AppendLine("");
            }



            return builder.ToString();
        }
    }

   
}
