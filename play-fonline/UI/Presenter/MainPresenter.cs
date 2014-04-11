namespace PlayFOnline.UI.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PlayFOnline.Core;
    using PlayFOnline.UI.View;
    using System.Net;
    using System.IO;
    using System.Reflection;
    using PlayFOnline.Core.Resources;
    using PlayFOnline.Data;
    using System.Diagnostics;

    public interface IMainPresenter
    {
        void ShowView();
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainPresenter
    {
        IMainView view;
        bool showOffline;

        private FOServerManager serverManager;
        private LogoManager logoManager;
        private InstallHandler installHandler;
        private FOSettings settings;
        private FOGameInfo currentGame;

        void OnRefreshServers(object sender, EventArgs e)
        {
            this.UpdateGameList();
        }

        void OnFoDevLinkClick(object sender, EventArgs e)
        {
            Process.Start("http://fodev.net");
        }

        void OnLaunchProgram(object sender, ItemEventArgs<string> program)
        {
            string path = Path.Combine(this.currentGame.InstallPath, program.Item);

            if (!File.Exists(path))
            {
                this.view.ShowError("Can't find " + path);
                return;
            }

            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(Path.Combine(this.currentGame.InstallPath, program.Item));
            process.Start();
        }

        void OnShowOfflineChanged(object sender, ItemEventArgs<bool> mode)
        {
            showOffline = mode.Item;
            this.UpdateGameList();
        }

        void OnInstallGame(object sender, FOGameInfo game)
        {
            this.AddGame(game);
        }

        void OnChangedGame(object sender, FOGameInfo game)
        {
            this.SelectGame(game);
        }

        public MainPresenter(IMainView view, FOSettings settings)
        {
            this.view = view;
            this.settings = settings;
        }

        public void Exit()
        {
            this.settings.UI = this.view.GetWindowProperties();
            SettingsManager.SaveSettings(this.settings);
            Environment.Exit(0);
        }

        public void StartApplication()
        {
            // Install eventhandlers
            this.view.RefreshServers += OnRefreshServers;
            this.view.ChangedGame += OnChangedGame;
            this.view.FoDevLinkClicked += OnFoDevLinkClick;
            this.view.InstallGame += OnInstallGame;
            this.view.LaunchProgram += OnLaunchProgram;
            this.view.ShowOfflineChanged += OnShowOfflineChanged;

            this.view.Load();
            this.view.SetTitle(this.GetBaseTitle());

            if (this.settings == null)
            {
                this.view.ShowError("Unable to load settings!");
            }

            // Initialize settings...
            if (this.settings.UI == null)
                this.settings.UI = new UISettings();
            else
                this.view.SetWindowProperties(this.settings.UI);

            if (this.settings.Paths == null)
            {
                string baseDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
                this.settings.Paths = new PathSettings();
                this.settings.Paths.Scripts      = Path.Combine(baseDir, "scripts");
                this.settings.Paths.DownloadTemp = Path.Combine(baseDir, "temp");
                this.settings.Paths.Logos        = Path.Combine(baseDir, "logos");
            }

            WebRequest.DefaultWebProxy = null; // Avoid possible lag due to .NET trying to resolve non-existing proxy.

            JsonFetcher jsonFetch = new JsonFetcher();
            FOJsonDeserializer jsonDeserialize = new FOJsonDeserializer();

            var jsonNode = jsonFetch.DownloadJson(this.settings.InstallUrl);
            this.installHandler = new InstallHandler(jsonDeserialize.GetInstallData(jsonNode), settings.Games, settings.Dependencies);
            this.logoManager = new LogoManager(this.settings.Paths.Logos, "http://fodev.net/status/json/logo/");
            this.serverManager = new FOServerManager(
                new FOServerJson(settings.ConfigUrl, settings.StatusUrl, settings.CombinedUrl), 
                this.installHandler);

            this.UpdateGameList();

            this.view.StartApplication();
        }

        private bool AddGame(FOGameInfo game)
        {
            if (!this.installHandler.HasInstallInfo(game.Id))
            {
                this.view.ShowError("No install info available for " + game.Name + " :(" + Environment.NewLine + "Please report this, so that it can be implemented.");
                return false;
            }

            if (view.AskYesNoQuestion("Is " + game.Name + " already installed on your computer?" + Environment.NewLine + "If this is the case, the installed directory can be added directly. This assumes that the game is working in its current state.", game.Name + " already installed?"))
            {
                string path = this.view.GetFolderPath();

                if (!this.installHandler.VerifyGameFolderPath(game.Id, path))
                {
                    this.view.ShowError(path + " does not contain a valid " + game.Name + " installation.");
                    return false;
                }

                game.InstallPath = path;

                string msg = "Successfully added " + game.Name + "!";
                this.view.ShowInfo(msg);
                this.installHandler.AddInstalledGame(game.Id, game.InstallPath);
            }
            else
            {
                InstallPresenter installer = new InstallPresenter(
                    new WinFormsInstallView(),
                    game, installHandler, 
                    logoManager, 
                    this.settings.Paths.Scripts, 
                    this.settings.Paths.DownloadTemp);
                installer.Show();

                if (!installer.IsSuccess)
                    return false;

                this.currentGame.InstallPath = this.installHandler.GetInstallPath(game.Id);

                string msg = "Successfully installed " + game.Name + "!";
                this.view.ShowInfo(msg);
            }
            this.settings.Games = this.installHandler.GetInstalledGames();
            SettingsManager.SaveSettings(this.settings);
            return true;
        }

        private string GetBaseTitle()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            return string.Format(
                "Play FOnline {0}.{1}.{2}",
                assemblyName.Version.Major, 
                assemblyName.Version.Minor, 
                assemblyName.Version.Build);
        }

        public void SetTitle(string text)
        {
            this.view.SetTitle(text);
        }

        public void SetOnlineTitleInfo(int players, int servers)
        {
            this.SetTitle(string.Format(
                "{0} - {1} players online on {2} servers",
                this.GetBaseTitle(),
                players, 
                servers));
        }

        private void PingServers(List<FOGameInfo> servers)
        {
            Action refreshDisplay = delegate()
            {
                this.view.RefreshServerList(servers);
            };
            this.serverManager.GetServers(true).ForEach(x => x.Ping(refreshDisplay));
        }

        private void SelectGame(FOGameInfo game)
        {
            if (game == null) return;

            bool installed = installHandler.IsInstalled(game.Id);
            this.view.ClearGameSelection();
            if (!this.installHandler.IsInstalled(game.Id))
            {
                this.view.AddInstallButton();
            }
            else 
            { 
                var programs = this.installHandler.GetLaunchPrograms(game.Id);
                this.view.SelectGame(game, programs);
            }
            this.currentGame = game;
        }

        public void UpdateGameList()
        {
            this.view.UpdateStatusBar("Updating gamelist...");
            try
            {
                //var servers = this.GetServers();
                //servers.Where(x => this.settings.IsInstalled(x.Id)).ToList().ForEach(x => x.InstallPath = this.settings.GetInstallPath(x.Id));

                serverManager.UpdateStatus();

                var servers = serverManager.GetServers(!this.showOffline);

                var online = servers.Where(x => !x.Status.IsOffline());
                this.SetOnlineTitleInfo(online.Sum(x => x.Status.Players), online.Count());

                this.PingServers(servers.ToList());

                this.view.RefreshServerList(servers);
                
                this.view.UpdateStatusBar("Updated gamelist.");

                return;
            }
            catch (System.Net.WebException e)
            {
                this.view.ShowError(string.Format("Unable to download {0}: {1}", e.Response.ResponseUri, e.Message));
            }
            catch (Exception e)
            {
                this.view.ShowError(string.Format("{0}{1}Please see log for more information.", e.Message, Environment.NewLine));
            }
            this.view.UpdateStatusBar("Failed to update gamelist.");
        }
    }
}
