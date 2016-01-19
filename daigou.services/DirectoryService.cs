using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace daigou.services
{
    public class DirectoryService
    {

        private string baseDir = @"D:\nxdaigou\NxdaigouMain";
        private string cnLabelDir = @"D:\nxdaigou\CnLabel";
        private string excelDir = @"D:\nxdaigou\WaybillExcel";
        private string waybillRepositoryDir = @"D:\nxdaigou\waybill_repository";
        private string waybillSearchDir = @"D:\nxdaigou\SearchRet";

        public string WaybillSearchDir
        {
            get { return waybillSearchDir; }
        }

        public string WaybillRepositoryDir
        {
            get { return waybillRepositoryDir; }
        }

        public string ExcelDir
        {
            get { return excelDir; }
        }

        public string CnLabelDir
        {
            get 
            {
                return cnLabelDir; 
            }
        }

        public string BaseDir
        {
            get { return baseDir; }
        }

        public string GetOrCreateBaseDir()
        {
            if (!Directory.Exists(BaseDir)) Directory.CreateDirectory(BaseDir);
            return BaseDir;
        }

        public string GetOrCreateExcelDir()
        {
            if (!Directory.Exists(ExcelDir)) Directory.CreateDirectory(ExcelDir);
            return ExcelDir;
        }

        public string GetOrCreateSearchDir()
        {
            if (!Directory.Exists(waybillSearchDir)) Directory.CreateDirectory(waybillSearchDir);
            return waybillSearchDir;
        }

        public string GetOrCreateCnLabelDir()
        {
            if (!Directory.Exists(cnLabelDir)) Directory.CreateDirectory(cnLabelDir);
            return cnLabelDir;
        }

        public string DetailsLogFilePath
        {
            get { return  Path.Combine(GetOrCreateBaseDir(), "d.log");}
        }

        public string CreateDhlWaybillFolder(List<string> nameList)
        {
            string NameListTxt = string.Join("_", nameList);
            string dhlWaybillFolder = Path.Combine(GetOrCreateExcelDir(), "运单申请_" + NameListTxt + "_" + DateTime.Now.ToString("yyyy_MM_dd_HHmmss"));

            if (!Directory.Exists(dhlWaybillFolder))
            {
                Directory.CreateDirectory(dhlWaybillFolder);
            }
            return dhlWaybillFolder;
        }
    }
}
