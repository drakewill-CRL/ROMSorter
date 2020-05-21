using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RomDatabase5
{
    public class DatabaseEntities //EF access to database. Much less code, sometimes faster.
    {
        //Entity types are plural, my handmane ones are singular.       

        RomDBContext db = new RomDBContext();
        public ILookup<long, string> consoleIDs;
        public ILookup<long, string> datfileIDs;

        static ReaderWriterLockSlim consoleLock = new ReaderWriterLockSlim();
        static ReaderWriterLockSlim datfileLock = new ReaderWriterLockSlim();

        public DatabaseEntities()
        {
            consoleIDs = db.Consoles.ToLookup(k => k.Id, v => v.Name);
            datfileIDs = db.Datfiles.ToLookup(k => k.Id, v => v.Name);
        }

        public void InsertGame(RomDatabase5.Games g)
        {
            db.Games.Add(g);
            db.SaveChanges();
        }

        public void InsertGamesBatch(List<Games> games)
        {
            db.Games.AddRange(games);
            db.SaveChanges();
        }

        public void InsertDisc(RomDatabase5.Discs g)
        {
            db.Discs.Add(g);
            db.SaveChanges();
        }

        public void InsertDiscsBatch(List<Discs> games)
        {
            db.Discs.AddRange(games);
            db.SaveChanges();
        }

        public int CountGames()
        {
            int gameCount = db.Games.Count();
            int discCount = db.Discs.Select(d => d.Name).Distinct().Count();
            return gameCount + discCount;
        }

        public List<Tuple<string, int>> CountGamesByConsole()
        {
            var consoles = db.Consoles.ToLookup(k => k.Id, v => v.Name);
            var results = db.Games.GroupBy(g => g.Console).Select(g => new { Key = consoles[g.Key.Value].First(), Count = g.Count() }).ToList();
            var results2 = db.Discs.GroupBy(g => g.Console).Select(g => new { Key = consoles[g.Key.Value].First(), Count = g.Count() }).ToList();

            List<Tuple<string, int>> finalResults = new List<Tuple<string, int>>();

            foreach (var console in results)
            {
                if (results2.Any(r => r.Key == console.Key))
                {
                    finalResults.Add(new Tuple<string, int>(console.Key, console.Count + results2.Where(r => r.Key == console.Key).First().Count));
                }
                else
                {
                    finalResults.Add(new Tuple<string, int>(console.Key, console.Count));
                }
            }
            foreach (var console2 in results2)
            {
                if (!finalResults.Any(r => r.Item1 == console2.Key)) //don't re-add results we already put in.
                {
                    finalResults.Add(new Tuple<string, int>(console2.Key, console2.Count));
                }
            }

            return finalResults.OrderBy(fr => fr.Item1).ToList();
        }

        public List<Games> FindGame(long size, string[] hashes)
        {
            return FindGame(size, hashes[2], hashes[0], hashes[1]);
        }

        public List<Games> FindGame(long size, string crc, string md5, string sha1)
        {
            //Cascading where clauses to ensure we correctly match as much stuff as possible without failling on entries that are missing a value (or 3)
            //Pinball files use CRC + SHA1, ignore MD5.
            //MAME uses ONLY SHA1 for CHD files. Doesn't include a size either, so can't expect to use size first either.

            var matchingGames = db.Games.AsQueryable();

            if (!String.IsNullOrWhiteSpace(md5))
                matchingGames = matchingGames.Where(g => g.Md5 == md5);

            if (!String.IsNullOrWhiteSpace(sha1))
                matchingGames = matchingGames.Where(g => g.Sha1 == sha1);

            if (!String.IsNullOrWhiteSpace(crc))
                matchingGames = matchingGames.Where(g => g.Crc == crc);

            return matchingGames.ToList();
        }

        public List<Discs> FindDisc(long size, string[] hashes)
        {
            return FindDisc(size, hashes[2], hashes[0], hashes[1]);
        }

        public List<Discs> FindDisc(long size, string crc, string md5, string sha1)
        {
            List<Discs> d = db.Discs.Where(g => g.Size == size && g.Crc == crc && g.Md5 == md5 && g.Sha1 == sha1).ToList();
            return d;
        }

        public List<Games> GetAllGames()
        {
            return db.Games.ToList();
        }

        public List<Games> GetGamesByConsole(string console)
        {
            var consoleID = db.Consoles.Where(c => c.Name == console).FirstOrDefault().Id;
            return db.Games.Where(g => g.Console == consoleID).ToList();
        }

        public long GetConsoleID(string consoleName)
        {
            if (db.Consoles.Any(c => c.Name == consoleName))
                return db.Consoles.Where(c => c.Name == consoleName).FirstOrDefault().Id;
            else
            {
                consoleLock.EnterWriteLock();
                var nextID = (db.Consoles.Count() > 0 ? db.Consoles.Max(c => c.Id) + 1 : 1);
                var newConsole = new Consoles() { Name = consoleName, Id = nextID };
                db.Consoles.Add(newConsole);
                db.SaveChanges();
                consoleLock.ExitWriteLock();
                return nextID;
             }
        }

        public long GetDatFileID(string datfileName)
        {
            if (db.Datfiles.Any(c => c.Name == datfileName))
                return db.Datfiles.Where(c => c.Name == datfileName).FirstOrDefault().Id;
            else
            {
                datfileLock.EnterWriteLock();
                var nextID = (db.Datfiles.Count() > 0 ? db.Datfiles.Max(c => c.Id) + 1 : 1);
                var newFile = new Datfiles() { Name = datfileName, Id = nextID };
                db.Datfiles.Add(newFile);
                db.SaveChanges();
                datfileLock.ExitWriteLock();
                return nextID;
            }
        }

    }
}
