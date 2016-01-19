using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace daigou.services.test
{
    [TestClass]
    public class PDFTextExtracterTest
    {
        [TestMethod]
        public void PdfTextExtractorTestMethod1()
        {
            string text = new PdfTextExtractor().GetTextByCommandLine(@"D:\DHL\A.pdf");

            int textlength = text.Length;

        }

        [TestMethod]
        public void ExtractZdBpostWaybillSnTestMethod2()
        {
            string str = "Name CDE GmbH Business Street PO BOX Zipcode 1934 City EMC Brucargo Country Belgium Contact Tel* Contact Email* P Name Tang Wen Business Tang Wen Street No.6 Yuan She Hua Yuan Room501 Zipcode 311106 City HangZhou Country China Contact Tel* 13989855767 Contact Email* 30100687300000000093484432 Service level: PRI Customs documents to be validated for export: YES Sender's instruction in case of non-delivery: RETURN TO SENDER Date: 16/12/2013 Category of item: GOODS Description: privatepresent Detailed Description of Contents Quantity Net weight Value privatepresent Total gross weight: 7kg Total value: 85 EUR Postage fee: *This information will only be used for delivery purposes / Deze informatie zal enkel gebruikt worden voor het afleveren van uw zending / Cette information sera uniquement utilisée dans le cadre de la livraison du paquet / Diese Information wird ausschließlich zum Zweck der Sendungszustellung verwendet Page 2";

            string ret = RexHelper.ExtractZdBpostWaybillSn(str);

            Assert.IsTrue(ret == "30100687300000000093484432");

        }

        [TestMethod]
        public void ExtractZdDHLSnTestMethod1()
        {
            string str = "adfa 30100687300000000141854793 00340433836568371426 Ettore-Bugatti-Str.45 .";

            string ret = RexHelper.ExtractDHLWaybillSn(str);

            Assert.IsTrue(ret == "00340433836568371426");

        }


    }
}
