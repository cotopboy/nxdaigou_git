using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using daigou.services.Interfaces;


namespace daigou.services.Bpost
{
    public class BpostPacketDeliveredReportService : IPacketDeliveredReportService
    {

        private EmailService emailService;
        private WaybillQueryManager SearchManager;
        private OrderService orderService;

        public BpostPacketDeliveredReportService(EmailService emailService,WaybillQueryManager SearchManager,OrderService orderService)
        {
            this.orderService = orderService;
            this.SearchManager = SearchManager;
            this.emailService = emailService;
        }

        public ReportResult ReportPacketSentByEmail(DhlWaybillParam param, string additionInfo = "")
        {
            TryToGetEmsBillwaySn(param.Order);

            StringBuilder msg = new StringBuilder();
            msg.AppendLine(additionInfo + "<br/>");
            msg.AppendLine("<br/>");
            msg.AppendLine("");

            List<string> receiverList = new List<string>() { };
            ReportResult ret = new ReportResult();
            ret.RecipientName = param.Recipient.Name;
            ret.IsOK = false;

            if (param.Order.DhlSn.IsNullOrEmpty()) return ret;


            try
            {
                var waybillSnArray = param.Order.DhlSn.Split(';');
                string dhlWaybillSn = (waybillSnArray.Length >= 1) ? waybillSnArray[0] : "";
                string bpostWaybillsn = (waybillSnArray.Length >= 2) ? waybillSnArray[1] : "";
                string emsSn = (waybillSnArray.Length >= 3) ? waybillSnArray[2] : "";


                msg.AppendLine("<h2><font face='微软雅黑'>包裹查询信息</font></h2>");

                msg.AppendLine("<hr>");

                if (!param.Order.EmailRemark.IsNullOrEmpty())
                {
                    msg.AppendLine(param.Order.EmailRemark);
                    msg.AppendLine("<br/>");
                }

                msg.AppendLine(string.Format("<b>订单编号:</b> {0}{1},{2}",
                                              param.Order.ID,
                                              param.Recipient.Name,
                                              param.Order.CreatedTime.ToShortDateString()
                  ));
                msg.AppendLine("<br/>");
                msg.AppendLine(string.Format("<b>货运单号:</b> {0},{1}", dhlWaybillSn.AddSquareBracket(), bpostWaybillsn.AddSquareBracket()));
                msg.AppendLine("<br/>");
                msg.AppendLine("<b>发货详情:</b> " + string.Join("|", param.Order.Content.ToLines().ForEach(x => x.Replace(',', '_'))));
                msg.AppendLine("<br/>");
                msg.AppendLine("<b>报关详情:</b> " + string.Join("|", param.Order.Detail.ToLines().ForEach(x => x.Replace(',', '_'))));
                msg.AppendLine("<br/>");
                msg.AppendLine("<b>收件信息:</b> " + string.Format("{0}  {1}  {2}  {3}  {4}",
                    param.Recipient.Name,
                    param.Recipient.CnAddress,
                    param.Recipient.PostCode,
                    param.Recipient.MainTel,
                    param.Recipient.OtherTels
                    ));
                msg.AppendLine("<br/>");
                msg.AppendLine("<b>包裹重量:</b> " + param.Order.PacketWeight.ToString() + " Kg");
                msg.AppendLine("<br/>");
                string dhlTemplate = "<a href=\"http://nolp.dhl.de/nextt-online-public/set_identcodes.do?lang=en&idc={0}&rfn=&extendedSearch=true\">http://nolp.dhl.de/nextt-online-public/set_identcodes.do?lang=en&idc={0}&rfn=&extendedSearch=true</a>";
                string dhlSearchLink = string.Format("<b>境内查询:</b>" + dhlTemplate, dhlWaybillSn);
                msg.AppendLine(dhlSearchLink);
                msg.AppendLine("<br/>");
                string bpostTempalte = "<a href=\"http://www.bpost2.be/bpi/track_trace/find.php?search=s&lng=en&trackcode={0}\">http://www.bpost2.be/bpi/track_trace/find.php?search=s&lng=en&trackcode={0}</a>";
                string bpostSearchLink = string.Format("<b>国际查询:</b>" + bpostTempalte, bpostWaybillsn);
                msg.AppendLine(bpostSearchLink);
                msg.AppendLine("<br/>");

                msg.AppendLine();
                msg.AppendLine("<br/>");

                if (emsSn.IsNullOrEmpty())
                {
                    msg.AppendLine();
                    msg.AppendLine("待包裹转交给中国EMS之后，国际运单查询里会生成 中国EMS转单号【Local Barcode】");
                    msg.AppendLine("<br/>");
                    msg.AppendLine("可以通过中国EMS网站进行更进一步的查询：http://www.ems.com.cn");
                    msg.AppendLine("<br/>");
                }
                else
                {
                    string baiduLink = "http://www.baidu.com/s?wd={0}".FormatAs(emsSn);
                    msg.AppendLine();
                    msg.AppendLine("<b>中国EMS,转单号:</b> {0}".FormatAs(emsSn));
                    msg.AppendLine("<br/>");
                    msg.AppendLine("<b>通过中国EMS网站进行更进一步的查询：</b> http://www.ems.com.cn");
                    msg.AppendLine("<br/>");
                    msg.AppendLine("<b>通过百度查询：</b>");
                    msg.AppendLine("<a href=\"{0}\">{0}</a>".FormatAs(baiduLink));
                    msg.AppendLine("<br/>");
                    msg.AppendLine("<b>通过EMS全国统一服务电话查询: </b> 11183");
                    msg.AppendLine("<br/>");
                }

                if (!param.Recipient.QQNumber.IsNullOrEmpty())
                {
                    if (Regex.IsMatch(param.Recipient.QQNumber, @"^\d+$"))
                    {
                        string qqEmail = param.Recipient.QQNumber + "@qq.com";
                        receiverList.AddIfUnique(qqEmail);
                    }
                    else
                    {
                        receiverList.AddIfUnique(param.Recipient.QQNumber);
                    }
                }
                else
                {
                    receiverList.Add("ruifeng.zhang@foxmail.com");
                }

            }
            catch { }

            ret.IsOK = true;

            msg.AppendLine(); msg.AppendLine("<br/>"); msg.AppendLine(); msg.AppendLine("<br/>"); msg.AppendLine();

            do
            {
                try
                {
                    this.emailService.SendMail(
                                    string.Format("Nx-daigou Luebeck 包裹已寄出 [ {0} ] ", param.Recipient.Name),
                                    msg.ToString(),
                                    receiverList,
                                    new List<string>() { "2777888@qq.com" },
                                    System.Net.Mail.MailPriority.High,
                                    true);
                }
                catch
                {
                    string errorMsg = "[" + param.Recipient.Name + "]包裹查询邮件 发送失败,是否重试?";
                    if (MessageBox.Show(errorMsg, "重试?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        continue;
                    }
                }

                break;
            } while (true);

            return ret;

        }

        private void TryToGetEmsBillwaySn(Order order)
        {
            
            var array = order.DhlSn.Split(';');

            string dhl = (array.Length >= 1) ? array[0] : "";
            string bpost = (array.Length >= 2) ? array[1] : "";
            string ems = (array.Length >= 3) ? array[2] : "";
            string emsQuery = "";
            if (ems.IsNullOrEmpty())
            {
                emsQuery = this.SearchManager.GetEMSCodeByBpostCode(bpost);
            }

            if (ems.IsNullOrEmpty() && !emsQuery.IsNullOrEmpty())
            {
                order.DhlSn += ";" + emsQuery;
                this.orderService.Save(order);
            }
        }

    }
}
