namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using PlayFOnline.Data;

    public class InstallHandler
    {
        private Dictionary<string, FOGameInstallInfo> installInfo;
        private NLog.Logger logger = NLog.LogManager.GetLogger("InstallHandler");

        public InstallHandler(Dictionary<string, FOGameInstallInfo> installInfo)
        {
            this.installInfo = installInfo;
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
    }
}