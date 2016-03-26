using System;
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
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using NinjaScan;

namespace NinjaScan_GUI
{
    public partial class Form1 : Form
    {
        // debug用
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        CancellationTokenSource disconnectTokenSource = null;
        CancellationTokenSource closeLogTokenSource = null;

        StreamWriter csv_A = new StreamWriter(Stream.Null);
        StreamWriter csv_P = new StreamWriter(Stream.Null);
        StreamWriter csv_M = new StreamWriter(Stream.Null);
        StreamWriter csv_G = new StreamWriter(Stream.Null);
        FileStream ubx_G = new FileStream("garbage.bin", FileMode.Append, FileAccess.Write);

        public AHRS.MadgwickAHRS AHRS = new AHRS.MadgwickAHRS(1f / 100f, 0.1f);
        
        public const uint defaultTcpPort = 28080;

        private void updatePortList(){
            comboBoxCOM.Items.Clear();
            string[] serial_ports = SerialPort.GetPortNames();
            string[] ports = new string[serial_ports.Length + 1];
            Array.Copy(serial_ports, ports, serial_ports.Length);
            ports[ports.Length - 1] = string.Format("TCP({0})", defaultTcpPort);
            comboBoxCOM.Items.AddRange(ports);
        }

        public Form1()
        {
            InitializeComponent();
            updatePortList();
        }

        private void butonGyro_Click(object sender, EventArgs e)
        {
            GyroPlot gp = new GyroPlot(this);
            gp.Show();
        }

        private void buttonAcc_Click(object sender, EventArgs e)
        {
            AccPlot ap = new AccPlot(this);
            ap.Show();
        }

        private void buttonMag_Click(object sender, EventArgs e)
        {
            MagPlot mp = new MagPlot(this);
            mp.Show();
        }

        private void buttonPress_Click(object sender, EventArgs e)
        {
            PressPlot pp = new PressPlot(this);
            pp.Show();
        }

        private void buttonAtti_Click(object sender, EventArgs e)
        {
            AttiPlot atp = new AttiPlot(this);
            atp.Show();
        }

        private void buttonMap_Click(object sender, EventArgs e)
        {
            GoogleEarth ge = new GoogleEarth(this);
            ge.Show();
        }

        private void button3DCube_Click(object sender, EventArgs e)
        {
            using(Cube3D c3d = new Cube3D(this))
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
            updatePortList();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (disconnectTokenSource == null)
            {
                try
                {
                    string PortName = comboBoxCOM.Text;
                    //MessageBox.Show(PortName);
                    Match m = Regex.Match(PortName, @"TCP\((?:([^:]+):)?(\d+)\)");

                    var tokenSource = new CancellationTokenSource();
                    var token = tokenSource.Token;
                    Task task = null;

                    if (m.Success) // TCP
                    {
                        IPAddress addr = IPAddress.Any;

                        if (m.Groups[1].Value != "")
                        {
                            try
                            {
                                addr = IPAddress.Parse(m.Groups[1].Value);
                            }
                            catch(FormatException)
                            {
                                addr = Dns.GetHostEntry(m.Groups[1].Value).AddressList[0];
                            }
                        }

                        IPEndPoint endPoint = new IPEndPoint(addr, int.Parse(m.Groups[2].Value));
                        TcpListener listener = new TcpListener(endPoint);

                        MessageBox.Show(string.Format("Server({0}:{1}) ready.", 
                            endPoint.Address.ToString(), endPoint.Port.ToString()));
                        
                        task = Task.Factory.StartNew(() =>
                        {
                            listener.Start();

                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    int clients = 0;
                                    while (true)
                                    {
                                    // ソケット接続待ち
                                    
                                        TcpClient client_new = listener.AcceptTcpClient();
                                        if (++clients > 1) // 最大接続数チェック(1)
                                        {
                                            client_new.Close();
                                            continue;
                                        }

                                        Task.Factory.StartNew(() =>
                                        {
                                            TcpClient client = client_new;
                                            try
                                            {
                                                readPacket(new BufferedStream(client.GetStream()), token);
                                            }
                                            finally
                                            {
                                                if (client.Connected) client.Close();
                                                clients--;
                                            }
                                        });
                                    }
                                }
                                catch (SocketException) { }
                            });

                            while (true) token.ThrowIfCancellationRequested();                         
                        }, token).ContinueWith(t =>
                        {
                            listener.Stop();
                        });
                    }
                    else
                    { // COM
                        SerialPort port = new SerialPort(PortName, 115200, Parity.None, 8, StopBits.One);
                        port.Open();

                        // バックグラウンド処理
                        task = Task.Factory.StartNew(() =>
                        {
                            readPacket(new BufferedStream(port.BaseStream), token);
                        }, token).ContinueWith(t =>
                        {
                            port.Close();
                        });
                    }

