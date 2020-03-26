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
        //TODO: reorganize code to make 'games' single-file entries, 'discs' multi-file entries inside folders. Can then update DATImporter to automatically save game info to the right spot.
        //TODO: Apply some math and logic to attempt to find 'headerless' NES ROMs? (If the file isn't divisble by 8192, skip the first 16 bytes, re-hash, re-search to see if there's a headerless entry in the DB) - NES ONLY.        
        //TODO: enable 'zip files at destination' checkbox afer implementing code to zip instead of copy files.
        //TODO: make console table (done), change console field to INTEGER to reference it via join (may not be best SQLite behavior)
        //TODO: make a crawler to try and fill in data on games where i can search
        //TODO: Write an autoguess routine to attempt to fill in some extra data columns. Might need a separate database for manually edited entries and apply those changes after guessing. Ex: set genre to 'edutainment' if name has 'learning' in it, racing for racing, sports for ball, etc.
        //TODO: Keep the UI simple. List big counts and big buttons for easy stuff. Maybe a couple drop downs for how to sort games. Checkboxes for toggles.
        //TODO: Track data sources contained in the database? For external reference and credit purposes.
        //TODO: Reference tables. Set up reference tables for genre, console, other columns with repeated data.
        //TODO Set up entities to see if that can save me some time in the future instead of writing boilerplate database code
        //TODO: use prepared statements and recycle SQLiteCOmmand objects, changing parameters where possible.
        //TODO: make the UI use native windows components to look a little more modern. Not totally sure how .NET does that.
        //TODO: figure out how to handle files over 2GB in size? This would be for ISOs for modern systems.
        //TODO enable scanning a folder for duplicate files. (For making dats, mostly, so i dont have to manually find dupes)
        //TODO: do c64 games need more than 1 file? might need to set them up more like Discs than Games in that case.
        //TODO: include checking for discs in the search. Currently only does Games.
        //TODO: make a way for a user to make a dat file for games they had unidentified for potential inclusion?
        //TODO: Filter down database to only bother with games. I don't need applications for every single various home PC?
        //--Maybe I should start by white-listing dats instead of blacklisting them?
        //TODO: find dat files for modern systems. TOSEC has approx. nothing on PSX (got Switch, need to parse and re-format)
        //TODO: Reporter will also need to look into zip files if the checkbox is selected.
        //NOT TODO: if I use description as filename all the time, I could consolidate this down to one table, but I'd have to add some processing logic to everything to figure out if i need a single file or multiples that way. Lets not do this
        //TODO: do I need to make sure discs can support sub-folders for SCUMMVM? I suspect I do.

        //TOSEC files were 12-24-2019 release.
        //NO-INTRO files were  gathered on the date listed, should still be in the filename

        //Current systems documented:
        //Nintendo:
        //NES/Famicom (TOSEC and NOINTRO)
        //FAmicom Disk System (TOSEC and NOINTRO)
        //SNES (TOSEC and NOINTRO)
        //Sufami Turbo (NOINTRO) - whatever this is.
        //Satellaview (NOINTRO)
        //N64 and DD (TOSEC and NOINTRO)
        //GameCube (TOSEC, ISO files treated as Games not Discs)
        //GB (TOSEC and NOINTRO)
        //GBC (TOSEC and NOINTRO)
        //GBA (TOSEC and NOINTRO)
        //VB (TOSEC and NOINTRO)
        //Wii (NO-Intro 3/21, WAD format)
        //DS (NOINTRO)
        //DSi (NOINTRO, datfile structure is different from others. Description is subnode, name is mostly numbers)
        //3DS (NOINTRO, Digital-CDN is Discs)
        //NEW 3DS (NOINTRO)

        //Sega:
        //Master System. (TOSEC and NoIntro)
        //GameGear (TOSEC and NOINTRO)
        //Genesis-megadrive (TOSEC, minus multipart, and NoIntro)
        //32X (TOSEC and NOINTRO)

        //SNK:
        //NGP (TOSEC and NOIntro)
        //NGPC (TOSEC and NOINTRO)

        //Tiger:
        //game.com (TOSEC and NOINTRO)

        //Additional, self-made Dats currently in DB
        //Tecmo Bowl hacks (several not previously documented, see if there's newer stuff somewhere)
        //NES Homebrews (several not previously documented) Check itch.io for more.
        //NES Prototypes (newer discoveries, see HiddenPalace.org for newer dumps than these maintainers have done. Check multiple pages (As table seems to have no NES entries after P, but By Console has lots more)
        //--Also need to dig through their 'Unused Files' page to see what files might be uploaded and not linked to correctly.
        //SCUMMVM 2.1 
        //Future Pinball (in process) from Pleasuredome torrent
        //Visual Pinball (in process) from pleasuredome torrent.


        #region SQL Commands
        //All my command text is stored up here for referencing elsewhere.

        static string CreateGamesTable = "CREATE TABLE games(id INTEGER PRIMARY KEY, "
            + "name TEXT, "
            + "description TEXT, "
            + "size INT, "
            + "crc TEXT, "
            + "md5 TEXT, "
            + "sha1 TEXT, "
            + "console TEXT, "
            + "genre TEXT, "
            + "year INT, "
            + "releaseDate TEXT, "
            + "developer TEXT, "
            + "publisher TEXT, "
            + "Is1G1R INT,"
            + "region TEXT"  //,
            //+ "CONSTRAINT hashes_unique UNIQUE(crc, md5, sha1)"
            + ")";

        //Currently identical in structure, name and description are used differently.
        static string CreateDiscsTable = "CREATE TABLE discs(id INTEGER PRIMARY KEY, "
            + "name TEXT, "
            + "description TEXT, "
            + "size INT, "
            + "crc TEXT, "
            + "md5 TEXT, "
            + "sha1 TEXT, "
            + "console TEXT, "
            + "genre TEXT, "
            + "year INT, "
            + "releaseDate TEXT, "
            + "developer TEXT, "
            + "publisher TEXT, "
            + "Is1G1R INT,"
            + "region TEXT"
            + ")";

        static string DropGameTable = "DROP TABLE IF EXISTS games";
        static string DropDiscTable = "DROP TABLE IF EXISTS discs";

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


        static string InsertGameCmd = "INSERT INTO games(name, description, size, crc, md5, sha1, console, genre, year, releaseDate, developer, publisher, Is1G1R, region)"
                        + "VALUES(@name, @description, @size, @crc, @md5, @sha1, @console, @genre, @year, @releaseDate, @developer, @publisher, @Is1G1R, @region)";

        static string InsertDiscCmd = "INSERT INTO discs(name, description, size, crc, md5, sha1, console, genre, year, releaseDate, developer, publisher, Is1G1R, region)"
                + "VALUES(@name, @description, @size, @crc, @md5, @sha1, @console, @genre, @year, @releaseDate, @developer, @publisher, @Is1G1R, @region)";

        static string CountGamesCmd = "SELECT COUNT(*) FROM games";
        static string CountGamesByConsoleCmd = "SELECT console, COUNT(console) FROM games GROUP BY console";

        static string CountDiscsCmd = "SELECT COUNT(DISTINCT name) FROM discs";
        static string CountDiscsByConsoleCmd = "SELECT console, COUNT(DISTINCT name) FROM discs GROUP BY console";
        //Total game count is CountGamesCmd + CountDiscsCmd, but SQLite doesn't do outer joins.

        static string FindGameQuery = "SELECT * FROM games WHERE size = @size AND crc = @crc AND md5= @md5 AND sha1 = @sha1";
        static string GetAllGamesQuery = "SELECT * FROM GAMES";

        static string GamesByConsoleQuery = "SELECT * FROM games WHERE console = @console";

        static string GetConsolesQuery = "SELECT DISTINCT console FROM games";
        static string GetDiscConsolesQuery = "SELECT DISTINCT console FROM discs";


        static string FindCollisionsCountCRC = "SELECT crc, COUNT(crc) FROM games GROUP BY crc ORDER BY 2 DESC";
        static string FindCollisionsDetails = "SELECT * FROM games GROUP BY crc ORDER BY 2";

        static string Set1G1RByID = "UPDATE games SET Is1G1R = 1 WHERE id = @id";

        //Note 1: INTEGER PRIMARY KEY is long instead of int. other INTEGERS might be too
        public static string conString = "Data Source=RomDB.sqlite;Synchronous=Off;Journal_Mode=MEMORY;"; //Pragman statements go in the connection string.
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

            //MakeIndexes(); //Doing this now, since after this i will be searching.
            
            //add lookup tables for genre and/or other columns?
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
            p.Add(new SQLiteParameter("@console", g.console));
            p.Add(new SQLiteParameter("@genre", g.genre));
            p.Add(new SQLiteParameter("@year", g.year));
            p.Add(new SQLiteParameter("@releaseDate", g.releaseDate));
            p.Add(new SQLiteParameter("@developer", g.developer));
            p.Add(new SQLiteParameter("@publisher", g.publisher));
            p.Add(new SQLiteParameter("@Is1G1R", g.Is1G1R));
            p.Add(new SQLiteParameter("@region", g.region));
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
                        cmd.Parameters.Add(new SQLiteParameter("@console", g.console));
                        cmd.Parameters.Add(new SQLiteParameter("@genre", g.genre));
                        cmd.Parameters.Add(new SQLiteParameter("@year", g.year));
                        cmd.Parameters.Add(new SQLiteParameter("@releaseDate", g.releaseDate));
                        cmd.Parameters.Add(new SQLiteParameter("@developer", g.developer));
                        cmd.Parameters.Add(new SQLiteParameter("@publisher", g.publisher));
                        cmd.Parameters.Add(new SQLiteParameter("@Is1G1R", g.Is1G1R));
                        cmd.Parameters.Add(new SQLiteParameter("@region", g.region));
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
                        cmd.Parameters.Add(new SQLiteParameter("@console", g.console));
                        cmd.Parameters.Add(new SQLiteParameter("@genre", g.genre));
                        cmd.Parameters.Add(new SQLiteParameter("@year", g.year));
                        cmd.Parameters.Add(new SQLiteParameter("@releaseDate", g.releaseDate));
                        cmd.Parameters.Add(new SQLiteParameter("@developer", g.developer));
                        cmd.Parameters.Add(new SQLiteParameter("@publisher", g.publisher));
                        cmd.Parameters.Add(new SQLiteParameter("@Is1G1R", g.Is1G1R));
                        cmd.Parameters.Add(new SQLiteParameter("@region", g.region));
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
                cmd.CommandText = CreateIndex1G1R;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexGenre;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexReleaseDate;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexYear;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexIdentity;
                cmd.ExecuteNonQuery();

                //add indexes for discs.
                cmd.CommandText = CreateIndexNameD;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexConsoleD;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndex1G1RD;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexGenreD;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexReleaseDateD;
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateIndexYearD;
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
                g.genre = reader[8].ToString();
                g.year = Int32.Parse(reader[9].ToString());
                g.releaseDate = reader[10].ToString();
                g.developer = reader[11].ToString();
                g.publisher = reader[12].ToString();
                g.Is1G1R = Int32.Parse(reader[13].ToString());
                g.region = reader[14].ToString();
                results.Add(g);
            }

            return results;
        }

        public static int CountGames(IProgress<string> progress)
        {
            int results = Int32.Parse(ExecuteSQLiteScalarQuery(CountGamesCmd).ToString());
            results += Int32.Parse(ExecuteSQLiteScalarQuery(CountDiscsCmd).ToString());
            if (progress != null)
                progress.Report(results +  " games detectable");
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
                        int newTotal =  existingEntry.Item2 + Int32.Parse(results1[1].ToString());

                        results1.Remove(existingEntry);
                        results1.Add(new Tuple<string, int>(results[0].ToString(), newTotal));
                    }
                    else
                        results1.Add(new Tuple<string, int>(results[0].ToString(), Int32.Parse(results[1].ToString())));
                }

            }
            return results1;
            
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

        public static List<Tuple<Object, Object>> FindCollisions()
        {
            //TODO: check other fields for collisions too
            var results = ExecuteSQLiteQueryAsTuple(FindCollisionsCountCRC);
            var a = 1;

            var b = results.Where(r => (int)r.Item2 > 1).ToList();

            //var results = ExecuteSQLiteQueryAsGameList(FindCollisionsDetails);
            return results;
        }

        public static void SetGameAs1G1R(int id)
        {
            List<SQLiteParameter> p = new List<SQLiteParameter>();
            p.Add(new SQLiteParameter("@id", id));
            ExecuteSQLiteNonQueryWithParameters(Set1G1RByID, p);
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

        public static List<string> GetAllConsoles()
        {
            using (var connection = new SQLiteConnection(conString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(GetConsolesQuery, connection);
                var values = cmd.ExecuteReader();
                List<string> results = new List<string>();
                while (values.Read())
                    results.Add(values[0].ToString());
                return results;
            }
        }
    }
}
