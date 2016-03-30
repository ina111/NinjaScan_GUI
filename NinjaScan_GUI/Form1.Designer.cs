namespace NinjaScan_GUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.comboBoxCOM = new System.Windows.Forms.ComboBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.butonGyro = new System.Windows.Forms.Button();
            this.buttonAcc = new System.Windows.Forms.Button();
            this.buttonMag = new System.Windows.Forms.Button();
            this.buttonPress = new System.Windows.Forms.Button();
            this.button3DCube = new System.Windows.Forms.Button();
            this.buttonAtti = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBoxPlot = new System.Windows.Forms.GroupBox();
            this.buttonMap = new System.Windows.Forms.Button();
            this.groupBoxPort = new System.Windows.Forms.GroupBox();
            this.buttonCOMlist = new System.Windows.Forms.Button();
            this.groupBoxSD = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.buttonSDinput = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSDConvert = new System.Windows.Forms.Button();
            this.buttonSDBrowse = new System.Windows.Forms.Button();
            this.groupBoxUSB = new System.Windows.Forms.GroupBox();
            this.buttonUSBStop = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonUSBStart = new System.Windows.Forms.Button();
            this.buttonUSBBrowse = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBoxPlot.SuspendLayout();
            this.groupBoxPort.SuspendLayout();
            this.groupBoxSD.SuspendLayout();
            this.groupBoxUSB.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxCOM
            // 
            this.comboBoxCOM.FormattingEnabled = true;
            this.comboBoxCOM.Location = new System.Drawing.Point(48, 23);
            this.comboBoxCOM.Name = "comboBoxCOM";
            this.comboBoxCOM.Size = new System.Drawing.Size(169, 20);
            this.comboBoxCOM.TabIndex = 0;
            this.comboBoxCOM.SelectedIndexChanged += new System.EventHandler(this.comboBoxCOM_SelectedIndexChanged);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(329, 21);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(100, 23);
            this.buttonConnect.TabIndex = 1;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // butonGyro
            // 
            this.butonGyro.Location = new System.Drawing.Point(8, 18);
            this.butonGyro.Name = "butonGyro";
            this.butonGyro.Size = new System.Drawing.Size(100, 23);
            this.butonGyro.TabIndex = 3;
            this.butonGyro.Text = "Gyroscope";
            this.butonGyro.UseVisualStyleBackColor = true;
            this.butonGyro.Click += new System.EventHandler(this.butonGyro_Click);
            // 
            // buttonAcc
            // 
            this.buttonAcc.Location = new System.Drawing.Point(117, 18);
            this.buttonAcc.Name = "buttonAcc";
            this.buttonAcc.Size = new System.Drawing.Size(100, 23);
            this.buttonAcc.TabIndex = 4;
            this.buttonAcc.Text = "Accelarator";
            this.buttonAcc.UseVisualStyleBackColor = true;
            this.buttonAcc.Click += new System.EventHandler(this.buttonAcc_Click);
            // 
            // buttonMag
            // 
            this.buttonMag.Location = new System.Drawing.Point(223, 18);
            this.buttonMag.Name = "buttonMag";
            this.buttonMag.Size = new System.Drawing.Size(100, 23);
            this.buttonMag.TabIndex = 5;
            this.buttonMag.Text = "Magnetic";
            this.buttonMag.UseVisualStyleBackColor = true;
            this.buttonMag.Click += new System.EventHandler(this.buttonMag_Click);
            // 
            // buttonPress
            // 
            this.buttonPress.Location = new System.Drawing.Point(329, 18);
            this.buttonPress.Name = "buttonPress";
            this.buttonPress.Size = new System.Drawing.Size(100, 23);
            this.buttonPress.TabIndex = 6;
            this.buttonPress.Text = "Pressure";
            this.buttonPress.UseVisualStyleBackColor = true;
            this.buttonPress.Click += new System.EventHandler(this.buttonPress_Click);
            // 
            // button3DCube
            // 
            this.button3DCube.Location = new System.Drawing.Point(8, 56);
            this.button3DCube.Name = "button3DCube";
            this.button3DCube.Size = new System.Drawing.Size(125, 23);
            this.button3DCube.TabIndex = 7;
            this.button3DCube.Text = "3D Cube";
            this.button3DCube.UseVisualStyleBackColor = true;
            this.button3DCube.Click += new System.EventHandler(this.button3DCube_Click);
            // 
            // buttonAtti
            // 
            this.buttonAtti.Location = new System.Drawing.Point(154, 56);
            this.buttonAtti.Name = "buttonAtti";
            this.buttonAtti.Size = new System.Drawing.Size(125, 23);
            this.buttonAtti.TabIndex = 8;
            this.buttonAtti.Text = "attitude";
            this.buttonAtti.UseVisualStyleBackColor = true;
            this.buttonAtti.Click += new System.EventHandler(this.buttonAtti_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "Port #";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(96, 51);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(282, 19);
            this.textBox1.TabIndex = 10;
            // 
            // groupBoxPlot
            // 
            this.groupBoxPlot.Controls.Add(this.buttonMap);
            this.groupBoxPlot.Controls.Add(this.butonGyro);
            this.groupBoxPlot.Controls.Add(this.buttonAcc);
            this.groupBoxPlot.Controls.Add(this.buttonMag);
            this.groupBoxPlot.Controls.Add(this.buttonPress);
            this.groupBoxPlot.Controls.Add(this.buttonAtti);
            this.groupBoxPlot.Controls.Add(this.button3DCube);
            this.groupBoxPlot.Location = new System.Drawing.Point(12, 84);
            this.groupBoxPlot.Name = "groupBoxPlot";
            this.groupBoxPlot.Size = new System.Drawing.Size(439, 92);
            this.groupBoxPlot.TabIndex = 12;
            this.groupBoxPlot.TabStop = false;
            this.groupBoxPlot.Text = "Plot";
            // 
            // buttonMap
            // 
            this.buttonMap.Location = new System.Drawing.Point(304, 56);
            this.buttonMap.Name = "buttonMap";
            this.buttonMap.Size = new System.Drawing.Size(125, 23);
            this.buttonMap.TabIndex = 9;
            this.buttonMap.Text = "Map";
            this.buttonMap.UseVisualStyleBackColor = true;
            this.buttonMap.Click += new System.EventHandler(this.buttonMap_Click);
            // 
            // groupBoxPort
            // 
            this.groupBoxPort.Controls.Add(this.buttonCOMlist);
            this.groupBoxPort.Controls.Add(this.comboBoxCOM);
            this.groupBoxPort.Controls.Add(this.label2);
            this.groupBoxPort.Controls.Add(this.buttonConnect);
            this.groupBoxPort.Location = new System.Drawing.Point(12, 12);
            this.groupBoxPort.Name = "groupBoxPort";
            this.groupBoxPort.Size = new System.Drawing.Size(439, 66);
            this.groupBoxPort.TabIndex = 13;
            this.groupBoxPort.TabStop = false;
            this.groupBoxPort.Text = "USB Port";
            // 
            // buttonCOMlist
            // 
            this.buttonCOMlist.Location = new System.Drawing.Point(223, 21);
            this.buttonCOMlist.Name = "buttonCOMlist";
            this.buttonCOMlist.Size = new System.Drawing.Size(100, 23);
            this.buttonCOMlist.TabIndex = 10;
            this.buttonCOMlist.Text = "Refresh PortList";
            this.buttonCOMlist.UseVisualStyleBackColor = true;
            this.buttonCOMlist.Click += new System.EventHandler(this.buttonCOMlist_Click);
            // 
            // groupBoxSD
            // 
            this.groupBoxSD.Controls.Add(this.label5);
            this.groupBoxSD.Controls.Add(this.textBox4);
            this.groupBoxSD.Controls.Add(this.buttonSDinput);
            this.groupBoxSD.Controls.Add(this.label4);
            this.groupBoxSD.Controls.Add(this.textBox3);
            this.groupBoxSD.Controls.Add(this.label1);
            this.groupBoxSD.Controls.Add(this.buttonSDConvert);
            this.groupBoxSD.Controls.Add(this.buttonSDBrowse);
            this.groupBoxSD.Controls.Add(this.textBox1);
            this.groupBoxSD.Location = new System.Drawing.Point(12, 182);
            this.groupBoxSD.Name = "groupBoxSD";
            this.groupBoxSD.Size = new System.Drawing.Size(572, 109);
            this.groupBoxSD.TabIndex = 14;
            this.groupBoxSD.TabStop = false;
            this.groupBoxSD.Text = "Log binary data -> CSV format log file";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 12);
            this.label5.TabIndex = 18;
            this.label5.Text = "CSV file name";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(96, 82);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(282, 19);
            this.textBox4.TabIndex = 17;
            this.textBox4.Text = "NinjaScanLite_LOG";
            // 
            // buttonSDinput
            // 
            this.buttonSDinput.Location = new System.Drawing.Point(390, 21);
            this.buttonSDinput.Name = "buttonSDinput";
            this.buttonSDinput.Size = new System.Drawing.Size(75, 23);
            this.buttonSDinput.TabIndex = 16;
            this.buttonSDinput.Text = "Browse...";
            this.buttonSDinput.UseVisualStyleBackColor = true;
            this.buttonSDinput.Click += new System.EventHandler(this.buttonSDinput_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 15;
            this.label4.Text = "Input .bin file";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(96, 23);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(282, 19);
            this.textBox3.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "Output folder";
            // 
            // buttonSDConvert
            // 
            this.buttonSDConvert.Enabled = false;
            this.buttonSDConvert.Location = new System.Drawing.Point(390, 78);
            this.buttonSDConvert.Name = "buttonSDConvert";
            this.buttonSDConvert.Size = new System.Drawing.Size(105, 23);
            this.buttonSDConvert.TabIndex = 12;
            this.buttonSDConvert.Text = "Convert";
            this.buttonSDConvert.UseVisualStyleBackColor = true;
            this.buttonSDConvert.Click += new System.EventHandler(this.buttonSDConvert_Click);
            // 
            // buttonSDBrowse
            // 
            this.buttonSDBrowse.Location = new System.Drawing.Point(390, 49);
            this.buttonSDBrowse.Name = "buttonSDBrowse";
            this.buttonSDBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonSDBrowse.TabIndex = 11;
            this.buttonSDBrowse.Text = "Browse...";
            this.buttonSDBrowse.UseVisualStyleBackColor = true;
            this.buttonSDBrowse.Click += new System.EventHandler(this.buttonSDBrowse_Click);
            // 
            // groupBoxUSB
            // 
            this.groupBoxUSB.Controls.Add(this.buttonUSBStop);
            this.groupBoxUSB.Controls.Add(this.label6);
            this.groupBoxUSB.Controls.Add(this.textBox5);
            this.groupBoxUSB.Controls.Add(this.label3);
            this.groupBoxUSB.Controls.Add(this.buttonUSBStart);
            this.groupBoxUSB.Controls.Add(this.buttonUSBBrowse);
            this.groupBoxUSB.Controls.Add(this.textBox2);
            this.groupBoxUSB.Location = new System.Drawing.Point(12, 297);
            this.groupBoxUSB.Name = "groupBoxUSB";
            this.groupBoxUSB.Size = new System.Drawing.Size(572, 93);
            this.groupBoxUSB.TabIndex = 15;
            this.groupBoxUSB.TabStop = false;
            this.groupBoxUSB.Text = "Create USB dump file";
            // 
            // buttonUSBStop
            // 
            this.buttonUSBStop.Enabled = false;
            this.buttonUSBStop.Location = new System.Drawing.Point(471, 52);
            this.buttonUSBStop.Name = "buttonUSBStop";
            this.buttonUSBStop.Size = new System.Drawing.Size(75, 23);
            this.buttonUSBStop.TabIndex = 18;
            this.buttonUSBStop.Text = "Stop";
            this.buttonUSBStop.UseVisualStyleBackColor = true;
            this.buttonUSBStop.Click += new System.EventHandler(this.buttonUSBStop_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 12);
            this.label6.TabIndex = 17;
            this.label6.Text = "CSV file name";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(96, 54);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(282, 19);
            this.textBox5.TabIndex = 16;
            this.textBox5.Text = "NinjaScanLite_USB";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "Output folder";
            // 
            // buttonUSBStart
            // 
            this.buttonUSBStart.Enabled = false;
            this.buttonUSBStart.Location = new System.Drawing.Point(390, 53);
            this.buttonUSBStart.Name = "buttonUSBStart";
            this.buttonUSBStart.Size = new System.Drawing.Size(75, 23);
            this.buttonUSBStart.TabIndex = 2;
            this.buttonUSBStart.Text = "Start";
            this.buttonUSBStart.UseVisualStyleBackColor = true;
            this.buttonUSBStart.Click += new System.EventHandler(this.buttonUSBStart_Click);
            // 
            // buttonUSBBrowse
            // 
            this.buttonUSBBrowse.Location = new System.Drawing.Point(390, 24);
            this.buttonUSBBrowse.Name = "buttonUSBBrowse";
            this.buttonUSBBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonUSBBrowse.TabIndex = 1;
            this.buttonUSBBrowse.Text = "Browse...";
            this.buttonUSBBrowse.UseVisualStyleBackColor = true;
            this.buttonUSBBrowse.Click += new System.EventHandler(this.buttonUSBBrowse_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(96, 26);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(282, 19);
            this.textBox2.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 402);
            this.Controls.Add(this.groupBoxUSB);
            this.Controls.Add(this.groupBoxSD);
            this.Controls.Add(this.groupBoxPort);
            this.Controls.Add(this.groupBoxPlot);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "NinjaScan GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBoxPlot.ResumeLayout(false);
            this.groupBoxPort.ResumeLayout(false);
            this.groupBoxPort.PerformLayout();
            this.groupBoxSD.ResumeLayout(false);
            this.groupBoxSD.PerformLayout();
            this.groupBoxUSB.ResumeLayout(false);
            this.groupBoxUSB.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxCOM;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button butonGyro;
        private System.Windows.Forms.Button buttonAcc;
        private System.Windows.Forms.Button buttonMag;
        private System.Windows.Forms.Button buttonPress;
        private System.Windows.Forms.Button button3DCube;
        private System.Windows.Forms.Button buttonAtti;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBoxPlot;
        private System.Windows.Forms.GroupBox groupBoxPort;
        private System.Windows.Forms.GroupBox groupBoxSD;
        private System.Windows.Forms.Button buttonSDConvert;
        private System.Windows.Forms.Button buttonSDBrowse;
        private System.Windows.Forms.GroupBox groupBoxUSB;
        private System.Windows.Forms.Button buttonUSBStart;
        private System.Windows.Forms.Button buttonUSBBrowse;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button buttonMap;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCOMlist;
        private System.Windows.Forms.Button buttonSDinput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Button buttonUSBStop;
    }
}