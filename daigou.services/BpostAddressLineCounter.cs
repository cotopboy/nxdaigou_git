using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;

namespace daigou.services
{
    public interface IManualAddressLineCounter
    {
        int GetAddressLine(string recipientName,string DocType);
    }

    public class BpostAddressLineCounter
    {
        private Image imageFile = new Bitmap(1000, 60);
        private Graphics g ;
        private IManualAddressLineCounter manualAddressLineCounter;

        public BpostAddressLineCounter(IManualAddressLineCounter manualAddressLineCounter)
        {
            this.manualAddressLineCounter = manualAddressLineCounter;
            g = Graphics.FromImage(imageFile);
        }

        public int GetBpostCN23AddressLineCount(string bpostCN23Content,string recipientName)
        {
            bool inSensitiveBlock = false;
            var array = bpostCN23Content.ToLines();
            try
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == "To") inSensitiveBlock = true;

                    if (inSensitiveBlock)
                    {
                        string AddressLine = string.Empty;

                        if (array[i + 4].Length > 14) AddressLine = array[i + 4];

                        else if (array[i + 2].Length > 55) AddressLine = array[i + 2];

                        string address = FromAddressLineGetAddress(AddressLine, recipientName);

                        return GetLineCount(address, 190);
                    }
                }
            }
            catch { }

            return manualAddressLineCounter.GetAddressLine(recipientName,"CN23");
        }

        private string FromAddressLineGetAddress(string AddressLine, string recipientName)
        {
            string address = string.Empty;
            if (AddressLine.StartsWith("Importer ref:"))
            {
                int startIndex = AddressLine.IndexOf(":") + 1;
                int length = AddressLine.IndexOf('.') - AddressLine.IndexOf(":") - 1;
                return AddressLine.Substring(startIndex, length).Trim();
            }
            else if (AddressLine.StartsWith("Name Business Street"))
            {
                address = AddressLine.Replace("Name Business Street", "").Trim();
                int nameLength = recipientName.NameToPinyin().Length;
                address = address.Substring(nameLength, address.IndexOf(".") - nameLength).Trim();
                return address;

            }

            return new string('W', 50); 
        }


        public int GetBpostAddressLineCount(string bpostContent, string recipientName)
        {
            bool inSensitiveBlock = false;
            var array = bpostContent.ToLines();

            try
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == "To") inSensitiveBlock = true;

                    if (inSensitiveBlock)
                    {
                        string AddressLine = array[i + 4];
                        string address;
                        if (AddressLine == "PRI NO")
                        {
                            AddressLine = array[i + 2];
                            address = AddressLine.SubStringAfter(":", 1);
                            address = address.Substring(13, address.IndexOf('.') - 12);
                            int nameLength = recipientName.NameToPinyin().Length;
                            address = address.Substring(nameLength, address.IndexOf(".") - nameLength).Trim();
                        }
                        else
                        {
                            address = AddressLine.Substring(0, AddressLine.IndexOf('.'));
                        }


                        return GetLineCount(address, 305);
                    }
                }
            }
            catch { }

            return manualAddressLineCounter.GetAddressLine(recipientName, "国际运单"); ;

        }

        private int GetLineCount(string input, int MaxLineWidth)
        {
            int lineCount = 1;
            int currentLineWidth = 0;
            int currentWordWidth = 0;
            var items = input.Split(new string[]{" ","-"}, StringSplitOptions.None);

            for (int i = 0; i < items.Length; i++)
            {
                currentWordWidth = MeasureItemLength(items[i], g);
                if (currentWordWidth > MaxLineWidth) 
                {
                    lineCount++;
                    continue;
                }
                currentLineWidth += currentWordWidth;

                if (currentLineWidth > MaxLineWidth)
                {
                    lineCount++;
                    currentLineWidth = 0;
                    i--;
                }
            }

            return lineCount;


        }

        private int MeasureItemLength(string input, Graphics g)
        {
            SizeF sizef = g.MeasureString(input, new Font("Arial", 10));
            return (int)sizef.Width;
        }

        public  int GetBpostAddressLineCountEx(string input,bool NeedManReadAddressLineCount,string recipientName,string DocType)
        {
            if (NeedManReadAddressLineCount)
            {
                return manualAddressLineCounter.GetAddressLine(recipientName, DocType); 
            }

            string target = input.SubStringAfter("市", 1).Substring(1).Replace("-",".");

            string targetPy = target.ToPinyin().ToWordFirstCharcterUpperCase().Replace(" ","") + ".";

            int totalLength = MeasureItemLength(targetPy, g);

            return (int)Math.Ceiling(totalLength / 300.0);
        }

        public int GetBpostCN23AddressLineCountEx(string input, bool NeedManReadAddressLineCount, string recipientName, string DocType)
        {
            if (NeedManReadAddressLineCount)
            {
                return manualAddressLineCounter.GetAddressLine(recipientName, DocType);
            }

            string target = input.SubStringAfter("市", 1).Substring(1).Replace("-", ".");

            string targetPy = target.ToPinyin().ToWordFirstCharcterUpperCase().Replace(" ", "") + ".";

            int totalLength = MeasureItemLength(targetPy, g);

            return (int)Math.Ceiling(totalLength / 190.0);
        }
    }
}
