using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace RomDatabase5
{
    public class Patcher
    {
        static string lastOutputMessage;
        private static void PathOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            lastOutputMessage = outLine.Data;
        }

        public static bool PatchWithFlips(string patch, string rom)
        {
            string romExt = System.IO.Path.GetExtension(rom);
            string command = "--apply \"" + patch + "\" \"" + rom + "\" \"" + patch.Replace(Path.GetExtension(patch), romExt) + "\"";

            Process flips = new Process();
            flips.StartInfo.FileName = "flips.exe";
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                flips.StartInfo.FileName = "flips-linux";
            flips.StartInfo.Arguments = command;
            flips.StartInfo.UseShellExecute = false;
            flips.StartInfo.CreateNoWindow = true;
            flips.StartInfo.RedirectStandardOutput = true;
            flips.StartInfo.RedirectStandardError = true;
            flips.OutputDataReceived += PathOutputHandler;
            flips.ErrorDataReceived += PathOutputHandler;

            flips.Start();
            flips.BeginErrorReadLine();
            flips.BeginOutputReadLine();
            flips.WaitForExit();

            return flips.ExitCode == 0;
        }

        public static bool PatchWithXDelta(string patch, string rom)
        {
            string romExt = System.IO.Path.GetExtension(rom);

            string command = "-f -d -s \"" + rom + "\" \"" + patch + "\" \"" + patch.Replace(Path.GetExtension(patch), romExt) + "\"";

            Process xdelta = new Process();
            xdelta.StartInfo.FileName = "xdelta.exe";
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                xdelta.StartInfo.FileName = "xdelta";
            xdelta.StartInfo.Arguments = command;
            xdelta.StartInfo.UseShellExecute = false;
            xdelta.StartInfo.CreateNoWindow = true;
            xdelta.StartInfo.RedirectStandardOutput = true;
            xdelta.StartInfo.RedirectStandardError = true;
            xdelta.OutputDataReceived += PathOutputHandler;
            xdelta.ErrorDataReceived += PathOutputHandler;

            xdelta.Start();
            xdelta.BeginErrorReadLine();
            xdelta.BeginOutputReadLine();
            xdelta.WaitForExit();

            return xdelta.ExitCode == 0;
        }

        public static bool PatchWithUPS(string patch, string rom) {
            //These are handled built-in with the code instead of calling an external executable.

            Nintenlord.UPSpatcher.UPSfile patcher = new Nintenlord.UPSpatcher.UPSfile(patch);

            if (!patcher.ValidPatch)
                return false;

            string romExt = System.IO.Path.GetExtension(rom);

            var results = patcher.Apply(rom);
            BinaryWriter bw = new BinaryWriter(File.Open(patch.Replace(Path.GetExtension(patch), romExt), FileMode.OpenOrCreate));
            bw.Write(results);
            bw.Close();
            return true;
        }
    }
}
