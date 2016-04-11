using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NinjaScan;

namespace NinjaScan_GUI
{
    public partial class GoogleEarth : Form
    {
        G_Page.UBlox ubx;

        // 起動からの時間
        double elapsed_t = 0;

        public GoogleEarth(Form1 owner)
        {
            ubx = owner.pages.g.ubx;

            InitializeComponent();

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                webBrowser1.AllowWebBrowserDrop = false;
                webBrowser1.IsWebBrowserContextMenuEnabled = false;
                webBrowser1.WebBrowserShortcutsEnabled = false;
                //webBrowser1.ObjectForScripting = this;

                webBrowser1.Navigate(System.Environment.CurrentDirectory + "\\GoogleEarth.html");
            }
            else
            {
                button1.Enabled = false;
            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            object[] args = { 1e-7 * ubx.llh.lat, 1e-7 * ubx.llh.lon, ubx.llh.height };
            webBrowser1.Document.InvokeScript("updateView", args);
        }

        const double update_marker_interval = 10;
        double previous_t_update_markers = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsed_t += 1E-3 * timer1.Interval;
            labelGPSFix.Text = "STATUS: " + ubx.status.gpsFix;
            var lat = 1e-7 * ubx.llh.lat; 
            var lng = 1e-7 * ubx.llh.lon;
            labelGPSLLH.Text = string.Format("latitude: {0:F6}  longitude: {1:F6}", lat, lng);
            labelTIME.Text = string.Format("UTC TIME: {0}:{1}:{2:F1}", ubx.utc.hour, ubx.utc.min, ubx.utc.sec);
            if (elapsed_t - previous_t_update_markers > update_marker_interval)
            {
                previous_t_update_markers = elapsed_t;
                object[] args = { lat, lng, ubx.llh.height };
                webBrowser1.Document.InvokeScript("updateMarkers", args);
            }
        }
    }
}
