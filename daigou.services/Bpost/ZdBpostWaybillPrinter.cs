using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;
using PdfSharp.Pdf.Printing;

namespace daigou.services
{
    public class ZdBpostWaybillPrinter : IWaybillPrinter
    {

        private ZdBpostWaybillAnalyser zdBpostWaybillAnalyser;
        private DirectoryService directoryService;
        private ZdBpostDetailsProvider zdBpostDetailsProvider;
        private ZdBpostWaybillInfoExtractor zdBpostWaybillInfoExtractor;
        private Random rd = new Random(DateTime.Now.Millisecond);
        private JokeProvide jokeProvide;

        public ZdBpostWaybillPrinter(ZdBpostWaybillAnalyser zdBpostWaybillAnalyser
            , DirectoryService directoryService,
            ZdBpostDetailsProvider zdBpostDetailsProvider,
            JokeProvide jokeProvide
            )
        {
            this.jokeProvide = jokeProvide;
            this.zdBpostWaybillAnalyser = zdBpostWaybillAnalyser;
            this.directoryService = directoryService;
            this.zdBpostDetailsProvider = zdBpostDetailsProvider;
        }

        public void PrintWayBill(List<PrintWayBillParam> list, IWaybillInfoExtractor CreateInfoExtractor)
        {
            this.zdBpostWaybillInfoExtractor = CreateInfoExtractor as ZdBpostWaybillInfoExtractor;

            foreach (var item in list)
            {
                PrintRecipientWayBill(item);
            }

        }

        private void PrintRecipientWayBill(PrintWayBillParam param)
        {

            domain.Recipient recipient = param.Recipient;
            domain.Order order = param.Order;

            List<string> recipientPdfFileList = this.zdBpostWaybillAnalyser.GetRecipientPdfFileList(recipient,(uint)param.NameSpecifier.StrToInt());
            
            foreach (var fileName in recipientPdfFileList)
            {
                this.AddWatermark(fileName, recipient,order);
            }

            string combinedPdfFilename = this.CombinePDFFiles(recipientPdfFileList, recipient, order);


            foreach (var fileName in recipientPdfFileList)
            {
                this.DeleteProcessedFile(fileName);
            }
        }

        private string CombinePDFFiles(List<string> recipientPdfFileList, domain.Recipient recipient,domain.Order order)
        {
            if (recipientPdfFileList.Count == 0) return string.Empty;

            PdfDocument outputDocument = new PdfDocument();
            List<XPdfForm> xpdList = new List<XPdfForm>();
            XGraphics gfx;

            int count = recipientPdfFileList.Count;
            foreach (var filePath in recipientPdfFileList)
            {
                XPdfForm form = XPdfForm.FromFile(filePath);
                PdfPage page = outputDocument.AddPage();

                // Get a graphics object for page
                gfx = XGraphics.FromPdfPage(page);

                // Draw the page identified by the page number like an image
                gfx.DrawImage(form, new XRect(0, -20, form.PointWidth, form.PointHeight));
            }

            // Save the document...
            string filename = recipient.Name + order.ID.ToString() + "_BPOST_运单.pdf";
            string outputDocFullname = Path.Combine(this.directoryService.GetOrCreateBaseDir(), filename);
            outputDocument.Save(outputDocFullname);

            //this.PrintPDF(outputDocFullname);

            return outputDocFullname;
        }

        [Conditional("RELEASE")]
        private void DeleteProcessedFile(string fileName)
        {
            File.Delete(fileName);
        }

        

        private void AddWatermark(string filename, domain.Recipient recipient, domain.Order order)
        {
            string line1 = string.Format(order.ID.ToString() + "收件人:{0}                  电话:{1},{3}              邮编:{2}", recipient.Name, recipient.MainTel, recipient.PostCode,recipient.OtherTels);
            string line2 = string.Format("地址:{0}", recipient.CnAddress);

            string greetingLineCn  = "亲爱的海关工作人员，国际包裹工作人员 还有 快递师傅们，您们辛苦了！";
            string greetingLineCn2 = "恳请您在第一时间帮我们顺利派送。";
            string greetingLineCn3 = "万分感谢了~！ + (谢谢！) X (无限次)";
            string greetingLineCn4 = "德国吕贝克 RF,Zhang";

            const int emSize = 9;
            // Set font encoding to unicode
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            // Create the font for drawing the watermark
            XFont font = new XFont("Microsoft YaHei", emSize, XFontStyle.Regular, options);
            //XFont handWriteFont = new XFont("Segoe Script", 13, XFontStyle.Regular, options);//
            XFont handWriteFont = new XFont("YouYuan", 10, XFontStyle.Regular, options);//

            // Open an existing document for editing and loop through its pages
            PdfDocument document = CompatiblePdfReader.Open(filename);

            PdfPage page = document.Pages[0];

            // Set version to PDF 1.4 (Acrobat 5) because we use transparency.
            if (document.Version < 14)
                document.Version = 14;
            // Get an XGraphics object for drawing beneath the existing content
            XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend);

