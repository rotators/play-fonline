using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayFOnline
{
    // .dat files and so on not supplied with game.
    public class FOGameDependency
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public FOScriptInfo Script { get; set; }
    }

    public class FOGameInfo
    {
        public bool Closed { get; set; }
        public string Host { get; set; }
        public string Id { get; set; }
        public string InstallPath { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public bool Singleplayer { get; set; }
        public string Source { get; set; }
        public FOGameStatus Status { get; set; }
        public string Website { get; set; }
    }

    public class FOGameInstallInfo
    {
        public List<FOGameDependency> Dependencies { get; set; }
        public List<String> Files { get; set; }
        public FOScriptInfo InstallScript { get; set; }
        public Int64 InstallSize { get; set; }
        public List<FOGameLaunchProgram> LaunchPrograms { get; set; }
        public string Updated { get; set; }
    }

    public class FOGameLaunchProgram
    {
        public string File { get; set; }
        public string Name { get; set; }
    }

    public class FOGameStatus
    {
        public int Players { get; set; }
        public string PlayersStr { get; set; }
        public int Seen { get; set; }
        public bool IsOffline() { return (Players == -1); }
    }

    public class FOLogoInfo
    {
        public string Hash { get; set; }
        public string Path { get; set; }
    }

    public class FOScriptInfo
    {
        public string Checksum { get; set; } // SHA-1
        public string Path { get; set; }
        public string Url { get; set; }
        public string Version { get; set; }
    }
}
