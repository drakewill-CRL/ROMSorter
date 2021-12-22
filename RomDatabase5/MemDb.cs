using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RomDatabase5
{

    public class FileEntry
    {
        public string name { get; set; }
        //public string description { get; set; }
        public string datfile { get; set; }
        public HashResults hashes { get; set; }
        public DiscEntry parentDisc {get;set;}
    }

    public class DiscEntry
    {
        public string name { get; set; }
        //public string description { get; set; }
        public string datfile { get; set; }
        public List<FileEntry> files { get; set; }

        public DiscEntry()
        {
            files = new List<FileEntry>();
        }
    }

    public class MemDb
    {
        //Next refactor of the DB logic.
        //This one will:
        //-Skip SQLite, and remain entirely in memory -OK
        //-Tracks Discs and Files separate, with Discs having Files inside them -OK
        // (so Files are Games renamed)
        //-check sub-folders if necessary on paths.
        //-Optimally use dictionaries or lookups as indexes.- OK
        //-will report progress via a Progress<string> object. 
        //-will probably replace DatImporter AND Sorter entirely. -OK?

        List<FileEntry> files = new List<FileEntry>();
        List<DiscEntry> discs = new List<DiscEntry>();

        ILookup<string, FileEntry> fileCRCs;
        ILookup<string, FileEntry> fileMD5s;
        ILookup<string, FileEntry> fileSHA1s;

        //ILookup<string, DiscEntry> discCRCs;
        //ILookup<string, DiscEntry> discMD5s;
        //ILookup<string, DiscEntry> discSHA1s;

        public MemDb()
        {

        }

        public async void loadDatFile(string datfile, IProgress<string> progress)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var dat = new System.Xml.XmlDocument();
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(datfile))
            using (var viewStream = mmf.CreateViewStream())
            {
                dat.Load(viewStream);
            }
            var entries = dat.GetElementsByTagName("game"); //has unique games to find. ROM has each file
            if (entries.Count == 0)
                entries = dat.GetElementsByTagName("machine"); //MAME support requires machine, but data is still in rom entries under it.
            if (entries.Count == 0)
            {
                progress.Report("No usable entries found in dat file " + datfile);
                return;
            }

            foreach (XmlElement entry in entries)
            {
                var roms = entry.SelectNodes("rom");
                if (roms.Count == 1)
                {
                    //file
                    XmlElement rom = (XmlElement)roms[0];
                    FileEntry fe = new FileEntry();
                    fe.name = rom.GetAttribute("name");
                    //fe.description = entry.GetAttribute("name"); //Leaving out for clarity, since single-file games won't need this.
                    fe.datfile = datfile;
                    HashResults hashes = new HashResults();
                    hashes.size = Int64.Parse(rom.GetAttribute("size"));
                    hashes.crc = rom.GetAttribute("crc").ToLower();
                    hashes.sha1 = rom.GetAttribute("sha1").ToLower();
                    hashes.md5 = rom.GetAttribute("md5").ToLower();
                    fe.hashes = hashes;

                    files.Add(fe);
                }
                else
                {
                    //disc
                    DiscEntry de = new DiscEntry();
                    de.name = entry.GetAttribute("name"); //Will be the folder/zip name of all the files contained.
                    //de.description = entry.GetAttribute("description");
                    de.datfile = datfile;
                    foreach (XmlElement rom in roms)
                    {
                        FileEntry fe = new FileEntry();
                        fe.name = entry.GetAttribute("name");
                        //fe.description = rom.GetAttribute("name");
                        fe.datfile = datfile;
                        HashResults hashes = new HashResults();
                        hashes.size = Int64.Parse(rom.GetAttribute("size"));
                        hashes.crc = rom.GetAttribute("crc").ToLower();
                        hashes.sha1 = rom.GetAttribute("sha1").ToLower();
                        hashes.md5 = rom.GetAttribute("md5").ToLower();
                        fe.hashes = hashes;
                        fe.parentDisc = de;
                        de.files.Add(fe);
                        files.Add(fe);
                    }
                }
            }

            //optimize lookups.
            fileCRCs = files.ToLookup(k => k.hashes.crc, v => v);
            fileMD5s = files.ToLookup(k => k.hashes.md5, v => v);
            fileSHA1s = files.ToLookup(k => k.hashes.sha1, v => v);

            sw.Stop();
            progress.Report("Loaded " + datfile + " in " + sw.Elapsed);
        }

        public FileEntry findFile(HashResults hash)
        {
            //This should probably return an empty entry rather than null.
            FileEntry emptyresults = new FileEntry();
            //NOTE: TOSEC and No-Intro use all 3 hashes. MAME skips MD5
            var crcMatches = fileCRCs[hash.crc];
            var md5Matches = fileMD5s[hash.md5];
            var sha1Matches = fileSHA1s[hash.sha1];

            //if all are empty, this file isnt a match
            if (crcMatches.Count() == 0 && md5Matches.Count() == 0 && sha1Matches.Count() == 0)
                return emptyresults;

            //if all 3 match, we are good
            var allMatches = crcMatches.Intersect(md5Matches).Intersect(sha1Matches);
            if (allMatches != null && allMatches.Count() == 1) 
                return allMatches.First();

            //ok, NOW we have some problems. Either a collision, or a missing hash entry in a dat file.
            //TODO: work out how to determine a resolution
            return emptyresults;
        }

        public DiscEntry findDisc(List<HashResults> hashes)
        {
            //NOTE: MAME does not use MD5s, so I MUST be able to find a game where a hash is empty. TOSEC and NOINTRO use all 3 hashes.
            DiscEntry emptyresults = new DiscEntry();

            //In addition to MAME, we might have a case like SCUMMVM, where there are several languages for a game
            //where MOST of the files across them are identical, but some aren't.  So we need to not bail immediately if we have mulitple matches.

            //Also cases for bin/cue files, where i will expect multiple files for a result and will want to rename them.
            List<DiscEntry> possibleMatches = new List<DiscEntry>();
            foreach (var hash in hashes)
            {
                //see if this file is a match with anything, and if so track it.
                var crcMatches = fileCRCs[hash.crc];
                //var md5Matches = fileMD5s[hash.md5];
                var sha1Matches = fileSHA1s[hash.sha1];

                var matchedFile = crcMatches.Intersect(sha1Matches);
                if (matchedFile.Count() == 0)
                    return emptyresults;

                if (matchedFile.Count() == 1)
                {
                    //We have it narrowed down to 1, lets check if all of the entries on its discentry match our hashes for an identical set.
                    var likelyDisc = matchedFile.First().parentDisc;
                    if (likelyDisc.files.All(f => hashes.Contains(f.hashes)))
                        return likelyDisc;
                }
                else
                {
                    //we need to keep going to narrow this down, but we can use this as a start.
                }               
            }           

            //ok, NOW we have some problems. Either a collision, or a missing hash entry in a dat file.
            //TODO: work out how to determine a resolution
            return emptyresults;
        }
    }
}
