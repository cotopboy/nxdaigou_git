using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace daigou.services.test
{
    [TestClass]
    public class ZdBpostWaybillExtractorTest
    {
        [TestMethod]
        public void TestMethodAddressLineCounter()
        {
            BpostAddressLineCounter counter = new BpostAddressLineCounter(null);
            string cnAddress = "浙江省杭州市上城区延安路391号707室中国移动通信集团浙江有限公司武林分公司";
            Assert.IsTrue(counter.GetBpostAddressLineCountEx(cnAddress,false,null,null) == 3);


            cnAddress = "江西省宜春市高安市筠阳街道办院背熊村";
            Assert.IsTrue(counter.GetBpostAddressLineCountEx(cnAddress, false, null, null) == 2);

        }
    }
}
