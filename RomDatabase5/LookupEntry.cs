using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomDatabase5
{
    public class LookupEntry //placeholder to track which files or zip entries need moved where
    {
        public string originalFileName; //location on disk before processing
        public string destinationFileName; //final location to be moved to
        public LookupEntryType fileType; // treat the file normally, or is it a zip/rar/etc entry?
        public string entryPath; //in a zip file, the entry's full name/path internally.

        public bool isIdentified = false; //have we identified this game?
        public string console; //the console for the game in question.
        public bool isDiscEntry = false; // should be treated differently when zipping files
        public string discEntryName; //the name of the entry in a zip archive to save this file too when zipping.
        public string discGameName; //the zip file name to use for a disc when zipping files

        //For use during processing to track info on the file without multiple reads.
        public string crc;
        public string md5;
        public string sha1;
        public long size;
    }

    public enum LookupEntryType
    {
        File = 1,
        ZipEntry,
        RarEntry,
        GZipEntry,
        TarEntry,
        SevenZEntry
    }
}
