using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using SharpCompress;

namespace RomDatabase
{
    public static class Sorter
    {
        public static void SortAllGamesMultithread(string topFolder, string folderToScan, IProgress<string> progress = null, bool ZipInsteadOfMove = false)
        {
            foreach (var dir in Directory.EnumerateDirectories(folderToScan))
            {
                SortAllGamesMultithread(topFolder, dir, progress, ZipInsteadOfMove);
            }

            Parallel.ForEach(Directory.EnumerateFiles(folderToScan), (folder) =>
            {
                InnerLoop(folder, topFolder, progress, ZipInsteadOfMove);
            });
        }

        public static void SortAllGamesSinglethread(string topFolder, string folderToScan, IProgress<string> progress = null, bool ZipInsteadOfMove = false)
        {
            foreach (var dir in Directory.EnumerateDirectories(folderToScan))
            {
                SortAllGamesSinglethread(topFolder, dir, progress, ZipInsteadOfMove);
            }

            foreach (var file in Directory.EnumerateFiles(folderToScan))
            {
                InnerLoop(file, topFolder, progress, ZipInsteadOfMove);
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

        static void CopyFile(string folderpath, string localFilepath, string newFileName)
        {
            Directory.CreateDirectory(folderpath); //does nothing if folder already exists
            if (!File.Exists(folderpath + "\\" + newFileName))
                File.Copy(localFilepath, folderpath + "\\" + newFileName);
            else
                File.Delete(localFilepath);
        }

        //TODO: rework logic again. Use return when it's done with a path to minimize nested conditionals.
        static void InnerLoop(string file, string topFolder, IProgress<string> progress, bool zipInsteadOfMove)
        {
            var fi = new FileInfo(file);
            if (progress != null)
                progress.Report(fi.Name);
            var hashes = Hasher.HashFile(File.ReadAllBytes(file));

            //Check 1: is this file a single-file game entry?
            var game = Database.FindGame((int)fi.Length, hashes); //find a single-file entry.
            if (game != null)
            {
                if (zipInsteadOfMove)
                {
                    CreateSingleZip(file, topFolder + "\\" + game.console + "\\" + game.name);
                }
                else
                    MoveFile(topFolder + "\\" + game.console, file, game.name);

                return;
            }

            //Check 2: is this a zip file containing a single-file game entry (or multiple single-file entries?)
            if (file.EndsWith(".zip"))
            {
                bool identified = false; //Was a game identified?
                ZipArchive zf = new ZipArchive(new FileStream(file, FileMode.Open));
                foreach (var entry in zf.Entries)
                {
                    if (entry.Length > 0)
                    {
                        var ziphashes = Hasher.HashZipEntry(entry);
                        game = Database.FindGame((int)entry.Length, ziphashes);
                        if (game != null && game.id != null)
                        {
                            identified = true;
                            Directory.CreateDirectory(topFolder + "\\" + game.console);
                            if (!File.Exists(topFolder + "\\" + game.console + "\\" + game.name)) //extract fails if file exists.
                                entry.ExtractToFile(topFolder + "\\" + game.console + "\\" + game.name, true);

                        }
                        else
                        {
                            Directory.CreateDirectory(topFolder + "\\Unidentified");
                            entry.ExtractToFile(topFolder + "\\Unidentified\\" + entry.Name, true);
                        }
                    }
                }
                zf.Dispose();
                if (identified)
                    MoveFile(topFolder + "\\Identified Originals", file, System.IO.Path.GetFileName(file));

                return;
            }

            //check 3: Is this a single entry for a disc (multi-file game)? //or should be check 2?
            var diskEntry = Database.FindDisc((int)fi.Length, hashes);
            if (diskEntry != null)
            {
                foreach (var de in diskEntry) //Disc games often share some files. This keeps up from breaking a game by mis-IDing a file for a different version of a game.
                {
                    //Check for sub-folders in the description.
                    var totalFolderPath = topFolder + "\\" + de.console + "\\" + de.name;
                    var fileName = de.description;
                    var fileSubFolder = de.description.Split('/'); //TODO: May need to check for both slashes, not just one.
                    if (fileSubFolder.Count() > 1)
                    {
                        fileName = fileSubFolder[fileSubFolder.Count() - 1];
                        totalFolderPath = totalFolderPath + "\\" + string.Join("\\", fileSubFolder).Replace(fileName, "");
                    }

                    //Create the folder or zip, add entry. TODO check for checkbox status.
                    CopyFile(totalFolderPath, file, fileName);
                }
                //now can remove the original
                File.Delete(file); //TODO: if the folder is empty, delete it?
                //return?
            }

            //Check 4: is this a zip with multiple disc entries? Or is this part of check 2/3?
            //if so, check all the entries, and if its good handle moving it in bulk?
        }

        static void UnRar(string file)
        {
            var archive = SharpCompress.Archives.Rar.RarArchive.Open(file);
            //archive.

        }

        static void CreateSingleZip(string file, string zipPath)
        {
            FileStream fs = new FileStream(zipPath, FileMode.CreateNew);
            ZipArchive zf = new ZipArchive(fs, ZipArchiveMode.Create);

            zf.CreateEntryFromFile(file, Path.GetFileName(file));

            zf.Dispose();
            fs.Close();
            fs.Dispose();
        }

        static void HandleMutiFileZip(string file, string zipPath)
        {
            FileStream fs = new FileStream(zipPath, FileMode.CreateNew);
            ZipArchive zf = new ZipArchive(fs, ZipArchiveMode.Update);

            zf.CreateEntryFromFile(file, Path.GetFileName(file));

            zf.Dispose();
            fs.Close();
            fs.Dispose();
        }
    }
}
