using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayFO
{
    // .dat files and so on not supplied with game.
    public class FOGameDependency
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ScriptPath { get; set; } // Local path
        public string ScriptUrl { get; set; } // For dependency checking scripts.
        public string Path { get; set; }
    }

    class FOGameInstallInfo
    {
        public List<FOGameLaunchProgram> LaunchPrograms { get; set; }
        public List<FOGameDependency> Dependencies { get; set; }
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
        public string Id { get; set; }
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
