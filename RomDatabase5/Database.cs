using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace RomDatabase5
{
    public class Database
    {

        //Notes
        //This app is 20% faster in .NET 5 (2:02 to sort versus 2:32  in 4.5), when only copying over code and libraries directly. Impressive
        //TODO: copy over appropriate TODOs
        //TODO: personal Dats pending an update:
        //all good for now.

        #region SQL Commands
        //All my command text is stored up here for referencing elsewhere.

        static string CreateGamesTable = "CREATE TABLE games(id INTEGER PRIMARY KEY, "
            + "name TEXT, "
            + "description TEXT, "
            + "size INT, "
            + "crc TEXT, "
            + "md5 TEXT, "
            + "sha1 TEXT, "
            + "console INT, " 
            + "datFile INT " 
            + ")";

        //Currently identical in structure, name and description are used differently.
        static string CreateDiscsTable = "CREATE TABLE discs(id INTEGER PRIMARY KEY, "
            + "name TEXT, "
            + "description TEXT, "
            + "size INT, "
            + "crc TEXT, "
            + "md5 TEXT, "
            + "sha1 TEXT, "
            + "console INT, "
            + "datFile INT " 
            + ")";

        static string CreateConsoleTable = "CREATE TABLE consoles(id INTEGER PRIMARY KEY,"
            + "name TEXT"
            + ")";

        static string CreateDatFilesTable = "CREATE TABLE datfiles(id INTEGER PRIMARY KEY,"
            + "name TEXT"
            + ")";

        static string DropGameTable = "DROP TABLE IF EXISTS games";
        static string DropDiscTable = "DROP TABLE IF EXISTS discs";
        static string DropConsoleTable = "DROP TABLE IF EXISTS consoles";
        static string DropDatFilesTable = "DROP TABLE IF EXISTS datfiles";

        static string CreateIndexName = "CREATE INDEX idx_gamename ON games(name)";
        static string CreateIndexConsole = "CREATE INDEX idx_gameconsole ON games(console)";
        static string CreateIndexIdentity = "CREATE INDEX idx_gameidentity ON games(size, crc, sha1, md5)";

        static string CreateIndexNameD = "CREATE INDEX idx_discname ON discs(name)";
        static string CreateIndexConsoleD = "CREATE INDEX idx_discconsole ON discs(console)";
        static string CreateIndexIdentityD = "CREATE INDEX idx_discidentity ON discs(size, crc, sha1, md5)";


        static string InsertGameCmd = "INSERT INTO games(name, description, size, crc, md5, sha1, console, datFile)" 
                        + "VALUES(@name, @description, @size, @crc, @md5, @sha1, @console, @datFile)";

        static string InsertDiscCmd = "INSERT INTO discs(name, description, size, crc, md5, sha1, console, datFile)" 
                        + "VALUES(@name, @description, @size, @crc, @md5, @sha1, @console, @datFile)"; 

        static string InsertConsoleCmd = "INSERT INTO consoles(name) VALUES (@name)";
        static string InsertDatFileCmd = "INSERT INTO datfiles(name) VALUES (@name)";

        static string CountGamesCmd = "SELECT COUNT(DISTINCT name) FROM games";
        static string CountGamesByConsoleCmd = "SELECT c.name, COUNT(g.console) FROM games g INNER JOIN consoles c on c.id = g.console GROUP BY g.console";

        static string CountDiscsCmd = "SELECT COUNT(DISTINCT name) FROM discs";
        static string CountDiscsByConsoleCmd = "SELECT c.name, COUNT(DISTINCT d.name) FROM discs d INNER JOIN consoles c on c.id = d.console GROUP BY d.console";
        //Total game count is CountGamesCmd + CountDiscsCmd, but SQLite doesn't do outer joins.

        static string FindGameQuery = "SELECT g.id, g.name, g.description, g.size, g.crc, g.sha1, g.md5, c.name, d.name FROM games g INNER JOIN consoles c on c.id = g.console INNER JOIN datfiles d on d.id = g.datfile WHERE g.size = @size AND g.crc = @crc AND g.md5= @md5 AND g.sha1 = @sha1";
        static string GetAllGamesQuery = "SELECT g.id, g.name, g.description, g.size, g.crc, g.sha1, g.md5, c.name, d.name FROM games g INNER JOIN consoles c on c.id = g.console INNER JOIN datfiles d on d.id = g.datfile ORDER BY console, name";

        static string FindDiskFileQuery = "SELECT g.id, g.name, g.description, g.size, g.crc, g.sha1, g.md5, c.name, d.name FROM discs g INNER JOIN consoles c on c.id = g.console INNER JOIN datfiles d on d.id = g.datfile WHERE g.size = @size AND g.crc = @crc AND g.md5= @md5 AND g.sha1 = @sha1";
        static string GetAllDiskFilesQuery = "SELECT g.id, g.name, g.description, g.size, g.crc, g.sha1, g.md5, c.name, d.name FROM discs g INNER JOIN consoles c on c.id = g.console INNER JOIN datfiles d on d.id = g.datfile ORDER BY console, name";

        static string GamesByConsoleQuery = "SELECT * FROM games WHERE console = @console";

        static string GetConsolesQuery = "SELECT * FROM consoles";
        //static string GetDiscConsolesQuery = "SELECT DISTINCT console FROM discs";
        static string GetDatfileQuery = "SELECT * FROM datfiles";

        static string FindCollisionsCountCRC = "SELECT crc, COUNT(crc) FROM games GROUP BY crc ORDER BY 2 DESC";
        //static string FindCollisionsDetails = "SELECT * FROM games GROUP BY crc ORDER BY 2";

        //Note 1: INTEGER PRIMARY KEY is long instead of int. other INTEGERS might be too
        public static string conString = "Data Source=RomDB.sqlite;Synchronous=Off;Journal_Mode=MEMORY;"; //Pragma statements go in the connection string.
                                                                                                          //string conString = "Data Source=:memory:"; //Also works, doesn't write to disk.

        #endregion

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

        public static SQLiteDataReader ExecuteSQLiteQuery(string command)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(command, connection);
                var results = cmd.ExecuteReader();
                return results;
            }
        }

        public static object ExecuteSQLiteScalarQuery(string command)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(command, connection);
                var results = cmd.ExecuteScalar();
                return results;
            }
        }

        public static List<Game> ExecuteSQLiteQueryAsGameList(string command)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(command, connection);
                var results = cmd.ExecuteReader();
                var results2 = MapReaderToGame(results);
                return results2;
            }
        }

        public static List<Tuple<object, object>> ExecuteSQLiteQueryAsTuple(string command)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(command, connection);
                var results = cmd.ExecuteReader();
                var results2 = new List<Tuple<object, object>>();
                while (results.Read())
                {
                    results2.Add(new Tuple<object, object>(results[0], results[1]));
                }
                return results2;
            }
        }

        public static void RebuildInitialDatabase()
        {
            //Create up the baseline tables.
            ExecuteSQLiteNonQuery(DropGameTable);
            ExecuteSQLiteNonQuery(CreateGamesTable);

            ExecuteSQLiteNonQuery(DropDiscTable);
            ExecuteSQLiteNonQuery(CreateDiscsTable);

            ExecuteSQLiteNonQuery(DropConsoleTable);
            ExecuteSQLiteNonQuery(CreateConsoleTable);

            ExecuteSQLiteNonQuery(DropDatFilesTable);
            ExecuteSQLiteNonQuery(CreateDatFilesTable);

            MakeIndexes();
        }

        public static void InsertGame(Game g)
        {
            List<SQLiteParameter> p = new List<SQLiteParameter>();
            p.Add(new SQLiteParameter("@name", g.name));
            p.Add(new SQLiteParameter("@description", g.description));
            p.Add(new SQLiteParameter("@size", g.size));
            p.Add(new SQLiteParameter("@crc", g.crc));
            p.Add(new SQLiteParameter("@md5", g.md5));
            p.Add(new SQLiteParameter("@sha1", g.sha1));
            p.Add(new SQLiteParameter("@console", g.consoleID));
            p.Add(new SQLiteParameter("@datFile", g.datFileID));
            ExecuteSQLiteNonQueryWithParameters(InsertGameCmd, p);
        }

        public static void InsertGamesBatch(List<Game> games)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var cmd = new SQLiteCommand(InsertGameCmd, connection);
                    foreach (var g in games)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SQLiteParameter("@name", g.name));
                        cmd.Parameters.Add(new SQLiteParameter("@description", g.description));
                        cmd.Parameters.Add(new SQLiteParameter("@size", g.size));
                        cmd.Parameters.Add(new SQLiteParameter("@crc", g.crc));
                        cmd.Parameters.Add(new SQLiteParameter("@md5", g.md5));
                        cmd.Parameters.Add(new SQLiteParameter("@sha1", g.sha1));
                        cmd.Parameters.Add(new SQLiteParameter("@console", g.consoleID));
                        cmd.Parameters.Add(new SQLiteParameter("@datFile", g.datFileID));
                        var results = cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        public static void InsertDiscsBatch(List<Game> games)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var cmd = new SQLiteCommand(InsertDiscCmd, connection);
                    foreach (var g in games)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SQLiteParameter("@name", g.name));
                        cmd.Parameters.Add(new SQLiteParameter("@description", g.description));
                        cmd.Parameters.Add(new SQLiteParameter("@size", g.size));
                        cmd.Parameters.Add(new SQLiteParameter("@crc", g.crc));
                        cmd.Parameters.Add(new SQLiteParameter("@md5", g.md5));
                        cmd.Parameters.Add(new SQLiteParameter("@sha1", g.sha1));
                        cmd.Parameters.Add(new SQLiteParameter("@console", g.consoleID));
                        cmd.Parameters.Add(new SQLiteParameter("@datFile", g.datFileID));
                        var results = cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        public static void MakeIndexes()
        {
            var connection = new SQLiteConnection(conString);
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                //add indexes for games.
                var cmd = new SQLiteCommand(connection);
                cmd.CommandText = CreateIndexName;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexConsole;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexIdentity;
                cmd.ExecuteNonQuery();

                //add indexes for discs.
                cmd.CommandText = CreateIndexNameD;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexConsoleD;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexIdentityD;
                cmd.ExecuteNonQuery();

                transaction.Commit();
            }
            connection.Close();
            connection.Dispose();
        }

        public static List<Game> MapReaderToGame(SQLiteDataReader reader)
        {
            //doing manually what entities do automatically
            //take the Reader, for each row make a Game object, return List<Game> that C# can deal with easier.
            //Fill in with proper values from home.
            List<Game> results = new List<Game>();
            if (!reader.HasRows)
                return results;

            while (reader.Read())
            {
                var g = new Game();
                g.id = Int32.Parse(reader[0].ToString());
                g.name = reader[1].ToString().Replace("''", "'");
                g.description = reader[2].ToString().Replace("''", "'");
                g.size = Int32.Parse(reader[3].ToString());
                g.crc = reader[4].ToString();
                g.md5 = reader[5].ToString();
                g.sha1 = reader[6].ToString();
                g.console = reader[7].ToString();
                g.datFile = reader[8].ToString();
                results.Add(g);
            }

            return results;
        }

        public static int CountGames()
        {
            int singleFilesGames = Int32.Parse(ExecuteSQLiteScalarQuery(CountGamesCmd).ToString());
            int multiFileGames = Int32.Parse(ExecuteSQLiteScalarQuery(CountDiscsCmd).ToString());
            return singleFilesGames + multiFileGames;
        }

        public static List<Tuple<string, int>> CountGamesByConsole()
        {
            List<Tuple<string, int>> results1 = new List<Tuple<string, int>>();
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(CountGamesByConsoleCmd, connection);
                var results = cmd.ExecuteReader();

                while (results.Read())
                    results1.Add(new Tuple<string, int>(results[0].ToString(), Int32.Parse(results[1].ToString())));

                //adding discs table.
                cmd = new SQLiteCommand(CountDiscsByConsoleCmd, connection);
                results = cmd.ExecuteReader();
                while (results.Read())
                {
                    if (results1.Any(r => r.Item1 == results[0].ToString()))
                    {
                        var existingEntry = results1.Where(r => r.Item1 == results[0].ToString()).FirstOrDefault();
                        int newTotal = existingEntry.Item2 + Int32.Parse(results[1].ToString());

                        results1.Remove(existingEntry);
                        results1.Add(new Tuple<string, int>(results[0].ToString(), newTotal));
                    }
                    else
                        results1.Add(new Tuple<string, int>(results[0].ToString(), Int32.Parse(results[1].ToString())));
                }

            }
            return results1.OrderBy(r => r.Item1).ToList();

        }

        public static Game FindGame(long size, string[] hashes)
        {
            return FindGame(size, hashes[2], hashes[0], hashes[1]);
        }

        public static Game FindGame(long size, string crc, string md5, string sha1)
        {
            Game g = new Game();

            var connection = new SQLiteConnection(conString);
            connection.Open();
            var cmd = new SQLiteCommand(connection);
            cmd.CommandText = FindGameQuery;
            cmd.Parameters.Add(new SQLiteParameter("@size", size));
            cmd.Parameters.Add(new SQLiteParameter("@crc", crc));
            cmd.Parameters.Add(new SQLiteParameter("@md5", md5));
            cmd.Parameters.Add(new SQLiteParameter("@sha1", sha1));

            var results = cmd.ExecuteReader();
            if (results != null)
                g = MapReaderToGame(results).FirstOrDefault();
            return g;
        }

        public static List<Game> FindDisc(long size, string[] hashes)
        {
            return FindDisc(size, hashes[2], hashes[0], hashes[1]);
        }

        public static List<Game> FindDisc(long size, string crc, string md5, string sha1)
        {
            List<Game> g = new List<Game>();

            var connection = new SQLiteConnection(conString);
            connection.Open();
            var cmd = new SQLiteCommand(connection);
            cmd.CommandText = FindDiskFileQuery;
            cmd.Parameters.Add(new SQLiteParameter("@size", size));
            cmd.Parameters.Add(new SQLiteParameter("@crc", crc));
            cmd.Parameters.Add(new SQLiteParameter("@md5", md5));
            cmd.Parameters.Add(new SQLiteParameter("@sha1", sha1));

            var results = cmd.ExecuteReader();
            if (results != null)
                g = MapReaderToGame(results).ToList();
            return g;
        }

        public static List<Tuple<Object, Object>> FindCollisions()
        {
            //TODO: check other fields for collisions too
            var results = ExecuteSQLiteQueryAsTuple(FindCollisionsCountCRC);
            var b = results.Where(r => (int)r.Item2 > 1).ToList();

            return results;
        }

        public static List<Game> GetAllGames()
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(GetAllGamesQuery, connection);
                var values = cmd.ExecuteReader();
                List<Game> results = MapReaderToGame(values);
                return results;
            }
        }

        public static List<Game> GetGamesByConsole(string console)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(GamesByConsoleQuery, connection);
                cmd.Parameters.Add(new SQLiteParameter("@console", console));
                var values = cmd.ExecuteReader();
                List<Game> results = MapReaderToGame(values).OrderBy(g => g.name).ToList();
                return results;
            }
        }

        public static ILookup<string, Game> GetAllDiscs()
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(GetAllDiskFilesQuery, connection);
                var values = cmd.ExecuteReader();
                ILookup<string, Game> results = MapReaderToGame(values).ToLookup(k => k.name, v => v);
                return results;
            }
        }

        public static List<string> GetAllConsoles()
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(GetConsolesQuery, connection);
                var values = cmd.ExecuteReader();
                List<string> results = new List<string>();
                while (values.Read())
                    results.Add(values[1].ToString());
                return results;
            }
        }

        public static int GetConsoleID(string consoleName)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                int results = 0;
                connection.Open();
                var cmd = new SQLiteCommand(GetConsolesQuery, connection);
                var values = cmd.ExecuteReader();
                while (values.Read())
                {
                    if (values[1].ToString() == consoleName)
                        results = Int32.Parse(values[0].ToString());
                }
                if (results == 0)
                {
                    cmd = new SQLiteCommand(InsertConsoleCmd);
                    cmd.Connection = connection;
                    cmd.Parameters.Add(new SQLiteParameter("@name", consoleName));
                    cmd.ExecuteNonQuery();
                    results = GetConsoleID(consoleName);
                }

                return results;
            }
        }

        public static int GetDatFileID(string datfileName)
        {
            using (var connection = new SQLiteConnection(conString))
            {
                int results = 0;
                connection.Open();
                var cmd = new SQLiteCommand(GetDatfileQuery, connection);
                var values = cmd.ExecuteReader();
                while (values.Read())
                {
                    if (values[1].ToString() == datfileName)
                        results = Int32.Parse(values[0].ToString());
                }
                if (results == 0)
                {
                    cmd = new SQLiteCommand(InsertDatFileCmd);
                    cmd.Parameters.Add(new SQLiteParameter("@name", datfileName));
                    cmd.Connection = connection;
                    var r = cmd.ExecuteNonQuery();
                    results = GetDatFileID(datfileName);
                }

                return results;
            }
        }
    }
}
