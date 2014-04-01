using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayFO
{
    public class FOLogoInfo
    {
        public string Path { get; set; }
        public string Hash { get; set; }
    }

    public class FOScriptInfo
    {
        public string Checksum { get; set; } // SHA-1
        public string Path { get; set; }
        public string Url { get; set; }
        public string Version { get; set; }
    }

    // .dat files and so on not supplied with game.
    public class FOGameDependency
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public FOScriptInfo Script { get; set; }
        public string Path { get; set; }
    }

    public class FOGameInstallInfo
    {
        public List<FOGameLaunchProgram> LaunchPrograms { get; set; }
        public List<FOGameDependency> Dependencies { get; set; }
        public FOScriptInfo InstallScript { get; set; }
        public List<String> Files {get; set; }
        public Int64 InstallSize { get; set; }
        public string Updated { get; set; }
    }

    public class FOGameLaunchProgram
    {
        public string Name { get; set; }
        public string File { get; set; }
    }

    public class FOGameInfo
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

    public class FOGameStatus
    {
        public bool IsOffline() { return (Players == -1); }
        public int Players { get; set; }
        public string PlayersStr { get; set; }
        public int Seen { get; set; }
    }
}
