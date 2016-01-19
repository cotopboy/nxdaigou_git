using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Resources;

namespace daigou.services
{
    public class ResourceReleaser
    {
        public string ReleaseXls(string ResourceName, string TargetDir)
        {
            string FileName = string.Format("{0}{1}.xls", ResourceName, DateTime.Now.Ticks);

            string fullFileName = Path.Combine(TargetDir, FileName);

            byte[] Save = (byte[])Properties.Resources.ResourceManager.GetObject(ResourceName);

            FileStream fsObj = new FileStream(fullFileName, FileMode.Create);
            fsObj.Write(Save, 0, Save.Length);
            fsObj.Close();

            return fullFileName;
        }

        public string ReleaseCXTemplate(string TargetDir)
        {
            string FileName = string.Format("CXTemplate{0}.xls", DateTime.Now.Ticks);

            string fullFileName = Path.Combine(TargetDir, FileName);

            byte[] Save = Properties.Resources.CX_Template;
            FileStream fsObj = new FileStream(fullFileName, FileMode.Create);
            fsObj.Write(Save, 0, Save.Length);
            fsObj.Close();

            return fullFileName;
        }
    }
}