            // Create a string format
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Near;
            format.LineAlignment = XLineAlignment.Near;


            // Create a dimmed red brush
            XBrush brush = new XSolidBrush(XColor.FromArgb(255, 0, 0, 0));
            XBrush HandWriteBrush = new XSolidBrush(XColor.FromArgb(255, 0, 0, 0));
 
            if (IsBpostWaybillwithBarCode(filename))
            {
                gfx.DrawString(line1, font, brush, new XPoint(10, 420), format);
                gfx.DrawString(line2, font, brush, new XPoint(10, 440), format);


                gfx.DrawString(greetingLineCn, handWriteFont, brush, new XPoint(10, 490), format);
                gfx.DrawString(greetingLineCn2, handWriteFont, brush, new XPoint(10, 510), format);
                gfx.DrawString(greetingLineCn3, handWriteFont, brush, new XPoint(10, 530), format);
                gfx.DrawString(greetingLineCn4, handWriteFont, brush, new XPoint(10, 550), format);

                string[] jokeList = jokeProvide.GetMeFormattedJoke();

                for (int i = 0; i < jokeList.Length; i++)
                {
                      gfx.DrawString(jokeList[i], handWriteFont, brush, new XPoint(10, 580+i*20), format);
                }

                gfx.DrawString(line1, font, brush, new XPoint(10, 770), format);
                gfx.DrawString(line2, font, brush, new XPoint(10, 790), format);

            }

            if (IsBpostWayCn23bill(filename))
            {
                gfx.DrawString(line1, font, brush, new XPoint(10, 23), format);
            }

            if (IsDhlWaybill(filename))
            {
                gfx.DrawString(line1, font, brush, new XPoint(20, 30), format);
                AddOrderInOneLineWaterMark(order, font, gfx, format, brush);
                AddOrderContentWaterMark(recipient, order, font, gfx, format, brush);
                gfx.DrawString(order.ID.ToString() + "000" + DateTime.Now.ToString("u"),font, brush, new XPoint(20, 60), format);
            }

            document.Save(filename);
        }

        private void AddOrderInOneLineWaterMark(domain.Order order, XFont font, XGraphics gfx, XStringFormat format, XBrush brush)
        {
            string content = string.Join(" | ", order.Content.ToLines());
            gfx.DrawString(content, font, brush, new XPoint(20, 45), format);
        }

        private void AddOrderContentWaterMark(domain.Recipient recipient, domain.Order order, XFont font, XGraphics gfx, XStringFormat format, XBrush brush)
        {
            int x = 435;
            int ybase = 425;
            int y = ybase;

            var lines = order.Content.ToLines();

            foreach (var line in lines)
            {
                gfx.DrawString(line, font, brush, new XPoint(x, y), format);
                y += 12;
            }
        }

        private static bool IsDhlWaybill(string filename)
        {
            return filename.Contains("0");
        }

        private static bool IsBpostWaybillwithBarCode(string filename)
        {
            return filename.Contains("1") ;
        }

        private static bool IsBpostWayCn23bill(string filename)
        {
            return filename.Contains("2");
        }


        private bool isBpost = false;
       

        private int Xadjust
        {
            get { return  rd.Next(-10,10) * 0 - 350;}
        }

        private int GetYAdjustValue(string fileName, ZdBpostAddressLineInfo lineinfo)
        {
            bool isBpost = fileName.Contains("1") && !fileName.Contains("0");

            int adjustValue = 0;

            int offset = 100 + 260 - 14 + 57 ; 


            adjustValue = offset + 16;
          

            return adjustValue;
        }

       
    }

}
