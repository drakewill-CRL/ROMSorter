using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.IO.Compression;
using SharpCompress;

namespace RomDatabase
{
    public static class Sorter
    {
        public static bool moveUnidentified = false;
        public static bool ZipInsteadOfMove = false;
        public static bool UseMultithreading = true;
        public static bool PreserveOriginals = true;
        static ReaderWriterLockSlim lockLock = new ReaderWriterLockSlim();
        static ConcurrentDictionary<string, ReaderWriterLockSlim> locks = new ConcurrentDictionary<string, ReaderWriterLockSlim>();

        public static void SortAllGamesMultithread(string topFolder, string folderToScan, IProgress<string> progress = null)
        {
            var folders = Directory.EnumerateDirectories(folderToScan).ToList();
            foreach (var dir in folders)
            {
                SortAllGamesMultithread(topFolder, dir, progress);
                if (Directory.EnumerateFiles(dir).Count() == 0 && Directory.EnumerateDirectories(dir).Count() == 0)
                    Directory.Delete(dir);
            }

            var files = Directory.EnumerateFiles(folderToScan).ToList(); //not using ToList means it re-scans the Identified/Unidentified folders
            Parallel.ForEach(files, (file) =>
            {
                InnerLoop(file, topFolder, progress);
            });
        }

        public static void SortAllGamesSinglethread(string topFolder, string folderToScan, IProgress<string> progress = null)
        {
            foreach (var dir in Directory.EnumerateDirectories(folderToScan).ToList())
            {
                SortAllGamesSinglethread(topFolder, dir, progress);
                if (Directory.EnumerateFiles(dir).Count() == 0 && Directory.EnumerateDirectories(dir).Count() == 0)
                    Directory.Delete(dir);
            }

            foreach (var file in Directory.EnumerateFiles(folderToScan).ToList())
            {
                InnerLoop(file, topFolder, progress);
            }
        }

        static void MoveFile(string folderpath, string localFilepath, string newFileName)
        {
            Directory.CreateDirectory(folderpath); //does nothing if folder already exists
            if (!File.Exists(folderpath + "\\" + newFileName))
                File.Move(localFilepath, folderpath + "\\" + newFileName);
            else if (localFilepath != folderpath + "\\" + newFileName) //If we scan a directory that's also a destination, don't remove the file that we were going to move onto itself.
                File.Delete(localFilepath);
        }

        static void CopyFile(string folderpath, string localFilepath, string newFileName)
        {
            var fileLock = GetLock(newFileName);
            fileLock.EnterWriteLock();
            Directory.CreateDirectory(folderpath); //does nothing if folder already exists
            if (newFileName.Contains("\\"))
                Directory.CreateDirectory(folderpath + "\\" +  Path.GetDirectoryName(newFileName)); //does nothing if folder already exists
            if (!File.Exists(folderpath + "\\" + newFileName))
                File.Copy(localFilepath, folderpath + "\\" + newFileName);
            fileLock.ExitWriteLock();
        }

        static void InnerLoop(string file, string topFolder, IProgress<string> progress)
        {
            var fi = new FileInfo(file);
            if (progress != null)
                progress.Report(fi.Name);
            var fileLock = GetLock(file);
            fileLock.EnterReadLock();
            var hashes = Hasher.HashFile(File.ReadAllBytes(file));
            fileLock.ExitReadLock();
            var identified = false;

            //Check 1: is this file a single-file game entry?
            var game = Database.FindGame((int)fi.Length, hashes); //find a single-file entry.
            if (game != null)
            {
                identified = true;
                if (ZipInsteadOfMove)
                {
                    CreateSingleZip(file, topFolder + "\\" + game.console + "\\" + game.name + ".zip");
                    System.IO.File.Delete(file);
                }
                else
                    MoveFile(topFolder + "\\" + game.console, file, game.description);
                if (identified) return;
            }

            //check 2: Is this a single entry for a disc (multi-file game)?
            var diskEntry = Database.FindDisc((int)fi.Length, hashes);
            if (diskEntry.Count >= 1)
            {
                identified = true;
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

                    //Create the folder or zip, add entry.
                    if (ZipInsteadOfMove)
                    {
                        HandleMutiFileZip(file, topFolder + "\\" + de.console + "\\" + de.name + ".zip");
                    }
                    else
                        CopyFile(totalFolderPath, file, fileName);//this is where I'm getting my current lock issue. also always in the Unidentified folder.
                }
                //now can remove the original
                if (identified && !PreserveOriginals)
                {
                    File.Delete(file);
                    return;
                }
                if (PreserveOriginals)
                {
                    CopyFile(topFolder + "\\Identified Originals", file, Path.GetFileName(file));
                }
            }

            //Check 3: is this a zip file containing a single-file game entry (or multiple single-file entries?)
            if (file.EndsWith(".zip"))
            {
                identified = handleZipFile(file, topFolder);
                if (identified)
                    return;
            }

            //check 4: rar file, as above.
            if (file.EndsWith(".rar"))
            {
                identified = HandleRarFile(file, topFolder);
                if (identified)
                    return;
            }

            //Check last - we didn't identify it, move if if the option was set.
            if (!identified && moveUnidentified)
            {
                fileLock.EnterWriteLock();
                MoveFile(topFolder + "\\Unidentified", file, System.IO.Path.GetFileName(file));
                fileLock.ExitWriteLock();
            }
        }

