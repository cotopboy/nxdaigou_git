using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;
using System.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.IO;
using Utilities.Serialization;
using Utilities.FileFormats.Zip;

namespace daigou.dal.DaigouDataFile
{

    public class FileDBMgr
    {
        private FileDB db = null;
        private string fileDbPath;

        public FileDBMgr()
        {
            fileDbPath = DirectoryHelper.CombineWithCurrentExeDir("db.xml");
        }

        public FileDB LoadFileDB()
        {
            if (db == null)
            {
                FileInfo fileInfo = new FileInfo(this.fileDbPath);

                var bytes = fileInfo.ReadBinary();

                db = FormatterMg.XMLDerObjectFromBytes(typeof(FileDB), bytes) as FileDB;
            }

            return db;
        }


        public void SaveAsBin()
        {
            FormatterMg.BinarySerObject(this.db, DirectoryHelper.CombineWithCurrentExeDir("bindb.dat"));
        }

        public void Save()
        {
            Byte[] bytes = this.db.ObjectToBlob();

            string x  = FormatterMg.XMLSerObjectToString(this.db);

            FileInfo xf = new FileInfo(this.fileDbPath);

            xf.Save(x, Encoding.UTF8);


            string basePath = Path.GetDirectoryName(this.fileDbPath);
            string backupPath = Path.Combine(basePath,"db_backup");

            string zipFileName = "db_" + DateTime.Now.ToString("yyyy_MMdd_HHmm") + "_.zip";
            string zipFileFullpath = Path.Combine(backupPath, zipFileName);

            if (!Directory.Exists(backupPath)) Directory.CreateDirectory(backupPath);
            
            ZipFile zip = new ZipFile(zipFileFullpath, true);
            zip.AddFile(this.fileDbPath);           
            zip.Dispose();
            

        }
    }



    [Serializable]
    public class FileDB
    {
        private List<Order> orderList;

        private List<Recipient> recipientList;

        private List<Configuration> configurationList;

        private List<Product> productList;

        private List<Bill> billList;

        public List<Bill> BillList
        {
            get { return billList; }
            set { billList = value; }
        }

        public List<Product> ProductList
        {
            get { return productList; }
            set { productList = value; }
        }

        public List<Configuration> ConfigurationList
        {
            get { return configurationList; }
            set { configurationList = value; }
        }

        public List<Recipient> RecipientList
        {
            get { return recipientList; }
            set { recipientList = value; }
        }

        public List<Order> OrderList
        {
            get { return orderList; }
            set { orderList = value; }
        }

    }
}
