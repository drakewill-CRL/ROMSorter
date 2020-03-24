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
            File.Move(localFilepath, folderpath + "\\" + newFileName);
        }

        //TODO: rework logic. Extract all files in zip to appropriate folders, including 'unidentified' per file, move original zip files to 'Originals' folder when done.
        static void InnerLoop(string file, string topFolder, bool readZips, bool extractZips, IProgress<string> progress)
        {
            var fi = new FileInfo(file);
            if (progress != null)
                progress.Report(fi.Name);

            var game = Database.FindGame((int)fi.Length, Hasher.HashFile(File.ReadAllBytes(file)));
            if (game != null && game.id != null)
            {
                MoveFile(topFolder + "\\" + game.console, file, game.name);
            }
            else
            {
                //If this is a zip file and wasn't identified, it might contain the game that we want to scan.
                if (readZips && file.EndsWith(".zip"))
                {
                    bool identified = false; //Was a game identified?
                    ZipArchive zf = new ZipArchive(new FileStream(file, FileMode.Open));
                    foreach (var entry in zf.Entries)
                    {
                        if (entry.Length > 0)
                        {
                            var br = new BinaryReader(entry.Open());
                            byte[] data = new byte[(int)entry.Length];
                            br.Read(data, 0, (int)entry.Length); //Exception occurs if length is 0 or negative?
                            var hashes = Hasher.HashFile(data);
                            game = Database.FindGame((int)entry.Length, hashes);
                            if (game != null && game.id != null)
                            {
                                identified = true;
                                if (extractZips)
                                {
                                    Directory.CreateDirectory(topFolder + "\\" + game.console);
                                    entry.ExtractToFile(topFolder + "\\" + game.console + "\\" + game.name, true);
                                }
                                else
                                {
                                    //This might cause issues if a zip has multiple entries but we moved it after finding one file match
                                    zf.Dispose();
                                    MoveFile(topFolder + "\\" + game.console, file, game.name + ".zip");
                                }
                            }
                            else if (extractZips)
                            {
                                Directory.CreateDirectory(topFolder + "\\Unidentified");
                                entry.ExtractToFile(topFolder + "\\Unidentified\\" + entry.Name, true);
                            }
                            data = null;
                            br.Close();
                            br.Dispose();
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
        }
    }
}
