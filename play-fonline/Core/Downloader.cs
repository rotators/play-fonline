namespace PlayFOnline.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PlayFOnline.UI.Presenter;
    using PlayFOnline.UI.View;
    using PlayFOnline.UI.View.WinForms;
    

    /// <summary>
    /// Downloading a file with progressbar.
    /// </summary>
    public class ProgressDownloader
    {
        public void Download(string game, string url, string path)
        {
            IDownloadView view = new WinFormsDownloadView();
            DownloadPresenter presenter = new DownloadPresenter(view, game, url, path);
        }
    }
}
