using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.infrastructure.ExtensionMethods;

namespace daigou.services.EMS
{

    public class EMSApplyEmailBuilder : daigou.services.IDhlWaybillEmailBuilder
    {
        protected ProcessModeInfo processModeInfo;

        public EMSApplyEmailBuilder(ProcessModeInfo modeInfo)
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

            builder.AppendLine("淘宝订单编号：" + taoBaoOrderSn);

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


            builder.AppendLine("[祝生意兴隆，合家美满]");


            return builder.ToString();
        }

        public string GetEmailHeader(List<DhlWaybillParam> dhlWaybillParamList, string taoBaoOrderSn = "")
        {           
            return string.Format("ems + {0} + {1}", WanwanName, DateTime.Now.ToString("MM.dd"));

        }

    }
}
