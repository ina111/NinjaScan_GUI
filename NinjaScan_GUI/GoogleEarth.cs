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
        private int sec = 0;

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
            object[] args = { ubx.llh.lat * Math.Pow(10, -7), ubx.llh.lon * Math.Pow(10, -7), ubx.llh.height };
            webBrowser1.Document.InvokeScript("js_func", args);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sec++;
            labelGPSFix.Text = "STATUS: " + ubx.status.gpsFix;
            labelGPSLLH.Text = "latitude: " + ubx.llh.lat + "  longitude: " + ubx.llh.lon;
            labelTIME.Text = "UTC TIME: " + ubx.utc.hour + ":" + ubx.utc.min + ":" + ubx.utc.sec.ToString("F1");
            if (sec >= 30 && (sec % 10 == 0))
            {
                object[] args = { ubx.llh.lat * Math.Pow(10, -7), ubx.llh.lon * Math.Pow(10, -7), ubx.llh.height };
                webBrowser1.Document.InvokeScript("js_func1", args);
            }
        }
    }
}
