using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using Utilities.DataTypes.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;
using System.IO;
using Utilities.IO.ExtensionMethods;

namespace daigou.services
{
    public class BusinessInvoiceBuilder
    {
        private WaybillSettingFactory waybillSettingFactory;
        private ResourceReleaser resourceReleaser;
        private ZdBpostDetailsProvider zdBpostDetailsProvider;
        private DirectoryService directoryService;

        public BusinessInvoiceBuilder(
            WaybillSettingFactory waybillSettingFactory,
            ZdBpostDetailsProvider zdBpostDetailsProvider,
            DirectoryService directoryService,
            ResourceReleaser resourceReleaser

            )
        {
           this.waybillSettingFactory = waybillSettingFactory;
           this.zdBpostDetailsProvider = zdBpostDetailsProvider;
           this.resourceReleaser = resourceReleaser;
           this.directoryService = directoryService;
        }

        public void BuildInvoice(List<PrintWayBillParam> list, Dictionary<string, string> NameToWaybillSn)
        {
            foreach (var item in list)
            {
                if (item.Order.PacketWeight < 8) continue;

                BuildSingleInvoice(item.Recipient, item.Order, NameToWaybillSn[item.Recipient.Name + item.NameSpecifier]);
            }
        }

        private void BuildSingleInvoice(domain.Recipient recipient,domain.Order order, string BpostSn)
        {
            ZdBpostDetails detail = this.zdBpostDetailsProvider.GetZdBpostDetails(recipient.Name);

            string ExcelFile = this.resourceReleaser.ReleaseXls("Buisness_Invoice_Template", this.directoryService.GetOrCreateBaseDir());

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlApp = new Excel.Application();

            xlWorkBook = xlApp.Workbooks.Open(ExcelFile);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Cells[8, ToIndex("A")] = GetRecipientFormattedAddress(recipient);
            xlWorkSheet.Cells[3, ToIndex("H")] = BpostSn;
            xlWorkSheet.Cells[5, ToIndex("G")] = DateTime.Today.ToString();
            xlWorkSheet.Cells[16, ToIndex("A")] = recipient.Name;
            xlWorkSheet.Cells[27, ToIndex("K")] = order.PacketWeight;

            if (detail != null && detail.Items.Count >= 1)
            {
                xlWorkSheet.Cells[18, ToIndex("A")] = detail.Items[0].Quantity;
                xlWorkSheet.Cells[18, ToIndex("B")] = "pcs";
                xlWorkSheet.Cells[18, ToIndex("C")] = detail.Items[0].Content;
                xlWorkSheet.Cells[18, ToIndex("G")] = "Germany";
                xlWorkSheet.Cells[18, ToIndex("H")] = detail.Items[0].Value;
                xlWorkSheet.Cells[18, ToIndex("K")] = detail.Items[0].IntTotalValue;
            }

            if (detail != null && detail.Items.Count >= 2)
            {
                xlWorkSheet.Cells[19, ToIndex("A")] = detail.Items[1].Quantity;
                xlWorkSheet.Cells[19, ToIndex("B")] = "pcs";
                xlWorkSheet.Cells[19, ToIndex("C")] = detail.Items[1].Content;
                xlWorkSheet.Cells[19, ToIndex("G")] = "Germany";
                xlWorkSheet.Cells[19, ToIndex("H")] = detail.Items[1].Value;
                xlWorkSheet.Cells[19, ToIndex("K")] = detail.Items[1].IntTotalValue;
            }

            if (detail != null && detail.Items.Count >= 3)
            {
                xlWorkSheet.Cells[20, ToIndex("A")] = detail.Items[2].Quantity;
                xlWorkSheet.Cells[20, ToIndex("B")] = "pcs";
                xlWorkSheet.Cells[20, ToIndex("C")] = detail.Items[2].Content;
                xlWorkSheet.Cells[20, ToIndex("G")] = "Germany";
                xlWorkSheet.Cells[20, ToIndex("H")] = detail.Items[2].Value;
                xlWorkSheet.Cells[20, ToIndex("K")] = detail.Items[2].IntTotalValue;
            }

            FileInfo fileInfo = new FileInfo(ExcelFile);
            string newfileName = string.Format("{0}_{1}商业发票.xls",recipient.Name,order.ID);

            string newFullFileName = Path.Combine(fileInfo.Directory.FullName, newfileName);
            xlWorkBook.SaveAs(newFullFileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            File.Delete(ExcelFile);
        }

        private string GetRecipientFormattedAddress(domain.Recipient recipient)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("Name:{0}", recipient.Name.NameToPinyin()));
            builder.AppendLine(string.Format("{0}", recipient.CnAddress.ToPinyin()));
            builder.AppendLine(string.Format("Zipcode {0} City {1}", recipient.PostCode,recipient.ProviceCity.ToPinyin()));
            builder.AppendLine(string.Format("Tel {0}", recipient.MainTel));
            builder.AppendLine("Contact Email*");
            return builder.ToString();

        }

         private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }


        private int ToIndex(string columnName)
        {
            if (!Regex.IsMatch(columnName.ToUpper(), @"[A-Z]+"))
                throw new Exception("invalid parameter");
            int index = 0;
            char[] chars = columnName.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index;
        }
    }
}
