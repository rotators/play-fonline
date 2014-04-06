﻿namespace PlayFOnline.UI.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PlayFOnline.Data;
    using PlayFOnline.Core;
    using System.Windows.Forms;
    using System.Drawing;

    public interface IMainView : IBaseView
    {
        event EventHandler RefreshServers;
        event EventHandler FoDevLinkClicked;
        event EventHandler<FOGameInfo> InstallGame;
        event EventHandler<FOGameInfo> ChangedGame;
        event ItemEventHandler<string> LaunchProgram;
        event ItemEventHandler<bool> ShowOfflineChanged;

        bool AskYesNoQuestion(string question, string title);
        string GetFolderPath();

        void ShowError(string errorMsg);
        void ShowInfo(string infoMsg);
        void RefreshServerList(List<FOGameInfo> servers);
        
        void UpdateStatusBar(string text);
        UISettings GetWindowProperties();
        void ClearGameSelection();
        void AddInstallButton();
        void SelectGame(FOGameInfo game, List<FOGameLaunchProgram> programs);
        void SetWindowProperties(UISettings settings);
        void SetTitle(string title);

        void Load();
        void StartApplication();
    }

    public class MainView : BaseWinFormsView, IMainView
    {
        private frmMain Main;

        public event EventHandler RefreshServers;
        public event EventHandler FoDevLinkClicked;
        public event EventHandler<FOGameInfo> InstallGame;
        public event EventHandler<FOGameInfo> ChangedGame;
        public event ItemEventHandler<string> LaunchProgram;
        public event ItemEventHandler<bool> ShowOfflineChanged;

        public void Load()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            this.Main = new frmMain();
            this.Main.olvInstallPath.AspectToStringConverter = delegate(object x)
            {
                string path = (string)x;
                if (string.IsNullOrWhiteSpace(path))
                    return "Not installed/added";
                return path;
            };

            this.Main.olvPing.AspectToStringConverter = delegate(object x)
            {
                int latency = (int)x;
                if (latency != int.MaxValue)
                    return string.Format("{0} ms", latency);
                return string.Empty;
            };

            this.Main.btnRefresh.Click += RefreshServers;
            this.Main.chkShowOffline.CheckedChanged += new EventHandler(chkShowOffline_CheckedChanged);
            this.Main.linkFoDev.Click += FoDevLinkClicked;
            this.Main.lstGames.SelectionChanged += new EventHandler(lstGames_SelectedIndexChanged);
            this.Main.btnInstall.Click += new EventHandler(btnInstall_Click);
        }

        public void StartApplication()
        {
            Application.Run(this.Main);
        }

        void chkShowOffline_CheckedChanged(object sender, EventArgs e)
        {
            ShowOfflineChanged(this, new ItemEventArgs<bool>(this.Main.chkShowOffline.Checked));
        }

        void btnInstall_Click(object sender, EventArgs e)
        {
            FOGameInfo game = (FOGameInfo)this.Main.lstGames.SelectedObject;
            InstallGame(this, game);
        }

        void lstGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            FOGameInfo game = (FOGameInfo)this.Main.lstGames.SelectedObject;
            ChangedGame(this, game);
        }

        public void ClearGameSelection()
        {
           // this.Main.btnInstall.Enabled = !installed;
           // this.Main.btnOptions.Enabled = installed;

            var controls = this.Main.flowMenu.Controls;

            this.Main.flowMenu.Controls.Clear();
            this.Main.flowMenu.Update();
        }

        public void AddInstallButton()
        {
            this.Main.flowMenu.Controls.Add(this.Main.btnInstall);
        }

        public void SelectGame(FOGameInfo game, List<FOGameLaunchProgram> programs)
        {
            foreach (FOGameLaunchProgram program in programs)
            {
                Button btnCustom = new Button();
                btnCustom.Text = program.Name;
                btnCustom.Width = this.Main.btnInstall.Width;
                btnCustom.Height = this.Main.btnInstall.Height;
                btnCustom.Tag = program.File;
                btnCustom.Click += new EventHandler(btnCustom_Click);
                this.Main.flowMenu.Controls.Add(btnCustom);
            }
        }

        void btnCustom_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            LaunchProgram(this, new ItemEventArgs<String>((string)btn.Tag));

        }

        public void SetTitle(string title)
        {
            this.Main.Text = title;
        }

        public void SetWindowProperties(UISettings settings)
        {
            this.Main.DesktopLocation = new Point(settings.X, settings.Y);
            this.Main.Width = settings.Width;
            this.Main.Height = settings.Height;
        }

        public UISettings GetWindowProperties()
        {
            UISettings settings = new UISettings();
            settings.X = this.Main.DesktopLocation.X;
            settings.Y = this.Main.DesktopLocation.Y;
            settings.Width = this.Main.Width;
            settings.Height = this.Main.Height;
            return settings;
        }

        public void RefreshServerList(List<FOGameInfo> servers)
        {
            this.Main.lstGames.SetObjects(servers);
            this.Main.lstGames.Refresh();
        }

        public void UpdateStatusBar(string text)
        {
            this.Main.statusBarLabel.Text = DateTime.Now + " | " + text;
        }
    }
}
