using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Librarian5Console
{
    class Program
    {
        public static string workingPath = Directory.GetCurrentDirectory();
        public static string conString = "Data Source=" + workingPath  + "Librarian.sqlite;Synchronous=Off;Journal_Mode=MEMORY;"; //Pragma statements go in the connection string.
        
        public static bool verbose = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Working Path:" + workingPath);
            //Rules:
            //If no args passed, auto-detect mode
            //If no database file present, scan all files in folder and sub-folder, hash, record to local DB.
            //if database file present, validate all files in folder and sub-folder against saved hash, report if any changed.
            //Try to minimize external dependencies. The app shouldn't be super big.

            if (args.Any(a => a == "-quiet"))
                verbose = false;

            var dbFile = new FileInfo(workingPath + "Librarian.sqlite");
            if (dbFile.Exists)
            {
                //Check files against DB
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                ValidateAllFiles(workingPath);
                sw.Stop();
                Console.WriteLine("All files scanned in " + sw.Elapsed);
            }
            else
            {
                //Create DB, populate with files as they are.
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                CreateDatabase();
                ScanAllFiles(workingPath);
                sw.Stop();
                Console.WriteLine("All files hashes and database saved in " + sw.Elapsed);
            }
        }

        static void ScanAllFiles(string path)
        {
            var subfolders = Directory.EnumerateDirectories(path);
            if (subfolders.Count() > 0)
                foreach (var sf in subfolders)
                    ScanAllFiles(sf);


            RomDatabase5.Hasher hasher = new RomDatabase5.Hasher();
            var filesToParse = Directory.EnumerateFiles(path);
            foreach (var file in filesToParse)
            {
                var fi = new FileInfo(file);
                if (fi.Name == "Librarian5Console.exe" || fi.Name == "Librarian.sqlite")
                    continue; //We dont need to scan ourselves.

                //Hash file. Save results vto DB.
                var hashes = hasher.HashFile(File.ReadAllBytes(file));
                InsertEntry(file, fi.Length, hashes[0], hashes[1], hashes[2]);

                if (verbose)
                {
                    Console.WriteLine("File:" + file.Replace(workingPath, ""));
                    Console.WriteLine("Size: " + fi.Length);
                    Console.WriteLine("MD5: " + hashes[0]);
                    Console.WriteLine("SHA1: " + hashes[1]);
                    Console.WriteLine("CRC: " + hashes[2]);
                }
            }
        }

        static void ValidateAllFiles(string path)
        {
            var subfolders = Directory.EnumerateDirectories(path);
            if (subfolders.Count() > 0)
                foreach (var sf in subfolders)
                    ValidateAllFiles(sf);

            RomDatabase5.Hasher hasher = new RomDatabase5.Hasher();
            var filesToParse = Directory.EnumerateFiles(path);
            foreach (var file in filesToParse)
            {
                var entry = GetEntry(file);
                if (entry == null)
                {
                    Console.WriteLine("File " + file + " Not Found in database.");
                    continue;
                }
                var fi = new FileInfo(file);
                var hashes = hasher.HashFile(File.ReadAllBytes(file));
                if (fi.Length == entry.size && hashes[0] == entry.md5 && hashes[1] == entry.sha1 && hashes[2] == entry.crc)
                {
                    //good
                    if (verbose)
                        Console.WriteLine("File " + file + " OK");
                }
                else
                {
                    Console.WriteLine("File " + file + " changed!");
                    if (verbose)
                    {
                        Console.WriteLine("File:" + file.Replace(workingPath, ""));
                        Console.WriteLine("Size: " + fi.Length + " VS " + entry.size);
                        Console.WriteLine("MD5: " + hashes[0] + " VS " + entry.md5);
                        Console.WriteLine("SHA1: " + hashes[1] + " VS " + entry.sha1);
                        Console.WriteLine("CRC: " + hashes[2] + " VS " + entry.crc);
                    }
                }
            }
        }

        static void CreateDatabase()
        {
            //Database can be one table.
            string mainTable = "CREATE TABLE files(id INTEGER PRIMARY KEY, relativePath TEXT, size INT, crc TEXT, md5 TEXT, sha1 TEXT);";
            ExecuteSQLiteNonQuery(mainTable);
        }

        static void InsertEntry(string path, long size, string md5, string sha1, string crc)
        {
            string insert = "INSERT INTO files(relativePath,  size, crc, md5, sha1)"
                         + "VALUES(@path,  @size, @crc, @md5, @sha1)";
            List<SQLiteParameter> p = new List<SQLiteParameter>();
            p.Add(new SQLiteParameter("@path", path.Replace(workingPath, ""))); //Save relative path.
            p.Add(new SQLiteParameter("@size", size));
            p.Add(new SQLiteParameter("@crc", crc));
            p.Add(new SQLiteParameter("@md5", md5));
            p.Add(new SQLiteParameter("@sha1", sha1));
            ExecuteSQLiteNonQueryWithParameters(insert, p);
        }

        static Entry GetEntry(string path)
        {
            string query = "SELECT relativePath, size, md5, sha1, crc FROM files WHERE relativePath = '" + path + "'";
            return FindEntry(query);
        }

        public static void ExecuteSQLiteNonQuery(string command)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(command, connection);
                var results = cmd.ExecuteNonQuery();
            }
        }

        public static void ExecuteSQLiteNonQueryWithParameters(string command, List<SQLiteParameter> parameters)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(command, connection);
                cmd.Parameters.AddRange(parameters.ToArray());

                var results = cmd.ExecuteNonQuery();
            }
        }

        public static Entry FindEntry(string query)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(query, connection);
                var results = cmd.ExecuteReader();

                if (results != null)
                    return MapEntry(results);

                return null;
            }
        }

        public class Entry
        {
            public string path;
            public long size;
            public string md5;
            public string sha1;
            public string crc;
        }

        public static Entry MapEntry(SQLiteDataReader data)
        {
            while (data.Read()) //only if there's an entry.
            {
                var e = new Entry();
                e.path = data[0].ToString();
                e.size = long.Parse(data[1].ToString());
                e.md5 = data[2].ToString();
                e.sha1 = data[3].ToString();
                e.crc = data[4].ToString();

                return e;
            }
            return null;
        }
    }
}
