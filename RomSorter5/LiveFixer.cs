using RomDatabase5;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomSorter5WinForms
{
    public partial class LiveFixer : Form
    {
        public LiveFixer()
        {
            InitializeComponent();
        }

        //MemDb entries;
        Dictionary<string, MemDb> entries = new Dictionary<string, MemDb>();
        Hasher h = new Hasher();

        //The idea here is to have the FileSystemWatchers handle scanning and renaming stuff. SO:
        // - I should ignore rename events, since I'll be making those myself.
        // - I should ignore /Dupes and /Unmatched folders, since I'll be dropping stuff into those myself.
        // - I may need 1 FSW per sub-folder? Nah. I may need to lookup which XML file to reference. So maybe a 
        // Dictionary<string, MemDb> with string as the folder?

        private void LiveFixer_Load(object sender, EventArgs e)
        {

        }

        private void txtFolder_TextChanged(object sender, EventArgs e)
        {
            try
            {
                fileWatcher.Path = txtFolder.Text;
                lblStatus.Text = "Directory set";
                btnStart.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Invalid directory, not monitoring.";
                btnStart.Enabled = false;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Actually use the FileSystemWatcher now.
            Progress<string> progress = new Progress<string>();
            var xml = Directory.EnumerateFiles(txtFolder.Text, "*.xml");
            foreach (var x in xml)
            {
                try
                {
                    var db = new MemDb();
                    db.loadDatFile(x, progress).Wait();
                    entries.Add(txtFolder.Text, db);
                    break;
                }
                catch { }
            }

            var folders = Directory.EnumerateDirectories(txtFolder.Text);
            foreach (var folder in folders)
            {
                //Check for usable XML files and load those into an entry


                var xmls = Directory.EnumerateFiles(folder, "*.xml");
                foreach(var x in xmls)
                {
                    try 
                    {
                        var db = new MemDb();
                        db.loadDatFile(x, progress).Wait();
                        entries.Add(folder, db);
                        break;
                    }
                    catch { }  
                }
            }

            fileWatcher.Created += CheckFile;
            fileWatcher.Changed += CheckFile;

            //TODO: confirm i dont need FileName/DirectoryName in NotifyFilter to get Created events.

            fileWatcher.EnableRaisingEvents = true;
        }

        private void CheckFile(object sender, FileSystemEventArgs e)
        {
            var hashes = h.HashFileAtPath(e.FullPath);

            //A new file. Can we scan it?
            var folder = Path.GetDirectoryName(e.FullPath);
            if (entries.ContainsKey(folder))
            {
                var db = entries[folder];
                var match = db.findFile(hashes);
            }
        }




    }
}
