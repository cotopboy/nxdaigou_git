using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MigraDoc.DocumentObjectModel;
using MigraDoc.RtfRendering;
using PdfSharp.Pdf;
using MigraDoc.Rendering;
using System.Diagnostics;
using Utilities.IO;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;
using System.Globalization;


namespace daigou.services
{
    public class ProductExportToPdfService
    {
        private ProductPriceCalcuateService productPriceCalcuateService;
        public ProductExportToPdfService(ProductPriceCalcuateService productPriceCalcuateService)
        {
            this.productPriceCalcuateService = productPriceCalcuateService;
        }

        public void Export(List<domain.Product> productList, decimal euro2cny, decimal serviceRate)
        {
            Document document = CreateDocument(productList,euro2cny,serviceRate);
            string ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(document);          
         
            bool unicode = true;
            PdfFontEmbedding embedding = PdfFontEmbedding.Always;  
                      
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);            
            pdfRenderer.Document = document;           
            pdfRenderer.RenderDocument();            
            string filename = Guid.NewGuid().ToString() + ".pdf";
            pdfRenderer.PdfDocument.Save(filename);            
            Process.Start(filename);
        }

        private Document CreateDocument(List<domain.Product> productList, decimal euro2cny, decimal serviceRate)
        {
            productList.OrderBy(x => x.Brand);

            Document document = new Document();
            document.Styles[StyleNames.Normal].Font.Name = "Arial Unicode MS";
            document.Styles[StyleNames.Normal].Font.Color = new Color(30, 30, 30);

            Section section = document.AddSection();
            section.PageSetup.LeftMargin = 20;
            section.PageSetup.TopMargin = 20;
            Paragraph header = new Paragraph();
            header.Format.Font.Size = 35;
            header.Format.Alignment = ParagraphAlignment.Center;
            header.AddText("");
            header.AddLineBreak(); header.AddLineBreak(); header.AddLineBreak();
            header.AddText("吕贝克Nx-daigou商品报价单");
            header.AddLineBreak(); header.AddLineBreak(); header.AddLineBreak();
            document.LastSection.Add(header);

            Paragraph code = new Paragraph();
            code.Format.Font.Size = 15;
            string codeTxt = "编号: " + (euro2cny * 100.0m).ToString() + "-" + (serviceRate * 100.0m).ToString();
            code.AddText(codeTxt.Replace(".","#"));
            code.Format.Alignment = ParagraphAlignment.Center;
            code.AddLineBreak(); code.AddLineBreak(); code.AddLineBreak();
            document.LastSection.Add(code);

            Paragraph time = new Paragraph();
            time.Format.Alignment = ParagraphAlignment.Center;
            time.Format.Font.Size = 15;

            DateTimeFormatInfo chDatetimeCulture = new CultureInfo("zh-CN", false).DateTimeFormat;
            string result = DateTime.Today.ToString(chDatetimeCulture.LongDatePattern);
            time.AddText("日期: " + result);
            time.AddLineBreak(); time.AddLineBreak(); time.AddLineBreak();
            document.LastSection.Add(time);


            string productCountTxt = "商品总数: " + productList.Count.ToString();
            Paragraph productCountPara = new Paragraph();
            productCountPara.Format.Alignment = ParagraphAlignment.Center;
            productCountPara.AddText(productCountTxt);
            productCountPara.AddLineBreak(); productCountPara.AddLineBreak();
            document.LastSection.Add(productCountPara);


            string remarkTxt = "说明: 报价单里的价格均不包含邮费";
            Paragraph remarkheaderPara = new Paragraph();
            remarkheaderPara.Format.Alignment = ParagraphAlignment.Center;
            remarkheaderPara.Format.Font.Size = 10;
            remarkheaderPara.AddText(remarkTxt);
            remarkheaderPara.AddLineBreak();
            document.LastSection.Add(remarkheaderPara);


            document.LastSection.AddPageBreak();
            
            string lastBrand = "";
            int index = 1;

            foreach (var item in productList)
            {
                if ( lastBrand != item.Brand)
                {
                    Paragraph p = new Paragraph();
                    p.Format.Font.Size = 15;
                    p.Format.Font.Bold = true;
                    p.Format.Font.Color = Colors.DarkBlue;
                    p.AddText("==" + item.Brand + "==");
                    document.LastSection.Add(p);
                    lastBrand = item.Brand;                    
                }
 
                Table table = document.LastSection.AddTable();                
                table.Borders.Visible = true;
                table.Borders.Color = Colors.Gray;


                Column column = table.AddColumn(Unit.FromCentimeter(5));
                column.Format.Alignment = ParagraphAlignment.Center;

                column = table.AddColumn(Unit.FromCentimeter(12));
                column.Format.Alignment = ParagraphAlignment.Left;

                column = table.AddColumn(Unit.FromCentimeter(3));
                column.Format.Alignment = ParagraphAlignment.Left;
    

                table.Rows.Height = 10;

                Row row = table.AddRow();
                row.Cells[0].MergeDown = 2;
                string path = (item.Photo.IsNullOrEmpty()) ? "images\\default.png" : item.Photo;

                Paragraph imagePara = new Paragraph();
                imagePara.Add(CreateImage(DirectoryHelper.CombineWithCurrentExeDir(path)));
                imagePara.Format.Alignment = ParagraphAlignment.Center;

                row.Cells[0].Add(imagePara);
                row.Cells[1].AddParagraph("编号: " + item.ID + "\t\t别名:" + item.Code + "   ");

                string priceTxt = this.productPriceCalcuateService.GetPrice(item, euro2cny * serviceRate).ToString() + "元";
                Paragraph pricePara = new Paragraph();
                pricePara.AddText(priceTxt);
                pricePara.Format.Font.Color = Colors.OrangeRed;
                row.Cells[2].Add(pricePara);               


                row = table.AddRow();
                string productNameTxt = item.Brand + "->" + item.Name;
                Paragraph productNamePara = new Paragraph();
                productNamePara.AddText(productNameTxt);
                productNamePara.Format.Font.Color = Colors.DarkBlue;
                row.Cells[1].Add(productNamePara);      
          
                row.Cells[2].AddParagraph("重量: " + item.GrossWeight + " g");


                row = table.AddRow();
                row.Cells[1].AddParagraph("[规格:" + item.Spec + "]              [适用于:" + item.ApplicableCrowd+"]");

                Paragraph remarkPara = new Paragraph();
                remarkPara.AddText(item.Remark);
                remarkPara.Format.Font.Size = 8;
                row.Cells[1].Add(remarkPara);

                Paragraph tagListPara = new Paragraph();
                tagListPara.AddText(item.TagList);
                tagListPara.Format.Font.Size = 8;
                row.Cells[2].Add(tagListPara);                    

                table.SetEdge(0, 0, 3, 3, Edge.Top, BorderStyle.Single, 2, Colors.Black);

                index++; 
            }

            return document;
        }


        private Image CreateImage(string path)
        {
            if(path.IsNullOrEmpty()) path = "images\\default.png";

            Image image = new Image(path);
            image.Height = "2.5cm";
            image.LockAspectRatio = true;
            image.WrapFormat.Style = WrapStyle.TopBottom;

            return image;
        }

    }
}
