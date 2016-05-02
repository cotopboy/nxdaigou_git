using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;
using PdfSharp.Pdf.Printing;


namespace daigou.services.EMS
{
    public class EMSWayBillPrinter : IWaybillPrinter
    {
        private DirectoryService directoryService;
        private EMSWaybillAnalyser eMSWaybillAnalyser;

        public EMSWayBillPrinter(
                DirectoryService directoryService,
                EMSWaybillAnalyser eMSWaybillAnalyser           
            )
        {
            this.eMSWaybillAnalyser = eMSWaybillAnalyser;
            this.directoryService = directoryService;
           
        }

        public void PrintWayBill(List<PrintWayBillParam> list, IWaybillInfoExtractor CreateInfoExtractor)
        {


            foreach (var item in list)
            {
                PrintRecipientWayBill(item);
            }

          
        }

        private void PrintRecipientWayBill(PrintWayBillParam param)
        {

            domain.Recipient recipient = param.Recipient;
            domain.Order order = param.Order;


            List<string> recipientPdfFileList = this.eMSWaybillAnalyser.GetRecipientPdfFileList(recipient, param.NameSpecifier);

            string combinedPdfFilename = this.CombinePDFFiles(recipientPdfFileList, recipient, order);


            foreach (var fileName in recipientPdfFileList)
            {
                this.DeleteProcessedFile(fileName);
            }
        }

        [Conditional("RELEASE")]
        private void DeleteProcessedFile(string fileName)
        {
            File.Delete(fileName);
        }

        private string CombinePDFFiles(List<string> recipientPdfFileList, domain.Recipient recipient, domain.Order order)
        {
            if (recipientPdfFileList.Count == 0) return string.Empty;

            string emsFile = recipientPdfFileList.FirstOrDefault(x => x.Contains("+"));
            string dhlFile = recipientPdfFileList.FirstOrDefault(x => !x.Contains("+"));
            PdfDocument outputDocument = new PdfDocument();
            List<XPdfForm> xpdList = new List<XPdfForm>();
            XGraphics gfx;

            foreach (var filePath in recipientPdfFileList)
            {
                XPdfForm form = XPdfForm.FromFile(filePath);

                for (int i = 0; i < form.PageCount; i++)
                {
                    PdfPage page = outputDocument.AddPage();
                    // Get a graphics object for page
                    gfx = XGraphics.FromPdfPage(page);
                    form.PageIndex = i;
                    // Draw the page identified by the page number like an image
                    gfx.DrawImage(form, new XRect(0, 0, form.PointWidth, form.PointHeight));
                }
               
              
            }

            // Save the document...
            string filename = recipient.Name + order.ID.ToString() + "_" + order.LogisticsType + "_运单.pdf";
            string outputDocFullname = Path.Combine(this.directoryService.GetOrCreateBaseDir(), filename);
            outputDocument.Save(outputDocFullname);


            return outputDocFullname;
        }
    }
}
