﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using NinjaScan;

namespace NinjaScan_GUI
{
    public partial class Form1 : Form
    {
        // debug用
        static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        SerialPort myPort;
        Stream st;
        StreamWriter csv_A = new StreamWriter(Stream.Null);
        StreamWriter csv_P = new StreamWriter(Stream.Null);
        StreamWriter csv_M = new StreamWriter(Stream.Null);
        StreamWriter csv_G = new StreamWriter(Stream.Null);
        FileStream ubx_G = new FileStream("garbage.bin", FileMode.Append, FileAccess.Write);

        public static AHRS.MadgwickAHRS AHRS = new AHRS.MadgwickAHRS(1f / 100f, 0.1f);


        public static double gpstime;
        public static double ax, ay, az;
        public static double gx, gy, gz;
        public static double drift_gx, drift_gy, drift_gz;
        public static double mx, my, mz;
        public static double press, temp;
        public static double latitude, longitude, altitude;

                
        public Form1()
        {
            InitializeComponent();

            comboBoxCOM.Items.AddRange(SerialPort.GetPortNames());

        }

        private void butonGyro_Click(object sender, EventArgs e)
        {
            GyroPlot gp = new GyroPlot();
            gp.Show();
        }

        private void buttonAcc_Click(object sender, EventArgs e)
        {
            AccPlot ap = new AccPlot();
            ap.Show();
        }

        private void buttonMag_Click(object sender, EventArgs e)
        {
            MagPlot mp = new MagPlot();
            mp.Show();
        }

        private void buttonPress_Click(object sender, EventArgs e)
        {
            PressPlot pp = new PressPlot();
            pp.Show();
        }

        private void buttonAtti_Click(object sender, EventArgs e)
        {
            AttiPlot atp = new AttiPlot();
            atp.Show();
        }

        private void buttonMap_Click(object sender, EventArgs e)
        {
            GoogleEarth ge = new GoogleEarth();
            ge.Show();
        }

        private void button3DCube_Click(object sender, EventArgs e)
        {
            using(Cube3D c3d = new Cube3D())
            {
                c3d.Run(30.0);
            }
        }


