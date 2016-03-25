using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using NinjaScan;

namespace NinjaScan_GUI
{
    public partial class GyroPlot : Form
    {
        int tickStart = 0;
        GraphPane gyroPane;

        static int average_length = 200;
        double[] array_x = new double[average_length];
        double[] array_y = new double[average_length];
        double[] array_z = new double[average_length];
        double mean_x, mean_y, mean_z;
        int average_pointer = 0;

        // 移動平均を更新、観測値を入力し、mean_Xを更新
        private void SMA_Update(double obj_x, double obj_y, double obj_z)
        {
            array_x[average_pointer] = obj_x;
            array_y[average_pointer] = obj_y;
            array_z[average_pointer] = obj_z;
            if (average_pointer + 1 >= average_length) average_pointer = -1; // 配列外に出るなら-1に戻す
            mean_x += (obj_x - array_x[average_pointer + 1]) / average_length;
            mean_y += (obj_y - array_y[average_pointer + 1]) / average_length;
            mean_z += (obj_z - array_z[average_pointer + 1]) / average_length;
            average_pointer++;

        }

        A_Page a_page;

        public GyroPlot(Form1 owner)
        {
            a_page = owner.pages.a;

            InitializeComponent();
            gyroPane = zedGraphControl1.GraphPane;
            gyroPane.Title.IsVisible = false;
            gyroPane.XAxis.Title.Text = "time (sec)";
            gyroPane.YAxis.Title.Text = "gyro (deg/s)";

            // Save 1200 points.  At 50 ms sample rate, this is one minute
            // The RollingPointPairList is an efficient storage class that always
            // keeps a rolling set of point data without needing to shift any data values
            RollingPointPairList gxlist = new RollingPointPairList(2000);
            RollingPointPairList gylist = new RollingPointPairList(2000);
            RollingPointPairList gzlist = new RollingPointPairList(2000);

            // Initially, a curve is added with no data points (list is empty)
            // Color is blue, and there will be no symbols
            LineItem curve = gyroPane.AddCurve("x axis", gxlist, Color.Blue, SymbolType.None);
            curve = gyroPane.AddCurve("y axis", gylist, Color.Red, SymbolType.None);
            curve = gyroPane.AddCurve("z axis", gzlist, Color.Green, SymbolType.None);

            // Sample at 50ms intervals
            timer1.Interval = 10;
            timer1.Enabled = true;
            timer1.Start();

            // Just manually control the X axis range so it scrolls continuously
            // instead of discrete step-sized jumps
            gyroPane.XAxis.Scale.Min = 0;
            gyroPane.XAxis.Scale.Max = (double)numericUpDown1.Value;
            gyroPane.XAxis.Scale.MinorStep = 0.5;
            gyroPane.XAxis.Scale.MajorStep = 1;
            gyroPane.YAxis.Scale.Max = A_Page.defaultCalibrationData.fullScale_gyro / 5;
            gyroPane.YAxis.Scale.Min = -A_Page.defaultCalibrationData.fullScale_gyro / 5;

            // Scale the axes
            zedGraphControl1.AxisChange();

            // Save the beginning time for reference
            tickStart = Environment.TickCount;
        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Make sure that the curvelist has at least one curve
            if (zedGraphControl1.GraphPane.CurveList.Count <= 0)
                return;

            // Get the first CurveItem in the graph
            LineItem gxcurve = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            LineItem gycurve = zedGraphControl1.GraphPane.CurveList[1] as LineItem;
            LineItem gzcurve = zedGraphControl1.GraphPane.CurveList[2] as LineItem;
            if (gxcurve == null || gycurve == null || gzcurve == null)
                return;

            // Get the PointPairList
            IPointListEdit gxlist = gxcurve.Points as IPointListEdit;
            IPointListEdit gylist = gycurve.Points as IPointListEdit;
            IPointListEdit gzlist = gzcurve.Points as IPointListEdit;
            // If this is null, it means the reference at curve.Points does not
            // support IPointListEdit, so we won't be able to modify it
            if (gxlist == null || gylist == null || gzlist == null)
                return;

            // Time is measured in seconds
            //double time = (Environment.TickCount - tickStart) / 1000.0;
            double time = a_page.gps_time / 1000.0;

            // 3 seconds per cycle
            //list.Add(time, Math.Sin(2.0 * Math.PI * time / 3.0));
            gxlist.Add(time, a_page.cal_gx);
            gylist.Add(time, a_page.cal_gy);
            gzlist.Add(time, a_page.cal_gz);

            // Keep the X scale at a rolling 30 second interval, with one
            // major step between the max X value and the end of the axis
            Scale xScale = zedGraphControl1.GraphPane.XAxis.Scale;
            //if (time > xScale.Max - xScale.MajorStep)
            //{
                xScale.Max = time + (double)numericUpDown1.Value / 8;
                xScale.Min = xScale.Max - (double)numericUpDown1.Value;
            //}
                if (xScale.Min < 0)
                    xScale.Min = 0;

            // Make sure the Y axis is rescaled to accommodate actual data
            zedGraphControl1.AxisChange();
            // Force a redraw
            zedGraphControl1.Invalidate();


            // 平均の表示
            SMA_Update(a_page.cal_gx, a_page.cal_gy, a_page.cal_gz);
            labelx.Text = "x (deg/s) :" + mean_x.ToString("F5");
            labely.Text = "y (deg/s) :" + mean_y.ToString("F5");
            labelz.Text = "z (deg/s) :" + mean_z.ToString("F5");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }


        /// <summary>
        /// X軸のパラメータ変更したときのイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            gyroPane.XAxis.Scale.Max = (double)numericUpDown1.Value;
        }

        /// <summary>
        /// Y軸のパラメータ変更したときのイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                gyroPane.YAxis.Scale.Max = A_Page.defaultCalibrationData.fullScale_gyro / 5;
                gyroPane.YAxis.Scale.Min = -A_Page.defaultCalibrationData.fullScale_gyro / 5;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                gyroPane.YAxis.Scale.Max = A_Page.defaultCalibrationData.fullScale_gyro / 2;
                gyroPane.YAxis.Scale.Min = -A_Page.defaultCalibrationData.fullScale_gyro / 2;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                gyroPane.YAxis.Scale.Max = A_Page.defaultCalibrationData.fullScale_gyro;
                gyroPane.YAxis.Scale.Min = -A_Page.defaultCalibrationData.fullScale_gyro;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Image img = gyroPane.GetImage();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "NinjaScanLite_GYRO";
            sfd.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            sfd.Filter = "PNG file(*.png)|*.png";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                img.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void buttonSetDrift_Click(object sender, EventArgs e)
        {
            Form1 owner = (Form1)this.Owner;
            
            owner.drift_gx += mean_x;
            owner.drift_gy += mean_y;
            owner.drift_gz += mean_z;
        }


    }
}
