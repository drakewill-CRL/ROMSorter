using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Compressors.Xz;
using SharpCompress.Readers;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RomDatabase5
{
    public static class CoreFunctions
    {
        public static bool MoveMissedPatches = false;
        public static void DetectDupes(IProgress<string> p, string path)
        {
            //Detect duplicates.
            Dictionary<string, string> crcHashes = new Dictionary<string, string>();
            Dictionary<string, string> md5Hashes = new Dictionary<string, string>();
            Dictionary<string, string> sha1Hashes = new Dictionary<string, string>();
            bool foundDupe = false;
            Hasher h = new Hasher();
            Directory.CreateDirectory(path + "/Duplicates");
            foreach (var file in Directory.EnumerateFiles(path))
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                //TODO: check zipped data separately? Or assume stuff was run to make files consistent.
                p.Report(Path.GetFileName(file));
                HashResults results = h.HashFileAtPath(file);

                if (crcHashes.ContainsKey(results.crc) && md5Hashes.ContainsKey(results.md5) && sha1Hashes.ContainsKey(results.sha1))
                {
                    // this is a dupe, we hit on all 3 hashes.
                    foundDupe = true;
                    var origName = crcHashes[results.crc];
                    var dirName = path + "/Duplicates/" + origName.Replace("(", "").Replace(")", "").Trim();
                    Directory.CreateDirectory(dirName);
                    File.Move(file, dirName + "/" + Path.GetFileName(file));
                }

                crcHashes.TryAdd(results.crc, filename);
                md5Hashes.TryAdd(results.md5, filename);
                sha1Hashes.TryAdd(results.sha1, filename);
            }

            if (foundDupe)
                p.Report("Completed, duplicates found and moved.");
            else
                p.Report("Completed, no duplicates.");
        }

        public static void UnzipLogic(IProgress<string> progress, string path)
        {
            var files = Directory.EnumerateFiles(path).ToList();
            foreach (var file in files)
            {
                progress.Report(file);
                switch (Path.GetExtension(file.ToLower()))
                {
                    case ".zip":
                    case ".rar":
                    case ".gz":
                    case ".gzip":
                    case ".tar":
                    case ".7z":
                        //case ".lz":
                        try
                        {
                            using (var mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(file))
                            using (var fileData = mmf.CreateViewStream())
                            {
                                using (var existingZip = SharpCompress.Archives.ArchiveFactory.Open(fileData))
                                {
                                    if (existingZip != null)
                                    {
                                        foreach (var e in existingZip.Entries)
                                            e.WriteToDirectory(path);
                                    }
                                }
                            }
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                        }
                        break;
                    case ".lz": //SharpCompress does not have a setup to nicely handle .tar.lz files internally.
                        using (var mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(file))
                        using (var fileData = mmf.CreateViewStream())
                        {
                            var zf = ReaderFactory.Open(fileData);

                            //This block works, but needs more disk space since it unzips the .tar and then unzips the tar's contents.
                            //Might need to manually set this up to read an lzma stream. This doesn't nicely chain together, so I need the temp file.
                            var outerStream = new SharpCompress.Compressors.LZMA.LZipStream(fileData, SharpCompress.Compressors.CompressionMode.Decompress);
                            var testFileOut = File.Create(path + "/temp.tar");
                            outerStream.CopyTo(testFileOut);
                            testFileOut.Close();
                            outerStream.Dispose();

                            var innerStream = SharpCompress.Archives.Tar.TarArchive.Open(testFileOut);
                            var reader = innerStream.ExtractAllEntries();
                            reader.WriteAllToDirectory(path);
                            //using (var existingZip = SharpCompress.Archives.Tar.TarArchive.Open(fileData))
                            //foreach(var  e in innerStream.)
                            //{ 

                            //e.WriteToDirectory(path); 
                            //}
                            innerStream.Dispose();

                            File.Delete(path + "/temp.tar");

                        }
                        break;
                    case ".7zother":
                        try
                        {
                            using (var mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(file))
                            using (var fileData = mmf.CreateViewStream())
                            {
                                using (var existingZip = SharpCompress.Archives.ArchiveFactory.Open(fileData))
                                {
                                    if (existingZip != null)
                                    {
                                        var reader = existingZip.ExtractAllEntries();
                                        reader.WriteAllToDirectory(path);
                                    }
                                }
                            }
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                        }
                        break;
                }
            }
            progress.Report("Complete");
        }

        public static void ZipLogic(IProgress<string> progress, string path)
        {
            var files = Directory.EnumerateFiles(path).ToList();
            int count = 1;
            foreach (var file in files)
            {
                progress.Report(count + "/" + files.Count() + ":" + Path.GetFileName(file));
                string tempfilename = Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".zip-temp";

                var zfs = File.Create(tempfilename);
                var zf = new ZipArchive(zfs, ZipArchiveMode.Create);
                switch (Path.GetExtension(file.ToLower()))
                {
                    case ".zip":
                    case ".rar":
                    case ".gz":
                    case ".gzip":
                    case ".tar":
                    case ".7z":
                        using (var mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(file))
                        using (var fileData = mmf.CreateViewStream())
                        {
                            using (var existingZip = SharpCompress.Archives.ArchiveFactory.Open(fileData))
                                Helpers.RezipFromArchive(existingZip, zf);
                        }
                        break;
                    case ".lz":
                        using (var mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(file))
                        using (var fileData = mmf.CreateViewStream())
                        {
                            var outerStream = new SharpCompress.Compressors.LZMA.LZipStream(fileData, SharpCompress.Compressors.CompressionMode.Decompress);
                            string tempfile = path + "/temp.tar";
                            var testFileOut = File.Create(tempfile);
                            outerStream.CopyTo(testFileOut);
                            testFileOut.Close();
                            outerStream.Dispose();

                            var stream2 = File.OpenRead(tempfile);
                            var innerStream = SharpCompress.Archives.Tar.TarArchive.Open(stream2);
                            var reader = innerStream.ExtractAllEntries();
                            reader.WriteAllToDirectory(path);
                            reader.Dispose();

                            //Might need to manually set this up to read an lzma stream.
                            //var outerStream = new SharpCompress.Compressors.LZMA.LZipStream(fileData, SharpCompress.Compressors.CompressionMode.Decompress);
                            //var innerStream = SharpCompress.Archives.Tar.TarArchive.Open(outerStream);
                            //using (var existingZip = SharpCompress.Archives.Tar.TarArchive.Open(fileData))
                            //Helpers.RezipFromArchive(innerStream, zf);
                            innerStream.Dispose();
                            stream2.Close(); stream2.Dispose();
                            File.Delete(tempfile);
                        }
                        break;
                    default:
                        zf.CreateEntryFromFile(file, Path.GetFileName(file));
                        break;
                }
                zf.Dispose();
                zfs.Close(); zfs.Dispose();
                File.Move(tempfilename, tempfilename.Replace("-temp", ""), true);
                if (!file.EndsWith(".zip")) //we just overwrote this file, don't remove it.
                {
                    File.Delete(file);
                }
                count++;
            }
            progress.Report("Complete");
        }

        public static void LZipLogic(IProgress<string> progress, string path)
        {
            var files = Directory.EnumerateFiles(path).ToList();
            int count = 1;
            foreach (var file in files)
            {
                progress.Report(count + "/" + files.Count() + ":" + Path.GetFileName(file));

                string tempfilename = Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".lzmazip-temp";

                var zfs = File.Create(tempfilename);
                var zf = WriterFactory.Open(zfs, SharpCompress.Common.ArchiveType.Zip, new WriterOptions(SharpCompress.Common.CompressionType.LZMA)); //LZMA does 
                switch (Path.GetExtension(file.ToLower()))
                {
                    case ".zip":
                    case ".rar":
                    case ".gz":
                    case ".gzip":
                    case ".tar":
                    case ".7z":
                        using (var mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(file))
                        using (var fileData = mmf.CreateViewStream())
                        {
                            using (var existingZip = SharpCompress.Archives.ArchiveFactory.Open(fileData))
                                Helpers.RezipFromArchive(existingZip, zf);
                        }
                        break;
                    case ".lz":
                        //Already done under this method, skip it.
                        break;
                    default:
                        zf.Write(file.Replace(path, ""), new FileInfo(file));
                        break;
                }
                zf.Dispose();
                zfs.Close(); zfs.Dispose();
                if (!file.EndsWith(".lzmazip")) //if this was an lzmazip file, we skipped it.
                {
                    File.Move(tempfilename, tempfilename.Replace("-temp", ""), true);
                    File.Delete(file);
                }
                count++;
            }
            progress.Report("Complete");
        }

        public static void Catalog(IProgress<string> progress, string path)
        {
            //Hash all files in directory, write results to a tab-separated values file 
            FileStream fs = File.OpenWrite(path + "/catalog.tsv");
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("name\tmd5\tsha1\tcrc\tsize");
            Hasher hasher = new Hasher();
            var files = Directory.EnumerateFiles(path).Where(f => Path.GetFileName(f) != "catalog.tsv").ToList();
            foreach (var file in files)
            {
                progress.Report(file);
                var hashes = hasher.HashFileAtPath(file);
                sw.WriteLine(Path.GetFileName(file) + "\t" + hashes.md5 + "\t" + hashes.sha1 + "\t" + hashes.crc + "\t" + hashes.size);
            }
            sw.Close(); sw.Dispose(); fs.Close(); fs.Dispose();
            progress.Report("Complete");
        }

        public static void Verify(IProgress<string> progress, string path)
        {
            bool alert = false;
            var files = File.ReadAllLines(path + "/catalog.tsv");
            var foundfiles = new List<string>();
            var filesInFolder = Directory.EnumerateFiles(path).Where(s => Path.GetFileName(s) != "catalog.tsv").Select(s => Path.GetFileName(s)).ToList();
            Hasher hasher = new Hasher();
            foreach (var file in files.Skip(1))  //ignore header row.
            {
                string[] vals = file.Split("\t");
                foundfiles.Add(vals[0]);
                progress.Report(vals[0]);
                try
                {
                    var hashes = hasher.HashFileAtPath(file);
                    if (vals[1] == hashes.md5 && vals[2] == hashes.sha1 && vals[3] == hashes.crc) //intentionally leaving size out, despite recording it.
                    {
                        continue;
                    }
                    else
                    {
                        alert = true;
                        File.AppendAllText(path + "/report.txt", vals[0] + " did not match:" + vals[1] + "|" + hashes.md5 + " " + vals[2] + "|" + hashes.sha1 + " " + vals[3] + "|" + hashes.crc + " " + vals[4] + "|" + hashes.size);
                    }
                }
                catch (Exception ex)
                {
                    alert = true;
                    File.AppendAllText(path + "/report.txt", "Error checking on " + vals[0] + ":" + ex.Message);
                }
            }

            var missingfiles = filesInFolder.Except(foundfiles);
            foreach (var fif in missingfiles)
            {
                File.AppendAllText(path + "/report.txt", "File " + fif + " not found in catalog");
            }
            if (!alert && missingfiles.Count() == 0)
                progress.Report("Complete, all files verified");
            else if (alert)
                progress.Report("Complete, error found, read report.txt for info");
            else
                progress.Report("Complete, uncataloged files found, read report.txt for info");
        }

        public static void CreateChdLogic(IProgress<string> progress, string path)
        {
            foreach (var folder in Directory.EnumerateDirectories(path))
            {
                CreateChdLogic(progress, folder);
            }

            foreach (var cue in Directory.EnumerateFiles(path).Where(f => f.EndsWith(".cue") || f.EndsWith(".iso")))
            {
                progress.Report(cue);
                var results = new CHD().CreateChd(cue);
                if (results)
                {
                    if (cue.EndsWith("cue"))
                    {
                        //find referenced files that were pulled in by the cue
                        var bins = Helpers.FindBinsInCue(cue);
                        foreach (var b in bins)
                            File.Delete(path + "/" + b);
                    }
                    File.Delete(cue);
                }
            }
            //progress.Report("Complete");
        }

        public static void ExtractChdLogic(IProgress<string> progress, string path)
        {
            foreach (var folder in Directory.EnumerateDirectories(path))
            {
                ExtractChdLogic(progress, folder);
            }

            foreach (var chd in Directory.EnumerateFiles(path).Where(f => f.EndsWith(".chd")).ToList())
            {
                progress.Report(chd);
                var results = new CHD().ExtractCHD(chd);
                if (results)
                {
                    File.Delete(chd);
                }
            }
            //progress.Report("Complete");
        }

        public static void DatLogic(IProgress<string> progress, string path)
        {
            DatCreator.MakeDat(path, progress);
            progress.Report("Completed making DAT file");
        }

        public static void IdentifyLogic(IProgress<string> progress, string path, bool moveUnidentified, MemDb db)
        {
            var files = System.IO.Directory.EnumerateFiles(path).ToList();
            if (moveUnidentified)
                Directory.CreateDirectory(path + "/Unknown");

            //bool useOffsets = chkUseIDOffsets.Checked;
            string errors = "";
            Hasher h = new Hasher();
            foreach (var file in files)
            {
                try
                {
                    progress.Report(Path.GetFileName(file));
                    //Identify it first.
                    var hashes = h.HashFileAtPath(file);
                    var identifiedFiles = db.findFile(hashes);
                    if (identifiedFiles.Count > 1)
                    {
                        //TODO: duplicate entries in DAT file unhandled
                        throw new Exception("multiple entries in provided DAT file for " + file);
                    }

                    var identifiedFile = identifiedFiles.FirstOrDefault()?.name;
                    var destFileName = (!string.IsNullOrWhiteSpace(identifiedFile) ? identifiedFile : (moveUnidentified ? "/Unknown/" : "") + Path.GetFileName(file));

                    if (identifiedFile != destFileName)
                        File.Move(file, path + "/" + destFileName);

                }
                catch (Exception ex)
                {
                    errors += file + ": " + ex.Message + Environment.NewLine;
                }
            }


            if (errors != "")
            {
                progress.Report("Complete, Errors occurred: " + errors);
            }
            else
                progress.Report("Complete");
        }

        public static void IdentifyLogicMultiFile(IProgress<string> progress, string path, bool moveUnidentified, MemDb db)
        {
            //
            var files = System.IO.Directory.EnumerateFiles(path).ToList();
            if (moveUnidentified)
                Directory.CreateDirectory(path + "/Unknown");

            //bool useOffsets = chkUseIDOffsets.Checked;
            string errors = "";
            Hasher h = new Hasher();
            foreach (var file in files)
            {
                try
                {
                    progress.Report(Path.GetFileName(file));
                    //Identify it first.
                    var hashes = h.HashFileAtPath(file);
                    var identifiedFiles = db.findFile(hashes);
                    if (identifiedFiles.Count > 0)
                    {
                        //TODO: duplicate entries in DAT file unhandled
                        throw new Exception("multiple entries in provided DAT file for " + file);
                    }

                    var identifiedFile = identifiedFiles.FirstOrDefault().name; //TODO test this works as expected
                    var destFileName = (identifiedFile != "" ? identifiedFile : (moveUnidentified ? "/Unknown/" : "") + Path.GetFileName(file));

                    if (identifiedFile != destFileName)
                        File.Move(file, path + "/" + destFileName);

                }
                catch (Exception ex)
                {
                    errors += file + ": " + ex.Message + Environment.NewLine;
                }
            }


            if (errors != "")
            {
                progress.Report("Complete, Errors occurred: " + errors);
            }
            else
                progress.Report("Complete");
        }

        public static void OneGameOneRomSort(IProgress<string> progress, string path, MemDb db, List<string> regionPrefs)
        {
            var files = Directory.EnumerateFiles(path);
            Directory.CreateDirectory(path + "/1G1R/");

            foreach (var pciSet in db.parentClones)
            {
                progress.Report(pciSet.name);
                if (pciSet.Clones.Count == 1)
                {
                    if (File.Exists(path + "/" + pciSet.fileName))
                    {
                        File.Move(path + "/" + pciSet.fileName, path + "/1G1R/" + pciSet.fileName);
                    }
                }
                else
                    foreach (string pref in regionPrefs)
                    {
                        var clone = pciSet.Clones.FirstOrDefault(c => c.region == pref);
                        if (clone != null)
                        {
                            if (File.Exists(path + "/" + clone.fileName))
                            {
                                File.Move(path + "/" + clone.fileName, path + "/1G1R/" + clone.fileName);
                                break;
                            }
                        }
                    }
            }

            progress.Report("Done! Check the /1G1R folder for your set.");
        }

        public static void EverdriveSort(IProgress<string> progress, string path)
        {
            //Everdrive sort.
            //assume that the picked folder is the destination and already sorted/IDed.
            //Also, all items are in one folder.

            var fileList = System.IO.Directory.EnumerateFiles(path);
            fileList = fileList.Select(f => System.IO.Path.GetFileName(f)).ToList();

            //Create folders for each letter
            var letters = fileList.Select(f => System.IO.Path.GetFileName(f).ToUpper().Substring(0, 1)).Distinct().ToList(); //pick first letter.
            foreach (var l in letters)
            {
                progress.Report(l);
                System.IO.Directory.CreateDirectory(path + "/" + l);

                var filesToMove = fileList.Where(f => f.StartsWith(l) || f.StartsWith(l.ToLower())).ToList();
                foreach (var rf in filesToMove)
                    System.IO.File.Move(path + "/" + rf, path + "/" + l + "/" + rf);
            }

            progress.Report("Everdrive Sort completed.");
        }

        /// <summary>
        /// Finds any disk images with "disk 1" in the name, then finds any other images 
        /// with the same name and writes them to a .m3u file
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="path"></param>
        public static void CreateM3uPlaylists(IProgress<string> progress, string path)
        {
            var fileList = System.IO.Directory.EnumerateFiles(path)
                .Where(x => x.Contains("disc 1", StringComparison.CurrentCultureIgnoreCase)
                && (x.Contains(".chd") || x.Contains(".iso") || x.Contains(".cue")))
                .ToList();

            foreach (var file in fileList)
            {
                progress.Report(file);
                var fullTitle = System.IO.Path.GetFileName(file);
                var titlePosition = fullTitle.IndexOf("disc", StringComparison.InvariantCultureIgnoreCase);
                var partialTitle = fullTitle.Substring(0, titlePosition);

                var diskFiles = System.IO.Directory.EnumerateFiles(path)
                .Where(x => x.Contains(partialTitle, StringComparison.CurrentCultureIgnoreCase)
                && (x.Contains(".chd") || x.Contains(".iso") || x.Contains(".cue")))
                .OrderBy(x => x)
                .Select(x => Path.GetFileName(x))
                .ToList();

                var filename = $"{partialTitle.Trim('(', '[', ' ')}.m3u";
                var outputFilePath = $"{Path.GetDirectoryName(file)}{Path.DirectorySeparatorChar}{filename}";

                File.WriteAllLines(outputFilePath, diskFiles);
            }

            progress.Report($"M3U playlists created for {fileList.Count} game(s)");
        }

        public static void ApplyAllPatches(IProgress<string> progress, string path)
        {
            var patchList = System.IO.Directory.EnumerateFiles(path).Where(x => x.ToLower().EndsWith(".ips") || x.ToLower().EndsWith(".bps") || x.ToLower().EndsWith(".ups") || x.ToLower().EndsWith(".xdelta")).ToList();
            var possibleROM = System.IO.Directory.EnumerateFiles(path).Where(x => !x.ToLower().EndsWith(".ips") && !x.ToLower().EndsWith(".bps") && !x.ToLower().EndsWith(".ups") && !x.ToLower().EndsWith(".xdelta")).ToList();
            possibleROM = possibleROM.Where(r => !r.ToLower().Contains("desktop.ini")).ToList();

            if (possibleROM.Count > 1 || possibleROM.Count == 0)
            {
                //TODO error out.
                return;
            }

            string romName = possibleROM.FirstOrDefault();


            foreach (var patch in patchList)
            {
                bool result = true;
                progress.Report(patch);
                var extension = Path.GetExtension(patch.ToLower());
                switch (extension)
                {
                    case ".ips":
                    case ".bps":
                        result = Patcher.PatchWithFlips(patch, romName);
                        break;
                    case ".xdelta":
                        result = Patcher.PatchWithXDelta(patch, romName);
                        break;
                    case ".ups":
                        result = Patcher.PatchWithUPS(patch, romName);
                        break;
                }

                if (!result && MoveMissedPatches)
                {
                    Directory.CreateDirectory(path + @"\Unapplied");
                    File.Move(patch, path + @"\Unapplied\" + Path.GetFileName(patch));
                }
                else
                    File.Delete(patch);
            }
            File.Delete(romName);

            progress.Report("Patching Complete.");
        }

        public static void DeletePatches(IProgress<string> progress, string path)
        {
            var patchList = System.IO.Directory.EnumerateFiles(path).Where(x => x.ToLower().EndsWith(".ips") || x.ToLower().EndsWith(".bps") || x.ToLower().EndsWith(".ups") || x.ToLower().EndsWith(".xdelta")).ToList();
            foreach (var p in patchList)
                File.Delete(p);

            progress.Report("Deleting Complete.");
        }

        public static void DeleteIfNoXDelta(IProgress<string> progress, string path)
        {
            var xdeltas = System.IO.Directory.EnumerateFiles(path, "*.xdelta", SearchOption.AllDirectories);

            try
            {
                if (xdeltas.Count() == 0)
                    Directory.Delete(path, true);
            }
            catch { }
        }

        public static void DeleteIfNoUPS(IProgress<string> progress, string path)
        {
            var xdeltas = System.IO.Directory.EnumerateFiles(path, "*.ups", SearchOption.AllDirectories);

            try
            {
                if (xdeltas.Count() == 0)
                    Directory.Delete(path, true);
            }
            catch { }
        }

        public static void DeleteLowercase(IProgress<string> progress, string path)
        {
            var dirs = Directory.EnumerateDirectories(path);
            foreach (var dir in dirs)
            {
                var files = Directory.EnumerateFiles(dir);
                var maybeMissed = files.Where(f => f.EndsWith("IPS") || f.EndsWith("BPS") || f.EndsWith("UPS") || f.EndsWith("XDELTA")).ToList();

                try
                {
                    if (maybeMissed.Count() == 0)
                        Directory.Delete(dir, true);
                    else
                    {
                        foreach (var f in files)
                            if (!maybeMissed.Contains(f))
                                File.Delete(f);
                    }
                }
                catch { }
            }
        }

        public static void PrepareNesForMAME(IProgress<string> progress, string path)
        {
            //TODO: wram is some kind of writable ram. Air Fortress?
            //TODO mapper_ram is MMC5/ExRAM sutff.
            //TODO mapper_bram is mapper-specific battery ram

            HashSet<string> usedShortNames = new HashSet<string>();
            Hasher h = new Hasher();
            var files = Directory.EnumerateFiles(path, "*.nes");
            foreach (var file in files)
            {
                //Path needs to be unzipped game.
                byte[] game = File.ReadAllBytes(file);
                byte[] output = null;

                //TODO: take decent guess on this programmatically.
                string shortName = Path.GetFileNameWithoutExtension(file).ToLower().Substring(0, 9).Replace(" ", ""); 

                int counter = 1;
                while (usedShortNames.Contains(shortName))
                {
                    shortName = shortName.Substring(0, shortName.Length - 1) + counter;
                    counter++;
                }
                usedShortNames.Add(shortName);


                string endFilename = Path.GetFileNameWithoutExtension(file).ToLower() + ".prg";
                string mapper = "nrom";
                int mapperNumber;
                string mirroring = "";
                string chrram = "";
                string battery = "";
                string romSection = "";

                Dictionary<int, string> mapperNames = new Dictionary<int, string>() {
                    { 0, "nrom" },
                    { 1, "sxrom" },
                    { 2, "uxrom" },
                    { 3, "cnrom" },
                    { 4, "txrom" },
                    { 5, "exrom" },
                    { 7, "axrom" },
                    { 9, "pxrom" },
                    { 10, "fxrom" },
                    { 28, "action53" },
                    { 29, "cufrom" },
                    { 30, "unrom512" },
                    { 31, "2a03pur" },
                    { 34, "bnrom" },
                    { 66, "gxrom" },
                    { 69, "sunsoft5a" }, //5b is also the same number, but has expansion audio.
                    { 105, "nes_event" },
                    //{ 111, "doesntwork" }, //doesnt work. WILL be gtrom/cheapacabra.
                    { 555, "nes_event2" },
                };


                //TODO: better detection of INES 2.0 header and processing it later.
                /*
                 *     If byte 7 AND $0C = $08, and the size taking into account byte 9 does not exceed the actual size of the ROM image, then NES 2.0.
                        If byte 7 AND $0C = $04, archaic iNES.
                        If byte 7 AND $0C = $00, and bytes 12-15 are all 0, then iNES.
                        Otherwise, iNES 0.7 or archaic iNES.
                 */
                if (game.Length % 8192 == 16)
                {
                    //headered game, strip from final.
                    var header = new byte[16];
                    Buffer.BlockCopy(game, 0, header, 0, 16);

                    if (header[0] != 'N' && header[1] != 'E' && header[2] != 'S') //Sanity check.
                        continue;

                    //is NES 2.0?
                    if ((header[7] & 12) == 8) //and size
                    {
                        //NES 2.0 uses bytes 6, 7, and 8 for mapper number
                        int mapperLow2 = header[6] >> 4;
                        int mapperHigh2 = header[7] & 240;
                        int mapperHighest = header[8] & 15;
                        int subMapper = header[8] >> 4;

                        mapperNumber = mapperLow2 + mapperHigh2 + (mapperHighest * 255);

                        //INES 2.0 means CHRRAM is deteremined from the header
                        if (header[11] > 0)
                        {
                            int ramSize = header[11] & 15;
                            chrram = "\t\t\t<dataarea name=\"vram\" size=\"" + (64 << ramSize) + "\"/>\r\n";
                        }
                    }
                    else
                    {
                        //older INES header.
                        int mapperLow = header[6] >> 4;
                        int mapperHigh = header[7] & 240;
                        mapperNumber = mapperLow + mapperHigh;

                        //old INES means CHRRAM is automatically 8kb if header[5] == 0
                        if (header[5] == 0)
                            chrram = "<dataarea name=\"vram\" size=\"8192\"/>\r\n</dataarea>\r\n";
                    }

                    if (!mapperNames.TryGetValue(mapperNumber, out mapper))
                        continue; //Probably not going to work here. come back after adding mapper.

                    FileStream outputFile = File.Create(Path.GetDirectoryName(file) + "\\" + shortName + ".zip");
                    ZipArchive outputZip = new ZipArchive(outputFile, ZipArchiveMode.Create);

                    if(header[5] > 0)
                    {
                        //may need to split files for MAME to load it correctly if there's CHRROM.
                        var p1 = outputZip.CreateEntry("0.prg");
                        var p1Stream = p1.Open();
                        int prgSize = header[4] * 16384;
                        byte[] prg = new byte[prgSize];
                        Buffer.BlockCopy(game, 16, prg, 0, prgSize);
                        //File.WriteAllBytes(Path.GetDirectoryName(file) + "\\0.prg", prg);
                        BinaryWriter sw = new BinaryWriter(p1Stream);
                        sw.Write(prg);
                        sw.Close();
                        p1Stream.Close(); p1Stream.Dispose();
                        string crcHashprg = h.GetCRC32String(ref prg);
                        string sha1Hashprg = h.GetSHA1String(ref prg);


                        var c1 = outputZip.CreateEntry("0.chr");
                        var c1Stream = c1.Open();
                        int chrSize = header[5] * 8192;
                        byte[] chr = new byte[chrSize];
                        Buffer.BlockCopy(game, prgSize + 16, chr, 0, chrSize);
                        sw = new BinaryWriter(c1Stream);
                        sw.Write(chr);
                        sw.Close();
                        c1Stream.Close(); c1Stream.Dispose();
                        string crcHashchr = h.GetCRC32String(ref chr);
                        string sha1Hashchr = h.GetSHA1String(ref chr);

                        romSection = "\t\t\t<dataarea name=\"prg\" size=\"" + prg.Length + "\">\r\n" +
                        "\t\t\t\t<rom name=\"0.prg\" size=\"" + prg.Length + "\" crc=\"" + crcHashprg + "\" sha1=\"" + sha1Hashprg + "\"/>\r\n" +
                        "\t\t\t</dataarea>\r\n" +
                        "\t\t\t<dataarea name=\"chr\" size=\"" + chr.Length + "\">\r\n" +
                        "\t\t\t\t<rom name=\"0.chr\" size=\"" + chr.Length + "\" crc=\"" + crcHashchr + "\" sha1=\"" + sha1Hashchr + "\"/>\r\n" +
                        "\t\t\t</dataarea>";
                    }
                    else
                    {
                        //No CHRRAM, use the existing file and name.
                        var entry = outputZip.CreateEntry(endFilename);
                        var entryStream = entry.Open();
                        output = new byte[game.Length - 16];
                        Buffer.BlockCopy(game, 16, output, 0, output.Length);
                        //File.WriteAllBytes(Path.GetDirectoryName(file) + "\\" + endFilename, output);
                        //StreamWriter sw1 = new StreamWriter(entryStream);
                        BinaryWriter sw1 = new BinaryWriter(entryStream);
                        sw1.Write(output);
                        sw1.Close();
                        entryStream.Close(); entryStream.Dispose();

                        string crcHash = h.GetCRC32String(ref output);
                        string sha1Hash = h.GetSHA1String(ref output);

                        romSection = "\t\t\t<dataarea name=\"prg\" size=\"" + output.Length + "\">\r\n" +
                        "\t\t\t\t<rom name=\"" + endFilename + "\" size=\"" + output.Length + "\" crc=\"" + crcHash + "\" sha1=\"" + sha1Hash + "\"/>\r\n" +
                        "\t\t\t</dataarea>";
                    }

                    outputZip.Dispose();
                    outputFile.Close(); outputFile.Dispose();

                    if ((header[6] & 1) == 1)
                        mirroring = "\t\t\t<feature name=\"mirroring\" value=\"vertical\"/>\r\n";

                    if ((header[6] & 2) == 2)
                        battery = "\t\t\t<dataarea name=\"bwram\" size=\"8192\">\r\n\t\t\t\t<rom value=\"0x00\" size=\"8192\" offset=\"0\" " +
                            "loadflag=\"fill\" />\r\n\t\t\t</dataarea>\r\n";


                        //Header processing per INES 1.0:
                        /*
                         * Bytes 	Description 
                            0-3 	Constant $4E $45 $53 $1A (ASCII "NES" followed by MS-DOS end-of-file)
                            4 	Size of PRG ROM in 16 KB units
                            5 	Size of CHR ROM in 8 KB units (value 0 means the board uses CHR RAM)
                            6 	Flags 6 – Mapper, mirroring, battery, trainer
                            7 	Flags 7 – Mapper, VS/Playchoice, NES 2.0
                            8 	Flags 8 – PRG-RAM size (rarely used extension)
                            9 	Flags 9 – TV system (rarely used extension)
                            10 	Flags 10 – TV system, PRG-RAM presence (unofficial, rarely used extension) 
                         */

                        //Mapper note:
                        //nes_pcb.hxx is what defines and connects the slot value to the actual code, if I need to work out which one to use for a given game.
                }
                else
                {
                    //Unheadered game logic.
                    //TODO: converting unheadered game should just save that to a .prg file and make the default XML entry for it.
                    output = game;
                    File.Copy(file, Path.GetDirectoryName(file) + "\\" + endFilename);
                }

                
                StringBuilder xml = new StringBuilder();
                xml.AppendLine("\t<software name=\"" + shortName + "\" cloneof=\"none\" supported=\"partial\">");
                xml.AppendLine("\t\t<description>" + Path.GetFileNameWithoutExtension(file) + "</description>");
                xml.AppendLine("\t\t<year>????</year>");
                xml.AppendLine("\t\t<publisher>&lt;unknown&gt;</publisher>");
                xml.AppendLine("\t\t<info name=\"release\" value=\"xxxxxxxx\"/>");
                xml.AppendLine("\t\t<part name=\"cart\" interface=\"nes_cart\">"); //UXROM is marker for most homebrew games
                xml.AppendLine("\t\t\t<feature name=\"slot\" value=\"" + mapper + "\"/>"); //This is what MAME uses to set mapper usage.
                xml.Append(mirroring);
                xml.AppendLine(romSection);
                xml.Append(chrram);
                xml.Append(battery);
                xml.AppendLine("\t\t</part>");
                xml.AppendLine("\t</software>");

                File.WriteAllText(Path.GetDirectoryName(file) + "\\" + shortName + ".xml", xml.ToString());
            }
        }

        public static string QuickMergeFolders(IProgress<string> progress, string path1, string path2)
        {
            //This looks at the files by name in both folders, and copies any files not present in 1 to the other, in both directions

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Folder Merge Report");

            if (!path1.EndsWith("\\"))
                path1 += "\\";
            
            if (!path2.EndsWith("\\"))
                path2 += "\\";

            //recursive eventually
            //var subfolders1 = System.IO.Directory.EnumerateDirectories(path1);
            //var subfolders2 = System.IO.Directory.EnumerateDirectories(path2);

            //var missing2 = subfolders1.Where(s => !subfolders2.Contains(s));
            //var missing1 = subfolders2.Where(s => !subfolders1.Contains(s));

            //foreach(var sf in subfolders1)
            //{
            //    if (missing2.Contains(sf))
            //    {
            //        //copy folder
            //    }
            //    else
            //    {
            //        //recursive call.
            //    }
            //}

            var files1 = System.IO.Directory.EnumerateFiles(path1);
            var files2 = System.IO.Directory.EnumerateFiles(path2);

            var moveTo2 = files1.Where(f => !files2.Contains(f));
            var moveTo1 = files2.Where(f => !files1.Contains(f));

            foreach (var file in moveTo1)
                File.Copy(file, path2 + Path.GetFileName(file));

            foreach (var file in moveTo2)
                File.Copy(file, path1 + Path.GetFileName(file));

            sb.AppendLine("Files moved to " + path1 + ":");
            foreach (var f in moveTo1)
                sb.AppendLine(f);

            sb.AppendLine("Files moved to " + path2 + ":");
            foreach (var f in moveTo2)
                sb.AppendLine(f);

            return sb.ToString();

        }
    }
}
