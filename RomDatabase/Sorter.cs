using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace RomDatabase
{
    public static class Sorter
    {
        public static void SortAllGamesMultithread(string topFolder, string folderToScan, bool readZips = true, bool extractZips = true, IProgress<string> progress = null)
        {
            foreach (var dir in Directory.EnumerateDirectories(folderToScan))
            {
                SortAllGamesMultithread(topFolder, dir, readZips, extractZips, progress);
            }

            Parallel.ForEach(Directory.EnumerateFiles(folderToScan), (folder) =>
            {
                InnerLoop(folder, topFolder, readZips, extractZips, progress);
            });
        }

        public static void SortAllGamesSinglethread(string topFolder, string folderToScan, bool readZips = true, bool extractZips = true, IProgress<string> progress = null)
        {
            foreach (var dir in Directory.EnumerateDirectories(folderToScan))
            {
                SortAllGamesSinglethread(topFolder, dir, readZips, extractZips);
            }

            foreach (var file in Directory.EnumerateFiles(folderToScan))
            {
                InnerLoop(file, topFolder, readZips, extractZips, progress);
            }
        }

        static void MoveFile(string folderpath, string localFilepath, string newFileName)
        {
            Directory.CreateDirectory(folderpath); //does nothing if folder already exists
            if (!File.Exists(folderpath + "\\" + newFileName))
                File.Move(localFilepath, folderpath + "\\" + newFileName);
            else
                File.Delete(localFilepath);
        }

        //TODO: rework logic again. Use return when it's done with a path to minimize nested conditionals.
        static void InnerLoop(string file, string topFolder, bool readZips, bool extractZips, IProgress<string> progress)
        {
            var fi = new FileInfo(file);
            if (progress != null)
                progress.Report(fi.Name);

            //Check 1: is this file a single-file game entry?
            var game = Database.FindGame((int)fi.Length, Hasher.HashFile(File.ReadAllBytes(file))); //find a single-file entry.
            if (game != null && game.id != null)
            {
                MoveFile(topFolder + "\\" + game.console, file, game.name);
                return;
            }

            //Check 2: is this a zip file containing a single-file game entry (or multiple single-file entries?)
            if (readZips && file.EndsWith(".zip"))
            {
                bool identified = false; //Was a game identified?
                ZipArchive zf = new ZipArchive(new FileStream(file, FileMode.Open));
                foreach (var entry in zf.Entries)
                {
                    if (entry.Length > 0)
                    {
                        var hashes = Hasher.HashZipEntry(entry);
                        game = Database.FindGame((int)entry.Length, hashes);
                        if (game != null && game.id != null)
                        {
                            identified = true;
                            if (extractZips)
                            {
                                Directory.CreateDirectory(topFolder + "\\" + game.console);
                                if (!File.Exists(topFolder + "\\" + game.console + "\\" + game.name)) //extract fails if file exists.
                                    entry.ExtractToFile(topFolder + "\\" + game.console + "\\" + game.name, true);
                            }
                            else
                            {
                                //We are reading but aren't extracting zips but we did identify at least one game. Stop looking for other games. This is handled outside of the loop
                                //zf.Dispose();
                                //MoveFile(topFolder + "\\" + game.console, file, game.name + ".zip");
                                break; //TODO ensure this breaks only the foreach.
                            }
                        }
                        else if (extractZips)
                        {
                            Directory.CreateDirectory(topFolder + "\\Unidentified");
                            entry.ExtractToFile(topFolder + "\\Unidentified\\" + entry.Name, true);
                        }
                    }
                }
                zf.Dispose();
                if (identified)
                    MoveFile(topFolder + "\\Identified Originals", file, System.IO.Path.GetFileName(file));
                else
                    MoveFile(topFolder + "\\Unidentified Originals", file, Path.GetFileName(file));
            }
            else
                MoveFile(topFolder + "\\Unidentified Originals", file, Path.GetFileName(file));

        }

        static void ScanMultiFile()
        {
            //
        }
    }
}
