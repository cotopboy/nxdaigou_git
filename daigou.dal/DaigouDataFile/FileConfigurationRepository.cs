using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;

namespace daigou.dal.DaigouDataFile
{
    public class FileConfigurationRepository : FileRepositoryBase, ConfigurationRepository
    {
        private List<Configuration> configList;

        public FileConfigurationRepository(FileDBMgr fileDbmgr) :base(fileDbmgr)
        {
            this.configList = FileDB.ConfigurationList;
        }

        public  Dictionary<string, string> ConfigDict
        {
            get { return this.configList.ToDictionary(x => x.Key, x => x.Value); }
        }

        public  IEnumerable<Configuration> GetAllConfiguration()
        {
            return this.configList;
        }

        public  void Save(Configuration item)
        {
            this.fileDbmgr.Save();
        }

        public  void AddOrUpdate(string key, string value)
        {
            var dbObj = this.configList.SingleOrDefault(x => x.Key == key);
            if (dbObj != null)
            {

                try
                {
                    dbObj.Value = value;
                    this.fileDbmgr.Save();
                }
                catch { }
            }
            else
            {
                Configuration config = new Configuration();
                config.Key = key;
                config.Value= value;
                int maxid = configList.Max(x => x.ID);
                config.ID = maxid + 1;
                this.configList.Add(config);
                this.fileDbmgr.Save();
            }
        }
    }
}
