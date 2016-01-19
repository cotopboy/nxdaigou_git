using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using Utilities.DataTypes.ExtensionMethods;
using System.IO;
using daigou.infrastructure.ExtensionMethods;

namespace daigou.services
{
    public class CxDhlWaybillExcelBuilder : daigou.services.IDhlWaybillExcelBuilder
    {
        public event Action<float> OnReportStep;

        private WaybillSettingFactory waybillSettingFactory;
        private ResourceReleaser resourceReleaser;

        public CxDhlWaybillExcelBuilder(WaybillSettingFactory waybillSettingFactory,ResourceReleaser resourceReleaser)
        {
           this.waybillSettingFactory = waybillSettingFactory;
           this.resourceReleaser = resourceReleaser;
        }


        public string Generate( string targetDir,List<DhlWaybillParam> dhlWaybillParamList)
        {
            string ExcelFile = this.resourceReleaser.ReleaseXls("CX_Template", targetDir);

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlApp = new Excel.Application();
            int realRowIndex = 7;

            xlWorkBook = xlApp.Workbooks.Open(ExcelFile);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            foreach (var info_xls_Pre in dhlWaybillParamList)
            {
                FillXlWorkSheet(xlWorkSheet, realRowIndex, info_xls_Pre);

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

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            File.Delete(ExcelFile);

            return newFullFileName;

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

        private string GetFileName(List<DhlWaybillParam> dhlWaybillParamList)
        {
            var waybillSetting = this.waybillSettingFactory.Create();
            Dictionary<uint, int> dict = new Dictionary<uint, int>();

            foreach (var item in dhlWaybillParamList)
            {
                if (dict.ContainsKey(item.Order.PacketWeight))
                {
                    dict[item.Order.PacketWeight]++;
                }
                else
                {
                    dict.Add(item.Order.PacketWeight, 1);
                }
            }

            List<string> list = new List<string>();

            foreach (var dictItem in dict)
            {
                list.Add(string.Format("{0}kg×{1}", dictItem.Key, dictItem.Value));
            }

            string dynanicName = string.Join(" ", list);

            return string.Format("DHL包裹单 {0} {1}.xls",waybillSetting.WanwanName , dynanicName);
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

        private void FillXlWorkSheet(Excel.Worksheet xlWorkSheet, int realRowIndex, DhlWaybillParam param)
        {
            var waybillSetting = this.waybillSettingFactory.Create();

            xlWorkSheet.Cells[realRowIndex, ToIndex("A")] = realRowIndex - 6;
            xlWorkSheet.Cells[realRowIndex, ToIndex("B")] = waybillSetting.WanwanName;
            xlWorkSheet.Cells[realRowIndex, ToIndex("C")] = waybillSetting.WaybillEmail;
            xlWorkSheet.Cells[realRowIndex, ToIndex("D")] = waybillSetting.SenderName;
            xlWorkSheet.Cells[realRowIndex, ToIndex("E")] = waybillSetting.SenderStreet;
            xlWorkSheet.Cells[realRowIndex, ToIndex("F")] = waybillSetting.SenderHouseNumber;
            xlWorkSheet.Cells[realRowIndex, ToIndex("G")] = waybillSetting.SenderPostCode;
            xlWorkSheet.Cells[realRowIndex, ToIndex("H")] = waybillSetting.SenderCity;
            xlWorkSheet.Cells[realRowIndex, ToIndex("I")] = waybillSetting.SenderCountry;
            xlWorkSheet.Cells[realRowIndex, ToIndex("J")] = waybillSetting.SenderTel;
            xlWorkSheet.Cells[realRowIndex, ToIndex("M")] = "China";
            xlWorkSheet.Cells[realRowIndex, ToIndex("U")] = "Private Present";


            xlWorkSheet.Cells[realRowIndex, ToIndex("L")] = param.Recipient.Name.NameToPinyin();
            xlWorkSheet.Cells[realRowIndex, ToIndex("O")] = param.Recipient.ProviceCity.ToPinyin();


            xlWorkSheet.Cells[realRowIndex, ToIndex("N")] = param.Recipient.PostCode.Trim();
            xlWorkSheet.Cells[realRowIndex, ToIndex("S")] = string.Format("0086-{0}", param.Recipient.MainTel).Trim();
            xlWorkSheet.Cells[realRowIndex, ToIndex("V")] = param.Order.PacketWeight;
            xlWorkSheet.Cells[realRowIndex, ToIndex("W")] = 100;
            xlWorkSheet.Cells[realRowIndex, ToIndex("Y")] = param.Recipient.Name;
            xlWorkSheet.Cells[realRowIndex, ToIndex("Z")] = param.Recipient.ProviceCity;
            xlWorkSheet.Cells[realRowIndex, ToIndex("AA")] = param.Recipient.CnAddress ;
            xlWorkSheet.Cells[realRowIndex, ToIndex("AB")] = param.Recipient.PostCode;
            xlWorkSheet.Cells[realRowIndex, ToIndex("AC")] = param.Recipient.MainTel;
        }
    }
}
