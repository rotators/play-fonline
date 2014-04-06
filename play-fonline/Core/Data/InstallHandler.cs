namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using PlayFOnline.Data;
    using PlayFOnline.Core;

    public class InstallHandler
    {
        private Dictionary<string, FOGameInstallInfo> installInfo;
        private List<InstalledGame> installedGames { get; set; }
        private List<FOGameDependency> dependencies { get; set; }
        private NLog.Logger logger = NLog.LogManager.GetLogger("InstallHandler");

        public InstallHandler(
            Dictionary<string, FOGameInstallInfo> installInfo, 
            List<InstalledGame> games,
            List<FOGameDependency> dependencies)
        {
            this.installInfo = installInfo;
            this.installedGames = games;
            this.dependencies = dependencies;
        }

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

        public bool HasInstallInfo(string gameId)
        {
            return this.installInfo.ContainsKey(gameId);
        }

        public bool VerifyGameFolderPath(string gameId, string path)
        {
            if (!this.installInfo.ContainsKey(gameId))
            {
                this.logger.Error("No install data available for " + gameId);
                return false;
            }

            foreach (string file in this.installInfo[gameId].Files)
            {
                if (!File.Exists(path + "/" + file))
                    return false;
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

        public void InstalledGame(string id, string path)
        {
            InstalledGame game = new InstalledGame();
            game.Id = id;
            game.Path = path;
            if (this.installedGames == null) this.installedGames = new List<InstalledGame>();
            this.installedGames.Add(game);
        }
        public bool IsInstalled(string id)
        {
            if (this.installedGames == null) return false;
            return this.installedGames.Exists(x => x.Id == id);
        }
    }
}