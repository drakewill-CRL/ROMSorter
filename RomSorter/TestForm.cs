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
    public partial class TestForm : Form
    {

        private BackgroundWorker _worker;
        StatusWindow sw = new StatusWindow();
        public TestForm()
        {
            InitializeComponent();
            //sw.Show();
            InitWorker();
        }

        //This is an old way to handle this sort of thing. The new way is on Form1, using Progress<> and Task()
        private void InitWorker()
        {
            if (_worker != null)
            {
                _worker.Dispose();
            }

            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            //_worker.DoWork += DoWork; //I think I should set this to my long-running ops.
            //_worker.RunWorkerCompleted += RunWorkerCompleted;
            _worker.ProgressChanged += ProgressChanged;
            _worker.RunWorkerAsync();
        }

        void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //I need a reference to status window
            ((Label)sw.Controls["lblStatus"]).Text = string.Format("Progress Made");
        }

        void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // Display some message to the user that task has been
                // cancelled
            }
            else if (e.Error != null)
            {
                // Do something with the error
            }
            else
            {
                ((Label)sw.Controls["lblStatus"]).Text = string.Format("Idle");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Rebuild Database
            //if (ofdDats.ShowDialog() == DialogResult.OK)
            //{
                Database.RebuildInitialDatabase();

                //RomDatabase.DATImporter.LoadAllDatFiles(System.IO.Path.GetDirectoryName(ofdDats.FileName));
                //MessageBox.Show("Import Completed.");
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Database.CountGames(null) + " entries total");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var results = Database.CountGamesByConsole();
            string display = string.Join(Environment.NewLine, results.Select(r => r.Item1.ToString() + ":" + r.Item2.ToString()));
            MessageBox.Show(display);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                RomDatabase.DATImporter.LoadAllDiscDatFiles(System.IO.Path.GetDirectoryName(ofdDats.FileName));
                MessageBox.Show("Import Completed.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RomDatabase.Database.MakeIndexes();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Make Dat From Folder
            if (ofdDats.ShowDialog() ==  DialogResult.OK)
            {
                DatCreator.MakeDat(System.IO.Path.GetDirectoryName(ofdDats.FileName));
                MessageBox.Show("Dat file made");
            }
            //Takes about 10 minutes for SCUMMVM single-threaded, skipping files too big for .NET libraries.
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //ID games in folder
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                var dir = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(ofdDats.FileName));
                foreach (var file in dir.EnumerateFiles())
                {
                    var fileData = System.IO.File.ReadAllBytes(file.FullName);
                    var hashes = Hasher.HashFile(fileData);
                    var result = Database.FindGame(fileData.Length, hashes[2], hashes[0], hashes[1]);
                    MessageBox.Show("Game:" + result.name);
                }
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            //find collisions
             var results = Database.FindCollisions(); //There's over 8000 collisions on CRCs. I will always check all 3 hashes.
            var a = 1; 
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //Load 1G1R files

            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                var dir = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(ofdDats.FileName));
                foreach (var file in dir.EnumerateFiles())
                {
                    DATImporter.Read1G1RFile(file.FullName);
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //test full good path
            //read files, find files, rename and move files.
            //no config options yet. 
            //Make folders for console, then rename file and move it to that folder.
            //move unidentified ones to Unidentified folder
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                //this needs to be a recursive function.
                string path = System.IO.Path.GetDirectoryName(ofdDats.FileName);
                Sorter.SortAllGamesMultithread(path, path);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //load pinball dats
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                var dir = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(ofdDats.FileName));
                foreach (var file in dir.EnumerateFiles())
                {
                    DATImporter.ReadPinballDatFile(file.FullName);
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //make dat from DB
            DatCreator.DumpDBToDat();            
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //console dats fast
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                RomDatabase.DATImporter.LoadAllDatFiles(System.IO.Path.GetDirectoryName(ofdDats.FileName));
                MessageBox.Show("Import Completed.");
            }
        }

        private async void button14_Click(object sender, EventArgs e)
        {
            //console dats high integrity. Make indexes first.
            Progress<string> progress = new Progress<string>(s => lblTestStatus.Text = s);
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                await Task.Factory.StartNew(() => RomDatabase.DATImporter.LoadAllDatFilesIntegrity(System.IO.Path.GetDirectoryName(ofdDats.FileName), progress));
                MessageBox.Show("Import Completed.");
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //single console dat
            ofdDats.FileName = "";
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                Progress<string> p = new Progress<string>(s => lblTestStatus.Text = s);
                Task.Factory.StartNew(() =>DATImporter.ParseDatFileHighIntegrity(ofdDats.FileName, p));
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            //hash and find single game.
            ofdDats.FileName = "";
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                byte[] fileData = System.IO.File.ReadAllBytes(ofdDats.FileName);
                var hashes = Hasher.HashFile(fileData);
                var gameData = Database.FindGame(fileData.LongCount(), hashes);

                if (gameData == null)
                {
                    gameData = new Data.Game();
                    gameData.name = "Unknown";
                }

                MessageBox.Show("Game Hashes:" + Environment.NewLine + String.Join(Environment.NewLine, hashes)
                    + Environment.NewLine + "Game Name:" + Environment.NewLine + gameData.name  );
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            //test hash
            ofdDats.FileName = "";
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                var files = System.IO.Directory.EnumerateFiles(System.IO.Path.GetDirectoryName(ofdDats.FileName));

                var file1 = System.IO.File.ReadAllBytes(files.First());
                var file2 = System.IO.File.ReadAllBytes(files.Skip(1).First());


                //ZipArchive zf = new ZipArchive(new FileStream(file, FileMode.Open));

                //var hashes = Hasher.HashFile(fileData);
                //var gameData = Database.FindGame(fileData.LongCount(), hashes);

                //MessageBox.Show("Game Hashes:" + Environment.NewLine + String.Join(Environment.NewLine, hashes)
                //    + Environment.NewLine + "Game Name:" + Environment.NewLine + gameData.name);

            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            //Multi-file dat from folder.
            //This needs to read through zip files (or folders, but not both), and make each one an entry. 
            //I think Name is the actual game, and Description is the indiviudal filename.
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (ofdDats.ShowDialog() == DialogResult.OK)
                MessageBox.Show(DatCreator.GetGameAndRomEntryMultifileFromZip(ofdDats.FileName));
        }

        private void button20_Click(object sender, EventArgs e)
        {
            //load Single disc dat
            ofdDats.FileName = "";
            if (ofdDats.ShowDialog() == DialogResult.OK)
            {
                Progress<string> p = new Progress<string>(s => lblTestStatus.Text = s);
                Task.Factory.StartNew(() => DATImporter.ParseDiscDatFile(ofdDats.FileName, p));
            }
        }
    }
}
