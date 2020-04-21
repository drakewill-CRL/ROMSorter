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

        static ConcurrentBag<string> files = new ConcurrentBag<string>();

        static void EnumerateAllFiles(string topFolder)
        {
            foreach (var file in Directory.EnumerateFiles(topFolder).ToList())
                files.Add(file);
            foreach (var folder in Directory.EnumerateDirectories(topFolder).ToList())
                EnumerateAllFiles(folder);
        }

        public static void ScanFilesOnly(string topFolder, IProgress<string> progress = null)
        {
            //stealing the logic from Sorter, but not doing renames. Only hashing and reporting out found files.
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //Step 1: enumerate all files first.
            files = new ConcurrentBag<string>();
            progress.Report("Scanning for files");
            EnumerateAllFiles(topFolder);
            progress.Report(files.Count() + " files found in " + sw.Elapsed.ToString());
            sw.Restart();

            //Step 2: hash files, looking into zip files
            progress.Report("Hashing files");
            ConcurrentBag<LookupEntry> filesToFind = new ConcurrentBag<LookupEntry>();
            int hashedFileCount = 0;
            Parallel.ForEach(files, (file) =>
            {
                switch (Path.GetExtension(file))
                {
                    case ".zip":
                        var zipResults = Hasher.HashFromZip(file);
                        if (zipResults != null)
                            foreach (var zr in zipResults)
                                filesToFind.Add(zr);
                        break;
                    case ".rar":
                        var rarResults = Hasher.HashFromRar(file);
                        foreach (var rr in rarResults)
                            filesToFind.Add(rr);
                        break;
                    case ".gz":
                    case ".gzip":
                        var gzResults = Hasher.HashFromGzip(file);
                        foreach (var gz in gzResults)
                            filesToFind.Add(gz);
                        break;
                    case ".tar":
                        var tarResults = Hasher.HashFromTar(file);
                        foreach (var tr in tarResults)
                            filesToFind.Add(tr);
                        break;
                    case ".7z":
                        var sevenZResults = Hasher.HashFrom7z(file);
                        foreach (var sz in sevenZResults)
                            filesToFind.Add(sz);
                        break;
                    default:
                        filesToFind.Add(GetFileHashes(file));
                        break;
                }

                hashedFileCount++;
                progress.Report("Hashed " + file + " (" + hashedFileCount + " done so far)");
            });
            progress.Report(filesToFind.Count() + " files hashed in " + sw.Elapsed.ToString());

            //Step 3
            //identify files we found, including zip entries.
            progress.Report("Identifying files");
            sw.Restart();
            int foundCount = 0;
            Parallel.ForEach(filesToFind, (possibleGame) =>
            {
                var gameEntry = Database.FindGame(possibleGame.size, possibleGame.crc, possibleGame.md5, possibleGame.sha1);
                if (gameEntry != null)
                {
                    foundCount++;
                    possibleGame.destinationFileName = gameEntry.console + "\\" + gameEntry.description;
                    possibleGame.console = gameEntry.console;
                    possibleGame.isIdentified = true;
                }
                else
                {
                    var discEntries = Database.FindDisc(possibleGame.size, possibleGame.crc, possibleGame.md5, possibleGame.sha1);
                    if (discEntries.Count > 0)
                    {
                        foreach (var de in discEntries)
                        {
                            foundCount++;
                            possibleGame.destinationFileName = de.console + "\\" + de.name + "\\" + de.description;
                            possibleGame.console = de.console + "\\" + de.name; //Discs treat games as folders
                            possibleGame.isIdentified = true;
                        }
                    }
                }
                if (!String.IsNullOrEmpty(possibleGame.destinationFileName))
                    progress.Report("Identified " + Path.GetFileName(possibleGame.originalFileName) + " as " + Path.GetFileName(possibleGame.destinationFileName));
                else
                    progress.Report("Couldn't identify " + Path.GetFileName(possibleGame.originalFileName));

            });
            progress.Report(foundCount + " files identified out of " + filesToFind.Count() + " in " + sw.Elapsed.ToString());
            sw.Stop();
        }

        static LookupEntry GetFileHashes(string file)
        {
            var hashes = Hasher.HashFile(File.ReadAllBytes(file));
            FileInfo fi = new FileInfo(file);
            LookupEntry le = new LookupEntry();
            le.originalFileName = file;
            le.fileType = LookupEntryType.File;
            le.md5 = hashes[0];
            le.sha1 = hashes[1];
            le.crc = hashes[2];
            le.size = fi.Length;

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
                var game = Database.FindGame((int)fi.Length, Hasher.HashFile(File.ReadAllBytes(file)));
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
