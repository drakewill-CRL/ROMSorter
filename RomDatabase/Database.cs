//using Microsoft.Data.Sqlite; //uses the lowercase Sql classes
using System.Data.SQLite; //uses uppercase SQL classes
using System;
using System.Collections.Generic;
using System.Xml;
using RomSorter.Data;
using System.Linq;

namespace RomDatabase
{
    public class Database
    {
        //TODO: set up app to create DB and loads dat from an internet location if DB is missing. (Maybe 1 URL for current file and date, 1 URL for actual file. This would let me update apps remotely)
        //TODO: allow users to load files themselves in case they want to use their own set. Then I have to handle parsing filenames though.
        //TODO: remember window position on close, restore on start.
        //TODO: Apply some math and logic to attempt to find 'headerless' NES ROMs? (If the file isn't divisble by 8192, skip the first 16 bytes, re-hash, re-search to see if there's a headerless entry in the DB) - NES ONLY.        
        //TODO: Keep the UI simple. List big counts and big buttons for easy stuff. Maybe a couple drop downs for how to sort games. Checkboxes for toggles.
        //TODO: Reference tables. Set up reference tables for genre, console, other columns with repeated data.
        //TODO Set up entities to see if that can save me some time in the future instead of writing boilerplate database code. Or remove the EF as a dependency
        //TODO: use prepared statements and recycle SQLiteCOmmand objects, changing parameters where possible.
        //TODO: make the UI use native windows components to look a little more modern. Not totally sure how .NET does that.
        //TODO: figure out how to handle files over 2GB in size? This would be for ISOs for modern systems.
        //TODO enable scanning a folder for duplicate files. (For making dats, mostly, so i dont have to manually find dupes)
        //TODO: make a way for a user to make a dat file for games they had unidentified for potential inclusion?
        //TODO: Reporter will also need to look into zip files if the checkbox is selected.
        //TODO: reporter needs to support discs as well as games. Only scanning by folder name for consoles available in the discs table might help?
        //NOT TODO: if I use description as filename all the time, I could consolidate this down to one table, but I'd have to add some processing logic to everything to figure out if i need a single file or multiples that way. Lets not do this
        //TODO: add .rar support (for reading)
        //TODO: add .7z support (for reading)?
        //TODO: add high-integrity disc dat reading. If an entry is already found in 1 game, check to see if all of its entries match on size/hashes.
        //TODO: make dat cleaner, to remove entries that are already tracked in an earlier file
        //TODO: submit NES Homebrew data to BizHawk file in Assets/gamedb. Current file is empty. Possibly also TOSEC for their Demo file.
        //TODO: https://api.thegamesdb.net/#/Games/GamesByGameID has all the stuff I initially wanted to include. So I guess I can skip all that stuff for a while.
        //TODO: code cleanup. Pare down files to used functions and remove commented code
        //TODO: Set up app to read from zipped DB file (zipped is ~200MB currently, instead of ~500MB)
        //TODO: redo reporting. Make it use an HTML, and substitute in StringBuilder results instead of this small text file dump.

        //TOSEC files were 12-24-2019 release.
        //NO-INTRO files were  gathered on the date listed, should still be in the filename
        //Redump.org files for cd-based systems (end of march 2020ish)

        //See the Vetted Dats folder for what's tracked. Too much stuff to list in this file.

        //stuff to track down
        //non-NES homebrew on NESWorld.com

        //TO-add:
        //NOne of my Sega CD games are good?

        //Additional, self-made Dats currently in DB
        //Tecmo Bowl hacks (several not previously documented, see if there's newer stuff somewhere)
        //NES Homebrews (several not previously documented) Check itch.io for more.
        //NES Prototypes (newer discoveries, see HiddenPalace.org for newer dumps than these maintainers have done. Check multiple pages (As table seems to have no NES entries after P, but By Console has lots more)
        //SMS Prototypes (brand new one!)
        //SEga CD Prototypes (no one had Penn and Teller yet)
        //Hidden Palace Prototypes - I have a bunch already from 2008, mostly Sega, should check if they're documented already. Those are.
        //--Also need to dig through their 'Unused Files' page to see what files might be uploaded and not linked to correctly.
        //SCUMMVM 2.1 
        //Future Pinball (in process) from Pleasuredome torrent. Needs to be sorted into single-file and multi-file tables. Not just distribution packs
        //Visual Pinball (in process) from pleasuredome torrent.
        //IFArchives ZCode game collection 
        //IFArchives other parser games (todo)


        //Starting to feel like my goal is going to be to document all the games, even the forgotten ones and fan-made stuff that might be neglected to archive or collect.
        //Which is important if you arent just being a major pirate.


        #region SQL Commands
        //All my command text is stored up here for referencing elsewhere.

        static string CreateGamesTable = "CREATE TABLE games(id INTEGER PRIMARY KEY, "
            + "name TEXT, "
            + "description TEXT, "
            + "size INT, "
            + "crc TEXT, "
            + "md5 TEXT, "
            + "sha1 TEXT, "
            + "console INT, " //was text
            + "datFile INT " //was text
                             //+ "genre TEXT, "
                             //+ "year INT, "
                             //+ "releaseDate TEXT, "
                             //+ "developer TEXT, "
                             //+ "publisher TEXT, "
                             //+ "Is1G1R INT,"
                             //+ "region TEXT"
            + ")";

