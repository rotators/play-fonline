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
using PlayFOnline.Scripts;

namespace PlayFOnline
{
    public partial class frmMain : Form
    {
        private FOSettings settings;
        private FOServerQuery query;
        private InstallHandler installHandler;
        private LogoManager logoManager;
        private FOGameInfo currentGame;

        private Logger logger = LogManager.GetLogger("UI::Main");

        #region frmMain handlers

        public frmMain()
        {
            InitializeComponent();
            LoadStuff();
        }

        private void chkShowOffline_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGameList();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Exit();
        }

        private void lstGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            FOGameInfo Game = (FOGameInfo)lstGames.SelectedObject;
            if (Game == null) return;
            SelectGame(Game);
        }

        private void btnLaunchProgram_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(currentGame.InstallPath + Path.DirectorySeparatorChar + btn.Tag);
            process.Start();
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            FOGameInfo Game = (FOGameInfo)lstGames.SelectedObject;
            if (Game == null)
                MessageBox.Show("No game selected!");
            else
                AddGame(Game);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateGameList();
        }

        private void lstGames_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            FOGameInfo Game = (FOGameInfo)e.Model;
            if (String.IsNullOrEmpty(Game.InstallPath))
            {
                e.Item.ForeColor = Color.Gray;
            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
        }

        private void linkFoDev_MouseClick(object sender, MouseEventArgs e)
        {
            Process.Start("http://fodev.net");
        }

        #endregion frmMain handlers

        private void setTitle(int players = -1, int servers = -1)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            this.Text = string.Format("Play FOnline {0}.{1}.{2}", assemblyName.Version.Major, assemblyName.Version.Minor, assemblyName.Version.Build);
            if (players != -1)
            {
                this.Text += " - " + players + " players online on " + servers + " servers";
            }
        }

        private void LoadStuff()
        {
            settings = SettingsManager.LoadSettings();
            logger.Info("Loading config {0}", SettingsManager.path);

            WebRequest.DefaultWebProxy = null; // Avoid possible lag due to .NET trying to resolve non-existing proxy.

            setTitle();

            if (settings.UI == null)
            {
                this.DesktopLocation = new Point(settings.UI.x, settings.UI.y);
                this.Width = settings.UI.width;
                this.Height = settings.UI.height;
            }

            if (settings.Paths == null)
            {
                string baseDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
                settings.Paths = new PathSettings();
                settings.Paths.scripts = baseDir + Path.DirectorySeparatorChar + "scripts";
                settings.Paths.downloadTemp = baseDir + Path.DirectorySeparatorChar + "temp";
            }

            UpdateGameList();

            JsonFetcher jsonFetch = new JsonFetcher();
            JObject o = jsonFetch.downloadJson(settings.installURL);
            installHandler = new InstallHandler(JsonConvert.DeserializeObject<Dictionary<String, FOGameInstallInfo>>(o["fonline"]["install-data"].ToString()));

            logoManager = new LogoManager("./logos", "http://fodev.net/status/json/logo/");

            this.olvInstallPath.AspectToStringConverter = delegate(object x)
            {
                string Path = (string)x;
                if (String.IsNullOrWhiteSpace(Path))
                    return "Not installed/added";
                return Path;
            };
        }

        private void Exit()
        {
            settings.UI = new UISettings();
            settings.UI.x = this.DesktopLocation.X;
            settings.UI.y = this.DesktopLocation.Y;
            settings.UI.width = this.Width;
            settings.UI.height = this.Height;

            SettingsManager.SaveSettings(settings);
            Environment.Exit(0);
        }

        private void UpdateStatusBar(string text)
        {
            statusBarLabel.Text = DateTime.Now + " | " + text;
            logger.Info(text);
        }

        private void UpdateGameList()
        {
            UpdateStatusBar("Updating gamelist...");
            try
            {
                query = new FOServerQuery(settings.configURL, settings.statusURL);
                List<FOGameInfo> servers;

                if (chkShowOffline.Checked)
                    servers = query.GetServers(true);
                else
                    servers = query.GetServers(true).Where(x => !x.Status.IsOffline() || settings.IsInstalled(x.Id)).ToList(); // Always add installed, even if offline

                servers.Where(x => settings.IsInstalled(x.Id)).ToList().ForEach(x => x.InstallPath = settings.GetInstallPath(x.Id));

                var online = servers.Where(x => !x.Status.IsOffline());
                setTitle(online.Sum(x => x.Status.Players), online.Count());

                lstGames.SetObjects(servers);
                lstGames.Refresh();
                UpdateStatusBar("Updated gamelist.");

                return;
            }
            catch (WebException e)
            {
                MessageBox.Show(String.Format("Unable to download {0}: {1}", e.Response.ResponseUri, e.Message));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + "Please see log.txt for more information.");
            }
            UpdateStatusBar("Failed to update gamelist.");
        }

        private void SelectGame(FOGameInfo Game)
        {
            bool installed = !String.IsNullOrWhiteSpace(Game.InstallPath);

            btnInstall.Enabled = !installed;
            btnOptions.Enabled = installed;

            var controls = flowMenu.Controls;

            flowMenu.Controls.Clear();
            flowMenu.Update();
            if (installed)
            {
                foreach (FOGameLaunchProgram program in installHandler.GetLaunchPrograms(Game.Id))
                {
                    Button btnCustom = new Button();
                    btnCustom.Text = program.Name;
                    btnCustom.Width = btnInstall.Width;
                    btnCustom.Height = btnInstall.Height;
                    btnCustom.Tag = program.File;
                    btnCustom.Click += btnLaunchProgram_Click;
                    flowMenu.Controls.Add(btnCustom);
                }
            }
            else
                flowMenu.Controls.Add(btnInstall);
            flowMenu.Controls.Add(btnAbout);

            currentGame = Game;
        }

        private void MessageBoxError(string Message)
        {
            MessageBox.Show(Message);
            logger.Error(Message);
        }

        private bool DownloadInstallScript(string url, string localPath)
        {
            WebClient webClient = new WebClient();
            webClient.Proxy = null;
            webClient.DownloadFile(url, localPath);
            return true;
        }

        private bool InstallGame(FOGameInfo Game)
        {
            // Check for dependencies and handle them...
            foreach (FOGameDependency depend in installHandler.GetDependencies(Game.Id))
            {
                if (!settings.HasDependency(depend.Name))
                {
                    if (MessageBox.Show(depend.Description + " is required to run this game. " + Environment.NewLine + "Do you have it?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
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
                                            string fullPath = Path.Combine(settings.Paths.scripts, Utils.GetFilenameFromUrl(depend.Script.Url));

                                            if (!File.Exists(fullPath))
                                            {
                                                webClient.Proxy = null;
                                                webClient.DownloadFile(depend.Script.Url, fullPath);
                                            }
                                            depend.Script.Path = fullPath;
                                        }
                                        catch (WebException ex)
                                        {
                                            MessageBoxError("Failed to download " + depend.Script.Url + ":" + ex.Message);
                                            return false;
                                        }
                                    }
                                }
                                else
                                    MessageBoxError("No script available for verifying dependency " + depend.Name);
                            }

                            ResolveHost resolveHost = new ResolveHost();
                            bool chooseNew = false;
                            do
                            {
                                if (chooseNew) OpenFile.ShowDialog();
                                chooseNew = false;
                                if (!resolveHost.RunResolveScript(depend.Script.Path, depend.Name, OpenFile.FileName))
                                {
                                    chooseNew = (MessageBox.Show(OpenFile.FileName + " doesn't seem to be a valid file for " + depend.Name + ", do you want to use it anyway?", "Play FOnline",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No);
                                }
                            } while (chooseNew);
                            settings.AddDependency(depend, OpenFile.FileName);
                        }
                    }
                    else
                        return false;
                }
            }

            // Fetch and run install script
            FOScriptInfo installScriptInfo = installHandler.GetInstallScriptInfo(Game.Id);
            string scriptName = Utils.GetFilenameFromUrl(installScriptInfo.Url);
            string localScriptPath = Path.Combine(settings.Paths.scripts, scriptName);

            // File exists, verify if checksum is the same...
            if (File.Exists(localScriptPath))
            {
                string localChecksum = Utils.GetSHA1Checksum(localScriptPath);
                if (localChecksum != installScriptInfo.Checksum)
                {
                    logger.Info("Local checksum of {0} = {1}, remote is {2}. More recent script is available or local file has been modified."
                        , localScriptPath, localChecksum, installScriptInfo.Checksum);

                    DownloadInstallScript(installScriptInfo.Url, localScriptPath);
                }
            }
            else
            {
                if (!DownloadInstallScript(installScriptInfo.Url, localScriptPath))
                    return false;
            }

            if (!File.Exists(localScriptPath))
            {
                MessageBoxError(String.Format("Failed to download {0} to {1}", installScriptInfo.Url, localScriptPath));
            }

            InstallHost installHost = new InstallHost();
            installHost.RunInstallScript(localScriptPath, Game.Name, settings.Paths.downloadTemp, Game.InstallPath);

            // Copy dependencies
            foreach (FOGameDependency installDepend in settings.Dependencies)
            {
                string filename = Path.GetFileName(installDepend.Path);
                string destPath = Game.InstallPath + Path.DirectorySeparatorChar + filename;
                if (File.Exists(destPath))
                    logger.Info("{0} already exists, skipping copy", destPath);

                logger.Info("Copying {0} to {1}...", installDepend.Path, destPath);
                File.Copy(installDepend.Path, destPath);
                logger.Info("Copied {0} to {1}", installDepend.Path, destPath);
            }
            return true;
        }

        private bool AddGame(FOGameInfo Game)
        {
            if (!installHandler.HasInstallInfo(Game.Id))
            {
                MessageBoxError("No install info available for " + Game.Name + " :(" + Environment.NewLine + "Please report this, so that it can be implemented.");
                return false;
            }

            DialogResult Result = MessageBox.Show("Is " + Game.Name + " already installed on your computer?" + Environment.NewLine + "If this is the case, the installed directory can be added directly. This assumes that the game is working in its current state.", Game.Name + " already installed?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                FolderBrowserDialog FolderBrowser = new FolderBrowserDialog();
                FolderBrowser.ShowNewFolderButton = false;
                if (FolderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return false;

                if (!installHandler.VerifyGameFolderPath(Game.Id, FolderBrowser.SelectedPath))
                {
                    MessageBox.Show(FolderBrowser.SelectedPath + " does not contain a valid " + Game.Name + " installation.");
                    return false;
                }

                Game.InstallPath = FolderBrowser.SelectedPath;

                string msg = "Successfully addded " + Game.Name + "!";
                MessageBox.Show(msg);
                logger.Info(msg);
            }
            else if (Result == System.Windows.Forms.DialogResult.No)
            {
                frmInstallMain installMain = new frmInstallMain(Game, installHandler, logoManager, settings.Paths.scripts);
                installMain.ShowDialog();

                if (!installMain.IsSuccess)
                    return false;

                string msg = "Successfully installed " + Game.Name + "!";
                MessageBox.Show(msg);
                logger.Info(msg);
            }
            else
                return false;

            settings.InstalledGame(Game.Id, Game.InstallPath);
            SettingsManager.SaveSettings(settings);
            return true;
        }
    }
}