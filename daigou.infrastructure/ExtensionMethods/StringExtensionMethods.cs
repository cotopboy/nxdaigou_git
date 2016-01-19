using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;


namespace daigou.infrastructure.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static string ToWordFirstCharcterUpperCase(this string input)
        {
            var list = input.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);


            var doneList = list.Select(x => x.ToFirstCharacterUpperCase());

            return string.Join(" ", doneList);
        }

        public static string AddSquareBracket(this string input)
        {
            return "[{0}]".FormatAs(input);
        }


        public static string MakeRelative(this string filePath, string referencePath)
        {
            var fileUri = new Uri(filePath,true);
            var referenceUri = new Uri(referencePath, true);
            
            return referenceUri.MakeRelativeUri(fileUri).ToString();
        }

       

        public static string RemoveSpaceForNumber(this string value)
        {

            try
            {
                string text = value.Trim();

                List<string> textItem = text.Split(' ').ToList();
                List<string> newTextItem = new List<string>();

                bool isLastItemNumber = false;
                StringBuilder number = new StringBuilder();

                for (int i = 0; i < textItem.Count; i++)
                {
                    if (Char.IsNumber(textItem[i], 0))
                    {
                        number.Append(textItem[i]);
                        isLastItemNumber = true;
                    }
                    else
                    {
                        if (isLastItemNumber)
                        {
                            newTextItem.Add(number.ToString());
                            isLastItemNumber = false;
                            number.Clear();
                            newTextItem.Add(textItem[i]);
                        }
                        else
                        {
                            newTextItem.Add(textItem[i]);
                        }
                    }
                }

                if (isLastItemNumber)
                {
                    newTextItem.Add(number.ToString());
                    number.Clear();
                }


                return string.Join("_", newTextItem);
            }
            catch
            {

            }
            return string.Empty;
        }

    }
}
