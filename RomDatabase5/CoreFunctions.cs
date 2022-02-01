using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomDatabase5
{
    public static class CoreFunctions
    {
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
                switch (Path.GetExtension(file))
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
                            {
                                if (existingZip != null)
                                {
                                    foreach (var e in existingZip.Entries)
                                        e.WriteToDirectory(path);
                                }
                            }
                        }
                        File.Delete(file);
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
                string tempfilename = Path.GetTempFileName();
                //SharpCompress.Archives.IArchive existingZip = null;
                //var fs = File.OpenRead(file);

                var zfs = File.Create(tempfilename);
                var zf = new ZipArchive(zfs, ZipArchiveMode.Create);
                switch (Path.GetExtension(file))
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
                    default:
                        zf.CreateEntryFromFile(file, Path.GetFileName(file));
                        break;
                }
                //if (existingZip != null) existingZip.Dispose();
                //fs.Close(); fs.Dispose();
                zf.Dispose();
                zfs.Close(); zfs.Dispose();
                File.Move(tempfilename, Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".zip", true);
                if (!file.EndsWith(".zip")) //we just overwrote this file, don't remove it.
                    File.Delete(file);
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
                            File.Delete(b);
                    }
                    File.Delete(cue);
                }
            }
            progress.Report("Complete");
        }

        public static void ExtractChdLogic(IProgress<string> progress, string path)
        {
            foreach (var chd in Directory.EnumerateFiles(path).Where(f => f.EndsWith(".chd")).ToList())
            {
                progress.Report(chd);
                var results = new CHD().ExtractCHD(chd);
                if (results)
                {
                    File.Delete(chd);
                }
            }
            progress.Report("Complete");
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
                    if (identifiedFiles.Count > 0)
                    {
                        //TODO: duplicate entries in DAT file unhandled
                        throw new Exception("multiple entries in provided DAT file for " + file);
                    }

                    var identifiedFile = identifiedFiles.FirstOrDefault().name;
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
        }
    }
}
