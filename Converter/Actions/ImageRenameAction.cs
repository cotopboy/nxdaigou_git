using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.dal.DaigouDataFile;
using Utilities.IO;
using System.IO;

namespace Converter.Actions
{
    public class ImageRenameAction
    {

        private List<string> delList = new List<string>();

        public void Start()
        {
            FileDBMgr fileDBMgr = new FileDBMgr();
            var db = fileDBMgr.LoadFileDB();

            int index = 1;

            foreach (var product in db.ProductList)
            {
                if (product.Photo.Length == 0) continue;

                string oldName = product.Photo.Trim();

                string newName = index.ToString()+ Path.GetExtension(oldName);

                if (ReNameFile(oldName, "x" + newName))
                {
                    product.Photo = "images/" + "x" + newName;
                }
                else
                {
                    
                }

                index++;
            }

            fileDBMgr.Save();

            delList.ForEach(x => File.Delete(x));
        }

        private bool ReNameFile(string oldName, string newName)
        {
            var fullPath = DirectoryHelper.CombineWithCurrentExeDir(oldName);

            
            var oldFilename = Path.GetFileName(fullPath);

            string newPath = fullPath.Replace(oldFilename, newName);

            try
            {
                File.Copy(fullPath, newPath);

                this.delList.Add(fullPath);
                return true;
            }
            catch(Exception exp)
            {
                Console.WriteLine(fullPath);
                return false;
            }
        }
    }
}
