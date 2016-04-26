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

namespace daigou.services
{
    public class ZdBpostWaybillExcelBuilder : daigou.services.IDhlWaybillExcelBuilder
    {
        public event Action<float> OnReportStep;
       
        private ResourceReleaser resourceReleaser;
        private RandomNameService randomNameService;
        protected ProcessModeInfo modeInfo;

        private List<string[]> mapping = new List<string[]>()
            {
                new string[]{"AJ","AK","AM","AN"},
                new string[]{"AQ","AR","AT","AU"},
                new string[]{"AX","AY","BA","BB"}
            };

        public ZdBpostWaybillExcelBuilder(ProcessModeInfo modeInfo, ResourceReleaser resourceReleaser, RandomNameService randomNameService)
        {
            this.modeInfo = modeInfo;
           this.resourceReleaser = resourceReleaser;
           this.randomNameService = randomNameService;
        }

        public string Generate(string targetDir, List<DhlWaybillParam> dhlWaybillParamList, string taobaoOrderSn = "")
        {
            string ExcelFile = this.resourceReleaser.ReleaseXls("ZD_Bpost_Template", targetDir);

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlApp = new Excel.Application();
            int realRowIndex = 6;

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

            xlWorkSheet.ReleaseComObject();
            xlWorkBook.ReleaseComObject();
            xlApp.ReleaseComObject();

            File.Delete(ExcelFile);

            return newFullFileName;
        }

     

        protected virtual string GetFileName(List<DhlWaybillParam> dhlWaybillParamList)
        {
            return string.Format("{0}_BPOST单_{1}.xls", this.modeInfo.WanwanName, DateTime.Now.ToString("MM.dd"));
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

            xlWorkSheet.Cells[realRowIndex, ToIndex("A")] = randomNameService.GetNextRandomName().NameToPinyin();
            xlWorkSheet.Cells[realRowIndex, ToIndex("N")] = param.Recipient.Name.NameToPinyin();

            xlWorkSheet.Cells[realRowIndex, ToIndex("Q")] = ToMax(GetShortPingYinAddress(param.Recipient.CnAddress),99);

            xlWorkSheet.Cells[realRowIndex, ToIndex("U")] = param.Recipient.PostCode;//邮编
            xlWorkSheet.Cells[realRowIndex, ToIndex("V")] = ToMax(GetFormattedCity(param.Recipient.ProviceCity), 40);//城市
            xlWorkSheet.Cells[realRowIndex, ToIndex("AA")] = param.Recipient.MainTel;//电话
            xlWorkSheet.Cells[realRowIndex, ToIndex("AB")] = param.Order.PacketWeight;

            FillItemDetail(xlWorkSheet,realRowIndex, param.Order.Detail);
            
        }

        private float FillItemDetail(Excel.Worksheet xlWorkSheet,int realRowIndex,string inputDetail)
        {
          
            ZdBpostDetails zbpostDetails = new ZdBpostDetails();

            var lines = inputDetail.ToLines();

            lines.ForEach(x => zbpostDetails.AcceptNewItem(x));

            for (int i = 0; i < zbpostDetails.Items.Count; i++)
            {
                var item = zbpostDetails.Items[i];
                xlWorkSheet.Cells[realRowIndex, ToIndex(mapping[i][0])] = item.IntQuantity;
                xlWorkSheet.Cells[realRowIndex, ToIndex(mapping[i][1])] = item.IntValue * item.IntQuantity;
                xlWorkSheet.Cells[realRowIndex, ToIndex(mapping[i][2])] = item.Content;
                xlWorkSheet.Cells[realRowIndex, ToIndex(mapping[i][3])] = item.NetWeight;
            }
           
            return (float)Math.Round(zbpostDetails.TotalValue, 2);
        }

        private string GetFormattedCity(string CnProvince)
        {
            string pinyinCity = CnProvince.ToPinyin();
            string output = pinyinCity.ToWordFirstCharcterUpperCase().Replace(" ","");
            return output;
        }

        private string FindRoom(string cnAddress)
        {
            bool isSectionStart = false;
            List<string> sectionList = new List<string>();
            StringBuilder section = new StringBuilder() ;
            foreach (var item in cnAddress)
            {
                if (Char.IsNumber(item))
                {
                   isSectionStart = true;
                   section.Append(item);
                }
                else
                {
                    if (isSectionStart)
                    {
                        sectionList.Add(section.ToString());
                        section = new StringBuilder();
                        isSectionStart = false;
                    }
                }
            }
            sectionList.Add(section.ToString());

            string result = string.Join("-", sectionList);

            if (result.Length == 0) return "EMPTY";

            return ToMax(result,8);
        }

        private string GetShortPingYinAddress(string input)
        {
            input = input.Replace("`", string.Empty).Replace("-",".");

            int shenIndex = input.IndexOf("省");
            if (shenIndex > 0) input = input.Substring(shenIndex + 1);

            int shiIndex = input.IndexOf("市");
            input = input.Substring(shiIndex + 1);

            var itemList = input.ToPinyin().Split(new string[]{" "}, StringSplitOptions.RemoveEmptyEntries);

            var CamelList = itemList.Select(x => x.ToFirstCharacterUpperCase()).ToList();

            return string.Join("", CamelList);
        }

        private string ToMax(string input,int maxCount)
        {
            if (input.Length < maxCount) return input;

            else return input.Substring(0, maxCount-1);

        }

    }
}
