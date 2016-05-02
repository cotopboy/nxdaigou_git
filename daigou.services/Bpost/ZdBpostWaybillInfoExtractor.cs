using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;


namespace daigou.services
{
    public class ZdBpostWaybillInfoExtractor : IWaybillInfoExtractor
    {
        private ZdBpostWaybillAnalyser zdBpostWaybillAnalyser;
        private PdfTextExtractor pdfTextExtractor;        
        private DirectoryService directoryService;
        private BpostAddressLineCounter bpostAddressLineCounter;

        private Dictionary<string, string> nameToWaybillSnDict = null;

        public Dictionary<string, string> NameToWaybillSnDict
        {
            get
            {
                return this.nameToWaybillSnDict;
            }
            set {
                this.nameToWaybillSnDict = value;
            }
 
        }

        public ZdBpostWaybillInfoExtractor(ZdBpostWaybillAnalyser zdBpostWaybillAnalyser, 
            PdfTextExtractor pdfTextExtractor,
            BpostAddressLineCounter bpostAddressLineCounter,
            DirectoryService directoryService
            )
        {
            this.bpostAddressLineCounter = bpostAddressLineCounter;
            this.zdBpostWaybillAnalyser = zdBpostWaybillAnalyser;
            this.pdfTextExtractor = pdfTextExtractor;
            this.directoryService = directoryService;
        }


        public void ExtractWaybillInfo(List<PrintWayBillParam> list)
        {
            nameToWaybillSnDict = new Dictionary<string, string>();
            foreach (var item in list)
            {
                ExtractWaybillInfo(item);
            }

            DirectoryInfo dir = new DirectoryInfo(directoryService.GetOrCreateBaseDir());
            foreach (var item in dir.GetFiles())
            {
                if (item.Extension == ".txt")
                {
                    File.Delete(item.FullName);
                }
            }
        }

        private void ExtractWaybillInfo(PrintWayBillParam param)
        {

            string dhlFileName = this.zdBpostWaybillAnalyser.GetDHLFile(param.Recipient,(uint)param.NameSpecifier.StrToInt());

            string bpostFileName = this.zdBpostWaybillAnalyser.GetBpostFile(param.Recipient, (uint)param.NameSpecifier.StrToInt());

            string bpostCN23FileName = this.zdBpostWaybillAnalyser.GetBpostCN23File(param.Recipient, (uint)param.NameSpecifier.StrToInt());

            string dhlContent = this.pdfTextExtractor.GetTextByCommandLine(dhlFileName);

            string bpostContent = this.pdfTextExtractor.GetTextByCommandLine(bpostFileName);
            string bpostCN23Content = this.pdfTextExtractor.GetTextByCommandLine(bpostCN23FileName);

            string dhlWaybillSn = RexHelper.ExtractDHLWaybillSn(dhlContent);
            string bpostWaybillsn = RexHelper.ExtractZdBpostWaybillSn(bpostContent);

  
            param.Order.DhlSn = "{0};{1}".FormatAs(dhlWaybillSn, bpostWaybillsn);

            nameToWaybillSnDict.Add(param.Recipient.Name + param.NameSpecifier, bpostWaybillsn);

        }

    }

    public class ZdBpostAddressLineInfo
    {
        public int BpostAddressLineCount { get; set; }

        public int BpostCn23AddressLineCount { get; set; }
    }
}
