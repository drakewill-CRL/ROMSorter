using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomDatabase5
{
    public class Reporter //a straight port from the original version. Should re-work this entirely.
    {
        static Hasher hasher = new Hasher();
        public static void Report(string folder, IProgress<string> p, bool multithread)
        {
            //We're digging through a folder, marking which files are found.
            //Then listing off which entries we don't have for the consoles found.
            //We aren't moving files this time.

            StringBuilder sb = new StringBuilder();
            sb.Append("RomSorter Identification Report" + Environment.NewLine);
            sb.Append("Starting at " + DateTime.Now.ToString() + Environment.NewLine);
            sb.Append(ScanFolder(folder, p, multithread) + Environment.NewLine);
            sb.Append("Report completed at " + DateTime.Now.ToString() + Environment.NewLine);

            File.WriteAllText(folder + "\\RomSorterReport.txt", sb.ToString());
        }

        static LookupEntry GetFileHashes(string file)
        {
            byte[] fileData = File.ReadAllBytes(file);
            var hashes = hasher.HashFile(ref fileData);
            FileInfo fi = new FileInfo(file);
            LookupEntry le = new LookupEntry();
            le.originalFileName = file;
            le.fileType = LookupEntryType.File;
            le.md5 = hashes[0];
            le.sha1 = hashes[1];
            le.crc = hashes[2];
            le.size = fi.Length;
            fileData = null;

            return le;
        }

        public static string ScanFolder(string folder, IProgress<string> p, bool multithread)
        {
            StringBuilder results = new StringBuilder();
            StringBuilder foundFiles = new StringBuilder();
            StringBuilder unknownFiles = new StringBuilder();
            StringBuilder missingGames = new StringBuilder();

            var pathFolders = folder.Split('\\');
            var shortFolder = pathFolders.Last();

            var subfolders = Directory.EnumerateDirectories(folder);
            if (multithread)
            {
                Parallel.ForEach(subfolders, (sf) => { results.Append(ScanFolder(sf, p, multithread)); });
            }
            else
                foreach (var sf in subfolders)
                    results.Append(ScanFolder(sf, p, multithread));

            p.Report("Scanning " + shortFolder);

            var consoles = Database.GetAllConsoles();
            if (!consoles.Contains(shortFolder))
            {
                //Not a sorted folder, don't scan
                results.AppendLine("Folder " + shortFolder + " is not a RomSorter sorting folder, not scanning.");
                return results.ToString() + Environment.NewLine;
            }

            results.AppendLine("Scanning " + shortFolder);
            List<int> gameIDs = new List<int>();

            var filesToScan = Directory.EnumerateFiles(folder);
            foreach (var file in filesToScan)
            {
                var fi = new FileInfo(file);
                byte[] fileData = File.ReadAllBytes(file);
                var game = Database.FindGame((int)fi.Length, hasher.HashFile(ref fileData));
                if (game != null && game.id != null)
                {
                    foundFiles.AppendLine("Identified " + fi.Name + " as " + game.name);
                    gameIDs.Add(game.id);
                }
                else
                {
                    unknownFiles.AppendLine("Couldn't identify " + file);
                }
            }

            //list all games by console. Mark which ones are missing.
            missingGames.Append("Missing Games for " + shortFolder + ":" + Environment.NewLine);
            var lookupIDs = gameIDs.ToLookup(g => g, g => g);
            var consoleGames = Database.GetGamesByConsole(shortFolder);
            foreach (var game in consoleGames)
            {
                if (lookupIDs[game.id].Count() == 0)
                {
                    //game is missing
                    missingGames.Append(game.name + Environment.NewLine);
                }
                else
                {
                    //game found, nothing to report.
                }
            }

            results.AppendLine("Found: " + gameIDs.Count());
            results.AppendLine("Missing: " + (consoleGames.Count() - gameIDs.Count()));

            p.Report("Finished " + shortFolder);
            return results.ToString() + Environment.NewLine + foundFiles.ToString() + Environment.NewLine + unknownFiles.ToString() + Environment.NewLine + missingGames.ToString();
        }
    }
}
