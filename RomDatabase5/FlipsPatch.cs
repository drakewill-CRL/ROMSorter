using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace RomDatabase5
{
    public class FlipsPatch
    {
        static string lastOutputMessage;
        private static void FlipsOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            lastOutputMessage = outLine.Data;
        }

        public static bool PatchWithFlips(string patch, string rom)
        {
            string romExt = System.IO.Path.GetExtension(rom);

            string command = "--apply \"" + patch + "\" \"" + rom + "\"" + System.IO.Path.GetFileNameWithoutExtension(patch) + "." + romExt + "\"";
            string command2 = "--apply \"" + patch + "\" \"" + rom + "\" \"" + patch.Replace(Path.GetExtension(patch), romExt) + "\"";

            Process flips = new Process();
            flips.StartInfo.FileName = "flips.exe";
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                flips.StartInfo.FileName = "flips-linux";
            flips.StartInfo.Arguments = command2;
            flips.StartInfo.UseShellExecute = false;
            flips.StartInfo.CreateNoWindow = true;
            flips.StartInfo.RedirectStandardOutput = true;
            flips.StartInfo.RedirectStandardError = true;
            flips.OutputDataReceived += FlipsOutputHandler;
            flips.ErrorDataReceived += FlipsOutputHandler;

            flips.Start();
            flips.BeginErrorReadLine();
            flips.BeginOutputReadLine();
            flips.WaitForExit();

            return flips.ExitCode == 0;
        }
    }
}
