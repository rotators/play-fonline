using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PlayFO
{
    class InstallHandler
    {
        private Dictionary<String, FOGameInstallInfo> InstallInfo;
        NLog.Logger logger = NLog.LogManager.GetLogger("InstallHandler");

        public InstallHandler(Dictionary<String, FOGameInstallInfo> InstallInfo)
        {
            this.InstallInfo = InstallInfo;
        }

        public List<FOGameLaunchProgram> GetLaunchPrograms(string gameId)
        {
            return InstallInfo[gameId].LaunchPrograms;
        }

        public List<FOGameDependency> GetDependencies(string gameId)
        {
            return InstallInfo[gameId].Dependencies;
        }

        public FOScriptInfo GetInstallScriptInfo(string gameId)
        {
            return InstallInfo[gameId].InstallScript;
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
