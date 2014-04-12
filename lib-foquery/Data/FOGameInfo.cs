namespace FOQuery.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net.NetworkInformation;
    using System.Threading;

    public class FOGameInfo : EventArgs
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

        public void Ping(Action completedAction)
        {
            Ping ping = new Ping();
            PingOptions pingOptions = new PingOptions();

            string data = "rotate";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            ping.PingCompleted += new PingCompletedEventHandler(ping_PingCompleted);
            ping.SendAsync(this.Host, 500, buffer, pingOptions, completedAction);
        }

        void ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (e.Reply == null)
                return;

            PingReply reply = e.Reply;
            if (reply.Status == IPStatus.Success)
            {
                this.Status.Latency = (int)reply.RoundtripTime;
                ((Action)e.UserState).Invoke();
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

    public class FOGameStatus : IComparable
    {
        public FOGameStatus()    { this.Latency = int.MaxValue; }

        public int Players       { get; set; }
        public string PlayersStr { get; set; }
        public int Seen          { get; set; }
        public bool IsOffline()  { return this.Players == -1; }
        public int Latency       { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            FOGameStatus other = obj as FOGameStatus;
            if (other.IsOffline()) return 1;
            else if (this.IsOffline() && other.IsOffline()) return 0;
            else
            {
                if (this.IsOffline()) return -1;
                return this.Players.CompareTo(other.Players);
            }
        }
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
