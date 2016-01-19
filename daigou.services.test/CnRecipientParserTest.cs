using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.DataTypes.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;

namespace daigou.services.test
{
    [TestClass]
    public class CnRecipientParserTest
    {
        [TestMethod]
        public void Parser1()
        {
            string inputText = "佘慕荣 广东省潮州市名邦花苑A区4幢1202  521000 13413750015";  

            CnRecipientParser parser = new CnRecipientParser();

            var info = parser.Parse(inputText);

            Assert.AreEqual("广东省潮州市名邦花苑A区4幢1202", info.Address);
            Assert.AreEqual("佘慕荣", info.Name);
            Assert.AreEqual("521000", info.PostCode);
            Assert.AreEqual("13413750015", string.Join(",", info.TelList));

        }

        [TestMethod]
        public void Parser2()
        {
            string inputText = "佘慕荣 广东省潮州市名邦花苑A区4幢1202  521000 13413750015 13413750099 ";

            CnRecipientParser parser = new CnRecipientParser();

            var info = parser.Parse(inputText);

            Assert.AreEqual("广东省潮州市名邦花苑A区4幢1202", info.Address);
            Assert.AreEqual("佘慕荣", info.Name);
            Assert.AreEqual("521000", info.PostCode);
            Assert.AreEqual("13413750015,13413750099", string.Join(",", info.TelList));

        }


        [TestMethod]
        public void PinyinTest()
        {
            string inputText = "荍 佘慕荣 广东省潮州市名邦花苑A区4幢1202  521000 13413750015 13413750099 ";

            string pinying = inputText.ToPinyin();

            Assert.IsTrue(pinying.Length > 0);

        }
    }
}
