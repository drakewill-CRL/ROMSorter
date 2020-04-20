using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public Form1()
        {
            InitializeComponent();
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

        private void btnSort_Click(object sender, EventArgs e)
        {

        }

        private void btnReport_Click(object sender, EventArgs e)
        {

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
    }
}
