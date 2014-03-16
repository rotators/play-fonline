using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using PlayFO.Scripts;
using NLog;


namespace PlayFO
{
    public partial class frmMain : Form
    {
        FOSettings settings;
        FOServerQuery query;
        InstallHandler installHandler;
        FOGameInfo currentGame;

        Logger logger = LogManager.GetLogger("UI::Main");

        public frmMain()
        {
            InitializeComponent();

            settings = SettingsManager.LoadSettings();
            logger.Info("Loading config {0}", SettingsManager.path);

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
            installHandler = new InstallHandler(JsonConvert.DeserializeObject <Dictionary<String, FOGameInstallInfo>>(o["fonline"]["install-data"].ToString()));

            this.olvInstallPath.AspectToStringConverter = delegate(object x)
            {
                string Path = (string)x;
                if (String.IsNullOrWhiteSpace(Path))
                    return "Not installed/added";
                return Path;
            };

            this.olvPlayers.AspectToStringConverter = delegate(object x)
            {
                int Players = (int)x;
                if (Players == -1)
                    return "Offline";
                return Players.ToString();
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
                
                if(chkShowOffline.Checked)
                    servers = query.GetServers(true);
                else
                    servers = query.GetOnlineServers();

                foreach (FOGameInfo server in servers)
                {
                    if (settings.IsInstalled(server.Id))
                        server.InstallPath = settings.GetInstallPath(server.Id);
                }

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

        private void chkShowOffline_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowOffline.Checked)
                lstGames.SetObjects(query.GetServers(true));
            else
                lstGames.SetObjects(query.GetOnlineServers());
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Exit();
        }

        private void lstGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            FOGameInfo Game = (FOGameInfo)lstGames.SelectedObject;
            if (Game == null) return;

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

        private void btnLaunchProgram_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(currentGame.InstallPath + Path.DirectorySeparatorChar + btn.Tag);
            process.Start();
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            FOGameInfo Game = (FOGameInfo) lstGames.SelectedObject;
            if(Game == null)
            {
                MessageBox.Show("No game selected!");
            }

            DialogResult Result = MessageBox.Show("Is this game already installed on your computer?" + Environment.NewLine + "If this is the case, the installed directory can be added directly.", "Game already installed?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                FolderBrowserDialog FolderBrowser = new FolderBrowserDialog();
                FolderBrowser.ShowNewFolderButton = false;
                if (FolderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return;

                if (!installHandler.VerifyGameFolderPath(Game.Id, FolderBrowser.SelectedPath))
                {
                    MessageBox.Show(FolderBrowser.SelectedPath + " does not contain a valid " + Game.Name + " installation.");
                    return;
                }

                foreach (FOGameDependency depend in installHandler.GetDependencies(Game.Id))
                {
                    if (!settings.HasDependency(depend.Name))
                    {
                        if (MessageBox.Show(depend.Description + " is required to run this game. " + Environment.NewLine + "Do you have it?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                        {
                            OpenFileDialog OpenFile = new OpenFileDialog();
                            OpenFile.Filter = depend.Name + "|*.*";
                            if (OpenFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                                return;
                            if (OpenFile.CheckFileExists)
                            {
                                if (string.IsNullOrEmpty(depend.ScriptPath))
                                {
                                    if (!string.IsNullOrEmpty(depend.ScriptUrl))
                                    {
                                        using (var webClient = new System.Net.WebClient())
                                        {
                                            try
                                            {
                                                Uri uri = new Uri(depend.ScriptUrl);
                                                string scriptName = uri.Segments[uri.Segments.Length - 1];
                                                string fullPath = settings.Paths.scripts + Path.DirectorySeparatorChar + scriptName;

                                                if(!File.Exists(fullPath))
                                                {
                                                    webClient.Proxy = null;
                                                    webClient.DownloadFile(depend.ScriptUrl, fullPath);
                                                }
                                                depend.ScriptPath = fullPath;
                                            }
                                            catch(WebException ex)
                                            {
                                                MessageBox.Show("Failed to download " + depend.ScriptUrl + ":" + ex.Message);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                        MessageBox.Show("No script available for verifying dependency " + depend.Name);
                                }

                                ResolveHost resolveHost = new ResolveHost();
                                bool chooseNew = false;
                                do
                                {
                                    if (chooseNew) OpenFile.ShowDialog();
                                    chooseNew = false;
                                    if (!resolveHost.RunResolveScript(depend.ScriptPath, depend.Name, OpenFile.FileName))
                                    {
                                        chooseNew = (MessageBox.Show(OpenFile.FileName + " doesn't seem to be a valid file for " + depend.Name + ", do you want to use it anyway?", "Play FOnline",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No);
                                    }
                                    
                                } while(chooseNew);
                                settings.AddDependency(depend, OpenFile.FileName);
                            }
                        }
                        else
                            return;
                    }
                }

                Game.InstallPath = FolderBrowser.SelectedPath;

                MessageBox.Show("Successfully added " + Game.Name + "!");
                settings.InstalledGame(Game.Id, Game.InstallPath);
                SettingsManager.SaveSettings(settings);
            }
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
            //if (Game.Status.Players == -1)
            //    e.Item.BackColor = Color.OrangeRed;
        }
    }
}
