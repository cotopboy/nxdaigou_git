using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;
using System.IO;
using Microsoft.Office.Interop.Excel;
using daigou.infrastructure.ExtensionMethods;
using System.Text.RegularExpressions;


namespace daigou.services.EMS
{
    public class EmsExcelBuilder : daigou.services.IDhlWaybillExcelBuilder
    {
        public event Action<float> OnReportStep;

        private ResourceReleaser resourceReleaser;
        private RandomNameService randomNameService;
        protected ProcessModeInfo modeInfo;

        private List<string[]> mapping = new List<string[]>()
            {
                new string[]{"M","N","O","P","Q"},
                new string[]{"R","S","T","U","V"},
                new string[]{"W","X","Y","Z","AA"}
            };

        public EmsExcelBuilder(ProcessModeInfo modeInfo, 
                               ResourceReleaser resourceReleaser,
                               RandomNameService randomNameService
                                
                                )
        {
           this.modeInfo = modeInfo;
           this.resourceReleaser = resourceReleaser;
           this.randomNameService = randomNameService;
        }

        public string Generate(string targetDir, List<DhlWaybillParam> dhlWaybillParamList,string taobaoOrderSn = "")
        {
            string ExcelFile = this.resourceReleaser.ReleaseXls("EMS_Template", targetDir);

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlApp = new Excel.Application();
            int realRowIndex = 3;

            xlWorkBook = xlApp.Workbooks.Open(ExcelFile);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            foreach (var info_xls_Pre in dhlWaybillParamList)
            {
                FillXlWorkSheet(xlWorkSheet, realRowIndex, info_xls_Pre,taobaoOrderSn);

                if (OnReportStep != null)
                {
                    OnReportStep(0.0f);
                }
                realRowIndex++;
            }

            FileInfo fileInfo = new FileInfo(ExcelFile);
            string newfileName = GetFileName(dhlWaybillParamList);

            string newFullFileName = Path.Combine(fileInfo.Directory.FullName, newfileName);
            xlWorkBook.SaveAs(newFullFileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            xlWorkSheet.ReleaseComObject();
            xlWorkBook.ReleaseComObject();
            xlApp.ReleaseComObject();

            File.Delete(ExcelFile);

            return newFullFileName;
        }

        protected virtual string GetFileName(List<DhlWaybillParam> dhlWaybillParamList)
        {
            return string.Format("EMS_{0}_{1}.xls", this.modeInfo.WanwanName, DateTime.Now.ToString("MM.dd"));
        }

        private void FillXlWorkSheet(Excel.Worksheet xlWorkSheet, int realRowIndex, DhlWaybillParam param, string taobaoOrderSn )
        {

            double actualWeight = (param.Order.ActualWeight < 1) ? param.Order.PacketWeight * 0.95 : param.Order.ActualWeight;

            xlWorkSheet.Cells[realRowIndex, ToIndex("A")] = "EMS";
            xlWorkSheet.Cells[realRowIndex, ToIndex("B")] = Math.Round(actualWeight, 1);

            xlWorkSheet.Cells[realRowIndex, ToIndex("C")] = this.modeInfo.WanwanName;

            xlWorkSheet.Cells[realRowIndex, ToIndex("D")] = taobaoOrderSn; 

            xlWorkSheet.Cells[realRowIndex, ToIndex("E")] = "nxdaigou@gmail.com";

            xlWorkSheet.Cells[realRowIndex, ToIndex("F")] = "Ruifeng Zhang";

            xlWorkSheet.Cells[realRowIndex, ToIndex("G")] = "DamaschkeStr.";

            xlWorkSheet.Cells[realRowIndex, ToIndex("H")] = "39";

            xlWorkSheet.Cells[realRowIndex, ToIndex("I")] = "23560";

            xlWorkSheet.Cells[realRowIndex, ToIndex("J")] = "Luebeck";

            xlWorkSheet.Cells[realRowIndex, ToIndex("K")] = "nxdaigou@gmail.com";

            xlWorkSheet.Cells[realRowIndex, ToIndex("L")] = "017672886051";  

            FillItemDetail(xlWorkSheet, realRowIndex, param.Order.Detail);

            xlWorkSheet.Cells[realRowIndex, ToIndex("AB")] = param.Recipient.Name;

            xlWorkSheet.Cells[realRowIndex, ToIndex("AC")] = param.Recipient.MainTel;

            xlWorkSheet.Cells[realRowIndex, ToIndex("AD")] = param.Recipient.PostCode;

            xlWorkSheet.Cells[realRowIndex, ToIndex("AE")] = GetProvince(param.Recipient.ProviceCity);  
            xlWorkSheet.Cells[realRowIndex, ToIndex("AF")] = GetCity(param.Recipient.ProviceCity);  

            xlWorkSheet.Cells[realRowIndex, ToIndex("AG")] = param.Recipient.CnAddress;

        }

        private string GetCity(string input)
        {
            if (input.Contains(",")) return input.SubStringAfter(",", 1);
            else return input;
        }

        private string GetProvince(string input)
        {
            if (input.Contains(",")) return input.SubStringBefore(",");
            else return "";
        }

        private float FillItemDetail(Excel.Worksheet xlWorkSheet, int realRowIndex, string inputDetail)
        {

            ZdBpostDetails zbpostDetails = new ZdBpostDetails();

            var lines = inputDetail.ToLines();

            lines.ForEach(x => zbpostDetails.AcceptNewItem(x));

            for (int i = 0; i < zbpostDetails.Items.Count; i++)
            {
                var item = zbpostDetails.Items[i];
                xlWorkSheet.Cells[realRowIndex, ToIndex(mapping[i][0])] = item.Content; 
                xlWorkSheet.Cells[realRowIndex, ToIndex(mapping[i][1])] = item.IntQuantity;
                xlWorkSheet.Cells[realRowIndex, ToIndex(mapping[i][2])] = (item.FloatNetWeight * item.IntQuantity).ToString("F1");
                xlWorkSheet.Cells[realRowIndex, ToIndex(mapping[i][3])] = (item.FloatNetWeight * item.IntQuantity * 1.1).ToString("F1");
                xlWorkSheet.Cells[realRowIndex, ToIndex(mapping[i][4])] = (item.IntTotalValue).ToString("F1");
            }

            return (float)Math.Round(zbpostDetails.TotalValue, 2);
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
