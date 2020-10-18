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
        public SimpleInterface()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //this is slightly different from the original form.
            //This one is meant to be a minimum-options setup,
            //the user only has to declare a couple folders, and if they want games zipped or not.
            //I do need to import the dat files to SQLite on folder select and treat those slightly differently
            //(each dat file becomes an output folder, instead of parsing them for console names)
        }
    }
}
