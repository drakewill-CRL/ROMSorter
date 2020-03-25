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
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.chkUnzip = new System.Windows.Forms.CheckBox();
            this.chkReadZip = new System.Windows.Forms.CheckBox();
            this.chkMultithread = new System.Windows.Forms.CheckBox();
            this.chkReZip = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 22);
            this.button1.TabIndex = 0;
            this.button1.Text = "Pick Folder";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.CheckFileExists = false;
            this.openFileDialog1.FileName = "Folder Selection";
            this.openFileDialog1.ValidateNames = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 66);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(76, 76);
            this.button2.TabIndex = 1;
            this.button2.Text = "Sort!";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
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
            // chkUnzip
            // 
            this.chkUnzip.AutoSize = true;
            this.chkUnzip.Checked = true;
            this.chkUnzip.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnzip.Location = new System.Drawing.Point(19, 183);
            this.chkUnzip.Name = "chkUnzip";
            this.chkUnzip.Size = new System.Drawing.Size(150, 17);
            this.chkUnzip.TabIndex = 3;
            this.chkUnzip.Text = "Unzip games from .zip files";
            this.chkUnzip.UseVisualStyleBackColor = true;
            // 
            // chkReadZip
            // 
            this.chkReadZip.AutoSize = true;
            this.chkReadZip.Checked = true;
            this.chkReadZip.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReadZip.Location = new System.Drawing.Point(19, 160);
            this.chkReadZip.Name = "chkReadZip";
            this.chkReadZip.Size = new System.Drawing.Size(122, 17);
            this.chkReadZip.TabIndex = 4;
            this.chkReadZip.Text = "Read inside .zip files";
            this.chkReadZip.UseVisualStyleBackColor = true;
            // 
            // chkMultithread
            // 
            this.chkMultithread.AutoSize = true;
            this.chkMultithread.Checked = true;
            this.chkMultithread.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMultithread.Location = new System.Drawing.Point(19, 206);
            this.chkMultithread.Name = "chkMultithread";
            this.chkMultithread.Size = new System.Drawing.Size(114, 17);
            this.chkMultithread.TabIndex = 5;
            this.chkMultithread.Text = "Use Multithreading";
            this.chkMultithread.UseVisualStyleBackColor = true;
            // 
            // chkReZip
            // 
            this.chkReZip.AutoSize = true;
            this.chkReZip.Enabled = false;
            this.chkReZip.Location = new System.Drawing.Point(19, 229);
            this.chkReZip.Name = "chkReZip";
            this.chkReZip.Size = new System.Drawing.Size(128, 17);
            this.chkReZip.TabIndex = 6;
            this.chkReZip.Text = "Zip files at destination";
            this.chkReZip.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(107, 66);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(76, 76);
            this.button3.TabIndex = 7;
            this.button3.Text = "Report!";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(195, 356);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.chkReZip);
            this.Controls.Add(this.chkMultithread);
            this.Controls.Add(this.chkReadZip);
            this.Controls.Add(this.chkUnzip);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "ROMSorter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkUnzip;
        private System.Windows.Forms.CheckBox chkReadZip;
        private System.Windows.Forms.CheckBox chkMultithread;
        private System.Windows.Forms.CheckBox chkReZip;
        private System.Windows.Forms.Button button3;
    }
}

