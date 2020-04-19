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
using RomSorter.Data;
using RomDatabase.Data;

namespace RomDatabase
{
    public static class Sorter
    {
        //options
        public static bool moveUnidentified = false;
        public static bool ZipInsteadOfMove = false;
        public static bool UseMultithreading = true;
        public static bool PreserveOriginals = true;

        //internal stuff.
        static ReaderWriterLockSlim lockLock = new ReaderWriterLockSlim();
        static ConcurrentDictionary<string, ReaderWriterLockSlim> locks = new ConcurrentDictionary<string, ReaderWriterLockSlim>();
        static string tempFolderPath = Path.GetTempPath();

        static ConcurrentBag<string> files = new ConcurrentBag<string>();

        static int filesMovedOrExtracted = 0;
        static int filesToReportBetween = 1000; //set to 1% of the workload or 1000 files during sorting to keep the user aware that its doing work.

        public static void EnumerateAllFiles(string topFolder)
        {
            foreach (var file in Directory.EnumerateFiles(topFolder).ToList())
                files.Add(file);
            foreach (var folder in Directory.EnumerateDirectories(topFolder).ToList())
                EnumerateAllFiles(folder);
        }

        public static LookupEntry GetFileHashes(string file)
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

        public static List<LookupEntry> AltZipProcess(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            ZipArchive zf = new ZipArchive(new FileStream(file, FileMode.Open));
            foreach (var entry in zf.Entries)
            {
                if (entry.Length > 0)
                {
                    var ziphashes = Hasher.HashZipEntry(entry);
                    if (ziphashes != null) //is null if the zip file entry couldn't be read.
                    {
                        LookupEntry le = new LookupEntry();
                        le.fileType = LookupEntryType.ZipEntry;
                        le.originalFileName = file;
                        le.entryPath = entry.FullName;
                        le.crc = ziphashes[2];
                        le.sha1 = ziphashes[1];
                        le.md5 = ziphashes[0];
                        le.size = entry.Length;
                        zippedFiles.Add(le);
                    }
                }
            }
            zf.Dispose();
            return zippedFiles.Count > 0 ? zippedFiles : null;
        }

