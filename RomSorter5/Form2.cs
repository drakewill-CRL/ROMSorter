using RomDatabase5;
using SharpCompress.Archives;
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
        }

        private void btnDatFolderSelect_Click(object sender, EventArgs e)
        {
            //Set the folder path box
            //Attempt to auto-import any dat files found
            //also read zip files for dats if present
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                Database.RebuildInitialDatabase(); //clears out the exisiting data in sqlite
                txtDatPath.Text = System.IO.Path.GetDirectoryName(ofd1.FileName);
                DATImporter.LoadAllDats(System.IO.Path.GetDirectoryName(ofd1.FileName));
            }
        }

        private void btnRomFolderSelect_Click(object sender, EventArgs e)
        {
            txtRomPath.Text = System.IO.Path.GetDirectoryName(ofd1.FileName);
            sorter.getFilesToScan(txtRomPath.Text);
            progressBar1.Maximum = sorter.FilesToScanCount;
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

            foreach (var file in files)
            {
                using (FileStream fs = new FileStream(System.IO.Path.Combine(txtRomPath.Text, System.IO.Path.GetFileNameWithoutExtension(file) + ".rar"), FileMode.OpenOrCreate))
                using (ZipArchive zf = new ZipArchive(fs, ZipArchiveMode.Update)) //This is System.Compression, not SharpCrypt. Flip that over?
                {
                    var entry = zf.CreateEntry(System.IO.Path.GetFileName(file));
                    using (BinaryWriter bw = new BinaryWriter(entry.Open()))
                    {
                        bw.Write(System.IO.File.ReadAllBytes(file));
                    }
                }
            }
            progressBar1.Value++;
        }
    }
}
