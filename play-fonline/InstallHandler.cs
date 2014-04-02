using System;
using System.Collections.Generic;
using System.IO;

namespace PlayFOnline
{
    public class InstallHandler
    {
        private Dictionary<String, FOGameInstallInfo> InstallInfo;
        private NLog.Logger logger = NLog.LogManager.GetLogger("InstallHandler");

        public InstallHandler(Dictionary<String, FOGameInstallInfo> InstallInfo)
        {
            this.InstallInfo = InstallInfo;
        }

        public List<FOGameDependency> GetDependencies(string gameId)
        {
            return InstallInfo[gameId].Dependencies;
        }

        public FOScriptInfo GetInstallScriptInfo(string gameId)
        {
            return InstallInfo[gameId].InstallScript;
        }

        public List<FOGameLaunchProgram> GetLaunchPrograms(string gameId)
        {
            return InstallInfo[gameId].LaunchPrograms;
        }

        public bool HasInstallInfo(string gameId)
        {
            return InstallInfo.ContainsKey(gameId);
        }

        public bool VerifyGameFolderPath(string gameId, string path)
        {
            if (!InstallInfo.ContainsKey(gameId))
            {
                logger.Error("No install data available for " + gameId);
                return false;
            }

            foreach (String file in InstallInfo[gameId].Files)
            {
                if (!File.Exists(path + "/" + file))
                    return false;
            }
            return true;
        }
    }
}