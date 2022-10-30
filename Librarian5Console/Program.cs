using RomDatabase5;
using System;
using System.Collections.Generic;
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

            List<string> regionOrder = new List<string>() {"USA", "EUR", "JPN", "" };

            if (args.Count() == 0 || args.Any(a => a == "-help" || a == @"/h" || a == "-h"))
            {
                Console.WriteLine("Librarian - ROMSorter command line interface alternative");
                Console.WriteLine("Option Flags:");
                Console.WriteLine("-targetFolder:\"path\" (Required) || the folder full of files you want to operate on");
                Console.WriteLine("-datfile:\"path\" || The DAT file to use for identifying/renaming/1G1R commands.");
                Console.WriteLine("-defaultDat || Use the built-in DAT file, sufficient for most cartridge-based system.");
                Console.WriteLine("-moveUnidentified || Takes files that aren't found in a DAT and moves them to a /Unidentified sub-folder during rename.");
                Console.WriteLine("-regionOrder: || A comma separated list of regions in order of priority. Used by -1G1R (Default is -regionOrder:USA,EUR,JPN,).");
                Console.WriteLine("Commands:");
                Console.WriteLine(@"-help or /h or no args || Display this list of valid commands.");
                Console.WriteLine("-detectDupes || Confirm all files are unique, moves duplicates to a subfolder");
                Console.WriteLine("-unzipAll || Extracts all supported archives (zip, rar, 7z, tar, gzip) to the same folder and removes the archive.");
                Console.WriteLine("-zipAll || Zips each file to its own archive and removes the original.");
                Console.WriteLine("-catalog || Saves a list of hashes for all files to a tab-separated-values file.");
                Console.WriteLine("-verifyCatalog || Compares files to a previously generated catalog file to check for changes");
                Console.WriteLine("-makeChds || Convert bin/cue or iso files to chd. Requires chdman in the same folder as Librarian.");
                Console.WriteLine("-extractChds || Convert chd files back to their original format. Requires chdman in the same folder as Librarian.");
                Console.WriteLine("-chdPlaylist || Makes M3U playlists from disc formatted files.");
                Console.WriteLine("-makeDat || Create a DAT file from the given folder usable in other ROM managers");
                Console.WriteLine("-renameSingleFiles or -identify || Renames files in the folder to match their entry in the loaded DAT file");
                Console.WriteLine("-1G1R || Pulls out a single release of each unique game based on your regionOrder preferences");
                Console.WriteLine("-everdrive || Moves files into subfolders by initial letter of the filename.");
                Console.WriteLine("-patchAll || Applies all IPS/BPS patches to the only non-patch file in the folder.");
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
                datFilePath = "ROMSorterDefault.dat";
                memdb.loadDatFile(datFilePath, progress).Wait();
            }
            else if (args.Any(a => a == "-defaultDat"))
            {
                datFilePath = "ROMSorterDefault.dat";
                memdb.loadDatFile(datFilePath, progress).Wait();
            }

            if (args.Any(a => a == "-moveUnidentified"))
            {
                moveUnidentified = true;
            }

            if(args.Any(a => a.StartsWith("-regionOrder:")))
            {
                var regionOrderParam = args.Where(a => a.StartsWith("-regionOrder:")).First().Split(":")[1];
                regionOrder = regionOrderParam.Split(",").ToList();
                if (!regionOrder.Any(r => r == ""))
                    regionOrder.Add("");
            }

            Console.WriteLine("Working on: " + workingPath);
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

            if (args.Any(a => a == "-1G1R"))
            {
                CoreFunctions.OneGameOneRomSort(progress, workingPath, memdb, regionOrder);
            }

            if (args.Any(a => a == "-everdrive"))
            {
                CoreFunctions.EverdriveSort(progress, workingPath);
            }

            if (args.Any(a => a == "-chdPlaylist"))
            {
                CoreFunctions.CreateM3uPlaylists(progress, workingPath);
            }

            if (args.Any(a => a == "-patchAll"))
            {
                CoreFunctions.ApplyAllPatches(progress, workingPath);
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