        //Currently identical in structure, name and description are used differently.
        static string CreateDiscsTable = "CREATE TABLE discs(id INTEGER PRIMARY KEY, "
            + "name TEXT, "
            + "description TEXT, "
            + "size INT, "
            + "crc TEXT, "
            + "md5 TEXT, "
            + "sha1 TEXT, "
            + "console INT, " //was text
            + "datFile INT " //was text
                             //+ "genre TEXT, "
                             //+ "year INT, "
                             //+ "releaseDate TEXT, "
                             //+ "developer TEXT, "
                             //+ "publisher TEXT, "
                             //+ "Is1G1R INT,"
                             //+ "region TEXT"
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
        static string CreateIndexGenre = "CREATE INDEX idx_gamegenre ON games(genre)";
        static string CreateIndexYear = "CREATE INDEX idx_gameyear ON games(year)";
        static string CreateIndexReleaseDate = "CREATE INDEX idx_gamedate ON games(releaseDate)";
        static string CreateIndex1G1R = "CREATE INDEX idx_game1G1R ON games(Is1G1R)";
        static string CreateIndexIdentity = "CREATE INDEX idx_gameidentity ON games(size, crc, sha1, md5)";

        static string CreateIndexNameD = "CREATE INDEX idx_discname ON discs(name)";
        static string CreateIndexConsoleD = "CREATE INDEX idx_discconsole ON discs(console)";
        static string CreateIndexGenreD = "CREATE INDEX idx_discgenre ON discs(genre)";
        static string CreateIndexYearD = "CREATE INDEX idx_discyear ON discs(year)";
        static string CreateIndexReleaseDateD = "CREATE INDEX idx_discdate ON discs(releaseDate)";
        static string CreateIndex1G1RD = "CREATE INDEX idx_disc1G1R ON discs(Is1G1R)";
        static string CreateIndexIdentityD = "CREATE INDEX idx_discidentity ON discs(size, crc, sha1, md5)";


        static string InsertGameCmd = "INSERT INTO games(name, description, size, crc, md5, sha1, console, datFile)" // genre, year, releaseDate, developer, publisher, Is1G1R, region)"
                        + "VALUES(@name, @description, @size, @crc, @md5, @sha1, @console, @datFile)"; // @genre, @year, @releaseDate, @developer, @publisher, @Is1G1R, @region)";

        static string InsertDiscCmd = "INSERT INTO discs(name, description, size, crc, md5, sha1, console, datFile)" // genre, year, releaseDate, developer, publisher, Is1G1R, region)"
                        + "VALUES(@name, @description, @size, @crc, @md5, @sha1, @console, @datFile)"; // @genre, @year, @releaseDate, @developer, @publisher, @Is1G1R, @region)";

        static string InsertConsoleCmd = "INSERT INTO consoles(name) VALUES (@name)";
        static string InsertDatFileCmd = "INSERT INTO datfiles(name) VALUES (@name)";

        static string CountGamesCmd = "SELECT COUNT(*) FROM games";
        static string CountGamesByConsoleCmd = "SELECT c.name, COUNT(g.console) FROM games g INNER JOIN consoles c on c.id = g.console GROUP BY g.console";

        static string CountDiscsCmd = "SELECT COUNT(DISTINCT name) FROM discs";
        static string CountDiscsByConsoleCmd = "SELECT c.name, COUNT(d.console) FROM discs d INNER JOIN consoles c on c.id = d.console GROUP BY d.console";
        //Total game count is CountGamesCmd + CountDiscsCmd, but SQLite doesn't do outer joins.

        static string FindGameQuery = "SELECT g.id, g.name, g.description, g.size, g.crc, g.sha1, g.md5, c.name, d.name FROM games g INNER JOIN consoles c on c.id = g.console INNER JOIN datfiles d on d.id = g.datfile WHERE g.size = @size AND g.crc = @crc AND g.md5= @md5 AND g.sha1 = @sha1";
        static string GetAllGamesQuery = "SELECT g.id, g.name, g.description, g.size, g.crc, g.sha1, g.md5, c.name, d.name FROM games g INNER JOIN consoles c on c.id = g.console INNER JOIN datfiles d on d.id = g.datfile ORDER BY console, name";

        static string FindDiskFileQuery = "SELECT * FROM discs WHERE size = @size AND crc = @crc AND md5= @md5 AND sha1 = @sha1";
        static string GetAllDiskFilesQuery = "SELECT * FROM discs ORDER BY console, name, description";

        static string GamesByConsoleQuery = "SELECT * FROM games WHERE console = @console";

        static string GetConsolesQuery = "SELECT * FROM consoles";
        //static string GetDiscConsolesQuery = "SELECT DISTINCT console FROM discs";
        static string GetDatfileQuery = "SELECT * FROM datfiles";

