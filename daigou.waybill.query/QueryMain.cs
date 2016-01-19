using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;
using GetCodes;
using daigou.dal.DaigouDataFile;
using System.Threading;

namespace daigou.waybill.query
{
    public partial class QueryMain : Form
    {
        private FileDBMgr fileDbMgr;
        private FileDB fileDb;
        private bool isFirstComplete = true;

        public QueryMain()
        {
            InitializeComponent();
            this.textBox1.Text = AppInputParameters.Current.EMSCode;
            this.Text = this.textBox1.Text;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.isFirstComplete)
            {
                this.BtnRun.PerformClick();
                this.isFirstComplete = false;
            }
        }

        private Image GetRegCodePic(WebBrowser wbMail, string id, string Src, string Alt)
        {
            HTMLDocument doc = (HTMLDocument)wbMail.Document.DomDocument;
            HTMLBody body = (HTMLBody)doc.body;
            IHTMLControlRange rang = (IHTMLControlRange)body.createControlRange();
            IHTMLControlElement Img;
            if (id == "")
            {
                int ImgNum = GetPicIndex(wbMail, Src, Alt);
                if (ImgNum == -1) return null;
                Img = (IHTMLControlElement)wbMail.Document.Images[ImgNum].DomElement;
            }
            else
                Img = (IHTMLControlElement)wbMail.Document.GetElementById(id).DomElement;
            //Img = (IHTMLControlElement)wbMail.Document.All[ImgName].DomElement;
            rang.add(Img);
            rang.execCommand("Copy", false, null);
            
            Image RegImg = Clipboard.GetImage();
            Clipboard.Clear();
            return RegImg;
        }

        private int GetPicIndex(WebBrowser wbMail, string Src, string Alt)
        {
            int imgnum = -1;
            for (int i = 0; i < wbMail.Document.Images.Count; i++)
            {
                IHTMLImgElement img = (IHTMLImgElement)wbMail.Document.Images[i].DomElement;
                if (Alt == "")
                {
                    if (img.src.Contains(Src)) return i;
                }
                else
                {
                    if (!string.IsNullOrEmpty(img.alt))
                    {
                        if (img.alt.Contains(Alt)) return i;
                    }
                }
            }
            return imgnum;
        }


        private void BtnRun_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Document.GetElementById("mailNum").InnerText = AppInputParameters.Current.EMSCode ;

            HtmlElementCollection ret = webBrowser1.Document.All.GetElementsByName("checkCode");

            Image img = GetRegCodePic(this.webBrowser1, "checkCode", null, null);

            if (img == null) return;


            System.Drawing.Bitmap bitmap = new Bitmap(img);
            UnCodebase ud = new UnCodebase(bitmap);
            Bitmap processedImg = ud.GrayByPixels();
            ud.ClearNoise(128, 2);


            tessnet2.Tesseract ocr = new tessnet2.Tesseract();
            ocr.SetVariable("tessedit_char_whitelist", "0123456789");
            ocr.Init(Application.StartupPath + @"\\tmpe", "eng", true);
            List<tessnet2.Word> result = ocr.DoOCR(processedImg, Rectangle.Empty);
            string code = result[0].Text;

            foreach (HtmlElement item in ret)
            {
                item.InnerText = code;
            }


            HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
            HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
            IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
            element.text = "function SingleFormSubmit() { $(\"*\").hide(); $(\"#singleForm\").submit();}";
            head.AppendChild(scriptEl);
            webBrowser1.Document.InvokeScript("SingleFormSubmit");
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate("http://www.ems.com.cn/mailtracking/you_jian_cha_xun.html");
        }

        private void QueryMain_Shown(object sender, EventArgs e)
        {

            fileDbMgr = new FileDBMgr();
            this.fileDb = fileDbMgr.LoadFileDB();

            var recipient = this.fileDb.RecipientList.Single(x => x.ID == AppInputParameters.Current.RecipientID);

            this.label2.Text =   string.Format("{0}  {1}  {2}  {3}  {4}",
                      recipient.Name,
                      recipient.CnAddress,
                      recipient.PostCode,
                      recipient.MainTel,
                      recipient.OtherTels
                      );

            string bpostTempalte = "http://www.bpost2.be/bpi/track_trace/find.php?search=s&lng=en&trackcode={0}";
            string dhlTemplate = "http://nolp.dhl.de/nextt-online-public/set_identcodes.do?lang=en&idc={0}&rfn=&extendedSearch=true";

            this.webBrowser1.Navigate("http://www.ems.com.cn/mailtracking/you_jian_cha_xun.html");
            this.wbBpost.Navigate(string.Format(bpostTempalte, AppInputParameters.Current.BpostCode));

            this.wbDhl.Navigate(string.Format(dhlTemplate, AppInputParameters.Current.DHLCode));
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
            HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
            IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
            element.text = "function HideUnnecessary() { $(\".top-header\").hide();$(\".list_menu_container\").hide();$(\".body_footer\").hide();}";
            head.AppendChild(scriptEl);
            webBrowser1.Document.InvokeScript("HideUnnecessary");

            Action makeScreenshot = new Action(MakeScreenshotWait);

            makeScreenshot.BeginInvoke(null, null);
        }

        private void MakeScreenshotWait()
        {
            Thread.Sleep(200);

            this.Invoke(new MethodInvoker(delegate { MakeScreenshotMainThread(); }));
        }

        private void MakeScreenshotMainThread()
        {

            Bitmap FormScreenShot = new Bitmap(this.Width, this.Height);
            Graphics G = Graphics.FromImage(FormScreenShot);
            G.CopyFromScreen(this.Location, new Point(0, 0), this.Size);
            try
            {
                Clipboard.SetImage(FormScreenShot);
            }
            catch { }
        }

    }
}
