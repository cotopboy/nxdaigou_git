using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace daigou.services.test
{
    [TestClass]
    public class ResourceReleaserTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dirService = new DirectoryService();

            var dir =  dirService.CreateDhlWaybillFolder(new List<string> (){"张三","李四"});

            ResourceReleaser resourceReleaser = new ResourceReleaser();

            string fullPath = resourceReleaser.ReleaseCXTemplate(dir);
        }
    }
}
