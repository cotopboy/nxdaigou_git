using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;

namespace daigou.services.EMS
{

    public class EmsWaybillInfoExtractor : IWaybillInfoExtractor
    {
        private EMSWaybillAnalyser waybillAnalyser;
        private PdfTextExtractor pdfTextExtractor;
        private DirectoryService directoryService;

        
        public Dictionary<string, string> NameToWaybillSnDict { get; set; }

        public EmsWaybillInfoExtractor(EMSWaybillAnalyser waybillAnalyser,
            PdfTextExtractor pdfTextExtractor,
            DirectoryService directoryService
            )
        {

            this.waybillAnalyser = waybillAnalyser;
            this.pdfTextExtractor = pdfTextExtractor;
            this.directoryService = directoryService;
        }


        public void ExtractWaybillInfo(List<PrintWayBillParam> list)
        {
            
            foreach (var item in list)
            {
                GetBillWaySn(item);
            }

            DirectoryInfo dir = new DirectoryInfo(directoryService.GetOrCreateBaseDir());          
        }

        private void GetBillWaySn(PrintWayBillParam param)
        {

            string emsFileName = this.waybillAnalyser.GetEmsFileName(param.Recipient, param.NameSpecifier);
       
            string[] array = emsFileName.Replace(".pdf","").Split('+');

            param.Order.DhlSn = "{0};{1}".FormatAs(array[2], array[1]);
            
        }

    }


}
