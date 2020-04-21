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

        private void btnDBtoDAT_Click(object sender, EventArgs e)
        {
            DatCreator.DumpDBToDat();
        }

        private void btnCountbyConsole_Click(object sender, EventArgs e)
        {
            var results = Database.CountGamesByConsole();

            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head><body><ul>");

            sb.Append(string.Join(Environment.NewLine, results.Select(r => "<li>" + r.Item1.ToString() + " : " + r.Item2.ToString() + "</li>")));

            sb.Append("<ul></body></html>");

            System.IO.File.WriteAllText("CountByConsole.html", sb.ToString());
            System.Diagnostics.Process.Start("file://" + Environment.CurrentDirectory + "\\CountByConsole.html");
        }
    }
}
