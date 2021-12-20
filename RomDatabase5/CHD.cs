using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomDatabase5
{
    public class CHD
    {
        //For doing CHD commands via chdman in a child process.
        string lastOutputMessage;

        //Is expecting a .cue file.
        public string CreateChd(string name)
        {
            RunChdmanCommand("createcd -i \"" + name + "\" -o \"" + System.IO.Path.GetFileNameWithoutExtension(name) + ".chd\"");
            return lastOutputMessage;
        }

        public string ExtractCHD(string name)
        {
            RunChdmanCommand("extractcd -i \"" + name + "\" -o \"" + System.IO.Path.GetFileNameWithoutExtension(name) + ".cue\"");
            return lastOutputMessage;
        }

        public void RunChdmanCommand(string command)
        {
            Process chdman = new Process();
            chdman.StartInfo.FileName = "chdman.exe"; //TODO: remove extention on linux/mac
            chdman.StartInfo.Arguments = command; 
            //chdman.StartInfo.UseShellExecute = false;
            chdman.StartInfo.CreateNoWindow = true;
            chdman.StartInfo.RedirectStandardOutput = true;
            chdman.StartInfo.RedirectStandardError = true;
            chdman.OutputDataReceived += CHDOutputHandler;
            chdman.ErrorDataReceived += CHDOutputHandler;

            chdman.Start();
            chdman.BeginErrorReadLine();
            chdman.BeginOutputReadLine();
            chdman.WaitForExit();
        }

        private void CHDOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            lastOutputMessage = outLine.Data;
        }
    }
}
