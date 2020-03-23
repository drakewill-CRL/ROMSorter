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
            //Only if file doesnt exist
            //Database.RebuildInitialDatabase();

            //This is the proper way to update the UI from threads in modern .NET Framework
            var progress = new Progress<string>(s => lblStatus.Text = s);
            Task.Factory.StartNew(() => Database.CountGames(progress));
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
            openFileDialog1.ShowDialog();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //Sort!
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Progress<string> progress = new Progress<string>(s => lblStatus.Text = s);

            if (chkMultithread.Checked)
                await Task.Factory.StartNew(() =>Sorter.SortAllGamesMultithread(System.IO.Path.GetDirectoryName(openFileDialog1.FileName), System.IO.Path.GetDirectoryName(openFileDialog1.FileName), chkReadZip.Checked, chkUnzip.Checked, progress));
            else
                await Task.Factory.StartNew(() => Sorter.SortAllGamesSinglethread(System.IO.Path.GetDirectoryName(openFileDialog1.FileName), System.IO.Path.GetDirectoryName(openFileDialog1.FileName), chkReadZip.Checked, chkUnzip.Checked, progress));


            sw.Stop();
            lblStatus.Text = "Games sorted in " + sw.Elapsed.ToString();
        }
    }
}
