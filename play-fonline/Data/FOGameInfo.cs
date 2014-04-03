namespace PlayFOnline.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net.NetworkInformation;
    using System.Threading;

    public class FOGameInfo
    {
        public bool Closed         { get; set; }
        public string Host         { get; set; }
        public string Id           { get; set; }
        public string InstallPath  { get; set; }
        public string Link         { get; set; }
        public string Name         { get; set; }
        public int Port            { get; set; }
        public bool Singleplayer   { get; set; }
        public string Source       { get; set; }
        public FOGameStatus Status { get; set; }
        public string Website      { get; set; }

        public void Ping()
        {
            AutoResetEvent resetEvent = new AutoResetEvent(true);

            Ping ping = new Ping();
            ping.PingCompleted += new PingCompletedEventHandler(ping_PingCompleted);
            ping.SendAsync(this.Host, null);
        }

        void ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            PingReply reply = e.Reply;
            if (reply.Status == IPStatus.Success)
            {
                this.Status.Latency = (int)reply.RoundtripTime;
            }
        }
    }

    public class FOGameInstallInfo
    {
        public List<FOGameDependency> Dependencies      { get; set; }
        public List<string> Files                       { get; set; }
        public FOScriptInfo InstallScript               { get; set; }
        public long InstallSize                         { get; set; }
        public List<FOGameLaunchProgram> LaunchPrograms { get; set; }
        public string Updated                           { get; set; }
    }

    public class FOGameLaunchProgram
    {
        public string File { get; set; }
        public string Name { get; set; }
    }

    public class FOGameStatus
    {
        public int Players       { get; set; }
        public string PlayersStr { get; set; }
        public int Seen          { get; set; }
        public bool IsOffline()  { return this.Players == -1; }
        public int Latency       { get; set; }
    }

    public class FOLogoInfo
    {
        public string Hash { get; set; }
        public string Path { get; set; }
    }

    public class FOScriptInfo
    {
        public string Checksum { get; set; } // SHA-1
        public string Path     { get; set; }
        public string Url      { get; set; }
        public string Version  { get; set; }
    }
}
