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

        public InstallHandler(Dictionary<String, FOGameInstallInfo> InstallInfo)
        {
            this.InstallInfo = InstallInfo;
        }

        public List<FOGameLaunchProgram> GetLaunchPrograms(string game)
        {
            return InstallInfo[game].LaunchPrograms;
        }

        public List<FOGameDependency> GetDependencies(string game)
        {
            return InstallInfo[game].Dependencies;
        }

        public bool HasInstallInfo(string game)
        {
            return InstallInfo.ContainsKey(game);
        }

        public bool VerifyGameFolderPath(string game, string path)
        {
            foreach (String file in InstallInfo[game].Files)
            {
                if (!File.Exists(path + "/" + file))
                    return false;
            }
            return true;
        }
    }
}
