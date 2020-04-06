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
                int games = Database.CountGames(null);
                int systems = Database.CountGamesByConsole().Count();

                lblStatus.Text = games + " games across " + systems + " platforms.";
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

        private void button1_Click(object sender, EventArgs e)
        {
            //pick folder.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                btnSort.Enabled = true;
                btnReport.Enabled = true;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //Sort!
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Progress<string> progress = new Progress<string>(s => lblStatus.Text = s);

            if (chkMultithread.Checked)
                await Task.Factory.StartNew(() =>Sorter.SortAllGamesMultithread(System.IO.Path.GetDirectoryName(openFileDialog1.FileName), System.IO.Path.GetDirectoryName(openFileDialog1.FileName), progress, chkZipIdentified.Checked, chkMoveUnidentified.Checked));
            else
                await Task.Factory.StartNew(() => Sorter.SortAllGamesSinglethread(System.IO.Path.GetDirectoryName(openFileDialog1.FileName), System.IO.Path.GetDirectoryName(openFileDialog1.FileName), progress, chkZipIdentified.Checked, chkMoveUnidentified.Checked));

            sw.Stop();
            lblStatus.Text = "Games sorted in " + sw.Elapsed.ToString();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            //Report
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Progress<string> progress = new Progress<string>(s => lblStatus.Text = s);

            //TODO: multithreading option split
            await Task.Factory.StartNew(() => Reporter.Report(System.IO.Path.GetDirectoryName(openFileDialog1.FileName), progress, chkMultithread.Checked));

            sw.Stop();
            lblStatus.Text = "Report completed in " + sw.Elapsed.ToString();

            System.Diagnostics.Process.Start("notepad.exe", System.IO.Path.GetDirectoryName(openFileDialog1.FileName) + "\\RomSorterReport.txt");
        }
    }
}
