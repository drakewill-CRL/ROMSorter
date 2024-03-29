﻿using RomDatabase5;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomSorter5WinForms {

    //TODO IDEA: MAME Update Set Merge: (A = update set, B = existing set)
    //Pick 2 folders, for all zip files in A, check for that file in B. If not present, copy. If present, open and copy all entries from A to B.
    //OR use datfiles to determine which files should and should NOT be present in the final zip, remove ones not in the DAT from B and copy ones that 
    //should be updated from A to B.
    //Maybe generate a report when it's done to say what was updated, what wasn't and any errors found?
    //Will need to note this won't generate torrent-zipped output if you rejoin.  Can later figure out how to torrent-zip stuff.


    //TODO: 
    //do deep folder compare
    //(I have a Main and a Backup folder. I want to make sure that all files are in both).

    public partial class Form2 : Form
    {
        Sorter sorter = null;
        MemDb db = new MemDb();
        public Form2()
        {
            sorter = new Sorter();
            InitializeComponent();
            txtDatPath.Text = Properties.Settings.Default.datFile;

            if (Directory.Exists(Properties.Settings.Default.romPath))
                txtRomPath.Text = Properties.Settings.Default.romPath;
            else
                txtRomPath.Text = Directory.GetCurrentDirectory();

            lblStatus.Text = db.files.Count + " loaded from defaults.";
        }

        private void LockButtons()
        {
            btnCatalog.Enabled = false;
            btnCreateChds.Enabled = false;
            btnDatFolderSelect.Enabled = false;
            btnDetectDupes.Enabled = false;
            btnExtractChds.Enabled = false;
            btnIdentifyAndZip.Enabled = false;
            btnMakeDat.Enabled = false;
            btnRenameMultiFile.Enabled = false;
            btnRomFolderSelect.Enabled = false;
            btnUnzipAll.Enabled = false;
            btnVerify.Enabled = false;
            btnZipAllFiles.Enabled = false;
            btn1G1R.Enabled = false;
            btnEverdrive.Enabled = false;
            btnCreateM3uPlaylists.Enabled = false;
            btnMultiPatch.Enabled = false;
            btnLzip.Enabled = false;
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
            btn1G1R.Enabled = true;
            btnEverdrive.Enabled = true;
            btnCreateM3uPlaylists.Enabled = true;
            btnMultiPatch.Enabled = true;
            btnLzip.Enabled = true;
        }

        private async Task<bool> LoadDatToMemDb()
        {
            LockButtons();
            Progress<string> p = new Progress<string>(s => lblStatus.Text = s);
            db = new MemDb();
            var loadTask = await Task.Factory.StartNew(() => db.loadDatFile(txtDatPath.Text, p));
            UnlockButtons();
            return loadTask.Result;
        }

        private async void btnDatFolderSelect_Click(object sender, EventArgs e)
        {
            //Set the file path for the single dat.
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                System.Configuration.SettingsProperty prop = new System.Configuration.SettingsProperty("datFile");
                txtDatPath.Text = ofd1.FileName;
                var loaded = await LoadDatToMemDb();
                if (loaded)
                {
                    Properties.Settings.Default.datFile = txtDatPath.Text;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void btnRomFolderSelect_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                txtRomPath.Text = System.IO.Path.GetDirectoryName(ofd1.FileName);
                Properties.Settings.Default.romPath = txtRomPath.Text;
                Properties.Settings.Default.Save();
            }
        }

        //For functions moved to the shared library.
        private async void BaseBehavior(Action<IProgress<string>, string> functionToRun, string path)
        {
            LockButtons();
            List<string> paths = new List<string>();
            paths.Add(path);
            if (chkRecurse.Checked)
                paths.AddRange(Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories).ToList());

            foreach (var runpath in paths)
            {
                try
                {
                    var files = System.IO.Directory.EnumerateFiles(runpath);
                    progressBar1.Maximum = files.Count();
                    progressBar1.Value = 0;

                    Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
                    await Task.Factory.StartNew(() => functionToRun(p, runpath));
                }
                catch { }
            }
            progressBar1.Value = progressBar1.Maximum;
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
            //if (recurse)
            //{ 
            //var folders = System.IO.Directory.EnumerateFiles(txtRomPath.Text, "*", SearchOption.AllDirectories);
            //foreach(var f in folders)
            //
            //}
            //else
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count();
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => CoreFunctions.IdentifyLogic(p, txtRomPath.Text, chkMoveUnidentified.Checked, db));
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
            //Keep in mind that rename here isn't validate. Validate would be the harder function to do.
            //this rename would have to recurse through folders and check that the path ENDS with the filename,
            //possibly moving the file in the process, and that last bit of path manipulation is tricky.

            //This may need broken up into MAME-style split/exact values and 
            //SCUMMVM style folder hierarchy options so I quit being stuck on how to do this. 
            //Initial plan for this: use same logic as single-file games, but recurse through folders
            //Future plan: identify all files, and rename all and the parent folder as necessary. TODO all of that.
            //Maybe this should be: rename all single files, then check to see if all files are a disc?
            if (txtDatPath.Text == "")
            {
                MessageBox.Show("You need to supply a dat file to identify games.");
                return;
            }

            //Slightly different than BaseBehavior.
            LockButtons();
            var files = System.IO.Directory.EnumerateDirectories(txtRomPath.Text);
            //if (chkZipInsteadOfFolders.Checked) //TODO future state
            //files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count() + 1;
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            //await Task.Factory.StartNew(() => IdentifyMultiFileGames(p));
            foreach (var dir in Directory.EnumerateDirectories(txtRomPath.Text))
            {
                progressBar1.Value = 0;
                await Task.Factory.StartNew(() => CoreFunctions.IdentifyLogic(p, dir, false, db)); //force to not move unidentified files for now.
            }
            progressBar1.Value = progressBar1.Maximum;
            lblStatus.Text = "Complete";
            UnlockButtons();
        }

        private List<FileEntry> recurseIdentifyFiles(string path, IProgress<string> progress)
        {
            List<FileEntry> foundfiles = new List<FileEntry>();
            string errors = "";
            var subfolders = Directory.EnumerateDirectories(path).ToList();
            foreach (var s in subfolders)
                foundfiles.AddRange(recurseIdentifyFiles(s, progress));

            var files = Directory.EnumerateFiles(path);
            Hasher h = new Hasher();
            foreach (var file in files)
            {
                try
                {
                    progress.Report(Path.GetFileName(file));
                    //Identify it first.
                    var hashes = h.HashFileAtPath(file);
                    var identifiedFiles = db.findFile(hashes);
                    if (identifiedFiles.Count == 0)
                    {
                        //skip this file, we are allowing extra files to exist (EX: walkthroughs or bonus content)
                        continue;
                    }
                    else if (identifiedFiles.Count > 1)
                    {
                        //duplicate entries in DAT file, attempt to guess on filename
                        identifiedFiles = identifiedFiles.Where(i => i.name == GuessFileName(identifiedFiles, file)).ToList();
                    }

                    var identifiedFile = identifiedFiles.FirstOrDefault();
                    foundfiles.Add(identifiedFile);
                    if (!Path.GetFullPath(file).EndsWith(identifiedFile.name))
                        File.Move(file, path + "/" + Path.GetFileName(identifiedFile.name));
                }
                catch (Exception ex)
                {
                    errors += file + ": " + ex.Message + Environment.NewLine;
                }
            }

            return foundfiles;
        }


        //TODO test
        public string GuessFileName(List<FileEntry> results, string currentFilename)
        {

            //if all the name options are the same, use that.
            if (results.Select(r => r.name).Distinct().Count() == 1)
                return results[0].name;

            //if current filename is an option, use that from the DAT to fix capitalization
            var compareName = Path.GetFileName(currentFilename).ToLower();
            if (results.Any(r => r.name.ToLower().EndsWith(compareName)))
                return results.First(r => r.name.ToLower().EndsWith(compareName)).name;

            //If no other hints are available, take the first name from the dat
            return results.First().name;
        }

        //TODO test
        public string GuessGameName(List<List<FileEntry>> results, string currentFolderName)
        {
            //takes in a list of all possible fileentry values for all files in the disc
            var flatList = results.SelectMany(r => r.Select(rr => rr.parentDisc)).ToList();
            var groupedDict = flatList.GroupBy(r => r).OrderByDescending(r => r.Count()).ToDictionary(k => k.Key, v => v.Count());

            //THe first entry in the groupedDict will either be the name shared by all entries, or the most commonly found name
            //if there's not a perfectly common one.
            return groupedDict.First().Key.name;
        }

        private void IdentifyMultiFileGames(IProgress<string> progress)
        {
            //Simple version: 
            //check each file for a match in the dat. If found, rename it.
            //use the recurseIdentify function above for this.

            //more complicated versions will have to come later. test this one first.

            var topFolders = Directory.EnumerateDirectories(txtRomPath.Text).ToList();
            Hasher h = new Hasher();

            foreach (var folder in topFolders)
            {
                progress.Report(folder);
                var files = Directory.EnumerateFiles(folder);
                List<HashResults> currentHashes = new List<HashResults>();
                foreach (var file in files)
                {
                    var hashes = h.HashFileAtPath(file);
                    currentHashes.Add(hashes);
                }
                var discs = db.findDisc(currentHashes);

                //TODO: identify if there's 1 valid option this folder could be.
                //IF SO confirm that all files on the disc are present, and then
                //confirm all filenames are correct and change any that are not.

                //If multiple discs, or no discs:
                //add data to report file and continue on. 
                //Possibly list the closest match and the files that are missing
                //or the extra files that arent part of the disc.

                //and move any extra files to a new Unidentified sub-folder if the checkbox is set.
                //Probably: call a disc good and ignore extra files if 'ignore unknown' is set and all
                //required files are present.
            }
            progress.Report("Completed");
        }

        private int CountConvertingFilesRecursive(string path)
        {
            int fileCount = 0;
            foreach (var folder in (Directory.EnumerateDirectories(path)))
            {
                fileCount += CountConvertingFilesRecursive(folder);
            }

            return fileCount + Directory.EnumerateFiles(path).Where(f => f.EndsWith(".cue") || f.EndsWith(".iso")).Count();
        }

        private int CountCHDFilesRecursive(string path)
        {
            int fileCount = 0;
            foreach (var folder in (Directory.EnumerateDirectories(path)))
            {
                fileCount += CountCHDFilesRecursive(folder);
            }

            return fileCount + Directory.EnumerateFiles(path).Where(f => f.EndsWith(".chd")).Count();
        }

        private async void btnCreateChds_Click(object sender, EventArgs e)
        {
            LockButtons();
            int fileCount = CountConvertingFilesRecursive(txtRomPath.Text);


            progressBar1.Maximum = fileCount + 1; //files.Count() + 1;
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => CoreFunctions.CreateChdLogic(p, txtRomPath.Text));
            progressBar1.Value = progressBar1.Maximum;
            lblStatus.Text = "Complete";
            UnlockButtons();
        }

        private async void btnExtractChds_Click(object sender, EventArgs e)
        {
            LockButtons();
            //var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text).Where(f => f.EndsWith(".chd")); //Differs from BaseBehavior.
            int fileCount = CountCHDFilesRecursive(txtRomPath.Text);
            progressBar1.Maximum = fileCount + 1;
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => CoreFunctions.ExtractChdLogic(p, txtRomPath.Text));
            progressBar1.Value = progressBar1.Maximum;
            lblStatus.Text = "Complete";
            UnlockButtons();
        }

        private async void btnMakeDat_Click(object sender, EventArgs e)
        {
            BaseBehavior(CoreFunctions.DatLogic, txtRomPath.Text);
        }

        private async void Form2_Shown(object sender, EventArgs e)
        {
            if (txtDatPath.Text != "")
            {
                var loaded = await LoadDatToMemDb();
                if (!loaded)
                {
                    Properties.Settings.Default.datFile = Directory.GetCurrentDirectory() + "\\ROMSorterDefault.dat";
                    Properties.Settings.Default.Save();
                    txtDatPath.Text = Properties.Settings.Default.datFile;
                    await LoadDatToMemDb();
                }
            }
            else
            {
                Properties.Settings.Default.datFile = Directory.GetCurrentDirectory() + "\\ROMSorterDefault.dat";
                Properties.Settings.Default.Save();
                txtDatPath.Text = Properties.Settings.Default.datFile;
                await LoadDatToMemDb();
                lblStatus.Text = "Using default set for file detection.";
            }
        }

        private async void btn1G1R_Click(object sender, EventArgs e)
        {
            //ASSUMPTIONS:
            //User already ran 'rename', so we can skip re-scanning everything and run only on file names.
            if (txtDatPath.Text == "")
            {
                MessageBox.Show("You need to supply a dat file to identify games.");
                return;
            }

            if (db.files.Count() == db.parentClones.Count())
            {
                MessageBox.Show("You need to use a Parent/Clone DAT to make a valid 1G1R set");
                return;
            }

            LockButtons();
            RegionPriority formRP = new RegionPriority();
            formRP.entries = db.parentClones.SelectMany(p => p.Clones.Select(pp => pp.region)).Distinct().ToList();
            formRP.ShowDialog();

            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count();
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => CoreFunctions.OneGameOneRomSort(p, txtRomPath.Text, db, formRP.entries));
            UnlockButtons();
        }

        private async void btnEverdrive_Click(object sender, EventArgs e)
        {
            //Progress bar is different here, goes by letter rather than individual file.
            var files = Directory.EnumerateFiles(txtRomPath.Text).ToList();
            var letters = files.Select(f => System.IO.Path.GetFileName(f).ToUpper().Substring(0, 1)).Distinct().ToList(); //pick first letter.
            progressBar1.Maximum = letters.Count();

            LockButtons();
            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => CoreFunctions.EverdriveSort(p, txtRomPath.Text));
            UnlockButtons();
        }

        private async void btnCreateM3uPlaylists_Click(object sender, EventArgs e)
        {
            var files = Directory.EnumerateFiles(txtRomPath.Text)
                .Where(x => x.Contains("disc 1", StringComparison.CurrentCultureIgnoreCase)
                && (x.Contains(".chd") || x.Contains(".iso") || x.Contains(".cue")))
                .ToList();
            progressBar1.Maximum = files.Count;

            LockButtons();
            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            await Task.Factory.StartNew(() => CoreFunctions.CreateM3uPlaylists(p, txtRomPath.Text));
            UnlockButtons();
        }

        private void txtDatPath_Leave(object sender, EventArgs e)
        {
            if (txtDatPath.Text == "")
            {
                LockButtons();
                txtDatPath.Text = Directory.GetCurrentDirectory() + "\\ROMSorterDefault.dat";
                lblStatus.Text = "Loaded defaults because no file was selected.";
                UnlockButtons();
            }
        }

        private void btnMultiPatch_Click(object sender, EventArgs e)
        {
            if (chkRecurse.Checked || !chkMoveMissedPatches.Checked)
                CoreFunctions.MoveMissedPatches = false;
            else
                CoreFunctions.MoveMissedPatches = true;
            BaseBehavior(CoreFunctions.ApplyAllPatches, txtRomPath.Text);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            BaseBehavior(CoreFunctions.DeletePatches, txtRomPath.Text);
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            //BaseBehavior(CoreFunctions.DeleteIfNoUPS, txtRomPath.Text);
            //BaseBehavior(CoreFunctions.DeleteLowercase, txtRomPath.Text);


        }

        private void btnLzip_Click(object sender, EventArgs e)
        {
            BaseBehavior(CoreFunctions.LZipLogic, txtRomPath.Text);
        }

        private void btnPrepNES_Click(object sender, EventArgs e)
        {
            BaseBehavior(CoreFunctions.PrepareNesForMAME, txtRomPath.Text);
        }

        private void btnQuickMerge_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var lf = new LiveFixer();
            lf.Show();
        }
    }
}
