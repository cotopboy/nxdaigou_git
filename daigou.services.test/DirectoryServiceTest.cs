using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace daigou.services.test
{
    [TestClass]
    public class DirectoryServiceTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dirService = new DirectoryService();
            dirService.CreateDhlWaybillFolder(new List<string>() { "xxx", "xxddd" });
        }
    }
}
