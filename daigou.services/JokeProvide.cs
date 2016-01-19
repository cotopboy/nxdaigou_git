using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Utilities.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.domain;
using Utilities.Configuration;

namespace daigou.services
{
    public class JokeProvide
    {
        private static string[] lines = null;

        private readonly int lineLength = 48;

        private int iniIndex = DateTime.Now.Millisecond;

        private ConfigurationService confiService;

        private int jokeIndex = 1;

        private AppStatusManager status;

        public JokeProvide(AppStatusManager status)
        {
            this.status = status;

        }

        public string GiveMeAJoke()
        {
            if (lines == null)
            {
                FileInfo fileInfo = new FileInfo(DirectoryHelper.CombineWithCurrentExeDir("joke.txt"));

                lines = fileInfo.Read().ToLines();
            }

            jokeIndex = status.Get("joke_index","0").StrToInt() + 1;

            if (jokeIndex >= lines.Length) jokeIndex = 0;

            status.AddOrUpdate("joke_index", jokeIndex.ToString());


            status.SaveToFile();

            return lines[jokeIndex];
        }

        public string[] GetMeFormattedJoke()
        {
            List<string> listString = new List<string>();

            listString.Add("附上一笑话，祝您工作愉快,天天开心  ‵（*∩_∩*）′");
            string joke = this.GiveMeAJoke();

            int line = joke.Length / lineLength + 1;

            for (int i = 0; i < line; i++)
            {
                if ((i * lineLength + lineLength) > joke.Length)
                {
                    listString.Add(joke.Substring(i * lineLength, joke.Length % lineLength));
                }
                else
                {
                    listString.Add(joke.Substring(i * lineLength, lineLength));
                }
            }

            return listString.ToArray();
        }
    }
}
