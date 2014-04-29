using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra.Double;

namespace NinjaScan
{
    /// <summary>
    /// Sylphide Protocol
    /// </summary>
    public class Sylphide_Protocol
    {
        public static byte header0 = 0xF7;
        public static byte header1 = 0xE0;
        public byte sequence;
        public byte[] crc16;
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
        public static double zero_ax = Math.Pow(2, 16) / 2;
        public static double zero_ay = Math.Pow(2, 16) / 2;
        public static double zero_az = Math.Pow(2, 16) / 2;
        public static double zero_gx = Math.Pow(2, 16) / 2;
        public static double zero_gy = Math.Pow(2, 16) / 2;
        public static double zero_gz = Math.Pow(2, 16) / 2;
        public static double drift_ax = 0.0; //driftは物理量単位
        public static double drift_ay = 0.0;
        public static double drift_az = 0.0;
        public static double drift_gx = 0.0;
        public static double drift_gy = 0.0;
        public static double drift_gz = 0.0;
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
            cal_ax = (ax - zero_ax) * lsb_acc - drift_ax;
            cal_ay = (ay - zero_ay) * lsb_acc - drift_ay;
            cal_az = (az - zero_az) * lsb_acc - drift_az;
            cal_gx = (gx - zero_gx) * lsb_gyro - drift_gx;
            cal_gy = (gy - zero_gy) * lsb_gyro - drift_gy;
            cal_gz = (gz - zero_gz) * lsb_gyro - drift_gz;
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
        public static UInt16 inner_time;
        public static UInt32 gps_time;
        public static Int16 mx, my, mz; // uT
        // OUTPUT data range is -30000 to +300000, FullScale is +-1000uT

        public static double cal_mx, cal_my, cal_mz;
        public static double zero_mx = 0;
        public static double zero_my = 0;
        public static double zero_mz = 0;
        public static double fullScale_mag = 1000; // Full Scale ±1000uT
        public static double lsb_mag = 0.1; // sensitivity 0.1uT/LSB

        public static void Read(BinaryReader input)
        {
            input.ReadBytes(2);
            inner_time = Convert.ToByte(input.ReadByte());
            gps_time = BitConverter.ToUInt32(input.ReadBytes(4), 0);
            mx = Read_2byte_BigEndian(input.ReadBytes(2));
            my = Read_2byte_BigEndian(input.ReadBytes(2));
            mz = Read_2byte_BigEndian(input.ReadBytes(2));
            input.ReadBytes(18);
            Convert_M_page();
        }

        private static void Convert_M_page()
        {
            cal_mx = (mx - zero_mx) * lsb_mag;
            cal_my = (my - zero_my) * lsb_mag;
            cal_mz = (mz - zero_mz) * lsb_mag;
        }

        private static Int16 Read_2byte_BigEndian(byte[] data)
        {
            //Big Endianな2バイトのデータをBitConverterできるように2バイトのリトルエンディアンな
            //byte[] 配列に変換する
            byte[] buffer = { data[1], data[0] };
            Int16 output = BitConverter.ToInt16(buffer, 0);
            return output;
            //return BitConverter.ToUInt16(buffer, 0);
        }

        /// <summary>
        /// http://www.fujikura.co.jp/rd/gihou/backnumber/pages/__icsFiles/afieldfile/2012/11/08/122_R8.pdf
        /// 
        /// </summary>
        private static void Calibration()
        {
            // オフセット、全磁力を初期化
            double offset_x = 0;
            double offset_y = 0;
            double offset_z = 0;
            double total_magnetic_force = 0; // 全磁力
            double alpha = 1.0;

            // 共分散行列を初期化
            DenseMatrix covariance = new DenseMatrix(4, 4, new[] {alpha, 0, 0, 0,
                                                                  0, alpha, 0, 0,
                                                                  0, 0, alpha, 0,
                                                                  0, 0, 0, alpha});

            // 磁気データとオフセット、全磁力を使用し、誤差関数eを計算する
            double A = Math.Pow(cal_mx - offset_x, 2) +
                       Math.Pow(cal_my - offset_y, 2) +
                       Math.Pow(cal_my - offset_y, 2);
            double B = Math.Pow(total_magnetic_force, 2);
            double e = A - B;

            // 誤差関数e、磁気データ、共分散行列を使用し、オフセット残渣ηを計算する


            // 磁気データを使用し、共分散行列Pを更新する

            // オフセット残渣ηをオフセットに加算し、最新のオフセットとする


        }
    }

