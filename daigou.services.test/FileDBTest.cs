using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using daigou.dal;
using daigou.dal.DaigouDataFile;
using System.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;

namespace daigou.services.test
{
    [TestClass]
    public class FileDBTest
    {
   
        [TestMethod]
        public void TestMethod2()
        {
           

            FileInfo fileInfo = new FileInfo(@"T:\test.dat");

            var bytes = fileInfo.ReadBinary();

            FileDB db =  bytes.ToObject<FileDB>();

            List<domain.Order> list = db.OrderList;
        }
    }
}
