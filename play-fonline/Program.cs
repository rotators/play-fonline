namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PlayFOnline.Core;
    using PlayFOnline.UI;
    using PlayFOnline.UI.View;
    using PlayFOnline.UI.View.Console;
    using PlayFOnline.UI.Presenter;
    
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var view = new WinFormsMainView();
            MainPresenter presenter = new MainPresenter(view, SettingsManager.LoadSettings());
            presenter.StartApplication();
        }
    }
}
