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
using Microsoft.Extensions.DependencyModel;
using RomDatabase5;

namespace RomSorter5
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private async void btnReloadAllHighIntegrity_Click(object sender, EventArgs e)
        {
            Progress<string> progress = new Progress<string>(s => lblTestStatus.Text = s);
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                await Task.Factory.StartNew(() => DATImporter.LoadAllDats(System.IO.Path.GetDirectoryName(ofdDats.FileName), progress, true));
                MessageBox.Show("Import Completed.");
            }
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

        private void btnTestEntities_Click(object sender, EventArgs e)
        {
            //Run through new stuff for code correctness.
            Stopwatch sw = new Stopwatch();

            sw.Start();
            var results1 = DatabaseEntities.CountGames(); //135ms normal, 955ms cold, all games + distinct disc name 
            sw.Stop();
            var timing1 = sw.ElapsedMilliseconds;
            sw.Restart();
            var results2 = Database.CountGames(); //571ms normal, 578ms cold,, all games + distinct disc name, 10x slower?
            sw.Stop();
            var timing2 = sw.ElapsedMilliseconds;
            Debug.Assert(results1 == results2); //Speed Comparison  1: Entities are 4-10x faster stepping over in debugger after the first time
            double entityDifference1 = timing2 / (float)timing1;

            sw.Restart();
            var results3 = DatabaseEntities.CountGamesByConsole(); //237ms cold, 142ms hot
            sw.Stop();
            var timing3 = sw.ElapsedMilliseconds;
            sw.Restart();
            var results4 = Database.CountGamesByConsole(); //722ms cold or hot
            sw.Stop();
            var timing4 = sw.ElapsedMilliseconds;
            Debug.Assert(results3 == results4); //Speed Comparison 2: entities are 3-5x faster
            double entityDifference2 = timing4 / (float)timing3;


        }
    }
}