        public static List<LookupEntry> AltRarProcess(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.Rar.RarArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = Hasher.HashRarEntry(entry);
                    LookupEntry le = new LookupEntry();
                    le.fileType = LookupEntryType.RarEntry;
                    le.originalFileName = file;
                    le.entryPath = entry.Key;
                    le.crc = ziphashes[2];
                    le.sha1 = ziphashes[1];
                    le.md5 = ziphashes[0];
                    le.size = entry.Size;
                    zippedFiles.Add(le);
                }
            }
            archive.Dispose();
            return zippedFiles.Count > 0 ? zippedFiles : null;
        }

        public static List<LookupEntry> AltTarProcess(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.Tar.TarArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = Hasher.HashArchiveEntry(entry);
                    LookupEntry le = new LookupEntry();
                    le.fileType = LookupEntryType.TarEntry;
                    le.originalFileName = file;
                    le.entryPath = entry.Key;
                    le.crc = ziphashes[2];
                    le.sha1 = ziphashes[1];
                    le.md5 = ziphashes[0];
                    le.size = entry.Size;
                    zippedFiles.Add(le);
                }
            }
            archive.Dispose();
            return zippedFiles.Count > 0 ? zippedFiles : null;
        }

        public static List<LookupEntry> Alt7zProcess(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = Hasher.HashArchiveEntry(entry);
                    LookupEntry le = new LookupEntry();
                    le.fileType = LookupEntryType.SevenZEntry;
                    le.originalFileName = file;
                    le.entryPath = entry.Key;
                    le.crc = ziphashes[2];
                    le.sha1 = ziphashes[1];
                    le.md5 = ziphashes[0];
                    le.size = entry.Size;
                    zippedFiles.Add(le);
                }
            }
            archive.Dispose();
            return zippedFiles.Count > 0 ? zippedFiles : null;
        }

        public static List<LookupEntry> AltGZipProcess(string file)
        {
            List<LookupEntry> zippedFiles = new List<LookupEntry>();
            var archive = SharpCompress.Archives.GZip.GZipArchive.Open(file);
            foreach (var entry in archive.Entries)
            {
                if (entry.Size > 0)
                {
                    var ziphashes = Hasher.HashArchiveEntry(entry);
                    LookupEntry le = new LookupEntry();
                    le.fileType = LookupEntryType.GZipEntry;
                    le.originalFileName = file;
                    le.entryPath = entry.Key;
                    le.crc = ziphashes[2];
                    le.sha1 = ziphashes[1];
                    le.md5 = ziphashes[0];
                    le.size = entry.Size;
                    zippedFiles.Add(le);
                }
            }
            archive.Dispose();
            return zippedFiles.Count > 0 ? zippedFiles : null;
        }

        public static void TestAlternatePath(string topFolder, string destinationFolder, IProgress<string> progress = null)
        {
            filesMovedOrExtracted = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //Step 1: enumerate all files first.
            files = new ConcurrentBag<string>();
            progress.Report("Scanning for files");
            EnumerateAllFiles(topFolder);
            progress.Report(files.Count() + " files found in " + sw.Elapsed.ToString());
            sw.Restart();

            filesToReportBetween = files.Count() / 100;
            if (filesToReportBetween > 1000)
                filesToReportBetween = 1000;
            if (filesToReportBetween < 1)
                filesToReportBetween = 1;

            //Step 2: hash files, looking into zip files
            progress.Report("Hashing files");
            ConcurrentBag<LookupEntry> filesToFind = new ConcurrentBag<LookupEntry>();
            int hashedFileCount = 0;
            Parallel.ForEach(files, (file) =>
            {
                switch (Path.GetExtension(file))
                {
                    case ".zip":
                        var zipResults = AltZipProcess(file);
                        if (zipResults != null)
                            foreach (var zr in zipResults)
                                filesToFind.Add(zr);
                        break;
                    case ".rar":
                        var rarResults = AltRarProcess(file);
                        foreach (var rr in rarResults)
                            filesToFind.Add(rr);
                        break;
                    case ".gz":
                        var gzResults = AltGZipProcess(file);
                        foreach (var gz in gzResults)
                            filesToFind.Add(gz);
                        break;
                    case ".tar":
                        var tarResults = AltTarProcess(file);
                        foreach (var tr in tarResults)
                            filesToFind.Add(tr);
                        break;
                    case ".7z":
                        var sevenZResults = Alt7zProcess(file);
                        foreach (var sz in sevenZResults)
                            filesToFind.Add(sz);
                        break;
                    default:
                        filesToFind.Add(GetFileHashes(file));
                        break;
                }

                hashedFileCount++;
                if (hashedFileCount % filesToReportBetween == 0)
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
                    possibleGame.destinationFileName = destinationFolder + "\\" + gameEntry.console + "\\" + gameEntry.description;
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
                            possibleGame.destinationFileName = destinationFolder + "\\" + de.console + "\\" + de.name + "\\" + de.description;
                            possibleGame.console = de.console + "\\" + de.name; //Discs treat games as folders
                            possibleGame.isIdentified = true;
                        }
                    }
                }
                if (foundCount % filesToReportBetween == 0)
                    progress.Report("Identified " + Path.GetFileName(possibleGame.originalFileName) + " as" + Path.GetFileName(possibleGame.destinationFileName) + "(" + foundCount + " done so far)");

            });
            progress.Report(foundCount + " files identified in " + sw.Elapsed.ToString());
            sw.Restart();

            //step 4
            //start moving files. Requires a little bit of organizing in case a zip has multiple files.
            var unidentified = filesToFind.Where(f => !f.isIdentified).ToList();
            var foundFiles = filesToFind.Where(f => f.isIdentified).ToList();
            var plainFiles = foundFiles.Where(f => f.fileType == LookupEntryType.File).ToList();
            var zippedFiles = foundFiles.Where(f => f.fileType == LookupEntryType.ZipEntry).GroupBy(f => f.originalFileName).ToList();
            var raredFiles = foundFiles.Where(f => f.fileType == LookupEntryType.RarEntry).GroupBy(f => f.originalFileName).ToList();
            var taredFiles = foundFiles.Where(f => f.fileType == LookupEntryType.TarEntry).GroupBy(f => f.originalFileName).ToList();
            var sevenZippedFiles = foundFiles.Where(f => f.fileType == LookupEntryType.SevenZEntry).GroupBy(f => f.originalFileName).ToList();
            var gZippedFiles = foundFiles.Where(f => f.fileType == LookupEntryType.GZipEntry).GroupBy(f => f.originalFileName).ToList();
            progress.Report(unidentified.Count() + " files not identified in source folder.");

            progress.Report("Beginning file move/zip operations");
            sw.Restart();
            //Create all needed directories now, instead of attempting for each file.
            var dirsToMake = foundFiles.Select(f => f.console).Distinct().ToList();
            if (moveUnidentified)
                dirsToMake.Add("Unidentified");
            foreach (var dir in dirsToMake)
                Directory.CreateDirectory(destinationFolder + "\\" + dir);

            //TODO: might be able to use interfaces to cut these down all the SharpCompress paths to 1 function instead of 6.
            var plainFilesTask = Task.Factory.StartNew(() => HandlePlainFiles(plainFiles));
            var zipFilesTask = Task.Factory.StartNew(() => HandleZipEntries(zippedFiles));
            var rarFilesTask = Task.Factory.StartNew(() => HandleRarEntries(raredFiles));
            var tarFilesTask = Task.Factory.StartNew(() => HandleTarEntries(taredFiles));
            var sevenZipFilesTask = Task.Factory.StartNew(() => Handle7zEntries(sevenZippedFiles));
            var gZipFilesTask = Task.Factory.StartNew(() => HandleGZipEntries(gZippedFiles));
            var unidentifiedTask = Task.Factory.StartNew(() => HandleUnidentifiedFiles(unidentified, destinationFolder + "\\Unidentified\\"));

            Task.WaitAll(plainFilesTask, zipFilesTask, rarFilesTask, tarFilesTask, sevenZipFilesTask, gZipFilesTask, unidentifiedTask);
            progress.Report(filesMovedOrExtracted + " files moved or extracted in " + sw.Elapsed.ToString());
            sw.Restart();

            //step 5
            //clean up. Remove empty folders?
            CleanupLoop(topFolder);
            progress.Report("Empty folders removed from source directory in " + sw.Elapsed.ToString());
            sw.Stop();
        }

        public static void CleanupLoop(string folder)
        {
            foreach (var subfolder in Directory.EnumerateDirectories(folder))
                CleanupLoop(subfolder);

            if (Directory.EnumerateFiles(folder).Count() == 0 && Directory.EnumerateDirectories(folder).Count() == 0)
                Directory.Delete(folder);
        }

        public static void HandleUnidentifiedFiles(List<LookupEntry> unidentifiedFiles, string unidentifiedFolder)
        {
            if (!moveUnidentified)
                return;

            Parallel.ForEach(unidentifiedFiles.GroupBy(u => u.originalFileName), (uf) => //If we refer to the original file more than once, EX because it's a zip file, we only want to move it once.
            {    
                File.Move(uf.Key, unidentifiedFolder + Path.GetFileName(uf.Key));
                filesMovedOrExtracted++;
            });
        }

        public static void HandlePlainFiles(List<LookupEntry> plainFiles)
        {
            Parallel.ForEach(plainFiles, (pf) =>
            {
                if (pf.originalFileName == pf.destinationFileName) //dont bother moving a file onto itself.
                    return;

                if (ZipInsteadOfMove)
                {
                    CreateSingleZip(pf.originalFileName, pf.destinationFileName + ".zip");
                }
                else
                {
                    if (!File.Exists(pf.destinationFileName))
                        File.Move(pf.originalFileName, pf.destinationFileName);
                }

                if (!PreserveOriginals)
                    File.Delete(pf.originalFileName);

                filesMovedOrExtracted++;
            });
        }

        public static void HandleZipEntries(List<IGrouping<string, LookupEntry>> zippedFiles)
        {
            Parallel.ForEach(zippedFiles, (zf) =>
            {
                var zipFile = ZipFile.OpenRead(zf.Key); //might have multiple files to extract from a zip, thats why these are grouped.
                foreach (var entryToFind in zf)
                {
                    if (!File.Exists(entryToFind.destinationFileName))
                    {
                        var entry = zipFile.Entries.Where(e => e.FullName == entryToFind.entryPath).FirstOrDefault();
                        entry.ExtractToFile(entryToFind.destinationFileName);
                    }
                    filesMovedOrExtracted++;
                }
                zipFile.Dispose();

                if(!PreserveOriginals)
                    File.Delete(zf.Key);
            });
        }

        public static void HandleRarEntries(List<IGrouping<string, LookupEntry>> raredFiles)
        {
            Parallel.ForEach(raredFiles, (rf) =>
            {
                var rarFile = SharpCompress.Archives.Rar.RarArchive.Open(rf.Key);//might have multiple files to extract from a zip, thats why these are grouped.
                foreach (var entryToFind in rf)
                {
                    if (!File.Exists(entryToFind.destinationFileName))
                    {
                        var entry = rarFile.Entries.Where(e => e.Key == entryToFind.entryPath).FirstOrDefault();
                        byte[] fileData = new byte[entry.Size];
                        new BinaryReader(entry.OpenEntryStream()).Read(fileData, 0, (int)entry.Size);
                        File.WriteAllBytes(entryToFind.destinationFileName, fileData);
                    }
                    filesMovedOrExtracted++;
                }
                rarFile.Dispose();

                if (!PreserveOriginals)
                    File.Delete(rf.Key);
            });
        }

        public static void Handle7zEntries(List<IGrouping<string, LookupEntry>> sevenZdFiles)
        {
            Parallel.ForEach(sevenZdFiles, (sz) =>
            {
                var sevenZFile = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(sz.Key);//might have multiple files to extract from a zip, thats why these are grouped.
                foreach (var entryToFind in sz)
                {
                    if (!File.Exists(entryToFind.destinationFileName))
                    {
                        var entry = sevenZFile.Entries.Where(e => e.Key == entryToFind.entryPath).FirstOrDefault();
                        byte[] fileData = new byte[entry.Size];
                        new BinaryReader(entry.OpenEntryStream()).Read(fileData, 0, (int)entry.Size);
                        File.WriteAllBytes(entryToFind.destinationFileName, fileData);
                    }
                    filesMovedOrExtracted++;
                }
                sevenZFile.Dispose();

                if (!PreserveOriginals)
                    File.Delete(sz.Key);
            });
        }

        public static void HandleGZipEntries(List<IGrouping<string, LookupEntry>> gZippedFiles)
        {
            Parallel.ForEach(gZippedFiles, (gz) =>
            {
                var gzFile = SharpCompress.Archives.GZip.GZipArchive.Open(gz.Key);//might have multiple files to extract from a zip, thats why these are grouped.
                foreach (var entryToFind in gz)
                {
                    if (!File.Exists(entryToFind.destinationFileName))
                    {
                        var entry = gzFile.Entries.Where(e => e.Key == entryToFind.entryPath).FirstOrDefault();
                        byte[] fileData = new byte[entry.Size];
                        new BinaryReader(entry.OpenEntryStream()).Read(fileData, 0, (int)entry.Size);
                        File.WriteAllBytes(entryToFind.destinationFileName, fileData);
                    }
                    filesMovedOrExtracted++;
                }
                gzFile.Dispose();

                if (!PreserveOriginals)
                    File.Delete(gz.Key);
            });
        }

        public static void HandleTarEntries(List<IGrouping<string, LookupEntry>> taredFiles)
        {
            Parallel.ForEach(taredFiles, (tf) =>
            {
                var tarFile = SharpCompress.Archives.Tar.TarArchive.Open(tf.Key);//might have multiple files to extract from a zip, thats why these are grouped.
                foreach (var entryToFind in tf)
                {
                    if (!File.Exists(entryToFind.destinationFileName))
                    {
                        var entry = tarFile.Entries.Where(e => e.Key == entryToFind.entryPath).FirstOrDefault();
                        byte[] fileData = new byte[entry.Size];
                        new BinaryReader(entry.OpenEntryStream()).Read(fileData, 0, (int)entry.Size);
                        File.WriteAllBytes(entryToFind.destinationFileName, fileData);
                    }
                    filesMovedOrExtracted++;
                }
                tarFile.Dispose();

                if (!PreserveOriginals)
                    File.Delete(tf.Key);
            });
        }

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
            //Sanity check - if we would move this file to it's current location, don't. (EX: scanning a folder named Unidentified and finding an unidentified file)
            if (localFilepath == folderpath + "\\" + newFileName)
                return;

            Directory.CreateDirectory(folderpath); //does nothing if folder already exists
            if (!File.Exists(folderpath + "\\" + newFileName))
                File.Move(localFilepath, folderpath + "\\" + newFileName);
            else if (localFilepath != folderpath + "\\" + newFileName) //If we scan a directory that's also a destination, don't remove the file that we were going to move onto itself.
                File.Delete(localFilepath);
        }

        static void CopyFile(string folderpath, string localFilepath, string newFileName)
        {
            //Sanity check - if we would copy this file to it's current location, don't. (EX: scanning a folder named Unidentified and finding an unidentified file)
            if (localFilepath == folderpath + "\\" + newFileName)
                return;

            ReaderWriterLockSlim fileLock;
            GetLock(folderpath + "\\" + newFileName, out fileLock);
            fileLock.EnterWriteLock();
            Directory.CreateDirectory(folderpath); //does nothing if folder already exists
            if (newFileName.Contains("\\"))
                Directory.CreateDirectory(folderpath + "\\" + Path.GetDirectoryName(newFileName)); //does nothing if folder already exists
            if (!File.Exists(folderpath + "\\" + newFileName))
                File.Copy(localFilepath, folderpath + "\\" + newFileName);
            fileLock.ExitWriteLock();
        }

        static void InnerLoop(string file, string topFolder, IProgress<string> progress)
        {
            var fi = new FileInfo(file);
            if (progress != null)
                progress.Report(fi.Name);
            ReaderWriterLockSlim fileLock;
            GetLock(file, out fileLock);
            fileLock.EnterReadLock();
            var hashes = Hasher.HashFile(File.ReadAllBytes(file));
            fileLock.ExitReadLock();
            var identified = false;

            //Check 1: is this file a single-file game entry?
            var game = Database.FindGame((int)fi.Length, hashes); //find a single-file entry.
            if (game != null)
            {
                //Sanity check - if we would move this file to it's current location, don't. (EX: scanning a folder named Unidentified and finding an unidentified file)
                if (file == topFolder + "\\" + game.console + "\\" + game.name + ".zip" || file == topFolder + "\\" + game.console + "\\" + game.description)
                    return;

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
                    {
                        CopyFile(totalFolderPath, file, fileName);
                    }
                }
                //now can remove the original
                if (identified && !PreserveOriginals)
                {
                    fileLock.EnterWriteLock();
                    File.Delete(file);
                    fileLock.ExitWriteLock();
                    return;
                }
                else if (PreserveOriginals)
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

            if (!PreserveOriginals) //not identified, 
            {
                fileLock.EnterWriteLock();
                File.Delete(file);
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
                            string tempFilePath = tempFolderPath + game.description;
                            ReaderWriterLockSlim fileLock = null;
                            GetLock(tempFilePath, out fileLock);
                            fileLock.EnterWriteLock();
                            byte[] fileData = new byte[entry.Size];
                            new BinaryReader(entry.OpenEntryStream()).Read(fileData, 0, (int)entry.Size);
                            File.WriteAllBytes(tempFilePath, fileData);
                            CreateSingleZip(tempFilePath, topFolder + "\\" + game.console + "\\" + game.name + ".zip");
                            File.Delete(tempFilePath);
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
            ReaderWriterLockSlim fileLock;
            GetLock(file, out fileLock);
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
                            ReaderWriterLockSlim fileLock2;
                            GetLock("tempZip", out fileLock2);
                            fileLock2.EnterWriteLock();
                            byte[] fileData = new byte[entry.Length];
                            new BinaryReader(entry.Open()).Read(fileData, 0, (int)entry.Length);
                            File.WriteAllBytes(tempFolderPath + entry.Name, fileData);
                            CreateSingleZip(tempFolderPath + entry.Name, topFolder + "\\" + game.console + "\\" + game.name + ".zip");
                            File.Delete(tempFolderPath + entry.Name);
                            fileLock2.ExitWriteLock();
                        }
                        else
                        {
                            if (!File.Exists(topFolder + "\\" + game.console + "\\" + game.description)) //extract fails if file exists.
                            {
                                ReaderWriterLockSlim unzipLock;
                                GetLock(topFolder + "\\" + game.console + "\\" + game.description, out unzipLock);
                                unzipLock.EnterWriteLock();
                                entry.ExtractToFile(topFolder + "\\" + game.console + "\\" + game.description, true);
                                unzipLock.ExitWriteLock();
                            }
                        }
                    }
                    else if (moveUnidentified)
                    {
                        Directory.CreateDirectory(topFolder + "\\Unidentified");
                        ReaderWriterLockSlim unzipLock;
                        GetLock(topFolder + "\\Unidentified\\" + entry.Name, out unzipLock);
                        unzipLock.EnterWriteLock();
                        entry.ExtractToFile(topFolder + "\\Unidentified\\" + entry.Name, true);
                        unzipLock.ExitWriteLock();
                    }
                }
            }
            zf.Dispose();

            if (identified && PreserveOriginals)
                MoveFile(topFolder + "\\Identified Originals", file, System.IO.Path.GetFileName(file));
            else
                File.Delete(file);

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
            ReaderWriterLockSlim fileLock, zipLock;
            GetLock(file, out fileLock);
            GetLock(zipPath, out zipLock);
            fileLock.EnterWriteLock();
            zipLock.EnterWriteLock();

            FileStream fs = new FileStream(zipPath, FileMode.OpenOrCreate);
            ZipArchive zf = new ZipArchive(fs, ZipArchiveMode.Update);
            zf.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
            zf.Dispose();
            fs.Close();
            fs.Dispose();

            fileLock.ExitWriteLock();
            zipLock.ExitWriteLock();
        }

        static void GetLock(string file, out ReaderWriterLockSlim lockRef)
        {
            //lockLock.EnterUpgradeableReadLock();
            if (locks.Keys.Any(k => k == file))
            {
                //lockLock.ExitUpgradeableReadLock();
                lockRef = locks[file];
                if (lockRef.IsReadLockHeld || lockRef.IsWriteLockHeld)
                    Console.WriteLine("Lock for " + file + " requested while still open");
                return;
            }

            //lockLock.EnterWriteLock();
            ReaderWriterLockSlim newLock = new ReaderWriterLockSlim();
            locks[file] = newLock;
            //lockLock.ExitWriteLock();
            //lockLock.ExitUpgradeableReadLock();
            lockRef = newLock;
        }
    }
}
