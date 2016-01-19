using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace daigou.services.test
{
    [TestClass]
    public class WaybillSearchTest
    {
        [TestMethod]
        public void EMSLocalBarcodeServiceTestMethod1()
        {
            EMSLocalBarcodeService EMSLocalBarcodeService = new EMSLocalBarcodeService();

            string test = EMSLocalBarcodeService.GetEMSLocalBarcode("30100687300000000186100335");

            Console.WriteLine(test);
        }
    }
}
