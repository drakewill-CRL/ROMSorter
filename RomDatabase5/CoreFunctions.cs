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
            Directory.CreateDirectory(path + "\\Duplicates");
            foreach (var file in Directory.EnumerateFiles(path))
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                //TODO: check zipped data separately? Or assume stuff was run to make files consistent.
                p.Report(Path.GetFileName(file));
                string[] results = h.HashFileAtPath(file);

                if (crcHashes.ContainsKey(results[0]) && md5Hashes.ContainsKey(results[1]) && sha1Hashes.ContainsKey(results[2]))
                {
                    // this is a dupe, we hit on all 3 hashes.
                    foundDupe = true;
                    var origName = crcHashes[results[0]];
                    var dirName = path + "\\Duplicates\\" + origName.Replace("(", "").Replace(")", "").Trim();
                    Directory.CreateDirectory(dirName);
                    File.Move(file, dirName + "\\" + Path.GetFileName(file));
                }
                crcHashes.TryAdd(results[0], filename);
                md5Hashes.TryAdd(results[1], filename);
                sha1Hashes.TryAdd(results[2], filename);
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
                                ZipHelper.RezipFromArchive(existingZip, zf);
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
                File.Move(tempfilename, Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".zip", true);
                if (!file.EndsWith(".zip")) //we just overwrote this file, don't remove it.
                    File.Delete(file);
                count++;
            }
            progress.Report("Complete");
        }

        public static void Catalog(IProgress<string> progress, string path)
        {
            //Hash all files in directory, write results to a tab-separated values file 
            FileStream fs = File.OpenWrite(path + "\\catalog.tsv");
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("name\tmd5\tsha1\tcrc\tsize");
            Hasher hasher = new Hasher();
            var files = System.IO.Directory.EnumerateFiles(path).Where(f => Path.GetFileName(f) != "catalog.tsv").ToList();
            foreach (var file in files)
            {
                progress.Report(file);
                var hashes = hasher.HashFileAtPath(file);
                FileInfo fi = new FileInfo(file);
                sw.WriteLine(Path.GetFileName(file) + "\t" + hashes[0] + "\t" + hashes[1] + "\t" + hashes[2] + "\t" + fi.Length);
            }
            sw.Close(); sw.Dispose(); fs.Close(); fs.Dispose();
            progress.Report("Complete");
        }

        public static void Verify(IProgress<string> progress, string path)
        {
            bool alert = false;
            var files = File.ReadAllLines(path + "\\catalog.tsv");
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
                    FileInfo fi = new FileInfo(file);
                    var hashes = hasher.HashFileAtPath(file);
                    if (vals[1] == hashes[0] && vals[2] == hashes[1] && vals[3] == hashes[2]) //intentionally leaving size out, despite recording it.
                    {
                        continue;
                    }
                    else
                    {
                        alert = true;
                        File.AppendAllText(path + "\\report.txt", vals[0] + " did not match:" + vals[1] + "|" + hashes[0] + " " + vals[2] + "|" + hashes[1] + " " + vals[3] + "|" + hashes[2] + " " + vals[4] + "|" + fi.Length);
                    }
                }
                catch (Exception ex)
                {
                    alert = true;
                    File.AppendAllText(path + "\\report.txt", "Error checking on " + vals[0] + ":" + ex.Message);
                }
            }

            var missingfiles = filesInFolder.Except(foundfiles);
            foreach (var fif in missingfiles)
            {
                File.AppendAllText(path + "\\report.txt", "File " + fif + " not found in catalog");
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
                        var bins = ZipHelper.FindBinsInCue(cue);
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

        public static void IdentifyLogic(IProgress<string> progress, string path, bool moveUnidentified)
        {
            var files = System.IO.Directory.EnumerateFiles(path).ToList();
            if (moveUnidentified)
                Directory.CreateDirectory(path + "\\Unknown");

            //bool useOffsets = chkUseIDOffsets.Checked;
            string errors = "";
            Sorter sorter = new Sorter();
            foreach (var file in files)
            {
                try
                {
                    progress.Report(Path.GetFileName(file));
                    //Identify it first.
                    var identifiedFile = sorter.IdentifyOneFile(file, false);
                    var destFileName = path + "\\" + (identifiedFile != "" ? identifiedFile : (moveUnidentified ? "\\Unknown\\" : "") + Path.GetFileName(file));

                    if (identifiedFile != destFileName)
                        File.Move(file, destFileName);

                }
                catch (Exception ex)
                {
                    errors += file + ": " + ex.Message + Environment.NewLine;
                }
            }

            
            if (errors != "")
            {
                progress.Report("Comlete, Errors occurred: " + errors);
            }
            else
                progress.Report("Complete");
        }
    }
}
