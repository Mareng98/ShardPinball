using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Shard.Pinball
{
    class PinballUtils
    {
        public static List<Tuple<string, int>> LoadHighscores()
        {
            List<Tuple<string, int>> highscores = new();

            string fileContent = "";

            string workDir = Environment.CurrentDirectory;
            string baseDir = Directory.GetParent(workDir).Parent.Parent.Parent.Parent.FullName;

            var filePath = baseDir + "\\Shard\\ConsoleApp1\\Pinball\\pinball_highscores.dat";
            using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    fileContent = sr.ReadToEnd();
                }
            }

            var fileContentLines = fileContent.Split("\r\n");
            foreach (var line in fileContentLines)
            {
                var parts = line.Split(":");
                var name = parts[0];
                int score;

                if (int.TryParse(parts[1], out score))
                {
                    highscores.Add(new Tuple<string, int>(name, score));
                }
                else
                {
                    Debug.Log("Couldn't parse score");
                }
            }

            return highscores;
        }
        public static List<Tuple<string, int>> UpdateHighscores(List<Tuple<string, int>> highscores, Tuple<string, int> newEntry)
        {
            List<Tuple<string, int>> updatedHighscores = new();
            bool copyRest = false;

            var newScore = newEntry.Item2;
            for (int i = 0; i < highscores.Count; i++)
            {
                var score = highscores[i].Item2;
                if (score < newScore && !copyRest)
                {
                    updatedHighscores.Add(newEntry);
                    copyRest = true;
                }
                else
                {
                    updatedHighscores.Add(highscores[i]);
                }
            }
            return updatedHighscores;
        }
        public static void saveHighscores(List<Tuple<string, int>> highscores)
        {
            string workDir = Environment.CurrentDirectory;
            string baseDir = Directory.GetParent(workDir).Parent.Parent.Parent.Parent.FullName;

            var filePath = baseDir + "\\Shard\\ConsoleApp1\\Pinball\\pinball_highscores.dat";

            //StringBuilder sb = new();

//            foreach (var entry in highscores)
//            {
//                sb.Append(entry.Item1 + ":" +  entry.Item2 + "\r\n");
//            }

            var fileOutput = string.Join("\r\n", highscores.Select(entry => $"{entry.Item1}:{entry.Item2}"));
//            var fileOutput = sb.ToString();

            using (StreamWriter sw = new StreamWriter(filePath, false) )
            {
                sw.Write(fileOutput);
            }
        }
    }
}
