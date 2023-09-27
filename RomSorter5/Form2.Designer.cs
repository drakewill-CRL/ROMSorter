namespace RomSorter5WinForms {
    partial class Form2 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            //base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnDetectDupes = new System.Windows.Forms.Button();
            txtDatPath = new System.Windows.Forms.TextBox();
            btnDatFolderSelect = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            txtRomPath = new System.Windows.Forms.TextBox();
            btnRomFolderSelect = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            ofd1 = new System.Windows.Forms.OpenFileDialog();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            lblStatus = new System.Windows.Forms.Label();
            btnUnzipAll = new System.Windows.Forms.Button();
            btnZipAllFiles = new System.Windows.Forms.Button();
            btnIdentifyAndZip = new System.Windows.Forms.Button();
            chkMoveUnidentified = new System.Windows.Forms.CheckBox();
            btnCatalog = new System.Windows.Forms.Button();
            btnVerify = new System.Windows.Forms.Button();
            btnRenameMultiFile = new System.Windows.Forms.Button();
            btnCreateChds = new System.Windows.Forms.Button();
            btnExtractChds = new System.Windows.Forms.Button();
            btnMakeDat = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            btn1G1R = new System.Windows.Forms.Button();
            btnEverdrive = new System.Windows.Forms.Button();
            btnCreateM3uPlaylists = new System.Windows.Forms.Button();
            chkRecurse = new System.Windows.Forms.CheckBox();
            btnMultiPatch = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            chkMoveMissedPatches = new System.Windows.Forms.CheckBox();
            btnLzip = new System.Windows.Forms.Button();
            btnPrepNES = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // btnDetectDupes
            // 
            btnDetectDupes.Location = new System.Drawing.Point(130, 104);
            btnDetectDupes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDetectDupes.Name = "btnDetectDupes";
            btnDetectDupes.Size = new System.Drawing.Size(111, 75);
            btnDetectDupes.TabIndex = 0;
            btnDetectDupes.Text = "Detect Duplicate (Unzipped) Files";
            btnDetectDupes.UseVisualStyleBackColor = true;
            btnDetectDupes.Click += button1_Click;
            // 
            // txtDatPath
            // 
            txtDatPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtDatPath.Location = new System.Drawing.Point(195, 9);
            txtDatPath.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            txtDatPath.Name = "txtDatPath";
            txtDatPath.Size = new System.Drawing.Size(589, 25);
            txtDatPath.TabIndex = 15;
            txtDatPath.Leave += txtDatPath_Leave;
            // 
            // btnDatFolderSelect
            // 
            btnDatFolderSelect.Location = new System.Drawing.Point(136, 5);
            btnDatFolderSelect.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            btnDatFolderSelect.Name = "btnDatFolderSelect";
            btnDatFolderSelect.Size = new System.Drawing.Size(55, 27);
            btnDatFolderSelect.TabIndex = 14;
            btnDatFolderSelect.Text = "Select";
            btnDatFolderSelect.UseVisualStyleBackColor = true;
            btnDatFolderSelect.Click += btnDatFolderSelect_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 10);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(116, 17);
            label1.TabIndex = 13;
            label1.Text = "Dat File: (Optional)";
            // 
            // txtRomPath
            // 
            txtRomPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtRomPath.Location = new System.Drawing.Point(195, 39);
            txtRomPath.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            txtRomPath.Name = "txtRomPath";
            txtRomPath.Size = new System.Drawing.Size(589, 25);
            txtRomPath.TabIndex = 18;
            // 
            // btnRomFolderSelect
            // 
            btnRomFolderSelect.Location = new System.Drawing.Point(136, 34);
            btnRomFolderSelect.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            btnRomFolderSelect.Name = "btnRomFolderSelect";
            btnRomFolderSelect.Size = new System.Drawing.Size(55, 27);
            btnRomFolderSelect.TabIndex = 17;
            btnRomFolderSelect.Text = "Select";
            btnRomFolderSelect.UseVisualStyleBackColor = true;
            btnRomFolderSelect.Click += btnRomFolderSelect_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(10, 41);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(95, 17);
            label2.TabIndex = 16;
            label2.Text = "Current Folder:";
            // 
            // ofd1
            // 
            ofd1.CheckFileExists = false;
            ofd1.FileName = "Folder Selection";
            ofd1.ValidateNames = false;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            progressBar1.Location = new System.Drawing.Point(10, 441);
            progressBar1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(777, 35);
            progressBar1.TabIndex = 20;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(12, 487);
            lblStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(29, 17);
            lblStatus.TabIndex = 19;
            lblStatus.Text = "Idle";
            // 
            // btnUnzipAll
            // 
            btnUnzipAll.Location = new System.Drawing.Point(246, 104);
            btnUnzipAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnUnzipAll.Name = "btnUnzipAll";
            btnUnzipAll.Size = new System.Drawing.Size(111, 75);
            btnUnzipAll.TabIndex = 21;
            btnUnzipAll.Text = "Unzip All Files";
            btnUnzipAll.UseVisualStyleBackColor = true;
            btnUnzipAll.Click += btnUnzipAll_Click;
            // 
            // btnZipAllFiles
            // 
            btnZipAllFiles.Location = new System.Drawing.Point(12, 104);
            btnZipAllFiles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnZipAllFiles.Name = "btnZipAllFiles";
            btnZipAllFiles.Size = new System.Drawing.Size(111, 75);
            btnZipAllFiles.TabIndex = 22;
            btnZipAllFiles.Text = "Zip (or Convert) All Files";
            btnZipAllFiles.UseVisualStyleBackColor = true;
            btnZipAllFiles.Click += btnZipAllFiles_Click;
            // 
            // btnIdentifyAndZip
            // 
            btnIdentifyAndZip.Location = new System.Drawing.Point(12, 267);
            btnIdentifyAndZip.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnIdentifyAndZip.Name = "btnIdentifyAndZip";
            btnIdentifyAndZip.Size = new System.Drawing.Size(111, 75);
            btnIdentifyAndZip.TabIndex = 23;
            btnIdentifyAndZip.Text = "Rename Single-File Games";
            btnIdentifyAndZip.UseVisualStyleBackColor = true;
            btnIdentifyAndZip.Click += btnIdentifyAndZip_Click;
            // 
            // chkMoveUnidentified
            // 
            chkMoveUnidentified.AutoSize = true;
            chkMoveUnidentified.Location = new System.Drawing.Point(12, 69);
            chkMoveUnidentified.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkMoveUnidentified.Name = "chkMoveUnidentified";
            chkMoveUnidentified.Size = new System.Drawing.Size(333, 21);
            chkMoveUnidentified.TabIndex = 25;
            chkMoveUnidentified.Text = "Move unidentified files to sub-folder during Rename";
            chkMoveUnidentified.UseVisualStyleBackColor = true;
            // 
            // btnCatalog
            // 
            btnCatalog.Location = new System.Drawing.Point(12, 186);
            btnCatalog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCatalog.Name = "btnCatalog";
            btnCatalog.Size = new System.Drawing.Size(111, 75);
            btnCatalog.TabIndex = 27;
            btnCatalog.Text = "Catalog Files";
            btnCatalog.UseVisualStyleBackColor = true;
            btnCatalog.Click += btnCatalog_Click;
            // 
            // btnVerify
            // 
            btnVerify.Location = new System.Drawing.Point(130, 186);
            btnVerify.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnVerify.Name = "btnVerify";
            btnVerify.Size = new System.Drawing.Size(111, 75);
            btnVerify.TabIndex = 28;
            btnVerify.Text = "Verify Catalog";
            btnVerify.UseVisualStyleBackColor = true;
            btnVerify.Click += btnVerify_Click;
            // 
            // btnRenameMultiFile
            // 
            btnRenameMultiFile.Location = new System.Drawing.Point(386, 343);
            btnRenameMultiFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRenameMultiFile.Name = "btnRenameMultiFile";
            btnRenameMultiFile.Size = new System.Drawing.Size(111, 75);
            btnRenameMultiFile.TabIndex = 29;
            btnRenameMultiFile.Text = "Rename Multi-File Games (Incomplete)";
            btnRenameMultiFile.UseVisualStyleBackColor = true;
            btnRenameMultiFile.Visible = false;
            btnRenameMultiFile.Click += btnRenameMultiFile_Click;
            // 
            // btnCreateChds
            // 
            btnCreateChds.Location = new System.Drawing.Point(12, 349);
            btnCreateChds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCreateChds.Name = "btnCreateChds";
            btnCreateChds.Size = new System.Drawing.Size(111, 75);
            btnCreateChds.TabIndex = 30;
            btnCreateChds.Text = "Create CHD Files";
            btnCreateChds.UseVisualStyleBackColor = true;
            btnCreateChds.Click += btnCreateChds_Click;
            // 
            // btnExtractChds
            // 
            btnExtractChds.Location = new System.Drawing.Point(130, 349);
            btnExtractChds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnExtractChds.Name = "btnExtractChds";
            btnExtractChds.Size = new System.Drawing.Size(111, 75);
            btnExtractChds.TabIndex = 31;
            btnExtractChds.Text = "Extract CHD Files";
            btnExtractChds.UseVisualStyleBackColor = true;
            btnExtractChds.Click += btnExtractChds_Click;
            // 
            // btnMakeDat
            // 
            btnMakeDat.Location = new System.Drawing.Point(246, 186);
            btnMakeDat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnMakeDat.Name = "btnMakeDat";
            btnMakeDat.Size = new System.Drawing.Size(111, 75);
            btnMakeDat.TabIndex = 32;
            btnMakeDat.Text = "Make DAT File for Folder";
            btnMakeDat.UseVisualStyleBackColor = true;
            btnMakeDat.Click += btnMakeDat_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(1198, 130);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(155, 17);
            label3.TabIndex = 33;
            label3.Text = "Nothing over here, sorry.";
            // 
            // btn1G1R
            // 
            btn1G1R.Location = new System.Drawing.Point(130, 267);
            btn1G1R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn1G1R.Name = "btn1G1R";
            btn1G1R.Size = new System.Drawing.Size(111, 75);
            btn1G1R.TabIndex = 35;
            btn1G1R.Text = "Make 1G1R Set";
            btn1G1R.UseVisualStyleBackColor = true;
            btn1G1R.Click += btn1G1R_Click;
            // 
            // btnEverdrive
            // 
            btnEverdrive.Location = new System.Drawing.Point(246, 267);
            btnEverdrive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnEverdrive.Name = "btnEverdrive";
            btnEverdrive.Size = new System.Drawing.Size(111, 75);
            btnEverdrive.TabIndex = 36;
            btnEverdrive.Text = "Everdrive Sort";
            btnEverdrive.UseVisualStyleBackColor = true;
            btnEverdrive.Click += btnEverdrive_Click;
            // 
            // btnCreateM3uPlaylists
            // 
            btnCreateM3uPlaylists.Location = new System.Drawing.Point(246, 349);
            btnCreateM3uPlaylists.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCreateM3uPlaylists.Name = "btnCreateM3uPlaylists";
            btnCreateM3uPlaylists.Size = new System.Drawing.Size(111, 75);
            btnCreateM3uPlaylists.TabIndex = 37;
            btnCreateM3uPlaylists.Text = "Create .m3u Playlists";
            btnCreateM3uPlaylists.UseVisualStyleBackColor = true;
            btnCreateM3uPlaylists.Click += btnCreateM3uPlaylists_Click;
            // 
            // chkRecurse
            // 
            chkRecurse.AutoSize = true;
            chkRecurse.Location = new System.Drawing.Point(381, 69);
            chkRecurse.Name = "chkRecurse";
            chkRecurse.Size = new System.Drawing.Size(177, 21);
            chkRecurse.TabIndex = 38;
            chkRecurse.Text = "Operate on all subfolders";
            chkRecurse.UseVisualStyleBackColor = true;
            // 
            // btnMultiPatch
            // 
            btnMultiPatch.Location = new System.Drawing.Point(381, 104);
            btnMultiPatch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnMultiPatch.Name = "btnMultiPatch";
            btnMultiPatch.Size = new System.Drawing.Size(111, 75);
            btnMultiPatch.TabIndex = 39;
            btnMultiPatch.Text = "Batch-Apply Patches to ROM";
            btnMultiPatch.UseVisualStyleBackColor = true;
            btnMultiPatch.Click += btnMultiPatch_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(539, 267);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(111, 75);
            button1.TabIndex = 40;
            button1.Text = "test lzip logic";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_2;
            // 
            // chkMoveMissedPatches
            // 
            chkMoveMissedPatches.AutoSize = true;
            chkMoveMissedPatches.Location = new System.Drawing.Point(496, 214);
            chkMoveMissedPatches.Name = "chkMoveMissedPatches";
            chkMoveMissedPatches.Size = new System.Drawing.Size(291, 21);
            chkMoveMissedPatches.TabIndex = 41;
            chkMoveMissedPatches.Text = "Move unapplied patches instead of deleteing";
            chkMoveMissedPatches.UseVisualStyleBackColor = true;
            // 
            // btnLzip
            // 
            btnLzip.Location = new System.Drawing.Point(676, 349);
            btnLzip.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnLzip.Name = "btnLzip";
            btnLzip.Size = new System.Drawing.Size(111, 75);
            btnLzip.TabIndex = 42;
            btnLzip.Text = "LZMA-Zip All Files (Max Compression)";
            btnLzip.UseVisualStyleBackColor = true;
            btnLzip.Visible = false;
            btnLzip.Click += btnLzip_Click;
            // 
            // btnPrepNES
            // 
            btnPrepNES.Location = new System.Drawing.Point(496, 104);
            btnPrepNES.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnPrepNES.Name = "btnPrepNES";
            btnPrepNES.Size = new System.Drawing.Size(111, 75);
            btnPrepNES.TabIndex = 43;
            btnPrepNES.Text = "Prep NES Files For MAME";
            btnPrepNES.UseVisualStyleBackColor = true;
            btnPrepNES.Click += btnPrepNES_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(794, 515);
            Controls.Add(btnPrepNES);
            Controls.Add(btnLzip);
            Controls.Add(chkMoveMissedPatches);
            Controls.Add(button1);
            Controls.Add(btnMultiPatch);
            Controls.Add(chkRecurse);
            Controls.Add(btnCreateM3uPlaylists);
            Controls.Add(btnEverdrive);
            Controls.Add(btn1G1R);
            Controls.Add(label3);
            Controls.Add(btnMakeDat);
            Controls.Add(btnExtractChds);
            Controls.Add(btnCreateChds);
            Controls.Add(btnRenameMultiFile);
            Controls.Add(btnVerify);
            Controls.Add(btnCatalog);
            Controls.Add(chkMoveUnidentified);
            Controls.Add(btnIdentifyAndZip);
            Controls.Add(btnZipAllFiles);
            Controls.Add(btnUnzipAll);
            Controls.Add(progressBar1);
            Controls.Add(lblStatus);
            Controls.Add(txtRomPath);
            Controls.Add(btnRomFolderSelect);
            Controls.Add(label2);
            Controls.Add(txtDatPath);
            Controls.Add(btnDatFolderSelect);
            Controls.Add(label1);
            Controls.Add(btnDetectDupes);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "Form2";
            Text = "ROMSorter (Release 7)";
            Shown += Form2_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnDetectDupes;
        private System.Windows.Forms.TextBox txtDatPath;
        private System.Windows.Forms.Button btnDatFolderSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRomPath;
        private System.Windows.Forms.Button btnRomFolderSelect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog ofd1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnUnzipAll;
        private System.Windows.Forms.Button btnZipAllFiles;
        private System.Windows.Forms.Button btnIdentifyAndZip;
        private System.Windows.Forms.CheckBox chkMoveUnidentified;
        private System.Windows.Forms.Button btnCatalog;
        private System.Windows.Forms.Button btnVerify;
        private System.Windows.Forms.Button btnRenameMultiFile;
        private System.Windows.Forms.Button btnCreateChds;
        private System.Windows.Forms.Button btnExtractChds;
        private System.Windows.Forms.Button btnMakeDat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn1G1R;
        private System.Windows.Forms.Button btnEverdrive;
        private System.Windows.Forms.Button btnCreateM3uPlaylists;
        private System.Windows.Forms.CheckBox chkRecurse;
        private System.Windows.Forms.Button btnMultiPatch;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkMoveMissedPatches;
        private System.Windows.Forms.Button btnLzip;
        private System.Windows.Forms.Button btnPrepNES;
    }
}