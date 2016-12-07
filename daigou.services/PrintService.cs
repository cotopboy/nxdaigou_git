using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using daigou.infrastructure.ExtensionMethods;
using System.IO;
using System.Threading;

namespace daigou.services
{
    public class PrintService
    {
        private Dictionary<string, Action<string>> PrintFuncDict = new Dictionary<string, Action<string>>();

        public PrintService()
        {
            PrintFuncDict.Add(".xls", PrintXlsFile);
            PrintFuncDict.Add(".pdf", PrintPDF);
        }

        public void PrintFile(string filePath)
        {
            FileInfo finfo = new FileInfo(filePath);

            if(PrintFuncDict.ContainsKey(finfo.Extension))
            {
                PrintFuncDict[finfo.Extension](filePath);

                Thread.Sleep(finfo.Extension == ".pdf" ? 5000 : 0);
            }
        }

        private void PrintPDF(string fileName)
        {
            // On my computer (running Windows Vista 64) it is here:
           // PdfFilePrinter.AdobeReaderPath = @"C:\Program Files (x86)\Adobe\Reader 11.0\Reader\AcroRd32.exe";

            // Set the file to print and the Windows name of the printer.
            // At my home office I have an old Laserjet 6L under my desk.
          //  PdfFilePrinter printer = new PdfFilePrinter(fileName, "激光");

          //  printer.Print();
          
        }

        private void PrintXlsFile(string filePath)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlApp = new Excel.Application();


            xlWorkBook = xlApp.Workbooks.Open(filePath);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkBook.PrintOutEx();

            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            xlWorkSheet.ReleaseComObject();
            xlWorkBook.ReleaseComObject();
            xlApp.ReleaseComObject();
        
        
        }
    }
}
