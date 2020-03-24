namespace RomSorter
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
            this.button1 = new System.Windows.Forms.Button();
            this.fbdDats = new System.Windows.Forms.FolderBrowserDialog();
            this.ofdDats = new System.Windows.Forms.OpenFileDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.lblTestStatus = new System.Windows.Forms.Label();
            this.button15 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(39, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(138, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Recreate Database";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // fbdDats
            // 
            this.fbdDats.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            // 
            // ofdDats
            // 
            this.ofdDats.CheckFileExists = false;
            this.ofdDats.FileName = "Folder Selection";
            this.ofdDats.ValidateNames = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(262, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(141, 44);
            this.button2.TabIndex = 1;
            this.button2.Text = "Count Entries";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(445, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(104, 27);
            this.button3.TabIndex = 2;
            this.button3.Text = "Count By Console";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(39, 211);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(110, 29);
            this.button4.TabIndex = 3;
            this.button4.Text = "Load Disc Dats";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(39, 350);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(121, 58);
            this.button5.TabIndex = 4;
            this.button5.Text = "Build Indexes";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(690, 34);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(72, 50);
            this.button6.TabIndex = 5;
            this.button6.Text = "Make Dat From Folder (rescursive)";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(676, 103);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(85, 41);
            this.button7.TabIndex = 6;
            this.button7.Text = "ID games in folder";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(658, 350);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(88, 35);
            this.button8.TabIndex = 7;
            this.button8.Text = "Find collisions";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(43, 308);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(116, 26);
            this.button9.TabIndex = 8;
            this.button9.Text = "Load 1G1R Files";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(624, 246);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(121, 41);
            this.button10.TabIndex = 9;
            this.button10.Text = "Test Full Good Path";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(39, 246);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(120, 48);
            this.button11.TabIndex = 10;
            this.button11.Text = "Load Pinball Dats";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(676, 162);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(72, 50);
            this.button12.TabIndex = 11;
            this.button12.Text = "Make Dat From DB";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(57, 48);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(110, 36);
            this.button13.TabIndex = 12;
            this.button13.Text = "Load Console Dats (Fast)";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(57, 90);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(110, 37);
            this.button14.TabIndex = 13;
            this.button14.Text = "Load Console Dats (High Integrity)";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // lblTestStatus
            // 
            this.lblTestStatus.AutoSize = true;
            this.lblTestStatus.Location = new System.Drawing.Point(279, 99);
            this.lblTestStatus.Name = "lblTestStatus";
            this.lblTestStatus.Size = new System.Drawing.Size(68, 13);
            this.lblTestStatus.TabIndex = 14;
            this.lblTestStatus.Text = "lblTestStatus";
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(57, 145);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(125, 37);
            this.button15.TabIndex = 15;
            this.button15.Text = "Load Single Console Dats (High Integrity)";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(358, 205);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(85, 41);
            this.button16.TabIndex = 16;
            this.button16.Text = "Hash and find single game";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // button17
            // 
            this.button17.Location = new System.Drawing.Point(420, 344);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(85, 41);
            this.button17.TabIndex = 17;
            this.button17.Text = "Hash Test";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.button17_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button17);
            this.Controls.Add(this.button16);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.lblTestStatus);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FolderBrowserDialog fbdDats;
        private System.Windows.Forms.OpenFileDialog ofdDats;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Label lblTestStatus;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button17;
    }
}