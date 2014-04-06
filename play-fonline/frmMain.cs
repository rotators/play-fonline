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
        public frmMain()
        {
            this.InitializeComponent();
        }

        /*private bool DownloadInstallScript(string url, string localPath)
        {
            WebClient webClient = new WebClient();
            webClient.Proxy = null;
            webClient.DownloadFile(url, localPath);
            return true;
        }*/

        /*private bool InstallGame(FOGameInfo game)
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
        }*/
    }
}