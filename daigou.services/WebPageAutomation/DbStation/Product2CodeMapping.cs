using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;


namespace daigou.services.WebPageAutomation.DbStation
{
    public class Product2CodeMapping
    {
        private static Dictionary<string, string> dict = new Dictionary<string, string>() 
        {
                {"Ap 爱他美 Pre 段 X 6"  ,"4008976022350"},
                {"Ap 爱他美 1   段 X 6"  ,"4008976022329"},
                {"Ap 爱他美 2   段 X 6"  ,"4008976022336"},
                {"Ap 爱他美 3   段 X 6"  ,"4008976022343"},
                {"Ap 爱他美 1+  段 X 8"  ,"4008976022305"},
                {"Ap 爱他美 2+  段 X 8"  ,"4008976022312"},
                {"Combi益生菌 Pre 段 X 8","4062300232266"},
                {"Combi益生菌 1   段 X 8","4062300155091"},
                {"Combi益生菌 2   段 X 8","4062300155169"},
                {"Combi益生菌 3   段 X 8","4062300155220"},
                {"Combi益生菌 1+  段 X 8","4062300155268"},
                {"Combi益生菌 2+  段 X 8","4062300158429"},
                {"Bio有机 Pre 段 X 8"    ,"4062300295292"},
                {"Bio有机 1   段 X 8"    ,"4062300295322"},
                {"Bio有机 2   段 X 6"    ,"4062300001473"},
                {"Bio有机 3   段 X 6"    ,"4062300001503"},
                {"Bio有机 1+  段 X 6"    ,"4062300001534"}   
        };


        public string GetCode(string productionName)
        {
           var key =  productionName.Trim();

            return dict.GetValue_safe(key,"0");
        }
    }
}
