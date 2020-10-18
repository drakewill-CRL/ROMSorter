
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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dat Files:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 32);
            this.label2.TabIndex = 1;
            this.label2.Text = "Rom Files:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(169, 32);
            this.label3.TabIndex = 2;
            this.label3.Text = "Output Folder:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 273);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(211, 32);
            this.label4.TabIndex = 3;
            this.label4.Text = "Single-File Games:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 375);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(201, 32);
            this.label5.TabIndex = 4;
            this.label5.Text = "Multi-File Games:";
            // 
            // comboBox1
            // 
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Zipped",
            "Unzipped"});
            this.comboBox1.Location = new System.Drawing.Point(369, 270);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(226, 40);
            this.comboBox1.TabIndex = 5;
            // 
            // comboBox2
            // 
            this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Zipped",
            "Folder"});
            this.comboBox2.Location = new System.Drawing.Point(369, 372);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(226, 40);
            this.comboBox2.TabIndex = 6;
            // 
            // btnSort
            // 
            this.btnSort.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSort.Location = new System.Drawing.Point(72, 515);
            this.btnSort.Name = "btnSort";
            this.btnSort.Size = new System.Drawing.Size(143, 63);
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
            this.btnDatFolderSelect.Location = new System.Drawing.Point(216, 19);
            this.btnDatFolderSelect.Name = "btnDatFolderSelect";
            this.btnDatFolderSelect.Size = new System.Drawing.Size(102, 51);
            this.btnDatFolderSelect.TabIndex = 8;
            this.btnDatFolderSelect.Text = "Select";
            this.btnDatFolderSelect.UseVisualStyleBackColor = true;
            this.btnDatFolderSelect.Click += new System.EventHandler(this.btnDatFolderSelect_Click);
            // 
            // btnRomFolderSelect
            // 
            this.btnRomFolderSelect.Location = new System.Drawing.Point(216, 97);
            this.btnRomFolderSelect.Name = "btnRomFolderSelect";
            this.btnRomFolderSelect.Size = new System.Drawing.Size(102, 51);
            this.btnRomFolderSelect.TabIndex = 9;
            this.btnRomFolderSelect.Text = "Select";
            this.btnRomFolderSelect.UseVisualStyleBackColor = true;
            this.btnRomFolderSelect.Click += new System.EventHandler(this.btnRomFolderSelect_Click);
            // 
            // btnOutputFolderSelect
            // 
            this.btnOutputFolderSelect.Location = new System.Drawing.Point(216, 176);
            this.btnOutputFolderSelect.Name = "btnOutputFolderSelect";
            this.btnOutputFolderSelect.Size = new System.Drawing.Size(102, 51);
            this.btnOutputFolderSelect.TabIndex = 10;
            this.btnOutputFolderSelect.Text = "Select";
            this.btnOutputFolderSelect.UseVisualStyleBackColor = true;
            this.btnOutputFolderSelect.Click += new System.EventHandler(this.btnOutputFolderSelect_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(18, 702);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(53, 32);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Text = "Idle";
            // 
            // txtDatPath
            // 
            this.txtDatPath.Location = new System.Drawing.Point(370, 25);
            this.txtDatPath.Name = "txtDatPath";
            this.txtDatPath.Size = new System.Drawing.Size(418, 39);
            this.txtDatPath.TabIndex = 12;
            // 
            // txtRomPath
            // 
            this.txtRomPath.Location = new System.Drawing.Point(370, 103);
            this.txtRomPath.Name = "txtRomPath";
            this.txtRomPath.Size = new System.Drawing.Size(418, 39);
            this.txtRomPath.TabIndex = 13;
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(370, 182);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(418, 39);
            this.txtOutputPath.TabIndex = 14;
            // 
            // SimpleInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 737);
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
    }
}