using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomDatabase5
{
    internal class ZipHelper
    {
        public static void RezipFromArchive(SharpCompress.Archives.IArchive existingZip, System.IO.Compression.ZipArchive zf)
        {
            foreach (var ez in existingZip.Entries)
            {
                if (!ez.IsDirectory)
                {
                    var tempFile = Path.GetTempFileName();
                    ez.WriteToFile(tempFile);
                    zf.CreateEntryFromFile(tempFile, ez.Key);
                    File.Delete(tempFile);
                }
            }
        }

        public static List<string> FindBinsInCue(string cueFile)
        {
            //load file as lines
            //find lines like: FILE "1Xtreme (USA).bin" BINARY
            //remove ends, find all files in middle.
            string[] lines = File.ReadAllLines(cueFile);
            List<string> files = new List<string>();
            foreach (var line in lines)
            {
                var parts = line.Split('"');
                if (parts.Length != 3)
                    continue;

                var filename = parts[1];
                files.Add(filename);
            }

            return files;
        }
    }
}
