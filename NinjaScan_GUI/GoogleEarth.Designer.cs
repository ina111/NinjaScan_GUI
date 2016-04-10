namespace NinjaScan_GUI
{
    partial class GoogleEarth
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelTIME = new System.Windows.Forms.Label();
            this.labelGPSLLH = new System.Windows.Forms.Label();
            this.labelGPSFix = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.webBrowser1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.87013F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.12987F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(647, 497);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelTIME);
            this.panel1.Controls.Add(this.labelGPSLLH);
            this.panel1.Controls.Add(this.labelGPSFix);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 449);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(641, 45);
            this.panel1.TabIndex = 0;
            // 
            // labelTIME
            // 
            this.labelTIME.AutoSize = true;
            this.labelTIME.Location = new System.Drawing.Point(423, 13);
            this.labelTIME.Name = "labelTIME";
            this.labelTIME.Size = new System.Drawing.Size(36, 12);
            this.labelTIME.TabIndex = 3;
            this.labelTIME.Text = "Time: ";
            // 
            // labelGPSLLH
            // 
            this.labelGPSLLH.AutoSize = true;
            this.labelGPSLLH.Location = new System.Drawing.Point(194, 24);
            this.labelGPSLLH.Name = "labelGPSLLH";
            this.labelGPSLLH.Size = new System.Drawing.Size(77, 12);
            this.labelGPSLLH.TabIndex = 2;
            this.labelGPSLLH.Text = "Time of Week:";
            // 
            // labelGPSFix
            // 
            this.labelGPSFix.AutoSize = true;
            this.labelGPSFix.Location = new System.Drawing.Point(194, 8);
            this.labelGPSFix.Name = "labelGPSFix";
            this.labelGPSFix.Size = new System.Drawing.Size(55, 12);
            this.labelGPSFix.TabIndex = 1;
            this.labelGPSFix.Text = "STATUS: ";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(28, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Reload Current Position";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(3, 3);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(641, 440);
            this.webBrowser1.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // GoogleEarth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 497);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "GoogleEarth";
            this.Text = "GPS Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GoogleEarth_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Label labelGPSFix;
        private System.Windows.Forms.Label labelGPSLLH;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label labelTIME;

    }
}