using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Shard.Pinball
{
    class PinballUtils
    {
        private static string highscoresFilePath = Path.Join(Environment.CurrentDirectory, "highscores.dat");
        private static List<Tuple<string, int>> highScores = new();

        static public List<Tuple<string, int>> HighScores
        {
            get { return highScores; }
        }
        static PinballUtils()
        {
                LoadHighscores();
        }
        public static void LoadHighscores()
        {

            string fileContent = "";
            using (FileStream fs = new(highscoresFilePath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    fileContent = sr.ReadToEnd();
                }
            }

            /*
             * parse highscores with format
             * name:score
             * name:score
             * ...
             */

            var lines = fileContent.Split("\r\n");
            foreach (var line in lines)
            {
                var parts = line.Split(":");
                if(parts.Length >= 2)
                {
                    var name = parts[0];
                    int score;

                    if (int.TryParse(parts[1], out score))
                    {
                        highScores.Add(new Tuple<string, int>(name, score));
                    }
                    else
                    {
                        Debug.Log("Couldn't parse score");
                    }
                }
            }
        }
        public static List<Tuple<string, int>> UpdateHighscores(Tuple<string, int> newEntry)
        {
            highScores.Add(newEntry);
            
            return highScores;
        }
        public static void saveHighscores(List<Tuple<string, int>> highscores)
        {
            var fileOutput = string.Join("\r\n", highscores.Select(entry => $"{entry.Item1}:{entry.Item2}"));
            using (StreamWriter sw = new StreamWriter(highscoresFilePath, false /* append = false */) )
            {
                sw.Write(fileOutput);
            }
        }
    }
}
