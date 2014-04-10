namespace PlayFOnline.UI.View.WinForms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinFormsDownloadView : IDownloadView
    {
       public event EventHandler CancelDownload;
        frmDownload Form;

        public WinFormsDownloadView()
        {
            this.Form = new frmDownload();

            this.Form.labelDetailed.Text = this.Form.labelDownloading.Text = string.Empty;
            this.Form.progressFile.Value = 0;
        }

        public void ShowError(string errorMsg)
        {
            MessageBox.Show(errorMsg, "Play FOnline", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void SetDownloadUrl(string text)
        {
            this.Form.labelDownloading.Text = text;
        }

        public void SetStatus(string text)
        {
            this.Form.labelDetailed.Text = text;
        }

        public void UpdateProgress(int percentage, string statusText)
        {
            this.Form.labelDetailed.Text = statusText;
            this.Form.progressFile.Value = percentage;
        }

        public void SetTitle(string text)
        {
            this.Form.Text = text;
        }

        public void Load()
        {
            this.Form.buttonCancel.Click += CancelDownload;
        }

        public void Close()
        {
            this.Form.Close();
        }

        public void Show()
        {
            this.Form.ShowDialog();
        }
    }
}
