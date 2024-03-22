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
            btnQuickMerge = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // btnDetectDupes
            // 
            btnDetectDupes.Location = new System.Drawing.Point(130, 92);
            btnDetectDupes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDetectDupes.Name = "btnDetectDupes";
            btnDetectDupes.Size = new System.Drawing.Size(111, 66);
            btnDetectDupes.TabIndex = 0;
            btnDetectDupes.Text = "Detect Duplicate (Unzipped) Files";
            btnDetectDupes.UseVisualStyleBackColor = true;
            btnDetectDupes.Click += button1_Click;
            // 
            // txtDatPath
            // 
            txtDatPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtDatPath.Location = new System.Drawing.Point(195, 8);
            txtDatPath.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            txtDatPath.Name = "txtDatPath";
            txtDatPath.Size = new System.Drawing.Size(589, 23);
            txtDatPath.TabIndex = 15;
            txtDatPath.Leave += txtDatPath_Leave;
            // 
            // btnDatFolderSelect
            // 
            btnDatFolderSelect.Location = new System.Drawing.Point(136, 4);
            btnDatFolderSelect.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            btnDatFolderSelect.Name = "btnDatFolderSelect";
            btnDatFolderSelect.Size = new System.Drawing.Size(55, 24);
            btnDatFolderSelect.TabIndex = 14;
            btnDatFolderSelect.Text = "Select";
            btnDatFolderSelect.UseVisualStyleBackColor = true;
            btnDatFolderSelect.Click += btnDatFolderSelect_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 9);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(106, 15);
            label1.TabIndex = 13;
            label1.Text = "Dat File: (Optional)";
            // 
            // txtRomPath
            // 
            txtRomPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtRomPath.Location = new System.Drawing.Point(195, 34);
            txtRomPath.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            txtRomPath.Name = "txtRomPath";
            txtRomPath.Size = new System.Drawing.Size(589, 23);
            txtRomPath.TabIndex = 18;
            // 
            // btnRomFolderSelect
            // 
            btnRomFolderSelect.Location = new System.Drawing.Point(136, 30);
            btnRomFolderSelect.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            btnRomFolderSelect.Name = "btnRomFolderSelect";
            btnRomFolderSelect.Size = new System.Drawing.Size(55, 24);
            btnRomFolderSelect.TabIndex = 17;
            btnRomFolderSelect.Text = "Select";
            btnRomFolderSelect.UseVisualStyleBackColor = true;
            btnRomFolderSelect.Click += btnRomFolderSelect_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(10, 36);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(86, 15);
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
            progressBar1.Location = new System.Drawing.Point(10, 389);
            progressBar1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(777, 31);
            progressBar1.TabIndex = 20;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(12, 430);
            lblStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(26, 15);
            lblStatus.TabIndex = 19;
            lblStatus.Text = "Idle";
            // 
            // btnUnzipAll
            // 
            btnUnzipAll.Location = new System.Drawing.Point(246, 92);
            btnUnzipAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnUnzipAll.Name = "btnUnzipAll";
            btnUnzipAll.Size = new System.Drawing.Size(111, 66);
            btnUnzipAll.TabIndex = 21;
            btnUnzipAll.Text = "Unzip All Files";
            btnUnzipAll.UseVisualStyleBackColor = true;
            btnUnzipAll.Click += btnUnzipAll_Click;
            // 
            // btnZipAllFiles
            // 
            btnZipAllFiles.Location = new System.Drawing.Point(12, 92);
            btnZipAllFiles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnZipAllFiles.Name = "btnZipAllFiles";
            btnZipAllFiles.Size = new System.Drawing.Size(111, 66);
            btnZipAllFiles.TabIndex = 22;
            btnZipAllFiles.Text = "Zip (or Convert) All Files";
            btnZipAllFiles.UseVisualStyleBackColor = true;
            btnZipAllFiles.Click += btnZipAllFiles_Click;
            // 
            // btnIdentifyAndZip
            // 
            btnIdentifyAndZip.Location = new System.Drawing.Point(12, 236);
            btnIdentifyAndZip.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnIdentifyAndZip.Name = "btnIdentifyAndZip";
            btnIdentifyAndZip.Size = new System.Drawing.Size(111, 66);
            btnIdentifyAndZip.TabIndex = 23;
            btnIdentifyAndZip.Text = "Rename Single-File Games";
            btnIdentifyAndZip.UseVisualStyleBackColor = true;
            btnIdentifyAndZip.Click += btnIdentifyAndZip_Click;
            // 
            // chkMoveUnidentified
            // 
            chkMoveUnidentified.AutoSize = true;
            chkMoveUnidentified.Location = new System.Drawing.Point(12, 61);
            chkMoveUnidentified.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkMoveUnidentified.Name = "chkMoveUnidentified";
            chkMoveUnidentified.Size = new System.Drawing.Size(303, 19);
            chkMoveUnidentified.TabIndex = 25;
            chkMoveUnidentified.Text = "Move unidentified files to sub-folder during Rename";
            chkMoveUnidentified.UseVisualStyleBackColor = true;
            // 
            // btnCatalog
            // 
            btnCatalog.Location = new System.Drawing.Point(12, 164);
            btnCatalog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCatalog.Name = "btnCatalog";
            btnCatalog.Size = new System.Drawing.Size(111, 66);
            btnCatalog.TabIndex = 27;
            btnCatalog.Text = "Catalog Files";
            btnCatalog.UseVisualStyleBackColor = true;
            btnCatalog.Click += btnCatalog_Click;
            // 
            // btnVerify
            // 
            btnVerify.Location = new System.Drawing.Point(130, 164);
            btnVerify.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnVerify.Name = "btnVerify";
            btnVerify.Size = new System.Drawing.Size(111, 66);
            btnVerify.TabIndex = 28;
            btnVerify.Text = "Verify Catalog";
            btnVerify.UseVisualStyleBackColor = true;
            btnVerify.Click += btnVerify_Click;
            // 
            // btnRenameMultiFile
            // 
            btnRenameMultiFile.Location = new System.Drawing.Point(386, 303);
            btnRenameMultiFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRenameMultiFile.Name = "btnRenameMultiFile";
            btnRenameMultiFile.Size = new System.Drawing.Size(111, 66);
            btnRenameMultiFile.TabIndex = 29;
            btnRenameMultiFile.Text = "Rename Multi-File Games (Incomplete)";
            btnRenameMultiFile.UseVisualStyleBackColor = true;
            btnRenameMultiFile.Visible = false;
            btnRenameMultiFile.Click += btnRenameMultiFile_Click;
            // 
            // btnCreateChds
            // 
            btnCreateChds.Location = new System.Drawing.Point(12, 308);
            btnCreateChds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCreateChds.Name = "btnCreateChds";
            btnCreateChds.Size = new System.Drawing.Size(111, 66);
            btnCreateChds.TabIndex = 30;
            btnCreateChds.Text = "Create CHD Files";
            btnCreateChds.UseVisualStyleBackColor = true;
            btnCreateChds.Click += btnCreateChds_Click;
            // 
            // btnExtractChds
            // 
            btnExtractChds.Location = new System.Drawing.Point(130, 308);
            btnExtractChds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnExtractChds.Name = "btnExtractChds";
            btnExtractChds.Size = new System.Drawing.Size(111, 66);
            btnExtractChds.TabIndex = 31;
            btnExtractChds.Text = "Extract CHD Files";
            btnExtractChds.UseVisualStyleBackColor = true;
            btnExtractChds.Click += btnExtractChds_Click;
            // 
            // btnMakeDat
            // 
            btnMakeDat.Location = new System.Drawing.Point(246, 164);
            btnMakeDat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnMakeDat.Name = "btnMakeDat";
            btnMakeDat.Size = new System.Drawing.Size(111, 66);
            btnMakeDat.TabIndex = 32;
            btnMakeDat.Text = "Make DAT File for Folder";
            btnMakeDat.UseVisualStyleBackColor = true;
            btnMakeDat.Click += btnMakeDat_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(1198, 115);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(138, 15);
            label3.TabIndex = 33;
            label3.Text = "Nothing over here, sorry.";
            // 
            // btn1G1R
            // 
            btn1G1R.Location = new System.Drawing.Point(130, 236);
            btn1G1R.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn1G1R.Name = "btn1G1R";
            btn1G1R.Size = new System.Drawing.Size(111, 66);
            btn1G1R.TabIndex = 35;
            btn1G1R.Text = "Make 1G1R Set";
            btn1G1R.UseVisualStyleBackColor = true;
            btn1G1R.Click += btn1G1R_Click;
            // 
            // btnEverdrive
            // 
            btnEverdrive.Location = new System.Drawing.Point(246, 236);
            btnEverdrive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnEverdrive.Name = "btnEverdrive";
            btnEverdrive.Size = new System.Drawing.Size(111, 66);
            btnEverdrive.TabIndex = 36;
            btnEverdrive.Text = "Everdrive Sort";
            btnEverdrive.UseVisualStyleBackColor = true;
            btnEverdrive.Click += btnEverdrive_Click;
            // 
            // btnCreateM3uPlaylists
            // 
            btnCreateM3uPlaylists.Location = new System.Drawing.Point(246, 308);
            btnCreateM3uPlaylists.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCreateM3uPlaylists.Name = "btnCreateM3uPlaylists";
            btnCreateM3uPlaylists.Size = new System.Drawing.Size(111, 66);
            btnCreateM3uPlaylists.TabIndex = 37;
            btnCreateM3uPlaylists.Text = "Create .m3u Playlists";
            btnCreateM3uPlaylists.UseVisualStyleBackColor = true;
            btnCreateM3uPlaylists.Click += btnCreateM3uPlaylists_Click;
            // 
            // chkRecurse
            // 
            chkRecurse.AutoSize = true;
            chkRecurse.Location = new System.Drawing.Point(381, 61);
            chkRecurse.Name = "chkRecurse";
            chkRecurse.Size = new System.Drawing.Size(158, 19);
            chkRecurse.TabIndex = 38;
            chkRecurse.Text = "Operate on all subfolders";
            chkRecurse.UseVisualStyleBackColor = true;
            // 
            // btnMultiPatch
            // 
            btnMultiPatch.Location = new System.Drawing.Point(381, 92);
            btnMultiPatch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnMultiPatch.Name = "btnMultiPatch";
            btnMultiPatch.Size = new System.Drawing.Size(111, 66);
            btnMultiPatch.TabIndex = 39;
            btnMultiPatch.Text = "Batch-Apply Patches to ROM";
            btnMultiPatch.UseVisualStyleBackColor = true;
            btnMultiPatch.Click += btnMultiPatch_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(539, 236);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(111, 66);
            button1.TabIndex = 40;
            button1.Text = "test lzip logic";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_2;
            // 
            // chkMoveMissedPatches
            // 
            chkMoveMissedPatches.AutoSize = true;
            chkMoveMissedPatches.Location = new System.Drawing.Point(496, 189);
            chkMoveMissedPatches.Name = "chkMoveMissedPatches";
            chkMoveMissedPatches.Size = new System.Drawing.Size(263, 19);
            chkMoveMissedPatches.TabIndex = 41;
            chkMoveMissedPatches.Text = "Move unapplied patches instead of deleteing";
            chkMoveMissedPatches.UseVisualStyleBackColor = true;
            // 
            // btnLzip
            // 
            btnLzip.Location = new System.Drawing.Point(676, 308);
            btnLzip.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnLzip.Name = "btnLzip";
            btnLzip.Size = new System.Drawing.Size(111, 66);
            btnLzip.TabIndex = 42;
            btnLzip.Text = "LZMA-Zip All Files (Max Compression)";
            btnLzip.UseVisualStyleBackColor = true;
            btnLzip.Visible = false;
            btnLzip.Click += btnLzip_Click;
            // 
            // btnPrepNES
            // 
            btnPrepNES.Location = new System.Drawing.Point(496, 92);
            btnPrepNES.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnPrepNES.Name = "btnPrepNES";
            btnPrepNES.Size = new System.Drawing.Size(111, 66);
            btnPrepNES.TabIndex = 43;
            btnPrepNES.Text = "Prep NES Files For MAME";
            btnPrepNES.UseVisualStyleBackColor = true;
            btnPrepNES.Click += btnPrepNES_Click;
            // 
            // btnQuickMerge
            // 
            btnQuickMerge.Location = new System.Drawing.Point(381, 164);
            btnQuickMerge.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnQuickMerge.Name = "btnQuickMerge";
            btnQuickMerge.Size = new System.Drawing.Size(111, 66);
            btnQuickMerge.TabIndex = 44;
            btnQuickMerge.Text = "Quick-Merge 2 Folders";
            btnQuickMerge.UseVisualStyleBackColor = true;
            btnQuickMerge.Click += btnQuickMerge_Click;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(670, 92);
            button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(111, 66);
            button2.TabIndex = 45;
            button2.Text = "LiveFixer";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(794, 454);
            Controls.Add(button2);
            Controls.Add(btnQuickMerge);
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
        private System.Windows.Forms.Button btnQuickMerge;
        private System.Windows.Forms.Button button2;
    }
}