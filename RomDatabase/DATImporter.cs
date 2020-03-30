using RomSorter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace RomDatabase
{
    public static class DATImporter
    {
        //Note: in most clrmamepro format dat files, name and description are identical.
        //In my database, description is the filename, and name is the game name itself (for multi-file games)

        static ReaderWriterLock filelock = new ReaderWriterLock();
        static int totalCount = 0;
        //Read .dat file(s), parse and port into the database I'm using
        public static void LoadAllDatFiles(string directory)
        {
            var subfolders = System.IO.Directory.EnumerateDirectories(directory);
            foreach (var sf in subfolders)
                LoadAllDatFiles(sf);

            //Read all dat files in the given folder, parse them, insert records.
            var files = System.IO.Directory.EnumerateFiles(directory, "*.dat");
            System.Threading.Tasks.Parallel.ForEach(files, (file) =>
            //foreach (var file in files)
            {
                ParseDatFileFast(file);
            });
        }

        public static void LoadAllDiscDatFilesIntegrity(string directory, IProgress<string> progress = null)
        {
            var subfolders = System.IO.Directory.EnumerateDirectories(directory);
            foreach (var sf in subfolders)
                LoadAllDiscDatFilesIntegrity(sf);

            //Read all dat files in the given folder, parse them, insert records.
            var files = System.IO.Directory.EnumerateFiles(directory, "*.dat");
            System.Threading.Tasks.Parallel.ForEach(files, (file) =>
            //foreach (var file in files)
            {
                ParseDiscDatFileHighIntegrity(file, progress);
            });
        }

        public static void LoadAllDatFilesIntegrity(string directory, IProgress<string> progress = null)
        {
            var subfolders = System.IO.Directory.EnumerateDirectories(directory);
            foreach (var sf in subfolders)
                LoadAllDatFilesIntegrity(sf);

            //Read all dat files in the given folder, parse them, insert records.
            var files = System.IO.Directory.EnumerateFiles(directory, "*.dat");
            System.Threading.Tasks.Parallel.ForEach(files, (file) =>
            //foreach (var file in files)
            {
                ParseDatFileHighIntegrity(file, progress);
            });
        }

        public static void ParseDatFileFast(string file)
        {
            string consoleName = System.IO.Path.GetFileNameWithoutExtension(file).Split('=')[0].Trim(); //filenames are "Console = Subset[optionalsubtype](TOSEC date).dat
            //these are XML.
            //for each node, insert a game entry.
            var dat = new System.Xml.XmlDocument();
            dat.Load(file);
            //find nodes per the spec.
            List<Game> batchInserts = new List<Game>();
            var entries = dat.GetElementsByTagName("rom"); //has actual hash values, game is probably the parent that matters for MAME only.
            foreach (XmlElement entry in entries)
            {
                var tempGame = new Game();
                tempGame.name = entry.GetAttribute("name"); //.Replace("'", "''");
                string desc = entry.GetAttribute("description");
                if (desc == null || desc == "")
                    desc = System.IO.Path.GetFileNameWithoutExtension(tempGame.name); //entry.ParentNode.SelectNodes("/description").Item(0).InnerText;
                tempGame.description = desc;
                tempGame.console = consoleName;
                tempGame.crc = entry.GetAttribute("crc").ToLower();
                tempGame.sha1 = entry.GetAttribute("sha1").ToLower();
                tempGame.md5 = entry.GetAttribute("md5").ToLower();
                tempGame.size = Int64.Parse(entry.GetAttribute("size"));
                batchInserts.Add(tempGame);
            }
            Database.InsertGamesBatch(batchInserts);
        }

        public static void ParseDatFileHighIntegrity(string file, IProgress<string> progress = null)
        {
            //This version inserts games individually, and checks for exisiting entries with the same hashes. The odds of 3 hashes and filesize being the same on non-identical files is approx. 0.
            string consoleName = System.IO.Path.GetFileNameWithoutExtension(file).Split('=')[0].Trim(); //filenames are "Console - Subset[optionalsubtype](TOSEC date).dat
            //these are XML.
            //for each node, insert a game entry.
            var dat = new System.Xml.XmlDocument();
            dat.Load(file);
            //find nodes per the spec.
            var entries = dat.GetElementsByTagName("rom"); //has actual hash values, game is probably the parent that matters for MAME only.
            foreach (XmlElement entry in entries)
            {
                var tempGame = new Game();
                string name = entry.GetAttribute("name");
                int tempInt = 0;
                if (Int32.TryParse(name, out tempInt))
                    tempGame.name = entry.ParentNode.Attributes["name"].InnerText;
                else
                    tempGame.name = name;
                string desc = entry.GetAttribute("description");
                if (desc == null || desc == "")
                    desc = System.IO.Path.GetFileNameWithoutExtension(name); //entry.ParentNode.SelectNodes("/description").Item(0).InnerText;
                tempGame.description = desc;
                tempGame.console = consoleName;
                tempGame.crc = entry.GetAttribute("crc").ToLower();
                tempGame.sha1 = entry.GetAttribute("sha1").ToLower();
                tempGame.md5 = entry.GetAttribute("md5").ToLower();
                tempGame.size = Int64.Parse(entry.GetAttribute("size")); //3DS games and DVDs can go over 4GB
                var isDupe = Database.FindGame(tempGame.size, new string[3] { tempGame.md5, tempGame.sha1, tempGame.crc });
                if (isDupe != null && isDupe.id != null)
                {
                    //write log on duplicate entry.
                    filelock.AcquireWriterLock(Int32.MaxValue);
                    System.IO.File.AppendAllLines("insertLog.txt", new List<string>() { "Game '" + tempGame.name + "' already matches existing entry '" + isDupe.name + "'" });
                    filelock.ReleaseWriterLock();
                }
                else
                    Database.InsertGame(tempGame);
                totalCount++;
                progress.Report(totalCount + " entries scanned.");
            }
        }

        public static void LoadAllDiscDatFiles(string directory)
        {
            var subfolders = System.IO.Directory.EnumerateDirectories(directory);
            foreach (var sf in subfolders)
                LoadAllDiscDatFiles(sf);

            //Read all dat files in the given folder, parse them, insert records.
            var files = System.IO.Directory.EnumerateFiles(directory, "*.dat");
            System.Threading.Tasks.Parallel.ForEach(files, (file) =>
            //foreach (var file in files)
            {
                ParseDiscDatFile(file, null);
            });
        }

        public static void ParseDiscDatFile(string file, IProgress<string> p)
        {
            //should be similar to main dat file, but will have multiple files to pair up to one disc. 
            //Use name for sorting all files for one game, use description to identify each separate file.
            string consoleName = System.IO.Path.GetFileNameWithoutExtension(file).Split('=')[0].Trim(); //filenames are "Console - Subset[optionalsubtype](TOSEC date).dat
            //these are XML.
            //for each node, insert a game entry.
            var dat = new System.Xml.XmlDocument();
            dat.Load(file);
            //find nodes per the spec.
            List<Game> batchInserts = new List<Game>();
            var entries = dat.GetElementsByTagName("game"); //has unique games to find. ROM has each file.
            //has actual hash values, game is probably the parent that matters for MAME only.
            foreach (XmlElement entry in entries)
            {
                if (p != null)
                    p.Report(entry.GetAttribute("name"));
                var allFiles = entry.SelectNodes("rom");
                foreach (XmlElement romFile in allFiles)
                {
                    var tempGame = new Game();
                    string name = entry.GetAttribute("name");
                    int tempInt = 0; //NOTE: some DSi games have just digits for a filename. This makes those human-readable.
                    if (Int32.TryParse(name, out tempInt))
                        tempGame.name = romFile.GetElementsByTagName("name")[0].InnerText + ".nds"; //TODO: ensure this only applies to NDS games, doesn't trigger for games like 1943.nes
                    else
                        tempGame.name = name;
                    string desc = romFile.GetAttribute("name");
                    tempGame.description = desc;
                    tempGame.console = consoleName;
                    tempGame.crc = romFile.GetAttribute("crc").ToLower();
                    tempGame.sha1 = romFile.GetAttribute("sha1").ToLower();
                    tempGame.md5 = romFile.GetAttribute("md5").ToLower();
                    tempGame.size = Int64.Parse(romFile.GetAttribute("size"));
                    batchInserts.Add(tempGame);
                }
            }
            Database.InsertDiscsBatch(batchInserts);
        }

        public static void ParseDiscDatFileHighIntegrity(string file, IProgress<string> p)
        {
            //should be similar to main dat file, but will have multiple files to pair up to one disc. 
            //Use name for sorting all files for one game, use description to identify each separate file.
            string consoleName = System.IO.Path.GetFileNameWithoutExtension(file).Split('=')[0].Trim(); //filenames are "Console - Subset[optionalsubtype](TOSEC date).dat
            //these are XML.
            //for each node, insert a game entry.
            var dat = new System.Xml.XmlDocument();
            dat.Load(file);
            //find nodes per the spec.
            List<Game> batchInserts = new List<Game>();
            var entries = dat.GetElementsByTagName("game"); //has unique games to find. ROM has each file.
            //has actual hash values, game is probably the parent that matters for MAME only.
            foreach (XmlElement entry in entries)
            {
                if (p != null)
                    p.Report(entry.GetAttribute("name"));
                var allFiles = entry.SelectNodes("rom");
                int foundFileCount = 0;
                foreach (XmlElement romFile in allFiles)
                {
                    foundFileCount = 0;
                    var tempGame = new Game();
                    string name = entry.GetAttribute("name");
                    int tempInt = 0; //NOTE: some DSi games have just digits for a filename. This makes those human-readable.
                    if (Int32.TryParse(name, out tempInt))
                        try
                        {
                            tempGame.name = romFile.GetElementsByTagName("name")[0].InnerText + ".nds"; //TODO: ensure this only applies to NDS games, doesn't trigger for games like 1943.nes
                        }
                        catch (Exception ex)
                        {
                            tempGame.name = name;
                        }
                    else
                        tempGame.name = name;
                    string desc = romFile.GetAttribute("name");
                    tempGame.description = desc;
                    tempGame.console = consoleName;
                    tempGame.crc = romFile.GetAttribute("crc").ToLower();
                    tempGame.sha1 = romFile.GetAttribute("sha1").ToLower();
                    tempGame.md5 = romFile.GetAttribute("md5").ToLower();
                    tempGame.size = Int64.Parse(romFile.GetAttribute("size"));

                    var existingEntry = Database.FindDisc(tempGame.size, new string[] { tempGame.md5, tempGame.sha1, tempGame.crc });
                    if (existingEntry != null)
                        foundFileCount++;
                    batchInserts.Add(tempGame);
                }
                //This logic doesn't seem to be correct.
                //if (foundFileCount == allFiles.Count)
                //{
                //    filelock.AcquireWriterLock(Int32.MaxValue);
                //    System.IO.File.AppendAllLines("insertLog.txt", new List<string>() { "Game '" + entry.GetAttribute("name") + "' has no unique files." });
                //    filelock.ReleaseWriterLock();
                //}
                //else
                    Database.InsertDiscsBatch(batchInserts);
                batchInserts.Clear();
            }
        }

        public static void Read1G1RFile(string file)
        {
            int missingEntries = 0;
            var dat = new System.Xml.XmlDocument();
            dat.Load(file);
            var entries = dat.GetElementsByTagName("rom"); //has actual hash values, game is probably the parent that matters for MAME only.
            foreach (XmlElement entry in entries)
            {
                var tempGame = new Game();
                tempGame.crc = entry.GetAttribute("crc").ToLower();
                tempGame.sha1 = entry.GetAttribute("sha1").ToLower();
                tempGame.md5 = entry.GetAttribute("md5").ToLower();
                tempGame.size = Int64.Parse(entry.GetAttribute("size"));

                var gameExists = Database.FindGame(tempGame.size, tempGame.crc, tempGame.md5, tempGame.sha1);
                if (gameExists != null && gameExists.id != null)
                    Database.SetGameAs1G1R(gameExists.id);
                else
                {
                    //TODO I guess I should insert this entry? For now increment a counter.
                    missingEntries++;
                }
            }
        }

        public static void ReadPinballDatFile(string file)
        {
            var fileParts = System.IO.Path.GetFileNameWithoutExtension(file).Split(' ');
            string console = fileParts[0] + " " + fileParts[1];
            int missingEntries = 0;
            var dat = new System.Xml.XmlDocument();
            dat.Load(file);
            List<Game> batchInserts = new List<Game>();
            var entries = dat.GetElementsByTagName("rom"); //has actual hash values, game is probably the parent that matters for MAME only.
            foreach (XmlElement entry in entries)
            {
                if (entry.GetAttribute("name").EndsWith(".fpt") || entry.GetAttribute("name").EndsWith(".vpt"))
                {
                    var tempGame = new Game();
                    tempGame.name = entry.GetAttribute("name"); //.Replace("'", "''"); //with this replace, it puts '' in every file name that has a ', and parameters shoudl fix this.
                    tempGame.description = System.IO.Path.GetFileNameWithoutExtension(entry.GetAttribute("name"));
                    tempGame.console = console;
                    tempGame.crc = entry.GetAttribute("crc").ToLower();
                    tempGame.sha1 = entry.GetAttribute("sha1").ToLower();
                    //tempGame.md5 = entry.GetAttribute("md5").ToLower(); //pinball dats dont have MD5 entries. They're also sort-of like discs in that sometimes they have multiple files, but some of them are optional.
                    tempGame.size = Int64.Parse(entry.GetAttribute("size"));

                    batchInserts.Add(tempGame);
                }
            }

            Database.InsertGamesBatch(batchInserts);
        }

        public static void ParseSwitchDatFile(string file)
        {
            //TODO: implement this.
            //these have some different text format that isn't XML, but should have similar data entries. I just have to dig through it manually
        }
    }
}
