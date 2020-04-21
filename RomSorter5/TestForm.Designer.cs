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
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnDBtoDAT);
            this.Controls.Add(this.btnCreateDats);
            this.Controls.Add(this.btnReloadAllHighIntegrity);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnReloadAllHighIntegrity;
        private System.Windows.Forms.Button btnCreateDats;
        private System.Windows.Forms.OpenFileDialog ofdDats;
        private System.Windows.Forms.Button btnDBtoDAT;
    }
}