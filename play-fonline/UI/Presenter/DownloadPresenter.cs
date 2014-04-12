namespace PlayFOnline.UI.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;

    using PlayFOnline.UI.View;
using System.ComponentModel;

    public class DownloadPresenter
    {
        private WebClient webClient = new System.Net.WebClient();
        private IDownloadView view;

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

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
                this.view.SetStatus("Download error:" + e.Error.Message);
            else
            {
                this.view.SetStatus("Download completed.");
                this.view.Close();
            }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.view.UpdateProgress(e.ProgressPercentage, string.Format("{0} / {1} ({2}%)", ToHumanString(e.BytesReceived, true), ToHumanString(e.TotalBytesToReceive, true), e.ProgressPercentage));
        }

        void OnCancelDownload(object sender, EventArgs e)
        {
            this.webClient.CancelAsync();
            this.view.Close();
        }

        public void Show(string game, string url, string path)
        {
            this.view.CancelDownload += new EventHandler(OnCancelDownload);

            this.view.Load();

            this.view.SetTitle("Downloading " + game);
            try
            {
                this.webClient.Proxy = null;
                this.webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.webClient_DownloadProgressChanged);
                this.webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.webClient_DownloadFileCompleted);
                this.webClient.DownloadFileAsync(new Uri(url), path);
            }
            catch (UriFormatException ex)
            {
                this.view.ShowError(string.Format("{0} is an invalid URL: {1}", url, ex.Message));
                this.view.Close();
            }
            this.view.SetDownloadUrl(string.Format(url));
            this.view.Show();
        }

        public DownloadPresenter(IDownloadView view)
        {
            this.view = view;
        }
    }
}
