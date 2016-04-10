﻿using System;
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
    public partial class AttiPlot : Form
    {
        int tickStart = 0;
        GraphPane gyroPane;

        A_Page a_page;
        AHRS.MadgwickAHRS ahrs;

        public AttiPlot(Form1 owner)
        {
            a_page = owner.pages.a;
            ahrs = owner.AHRS;

            InitializeComponent();
            gyroPane = zedGraphControl1.GraphPane;
            gyroPane.Title.IsVisible = false;
            gyroPane.XAxis.Title.Text = "time (sec)";
            gyroPane.YAxis.Title.Text = "attitude (deg)";

            // Save 1200 points.  At 50 ms sample rate, this is one minute
            // The RollingPointPairList is an efficient storage class that always
            // keeps a rolling set of point data without needing to shift any data values
            RollingPointPairList gxlist = new RollingPointPairList(2000);
            RollingPointPairList gylist = new RollingPointPairList(2000);
            RollingPointPairList gzlist = new RollingPointPairList(2000);

            // Initially, a curve is added with no data points (list is empty)
            // Color is blue, and there will be no symbols
            LineItem curve = gyroPane.AddCurve("Pitch", gxlist, Color.Blue, SymbolType.None);
            curve = gyroPane.AddCurve("Roll", gylist, Color.Red, SymbolType.None);
            curve = gyroPane.AddCurve("Yaw", gzlist, Color.Green, SymbolType.None);

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
            gyroPane.YAxis.Scale.Max = 180;
            gyroPane.YAxis.Scale.Min = -180;

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
            gxlist.Add(time, ahrs.Euler_deg[0]);
            gylist.Add(time, ahrs.Euler_deg[1]);
            gzlist.Add(time, ahrs.Euler_deg[2]);

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

        private void AttiPlot_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
