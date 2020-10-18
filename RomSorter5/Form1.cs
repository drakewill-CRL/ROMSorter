using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RomDatabase5;

namespace RomSorter5
{
    public partial class Form1 : Form
    {
        //NET 5 Notes:
        //Make all buttons use FlatStyle.System - Looks so much nicer.
        //Consider making multithreading mandatory, remove the checkbox.
        //See if the AutoScale mode needs to be changed to make sense on new systems
        //WinForm designer is unstable - I've lost all controls off of the list at least twice now from mis-clicks.

        string sourceFolder = "";
        string destinationFolder = "";
        Sorter sorter = null;

        public Form1()
        {
            InitializeComponent();

            if (!System.IO.File.Exists("RomDB.sqlite"))
            {
                Database.RebuildInitialDatabase();
                lblStatus.Text = "Database is empty";
            }
            else
            {
                Progress<string> p = new Progress<string>(s => lblStatus.Text = s);
                Task.Factory.StartNew(() => CountGamesAndConsoles(p));
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                TestForm tf = new TestForm();
                tf.Show();
            }
        }

        private void btnPickFolder_Click(object sender, EventArgs e)
        {
            //pick source.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sourceFolder = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
                btnSort.Enabled = true;
                btnReport.Enabled = true;

                Progress<string> progress = new Progress<string>(s => {
                    lblStatus.Text = s; txtMessageLog.AppendText(s + Environment.NewLine); txtMessageLog.SelectionStart = txtMessageLog.TextLength; txtMessageLog.ScrollToCaret();
                });
                sorter = new Sorter();
                sorter.getFilesToScan(sourceFolder, progress);
            }
        }

        private void btnPickDestination_Click(object sender, EventArgs e)
        {
            //pick destination.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                destinationFolder = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
            }
        }

        private async void btnSort_Click(object sender, EventArgs e)
        {
            //Sort!
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            StringBuilder sb = new StringBuilder();

            Progress<string> progress = new Progress<string>(s => {
                lblStatus.Text = s;

                if (sb.Length > 1000000)
                    sb.Clear();
                sb.Append(s + Environment.NewLine);
                txtMessageLog.Text = sb.ToString();
                txtMessageLog.SelectionStart = txtMessageLog.TextLength; 
                txtMessageLog.ScrollToCaret();
            });

            if (destinationFolder == "")
                destinationFolder = sourceFolder;

            sorter.DisplayAllInfo = chkDisplayAllActions.Checked;
            sorter.moveUnidentified = chkMoveUnidentified.Checked;
            sorter.PreserveOriginals = chkPreserveOriginals.Checked;
            sorter.UseMultithreading = true;
            sorter.ZipInsteadOfMove = chkZipIdentified.Checked;
            sorter.IdentifyOnly = false;

            if (chkSingleThread.Checked)
                await Task.Factory.StartNew(() => sorter.SortSingleThreaded(sourceFolder, destinationFolder, progress));
            else
                await Task.Factory.StartNew(() => sorter.Sort(sourceFolder, destinationFolder, progress));

            sw.Stop();
            lblStatus.Text = "Games sorted in " + sw.Elapsed.ToString();
        }

        private async void btnReport_Click(object sender, EventArgs e)
        {
            //Only scans files and reports data back.
            //Sort!
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Progress<string> progress = new Progress<string>(s => {
                lblStatus.Text = s; txtMessageLog.AppendText(s + Environment.NewLine); txtMessageLog.SelectionStart = txtMessageLog.TextLength; txtMessageLog.ScrollToCaret();
            });

            if (destinationFolder == "")
                destinationFolder = sourceFolder;

            sorter.DisplayAllInfo = true; //always on for info-only.
            sorter.PreserveOriginals = chkPreserveOriginals.Checked;
            sorter.UseMultithreading = true;
            sorter.IdentifyOnly = true; //doesnt rename or move anything.

            await Task.Factory.StartNew(() => sorter.Sort(sourceFolder, destinationFolder, progress));

            sw.Stop();
            lblStatus.Text = "Games identified in " + sw.Elapsed.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists("RomDB.sqlite"))
            {
                Database.RebuildInitialDatabase();
                lblStatus.Text = "Database is empty";
            }
            else
            {
                Progress<string> p = new Progress<string>(s => lblStatus.Text = s);
                Task.Factory.StartNew(() => CountGamesAndConsoles(p));
            }
        }

        private void CountGamesAndConsoles(IProgress<string> progress)
        {
            int games = Database.CountGames();
            int systems = Database.CountGamesByConsole().Count();
            progress.Report(games + " games across " + systems + " platforms.");

        }

        private void chkDisplayAllActions_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
