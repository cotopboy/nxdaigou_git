using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using System.IO;
using Utilities.IO;

namespace daigou.infrastructure.ExtensionMethods
{
    public static class StringPinyinExtensionMethods
    {
        private static Dictionary<char, string> specailList = new Dictionary<char, string>();
        

        static StringPinyinExtensionMethods()
        {
            FileInfo file = new FileInfo(DirectoryHelper.CombineWithCurrentExeDir("pingyin_extension.txt"));
            string content = file.Read();
            var lines = content.ToLines();

            foreach (var item in lines)
            {
                var array = item.Split(',');

                if (array.Length == 2)
                    specailList.Add(array[0][0], array[1]);
            }
        }


        public static string NameToPinyin(this string Name)
        {
            string firstName = Name.Substring(0, 1).ToPinyin().ToFirstCharacterUpperCase();
            string lastName = Name.Substring(1).ToPinyin().ToFirstCharacterUpperCase().Replace(" ", "");

            return string.Format("{0} {1}", firstName, lastName);
        }

        public static string NameToPinyinEMSFormat(this string Name)
        {            
            return Name.ToPinyin().Replace(" ","");
        }

        public static string GetPinyinInitials(this string inputText)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var item in inputText)
            {
                if (item == ' ' || item == '　') continue;
                if (specailList.ContainsKey(item))
                {
                    builder.Append(specailList[item].Substring(0,1).ToUpper());
                }
                else
                {
                    builder.Append(NPinyin.Pinyin.GetInitials(item.ToString()));
                }
            }

            return builder.ToString().Trim();
        }

        public static string ToPinyin(this string inputText)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var item in inputText)
            {
                if (item == ' ' || item == '　') continue;
                if (specailList.ContainsKey(item))
                {
                    builder.Append(specailList[item] + " ");
                }
                else
                {
                    builder.Append(NPinyin.Pinyin.GetPinyin(item) + " ");
                }
            }

            return builder.ToString().Trim();

        }
    }
}
