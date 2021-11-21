namespace RomSorter5
{
    partial class TestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnReloadAllHighIntegrity = new System.Windows.Forms.Button();
            this.btnCreateDats = new System.Windows.Forms.Button();
            this.ofdDats = new System.Windows.Forms.OpenFileDialog();
            this.btnDBtoDAT = new System.Windows.Forms.Button();
            this.btnCountbyConsole = new System.Windows.Forms.Button();
            this.lblTestStatus = new System.Windows.Forms.Label();
            this.btnTestEntities = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnRezipAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnReloadAllHighIntegrity
            // 
            this.btnReloadAllHighIntegrity.Location = new System.Drawing.Point(12, 12);
            this.btnReloadAllHighIntegrity.Name = "btnReloadAllHighIntegrity";
            this.btnReloadAllHighIntegrity.Size = new System.Drawing.Size(111, 58);
            this.btnReloadAllHighIntegrity.TabIndex = 0;
            this.btnReloadAllHighIntegrity.Text = "Reload All Dats (high integrity)";
            this.btnReloadAllHighIntegrity.UseVisualStyleBackColor = true;
            this.btnReloadAllHighIntegrity.Click += new System.EventHandler(this.btnReloadAllHighIntegrity_Click);
            // 
            // btnCreateDats
            // 
            this.btnCreateDats.Location = new System.Drawing.Point(677, 12);
            this.btnCreateDats.Name = "btnCreateDats";
            this.btnCreateDats.Size = new System.Drawing.Size(111, 58);
            this.btnCreateDats.TabIndex = 0;
            this.btnCreateDats.Text = "Create Dat From Folder";
            this.btnCreateDats.UseVisualStyleBackColor = true;
            this.btnCreateDats.Click += new System.EventHandler(this.btnCreateDats_Click);
            // 
            // ofdDats
            // 
            this.ofdDats.CheckFileExists = false;
            this.ofdDats.FileName = "Folder Selection";
            this.ofdDats.ValidateNames = false;
            // 
            // btnDBtoDAT
            // 
            this.btnDBtoDAT.Location = new System.Drawing.Point(637, 105);
            this.btnDBtoDAT.Name = "btnDBtoDAT";
            this.btnDBtoDAT.Size = new System.Drawing.Size(103, 38);
            this.btnDBtoDAT.TabIndex = 1;
            this.btnDBtoDAT.Text = "Make Dat From DB";
            this.btnDBtoDAT.UseVisualStyleBackColor = true;
            this.btnDBtoDAT.Click += new System.EventHandler(this.btnDBtoDAT_Click);
            // 
            // btnCountbyConsole
            // 
            this.btnCountbyConsole.Location = new System.Drawing.Point(423, 19);
            this.btnCountbyConsole.Name = "btnCountbyConsole";
            this.btnCountbyConsole.Size = new System.Drawing.Size(108, 41);
            this.btnCountbyConsole.TabIndex = 2;
            this.btnCountbyConsole.Text = "Count Entries By Console";
            this.btnCountbyConsole.UseVisualStyleBackColor = true;
            this.btnCountbyConsole.Click += new System.EventHandler(this.btnCountbyConsole_Click);
            // 
            // lblTestStatus
            // 
            this.lblTestStatus.AutoSize = true;
            this.lblTestStatus.Location = new System.Drawing.Point(170, 77);
            this.lblTestStatus.Name = "lblTestStatus";
            this.lblTestStatus.Size = new System.Drawing.Size(39, 15);
            this.lblTestStatus.TabIndex = 3;
            this.lblTestStatus.Text = "Status";
            // 
            // btnTestEntities
            // 
            this.btnTestEntities.Location = new System.Drawing.Point(578, 208);
            this.btnTestEntities.Name = "btnTestEntities";
            this.btnTestEntities.Size = new System.Drawing.Size(121, 33);
            this.btnTestEntities.TabIndex = 4;
            this.btnTestEntities.Text = "Test Entities Code";
            this.btnTestEntities.UseVisualStyleBackColor = true;
            this.btnTestEntities.Click += new System.EventHandler(this.btnTestEntities_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(546, 304);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(172, 47);
            this.button1.TabIndex = 5;
            this.button1.Text = "Benchmark Hasher";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnRezipAll
            // 
            this.btnRezipAll.Location = new System.Drawing.Point(12, 119);
            this.btnRezipAll.Name = "btnRezipAll";
            this.btnRezipAll.Size = new System.Drawing.Size(111, 58);
            this.btnRezipAll.TabIndex = 6;
            this.btnRezipAll.Text = "Rezip All Files";
            this.btnRezipAll.UseVisualStyleBackColor = true;
            this.btnRezipAll.Click += new System.EventHandler(this.btnRezipAll_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnRezipAll);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnTestEntities);
            this.Controls.Add(this.lblTestStatus);
            this.Controls.Add(this.btnCountbyConsole);
            this.Controls.Add(this.btnDBtoDAT);
            this.Controls.Add(this.btnCreateDats);
            this.Controls.Add(this.btnReloadAllHighIntegrity);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnReloadAllHighIntegrity;
        private System.Windows.Forms.Button btnCreateDats;
        private System.Windows.Forms.OpenFileDialog ofdDats;
        private System.Windows.Forms.Button btnDBtoDAT;
        private System.Windows.Forms.Button btnCountbyConsole;
        private System.Windows.Forms.Label lblTestStatus;
        private System.Windows.Forms.Button btnTestEntities;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnRezipAll;
    }
}