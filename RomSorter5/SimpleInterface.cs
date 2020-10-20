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
                Progress<string> progress = new Progress<string>(s => lblStatus.Text = s);
                txtRomPath.Text = System.IO.Path.GetDirectoryName(ofd1.FileName);
                sorter.getFilesToScan(txtRomPath.Text, progress);
                progressBar1.Maximum = sorter.FilesToScanCount;
            }
        }

        private void btnOutputFolderSelect_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                txtOutputPath.Text = System.IO.Path.GetDirectoryName(ofd1.FileName);
            }
        }
    }
}
