using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace daigou.services
{
    public class DhlWaybillService
    {
        private EmailService emailService;
        private IDhlWaybillEmailBuilderFactory dhlWaybillEmailBuilderFactory;
        private ResourceReleaser resourceRelease;
        private DirectoryService directoryService;
        private IDhlWaybillExcelBuilderFactory dhlWaybillExcelBuilderFactory;
        private WaybillSettingFactory waybillSettingFactory;
        private IWaybillPrinterFactory waybillPrinterFactory;
        private IWaybillInfoExtractorFactory waybillInfoExtractorFactory;
        private BusinessInvoiceBuilder businessInvoiceBuilder;

        public DhlWaybillService(EmailService emailService,
            IDhlWaybillEmailBuilderFactory dhlWaybillEmailBuilderFactory,
            IDhlWaybillExcelBuilderFactory dhlWaybillExcelBuilderFactory,
            IWaybillPrinterFactory waybillPrinterFactory,
            ResourceReleaser resourceRelease,
            DirectoryService directoryService,
            WaybillSettingFactory waybillSettingFactory,
            BusinessInvoiceBuilder businessInvoiceBuilder,
            IWaybillInfoExtractorFactory waybillInfoExtractorFactory
            )
        {
            this.businessInvoiceBuilder = businessInvoiceBuilder;
            this.emailService = emailService;
            this.dhlWaybillEmailBuilderFactory = dhlWaybillEmailBuilderFactory;
            this.resourceRelease = resourceRelease;
            this.directoryService = directoryService;
            this.dhlWaybillExcelBuilderFactory = dhlWaybillExcelBuilderFactory;
            this.waybillSettingFactory = waybillSettingFactory;
            this.waybillPrinterFactory = waybillPrinterFactory;
            this.waybillInfoExtractorFactory = waybillInfoExtractorFactory;
        }

        public void Process(List<DhlWaybillParam> dhlWaybillParamList, ProcessModeInfo processMode, string enableSendEmail,string taobaoOrderSn)
        {
            if (dhlWaybillParamList.Count == 0)
            {
                return;
            }

            var setting = this.waybillSettingFactory.Create();
            List<string> nameList = dhlWaybillParamList.Select(x => x.Recipient.Name).ToList();
            string targetDir = this.directoryService.CreateDhlWaybillFolder(nameList);
            var excelBuilder = this.dhlWaybillExcelBuilderFactory.CreateBuilder(processMode);

            string xlsFullPath = excelBuilder.Generate(targetDir, dhlWaybillParamList);


            if (enableSendEmail == "true")
            {
                var emailBuilder = this.dhlWaybillEmailBuilderFactory.CreateBuilder(processMode);
                string emailHeader = emailBuilder.GetEmailHeader(dhlWaybillParamList,taobaoOrderSn);
                string emailBody = emailBuilder.GetEmailBody(dhlWaybillParamList, taobaoOrderSn);

                List<string> recipientList = new List<string>() { emailBuilder.ReceiverEmail };
                List<string> ccList = new List<string>();
                ccList.AddRange(setting.CcEmail.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));


                this.emailService.SendMail(emailHeader, emailBody, recipientList, ccList, System.Net.Mail.MailPriority.High, xlsFullPath);
            }
            else
            {
                FileInfo file = new FileInfo(xlsFullPath);
                System.Diagnostics.Process.Start(file.Directory.FullName);                
            }

        }


        public void PrintProcessWaybill(List<PrintWayBillParam> list, string AgentType)
        {
            var extractor = this.waybillInfoExtractorFactory.CreateInfoExtractor(AgentType);
            extractor.ExtractWaybillInfo(list);

            

            Dictionary<string, string> NameToWaybillSn = extractor.NameToWaybillSnDict;


            var printer = this.waybillPrinterFactory.CreatePrinter(AgentType);
            printer.PrintWayBill(list, extractor);

            
            this.businessInvoiceBuilder.BuildInvoice(list, NameToWaybillSn);



        }
    }

    public class DhlWaybillParam
    {
        public domain.Order Order{get;set;}
        public domain.Recipient Recipient{get;set;}
    }
}
