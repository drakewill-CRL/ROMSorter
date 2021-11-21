using RomDatabase5;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RomSorter5WinForms
{
    public partial class SimpleInterface : Form
    {
        Sorter sorter = null;

        public SimpleInterface()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;

            sorter = new Sorter();
        }

        private async void btnSort_Click(object sender, EventArgs e)
        {
            //this is slightly different from the original form.
            //This one is meant to be a minimum-options setup,
            //the user only has to declare a couple folders, and if they want games zipped or not.
            //I do need to import the dat files to SQLite on folder select and treat those slightly differently
            //(each dat file becomes an output folder, instead of parsing them for console names)

            sorter.PreserveOriginals = true;
            sorter.DisplayAllInfo = false;
            sorter.moveUnidentified = true;
            sorter.UseMultithreading = true;
            sorter.ZipInsteadOfMove = (comboBox1.SelectedIndex == 0);
            sorter.IdentifyOnly = false;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Progress<string> progress = new Progress<string>(s => { lblStatus.Text = s; });
            Progress<int> progress2 = new Progress<int>(i => progressBar1.Value = i);
            await Task.Factory.StartNew(() => sorter.Sort(txtRomPath.Text, txtOutputPath.Text, progress, progress2));

            sw.Stop();
            lblStatus.Text = "Operations Complete in " + sw.Elapsed.ToString();
        }

        private async void btnDatFolderSelect_Click(object sender, EventArgs e)
        {
            //Set the folder path box
            //Attempt to auto-import any dat files found
            //also read zip files for dats if present
            Progress<string> progress = new Progress<string>(s => lblStatus.Text = s);
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                Database.RebuildInitialDatabase(); //clears out the exisiting data in sqlite
                txtDatPath.Text = System.IO.Path.GetDirectoryName(ofd1.FileName);
                await Task.Factory.StartNew(() => DATImporter.LoadAllDats(System.IO.Path.GetDirectoryName(ofd1.FileName), progress, false));
                lblStatus.Text = "All datfiles loaded";
            }
        }

        private void btnRomFolderSelect_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                txtRomPath.Text = System.IO.Path.GetDirectoryName(ofd1.FileName);
                sorter.getFilesToScan(txtRomPath.Text);
                progressBar1.Maximum = sorter.FilesToScanCount;
                lblStatus.Text = "Folder selected";
            }
        }

        private void btnOutputFolderSelect_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                txtOutputPath.Text = System.IO.Path.GetDirectoryName(ofd1.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Everdrive sort.
            //assume that the picked folder is the destination and already sorted/IDed.
            //Also, all items are in one folder.

            var fileList = System.IO.Directory.EnumerateFiles(txtRomPath.Text);
            fileList = fileList.Select(f => System.IO.Path.GetFileName(f)).ToList();

            //Create folders for each region, and then by letter, making additional ones if there's over 250 files in one folder.
            //var regions = fileList.Select(f => f.Split('(')[0].Split(')')[0]).Distinct().ToList(); //pick first thing inside () as region.fileList.Select(f => f.Split('(')[0].Split(')')[0]).Distinct().ToList(); //pick first thing inside () as region.
            //foreach (var r in regions)
            //{
            //System.IO.Directory.CreateDirectory(txtRomPath.Text + "\\" + r);
            //var regionalFiles = fileList.Where(f => f.Contains("(" + r + ")")).ToList();

            //var letters = regionalFiles.Select(f => f[0]).Distinct().ToList(); //pick first letter.
            var letters = fileList.Select(f => System.IO.Path.GetFileName(f).ToUpper().Substring(0, 1)).Distinct().ToList(); //pick first letter.
            foreach (var l in letters)
                {
                    System.IO.Directory.CreateDirectory(txtRomPath.Text + "\\" + l);

                var filesToMove = fileList.Where(f => f.StartsWith(l) || f.StartsWith(l.ToLower())).ToList();
                    foreach (var rf in filesToMove)
                        System.IO.File.Move(txtRomPath.Text + "\\" + rf, txtRomPath.Text + "\\" + l + "\\" + rf);
                }

                //fileList = fileList.Where(f => !regionalFiles.Contains(f)).ToList();
            //}
        }
    }
}
