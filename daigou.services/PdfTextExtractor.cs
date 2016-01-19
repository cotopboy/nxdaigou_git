namespace daigou.services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Content;
    using PdfSharp.Pdf.Content.Objects;
    using PdfSharp.Pdf.IO;
    using Utilities.DataTypes.ExtensionMethods;
    using Utilities.IO.ExtensionMethods;
    using Utilities.IO;
    using System.IO;
    using System.Diagnostics;

      

    public class PdfTextExtractor
    {
        public PdfTextExtractor()
        {

        }

        public string GetTextByCommandLine(string pdfFileName)
        {
            string pdf2TextPath = Path.Combine(DirectoryHelper.CurrentExeDirectory, "ThirdParty", "pdftotext.exe");

            string outfilePath = pdfFileName.Replace(".pdf", ".txt");
            

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pdf2TextPath,
                    Arguments = pdfFileName.AddQuotes() + " " + outfilePath.AddQuotes(),
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            process.Start();
            process.WaitForExit();

            FileInfo txtFile = new FileInfo(outfilePath);
            string content = txtFile.Read();
            //File.Delete(txtFile.FullName);
            return content;
        }


        public static string GetText(string pdfFileName)
        {
            using (var _document = PdfReader.Open(pdfFileName, PdfDocumentOpenMode.ReadOnly))
            {
                var result = new StringBuilder();
               foreach (var page in _document.Pages.OfType<PdfPage>())
                {
                    ExtractText(ContentReader.ReadContent(page), result);
                    result.AppendLine();
                }
                return result.ToString();
            }
        }

        #region CObject Visitor
        private static void ExtractText(CObject obj, StringBuilder target)
        {
            if (obj is CArray)
                ExtractText((CArray)obj, target);
            else if (obj is CComment)
                ExtractText((CComment)obj, target);
            else if (obj is CInteger)
                ExtractText((CInteger)obj, target);
            else if (obj is CName)
                ExtractText((CName)obj, target);
            else if (obj is CNumber)
                ExtractText((CNumber)obj, target);
            else if (obj is COperator)
                ExtractText((COperator)obj, target);
            else if (obj is CReal)
                ExtractText((CReal)obj, target);
            else if (obj is CSequence)
                ExtractText((CSequence)obj, target);
            else if (obj is CString)
                ExtractText((CString)obj, target);
            else
                throw new NotImplementedException(obj.GetType().AssemblyQualifiedName);
        }

        private static void ExtractText(CArray obj, StringBuilder target)
        {
            foreach (var element in obj)
            {
                ExtractText(element, target);
            }
        }
        private static void ExtractText(CComment obj, StringBuilder target) {  }
        private static void ExtractText(CInteger obj, StringBuilder target) {  }
        private static void ExtractText(CName obj, StringBuilder target) { }
        private static void ExtractText(CNumber obj, StringBuilder target) { }
        private static void ExtractText(COperator obj, StringBuilder target)
        {
            if (obj.OpCode.OpCodeName == OpCodeName.Tj || obj.OpCode.OpCodeName == OpCodeName.TJ)
            {
                foreach (var element in obj.Operands)
                {
                    ExtractText(element, target);
                }
                target.Append(" ");
            }
        }
        private static void ExtractText(CReal obj, StringBuilder target) { Console.WriteLine(obj.Value); }
        private static void ExtractText(CSequence obj, StringBuilder target)
        {
            foreach (var element in obj)
            {
                ExtractText(element, target);
            }
        }
        private static void ExtractText(CString obj, StringBuilder target)
        {
            target.Append(obj.Value);
        }
        #endregion
    }
}