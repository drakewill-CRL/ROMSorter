using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RomDatabase5
{
    public class CHD
    {
        //For doing CHD commands via chdman in a child process.
        string lastOutputMessage;

        //Is expecting a .cue file.
        public bool CreateChd(string name)
        {
            //Its possible that i would need to read through the .cue file to find which other files to remove when done.
            string destFileName = Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".chd";
            var command = "createcd -i \"" + name + "\" -o \"" + destFileName + "\"";
            bool success = RunChdmanCommand("createcd -i \"" + name + "\" -o \"" + Path.GetFileNameWithoutExtension(name) + ".chd\"");

            //chdman might not care about paths provided for an output file.
            if (success && File.Exists(Path.GetFileNameWithoutExtension(name) + ".chd"))
            {
                File.Move(Path.GetFileNameWithoutExtension(name) + ".chd", destFileName);
            }

            return success;
        }

        public bool ExtractCHD(string name)
        {
            bool success = RunChdmanCommand("extractcd -i \"" + name + "\" -o \"" + Path.GetFileNameWithoutExtension(name) + ".cue\"");
            //chdman might not care about paths provided for an output file.
            if (success)
            {
                string baseName = Path.GetFileNameWithoutExtension(name);
                string destFolder = Path.GetDirectoryName(name);
                //these files will be in the starting directory for no good reason.
                File.Move(baseName + ".bin", destFolder + "/" + baseName + ".bin");
                File.Move(baseName + ".cue", destFolder + "/" + baseName + ".cue");
            }

            return success;
        }

        public bool RunChdmanCommand(string command)
        {
            Process chdman = new Process();
            chdman.StartInfo.FileName = "chdman.exe";
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                chdman.StartInfo.FileName = "chdman";
            chdman.StartInfo.Arguments = command; 
            chdman.StartInfo.UseShellExecute = false;
            chdman.StartInfo.CreateNoWindow = true;
            chdman.StartInfo.RedirectStandardOutput = true;
            chdman.StartInfo.RedirectStandardError = true;
            chdman.OutputDataReceived += CHDOutputHandler;
            chdman.ErrorDataReceived += CHDOutputHandler;

            chdman.Start();
            chdman.BeginErrorReadLine();
            chdman.BeginOutputReadLine();
            chdman.WaitForExit();

            return chdman.ExitCode == 0;
        }

        private void CHDOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            lastOutputMessage = outLine.Data;
        }
    }
}
