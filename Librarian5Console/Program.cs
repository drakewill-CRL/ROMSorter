﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace Librarian5Console
{
    class Program
    {
        public static string workingPath = Directory.GetCurrentDirectory();
        public static string conString = ""; 
        
        public static bool verbose = true;
        public static bool dbPerFolder = false;

        public static StringBuilder allErrors = new StringBuilder();

        static void Main(string[] args)
        {
            bool defaultVerify = false;
            if (args.Any(a => a.StartsWith("-wp:")))
            {
                var param = args.Where(a => a.StartsWith("-wp:")).First();
                workingPath = param.Substring(param.IndexOf(":") + 1, param.Length - param.IndexOf(":") - 1);
            }
            var dbFile = new FileInfo(workingPath + "\\Librarian.sqlite");
            if (dbFile.Exists)
                defaultVerify = true;
            
            UpdateConnectionString(workingPath);
            
            Console.WriteLine("Working Path:" + workingPath);
            //Rules:
            //If no args passed, auto-detect mode
            //If no database file present, scan all files in folder and sub-folder, hash, record to local DB.
            //if database file present, validate all files in folder and sub-folder against saved hash, report if any changed.
            //Try to minimize external dependencies. The app shouldn't be super big.

            //NOTE: i don't need a loggable text file for output. That's the .sqlite file. There's plenty of free Sqlite readers. DB Browser for Sqlite is my preferred one.

            if (args.Any(a => a == "-quiet"))
                verbose = false;

            if (args.Any(a => a == "-dbPerFolder"))
                dbPerFolder = true;

            if (args.Any(a => a == "-add")) //Add files that weren't present previously. Intended for hard drive storage that updates occasionally, versus DVD-R or BD-R that writes once.
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                UpdateFiles(workingPath);
                sw.Stop();
                Console.WriteLine("New files addded to database in " + sw.Elapsed);
                return;
            }

            //No args were parsed, run the default assumed functionality.
            if (defaultVerify)
            {
                //Check files against DB
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                ValidateAllFiles(workingPath);
                sw.Stop();
                Console.WriteLine("All files scanned in " + sw.Elapsed);
                if (allErrors.Length > 0)
                {
                    Console.WriteLine("Errored files:");
                    Console.Write(allErrors.ToString());
                }
            }
            else
            {
                //populate with files as they are.
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                ScanAllFiles(workingPath);
                sw.Stop();
                Console.WriteLine("All files hashes and database saved in " + sw.Elapsed);
            }
        }

        static void UpdateConnectionString(string path)
        {
            conString = "Data Source=" + path + "\\Librarian.sqlite;Synchronous=Off;"; //Pragma statements go in the connection string. //Journal_Mode=MEMORY;
            var dbFile = new FileInfo(path + "\\Librarian.sqlite");
            if (!dbFile.Exists)
                CreateDatabase(); 
        }

        static void ScanAllFiles(string path)
        {
            var subfolders = Directory.EnumerateDirectories(path);
            if (subfolders.Count() > 0)
                foreach (var sf in subfolders)
                    ScanAllFiles(sf);

            if (dbPerFolder)
            {
                UpdateConnectionString(path);
            }

            RomDatabase5.Hasher hasher = new RomDatabase5.Hasher();
            var filesToParse = Directory.EnumerateFiles(path);
            foreach (var file in filesToParse)
            {
                var fi = new FileInfo(file);
                if (fi.Name == "Librarian.exe" || fi.Name == "Librarian.sqlite")
                    continue; //We dont need to scan ourselves.

                //Hash file. Save results vto DB.
                System.IO.FileStream fs = new FileStream(file, FileMode.Open);
                var hashes = hasher.HashFile(fs); //required for files over 2GB.
                //var hashes = hasher.HashFile(File.ReadAllBytes(file));
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

        static void UpdateFiles(string path)
        {
            var subfolders = Directory.EnumerateDirectories(path);
            if (subfolders.Count() > 0)
                foreach (var sf in subfolders)
                    UpdateFiles(sf);

            RomDatabase5.Hasher hasher = new RomDatabase5.Hasher();
            var filesToParse = Directory.EnumerateFiles(path);
            foreach (var file in filesToParse)
            {
                var fi = new FileInfo(file);
                if (fi.Name == "Librarian.exe" || fi.Name == "Librarian.sqlite")
                    continue; //We dont need to scan ourselves.

                string relativePath = file.Replace(workingPath, "");
                var entry = GetEntry(relativePath.Replace("'", ""));
                if (entry != null)
                    continue; //We aren't updating existing files on this call.

                //Hash file. Save results vto DB.
                System.IO.FileStream fs = new FileStream(file, FileMode.Open);
                var hashes = hasher.HashFile(fs); //required for files over 2GB.
                //var hashes = hasher.HashFile(File.ReadAllBytes(file));
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
                var fi = new FileInfo(file);
                if (fi.Name == "Librarian.exe" || fi.Name == "Librarian.sqlite")
                    continue; //We dont need to scan ourselves.

                string relativePath = file.Replace(workingPath, "");
                var entry = GetEntry(relativePath.Replace("'", ""));
                if (entry == null)
                {
                    Console.WriteLine("File " + relativePath + " Not Found in database.");
                    continue;
                }

                System.IO.FileStream fs = new FileStream(file, FileMode.Open);
                var hashes = hasher.HashFile(fs); //required for files over 2GB.
                //var hashes = hasher.HashFile(File.ReadAllBytes(file));
                if (fi.Length == entry.size && hashes[0] == entry.md5 && hashes[1] == entry.sha1 && hashes[2] == entry.crc)
                {
                    //good
                    if (verbose)
                        Console.WriteLine("File " + relativePath + " OK");
                }
                else
                {
                    allErrors.AppendLine(relativePath);
                    Console.WriteLine("File " + relativePath + " changed!");
                    if (verbose)
                    {
                        Console.WriteLine("Size: " + fi.Length + " VS " + entry.size);
                        Console.WriteLine("MD5: " + hashes[0] + " VS " + entry.md5);
                        Console.WriteLine("SHA1: " + hashes[1] + " VS " + entry.sha1);
                        Console.WriteLine("CRC: " + hashes[2] + " VS " + entry.crc);
                        Console.WriteLine("Push any key to resume scanning.");
                        Console.ReadKey();
                    }
                }
            }
        }

        static void CreateDatabase()
        {
            //Database can be one table.
            string mainTable = "CREATE TABLE files(id INTEGER PRIMARY KEY, relativePath TEXT, size INT, crc TEXT, md5 TEXT, sha1 TEXT);";
            ExecuteSQLiteNonQuery(mainTable);
            string index = "CREATE INDEX idxPath ON files(relativePath);";
            ExecuteSQLiteNonQuery(index);
        }

        static void InsertEntry(string path, long size, string md5, string sha1, string crc)
        {
            string insert = "INSERT INTO files(relativePath,  size, crc, md5, sha1)"
                         + "VALUES(@path,  @size, @crc, @md5, @sha1)";
            List<SQLiteParameter> p = new List<SQLiteParameter>();
            p.Add(new SQLiteParameter("@path", path.Replace(workingPath, "").Replace("'", ""))); //Save relative path.
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
                cmd.Dispose();
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
                cmd.Dispose();
                connection.Close();
                connection.Dispose();
            }
        }

        public static Entry FindEntry(string query)
        {
            Entry e = null;
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(query, connection);
                var results = cmd.ExecuteReader();

                if (results != null)
                    e = MapEntry(results);

                results.Close(); //This specifically unlocks the DB after a query for writing.
                cmd.Dispose();
                connection.Close();
                connection.Dispose();
            }
            return e;
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
