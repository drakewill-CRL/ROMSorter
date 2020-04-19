using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Librarian
{
    public partial class Form1 : Form
    {
        //This app is the one meant for using if you backup or burn files to discs.
        //It hashes all your files, saves that info to a .dat file.
        //Then it tells you that the whole folder is ready to burn to disc or whatever.
        //Later, if the app is opened and finds a .dat file, it scans and confirms if each file is still good or not.
        //Ideally this is a single-file application, and any external dependencies become embedded resources.
        public Form1()
        {
            InitializeComponent();
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            //For each file in this folder and sub-folders,
            //hash it, save results to a .dat file.
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            //read the .Dat file into memory,
            //then scan and hash all files in this folder and sub-folders.
            //report if any are bad.
        }
    }
}
