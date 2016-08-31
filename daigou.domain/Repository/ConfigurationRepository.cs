using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain
{
    public interface ConfigurationRepository
    {
        Dictionary<string, string> ConfigDict { get;}

        IEnumerable<Configuration> GetAllConfiguration();

        void Save(Configuration item);

        void AddOrUpdate(string key, string value);

        void GenBinDb();
    }
}
