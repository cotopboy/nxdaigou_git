using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using daigou.domain.Base;

namespace daigou.services.test
{
    [TestClass]
    public class CnRecipientLabelBuilderTest
    {
        [TestMethod]
        public void CnRecipientLabelBuilderTest1()
        {
            CnRecipientInfo cnRecipientInfo = new CnRecipientInfo()
            {
                Name = "张锐锋",
                Address = "广东省潮州市名邦花苑A区4幢1202",
                PostCode = "521000",
                TelList = new List<string>() { "13903090504", "13903090505", "13903090504", "13903090505" },
                Remark = "Atm3*6"
            };

            CnRecipientLabelBuilder labelBuilder = new CnRecipientLabelBuilder();

            labelBuilder.BuildeDocument(cnRecipientInfo, 10,@"D:\");


        }
    }
}