    /// <summary>
    /// P page、Pressure
    /// Output: air pressure, temparature
    /// MS5611 calcuration
    /// </summary>
    public class P_Page
    {
        public static byte header = 0x50;
        public static string name = "P";

        public static UInt32 inner_time;
        public static UInt32 gps_time;
        public static Int32 pressure; //hPa*100
        public static Int32 temperature;  //deg*100
        public static UInt32 D1;
        public static UInt32 D2;
        public static UInt32 coef1, coef2, coef3, coef4, coef5, coef6;
        public static Int64 dT;
        public static Int64 OFF;
        public static Int64 SENS;

        public static void Read(BinaryReader input)
        {
            input.ReadBytes(2);
            inner_time = Convert.ToByte(input.ReadByte());
            gps_time = BitConverter.ToUInt32(input.ReadBytes(4), 0);
            D1 = Read_3byte_BigEndian(input.ReadBytes(3));
            D2 = Read_3byte_BigEndian(input.ReadBytes(3));
            input.ReadBytes(6);
            coef1 = Read_2byte_BigEndian(input.ReadBytes(2));
            coef2 = Read_2byte_BigEndian(input.ReadBytes(2));
            coef3 = Read_2byte_BigEndian(input.ReadBytes(2));
            coef4 = Read_2byte_BigEndian(input.ReadBytes(2));
            coef5 = Read_2byte_BigEndian(input.ReadBytes(2));
            coef6 = Read_2byte_BigEndian(input.ReadBytes(2));
            dT = (Int32)(D2 - coef5 * Math.Pow(2, 8));
            temperature = (Int32)(2000 + dT * coef6 / Math.Pow(2, 23));
            OFF = (Int64)(coef2 * Math.Pow(2, 16) + (coef4 * dT) / Math.Pow(2, 7));
            SENS = (Int64)(coef1 * Math.Pow(2, 15) + (coef3 * dT) / Math.Pow(2, 8));
            pressure = (Int32)((D1 * SENS / Math.Pow(2, 21) - OFF) / Math.Pow(2, 15));

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

    /// <summary>
    /// G page、GPS
    /// Output: u-blox format GPS binary data
    /// </summary>
    public class G_Page
    {
        public static byte header = 0x47;
        public static string name = "G";
        public static byte[] ubx = new byte[31];
        public static byte[] analysisObject = new byte[1000];

        public static int object_offset = 0; // コピー先のオフセット
        public static Boolean isAnalysisPayload = false; // ヘッドシークかペイロード解析か
        public static Boolean isEnoughPayload = false; // チェックサムまでペイロードがbuffに揃っているか

        public static byte[] ubxheader = { 0xB5, 0x62 };
        public static byte[] id_NAV_POSLLH = { 0x01, 0x02 };

        public static void Read(BinaryReader input)
        {
            ubx = input.ReadBytes(31);
            //Console.WriteLine("Object offset: " + object_offset);
            Array.Copy(ubx, 0, analysisObject, object_offset, 31); //31バイトをbuffにコピー,object_offsetがあれば、その分オフセット
        }

        /// <summary>
        /// ヘッドシークか、ペイロード解析なのか
        /// （ヘッドシーク）ヘッドがあるかどうか ->N/buff廃棄
        /// （ヘッドあり）次のinputが必要か？
        /// （input必要）inputを次の32byteを含めてヘッドシークでやり直し
        /// （input不必要）IDでペイロードの長さを求めてペイロード解析へ。解析対象がさらに必要かどうか？
        /// （buff必要）ペイロード解析としてやり直し
        /// （buff不必要）解析を行う。チェックサム以降はbuffに入れて、append_offsetをチェックサムのindexにする
        /// </summary>
        /// <param name="input"></param>
        public static void SeekHead(byte[] input)
        {
            try
            {
                if (isAnalysisPayload == false)
                {
                    // ヘッドシーク
                    int index_header0 = Array.IndexOf(input, ubxheader[0]);
                    int index_header1 = Array.IndexOf(input, ubxheader[1]);
                    if (index_header0 < 0)
                    {
                        object_offset = 0;
                        analysisObject = new byte[1000];
                        return;
                    }
                    else if (index_header0 > 27 + object_offset)
                    {
                        object_offset += 31; // offsetは32byteからG_head分を除いた31byteを足していく
                        return;
                    }
                    else if (index_header0 + 1 == index_header1)
                    {
                        // IDの読み取り
                        byte[] id_header = new byte[2];
                        Array.Copy(input, index_header0 + 2, id_header, 0, 2);
                        // ペイロードの長さの読み取り
                        byte[] byte_index_payload_length = new byte[2];
                        Array.Copy(input, index_header0 + 4, byte_index_payload_length, 0, 2);
                        UBX.length_payload = BitConverter.ToUInt16(byte_index_payload_length, 0);

                        // ペイロード長さが1000を超える場合はヘッダを適当なものに読み間違えているので、廃棄
                        if (UBX.length_payload > 1000)
                        {
                            object_offset = 0;
                            analysisObject = new byte[1000];
                            return;
                        }


                        if (index_header0 + UBX.length_payload + 8 < object_offset + 31)
                        {
                            // IDによる場合分け
                            if (id_header.SequenceEqual(UBX.id_NAV_POSLLH))
                            {
                                UBX.Analysis_NAV_POSLLH(input, index_header0);
                                // ここにパケットの残りを詰め込む処理を入れる
                            }
                            else if (id_header.SequenceEqual(UBX.id_NAV_STATUS))
                            {
                                UBX.Analysis_NAV_STATUS(input, index_header0);
                            }
                            else if (id_header.SequenceEqual(UBX.id_NAV_SOL))
                            {

                            }
                            else if (id_header.SequenceEqual(UBX.id_NAV_TIMEUTC))
                            {
                                UBX.Analysis_NAV_TIMEUTC(input, index_header0);
                            }

                            byte[] analysisObject_buff = new byte[1000];
                            Array.Copy(analysisObject, index_header0 + UBX.length_payload + 8, analysisObject_buff, 0, 31);
                            object_offset = (object_offset + 31) - (index_header0 + UBX.length_payload + 8);


                            analysisObject = new byte[1000];
                            Array.Copy(analysisObject_buff, 31, analysisObject, 0, 31);
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Payload length: " + (UBX.length_payload));
                            object_offset += 31; // offsetは32byteからG_head分を除いた31byteを足していく
                            return;

                        }

                    }
                    else
                    {
                        object_offset = 0;
                        analysisObject = new byte[1000];
                        return;
                    }

                }
                else
                {

                }
                // headerの1文字目があったら29個目以上だったら次のものも読み込む。
                // 次にIDを検索するそれで振り分け。ペイロード分だけ読み込み。
                int i = Array.IndexOf(input, ubxheader[0]);
                int j = Array.IndexOf(input, ubxheader[1]);
                byte[] id_head = new byte[2];
                // headerの判別をする or 必要な個数を全部読み込むまでバッファーをスルー or バッファーを破棄
                if (j == i + 1 && j < 29)
                {
                    Array.Copy(input, j + 1, id_head, 0, 2);
                    Console.WriteLine(id_head[0] + ", " + id_head[1]);

                    // ヘッダの数によって場合分けをする
                    // ペイロード長さによってappend_numを増やす、何回appendするかのカウンターも回す

                }
                else
                {
                    //buff = new byte[1000];
                    //append_num = 0;
                }
                Console.WriteLine(i + ", " + j);
            }
            catch (Exception e)
            { Console.WriteLine(e.ToString()); }
        }
    }

    public class UBX
    {
        public static byte[] header = { 0xB5, 0x62 };
        public static byte[] id_NAV_POSLLH = { 0x01, 0x02 };
        public static byte[] id_NAV_STATUS = { 0x01, 0x03 };
        public static byte[] id_NAV_DOP = { 0x01, 0x04 };
        public static byte[] id_NAV_SOL = { 0x01, 0x06 };
        public static byte[] id_NAV_VELNED = { 0x01, 0x12 };
        public static byte[] id_NAV_TIMEGPS = { 0x01, 0x20 };
        public static byte[] id_NAV_TIMEUTC = { 0x01, 0x21 };
        public static byte[] id_NAV_SVINFO = { 0x01, 0x30 };
        public static byte[] id_RXM_RAW = { 0x02, 0x10 };
        public static byte[] id_RXM_SFRB = { 0x02, 0x31 };
        public static byte[] id_RXM_EPH = { 0x02, 0x31 };
        public static byte[] id_AID_HUI = { 0x0B, 0x02 };

        public static int length_payload = 0;

        // NAV_POSLLH
        public static UInt32 itow;
        public static Int32 lon, lat, height; //緯度経度高度 緯度経度はe-7 deg 高度はmm
        public static Int32 hMSL; //平均海面高度
        public static UInt32 hAcc, vAcc; //推定精度

        // NAV_SOL
        public static Int32 ftow;
        public static Int16 week;
        public static string gpsFix; //0x00=no fix, 0x01=dead reckoning only, 0x02=2d fix, 0x03=3d fix, 0x05=time only fix
        public static byte flags;
        public static Int32 ecefX;
        public static Int32 ecefY;
        public static Int32 ecefZ;
        public static Int32 ecefVX;
        public static Int32 ecefVY;
        public static Int32 ecefVZ;
        public static UInt32 numSV;

        // gpsFix
        private static byte[] gpsFix_NoFix = new byte[1] { 0x00 };
        private static byte[] gpsFix_DeadReckoningOnly = new byte[1] { 0x01 };
        private static byte[] gpsFix_2DFix = new byte[1] { 0x02 };
        private static byte[] gpsFix_3DFix = new byte[1] { 0x03 };
        private static byte[] gpsFix_GPSDeadReckoningConbined = new byte[1] { 0x04 };
        private static byte[] gpsFix_TimeOnlyFix = new byte[1] { 0x05 };

        // NAV_STATUS
        public static byte status_flags;
        public static byte fixStat;
        public static byte status_flags2;
        public static UInt32 ttff;
        public static UInt32 msss;

        // NAV_TIMEUTC
        public static UInt16 year;
        public static byte month;
        public static byte day;
        public static byte hour;
        public static byte min;
        public static byte sec;

        // Time
        public static UInt32 tow_day;
        public static UInt32 tow_hour;
        public static UInt32 tow_min;
        public static double tow_sec;

        public static void Read_NAV_POSECEF(BinaryReader input)
        {
            // 20byteのペイロード以下しかなかったらどうするか

        }

        public static void Analysis_NAV_POSLLH(byte[] input, int index_header0)
        {
            byte[] byteitow = new byte[4];
            byte[] bytelon = new byte[4];
            byte[] bytelat = new byte[4];
            byte[] byteheight = new byte[4];
            byte[] bytehMSL = new byte[4];
            byte[] bytehAcc = new byte[4];
            byte[] bytevAcc = new byte[4];

            Array.Copy(input, index_header0 + 6, byteitow, 0, 4);
            Array.Copy(input, index_header0 + 10, bytelon, 0, 4);
            Array.Copy(input, index_header0 + 14, bytelat, 0, 4);
            Array.Copy(input, index_header0 + 18, byteheight, 0, 4);
            Array.Copy(input, index_header0 + 22, bytehMSL, 0, 4);
            Array.Copy(input, index_header0 + 26, bytehAcc, 0, 4);
            Array.Copy(input, index_header0 + 30, bytevAcc, 0, 4);

            itow = BitConverter.ToUInt32(byteitow, 0);
            lon = BitConverter.ToInt32(bytelon, 0);
            lat = BitConverter.ToInt32(bytelat, 0);
            height = BitConverter.ToInt32(byteheight, 0);
            hMSL = BitConverter.ToInt32(bytehMSL, 0);
            hAcc = BitConverter.ToUInt32(bytehAcc, 0);
            vAcc = BitConverter.ToUInt32(bytevAcc, 0);

            TOW2Time(itow);
        }

        public static void Analysis_NAV_STATUS(byte[] input, int index_header0)
        {
            byte[] byteitow = new byte[4];
            byte[] bytegpsFix = new byte[1];
            byte[] byteflags = new byte[1];
            byte[] bytefixStat = new byte[1];
            byte[] byteflags2 = new byte[1];
            byte[] bytettff = new byte[4];
            byte[] bytemsss = new byte[4];

            Array.Copy(input, index_header0 + 6, byteitow, 0, 4);
            Array.Copy(input, index_header0 + 10, bytegpsFix, 0, 1);
            Array.Copy(input, index_header0 + 11, byteflags, 0, 1);
            Array.Copy(input, index_header0 + 12, bytefixStat, 0, 1);
            Array.Copy(input, index_header0 + 13, byteflags2, 0, 1);
            Array.Copy(input, index_header0 + 14, bytettff, 0, 4);
            Array.Copy(input, index_header0 + 18, bytemsss, 0, 4);

            itow = BitConverter.ToUInt32(byteitow, 0);
            if (bytegpsFix.SequenceEqual(gpsFix_NoFix))
            {
                gpsFix = "No Fix";
            }
            else if (bytegpsFix.SequenceEqual(gpsFix_2DFix))
            {
                gpsFix = "2D Fix";
            }
            else if (bytegpsFix.SequenceEqual(gpsFix_3DFix))
            {
                gpsFix = "3D Fix";
            }
            else if (bytegpsFix.SequenceEqual(gpsFix_TimeOnlyFix))
            {
                gpsFix = "Time Only Fix";
            }

            TOW2Time(itow);
        }

        public static void Analysis_NAV_TIMEUTC(byte[] input, int index_header0)
        {
            byte[] byteitow = new byte[4];
            byte[] byteyear = new byte[2];
            byte[] bytemonth = new byte[1];
            byte[] byteday = new byte[1];
            byte[] bytehour = new byte[1];
            byte[] bytemin = new byte[1];
            byte[] bytesec = new byte[1];

            Array.Copy(input, index_header0 + 6, byteitow, 0, 4);
            Array.Copy(input, index_header0 + 18, byteyear, 0, 2);
            Array.Copy(input, index_header0 + 20, bytemonth, 0, 1);
            Array.Copy(input, index_header0 + 21, byteday, 0, 1);
            Array.Copy(input, index_header0 + 22, bytehour, 0, 1);
            Array.Copy(input, index_header0 + 23, bytemin, 0, 1);
            Array.Copy(input, index_header0 + 24, bytesec, 0, 1);

            itow = BitConverter.ToUInt32(byteitow, 0);
            year = BitConverter.ToUInt16(byteyear, 0);
            month = bytemonth[0];
            day = byteday[0];
            hour = bytehour[0];
            min = bytemin[0];
            sec = bytesec[0];

            TOW2Time(itow);
        }

        // itow(=Time of Weeks)から時刻を求める
        // http://www.novatel.com/support/knowledge-and-learning/published-papers-and-documents/unit-conversions/
        private static void TOW2Time(UInt32 iToW)
        {
            double remainder_tow_day, remainder_tow_hour, remainder_tow_min;
            tow_day = iToW / 1000 / 86400;
            remainder_tow_day = (double)(iToW) / 1000.0 / 86400.0 - tow_day;
            tow_hour = (UInt32)(remainder_tow_day * 86400 / 3600);
            remainder_tow_hour = (remainder_tow_day * 86400 / 3600) - tow_hour;
            tow_min = (UInt32)(remainder_tow_hour * 3600 / 60);
            remainder_tow_min = (remainder_tow_hour * 3600 / 60) - tow_min;
            tow_sec = remainder_tow_min * 60;
        }

    }
}
