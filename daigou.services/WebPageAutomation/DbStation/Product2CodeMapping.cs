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
                {"Ap 爱他美 Pre 段 X 6"  ,"2" },
                {"Ap 爱他美 1   段 X 6"  ,"8" },
                {"Ap 爱他美 2   段 X 6"  ,"6" },
                {"Ap 爱他美 3   段 X 6"  ,"4" },
                {"Ap 爱他美 1+  段 X 8"  ,"11"},
                {"Ap 爱他美 2+  段 X 8"  ,"10"},
                {"Combi益生菌 Pre 段 X 8","26"},
                {"Combi益生菌 1   段 X 8","27"},
                {"Combi益生菌 2   段 X 8","28"},
                {"Combi益生菌 3   段 X 8","29"},
                {"Combi益生菌 1+  段 X 8","30"},
                {"Combi益生菌 2+  段 X 8","31"},
                {"Bio有机 Pre 段 X 8"    ,"21"},
                {"Bio有机 1   段 X 8"    ,"22"},
                {"Bio有机 2   段 X 6"    ,"23"},
                {"Bio有机 3   段 X 6"    ,"24"},
                {"Bio有机 1+  段 X 6"    ,"25"}   
        };


        public string GetCode(string productionName)
        {
           var key =  productionName.Trim();

            return dict.GetValue_safe(key,"0");
        }
    }
}
