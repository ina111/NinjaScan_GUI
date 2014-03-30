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
        public GoogleEarth()
        {
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
            Object obj_lat = (object)Form1.latitude;
            Object obj_lon = (object)Form1.longitude;
            Object obj_alt = (object)Form1.altitude;
            object[] args = { UBX.lat * Math.Pow(10, -7), UBX.lon * Math.Pow(10, -7), UBX.height };
            webBrowser1.Document.InvokeScript("js_func", args);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelGPSFix.Text = "STATUS: " + UBX.gpsFix;
            labelGPSLLH.Text = "latitude: " + UBX.lat + "  longitude: " + UBX.lon;
            labelTIME.Text = "UTC TIME: " + UBX.tow_hour + ":" + UBX.tow_min + ":" + (UBX.tow_sec).ToString("F1");
        }
    }
}
