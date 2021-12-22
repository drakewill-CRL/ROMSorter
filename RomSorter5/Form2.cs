using RomDatabase5;
using SharpCompress.Archives;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomSorter5WinForms
{
    public partial class Form2 : Form
    {
        Sorter sorter = null;
        public Form2()
        {
            sorter = new Sorter();
            InitializeComponent();
            txtDatPath.Text = Properties.Settings.Default.datFile;
            txtRomPath.Text = Properties.Settings.Default.romPath;
        }

        private void LockButtons()
        {
            btnCatalog.Enabled = false;
            btnCreateChds.Enabled= false;
            btnDatFolderSelect.Enabled=false;
            btnDetectDupes.Enabled=false;
            btnExtractChds.Enabled = false; 
            btnIdentifyAndZip.Enabled=false;    
            btnMakeDat.Enabled = false;
            btnRenameMultiFile.Enabled=false;
            btnRomFolderSelect.Enabled=false;
            btnUnzipAll.Enabled=false;
            btnVerify.Enabled=false;
            btnZipAllFiles.Enabled=false;
        }

        private void UnlockButtons()
        {
            btnCatalog.Enabled = true;
            btnCreateChds.Enabled = true;
            btnDatFolderSelect.Enabled = true;
            btnDetectDupes.Enabled = true;
            btnExtractChds.Enabled = true;
            btnIdentifyAndZip.Enabled = true;
            btnMakeDat.Enabled = true;
            btnRenameMultiFile.Enabled = true;
            btnRomFolderSelect.Enabled = true;
            btnUnzipAll.Enabled = true;
            btnVerify.Enabled = true;
            btnZipAllFiles.Enabled = true;
        }

        private void btnDatFolderSelect_Click(object sender, EventArgs e)
        {
            //Set the folder path box
            //Attempt to auto-import any dat files found
            //also read zip files for dats if present
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                Database.RebuildInitialDatabase(); //clears out the exisiting data in sqlite
                txtDatPath.Text = ofd1.FileName;
                System.Configuration.SettingsProperty prop = new System.Configuration.SettingsProperty("datFile");
                Properties.Settings.Default.datFile = txtDatPath.Text;
                Properties.Settings.Default.Save();
                //Progress<string> p = new Progress<string>(s => lblStatus.Text = s);
                Task.Factory.StartNew(() => DATImporter.ParseDatFileFast(ofd1.FileName));
            }
        }

        private void btnRomFolderSelect_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                txtRomPath.Text = System.IO.Path.GetDirectoryName(ofd1.FileName);
                Properties.Settings.Default.romPath = txtRomPath.Text;
                Properties.Settings.Default.Save();
                progressBar1.Maximum = sorter.FilesToScanCount;
            }
        }

        //For functions still in this form.
        private async void BaseBehavior(Action<IProgress<string>> functionToRun)
        {
            LockButtons();
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count();
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => functionToRun(p));
            UnlockButtons();
        }

        //For functions moved to the shared library.
        private async void BaseBehavior(Action<IProgress<string>, string> functionToRun, string path)
        {
            LockButtons();
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count();
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => functionToRun(p, path));
            UnlockButtons();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            BaseBehavior(CoreFunctions.DetectDupes, txtRomPath.Text);
        }

        private async void btnUnzipAll_Click(object sender, EventArgs e)
        {
            BaseBehavior(CoreFunctions.UnzipLogic, txtRomPath.Text);
        }

        private async void btnZipAllFiles_Click(object sender, EventArgs e)
        {
            BaseBehavior(CoreFunctions.ZipLogic, txtRomPath.Text);
        }

        private async void btnIdentifyAndZip_Click(object sender, EventArgs e)
        {
            if (txtDatPath.Text == "")
            {
                MessageBox.Show("You need to supply a dat file to identify games.");
                return;
            }

            LockButtons();
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count();
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => CoreFunctions.IdentifyLogic(p, txtRomPath.Text, chkMoveUnidentified.Checked));
            UnlockButtons();
        }

        private async void btnCatalog_Click(object sender, EventArgs e)
        {
            BaseBehavior(CoreFunctions.Catalog, txtRomPath.Text);
        }

        private async void btnVerify_Click(object sender, EventArgs e)
        {
            BaseBehavior(CoreFunctions.Verify, txtRomPath.Text);
        }

        private async void btnRenameMultiFile_Click(object sender, EventArgs e)
        {
            if (txtDatPath.Text == "")
            {
                MessageBox.Show("You need to supply a dat file to identify games.");
                return;
            }

            //Slightly different than BaseBehavior.
            LockButtons();
            var files = System.IO.Directory.EnumerateDirectories(txtRomPath.Text);
            if (chkZipInsteadOfFolders.Checked)
                files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count() + 1;
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => IdentifyMultiFileGames(p));
            UnlockButtons();
        }

        private void IdentifyMultiFileGames(IProgress<string> progress)
        {
            //TODO: might need a toggle between 'check zip files' and 'check subfolders'
            //writing code for 'check subfolders now', will adapt to zip files later.
            var topFolders = Directory.EnumerateDirectories(txtRomPath.Text).ToList();

            //TODO: need to handle perfect matches, missing files, and extra files separately?
            //keep in mind this will be run on MAME, SCUMMVM, and some CD format games.
            //MAME wants exact matches. The others can live with bonus content. 
            foreach(var folder in topFolders)
            {
                progress.Report(folder);
            }
            progress.Report("Completed");

        }

        private async void btnCreateChds_Click(object sender, EventArgs e)
        {
            LockButtons();
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text).Where(f => f.EndsWith(".cue") || f.EndsWith(".iso")); //Differs from BaseBehavior.
            progressBar1.Maximum = files.Count() + 1;
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => CoreFunctions.CreateChdLogic(p, txtRomPath.Text));
            UnlockButtons();
        }

        private async void btnExtractChds_Click(object sender, EventArgs e)
        {
            LockButtons();
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text).Where(f => f.EndsWith(".chd")); //Differs from BaseBehavior.
            progressBar1.Maximum = files.Count() + 1;
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => CoreFunctions.ExtractChdLogic(p, txtRomPath.Text));
            UnlockButtons();
        }

        private async void btnMakeDat_Click(object sender, EventArgs e)
        {
            BaseBehavior(CoreFunctions.DatLogic, txtRomPath.Text);
        }
    }
}
