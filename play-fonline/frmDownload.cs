using System.Net;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// Based on https://github.com/wipe2238/FOUpdaterEx/blob/master/FOUpdater/frmMain.cs
// Thanks Wipe!
namespace PlayFO
{
    public partial class frmDownload : Form
    {
        private object winformLocker = new object();
        WebClient webClient = new System.Net.WebClient();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams result = base.CreateParams;
                if( Environment.OSVersion.Platform == PlatformID.Win32NT
                    && Environment.OSVersion.Version.Major >= 6 )
                {
                    result.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                }

                return result;
            }
        }

        public frmDownload(string game, string url, string path)
        {
            InitializeComponent();

            labelDetailed.Text = labelDownloading.Text = "";
            progressFile.Value = 0;

            this.Text = "Downloading " + game;

            try
            {
                webClient.Proxy = null;
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
                webClient.DownloadFileAsync(new Uri(url), path);
            }
            catch(UriFormatException)
            {
                MessageBox.Show(url + " is an invalid URL.");
                this.Close();
            }

            labelDownloading.Text = String.Format(url);
        }

        void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
                labelDetailed.Text = e.Error.Message;
            else
            {
                labelDetailed.Text = "Download completed.";
                this.Close();
            }
        }

        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressFile.Value = e.ProgressPercentage;
            lock (this.winformLocker)
            {
                labelDetailed.Text = String.Format("{0} / {1} ({2}%)", ToHumanString(e.BytesReceived, true), ToHumanString(e.TotalBytesToReceive, true), e.ProgressPercentage);
            }
        }

        private static string ToHuman(ref long size, long b, string what)
        {
            if (size > 0 && size >= b)
            {
                long xb = size / b;
                size -= xb * b;
                return (xb + what);
            }
            return ("");
        }

        private readonly static long Kilobyte = 1024;
        private readonly static long Megabyte = 1024 * Kilobyte;
        private readonly static long Gigabyte = 1024 * Megabyte;
        private readonly static long Terabyte = 1024 * Gigabyte;

        private static string ToHumanString(long size, bool getFirst = false)
        {
            string result = "";
            char[] trimEnd = new char[] { ',', ' ' };

            result += ToHuman(ref size, Terabyte, "TB, ");
            if (getFirst && result.Length > 0)
                return (result.TrimEnd(trimEnd));

            result += ToHuman(ref size, Gigabyte, "GB, ");
            if (getFirst && result.Length > 0)
                return (result.TrimEnd(trimEnd));

            result += ToHuman(ref size, Megabyte, "MB, ");
            if (getFirst && result.Length > 0)
                return (result.TrimEnd(trimEnd));

            result += ToHuman(ref size, Kilobyte, "KB, ");
            if (getFirst && result.Length > 0)
                return (result.TrimEnd(trimEnd));

            if (size > 0) result += size + "B";

            return (result.TrimEnd(trimEnd));
        }

        private void buttonCancel_Click_1(object sender, EventArgs e)
        {
            webClient.CancelAsync();
            this.Close();
        }
    }
}