        static string FindCollisionsCountCRC = "SELECT crc, COUNT(crc) FROM games GROUP BY crc ORDER BY 2 DESC";
        static string FindCollisionsDetails = "SELECT * FROM games GROUP BY crc ORDER BY 2";

        //static string Set1G1RByID = "UPDATE games SET Is1G1R = 1 WHERE id = @id";

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
            //p.Add(new SQLiteParameter("@genre", g.genre));
            //p.Add(new SQLiteParameter("@year", g.year));
            //p.Add(new SQLiteParameter("@releaseDate", g.releaseDate));
            //p.Add(new SQLiteParameter("@developer", g.developer));
            //p.Add(new SQLiteParameter("@publisher", g.publisher));
            //p.Add(new SQLiteParameter("@Is1G1R", g.Is1G1R));
            //p.Add(new SQLiteParameter("@region", g.region));
            ExecuteSQLiteNonQueryWithParameters(InsertGameCmd, p);
        }

        public static void InsertGamesBatch(List<Game> games)
        {
            //breaking the reusable pattern for performance here.
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
                        //cmd.Parameters.Add(new SQLiteParameter("@genre", g.genre));
                        //cmd.Parameters.Add(new SQLiteParameter("@year", g.year));
                        //cmd.Parameters.Add(new SQLiteParameter("@releaseDate", g.releaseDate));
                        //cmd.Parameters.Add(new SQLiteParameter("@developer", g.developer));
                        //cmd.Parameters.Add(new SQLiteParameter("@publisher", g.publisher));
                        //cmd.Parameters.Add(new SQLiteParameter("@Is1G1R", g.Is1G1R));
                        //cmd.Parameters.Add(new SQLiteParameter("@region", g.region));
                        var results = cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        public static void InsertDiscsBatch(List<Game> games)
        {
            //breaking the reusable pattern for performance here.
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
                        //cmd.Parameters.Add(new SQLiteParameter("@genre", g.genre));
                        //cmd.Parameters.Add(new SQLiteParameter("@year", g.year));
                        //cmd.Parameters.Add(new SQLiteParameter("@releaseDate", g.releaseDate));
                        //cmd.Parameters.Add(new SQLiteParameter("@developer", g.developer));
                        //cmd.Parameters.Add(new SQLiteParameter("@publisher", g.publisher));
                        //cmd.Parameters.Add(new SQLiteParameter("@Is1G1R", g.Is1G1R));
                        //cmd.Parameters.Add(new SQLiteParameter("@region", g.region));
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
                //cmd.CommandText = CreateIndex1G1R;
                //cmd.ExecuteNonQuery();
                //cmd.CommandText = CreateIndexGenre;
                //cmd.ExecuteNonQuery();
                //cmd.CommandText = CreateIndexReleaseDate;
                //cmd.ExecuteNonQuery();
                //cmd.CommandText = CreateIndexYear;
                //cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexIdentity;
                cmd.ExecuteNonQuery();

                //add indexes for discs.
                cmd.CommandText = CreateIndexNameD;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexConsoleD;
                cmd.ExecuteNonQuery();
                //cmd.CommandText = CreateIndex1G1RD;
                //cmd.ExecuteNonQuery();
                //cmd.CommandText = CreateIndexGenreD;
                //cmd.ExecuteNonQuery();
                //cmd.CommandText = CreateIndexReleaseDateD;
                //cmd.ExecuteNonQuery();
                //cmd.CommandText = CreateIndexYearD;
                //cmd.ExecuteNonQuery();
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
                //g.genre = reader[8].ToString();
                //g.year = Int32.Parse(reader[9].ToString());
                //g.releaseDate = reader[10].ToString();
                //g.developer = reader[11].ToString();
                //g.publisher = reader[12].ToString();
                //g.Is1G1R = Int32.Parse(reader[13].ToString());
                //g.region = reader[14].ToString();
                results.Add(g);
            }

            return results;
        }

        public static int CountGames(IProgress<string> progress)
        {
            int results = Int32.Parse(ExecuteSQLiteScalarQuery(CountGamesCmd).ToString());
            results += Int32.Parse(ExecuteSQLiteScalarQuery(CountDiscsCmd).ToString());
            if (progress != null)
                progress.Report(results + " games detectable");
            return results;
        }

        public static List<Tuple<string, int>> CountGamesByConsole()
        {
            List<Tuple<string, int>> results1 = new List<Tuple<string, int>>();
            //var results = ExecuteSQLiteQuery(CountGamesByConsoleCmd);
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
            var a = 1;

            var b = results.Where(r => (int)r.Item2 > 1).ToList();

            //var results = ExecuteSQLiteQueryAsGameList(FindCollisionsDetails);
            return results;
        }

        //public static void SetGameAs1G1R(int id)
        //{
        //    List<SQLiteParameter> p = new List<SQLiteParameter>();
        //    p.Add(new SQLiteParameter("@id", id));
        //    ExecuteSQLiteNonQueryWithParameters(Set1G1RByID, p);
        //}

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
