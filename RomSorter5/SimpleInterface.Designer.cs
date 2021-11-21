
namespace RomSorter5WinForms
{
    partial class SimpleInterface
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.btnSort = new System.Windows.Forms.Button();
            this.ofd1 = new System.Windows.Forms.OpenFileDialog();
            this.btnDatFolderSelect = new System.Windows.Forms.Button();
            this.btnRomFolderSelect = new System.Windows.Forms.Button();
            this.btnOutputFolderSelect = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtDatPath = new System.Windows.Forms.TextBox();
            this.txtRomPath = new System.Windows.Forms.TextBox();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dat Files:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 50);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Rom Files:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 87);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Output Folder:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 128);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "Output Files:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 176);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Multi-File Games:";
            this.label5.Visible = false;
            // 
            // comboBox1
            // 
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Zipped",
            "Unzipped",
            "Rename In Place"});
            this.comboBox1.Location = new System.Drawing.Point(199, 127);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(124, 23);
            this.comboBox1.TabIndex = 5;
            // 
            // comboBox2
            // 
            this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Zipped",
            "Folder"});
            this.comboBox2.Location = new System.Drawing.Point(199, 174);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(124, 23);
            this.comboBox2.TabIndex = 6;
            this.comboBox2.Visible = false;
            // 
            // btnSort
            // 
            this.btnSort.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSort.Location = new System.Drawing.Point(39, 241);
            this.btnSort.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.btnSort.Name = "btnSort";
            this.btnSort.Size = new System.Drawing.Size(77, 30);
            this.btnSort.TabIndex = 7;
            this.btnSort.Text = "Sort!";
            this.btnSort.UseVisualStyleBackColor = true;
            this.btnSort.Click += new System.EventHandler(this.btnSort_Click);
            // 
            // ofd1
            // 
            this.ofd1.CheckFileExists = false;
            this.ofd1.FileName = "Folder Selection";
            this.ofd1.ValidateNames = false;
            // 
            // btnDatFolderSelect
            // 
            this.btnDatFolderSelect.Location = new System.Drawing.Point(116, 9);
            this.btnDatFolderSelect.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.btnDatFolderSelect.Name = "btnDatFolderSelect";
            this.btnDatFolderSelect.Size = new System.Drawing.Size(55, 24);
            this.btnDatFolderSelect.TabIndex = 8;
            this.btnDatFolderSelect.Text = "Select";
            this.btnDatFolderSelect.UseVisualStyleBackColor = true;
            this.btnDatFolderSelect.Click += new System.EventHandler(this.btnDatFolderSelect_Click);
            // 
            // btnRomFolderSelect
            // 
            this.btnRomFolderSelect.Location = new System.Drawing.Point(116, 45);
            this.btnRomFolderSelect.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.btnRomFolderSelect.Name = "btnRomFolderSelect";
            this.btnRomFolderSelect.Size = new System.Drawing.Size(55, 24);
            this.btnRomFolderSelect.TabIndex = 9;
            this.btnRomFolderSelect.Text = "Select";
            this.btnRomFolderSelect.UseVisualStyleBackColor = true;
            this.btnRomFolderSelect.Click += new System.EventHandler(this.btnRomFolderSelect_Click);
            // 
            // btnOutputFolderSelect
            // 
            this.btnOutputFolderSelect.Location = new System.Drawing.Point(116, 82);
            this.btnOutputFolderSelect.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.btnOutputFolderSelect.Name = "btnOutputFolderSelect";
            this.btnOutputFolderSelect.Size = new System.Drawing.Size(55, 24);
            this.btnOutputFolderSelect.TabIndex = 10;
            this.btnOutputFolderSelect.Text = "Select";
            this.btnOutputFolderSelect.UseVisualStyleBackColor = true;
            this.btnOutputFolderSelect.Click += new System.EventHandler(this.btnOutputFolderSelect_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(10, 329);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(26, 15);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Text = "Idle";
            // 
            // txtDatPath
            // 
            this.txtDatPath.Location = new System.Drawing.Point(199, 12);
            this.txtDatPath.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.txtDatPath.Name = "txtDatPath";
            this.txtDatPath.Size = new System.Drawing.Size(227, 23);
            this.txtDatPath.TabIndex = 12;
            // 
            // txtRomPath
            // 
            this.txtRomPath.Location = new System.Drawing.Point(199, 48);
            this.txtRomPath.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.txtRomPath.Name = "txtRomPath";
            this.txtRomPath.Size = new System.Drawing.Size(227, 23);
            this.txtRomPath.TabIndex = 13;
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(199, 85);
            this.txtOutputPath.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(227, 23);
            this.txtOutputPath.TabIndex = 14;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(6, 283);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(418, 31);
            this.progressBar1.TabIndex = 15;
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button1.Location = new System.Drawing.Point(319, 241);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 30);
            this.button1.TabIndex = 16;
            this.button1.Text = "Everdrive Sort";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SimpleInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 345);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.txtRomPath);
            this.Controls.Add(this.txtDatPath);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnOutputFolderSelect);
            this.Controls.Add(this.btnRomFolderSelect);
            this.Controls.Add(this.btnDatFolderSelect);
            this.Controls.Add(this.btnSort);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.Name = "SimpleInterface";
            this.Text = "SimpleInterface";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button btnSort;
        private System.Windows.Forms.OpenFileDialog ofd1;
        private System.Windows.Forms.Button btnDatFolderSelect;
        private System.Windows.Forms.Button btnRomFolderSelect;
        private System.Windows.Forms.Button btnOutputFolderSelect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtDatPath;
        private System.Windows.Forms.TextBox txtRomPath;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
    }
}