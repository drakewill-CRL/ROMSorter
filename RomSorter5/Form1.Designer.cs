namespace RomSorter5
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
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
            this.chkPreserveOriginals = new System.Windows.Forms.CheckBox();
            this.txtMessageLog = new System.Windows.Forms.TextBox();
            this.btnPickDestination = new System.Windows.Forms.Button();
            this.chkDisplayAllActions = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnPickFolder
            // 
            this.btnPickFolder.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnPickFolder.Location = new System.Drawing.Point(14, 14);
            this.btnPickFolder.Name = "btnPickFolder";
            this.btnPickFolder.Size = new System.Drawing.Size(200, 25);
            this.btnPickFolder.TabIndex = 0;
            this.btnPickFolder.Text = "Pick Source Folder";
            this.btnPickFolder.UseVisualStyleBackColor = true;
            this.btnPickFolder.Click += new System.EventHandler(this.btnPickFolder_Click);
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
            this.btnSort.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSort.Location = new System.Drawing.Point(14, 76);
            this.btnSort.Name = "btnSort";
            this.btnSort.Size = new System.Drawing.Size(89, 88);
            this.btnSort.TabIndex = 1;
            this.btnSort.Text = "Identify, Sort, and Rename";
            this.btnSort.UseVisualStyleBackColor = true;
            this.btnSort.Click += new System.EventHandler(this.btnSort_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblStatus.Location = new System.Drawing.Point(19, 326);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(66, 15);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Counting...";
            // 
            // chkMultithread
            // 
            this.chkMultithread.AutoSize = true;
            this.chkMultithread.Checked = true;
            this.chkMultithread.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMultithread.Enabled = false;
            this.chkMultithread.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkMultithread.Location = new System.Drawing.Point(19, 170);
            this.chkMultithread.Name = "chkMultithread";
            this.chkMultithread.Size = new System.Drawing.Size(133, 20);
            this.chkMultithread.TabIndex = 5;
            this.chkMultithread.Text = "Use Multithreading";
            this.chkMultithread.UseVisualStyleBackColor = true;
            // 
            // chkZipIdentified
            // 
            this.chkZipIdentified.AutoSize = true;
            this.chkZipIdentified.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkZipIdentified.Location = new System.Drawing.Point(19, 197);
            this.chkZipIdentified.Name = "chkZipIdentified";
            this.chkZipIdentified.Size = new System.Drawing.Size(186, 20);
            this.chkZipIdentified.TabIndex = 6;
            this.chkZipIdentified.Text = "Zip games instead of moving";
            this.chkZipIdentified.UseVisualStyleBackColor = true;
            // 
            // btnReport
            // 
            this.btnReport.Enabled = false;
            this.btnReport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnReport.Location = new System.Drawing.Point(125, 76);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(89, 88);
            this.btnReport.TabIndex = 7;
            this.btnReport.Text = "Identify Only";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // chkMoveUnidentified
            // 
            this.chkMoveUnidentified.AutoSize = true;
            this.chkMoveUnidentified.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkMoveUnidentified.Location = new System.Drawing.Point(19, 223);
            this.chkMoveUnidentified.Name = "chkMoveUnidentified";
            this.chkMoveUnidentified.Size = new System.Drawing.Size(156, 20);
            this.chkMoveUnidentified.TabIndex = 8;
            this.chkMoveUnidentified.Text = "Move Unidentified Files";
            this.chkMoveUnidentified.UseVisualStyleBackColor = true;
            // 
            // chkPreserveOriginals
            // 
            this.chkPreserveOriginals.AutoSize = true;
            this.chkPreserveOriginals.Checked = true;
            this.chkPreserveOriginals.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPreserveOriginals.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkPreserveOriginals.Location = new System.Drawing.Point(19, 250);
            this.chkPreserveOriginals.Name = "chkPreserveOriginals";
            this.chkPreserveOriginals.Size = new System.Drawing.Size(161, 20);
            this.chkPreserveOriginals.TabIndex = 9;
            this.chkPreserveOriginals.Text = "Keep Identified Originals";
            this.chkPreserveOriginals.UseVisualStyleBackColor = true;
            // 
            // txtMessageLog
            // 
            this.txtMessageLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessageLog.Location = new System.Drawing.Point(14, 344);
            this.txtMessageLog.Multiline = true;
            this.txtMessageLog.Name = "txtMessageLog";
            this.txtMessageLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessageLog.Size = new System.Drawing.Size(198, 175);
            this.txtMessageLog.TabIndex = 10;
            // 
            // btnPickDestination
            // 
            this.btnPickDestination.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnPickDestination.Location = new System.Drawing.Point(14, 46);
            this.btnPickDestination.Name = "btnPickDestination";
            this.btnPickDestination.Size = new System.Drawing.Size(200, 25);
            this.btnPickDestination.TabIndex = 11;
            this.btnPickDestination.Text = "Pick Destination Folder";
            this.btnPickDestination.UseVisualStyleBackColor = true;
            this.btnPickDestination.Click += new System.EventHandler(this.btnPickDestination_Click);
            // 
            // chkDisplayAllActions
            // 
            this.chkDisplayAllActions.AutoSize = true;
            this.chkDisplayAllActions.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkDisplayAllActions.Location = new System.Drawing.Point(19, 276);
            this.chkDisplayAllActions.Name = "chkDisplayAllActions";
            this.chkDisplayAllActions.Size = new System.Drawing.Size(130, 20);
            this.chkDisplayAllActions.TabIndex = 9;
            this.chkDisplayAllActions.Text = "Display All Actions";
            this.chkDisplayAllActions.UseVisualStyleBackColor = true;
            this.chkDisplayAllActions.CheckedChanged += new System.EventHandler(this.chkDisplayAllActions_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 533);
            this.Controls.Add(this.chkDisplayAllActions);
            this.Controls.Add(this.btnPickDestination);
            this.Controls.Add(this.txtMessageLog);
            this.Controls.Add(this.chkPreserveOriginals);
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
        private System.Windows.Forms.CheckBox chkPreserveOriginals;
        private System.Windows.Forms.TextBox txtMessageLog;
        private System.Windows.Forms.Button btnPickDestination;
        private System.Windows.Forms.CheckBox chkDisplayAllActions;
    }
}

