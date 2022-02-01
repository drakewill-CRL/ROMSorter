using RomDatabase5;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Librarian5Console
{
    class Program
    {
        public static string workingPath = Directory.GetCurrentDirectory();
        public static string datFilePath = "";
        public static string conString = ""; 
        
        public static bool verbose = true;
        public static bool dbPerFolder = false;

        public static StringBuilder allErrors = new StringBuilder();

        static void Main(string[] args)
        {
            Progress<string> progress = new Progress<string>(s => Console.WriteLine(s));
            bool moveUnidentified = false;

            if (args.Count() == 0 || args.Any(a => a == "-help" || a == @"/h" || a == "-h"))
            {
                Console.WriteLine("Librarian - ROMSorter command line interface alternative");
                Console.WriteLine("Option Flags:");
                Console.WriteLine("-targetFolder:\"path\" (Required) || the folder full of files you want to operate on");
                Console.WriteLine("-datfile:\"path\" || The DAT file to use for identifying/renaming commands.");
                Console.WriteLine("-moveUnidentified || Takes files that aren't found in a DAT and moves them to a /Unidentified sub-folder.");
                Console.WriteLine("Commands:");
                Console.WriteLine(@"-help or /h or no args || Display this list of valid commands.");
                Console.WriteLine("-detectDupes || Confirm all files are unique, moves duplicates to a subfolder");
                Console.WriteLine("-unzipAll || Extracts all supported archives (zip, rar, 7z, tar, gzip) to the same folder and removes the archive.");
                Console.WriteLine("-zipAll || Zips each file to its own archive and removes the original.");
                Console.WriteLine("-catalog || Saves a list of hashes for all files to a tab-separated-values file.");
                Console.WriteLine("-verifyCatalog || Compares files to a previously generated catalog file to check for changes");
                Console.WriteLine("-makeChds || Convert bin/cue or iso files to chd. Requires chdman in the same folder as Librarian.");
                Console.WriteLine("-extractChds || Convert chd files back to their original format. Requires chdman in the same folder as Librarian.");
                Console.WriteLine("-makeDat || Create a DAT file from the given folder usable in other ROM managers");
                Console.WriteLine("-renameSingleFiles or -identify || Renames files in the folder to match their entry in the loaded DAT file");
                return;
            }

            if (args.Any(a => a.StartsWith("-targetFolder:")))
            {
                var param = args.Where(a => a.StartsWith("-targetFolder:")).First();
                workingPath = param.Substring(param.IndexOf(":") + 1, param.Length - param.IndexOf(":") - 1);
            }

            var memdb = new MemDb();
            if (args.Any(a => a.StartsWith("-datfile:")))
            {
                datFilePath = args.Where(a => a.StartsWith("-datfile:")).First();
                memdb.loadDatFile(datFilePath, progress);
            }

            if (args.Any(a => a.StartsWith("-datfile:")))
            {
                datFilePath = args.Where(a => a.StartsWith("-datfile:")).First();
                memdb.loadDatFile(datFilePath, progress);
            }

            if (args.Any(a => a == "-moveUnidentified"))
            {
                moveUnidentified = true;
            }

            Console.WriteLine("Working on: " + workingPath);
            //This app is now a command line version of ROMSorter

            if (args.Any(a => a == "-detectDupes"))
            {
                CoreFunctions.DetectDupes(progress, workingPath);
            }

            if (args.Any(a => a == "-unzipAll"))
            {
                CoreFunctions.UnzipLogic(progress, workingPath);
            }

            if (args.Any(a => a == "-zipAll"))
            {
                CoreFunctions.ZipLogic(progress, workingPath);
            }

            if (args.Any(a => a == "-catalog"))
            {
                CoreFunctions.Catalog(progress, workingPath);
            }

            if (args.Any(a => a == "-verifyCatalog"))
            {
                CoreFunctions.Verify(progress, workingPath);
            }

            if (args.Any(a => a == "-makeChds"))
            {
                if (!ChdmanCheck())
                    return;
                CoreFunctions.CreateChdLogic(progress, workingPath);
            }

            if (args.Any(a => a == "-extractChds"))
            {
                if (!ChdmanCheck())
                    return;
                CoreFunctions.ExtractChdLogic(progress, workingPath);
            }

            if (args.Any(a => a == "-makeDat"))
            {
                CoreFunctions.DatLogic(progress, workingPath);
            }

            if (args.Any(a => a == "-renameSingleFiles" || a == "-identify"))
            {
                CoreFunctions.IdentifyLogic(progress, workingPath, moveUnidentified, memdb);
            }
        }

        public static bool ChdmanCheck()
        {
            bool chdmanFound = false;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                chdmanFound = File.Exists("chdman.exe");
            else
                chdmanFound = File.Exists("chdman");

            if (chdmanFound)
                return true;

            Console.WriteLine("Did not find a usable version of chdman in the same folder as this app. Aborting command");
            return false;
        }
    }
}
