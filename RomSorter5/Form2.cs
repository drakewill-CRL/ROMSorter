﻿using RomDatabase5;
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
                Progress<string> p = new Progress<string>(s => lblStatus.Text = s);
                Task.Factory.StartNew(() => DATImporter.ParseDatFileFast(ofd1.FileName));
            }

        }

        private void btnRomFolderSelect_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                txtRomPath.Text = System.IO.Path.GetDirectoryName(ofd1.FileName);
                //sorter.getFilesToScan(txtRomPath.Text);
                Properties.Settings.Default.romPath = txtRomPath.Text;
                Properties.Settings.Default.Save();
                progressBar1.Maximum = sorter.FilesToScanCount;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count();
            progressBar1.Value = 0;
            
            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            Task.Factory.StartNew(() => DetectDupes(p));
        }

        private void DetectDupes(IProgress<string> p)
        {
            //Detect duplicates.
            Dictionary<string, string> crcHashes = new Dictionary<string, string>();
            Dictionary<string, string> md5Hashes = new Dictionary<string, string>();
            Dictionary<string, string> sha1Hashes = new Dictionary<string, string>();

            Hasher h = new Hasher();
            Directory.CreateDirectory(txtRomPath.Text + "\\Duplicates");
            foreach (var file in System.IO.Directory.EnumerateFiles(txtRomPath.Text))
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                //TODO: check zipped data separately? Or assume stuff was run to make files consistent.
                p.Report(Path.GetFileName(file));
                var fs = System.IO.File.Open(file, System.IO.FileMode.Open);
                var results = h.HashFile(fs);
                fs.Close(); fs.Dispose();
                if (crcHashes.ContainsKey(results[0]) && md5Hashes.ContainsKey(results[1]) && sha1Hashes.ContainsKey(results[2]))
                {
                    // this is a dupe, we hit on all 3 hashes.
                    var origName = crcHashes[results[0]];
                    var dirName = txtRomPath.Text + "\\Duplicates\\" + origName.Replace("(", "").Replace(")", "").Trim();
                    Directory.CreateDirectory(dirName);
                    File.Move(file, dirName + "\\" + Path.GetFileName(file));
                }
                crcHashes.TryAdd(results[0], filename);
                md5Hashes.TryAdd(results[1], filename);
                sha1Hashes.TryAdd(results[2], filename);                
            }
        }

        private void btnUnzipAll_Click(object sender, EventArgs e)
        {
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count();
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            Task.Factory.StartNew(() => UnzipLogic(p));
        }

        private void btnZipAllFiles_Click(object sender, EventArgs e)
        {
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count();
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            Task.Factory.StartNew(() => ZipLogic(p));
        }

        private void btnIdentifyAndZip_Click(object sender, EventArgs e)
        {
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count() + 1;
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            Task.Factory.StartNew(() => IdentifyZipLogic(p));
        }
        private void IdentifyZipLogic(IProgress<string> progress)
        {
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text).ToList();
            bool useLzma = chkLzma.Checked;
            bool moveUnidentified = chkMoveUnidentified.Checked;
            if (moveUnidentified)
                Directory.CreateDirectory(txtRomPath.Text + "\\Unknown");

            string errors = "";
            foreach (var file in files)
            {
                try
                {
                    progress.Report(Path.GetFileName(file));
                    //Identify it first.
                    var identifiedFile = sorter.IdentifyOneFile(file);
                    var destFileName = txtRomPath.Text + "\\" + (identifiedFile != "" ? identifiedFile : (moveUnidentified ? "\\Unknown\\" : "") + Path.GetFileNameWithoutExtension(file)) + ".zip";

                    string tempfilename = Path.GetTempFileName();
                    SharpCompress.Archives.IArchive existingZip = null;
                    //using (ZipArchive zf = new ZipArchive(System.IO.File.Create(tempfilename), ZipArchiveMode.Update))
                    var fs = File.OpenRead(file);

                    var options = new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.Deflate) { LeaveStreamOpen = false };
                    if (useLzma)
                        options.CompressionType = SharpCompress.Common.CompressionType.LZMA;

                    var zf = SharpCompress.Writers.WriterFactory.Open(System.IO.File.Create(tempfilename), SharpCompress.Common.ArchiveType.Zip, options);
                    switch (System.IO.Path.GetExtension(file))
                    {
                        case ".zip":
                        case ".rar":
                        case ".gz":
                        case ".gzip":
                        case ".tar":
                        case ".7z": //7z is super slow, but it also doesn't actually support the stream method that would let it go faster.
                            existingZip = SharpCompress.Archives.ArchiveFactory.Open(fs);
                            RezipFromArchive(existingZip, zf);
                            break;
                        default:
                            zf.Write(Path.GetFileName(file), new FileInfo(file));
                            break;
                    }
                    if (existingZip != null) existingZip.Dispose();
                    fs.Close(); fs.Dispose();
                    zf.Dispose();
                    File.Move(tempfilename, destFileName);
                    File.Delete(file);                    
                }
                catch (Exception ex)
                {
                    //add 
                    errors += file + ": " + ex.Message + Environment.NewLine;
                }
            }

            progress.Report("Complete");
            if (errors != "")
                MessageBox.Show(errors);
        }

        private void ZipLogic(IProgress<string> progress)
        {
            //NOTE: this doesn't seem to identify games correctly. 2 of my test set get correctly picked up in a NoIntro file.
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text).ToList();
            bool useLzma = chkLzma.Checked;
            int count = 1;
            foreach (var file in files)
            {
                progress.Report(count + "/" + files.Count() + ":" + Path.GetFileName(file));
                string tempfilename = Path.GetTempFileName();
                SharpCompress.Archives.IArchive existingZip = null;
                var fs = File.OpenRead(file);

                var options = new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.Deflate) { LeaveStreamOpen = false };
                if (useLzma)
                    options.CompressionType = SharpCompress.Common.CompressionType.LZMA;

                var zf = SharpCompress.Writers.WriterFactory.Open(System.IO.File.Create(tempfilename), SharpCompress.Common.ArchiveType.Zip, options);
                switch (System.IO.Path.GetExtension(file))
                {
                    case ".zip":
                    case ".rar":
                    case ".gz":
                    case ".gzip":
                    case ".tar":
                    case ".7z": //7z is super slow, but it also doesn't actually support the stream method that would let it go faster.
                        existingZip = SharpCompress.Archives.ArchiveFactory.Open(fs);
                        RezipFromArchive(existingZip, zf);
                        break;
                    default:
                        zf.Write(Path.GetFileName(file), new FileInfo(file));
                        break;
                }
                if (existingZip != null) existingZip.Dispose();
                fs.Close(); fs.Dispose();
                zf.Dispose();
                File.Move(tempfilename, Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".zip", true);
                if (!file.EndsWith(".zip")) //we just overwrote this file, don't remove it.
                    File.Delete(file);
                count++;
            }
            progress.Report("Complete");
        }

        private void UnzipLogic(IProgress<string> progress)
        {
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text).ToList();
            foreach (var file in files)
            {
                switch (Path.GetExtension(file))
                {
                    case ".zip":
                    case ".rar":
                    case ".gz":
                    case ".gzip":
                    case ".tar":
                    case ".7z":
                        var fs = File.OpenRead(file);
                        var existingZip = SharpCompress.Archives.ArchiveFactory.Open(fs);
                        if (existingZip != null)
                        {
                            foreach (var e in existingZip.Entries)
                                e.WriteToDirectory(txtRomPath.Text);

                            existingZip.Dispose();
                            fs.Close(); fs.Dispose();
                            System.IO.File.Delete(file);
                        }
                        break;
                }

                progress.Report(file);
            }
            progress.Report("Complete");
        }

        private void RezipFromArchive(SharpCompress.Archives.IArchive existingZip, SharpCompress.Writers.IWriter zf)
        {
            foreach (var ez in existingZip.Entries)
            {
                if (!ez.IsDirectory)
                    zf.Write(ez.Key, ez.OpenEntryStream());
            }
        }

        private void btnCatalog_Click(object sender, EventArgs e)
        {
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count();
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            Task.Factory.StartNew(() => Catalog(p));
        }

        private void Catalog(IProgress<string> progress)
        {
            //Hash all files in directory, write results to a CSV file 
            FileStream fs = File.OpenWrite(txtRomPath.Text + "\\catalog.tsv");
            StreamWriter sw = new StreamWriter(fs);
            Hasher hasher = new Hasher();
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text).Where(f => Path.GetFileName(f) != "catalog.tsv").ToList();
            foreach (var file in files)
            {
                progress.Report(file);
                byte[] fileData = File.ReadAllBytes(file);
                var hashes = hasher.HashFileRef(ref fileData);

                sw.WriteLine(Path.GetFileName(file)+ "\t" + hashes[0] + "\t" + hashes[1] + "\t" + hashes[2]);
            }
            sw.Close(); sw.Dispose(); fs.Close(); fs.Dispose();
            progress.Report("Complete");
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            //Hash all files in directory, confirm if they do or don't match values in catalog CSV file.
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count();
            progressBar1.Value = 0;

            Progress<string> p = new Progress<string>(s => { lblStatus.Text = s; if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value++; });
            Task.Factory.StartNew(() => Verify(p));
        }

        private void Verify(IProgress<string> progress)
        {
            //Hash all files in directory, write results to a CSV file 
            bool alert = false;
            var files = File.ReadAllLines(txtRomPath.Text + "\\catalog.tsv");
            Hasher hasher = new Hasher();
            foreach (var file in files)
            {
                string[] vals = file.Split("\t");
                progress.Report(vals[0]);
                try
                {
                    byte[] fileData = File.ReadAllBytes(txtRomPath.Text + "\\" + vals[0]);
                    var hashes = hasher.HashFileRef(ref fileData);
                    if (vals[1] == hashes[0] && vals[2] == hashes[1] && vals[3] == hashes[2])
                        continue;
                    else
                    {
                        alert = true;
                        File.AppendAllText(txtRomPath.Text + "\\report.txt", vals[0] + " did not match:" + vals[1] + "|" + hashes[0] + " " + vals[2] + "|" + hashes[1] + " " + vals[3] + "|" + hashes[2]);
                    }
                }
                catch(Exception ex)
                {
                    alert = true;
                    File.AppendAllText(txtRomPath.Text + "\\report.txt", "Error checking on " + vals[0] + ":" + ex.Message);
                }

            }
            if (!alert)
                progress.Report("Complete, all files verified");
            else
                progress.Report("Complete, error found, read report.txt for info");
        }
    }
}
