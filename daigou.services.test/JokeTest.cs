using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Configuration;
using Utilities.IO;

namespace daigou.services.test
{
    [TestClass]
    public class JokeTest
    {
        [TestMethod]
        public void TestMethod1()
        {


            AppStatusManager appStatu = new AppStatusManager(DirectoryHelper.CombineWithCurrentExeDir("settting"));
            appStatu.AddOrUpdate("joke_index","0");
            appStatu.SaveToFile();

        }
    }
}
