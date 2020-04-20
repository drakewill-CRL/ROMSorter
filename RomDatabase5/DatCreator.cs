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
        //Will want for homebrew games, entries on un-maintained lists, etc.

        public static void MakeDat(string folder)
        {
            //Header is partly for XmlDocument, partly in case I ever want to share or reuse it.
            string header = @"<?xml version=""1.0"" encoding=""UTF-8""?>" + Environment.NewLine + "<datafile>";  //the only important part for personal use.
            //"<!DOCTYPE datafile PUBLIC ""-//Logiqx//DTD ROM Management Datafile//EN"" ""http://www.logiqx.com/Dats/datafile.dtd"">";  +
            // "<datafile>" +
            // "<header>" + 
            //< name > Acorn Archimedes - Games - [ADF] </ name >
            //   < description > Acorn Archimedes - Games - [ADF](TOSEC - v2011 - 02 - 22) </ description >
            //      < category > TOSEC </ category >
            //      < version > 2011 - 02 - 22 </ version >
            //      < author > C0llector - Cassiel </ author >
            //      < email > contact@tosecdev.org </ email >
            //         < homepage > TOSEC </ homepage >
            //         < url > http://www.tosecdev.org/</url>
            // </ header >
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header);
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(folder);

            sb.AppendLine(GetEntries(folder));

            sb.Append("</datafile>");
            var folders = folder.Split('\\');
            var folderName = folders[folders.Length - 1];
            System.IO.File.WriteAllText(folder + @"\" + folderName + ".dat", sb.ToString());
        }

        static string GetEntries(string folder)
        {
            StringBuilder sb = new StringBuilder();
            System.Threading.Tasks.Parallel.ForEach(System.IO.Directory.EnumerateFiles(folder), (file) =>
            {
                if (Path.GetFileName(file).EndsWith(".zip"))
                {
                    sb.Append(GetGameAndRomEntryMultifileFromZip(file));
                }
                else
                {
                    //do the actual work.
                    var fileinfo = new System.IO.FileInfo(file);
                    if (fileinfo.Length < (Math.Pow(2, 31)))
                        sb.AppendLine(GetGameAndRomEntrySingleFile(System.IO.Path.GetFileName(file).Replace("&", "&amp;"), System.IO.File.ReadAllBytes(file)));
                    else
                    {
                        //Skipping until I can work this out.

                        //We will only hash the first 2GB of a file bigger than that for now?
                        //TODO: System.IO.File.ReadAllBytes() throws an error on files over 2GB. Read those manually
                        //var fs = new System.IO.FileStream(file, System.IO.FileMode.Open);
                        //byte[] contents = new byte[(int)Math.Pow(2, 30)]; //Still throws 'OutOfMemoryException' at 2^31
                        //var offset = 0;
                        //var filesize = Math.Pow(2, 31); // fileinfo.Length;
                        //while (offset < filesize)
                        //{
                        //    offset += fs.Read(contents, offset, (int)filesize - offset);
                        //}
                        //sb.Append(GetGameAndRomEntrySingleFile(file.Replace("&", "&amp;"), contents)); //Tag as partial?
                    }
                }
            });

            foreach (var dir in System.IO.Directory.EnumerateDirectories(folder))
            {
                sb.AppendLine(GetEntries(dir));
            }
            return sb.ToString(); //make sure there's something here to return. If everything is empty this is blank and throws an error.
        }

        static string GetGameAndRomEntrySingleFile(string filename, byte[] file)
        {
            var hashes = Hasher.HashFile(file);
            StringBuilder results = new StringBuilder();
            results.AppendLine("<game name=\"" + System.IO.Path.GetFileNameWithoutExtension(filename).Replace("&", "&amp;") + "\">");
            results.AppendLine("<description>" + System.IO.Path.GetFileNameWithoutExtension(filename).Replace("&", "&amp;") + "</description>");
            results.AppendLine("<rom name=\"" + filename.Replace("&", "&amp;") + "\" size=\"" + file.Count() + "\" crc=\"" + hashes[2] + "\" md5=\"" + hashes[0] + "\" sha1=\"" + hashes[1] + "\"/>");
            results.AppendLine("</game>");

            return results.ToString();
        }

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
                sb.AppendLine("</game>");
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
                sb.AppendLine("</game>");
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
                    var hashes = Hasher.HashFile(data);
                    results.AppendLine("<rom name=\"" + entry.FullName.Replace("&", "&amp;") + "\" size=\"" + entry.Length + "\" crc=\"" + hashes[2] + "\" md5=\"" + hashes[0] + "\" sha1=\"" + hashes[1] + "\"/>");
                    br.Close();
                    br.Dispose();
                }
            }
            zip.Dispose();

            results.AppendLine("</game>" + Environment.NewLine);
            return results.ToString();
        }

        public static string GetGameAndRomEntryMultifileFromFolder(string folderName)
        {
            //For zip files
            StringBuilder results = new StringBuilder();
            results.AppendLine("<game name=\"" + System.IO.Path.GetDirectoryName(folderName).Replace("&", "&amp;") + "\">");
            results.AppendLine("<description>" + System.IO.Path.GetDirectoryName(folderName).Replace("&", "&amp;") + "</description>");
            var files = Directory.EnumerateFiles(folderName);
            foreach (var file in files)
            {
                FileInfo fi = new FileInfo(file);
                var hashes = Hasher.HashFile(File.ReadAllBytes(file));
                results.AppendLine("<rom name=\"" + fi.Name.Replace("&", "&amp;") + "\" size=\"" + fi.Length + "\" crc=\"" + hashes[2] + "\" md5=\"" + hashes[0] + "\" sha1=\"" + hashes[1] + "\"/>");
            }

            results.AppendLine("</game>" + Environment.NewLine);
            return results.ToString();
        }
    }
}
