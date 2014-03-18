using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace NinjaScan_GUI
{
    public class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void Main2(string[] args)
        {
            string PortName = "COM3"; // 自分のPCのCOMポート
            int BaudRate = 115200; // baudrate is anything OK
            Parity Parity = Parity.None;
            int DataBits = 8;
            StopBits StopBits = StopBits.One;

            SerialPort myPort = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);
            myPort.Open();

            Stream st = myPort.BaseStream;
            BinaryReader br = new BinaryReader(st);

            try
            {
                do
                {
                    byte head = br.ReadByte();
                    if (head == A_Page.header)
                    {
                        A_Page.Read(br);
                        //Console.Write("[A page]:");
                        //Console.Write(A_Page.ax + "," + A_Page.ay + "," + A_Page.az + "," +
                        //    A_Page.gx + "," + A_Page.gy + "," + A_Page.gz + "\n");
                    }
                    else if (head == P_Page.header)
                    {
                        P_Page.Read(br);
                        Console.Write("[P page]:");
                        Console.Write(P_Page.pressure * 0.0001 + "(hPa)\n");
                    }
                    else if (head == M_Page.header)
                    {
                        M_Page.Read(br);
                        Console.Write("[M page]:");
                        double conv = 0.1;
                        Console.Write(M_Page.mx * conv + "," + M_Page.my * conv + "," + M_Page.mz * conv + "\n");
                    }
                    else if (head == G_Page.header)
                    {
                        G_Page.Read(br);
                        //Console.Write("[G page]:");
                        //Console.Write(G_Page.ubx + "\n");
                    }
                } while (true);
            }
            catch
            { }

            myPort.Close();


            /// log.datファイルを読み込む場合はこちら
            string file_path = "c:\\users\\111\\documents\\visual studio 2013\\Projects\\SylphideStream\\SylphideStream\\LOG.DAT";

            //using (BinaryReader br = new BinaryReader(File.OpenRead(file_path)))
            //{
            //    try
            //    {
            //        do
            //        {
            //            byte head = br.ReadByte();
            //            if (head == A_Page.header)
            //            {
            //                A_Page.Read(br);
            //                Console.Write(A_Page.ax + "," + A_Page.ay + "," + A_Page.az + "\n");
            //            } else if (head == P_Page.header)
            //            {
            //                P_Page.Read(br);
            //                Console.Write("PPP" + "\n");
            //            } else if (head == M_Page.header)
            //            {
            //                M_Page.Read(br);
            //                Console.Write("MMM" + "\n");
            //            } else if (head == G_Page.header)
            //            {
            //                G_Page.Read(br);
            //                Console.Write(G_Page.ubx + "\n");
            //            }
            //        } while (true);
            //    }
            //    catch
            //    { }
            //}


            Console.ReadLine();
        }
    }

    /// <summary>
    /// Sylphide Protocol
    /// </summary>
    public class Sylphide_Protocol
    {
        public static byte header0 = 0xF7;
        public static byte header1 = 0xE0;
        public static byte sequence;
        public static byte[] crc16;
    }

    /// <summary>
    /// A page、IMU sensor page
    /// Output: acceralation, gyrosope, pressure, temparature
    /// </summary>
    public class A_Page
    {
        public static byte header = 0x41;
        public static string name = "A";
        // Sylphide形式によってもunsigned, signed, endianは違うので注意
        public static UInt32 inner_time;
        public static UInt32 gps_time;
        public static UInt32 ax, ay, az;
        public static UInt32 gx, gy, gz;
        public static UInt32 pressure, temp_press;
        public static Int16 temp_gyro;

        public static double cal_ax, cal_ay, cal_az;
        public static double cal_gx, cal_gy, cal_gz;
        public static double mean_ax = Math.Pow(2, 16) / 2;
        public static double mean_ay = Math.Pow(2, 16) / 2;
        public static double mean_az = Math.Pow(2, 16) / 2;
        public static double mean_gx = Math.Pow(2, 16) / 2;
        public static double mean_gy = Math.Pow(2, 16) / 2;
        public static double mean_gz = Math.Pow(2, 16) / 2;
        public static double fullScale_gyro = 2000; //Full Scale ±2000dps
        public static double fullScale_acc = 8; // Full Scale ±8G
        public static double lsb_gyro = fullScale_gyro * 2 / Math.Pow(2, 16);
        public static double lsb_acc = fullScale_acc * 2 / Math.Pow(2, 16);


        public static void Read(BinaryReader input)
        {
            byte[] buf3 = new byte[3];
            byte[] buf4 = new byte[4];

            // Windows is little endian
            inner_time = Convert.ToByte(input.ReadByte());
            gps_time = BitConverter.ToUInt32(input.ReadBytes(4), 0);
            ax = Read_3byte_BigEndian(input.ReadBytes(3));
            ay = Read_3byte_BigEndian(input.ReadBytes(3));
            az = Read_3byte_BigEndian(input.ReadBytes(3));
            gx = Read_3byte_BigEndian(input.ReadBytes(3));
            gy = Read_3byte_BigEndian(input.ReadBytes(3));
            gz = Read_3byte_BigEndian(input.ReadBytes(3));
            pressure = Read_3byte_BigEndian(input.ReadBytes(3)); //すごいロガーではNo Data
            temp_press = Read_3byte_BigEndian(input.ReadBytes(3)); //すごいロガーではNo Data
            temp_gyro = BitConverter.ToInt16(input.ReadBytes(2), 0);
            Convert_A_page();
        }

        private static UInt32 Read_3byte_BigEndian(byte[] data)
        {
            //Big Endianな３バイトのデータをBitConverterできるように４バイトのリトルエンディアンな
            //byte[] 配列に変換する
            byte[] buffer = { data[2], data[1], data[0], 0 };
            return BitConverter.ToUInt32(buffer, 0);
        }

        private static void Convert_A_page()
        {
            cal_ax = (ax - mean_ax) * lsb_acc;
            cal_ay = (ay - mean_ay) * lsb_acc;
            cal_az = (az - mean_az) * lsb_acc;
            cal_gx = (gx - mean_gx) * lsb_gyro;
            cal_gy = (gy - mean_gy) * lsb_gyro;
            cal_gz = (gz - mean_gz) * lsb_gyro;
        }
    }

    /// <summary>
    /// G page、GPS
    /// Output: u-blox format GPS binary data
    /// </summary>
    public class G_Page
    {
        public static byte header = 0x47;
        public static string name = "G";
        public static byte[] ubx = new byte[31];

        public static void Read(BinaryReader input)
        {
            ubx = input.ReadBytes(31);
        }
    }

    /// <summary>
    /// M page、MagneticField
    /// Output: Magnetic Field
    /// Caution: xyz diffent order on product. Ninja-Scan-Lite is x-y-z order.
    /// </summary>
    public class M_Page
    {
        public static byte header = 0x4d;
        public static string name = "M";
        // すごいロガーとその他Sylphide形式のものではxyzの順番が違うので気をつけること
        public static UInt32 inner_time;
        public static UInt32 gps_time;
        public static UInt32 mx, my, mz;

        public static double cal_mx, cal_my, cal_mz;
        public static double mean_mx = Math.Pow(2, 16) / 2;
        public static double mean_my = Math.Pow(2, 16) / 2;
        public static double mean_mz = Math.Pow(2, 16) / 2;
        public static double fullScale_mag = 1000; // Full Scale ±1000uT
        public static double lsb_mag = 0.1; // sensitivity 0.1uT/LSB

        public static void Read(BinaryReader input)
        {
            input.ReadBytes(2);
            inner_time = Convert.ToByte(input.ReadByte());
            gps_time = BitConverter.ToUInt32(input.ReadBytes(4), 0);
            mx = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            my = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            mz = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            input.ReadBytes(18);
            Convert_M_page();
        }

        private static void Convert_M_page()
        {
            cal_mx = (mx - mean_mx) * lsb_mag;
            cal_my = (my - mean_my) * lsb_mag;
            cal_mz = (mz - mean_mz) * lsb_mag;
        }
    }

    /// <summary>
    /// P page、Pressure
    /// Output: air pressure, temparature
    /// </summary>
    public class P_Page
    {
        public static byte header = 0x50;
        public static string name = "P";

        public static UInt32 inner_time;
        public static UInt32 gps_time;
        public static UInt32 pressure; //Pa*100
        public static UInt32 temperature;  //deg*100
        public static UInt32 coef1, coef2, coef3, coef4, coef5, coef6;

        public static void Read(BinaryReader input)
        {
            input.ReadBytes(2);
            inner_time = Convert.ToByte(input.ReadByte());
            gps_time = BitConverter.ToUInt32(input.ReadBytes(4), 0);
            pressure = Read_3byte_BigEndian(input.ReadBytes(3));
            temperature = Read_3byte_BigEndian(input.ReadBytes(3));
            input.ReadBytes(6);
            coef1 = Read_2byte_BigEndian(input.ReadBytes(2));
            coef2 = Read_2byte_BigEndian(input.ReadBytes(2));
            coef3 = Read_2byte_BigEndian(input.ReadBytes(2));
            coef4 = Read_2byte_BigEndian(input.ReadBytes(2));
            coef5 = Read_2byte_BigEndian(input.ReadBytes(2));
            coef6 = Read_2byte_BigEndian(input.ReadBytes(2));
        }

        private static UInt32 Read_3byte_BigEndian(byte[] data)
        {
            //Big Endianな３バイトのデータをBitConverterできるように４バイトのリトルエンディアンな
            //byte[] 配列に変換する
            byte[] buffer = { data[2], data[1], data[0], 0 };
            return BitConverter.ToUInt32(buffer, 0);
        }

        private static UInt16 Read_2byte_BigEndian(byte[] data)
        {
            //Big Endianな2バイトのデータをBitConverterできるように2バイトのリトルエンディアンな
            //byte[] 配列に変換する
            byte[] buffer = { data[1], data[0] };
            return BitConverter.ToUInt16(buffer, 0);
        }
    }

    /// <summary>
    /// H page、HumanPoweredAirplane
    /// Output: counter of cadace meter, counter of airspeed meter, altitude, control stick, auxually
    /// </summary>
    public class H_Page
    {
        public static byte header = 0x48;
        public static string name = "H";

        public static UInt32 inner_time;
        public static UInt32 gps_time;
        public static UInt16 cadence_counter;
        public static UInt16 airspeed_counter;
        public static UInt16 altimeter;
        public static UInt16 control_stick1;
        public static UInt16 control_stick2;
        public static UInt16 control_stick3;
        public static UInt16 aux1, aux2, aux3, aux4;

        public static void Read(BinaryReader input)
        {
            input.ReadBytes(2);
            inner_time = Convert.ToByte(input.ReadByte());
            gps_time = BitConverter.ToUInt32(input.ReadBytes(4), 0);
            cadence_counter = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            airspeed_counter = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            altimeter = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            control_stick1 = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            control_stick2 = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            control_stick3 = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            aux1 = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            aux2 = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            aux3 = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            aux4 = BitConverter.ToUInt16(input.ReadBytes(2), 0);
        }
    }

    /// <summary>
    /// N page、Navigation
    /// Output: latitude, longitude, altitude, velocity, attitude
    /// </summary>
    public class N_Page
    {
        public static byte header = 0x4E;
        public static string name = "N";

        public static UInt32 sequence_num;
        public static UInt32 gps_time;
        public static UInt32 latitude;
        public static UInt32 longitude;
        public static UInt32 altitude;
        public static UInt16 velocity_north;
        public static UInt16 velocity_east;
        public static UInt16 velocity_down;
        public static UInt16 heading;
        public static UInt16 roll;
        public static UInt16 pitch;

        public static void Read(BinaryReader input)
        {
            sequence_num = Convert.ToByte(input.ReadByte());
            input.ReadBytes(2);
            gps_time = BitConverter.ToUInt32(input.ReadBytes(4), 0);
            latitude = BitConverter.ToUInt32(input.ReadBytes(4), 0);
            longitude = BitConverter.ToUInt32(input.ReadBytes(4), 0);
            altitude = BitConverter.ToUInt32(input.ReadBytes(4), 0);
            velocity_north = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            velocity_east = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            velocity_down = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            heading = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            roll = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            pitch = BitConverter.ToUInt16(input.ReadBytes(2), 0);
        }

    }
}
