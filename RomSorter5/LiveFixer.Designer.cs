namespace RomSorter5WinForms
{
    partial class LiveFixer
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
            label1 = new System.Windows.Forms.Label();
            txtFolder = new System.Windows.Forms.TextBox();
            fileWatcher = new System.IO.FileSystemWatcher();
            lblStatus = new System.Windows.Forms.Label();
            btnStart = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)fileWatcher).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(61, 15);
            label1.TabIndex = 0;
            label1.Text = "Watching:";
            // 
            // txtFolder
            // 
            txtFolder.Location = new System.Drawing.Point(79, 6);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new System.Drawing.Size(380, 23);
            txtFolder.TabIndex = 1;
            txtFolder.TextChanged += txtFolder_TextChanged;
            // 
            // fileWatcher
            // 
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.IncludeSubdirectories = true;
            fileWatcher.SynchronizingObject = this;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(79, 32);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(0, 15);
            lblStatus.TabIndex = 2;
            // 
            // btnStart
            // 
            btnStart.Location = new System.Drawing.Point(476, 5);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(75, 23);
            btnStart.TabIndex = 3;
            btnStart.Text = "Start!";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // LiveFixer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(822, 99);
            Controls.Add(btnStart);
            Controls.Add(lblStatus);
            Controls.Add(txtFolder);
            Controls.Add(label1);
            Name = "LiveFixer";
            Text = "LiveFixer";
            Load += LiveFixer_Load;
            ((System.ComponentModel.ISupportInitialize)fileWatcher).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFolder;
        private System.IO.FileSystemWatcher fileWatcher;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnStart;
    }
}