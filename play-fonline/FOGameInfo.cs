using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayFO
{
    class FOGameInstallInfo
    {
        public List<FOGameLaunchProgram> LaunchPrograms { get; set; }
        public FOInstallScriptInfo InstallScript { get; set; }
        public List<String> Files {get; set; }
        public string Updated { get; set; }
    }

    class FOGameLaunchProgram
    {
        public string Name { get; set; }
        public string File { get; set; }
    }

    class FOInstallScriptInfo
    {
        public string ScriptUrl { get; set; }
        public string ScriptUpdated { get; set; }
        public string ScriptVersion { get; set; }
    }

    class FOGameInfo
    {
        public string Name { get; set;}
        public string Host { get; set; }
        public string Website { get; set; }
        public string Source { get; set; }
        public int Port { get; set; }
        public string Link { get; set; }
        public string InstallPath { get; set; }
        public bool Closed { get; set; }
        public bool Singleplayer { get; set; }

        public FOGameStatus Status { get; set; }
    }

    class FOGameStatus
    {
        public int Players { get; set; }
    }
}
