using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RomDatabase;

namespace RomSorter
{
    public partial class Form1 : Form
    {
        string sourceFolder = "";
        string destinationFolder = "";

        public Form1()
        {
            InitializeComponent();
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
            int games = Database.CountGames(null);
            int systems = Database.CountGamesByConsole().Count();
            progress.Report(games + " games across " + systems + " platforms.");

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                TestForm tf = new TestForm();
                tf.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //pick folder.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sourceFolder = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
                btnSort.Enabled = true;
                btnReport.Enabled = true;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //Sort!
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Progress<string> progress = new Progress<string>(s => { lblStatus.Text = s; txtMessageLog.AppendText(s + Environment.NewLine); txtMessageLog.SelectionStart = txtMessageLog.TextLength; txtMessageLog.ScrollToCaret();
            });

            if (destinationFolder == "")
                destinationFolder = sourceFolder;

            //Temporary test path.
            await Task.Factory.StartNew(() => Sorter.TestAlternatePath(sourceFolder, destinationFolder, progress));

            //if (chkMultithread.Checked)
            //    await Task.Factory.StartNew(() => Sorter.SortAllGamesMultithread(System.IO.Path.GetDirectoryName(openFileDialog1.FileName), System.IO.Path.GetDirectoryName(openFileDialog1.FileName), progress));
            //else
            //    await Task.Factory.StartNew(() => Sorter.SortAllGamesSinglethread(System.IO.Path.GetDirectoryName(openFileDialog1.FileName), System.IO.Path.GetDirectoryName(openFileDialog1.FileName), progress));

            sw.Stop();
            lblStatus.Text = "Games sorted in " + sw.Elapsed.ToString();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            //Report
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Progress<string> progress = new Progress<string>(s => { lblStatus.Text = s; txtMessageLog.AppendText(s);
            });

            //TODO: multithreading option split
            await Task.Factory.StartNew(() => Reporter.Report(System.IO.Path.GetDirectoryName(openFileDialog1.FileName), progress, chkMultithread.Checked));

            sw.Stop();
            lblStatus.Text = "Report completed in " + sw.Elapsed.ToString();

            System.Diagnostics.Process.Start("notepad.exe", System.IO.Path.GetDirectoryName(openFileDialog1.FileName) + "\\RomSorterReport.txt");
        }

        private void chkMultithread_CheckedChanged(object sender, EventArgs e)
        {
            Sorter.UseMultithreading = chkMoveUnidentified.Checked;
        }

        private void chkZipIdentified_CheckedChanged(object sender, EventArgs e)
        {
            Sorter.ZipInsteadOfMove = chkZipIdentified.Checked;
        }

        private void chkMoveUnidentified_CheckedChanged(object sender, EventArgs e)
        {
            Sorter.moveUnidentified = chkZipIdentified.Checked;
        }

        private void chkPreserveOriginals_CheckedChanged(object sender, EventArgs e)
        {
            Sorter.PreserveOriginals = chkPreserveOriginals.Checked;
        }

        private void btnPickDestination_Click(object sender, EventArgs e)
        {
            //pick folder.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                destinationFolder = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
            }
        }
    }
}
