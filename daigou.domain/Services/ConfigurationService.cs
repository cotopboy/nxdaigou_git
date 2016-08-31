using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain
{
    public class ConfigurationService 
    {
       private readonly ConfigurationRepository repository;

       public ConfigurationService(ConfigurationRepository repository)
        {
            this.repository = repository;
        }

       public void AddOrUpdate(string key, string value)
       {
           this.repository.AddOrUpdate(key, value);
       }


       public Dictionary<string, string> ConfigDict
       {
           get { return this.repository.ConfigDict; }
       }

        public IEnumerable<Configuration> GetAllConfiguration()
        {
            return this.repository.GetAllConfiguration();
        }

        public void SaveConfigurations(IEnumerable<Configuration> configurationList)
        {
            foreach (var item in configurationList)
	        {
                this.repository.Save(item);
	        }
        }

        public void GenBinDb()
        {
            this.repository.GenBinDb();
        }


    }
}
