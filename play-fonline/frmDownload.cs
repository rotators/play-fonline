namespace PlayFOnline
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Windows.Forms;

    /// <summary>
    /// Form that displays downloading.
    /// Based on https://github.com/wipe2238/FOUpdaterEx/blob/master/FOUpdater/frmMain.cs
    /// Thanks Wipe!
    /// </summary>
    public partial class frmDownload : Form
    {
        private WebClient webClient = new System.Net.WebClient();
        private object winformLocker = new object();

        public frmDownload(string game, string url, string path)
        {
            this.InitializeComponent();

            this.labelDetailed.Text = this.labelDownloading.Text = string.Empty;
            progressFile.Value = 0;

            this.Text = "Downloading " + game;

            try
            {
                this.webClient.Proxy = null;
                this.webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.webClient_DownloadProgressChanged);
                this.webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.webClient_DownloadFileCompleted);
                this.webClient.DownloadFileAsync(new Uri(url), path);
            }
            catch (UriFormatException)
            {
                MessageBox.Show(url + " is an invalid URL.");
                this.Close();
            }

            this.labelDownloading.Text = string.Format(url);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams result = base.CreateParams;
                if (Environment.OSVersion.Platform == PlatformID.Win32NT
                    && Environment.OSVersion.Version.Major >= 6)
                {
                    result.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                }

                return result;
            }
        }
        private static string ToHuman(ref long size, long b, string what)
        {
            if (size > 0 && size >= b)
            {
                long xb = size / b;
                size -= xb * b;
                return xb + what;
            }
            return string.Empty;
        }

        private static string ToHumanString(long size, bool getFirst = false)
        {
            long Kilobyte = 1024;
            long Megabyte = 1024 * Kilobyte;
            long Gigabyte = 1024 * Megabyte;
            long Terabyte = 1024 * Gigabyte;

            string result = string.Empty;
            char[] trimEnd = new char[] { ',', ' ' };

            result += ToHuman(ref size, Terabyte, "TB, ");
            if (getFirst && result.Length > 0)
                return result.TrimEnd(trimEnd);

            result += ToHuman(ref size, Gigabyte, "GB, ");
            if (getFirst && result.Length > 0)
                return result.TrimEnd(trimEnd);

            result += ToHuman(ref size, Megabyte, "MB, ");
            if (getFirst && result.Length > 0)
                return result.TrimEnd(trimEnd);

            result += ToHuman(ref size, Kilobyte, "KB, ");
            if (getFirst && result.Length > 0)
                return result.TrimEnd(trimEnd);

            if (size > 0) result += size + "B";

            return result.TrimEnd(trimEnd);
        }

        private void buttonCancel_Click_1(object sender, EventArgs e)
        {
            this.webClient.CancelAsync();
            this.Close();
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
                this.labelDetailed.Text = e.Error.Message;
            else
            {
                this.labelDetailed.Text = "Download completed.";
                this.Close();
            }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressFile.Value = e.ProgressPercentage;
            lock (this.winformLocker)
            {
                this.labelDetailed.Text = string.Format("{0} / {1} ({2}%)", ToHumanString(e.BytesReceived, true), ToHumanString(e.TotalBytesToReceive, true), e.ProgressPercentage);
            }
        }
    }
}