namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FOQuery.Data;
    using PlayFOnline.Core;
    using PlayFOnline.Scripts;
    using PlayFOnline.Forms;
    using System.Net;

    public class InstallHandler
    {
        private Dictionary<string, FOGameInstallInfo> installInfo;
        private List<InstalledGame> installedGames { get; set; }
        private List<FOGameDependency> dependencies { get; set; }
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string installError;

        public InstallHandler(
            Dictionary<string, FOGameInstallInfo> installInfo, 
            List<InstalledGame> games,
            List<FOGameDependency> dependencies)
        {
            this.installInfo = installInfo;
            this.installedGames = games;
            this.dependencies = dependencies;
        }

        public List<InstalledGame> GetInstalledGames() { return this.installedGames; }
        public string GetInstallError() { return installError; }

        public List<FOGameDependency> GetDependencies(string gameId)
        {
            return this.installInfo[gameId].Dependencies;
        }

        public FOScriptInfo GetInstallScriptInfo(string gameId)
        {
            return this.installInfo[gameId].InstallScript;
        }

        public List<FOGameLaunchProgram> GetLaunchPrograms(string gameId)
        {
            return this.installInfo[gameId].LaunchPrograms;
        }

        public List<string> GetGameFiles(string gameId)
        {
            return this.installInfo[gameId].Files;
        }

        public bool HasInstallInfo(string gameId)
        {
            return this.installInfo.ContainsKey(gameId);
        }

        public bool VerifyGameFolderPath(string gameId, string path)
        {
            this.logger.Debug("Verifying path {0} for {1}.", gameId, path);
            if (!this.installInfo.ContainsKey(gameId))
            {
                this.logger.Error("No install data available for {0}", gameId);
                return false;
            }

            foreach (string file in this.installInfo[gameId].Files)
            {
                if (!File.Exists(path + "/" + file))
                {
                    this.logger.Error("File {0} for {1} not found. Invalid installation.", file, gameId);
                    return false;
                }
            }
            return true;
        }

        public void AddDependency(FOGameDependency depend, string Path)
        {
            FOGameDependency newDepend = depend;
            newDepend.Path = Path;
            if (this.dependencies == null) this.dependencies = new List<FOGameDependency>();
            this.dependencies.Add(newDepend);
        }

        public string GetInstallPath(string id)
        {
            return this.installedGames.Find(x => x.Id == id).Path;
        }

        public bool HasDependency(string name)
        {
            if (this.dependencies == null) return false;
            return this.dependencies.Exists(x => x.Name == name);
        }

        private void DownloadInstallScript(string url, string localPath)
        {
            this.logger.Info("Downloading {0} to {1}", url, localPath);
            WebClient webClient = new WebClient();
            webClient.DownloadFile(url, localPath);
            this.logger.Info("File downloaded.");
        }

        public bool InstallGame(FOGameInfo game, string scriptPath, string tempPath, string installPath, List<string> dependencyPaths, bool reviewCode)
        {
            // Fetch and run install script
            FOScriptInfo installScriptInfo = this.GetInstallScriptInfo(game.Id);

            string scriptName = Utils.GetFilenameFromUrl(installScriptInfo.Url);
            string localScriptPath = Path.Combine(scriptPath, scriptName);

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
                this.logger.Info("{0} doesn't exist", localScriptPath);
                this.DownloadInstallScript(installScriptInfo.Url, localScriptPath);
            }

            if (!File.Exists(localScriptPath))
            {
                this.installError = string.Format("Failed to download {0} to {1}", installScriptInfo.Url, localScriptPath);
                return false;
            }

            if (reviewCode)
            {
                string[] code = File.ReadAllLines(localScriptPath);
                frmReviewCode review = new frmReviewCode(string.Join("\r\n",code));
                review.ShowDialog();
                if (review.Cancelled)
                {
                    this.installError = "Cancelled during script review.";
                    return false;
                }
            }


            this.logger.Debug("Running install script for {0}, temp path: {1}, install path: {2}", game.Name, tempPath, installPath);
            InstallHost installHost = new InstallHost();
            if (!installHost.RunInstallScript(localScriptPath, game.Name, tempPath, installPath))
            {
                this.logger.Error("Installation failed.");
                return false;
            }

            // Copy dependencies
            foreach (string path in dependencyPaths)
            {
                string filename = Path.GetFileName(path);
                string destPath = Path.Combine(installPath, filename);
                if (File.Exists(destPath))
                {
                    this.logger.Info("{0} already exists, skipping copy", destPath);
                    continue;
                }

                this.logger.Info("Copying {0} to {1}...", path, destPath);
                File.Copy(path, destPath);
                this.logger.Info("Copied {0} to {1}", path, destPath);
            }
            this.logger.Info("Adding installed game {0}, path {1}", game.Id, installPath);
            this.AddInstalledGame(game.Id, installPath);

            return true;
        }

        public void AddInstalledGame(string id, string path)
        {
            InstalledGame installedGame = new InstalledGame();
            installedGame.Id = id;
            installedGame.Path = path;
            if (this.installedGames == null) this.installedGames = new List<InstalledGame>();
            this.installedGames.Add(installedGame);
        }

        public void RemoveInstalledGame(string gameId)
        {
            this.installedGames.Remove(this.installedGames.Find(x => x.Id == gameId));
        }


        public bool IsInstalled(string id)
        {
            if (this.installedGames == null) return false;
            return this.installedGames.Exists(x => x.Id == id);
        }
    }
}