namespace PlayFOnline.UI.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;
    using System.IO;
    using System.Reflection;
    using System.Diagnostics;
    using FOQuery;
    using FOQuery.Data;
    using FOQuery.Json;
    using PlayFOnline.Core;
    using PlayFOnline.UI.View;
    using NLog;
    using System.ComponentModel;
    

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
        bool pingServers;

        private ServerManager serverManager;
        private LogoManager logoManager;
        private InstallHandler installHandler;
        private FOSettings settings;
        private FOGameInfo currentGame;

        private NLog.Logger logger = LogManager.GetCurrentClassLogger();

        void OnRefreshServers(object sender, EventArgs e)
        {
            this.UpdateGameList();
        }

        void OnFoDevLinkClick(object sender, EventArgs e)
        {
            Process.Start("http://fodev.net");
            logger.Debug("Clicked fodev.net link.");
        }

        void OnLaunchProgram(object sender, ItemEventArgs<string> program)
        {
            string path = Path.Combine(this.currentGame.InstallPath, program.Item);

            if (!File.Exists(path))
            {
                this.view.ShowError("Can't find " + path);
                return;
            }

            logger.Debug("Starting {0}.", path);
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(path);
            process.Start();
            logger.Debug("Started {0}.", path);
        }

        void OnShowOfflineChanged(object sender, ItemEventArgs<bool> mode)
        {
            this.showOffline = mode.Item;
            this.UpdateGameList();
        }

        void OnPingChanged(object sender, ItemEventArgs<bool> mode)
        {
            this.pingServers = mode.Item;
            this.UpdateGameList();
        }

        void OnInstallGame(object sender, FOGameInfo game)
        {
            if (game == null) return;

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
            this.view.PingChanged += OnPingChanged;

            this.view.Load();
            this.view.SetTitle(this.GetBaseTitle());

            this.logger.Info("Starting {0}", GetBaseTitle());

            if (this.settings == null)
            {
                this.view.ShowError("Unable to load settings!");
                return;
            }

            // Initialize settings...
            if (this.settings.UI == null)
                this.settings.UI = new UISettings();
            else
                this.view.SetWindowProperties(this.settings.UI);

            if (this.settings.Paths == null)
            {
                this.logger.Info("Paths not found, setting default paths.");
                string baseDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
                this.settings.Paths = new PathSettings();
                this.settings.Paths.Scripts      = Path.Combine(baseDir, "scripts");
                this.settings.Paths.DownloadTemp = Path.Combine(baseDir, "temp");
                this.settings.Paths.Logos        = Path.Combine(baseDir, "logos");
            }

            if (!Directory.Exists(this.settings.Paths.DownloadTemp)) Directory.CreateDirectory(this.settings.Paths.DownloadTemp);
            if (!Directory.Exists(this.settings.Paths.Scripts)) Directory.CreateDirectory(this.settings.Paths.Scripts);
            if (!Directory.Exists(this.settings.Paths.Logos)) Directory.CreateDirectory(this.settings.Paths.Logos);

            this.pingServers = this.settings.UI.PingServers;

            WebRequest.DefaultWebProxy = null; // Avoid possible lag due to .NET trying to resolve non-existing proxy.

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();
            
            this.view.StartApplication();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            JsonFetcher jsonFetch = new JsonFetcher(new NLogWrapper("FOQuery"));
            FOJsonDeserializer jsonDeserialize = new FOJsonDeserializer();

            this.view.UpdateStatusBar("Downloading data...");

            var jsonNode = jsonFetch.DownloadJson(this.settings.InstallUrl);
            this.installHandler = new InstallHandler(jsonDeserialize.GetInstallData(jsonNode), settings.Games, settings.Dependencies);
            this.logger.Debug("Installhandler initialized.");
            this.logoManager = new LogoManager(this.settings.Paths.Logos, this.settings.LogoUrl);
            this.logger.Debug("Logos initialized.");
            this.serverManager = new ServerManager(
                new FOServerJson(settings.ConfigUrl, settings.StatusUrl, settings.CombinedUrl, new NLogWrapper("FOQuery")),
                this.installHandler);
            this.logger.Debug("Servermanager initialized.");
            this.VerifyInstalledGames();
            this.logger.Debug("Verified installed games.");
            this.view.UpdateStatusBar("Updating game list...");
            this.UpdateGameList();
            this.logger.Debug("Games updated.");
        }

        private bool AddGame(FOGameInfo game)
        {
            if (!this.installHandler.HasInstallInfo(game.Id))
            {
                this.view.ShowError(string.Format("No install info available for {0} :( {1} If you want to install this game anyway, navigate to the webpage ({2}) instead.",
                    game.Name,
                    Environment.NewLine,
                    game.Website));
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
                if (installHandler.GetInstallScriptInfo(game.Id) == null)
                {
                    this.view.ShowError(string.Format("No install script available for {0} :( {1} If you want to install this game anyway, download and install the game manually from {2} and then add the install directory.",
                    game.Name,
                    Environment.NewLine,
                    game.Website));
                    return false;
                }

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
            SaveInstalledStatus();
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
                this.view.RefreshServerList(servers, true);
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

        private void SaveInstalledStatus()
        {
            this.settings.Games = this.installHandler.GetInstalledGames();
            SettingsManager.SaveSettings(this.settings);
        }

        public void VerifyInstalledGames()
        {
            List<FOGameInfo> servers = this.serverManager.GetServers(false);
            List<InstalledGame> removeGames = new List<InstalledGame>();

            List<InstalledGame> installedGames = this.installHandler.GetInstalledGames();
            if(installedGames == null) 
            {
                this.logger.Info("No games installed to verify, returning.");    
                return;
            }

            foreach (InstalledGame installGame in installedGames)
            {
                if(installGame.IgnoreInvalid)
                    continue;

                FOGameInfo game = servers.Where(x => x.Id == installGame.Id).FirstOrDefault();

                string installPath = installHandler.GetInstallPath(game.Id);
                foreach(string file in installHandler.GetGameFiles(game.Id))
                {
                    string filePath = Path.Combine(installGame.Path, file);

                    if(!File.Exists(filePath))
                    {
                        if (this.view.AskYesNoQuestion(string.Format("Unable to find {0} for {1}, remove {1}?", filePath, game.Name), "Remove game?"))
                        {
                            removeGames.Add(installGame);
                        }
                        else
                        {
                            installGame.IgnoreInvalid = true;
                        }
                        
                        break;
                    }
                }
            }
            foreach (var game in removeGames)
            {
                this.installHandler.RemoveInstalledGame(game.Id);
            }

            SaveInstalledStatus();
            this.view.RefreshServerList(serverManager.GetServers(!this.showOffline), this.pingServers);
        }

        public bool UpdateGameList()
        {
            this.view.UpdateStatusBar("Updating gamelist...");
            try
            {
                serverManager.UpdateStatus();

                var servers = serverManager.GetServers(!this.showOffline);

                var online = servers.Where(x => !x.Status.IsOffline());
                this.SetOnlineTitleInfo(online.Sum(x => x.Status.Players), online.Count());

                if(this.pingServers)
                    this.PingServers(servers.ToList());

                this.view.RefreshServerList(servers, pingServers);
                
                this.view.UpdateStatusBar("Updated gamelist.");

                return true;
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
            return false;
        }
    }
}
