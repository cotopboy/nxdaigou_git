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
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;


namespace daigou.services.EMS
{
    public class EMSWayBillPrinter : IWaybillPrinter
    {
        private DirectoryService directoryService;
        private EMSWaybillAnalyser eMSWaybillAnalyser;

        private static Dictionary<string, string> replaceDict = new Dictionary<string, string>() 
        {
            {"Ap 爱他美 Pre 段 X 6"  ,"ap-6"      },
            {"Ap 爱他美 1   段 X 6"  ,"a1-6"      },
            {"Ap 爱他美 2   段 X 6"  ,"a2-6"      },
            {"Ap 爱他美 3   段 X 6"  ,"a3-6"      },
            {"Ap 爱他美 1+  段 X 8"  ,"a1+-8"     },
            {"Ap 爱他美 2+  段 X 8"  ,"a2+-8"     },
            {"Combi益生菌 Pre 段 X 8","cp-8"      },
            {"Combi益生菌 1   段 X 8","c1-8"      },
            {"Combi益生菌 2   段 X 8","c2-8"      },
            {"Combi益生菌 3   段 X 8","c3-8"      },
            {"Combi益生菌 1+  段 X 8","c1+-8"     },
            {"Combi益生菌 2+  段 X 8","c2+-8"     },
            {"Bio有机 Pre 段 X 8"    ,"bp-8"      },
            {"Bio有机 1   段 X 8"    ,"b1-8"      },
            {"Bio有机 2   段 X 6"    ,"b2-6"      },
            {"Bio有机 3   段 X 6"    ,"b3-6"      },
            {"Bio有机 1+  段 X 6"    ,"b1+-6"     }
        };

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

            foreach (var fileName in recipientPdfFileList)
            {
                this.AddWatermark(fileName, recipient, order);
            }

            string combinedPdfFilename = this.CombinePDFFiles(recipientPdfFileList, recipient, order);


            foreach (var fileName in recipientPdfFileList)
            {
                this.DeleteProcessedFile(fileName);
            }
        }

        private void AddWatermark(string filename, domain.Recipient recipient, domain.Order order)
        {
            string line1 = string.Format(order.ID.ToString() + "收件人:{0}                  电话:{1},{3}              邮编:{2}", recipient.Name, recipient.MainTel, recipient.PostCode, recipient.OtherTels);
           
            const int emSize = 9;
            
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);            
            XFont font = new XFont("Microsoft YaHei", emSize, XFontStyle.Regular, options);            
            XFont handWriteFont = new XFont("YouYuan", 10, XFontStyle.Regular, options);            
            PdfDocument document = CompatiblePdfReader.Open(filename);

            PdfPage page = document.Pages[0];

            
            if (document.Version < 14)
                document.Version = 14;

            XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend);


            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Near;
            format.LineAlignment = XLineAlignment.Near;


            // Create a dimmed red brush
            XBrush brush = new XSolidBrush(XColor.FromArgb(255, 0, 0, 0));
            XBrush HandWriteBrush = new XSolidBrush(XColor.FromArgb(255, 0, 0, 0));

 
            if (!filename.Contains("CZ"))
            {
                gfx.DrawString(line1, font, brush, new XPoint(20, 30), format);
                gfx.DrawString(recipient.CnAddress, font, brush, new XPoint(20, 45), format);
                AddOrderInOneLineWaterMark(order, font, gfx, format, brush);
                AddOrderContentWaterMark(recipient, order, font, gfx, format, brush);                
            }

            document.Save(filename);
        }

        [Conditional("RELEASE")]
        private void DeleteProcessedFile(string fileName)
        {
            File.Delete(fileName);
        }

        private void AddOrderInOneLineWaterMark(domain.Order order, XFont font, XGraphics gfx, XStringFormat format, XBrush brush)
        {
            string content = string.Join(" | ", order.Content.ToLines());

            foreach (var item in replaceDict)
            {
                content = content.Replace(item.Key, item.Value);
            }

            gfx.DrawString(content, font, brush, new XPoint(20, 60), format);
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
