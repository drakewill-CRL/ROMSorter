using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomDatabase5
{
    public static class DatCreator
    {
        //For making my own dat files.
        //XML file with .dat extension
        static Hasher hasher = new Hasher();

        public static void MakeDat(string folder, IProgress<string> progress = null)
        {
            string header = @"<?xml version=""1.0"" encoding=""UTF-8""?>" + Environment.NewLine +
            @"<!DOCTYPE datafile PUBLIC ""-/Logiqx/DTD ROM Management Datafile/EN"" ""http://www.logiqx.com/Dats/datafile.dtd"">" + Environment.NewLine
            + "<datafile>" + Environment.NewLine
            + "<header>" + Environment.NewLine
            + "<name>" + Path.GetFileName(folder) + "</name>" + Environment.NewLine
            + "<author>Created with ROMSorter</author>" + Environment.NewLine
            + "<version>" + DateTime.Now.ToString("yyyy-MM-dd") + "</version>" + Environment.NewLine 
            + "</header>" + Environment.NewLine;
            StringBuilder sb = new StringBuilder();
            sb.Append(header);

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(folder);
            sb.Append(GetEntries(folder, progress));
            sb.Append("</datafile>");
            System.IO.File.WriteAllText(folder + @"\" + Path.GetFileName(folder) + ".dat", sb.ToString());
        }

        static string GetEntries(string folder, IProgress<string> progress = null)
        {
            StringBuilder sb = new StringBuilder();
            //System.Threading.Tasks.Parallel.ForEach(System.IO.Directory.EnumerateFiles(folder), (file) => //parallel works fine but makes reporting harder.
            foreach (var file in System.IO.Directory.EnumerateFiles(folder))
            {
                progress.Report(Path.GetFileName(file));
                if (Path.GetFileName(file).EndsWith(".zip"))
                {
                    sb.Append(GetGameAndRomEntryMultifileFromZip(file));
                }
                else
                {
                    //do the actual work.
                    var fileinfo = new System.IO.FileInfo(file);
                    var mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(file);
                    sb.Append(GetGameAndRomEntrySingleFile(System.IO.Path.GetFileName(file).Replace("&", "&amp;"), mmf));
                    mmf.Dispose();
                }
            } //);

            foreach (var dir in System.IO.Directory.EnumerateDirectories(folder))
            {
                sb.Append(GetEntries(dir));
            }
            return sb.ToString(); //make sure there's something here to return. If everything is empty this is blank and throws an error.
        }

        static string GetGameAndRomEntrySingleFile(string filename, System.IO.MemoryMappedFiles.MemoryMappedFile file)
        {
            StringBuilder results = new StringBuilder();
            using (var viewStream = file.CreateViewStream())
            {
                var hashes = hasher.HashFile(viewStream);
                results.AppendLine("<game name=\"" + System.IO.Path.GetFileNameWithoutExtension(filename).Replace("&", "&amp;") + "\">");
                results.AppendLine("<description>" + System.IO.Path.GetFileNameWithoutExtension(filename).Replace("&", "&amp;") + "</description>");
                results.AppendLine("<rom name=\"" + filename.Replace("&", "&amp;") + "\" size=\"" + viewStream.Length + "\" crc=\"" + hashes[2] + "\" md5=\"" + hashes[0] + "\" sha1=\"" + hashes[1] + "\"/>");
                results.Append("</game>");
            }
            return results.ToString();
        }

        //static string GetGameAndRomEntrySingleFile(string filename, byte[] file)
        //{
        //    var hashes = hasher.HashFile(file);
        //    StringBuilder results = new StringBuilder();
        //    results.AppendLine("<game name=\"" + System.IO.Path.GetFileNameWithoutExtension(filename).Replace("&", "&amp;") + "\">");
        //    results.AppendLine("<description>" + System.IO.Path.GetFileNameWithoutExtension(filename).Replace("&", "&amp;") + "</description>");
        //    results.AppendLine("<rom name=\"" + filename.Replace("&", "&amp;") + "\" size=\"" + file.Count() + "\" crc=\"" + hashes[2] + "\" md5=\"" + hashes[0] + "\" sha1=\"" + hashes[1] + "\"/>");
        //    results.Append("</game>");

        //    return results.ToString();
        //}

        public static void DumpDBToDat()
        {
            //This should be good to go from now on.
            string header = @"<?xml version=""1.0"" encoding=""UTF-8""?>" + Environment.NewLine + "<datafile>";  //the only important part for personal use.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header);

            var roms = Database.GetAllGames();
            foreach (var entry in roms)
            {
                sb.AppendLine("<game name=\"" + entry.name.Replace("&", "&amp;") + "\">");
                sb.AppendLine("<description>" + entry.description.Replace("&", "&amp;") + "</description>");
                sb.AppendLine("<console>" + entry.console.Replace("&", "&amp;") + "</console>");
                sb.AppendLine("<rom name=\"" + entry.name.Replace("&", "&amp;") + "\" size=\"" + entry.size + "\" crc=\"" + entry.crc + "\" md5=\"" + entry.md5 + "\" sha1=\"" + entry.sha1 + "\"/>");
                sb.Append("</game>");
            }

            var discs = Database.GetAllDiscs();
            foreach (var entry in discs)
            {
                sb.AppendLine("<game name=\"" + entry.Key.Replace("&", "&amp;") + "\">");
                sb.AppendLine("<description>" + entry.Key.Replace("&", "&amp;") + "</description>");
                sb.AppendLine("<console>" + entry.First().console.Replace("&", "&amp;") + "</console>");
                var files = entry.ToList();
                foreach (var file in files)
                {
                    sb.AppendLine("<rom name=\"" + file.description.Replace("&", "&amp;") + "\" size=\"" + file.size + "\" crc=\"" + file.crc + "\" md5=\"" + file.md5 + "\" sha1=\"" + file.sha1 + "\"/>");
                }
                sb.Append("</game>");
            }

            System.IO.File.WriteAllText("RomSorterDB.dat", sb.ToString());
        }

        public static string GetGameAndRomEntryMultifileFromZip(string filename)
        {
            //For zip files
            StringBuilder results = new StringBuilder();
            results.AppendLine("<game name=\"" + System.IO.Path.GetFileNameWithoutExtension(filename).Replace("&", "&amp;") + "\">");
            results.AppendLine("<description>" + System.IO.Path.GetFileNameWithoutExtension(filename).Replace("&", "&amp;") + "</description>");
            ZipArchive zip = new ZipArchive(new FileStream(filename, FileMode.Open));
            foreach (var entry in zip.Entries)
            {
                if (entry.Length > 0)
                {
                    var br = new BinaryReader(entry.Open());
                    byte[] data = new byte[(int)entry.Length];
                    br.Read(data, 0, (int)entry.Length); //Exception occurs if length is 0 or negative?
                    var hashes = hasher.HashFile(data);
                    results.AppendLine("<rom name=\"" + entry.FullName.Replace("&", "&amp;") + "\" size=\"" + entry.Length + "\" crc=\"" + hashes[2] + "\" md5=\"" + hashes[0] + "\" sha1=\"" + hashes[1] + "\"/>");
                    br.Close();
                    br.Dispose();
                }
            }
            zip.Dispose();

            results.Append("</game>" + Environment.NewLine);
            return results.ToString();
        }

        //public static string GetGameAndRomEntryMultifileFromFolder(string folderName)
        //{
        //    //For zip files
        //    StringBuilder results = new StringBuilder();
        //    results.AppendLine("<game name=\"" + System.IO.Path.GetDirectoryName(folderName).Replace("&", "&amp;") + "\">");
        //    results.AppendLine("<description>" + System.IO.Path.GetDirectoryName(folderName).Replace("&", "&amp;") + "</description>");
        //    var files = Directory.EnumerateFiles(folderName);
        //    foreach (var file in files)
        //    {
        //        FileInfo fi = new FileInfo(file);
        //        byte[] fileData = File.ReadAllBytes(file);//will error on files over 2GB
        //        var hashes = hasher.HashFile(fileData);
        //        results.AppendLine("<rom name=\"" + fi.Name.Replace("&", "&amp;") + "\" size=\"" + fi.Length + "\" crc=\"" + hashes[2] + "\" md5=\"" + hashes[0] + "\" sha1=\"" + hashes[1] + "\"/>");
        //    }

        //    results.Append("</game>" + Environment.NewLine);
        //    return results.ToString();
        //}
    }
}
