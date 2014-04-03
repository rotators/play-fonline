namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Windows.Forms;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NLog;
    using PlayFOnline.Data;
    using PlayFOnline.Scripts;

    public partial class frmMain : Form
    {
        private FOGameInfo currentGame;
        private InstallHandler installHandler;
        private Logger logger = LogManager.GetLogger("UI::Main");
        private LogoManager logoManager;
        private FOSettings settings;

        delegate void UpdateDelegate();

        #region frmMain handlers

        public frmMain()
        {
            this.InitializeComponent();
            this.LoadStuff();
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            FOGameInfo game = (FOGameInfo)lstGames.SelectedObject;
            if (game == null)
                MessageBox.Show("No game selected!");
            else
                this.AddGame(game);
        }

        private void btnLaunchProgram_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(this.currentGame.InstallPath + Path.DirectorySeparatorChar + btn.Tag);
            process.Start();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.UpdateGameList();
        }

        private void chkShowOffline_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateGameList();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Exit();
        }

        private void linkFoDev_MouseClick(object sender, MouseEventArgs e)
        {
            Process.Start("http://fodev.net");
        }

        private void lstGames_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            FOGameInfo game = (FOGameInfo)e.Model;
            if (string.IsNullOrEmpty(game.InstallPath))
            {
                e.Item.ForeColor = Color.Gray;
            }
        }

        private void lstGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            FOGameInfo game = (FOGameInfo)lstGames.SelectedObject;
            if (game == null) return;
            this.SelectGame(game);
        }
        #endregion frmMain handlers

        private bool AddGame(FOGameInfo game)
        {
            if (!this.installHandler.HasInstallInfo(game.Id))
            {
                this.MessageBoxError("No install info available for " + game.Name + " :(" + Environment.NewLine + "Please report this, so that it can be implemented.");
                return false;
            }

            DialogResult result = MessageBox.Show("Is " + game.Name + " already installed on your computer?" + Environment.NewLine + "If this is the case, the installed directory can be added directly. This assumes that the game is working in its current state.", game.Name + " already installed?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                folderBrowser.ShowNewFolderButton = false;
                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return false;

                if (!this.installHandler.VerifyGameFolderPath(game.Id, folderBrowser.SelectedPath))
                {
                    MessageBox.Show(folderBrowser.SelectedPath + " does not contain a valid " + game.Name + " installation.");
                    return false;
                }

                game.InstallPath = folderBrowser.SelectedPath;

                string msg = "Successfully addded " + game.Name + "!";
                MessageBox.Show(msg);
                this.logger.Info(msg);
            }
            else if (result == System.Windows.Forms.DialogResult.No)
            {
                frmInstallMain installMain = new frmInstallMain(game, this.installHandler, this.logoManager, this.settings.Paths.Scripts);
                installMain.ShowDialog();

                if (!installMain.IsSuccess)
                    return false;

                string msg = "Successfully installed " + game.Name + "!";
                MessageBox.Show(msg);
                this.logger.Info(msg);
            }
            else
                return false;

            this.settings.InstalledGame(game.Id, game.InstallPath);
            SettingsManager.SaveSettings(this.settings);
            return true;
        }

        private bool DownloadInstallScript(string url, string localPath)
        {
            WebClient webClient = new WebClient();
            webClient.Proxy = null;
            webClient.DownloadFile(url, localPath);
            return true;
        }

        private void Exit()
        {
            this.settings.UI = new UISettings();
            this.settings.UI.X = this.DesktopLocation.X;
            this.settings.UI.Y = this.DesktopLocation.Y;
            this.settings.UI.Width = this.Width;
            this.settings.UI.Height = this.Height;

            SettingsManager.SaveSettings(this.settings);
            Environment.Exit(0);
        }

        private bool InstallGame(FOGameInfo game)
        {
            // Check for dependencies and handle them...
            foreach (FOGameDependency depend in this.installHandler.GetDependencies(game.Id))
            {
                if (!this.settings.HasDependency(depend.Name))
                {
                    if (MessageBox.Show(depend.Description + " is required to run this game. " + Environment.NewLine + "Do you have it?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                    {
                        OpenFileDialog OpenFile = new OpenFileDialog();
                        OpenFile.Filter = depend.Name + "|*.*";
                        if (OpenFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                            return false;
                        if (OpenFile.CheckFileExists)
                        {
                            if (string.IsNullOrEmpty(depend.Script.Path))
                            {
                                if (!string.IsNullOrEmpty(depend.Script.Url))
                                {
                                    using (var webClient = new System.Net.WebClient())
                                    {
                                        try
                                        {
                                            string fullPath = Path.Combine(this.settings.Paths.Scripts, Utils.GetFilenameFromUrl(depend.Script.Url));

                                            if (!File.Exists(fullPath))
                                            {
                                                webClient.Proxy = null;
                                                webClient.DownloadFile(depend.Script.Url, fullPath);
                                            }
                                            depend.Script.Path = fullPath;
                                        }
                                        catch (WebException ex)
                                        {
                                            this.MessageBoxError("Failed to download " + depend.Script.Url + ":" + ex.Message);
                                            return false;
                                        }
                                    }
                                }
                                else
                                    this.MessageBoxError("No script available for verifying dependency " + depend.Name);
                            }

                            ResolveHost resolveHost = new ResolveHost();
                            bool chooseNew = false;
                            do
                            {
                                if (chooseNew) OpenFile.ShowDialog();
                                chooseNew = false;
                                if (!resolveHost.RunResolveScript(depend.Script.Path, depend.Name, OpenFile.FileName))
                                {
                                    chooseNew = MessageBox.Show(
                                        OpenFile.FileName + " doesn't seem to be a valid file for " + depend.Name + ", do you want to use it anyway?",
                                        "Play FOnline",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No;
                                }
                            } 
                            while (chooseNew);

                            this.settings.AddDependency(depend, OpenFile.FileName);
                        }
                    }
                    else
                        return false;
                }
            }

            // Fetch and run install script
            FOScriptInfo installScriptInfo = this.installHandler.GetInstallScriptInfo(game.Id);
            string scriptName = Utils.GetFilenameFromUrl(installScriptInfo.Url);
            string localScriptPath = Path.Combine(this.settings.Paths.Scripts, scriptName);

            // File exists, verify if checksum is the same...
            if (File.Exists(localScriptPath))
            {
                string localChecksum = Utils.GetSHA1Checksum(localScriptPath);
                if (localChecksum != installScriptInfo.Checksum)
                {
                    this.logger.Info("Local checksum of {0} = {1}, remote is {2}. More recent script is available or local file has been modified.", localScriptPath, localChecksum, installScriptInfo.Checksum);

                    this.DownloadInstallScript(installScriptInfo.Url, localScriptPath);
                }
            }
            else
            {
                if (!this.DownloadInstallScript(installScriptInfo.Url, localScriptPath))
                    return false;
            }

            if (!File.Exists(localScriptPath))
            {
                this.MessageBoxError(string.Format("Failed to download {0} to {1}", installScriptInfo.Url, localScriptPath));
            }

            InstallHost installHost = new InstallHost();
            installHost.RunInstallScript(localScriptPath, game.Name, this.settings.Paths.DownloadTemp, game.InstallPath);

            // Copy dependencies
            foreach (FOGameDependency installDepend in this.settings.Dependencies)
            {
                string filename = Path.GetFileName(installDepend.Path);
                string destPath = game.InstallPath + Path.DirectorySeparatorChar + filename;
                if (File.Exists(destPath))
                    this.logger.Info("{0} already exists, skipping copy", destPath);

                this.logger.Info("Copying {0} to {1}...", installDepend.Path, destPath);
                File.Copy(installDepend.Path, destPath);
                this.logger.Info("Copied {0} to {1}", installDepend.Path, destPath);
            }
            return true;
        }

        private void LoadStuff()
        {


            this.settings = SettingsManager.LoadSettings();
            this.logger.Info("Loading config {0}", SettingsManager.SettingsPath);

            WebRequest.DefaultWebProxy = null; // Avoid possible lag due to .NET trying to resolve non-existing proxy.

            this.SetTitle();

            if (this.settings.UI == null)
            {
                this.DesktopLocation = new Point(this.settings.UI.X, this.settings.UI.Y);
                this.Width = this.settings.UI.Width;
                this.Height = this.settings.UI.Height;
            }

            if (this.settings.Paths == null)
            {
                string baseDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
                this.settings.Paths = new PathSettings();
                this.settings.Paths.Scripts = baseDir + Path.DirectorySeparatorChar + "scripts";
                this.settings.Paths.DownloadTemp = baseDir + Path.DirectorySeparatorChar + "temp";
            }

            this.UpdateGameList();

            JsonFetcher jsonFetch = new JsonFetcher();
            JObject o = jsonFetch.DownloadJson(this.settings.InstallUrl);
            this.installHandler = new InstallHandler(JsonConvert.DeserializeObject<Dictionary<string, FOGameInstallInfo>>(o["fonline"]["install-data"].ToString()));

            this.logoManager = new LogoManager("./logos", "http://fodev.net/status/json/logo/");

            this.olvInstallPath.AspectToStringConverter = delegate(object x)
            {
                string path = (string)x;
                MessageBox.Show(path);
                if (string.IsNullOrWhiteSpace(path))
                    return "Not installed/added";
                return path;
            };

            this.olvPing.AspectToStringConverter = delegate(object x)
            {
                int latency = (int)x;
                if (latency != int.MaxValue)
                    return string.Format("{0} ms", latency);
                return string.Empty;
            };

            // Causes BSOD - https://connect.microsoft.com/VisualStudio/feedback/details/721557/bluescreen-process-has-locked-pages-netframework-ping-send
            /*Thread pingThread = new System.Threading.Thread(delegate()
            {
                while (true)
                {
                    this.Invoke((MethodInvoker)delegate { this.PingServers(); });
                    Thread.Sleep(5000);
                }
            });
            pingThread.Start();*/
        }

        private void PingServers()
        {
            var servers = this.GetServers();
            Action refreshDisplay = delegate()
            {
                this.RefreshServerList(servers);
            };
            servers.Where(x => !x.Status.IsOffline()).ToList().ForEach(x => x.Ping(refreshDisplay));
        }

        private void MessageBoxError(string message)
        {
            MessageBox.Show(message);
            this.logger.Error(message);
        }

        private void SelectGame(FOGameInfo game)
        {
            bool installed = !string.IsNullOrWhiteSpace(game.InstallPath);

            this.btnInstall.Enabled = !installed;
            this.btnOptions.Enabled = installed;

            var controls = this.flowMenu.Controls;

            this.flowMenu.Controls.Clear();
            this.flowMenu.Update();
            if (installed)
            {
                foreach (FOGameLaunchProgram program in this.installHandler.GetLaunchPrograms(game.Id))
                {
                    Button btnCustom = new Button();
                    btnCustom.Text = program.Name;
                    btnCustom.Width = this.btnInstall.Width;
                    btnCustom.Height = this.btnInstall.Height;
                    btnCustom.Tag = program.File;
                    btnCustom.Click += this.btnLaunchProgram_Click;
                    this.flowMenu.Controls.Add(btnCustom);
                }
            }
            else
                this.flowMenu.Controls.Add(this.btnInstall);
            //this.flowMenu.Controls.Add(this.btnAbout);

            this.currentGame = game;
        }

        private void SetTitle(int players = -1, int servers = -1)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            this.Text = string.Format("Play FOnline {0}.{1}.{2}", assemblyName.Version.Major, assemblyName.Version.Minor, assemblyName.Version.Build);
            if (players != -1)
            {
                this.Text += string.Format(" - {0} players online on {1} servers", players, servers);
            }
        }

        private List<FOGameInfo> GetServers()
        {
            List<FOGameInfo> servers;

            FOServerQuery query = new FOServerQuery(this.settings.ConfigUrl, this.settings.StatusUrl);
            servers = query.GetServers(true);

            if (!this.chkShowOffline.Checked)
                servers = servers.Where(x => !x.Status.IsOffline() || this.settings.IsInstalled(x.Id)).ToList(); // Always add installed, even if offline
            return servers;
        }

        private void RefreshServerList(List<FOGameInfo> servers)
        {
            lstGames.SetObjects(servers);
            lstGames.Refresh();
        }

        private void UpdateGameList()
        {
            this.UpdateStatusBar("Updating gamelist...");
            try
            {
                var servers = this.GetServers();
                servers.Where(x => this.settings.IsInstalled(x.Id)).ToList().ForEach(x => x.InstallPath = this.settings.GetInstallPath(x.Id));

                var online = servers.Where(x => !x.Status.IsOffline());
                this.SetTitle(online.Sum(x => x.Status.Players), online.Count());

                this.PingServers();

                this.RefreshServerList(servers);
                this.UpdateStatusBar("Updated gamelist.");

                return;
            }
            catch (WebException e)
            {
                MessageBox.Show(string.Format("Unable to download {0}: {1}", e.Response.ResponseUri, e.Message));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + "Please see log for more information.");
            }
            this.UpdateStatusBar("Failed to update gamelist.");
        }

        private void UpdateStatusBar(string text)
        {
            statusBarLabel.Text = DateTime.Now + " | " + text;
            this.logger.Info(text);
        }
    }
}