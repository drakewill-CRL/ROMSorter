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
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void btnReloadAllHighIntegrity_Click(object sender, EventArgs e)
        {
                   
        }

        private void btnCreateDats_Click(object sender, EventArgs e)
        {
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                DatCreator.MakeDat(System.IO.Path.GetDirectoryName(ofdDats.FileName));
                MessageBox.Show("Dat file made");
            }
        }
    }
}