        private void comboBoxCOM_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonCOMlist_Click(object sender, EventArgs e)
        {
            comboBoxCOM.SelectedItem = null;
            comboBoxCOM.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            comboBoxCOM.Items.AddRange(ports);
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (buttonConnect.Text == "Connect")
            {
                try
                {
                    string PortName = comboBoxCOM.SelectedItem.ToString();
                    myPort = new SerialPort(PortName, 115200, Parity.None, 8, StopBits.One);
                    myPort.Open();

                    st = myPort.BaseStream;
                    BufferedStream st_buffer = new BufferedStream(st);
                    BinaryReader br = new BinaryReader(st_buffer);

                    comboBoxCOM.Enabled = false;
                    buttonCOMlist.Enabled = false;
                    buttonConnect.Text = "Disconnect";

                    // バックグラウンド処理
                    var task = Task.Factory.StartNew(() =>
                        {
                            Consol_Output(br);
                        });
                }
                catch (IOException)
                {
                    MessageBox.Show("The port can NOT bet opened.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (System.UnauthorizedAccessException)
                {
                    MessageBox.Show("The port is already opened other application.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("you must select COM port", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception except)
                { Console.WriteLine(except.ToString()); }
            }
            else
            {
                try
                {
                    myPort.Close();
                }
                catch { }
                comboBoxCOM.Enabled = true;
                buttonCOMlist.Enabled = true;
                buttonConnect.Text = "Connect";
            }
        }

        private void Consol_Output(BinaryReader br)
        {
            try
            {
                do
                {
                    // Sylphideプロトコルのheaderで頭出ししないと安定しない。log.datのときは必要ない
                    if (br.ReadByte() == Sylphide_Protocol.header0 && br.ReadByte() == Sylphide_Protocol.header1)
                    {
                        br.ReadBytes(2); // sequence number, not necessary
                        byte head = br.ReadByte();
                        if (head == A_Page.header)
                        {
                            A_Page.Read(br);
                            Console.Write("[A page]:");
                            Console.Write(A_Page.ax + "," + A_Page.ay + "," + A_Page.az + "," +
                                A_Page.gx + "," + A_Page.gy + "," + A_Page.gz + "\n");

                            gpstime = A_Page.gps_time;
                            ax = A_Page.cal_ax;
                            ay = A_Page.cal_ay;
                            az = A_Page.cal_az;
                            gx = A_Page.cal_gx;
                            gy = A_Page.cal_gy;
                            gz = A_Page.cal_gz;
                            A_Page.drift_gx = drift_gx;
                            A_Page.drift_gy = drift_gy;
                            A_Page.drift_gz = drift_gz;
                            AHRS.Update((float)gx / 180 * (float)Math.PI, (float)(gy / 180 * Math.PI), (float)(gz / 180 * Math.PI), (float)ax, (float)ay, (float)az);
                            //AHRS.Update((float)gx, (float)gy, (float)gz, (float)ax, (float)ay, (float)az, (float)mx, (float)my, (float)mz);
                            AHRS.Quaternion2Euler(AHRS.Quaternion);

                        }
                        else if (head == P_Page.header)
                        {
                            P_Page.Read(br);
                            Console.Write("[P page]:");
                            Console.Write(P_Page.pressure * 0.01 + "(hPa)\n");

                            press = P_Page.pressure;
                            temp = P_Page.temperature;
                        }
                        else if (head == M_Page.header)
                        {
                            M_Page.Read(br);
                            Console.Write("[M page]:");
                            double conv = 0.1;
                            Console.Write(M_Page.mx * conv + "," + M_Page.my * conv + "," + M_Page.mz * conv + "\n");

                            mx = M_Page.cal_mx;
                            my = M_Page.cal_my;
                            mz = M_Page.cal_mz;
                        }
                        else if (head == G_Page.header)
                        {
                            G_Page.Read(br);
                            Console.Write("[G page]:");
                            Console.Write(G_Page.ubx + "\n");
                            G_Page.SeekHead(G_Page.analysisObject);
                        }

                        // USBdumpボタンがONならファイル書き込みを行う。
                        if (buttonUSBStop.Enabled == true)
                        {
                            writeFrombinTocsv(head, csv_A, csv_M, csv_P, csv_G, ubx_G);
                        }
                    }

                } while (true);
            }
            catch ( Exception e)
            { Console.WriteLine(e.ToString());}
            finally
            {
                csv_A.Close();
                csv_M.Close();
                csv_P.Close();
                csv_G.Close();
                ubx_G.Close();
            }
        }

        private void writeFrombinTocsv(byte head, StreamWriter asw, StreamWriter msw, StreamWriter psw, StreamWriter gsw, FileStream gfs)
        {
            try
            {
                if (head == A_Page.header)
                {
                    asw.WriteLine(gpstime.ToString() + ","
                        + ax.ToString() + "," + ay.ToString() + "," + az.ToString() + ","
                        + gx.ToString() + "," + gy.ToString() + "," + gz.ToString());
                }
                else if (head == M_Page.header)
                {
                    msw.WriteLine(gpstime.ToString() + ","
                        + mx.ToString() + "," + my.ToString() + "," + mz.ToString());
                }
                else if (head == P_Page.header)
                {
                    psw.WriteLine(gpstime.ToString() + ","
                        + press.ToString() + "," + temp.ToString());
                }
                else if (head == G_Page.header)
                {
                    gfs.Write(G_Page.ubx, 0, G_Page.ubx.Length);
                    if (G_Page.isOutput == true)
                    {
                        gsw.WriteLine(UBX.NMEA_GPGGA);
                        gsw.WriteLine(UBX.NMEA_GPZDA);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// LOG.DATをCSVファイルに変換するためのLOG.DATファイルを選択させる
        /// 変換先のフォルダが既に選択済みならConvertボタンを有効にする。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSDinput_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "LOG.DAT";
            //ofd.InitialDirectory = @"Environment.SpecialFolder.Desktop";
            ofd.Filter = "Sylphide binary file|LOG.DAT";
            ofd.FilterIndex = 1;
            ofd.Title = "Select LOG.DAT";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = ofd.FileName;
                if (File.Exists(textBox3.Text) && Directory.Exists(textBox1.Text))
                {
                    buttonSDConvert.Enabled = true;
                }
            }
        }

        /// <summary>
        /// LOG.DATのCSV変換ファイルの保存先のフォルダ選択ダイアログを出す
        /// 変換するLOG.DATが選択されていればCovertボタンを有効にする。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSDBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the folder that saved CSV log files by SD card.";
            fbd.RootFolder = Environment.SpecialFolder.Desktop; // default: Desktop folder
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                textBox1.Text = fbd.SelectedPath;
                if (File.Exists(textBox3.Text) && Directory.Exists(textBox1.Text))
                {
                    buttonSDConvert.Enabled = true;
                }
            }

        }

        /// <summary>
        /// USBをdumpする保存先のフォルダ選択ダイアログを出す。
        /// 保存先が選択されていれば、Startボタンを有効にする。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUSBBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the folder that saved CSV log files by USB dump.";
            fbd.RootFolder = Environment.SpecialFolder.Desktop; // default: Desktop folder
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                textBox2.Text = fbd.SelectedPath;
                buttonUSBStart.Enabled = true;
            }

        }

        private void buttonSDConvert_Click(object sender, EventArgs e)
        {
            string input_file = textBox3.Text;
            string output_folder = textBox1.Text;
            string file_name = textBox4.Text;
            DialogResult result_overwrite;

            string A_file = output_folder + "\\" + file_name + "_A.csv";
            string P_file = output_folder + "\\" + file_name + "_P.csv";
            string M_file = output_folder + "\\" + file_name + "_M.csv";
            string G_file = output_folder + "\\" + file_name + "_G.nmea";
            string UBX_file = output_folder + "\\" + file_name + ".ubx";

            // ファイルが既に存在していたら警告を出す。キャンセルボタンを押すとメソッドを抜ける
            if (File.Exists(A_file) || File.Exists(P_file) ||
                File.Exists(M_file) || File.Exists(G_file) || File.Exists(UBX_file))
            {
                result_overwrite = MessageBox.Show("Are you sure you want to overwrite the files?",
                    "Overwrite save",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Exclamation);

                if (result_overwrite == DialogResult.Cancel)
                {
                    MessageBox.Show("Abort overwrite save", "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
            }

            StreamWriter csv_A = new StreamWriter(A_file, false);
            StreamWriter csv_P = new StreamWriter(P_file, false);
            StreamWriter csv_M = new StreamWriter(M_file, false);
            StreamWriter csv_G = new StreamWriter(G_file, false);
            BinaryWriter ubx_G = new BinaryWriter(File.OpenWrite(UBX_file));

            csv_A.WriteLine("#gpstime,acc_xa(g),acc_y(g),acc_z(g),gyro_x(dps),gyro_y(dps),gyro_z(dps)");
            csv_M.WriteLine("#gpstime,mag_x(uT),mag_y(uT),mag_z(uT)");
            csv_P.WriteLine("#gpstime,press(Pa),temp(deg)");

            buttonSDConvert.Enabled = false;

            using (BinaryReader br = new BinaryReader(File.OpenRead(input_file)))
            {
                try
                {
                    do
                    {
                        // LOG.DATの場合はProtocolのheaderの頭出しは必要ない
                        byte head = br.ReadByte();
                        if (head == A_Page.header)
                        {
                            A_Page.Read(br);
                            //csv_A.WriteLine(A_Page..gps_time + "," +
                            //    A_Page..ax + "," + A_Page..ay + "," + A_Page..az + "," +
                            //    A_Page..gx + "," + A_Page..gy + "," + A_Page.gz);
                            csv_A.WriteLine(A_Page.gps_time + "," +
                                A_Page.cal_ax + "," + A_Page.cal_ay + "," + A_Page.cal_az + "," + 
                                A_Page.cal_gx + "," + A_Page.cal_gy + "," + A_Page.cal_gz);
                        }
                        else if (head == P_Page.header)
                        {
                            P_Page.Read(br);
                            //csv_P.WriteLine(P_Page.gps_time + "," +
                            //    P_Page.pressure + "," + P_Page.temperature);
                            csv_P.WriteLine(P_Page.gps_time + "," +
                                P_Page.pressure + "," + (double)P_Page.temperature * 0.01);
                        }
                        else if (head == M_Page.header)
                        {
                            M_Page.Read(br);
                            //csv_M.WriteLine(M_Page.gps_time + "," +
                            //    M_Page.mx + "," + M_Page.my + "," + M_Page.mz);
                            csv_M.WriteLine(M_Page.gps_time + "," +
                                M_Page.cal_mx + "," + M_Page.cal_my + "," + M_Page.cal_mz);
                        }
                        else if (head == G_Page.header)
                        {
                            G_Page.Read(br);
                            ubx_G.Write(G_Page.ubx); // GPS pageだけubx形式
                            G_Page.SeekHead(G_Page.analysisObject);
                            if (G_Page.isOutput == true)
                            {
                                csv_G.WriteLine(UBX.NMEA_GPGGA);
                                if (UBX.time.Second == 0 && UBX.time.Millisecond == 0)
                                {
                                    csv_G.WriteLine(UBX.NMEA_GPZDA);
                                }
                            }
                        }
                    } while (true);
                }
                catch
                { }
                finally
                {
                    csv_A.Close();
                    csv_P.Close();
                    csv_M.Close();
                    csv_G.Close();
                    ubx_G.Close();
                }
            }

            buttonSDConvert.Enabled = true;
            DialogResult result = MessageBox.Show("Complete to output CSV files");
        }

        private void buttonUSBStart_Click(object sender, EventArgs e)
        {
            string output_folder = textBox2.Text;
            string file_name = textBox5.Text;
            DialogResult result_overwrite;

            string A_file = output_folder + "\\" + file_name + "_A.csv";
            string P_file = output_folder + "\\" + file_name + "_P.csv";
            string M_file = output_folder + "\\" + file_name + "_M.csv";
            string G_file = output_folder + "\\" + file_name + "_G.nmea";
            string UBX_file = output_folder + "\\" + file_name + ".ubx";

            // ファイルが既に存在していたら警告を出す
            if (File.Exists(A_file) || File.Exists(P_file) ||
                File.Exists(M_file) || File.Exists(G_file) || File.Exists(UBX_file))
            {
                result_overwrite = MessageBox.Show("Are you sure you want to overwrite the files?",
                    "Overwrite save",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Exclamation);

                if (result_overwrite == DialogResult.Cancel)
                {
                    MessageBox.Show("Abort overwrite save", "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
            }

            // COMポートが接続されていなかったらabort
            if (myPort == null || myPort.IsOpen == false)
            {
                MessageBox.Show("COM Port is NOT open.", "error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                return;
            }

            if (buttonConnect.Text == "Disconnect")
            {
                buttonUSBStart.Enabled = false;
                buttonUSBStop.Enabled = true;
                textBox5.Enabled = false;
                buttonUSBBrowse.Enabled = false;

                File.WriteAllText(A_file, "#gpstime,acc_x,acc_y,acc_z,gyro_x,gyro_y,gyro_z\n");
                File.WriteAllText(M_file, "#gpstime,mag_x,mag_y,mag_z\n");
                File.WriteAllText(P_file, "#gpstime,press,temp\n");

                csv_A = new StreamWriter(A_file, true);
                csv_M = new StreamWriter(M_file, true);
                csv_P = new StreamWriter(P_file, true);
                csv_G = new StreamWriter(G_file, true);
                ubx_G = new FileStream(UBX_file, FileMode.Append, FileAccess.Write);
            }
        }

        private void buttonUSBStop_Click(object sender, EventArgs e)
        {
            textBox5.Enabled = true;
            buttonUSBBrowse.Enabled = true;
            buttonUSBStart.Enabled = true;
            buttonUSBStop.Enabled = false;

            csv_A.Close();
            csv_M.Close();
            csv_P.Close();
            csv_G.Close();
            ubx_G.Close();
        }





    }
}