namespace RomSorter
{
    partial class Form1
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
            this.btnPickFolder = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnSort = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.chkMultithread = new System.Windows.Forms.CheckBox();
            this.chkZipIdentified = new System.Windows.Forms.CheckBox();
            this.btnReport = new System.Windows.Forms.Button();
            this.chkMoveUnidentified = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnPickFolder
            // 
            this.btnPickFolder.Location = new System.Drawing.Point(12, 12);
            this.btnPickFolder.Name = "btnPickFolder";
            this.btnPickFolder.Size = new System.Drawing.Size(171, 22);
            this.btnPickFolder.TabIndex = 0;
            this.btnPickFolder.Text = "Pick Folder";
            this.btnPickFolder.UseVisualStyleBackColor = true;
            this.btnPickFolder.Click += new System.EventHandler(this.button1_Click);
            this.btnPickFolder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.CheckFileExists = false;
            this.openFileDialog1.FileName = "Folder Selection";
            this.openFileDialog1.ValidateNames = false;
            // 
            // btnSort
            // 
            this.btnSort.Enabled = false;
            this.btnSort.Location = new System.Drawing.Point(12, 66);
            this.btnSort.Name = "btnSort";
            this.btnSort.Size = new System.Drawing.Size(76, 76);
            this.btnSort.TabIndex = 1;
            this.btnSort.Text = "Sort!";
            this.btnSort.UseVisualStyleBackColor = true;
            this.btnSort.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(16, 271);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(60, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status: Idle";
            // 
            // chkMultithread
            // 
            this.chkMultithread.AutoSize = true;
            this.chkMultithread.Checked = true;
            this.chkMultithread.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMultithread.Location = new System.Drawing.Point(12, 165);
            this.chkMultithread.Name = "chkMultithread";
            this.chkMultithread.Size = new System.Drawing.Size(114, 17);
            this.chkMultithread.TabIndex = 5;
            this.chkMultithread.Text = "Use Multithreading";
            this.chkMultithread.UseVisualStyleBackColor = true;
            // 
            // chkZipIdentified
            // 
            this.chkZipIdentified.AutoSize = true;
            this.chkZipIdentified.Location = new System.Drawing.Point(12, 188);
            this.chkZipIdentified.Name = "chkZipIdentified";
            this.chkZipIdentified.Size = new System.Drawing.Size(161, 17);
            this.chkZipIdentified.TabIndex = 6;
            this.chkZipIdentified.Text = "Zip games instead of moving";
            this.chkZipIdentified.UseVisualStyleBackColor = true;
            // 
            // btnReport
            // 
            this.btnReport.Enabled = false;
            this.btnReport.Location = new System.Drawing.Point(107, 66);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(76, 76);
            this.btnReport.TabIndex = 7;
            this.btnReport.Text = "Report!";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.button3_Click);
            // 
            // chkMoveUnidentified
            // 
            this.chkMoveUnidentified.AutoSize = true;
            this.chkMoveUnidentified.Location = new System.Drawing.Point(12, 211);
            this.chkMoveUnidentified.Name = "chkMoveUnidentified";
            this.chkMoveUnidentified.Size = new System.Drawing.Size(195, 17);
            this.chkMoveUnidentified.TabIndex = 8;
            this.chkMoveUnidentified.Text = "Move Unidentified Files (dangerous)";
            this.chkMoveUnidentified.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(207, 356);
            this.Controls.Add(this.chkMoveUnidentified);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.chkZipIdentified);
            this.Controls.Add(this.chkMultithread);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSort);
            this.Controls.Add(this.btnPickFolder);
            this.Name = "Form1";
            this.Text = "ROMSorter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPickFolder;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnSort;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkMultithread;
        private System.Windows.Forms.CheckBox chkZipIdentified;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.CheckBox chkMoveUnidentified;
    }
}

