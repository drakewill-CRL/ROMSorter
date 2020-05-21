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
            System.Diagnostics.Process.Start("file://" + Environment.CurrentDirectory + "\\CountByConsole.html"); //Doublecheck slashes?
        }

        private void btnTestEntities_Click(object sender, EventArgs e)
        {
            //Run through new stuff for code correctness.
            Stopwatch sw = new Stopwatch();
            var db = new DatabaseEntities();
            sw.Start();
            var results1 = db.CountGames(); //~125ms normal, ~1000ms cold, all games + distinct disc name 
            sw.Stop();
            var timing1 = sw.ElapsedMilliseconds;
            sw.Restart();
            var results2 = Database.CountGames(); //~525ms normal, ~540ms cold, all games + distinct disc name
            sw.Stop();
            var timing2 = sw.ElapsedMilliseconds;
            Debug.Assert(results1 == results2); //Speed Comparison  1: Entities are 4-10x faster stepping over in debugger after the first time
            double entityDifference1 = timing2 / (float)timing1;

            sw.Restart();
            var results3 = db.CountGamesByConsole(); //~240ms cold, ~125ms hot
            sw.Stop();
            var timing3 = sw.ElapsedMilliseconds;
            sw.Restart();
            var results4 = Database.CountGamesByConsole(); //~750ms cold, ~700ms hot hot
            sw.Stop();
            var timing4 = sw.ElapsedMilliseconds;
            Debug.Assert(results3.Count() == results4.Count()); //Speed Comparison 2: entities are 3-5x faster
            double entityDifference2 = timing4 / (float)timing3;

            sw.Restart();
            var results5 = db.FindGame(39318, "4f010d38", "567bf221ec4b13a1aa06540de6d49bf5", "b0c1382535d7ceebe4a69df81974eebf4363793d"); //~30ms cold, 1ms hot
            sw.Stop();
            var timing5 = sw.ElapsedMilliseconds;
            sw.Restart();
            var results6 = Database.FindGame(39318, "4f010d38", "567bf221ec4b13a1aa06540de6d49bf5", "b0c1382535d7ceebe4a69df81974eebf4363793d"); //~5ms cold, 1ms hot
            sw.Stop();
            var timing6 = sw.ElapsedMilliseconds;
            //Debug.Assert(results5 == results6); //Speed Comparison 3: entities are slower. A lot slower on first call, still 3x slower warmed up.
            double entityDifference3 = timing6 / (float)timing5;

            sw.Restart();
            var results7 = db.FindDisc(3075, "85f95735", "58cf7a07a84e3dc31be7c8816f67bb28", "d2c61d52fe40dced4ab6c611a328fed7afb9ddd4"); //6ms cold, 0ms hot
            sw.Stop();
            var timing7 = sw.ElapsedMilliseconds;
            sw.Restart();
            var results8 = Database.FindDisc(3075, "85f95735", "58cf7a07a84e3dc31be7c8816f67bb28", "d2c61d52fe40dced4ab6c611a328fed7afb9ddd4"); //1ms cold, 1ms hot
            sw.Stop();
            var timing8 = sw.ElapsedMilliseconds;
            //Debug.Assert(results7 == results8); //Speed Comparison 4:
            double entityDifference4 = timing8 / (float)timing7;

            sw.Restart();
            var results9 = db.GetAllGames(); //~9000ms cold, ~1000ms hot
            sw.Stop();
            var timing9 = sw.ElapsedMilliseconds;
            sw.Restart();
            var results10 = Database.GetAllGames(); //~5800ms cold, ~5600ms hot
            sw.Stop();
            var timing10 = sw.ElapsedMilliseconds;
            //Debug.Assert(results7 == results8); //Speed Comparison 4:
            double entityDifference5 = timing10 / (float)timing9;

            sw.Restart();
            var results11 = db.GetGamesByConsole("Nintendo DS"); //32ms cold, 16ms hot
            sw.Stop();
            var timing11 = sw.ElapsedMilliseconds;
            sw.Restart();
            var results12 = Database.GetGamesByConsole("Nintendo DS"); //1ms cold, 1ms hot
            sw.Stop();
            var timing12 = sw.ElapsedMilliseconds;
            //Debug.Assert(results7 == results8); //Speed Comparison 4:
            double entityDifference6 = timing12 / (float)timing11;


            string resultsSummary = "Entity Performance Differences:" + Environment.NewLine + 
                "Counting Games: " + entityDifference1 + Environment.NewLine + 
                "Counting By Console: " + entityDifference2 + Environment.NewLine +
                "Searching Games: " + entityDifference3 +Environment.NewLine +
                "Searching Discs: " + entityDifference4 +Environment.NewLine +
                "Load All Games: " + entityDifference5 + Environment.NewLine +
                "Load 1 Consoles Games: " + entityDifference6 + Environment.NewLine;

            MessageBox.Show(resultsSummary);
            int a = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hasher h = new Hasher();
            Stopwatch sw = new Stopwatch();
            byte[] b1 = new byte[4096];

            sw.Start();
            h.HashFile(b1);
            sw.Stop();
            var h1Time = sw.Elapsed; //.029

            sw.Restart();
            h.HashFileRef(ref b1);
            sw.Stop();
            var r1Time = sw.Elapsed; //4k time .0001

            b1 = new byte[4096000];

            sw.Restart();
            h.HashFile(b1);
            sw.Stop();
            var h2Time = sw.Elapsed; //.007

            sw.Restart();
            h.HashFileRef(ref b1);
            sw.Stop();
            var r2Time = sw.Elapsed; //4M time .018

            b1 = new byte[40960000];

            sw.Restart();
            h.HashFile(b1);
            sw.Stop();
            var h3Time = sw.Elapsed; //.07

            sw.Restart();
            h.HashFileRef(ref b1);
            sw.Stop();
            var r3Time = sw.Elapsed; //40M time .17s

            b1 = new byte[409600000];

            sw.Restart();
            h.HashFile(b1);
            sw.Stop();
            var h4Time = sw.Elapsed; //.8ms

            sw.Restart();
            h.HashFileRef(ref b1);
            sw.Stop();
            var r4Time = sw.Elapsed; //400M time 1.82s


            //byRef serial hashing is faster on 4k files, slower on all other, but also uses 1/4th the ram(?)
            var a = 1;

        }
    }
}
