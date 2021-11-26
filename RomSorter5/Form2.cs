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
            //Detect duplicates.
            Dictionary<string, string> crcHashes = new Dictionary<string, string>();
            Dictionary<string, string> md5Hashes = new Dictionary<string, string>();
            Dictionary<string, string> sha1Hashes = new Dictionary<string, string>();

            Hasher h = new Hasher();
            foreach (var file in System.IO.Directory.EnumerateFiles(txtRomPath.Text))
            {
                var results = h.HashFile(System.IO.File.Open(file, System.IO.FileMode.Open));
                if (!crcHashes.TryAdd(results[0], file) && !md5Hashes.TryAdd(results[1], file) && !sha1Hashes.TryAdd(results[2], file))
                {
                    // this is a dupe, we hit on all 3 hashes. Do some processing here.
                }
            }
        }

        private void btnUnzipAll_Click(object sender, EventArgs e)
        {
            //TODO: set this up to use proper Progress reporting and more condensed logic.
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            progressBar1.Maximum = files.Count();

            foreach (var file in files)
            {
                IArchive zippedFile = null;
                string extention = System.IO.Path.GetExtension(file);
                switch (extention)
                {
                    case "zip":
                        zippedFile = SharpCompress.Archives.Zip.ZipArchive.Open(file);
                        break;
                    case "rar":
                        zippedFile = SharpCompress.Archives.Rar.RarArchive.Open(file);
                        break;
                    case "7z":
                        zippedFile = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(file);
                        break;
                    case "gz":
                    case "tar":
                        zippedFile = SharpCompress.Archives.Tar.TarArchive.Open(file);
                        break;
                }

                foreach (var entry in zippedFile.Entries)
                    entry.WriteToFile(txtRomPath.Text + entry.Key);

                System.IO.File.Delete(file);
                progressBar1.Value++;
            }
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
            //NOTE: this doesn't seem to identify games correctly. 2 of my test set get correctly picked up in a NoIntro file.
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            bool useLzma = chkLzma.Checked;
            Directory.CreateDirectory(txtRomPath.Text + "\\Unknown");
            foreach (var file in files)
            {
                progress.Report(Path.GetFileName(file));
                //Identify it first.
                var destFileName = txtRomPath.Text + "\\" + sorter.IdentifyOneFile(file) + ".zip";

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
                File.Delete(file);
                File.Move(tempfilename, destFileName);
            }
            progress.Report("Complete");
        }

        private void ZipLogic(IProgress<string> progress)
        {
            //NOTE: this doesn't seem to identify games correctly. 2 of my test set get correctly picked up in a NoIntro file.
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
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
                File.Delete(file);
                File.Move(tempfilename, Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".zip");
                count++;
            }
            progress.Report("Complete");
        }

        private void UnzipLogic(IProgress<string> progress)
        {
            var files = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            bool useLzma = chkLzma.Checked;
            foreach (var file in files)
            {
                var fs = File.OpenRead(file);
                var existingZip = SharpCompress.Archives.ArchiveFactory.Open(fs);
                if (existingZip != null)
                    foreach(var e in existingZip.Entries)
                        e.WriteToDirectory(txtRomPath.Text);
            }
        }

        private void RezipFromArchive(SharpCompress.Archives.IArchive existingZip, SharpCompress.Writers.IWriter zf)
        {
            foreach (var ez in existingZip.Entries)
            {
                if (!ez.IsDirectory)
                    zf.Write(ez.Key, ez.OpenEntryStream());
            }
        }
    }
}