        static bool HandleRarFile(string file, string topFolder)
        {
            bool identified = false; //Was a game identified?
            var archive = SharpCompress.Archives.Rar.RarArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = Hasher.HashRarEntry(entry);
                    var game = Database.FindGame((int)entry.Size, ziphashes);
                    if (game != null && game.id != null)
                    {
                        identified = true;
                        Directory.CreateDirectory(topFolder + "\\" + game.console);
                        if (ZipInsteadOfMove)
                        {
                            var fileLock = GetLock("tempZip");
                            fileLock.EnterWriteLock();
                            byte[] fileData = new byte[entry.Size];
                            new BinaryReader(entry.OpenEntryStream()).Read(fileData, 0, (int)entry.Size);
                            File.WriteAllBytes(game.description, fileData);
                            CreateSingleZip(game.description, topFolder + "\\" + game.console + "\\" + game.name + ".zip");
                            File.Delete(game.description);
                            fileLock.ExitWriteLock();
                        }
                        else
                        {
                            if (!File.Exists(topFolder + "\\" + game.console + "\\" + game.description)) //extract fails if file exists.
                            {
                                byte[] filedata = new byte[entry.Size];
                                entry.OpenEntryStream().Read(filedata, 0, (int)entry.Size);
                                System.IO.File.WriteAllBytes(topFolder + "\\" + game.console + "\\" + game.description, filedata);
                            }
                        }
                    }
                    else if (moveUnidentified)
                    {
                        Directory.CreateDirectory(topFolder + "\\Unidentified");
                        byte[] filedata = new byte[entry.Size];
                        entry.OpenEntryStream().Read(filedata, 0, (int)entry.Size);
                        System.IO.File.WriteAllBytes(topFolder + "\\Unidentified\\" + entry.Key, filedata);
                    }
                }
            }
            archive.Dispose();

            if (identified && PreserveOriginals)
                MoveFile(topFolder + "\\Identified Originals", file, System.IO.Path.GetFileName(file));

            return identified;
        }

        static bool handleZipFile(string file, string topFolder)
        {
            bool identified = false; //Was a game identified?
            var fileLock = GetLock(file);
            fileLock.EnterReadLock();
            ZipArchive zf = new ZipArchive(new FileStream(file, FileMode.Open));
            foreach (var entry in zf.Entries)
            {
                if (entry.Length > 0)
                {
                    var ziphashes = Hasher.HashZipEntry(entry);
                    var game = Database.FindGame((int)entry.Length, ziphashes);
                    if (game != null && game.id != null)
                    {
                        identified = true;
                        Directory.CreateDirectory(topFolder + "\\" + game.console);
                        if (ZipInsteadOfMove)
                        {
                            var fileLock2 = GetLock("tempZip");
                            fileLock2.EnterWriteLock();
                            byte[] fileData = new byte[entry.Length];
                            new BinaryReader(entry.Open()).Read(fileData, 0, (int)entry.Length);
                            File.WriteAllBytes(entry.Name, fileData);
                            CreateSingleZip(entry.Name, topFolder + "\\" + game.console + "\\" + game.name + ".zip");
                            fileLock2.ExitWriteLock();
                        }
                        else
                        {
                            if (!File.Exists(topFolder + "\\" + game.console + "\\" + game.description)) //extract fails if file exists.
                            {
                                var unzipLock = GetLock(topFolder + "\\" + game.console + "\\" + game.description);
                                unzipLock.EnterWriteLock();
                                entry.ExtractToFile(topFolder + "\\" + game.console + "\\" + game.description, true);
                                unzipLock.ExitWriteLock();
                            }
                        }
                    }
                    else if (moveUnidentified)
                    {
                        Directory.CreateDirectory(topFolder + "\\Unidentified");
                        var unzipLock = GetLock(topFolder + "\\Unidentified\\" + entry.Name);
                        unzipLock.EnterWriteLock();
                        entry.ExtractToFile(topFolder + "\\Unidentified\\" + entry.Name, true);
                        unzipLock.ExitWriteLock();
                    }
                }
            }
            zf.Dispose();

            if (identified && PreserveOriginals)
                CopyFile(topFolder + "\\Identified Originals", file, System.IO.Path.GetFileName(file));

            fileLock.ExitReadLock();
            return identified;
        }

        static void CreateSingleZip(string file, string zipPath)
        {
            //try
            //{
                Directory.CreateDirectory(Path.GetDirectoryName(zipPath)); //does nothing if folder already exists
                FileStream fs;
                if (!File.Exists(zipPath))
                {
                    fs = new FileStream(zipPath, FileMode.CreateNew);
                }
                else
                    return; //We arent going to handle dupes.

                ZipArchive zf = new ZipArchive(fs, ZipArchiveMode.Create);

                zf.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);

                zf.Dispose();
                fs.Close();
                fs.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    //MOst likely, we were multithreading, and hit 2 files that got identified as the same one. Bail out.
            //}
        }


        static void HandleMutiFileZip(string file, string zipPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(zipPath)); //does nothing if folder already exists. Line below fails if this isn't handled.
            var fileLock = GetLock(zipPath);
            fileLock.EnterWriteLock();
            FileStream fs = new FileStream(zipPath, FileMode.OpenOrCreate);
            ZipArchive zf = new ZipArchive(fs, ZipArchiveMode.Update);

            zf.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);

            zf.Dispose();
            fs.Close();
            fs.Dispose();
            fileLock.ExitWriteLock();
        }

        static ReaderWriterLockSlim GetLock(string file)
        {
            lockLock.EnterUpgradeableReadLock();
            if (locks.Keys.Any(k => k == file))
            {
                lockLock.ExitUpgradeableReadLock();
                return locks[file];
            }

            lockLock.EnterWriteLock();
            ReaderWriterLockSlim newLock = new ReaderWriterLockSlim();
            locks[file] = newLock;
            lockLock.ExitWriteLock();
            lockLock.ExitUpgradeableReadLock();
            return newLock;
        }
    }
}