                    task.ContinueWith(t =>
                    {
                        disconnectTokenSource.Dispose();
                        disconnectTokenSource = null;

                        Invoke((MethodInvoker)(() => 
                        {
                            comboBoxCOM.Enabled = true;
                            buttonCOMlist.Enabled = true;
                            buttonConnect.Text = "Connect";
                        }));
                    });

                    comboBoxCOM.Enabled = false;
                    buttonCOMlist.Enabled = false;
                    buttonConnect.Text = "Disconnect";

                    disconnectTokenSource = tokenSource;
                }
                catch (IOException)
                {
                    MessageBox.Show("The port can NOT bet opened.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (System.UnauthorizedAccessException)
                {
                    MessageBox.Show("The port is already opened other application.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("you must select appropriate port", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                { 
                    Console.WriteLine(ex.ToString()); 
                }
            }
            else
            {
                disconnectTokenSource.Cancel(true);
            }
        }

        public class SylphidePages
        {
            public A_Page a = new A_Page();
            public P_Page p = new P_Page();
            public M_Page m = new M_Page();
            public G_Page g = new G_Page();
        }
        public SylphidePages pages = new SylphidePages();

        private void readPacket(Stream st, CancellationToken token)
        {
            byte[] buf = new byte[0x40];
            int offset = 0, buf_length = 0;

            MemoryStream st_page = new MemoryStream(buf, 4, 32, false);
            BinaryReader reader = new BinaryReader(st_page);

            const int packet_length = 38;

            bool read_next = true;
            while (read_next)
            {
                bool reading = true;
                
                if (offset > 0)
                {
                    Array.Copy(buf, offset, buf, 0, buf_length -= offset);
                }

                st.BeginRead(buf, buf_length, packet_length - buf_length, result =>
                {
                    try
                    {
                        int additional = st.EndRead(result);
                        if (additional == 0) { read_next = false; }
                        buf_length += additional;
                    }
                    catch (Exception ex)
                    {
                        if(ex is IOException || ex is ObjectDisposedException)
                        {
                            read_next = false;
                        }
                        else
                        {
                            throw;
                        }
                    }
                    reading = false;
                }, null);

                while (reading) token.ThrowIfCancellationRequested();
                
                if (buf_length < packet_length) { continue; }
                
                offset = 0;
                if (buf[0] != Sylphide_Protocol.header0)
                {
                    offset = 1;
                    continue;
                }
                if (buf[1] != Sylphide_Protocol.header1)
                {
                    offset = 2;
                    continue;
                }

                // buf[2..3] sequence number, not necessary
                // buf[36..37] cheksum, not necessary

                st_page.Position = 0;
                readOnePage(reader); // update by using page information

                buf_length = 0;
            }
        }

        private void readOnePage(BinaryReader br)
        {            
            byte head = br.ReadByte();
            if (head == A_Page.header)
            {
                pages.a.Update(br);
                Console.Write("[A page]:");
                Console.Write(
                    pages.a.ax + "," + pages.a.ay + "," + pages.a.az + "," +
                    pages.a.gx + "," + pages.a.gy + "," + pages.a.gz + "\n");
                
                AHRS.Update(
                    (float)pages.a.cal_gx / 180 * (float)Math.PI,
                    (float)pages.a.cal_gy / 180 * (float)Math.PI,
                    (float)pages.a.cal_gz / 180 * (float)Math.PI,
                    (float)pages.a.cal_ax, (float)pages.a.cal_ay, (float)pages.a.cal_az);
                //AHRS.Update((float)gx, (float)gy, (float)gz, (float)ax, (float)ay, (float)az, (float)mx, (float)my, (float)mz);
                AHRS.Quaternion2Euler(AHRS.Quaternion);
            }
            else if (head == P_Page.header)
            {
                pages.p.Update(br);
                Console.Write("[P page]:");
                Console.Write(pages.p.pressure * 0.01 + "(hPa)\n");
            }
            else if (head == M_Page.header)
            {
                pages.m.Update(br);
                Console.Write("[M page]:");
                double conv = 0.1;
                Console.Write(pages.m.mx * conv + "," + pages.m.my * conv + "," + pages.m.mz * conv + "\n");
            }
            else if (head == G_Page.header)
            {
                pages.g.Update(br);
                Console.Write("[G page]:");
                Console.Write(pages.g.ubx_raw + "\n");
            }

            // ファイルが開いているなら書き込みを行う。
            if (closeLogTokenSource != null)
            {
                writeFrombinTocsv(head, csv_A, csv_M, csv_P, csv_G, ubx_G);
            }
        }

        private void writeFrombinTocsv(byte head, StreamWriter asw, StreamWriter msw, StreamWriter psw, StreamWriter gsw, FileStream gfs)
        {
            if (head == A_Page.header)
            {
                asw.WriteLine(pages.a.gps_time.ToString() + ","
                    + pages.a.cal_ax.ToString() + "," + pages.a.cal_ay.ToString() + "," + pages.a.cal_az.ToString() + ","
                    + pages.a.cal_gx.ToString() + "," + pages.a.cal_gy.ToString() + "," + pages.a.cal_gz.ToString());
            }
            else if (head == M_Page.header)
            {
                msw.WriteLine(pages.m.gps_time.ToString() + ","
                    + pages.m.cal_mx.ToString() + "," + pages.m.cal_my.ToString() + "," + pages.m.cal_mz.ToString());
            }
            else if (head == P_Page.header)
            {
                psw.WriteLine(pages.p.gps_time.ToString() + ","
                    + pages.p.pressure.ToString() + "," + pages.p.temperature.ToString());
            }
            else if (head == G_Page.header)
            {
                gfs.Write(pages.g.ubx_raw, 0, pages.g.ubx_raw.Length);
                if (pages.g.hasOutput == true)
                {
                    gsw.WriteLine(pages.g.ubx.nmea.gpgga);
                    gsw.WriteLine(pages.g.ubx.nmea.gpzda);
                }
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
            ofd.Filter = "Sylphide binary file|LOG.DAT|All files (*.*)|*.*";
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

            string A_file = output_folder + "\\" + file_name + "_A.csv";
            string P_file = output_folder + "\\" + file_name + "_P.csv";
            string M_file = output_folder + "\\" + file_name + "_M.csv";
            string G_file = output_folder + "\\" + file_name + "_G.nmea";
            string UBX_file = output_folder + "\\" + file_name + ".ubx";

            // ファイルが既に存在していたら警告を出す。キャンセルボタンを押すとメソッドを抜ける
            if (File.Exists(A_file) || File.Exists(P_file) ||
                File.Exists(M_file) || File.Exists(G_file) || File.Exists(UBX_file))
            {
                DialogResult res = MessageBox.Show("Are you sure you want to overwrite the files?",
                    "Overwrite save",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Exclamation);

                if (res == DialogResult.Cancel)
                {
                    MessageBox.Show("Abort overwrite save", "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
            }

            StreamWriter csv_A_sd = new StreamWriter(A_file, false);
            StreamWriter csv_P_sd = new StreamWriter(P_file, false);
            StreamWriter csv_M_sd = new StreamWriter(M_file, false);
            StreamWriter csv_G_sd = new StreamWriter(G_file, false);
            BinaryWriter ubx_G_sd = new BinaryWriter(File.OpenWrite(UBX_file));

            csv_A_sd.WriteLine("#gpstime,acc_xa(g),acc_y(g),acc_z(g),gyro_x(dps),gyro_y(dps),gyro_z(dps)");
            csv_M_sd.WriteLine("#gpstime,mag_x(uT),mag_y(uT),mag_z(uT)");
            csv_P_sd.WriteLine("#gpstime,press(Pa),temp(deg)");

            Task.Factory.StartNew(() =>
            {
                BinaryReader br = new BinaryReader(File.OpenRead(input_file));

                SylphidePages pages_sd = new SylphidePages();

                do
                {
                    // LOG.DATの場合はProtocolのheaderの頭出しは必要ない
                    byte head = br.ReadByte();
                    if (head == A_Page.header)
                    {
                        pages_sd.a.Update(br);
                        csv_A_sd.WriteLine(pages_sd.a.gps_time + "," +
                            pages_sd.a.cal_ax + "," + pages_sd.a.cal_ay + "," + pages_sd.a.cal_az + "," +
                            pages_sd.a.cal_gx + "," + pages_sd.a.cal_gy + "," + pages_sd.a.cal_gz);
                    }
                    else if (head == P_Page.header)
                    {
                        pages_sd.p.Update(br);
                        csv_P_sd.WriteLine(pages_sd.p.gps_time + "," +
                            pages_sd.p.pressure + "," + (double)pages_sd.p.temperature * 0.01);
                    }
                    else if (head == M_Page.header)
                    {
                        pages_sd.m.Update(br);
                        csv_M_sd.WriteLine(pages_sd.m.gps_time + "," +
                            pages_sd.m.cal_mx + "," + pages_sd.m.cal_my + "," + pages_sd.m.cal_mz);
                    }
                    else if (head == G_Page.header)
                    {
                        pages_sd.g.Update(br);
                        ubx_G_sd.Write(pages_sd.g.ubx_raw); // GPS pageだけubx形式
                        if (pages_sd.g.hasOutput == true)
                        {
                            csv_G_sd.WriteLine(pages_sd.g.ubx.nmea.gpgga);
                            if (pages_sd.g.ubx.nmea.time.Second == 0 && pages_sd.g.ubx.nmea.time.Millisecond == 0)
                            {
                                csv_G_sd.WriteLine(pages_sd.g.ubx.nmea.gpzda);
                            }
                        }
                    }
                } while (true);
            }).ContinueWith(t =>
            {
                csv_A_sd.Close();
                csv_P_sd.Close();
                csv_M_sd.Close();
                csv_G_sd.Close();
                ubx_G_sd.Close();

                DialogResult result = MessageBox.Show("Complete to output CSV files");

                Invoke((MethodInvoker)(() =>
                {
                    buttonSDConvert.Text = "Convert";
                    buttonSDConvert.Enabled = true;
                }));
            });

            buttonSDConvert.Enabled = false;
            buttonSDConvert.Text = "Converting...";
        }

        private void buttonUSBStart_Click(object sender, EventArgs e)
        {
            string output_folder = textBox2.Text;
            string file_name = textBox5.Text;

            string A_file = output_folder + "\\" + file_name + "_A.csv";
            string P_file = output_folder + "\\" + file_name + "_P.csv";
            string M_file = output_folder + "\\" + file_name + "_M.csv";
            string G_file = output_folder + "\\" + file_name + "_G.nmea";
            string UBX_file = output_folder + "\\" + file_name + ".ubx";

            // ファイルが既に存在していたら警告を出す
            if (File.Exists(A_file) || File.Exists(P_file) ||
                File.Exists(M_file) || File.Exists(G_file) || File.Exists(UBX_file))
            {
                DialogResult res = MessageBox.Show("Are you sure you want to overwrite the files?",
                    "Overwrite save",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Exclamation);

                if (res == DialogResult.Cancel)
                {
                    MessageBox.Show("Abort overwrite save", "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
            }

            // COMポートが接続されていなかったら警告を出す
            if (disconnectTokenSource == null)
            {
                DialogResult res = MessageBox.Show("Port has NOT been opened yet. Continue?", "Port check",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Exclamation);
                if (res == DialogResult.Cancel)
                {
                    MessageBox.Show("Aborted", "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
            }

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Task task = null;

            // バックグラウンド処理
            task = Task.Factory.StartNew(() =>
            {
                File.WriteAllText(A_file, "#gpstime,acc_x,acc_y,acc_z,gyro_x,gyro_y,gyro_z\n");
                File.WriteAllText(M_file, "#gpstime,mag_x,mag_y,mag_z\n");
                File.WriteAllText(P_file, "#gpstime,press,temp\n");

                csv_A = new StreamWriter(A_file, true);
                csv_M = new StreamWriter(M_file, true);
                csv_P = new StreamWriter(P_file, true);
                csv_G = new StreamWriter(G_file, true);
                ubx_G = new FileStream(UBX_file, FileMode.Append, FileAccess.Write);

                while (true)
                {
                    token.ThrowIfCancellationRequested();
                }
            }, token).ContinueWith(t =>
            {
                csv_A.Close();
                csv_M.Close();
                csv_P.Close();
                csv_G.Close();
                ubx_G.Close();
            }).ContinueWith(t =>
            {
                closeLogTokenSource.Dispose();
                closeLogTokenSource = null;

                Invoke((MethodInvoker)(() =>
                {
                    buttonUSBBrowse.Enabled = true;
                    buttonUSBStart.Enabled = true;
                    textBox5.Enabled = true;
                    buttonUSBStop.Enabled = false;
                }));
            });

            buttonUSBStart.Enabled = false;
            buttonUSBStop.Enabled = true;
            textBox5.Enabled = false;
            buttonUSBBrowse.Enabled = false;

            closeLogTokenSource = tokenSource;
        }

        private void buttonUSBStop_Click(object sender, EventArgs e)
        {
            closeLogTokenSource.Cancel(true);
        }
    }
}
