using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace NinjaScan_GUI
{
    public class Program
    {

        static void Main(string[] args)
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
                        Console.Write(A_Page.ax + "," + A_Page.ay + "," + A_Page.az + "," +
                            A_Page.gx + "," + A_Page.gy + "," + A_Page.gz + "\n");
                    }
                    else if (head == P_Page.header)
                    {
                        P_Page.Read(br);
                        Console.Write("PPP" + "\n");
                    }
                    else if (head == M_Page.header)
                    {
                        M_Page.Read(br);
                        Console.Write("MMM" + "\n");
                    }
                    else if (head == G_Page.header)
                    {
                        G_Page.Read(br);
                        Console.Write(G_Page.ubx + "\n");
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



            // メッセージの出力
            Console.WriteLine("{0} ({1}歳) さん、ようこそお越しくださいました。", "taka", 27);
            Console.ReadLine();
        }
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
        }

        private static UInt32 Read_3byte_BigEndian(byte[] data)
        {
            //Big Endianな３バイトのデータをBitConverterできるように４バイトのリトルエンディアンな
            //byte[] 配列に変換する
            byte[] buffer = { data[2], data[1], data[0], 0 };
            return BitConverter.ToUInt32(buffer, 0);
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

        public static void Read(BinaryReader input)
        {
            input.ReadBytes(2);
            inner_time = Convert.ToByte(input.ReadByte());
            gps_time = BitConverter.ToUInt32(input.ReadBytes(4), 0);
            mx = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            my = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            mz = BitConverter.ToUInt16(input.ReadBytes(2), 0);
            input.ReadBytes(18);
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
        public static UInt32 pressure;
        public static UInt32 temperature;
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
