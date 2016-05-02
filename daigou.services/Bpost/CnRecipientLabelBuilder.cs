using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using System.IO;
using daigou.domain.Base;
using daigou.infrastructure.ExtensionMethods;
namespace daigou.services
{
    public class CnRecipientLabelBuilder
    {
        private CnRecipientInfo cnRecipientInfo = null;
        private int kg = 7;
        private float fontSize = 12f;

        public CnRecipientLabelBuilder()
        {

        }

        public void SetFontSize(float fontSize)
        {
            this.fontSize = fontSize;
        }

        public void BuildeDocument(CnRecipientInfo cnRecipientInfo, int kg, string docFilePath, bool IsPrintRequired = false)
        {
            this.cnRecipientInfo = cnRecipientInfo;
            this.kg = kg;

            Object Nothing = System.Reflection.Missing.Value;
            Application WordApp = new Application();
            Document WordDoc = WordApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);

            WordDoc.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;
            WordApp.Selection.Font.Name = "微软雅黑";
            WordApp.Selection.Font.Size = this.fontSize;
            WordApp.Selection.Font.Scaling = 100;
            WordApp.Selection.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
            WordApp.Selection.ParagraphFormat.LineSpacing = 20;

            WordApp.Selection.TypeText("亲爱的快递员，您辛苦了~！累了就休息休息~注意身体~");
            WordApp.Selection.TypeParagraph();

            WordApp.Selection.Font.Size = 8f;
            WordApp.Selection.TypeText(new string('-',120));
            WordApp.Selection.TypeParagraph();

            WordApp.Selection.Font.Size = this.fontSize; ;
            WordApp.Selection.TypeText(string.Format("邮编:\t\t{0}", this.cnRecipientInfo.PostCode));            
            WordApp.Selection.TypeParagraph();

            WordApp.Selection.TypeText(string.Format("地址:\t\t{0}", this.cnRecipientInfo.Address));
            WordApp.Selection.TypeParagraph();

            WordApp.Selection.TypeText(string.Format("收件人:\t\t{0} \t\t\t\t{1}", this.cnRecipientInfo.Name, this.cnRecipientInfo.Name.NameToPinyin()));
            WordApp.Selection.TypeParagraph();

            WordApp.Selection.TypeText(string.Format("电话:\t\t{0}", this.cnRecipientInfo.TelListText));
            WordApp.Selection.Font.Size = 8f;
            WordApp.Selection.TypeParagraph();

            WordApp.Selection.TypeText(new string('-', 120));
            WordApp.Selection.TypeParagraph();

            WordApp.Selection.TypeText("★ 祝愿宝宝与妈妈开心快乐每一天 ☆ 也祝快递员天天开心：）★ ");
            WordApp.Selection.TypeParagraph();

            WordApp.Selection.TypeParagraph();
 
            object filename = Path.Combine(docFilePath, string.Format("{0} {1} 地址.doc",this.cnRecipientInfo.Name, DateTime.Now.ToString("MM-dd_HHmmss")));

            WordDoc.SaveAs(ref filename, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);

            if (IsPrintRequired) WordDoc.PrintOut();
            
            WordDoc.Close(Type.Missing, Type.Missing, Type.Missing);
            WordApp.Quit(Type.Missing, Type.Missing, Type.Missing);


        }

    }
}
