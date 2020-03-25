using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomDatabase
{
    public class Reporter
    {
        public static void Report(string folder, IProgress<string> p)
        {
            //We're digging through a folder, marking which files are found.
            //Then listing off which entries we don't have for the consoles found.
            //We aren't moving files this time.

            StringBuilder sb = new StringBuilder();
            sb.Append("RomSorter Identification Report");
            sb.Append("Starting at " + DateTime.Now.ToString());
            sb.Append(ScanFolder(folder, p));
            sb.Append("Report completed at " + DateTime.Now.ToString());

            File.WriteAllText(folder + "\\RomSorterReport.txt", sb.ToString());
        }

        public static string ScanFolder(string folder, IProgress<string> p)
        {
            StringBuilder results = new StringBuilder();
            StringBuilder foundFiles = new StringBuilder();
            StringBuilder unknownFiles = new StringBuilder();
            StringBuilder missingGames = new StringBuilder();

            var pathFolders = folder.Split('\\');
            var shortFolder = pathFolders.Last();

            var subfolders = Directory.EnumerateDirectories(folder);
            foreach (var sf in subfolders)
                results.Append(ScanFolder(sf, p));

            p.Report("Scanning " + shortFolder);

            var consoles = Database.GetAllConsoles();
            if (!consoles.Contains(shortFolder))
            {
                //Not a sorted folder, don't scan
                results.Append("Folder " + shortFolder + " is not a RomSorter sorting folder, not scanning.");
                return results.ToString() + Environment.NewLine;
            }

            List<int> gameIDs = new List<int>();

            var filesToScan = Directory.EnumerateFiles(folder);
            foreach (var file in filesToScan)
            {
                var fi = new FileInfo(file);
                var game = Database.FindGame((int)fi.Length, Hasher.HashFile(File.ReadAllBytes(file)));
                if (game != null)
                {
                    foundFiles.Append("Identified " + fi.Name + " as " + game.name + Environment.NewLine);
                    gameIDs.Add(game.id);
                }
                else
                {
                    unknownFiles.Append("Couldn't identify " + file + Environment.NewLine);
                }
            }

            //list all games by console. Mark which ones are missing.
            var lookupIDs = gameIDs.ToLookup(g => g, g => g);
            var consoleGames = Database.GetGamesByConsole(shortFolder);
            foreach(var game in consoleGames)
            {
                if (lookupIDs[game.id] == null || lookupIDs[game.id].Count() == 0)
                {
                    //game is missing
                    missingGames.Append(game.name + " not found in expected folder." + Environment.NewLine);
                }
                else
                {
                }
            }

            p.Report("Finished " + shortFolder);
            return results.ToString() + Environment.NewLine + foundFiles.ToString() + Environment.NewLine + unknownFiles.ToString() + Environment.NewLine + missingGames.ToString(); 
        }
    }
}
