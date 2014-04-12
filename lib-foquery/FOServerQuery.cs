namespace FOQuery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using FOQuery;
    using FOQuery.Data;
    using FOQuery.Json;

    public class FOServerQuery
    {
        //private Logger logger = LogManager.GetLogger("FOServerQuery");
        private List<FOGameInfo> servers;
        private FOServerJson json;
        private FOJsonDeserializer deserializer;

        public FOServerQuery(FOServerJson json)
        {
            this.json = json;
            this.deserializer = new FOJsonDeserializer();
            this.Update();
        }

        public List<FOGameInfo> GetOnlineServers()
        {
            return this.servers.Where(x => x.Status != null && x.Status.Players != -1).ToList();
        }

        public List<FOGameInfo> GetServers(bool onlyOpen = true)
        {
            if (onlyOpen) { return this.servers.Where(x => !x.Closed && !x.Singleplayer).ToList(); }
            return this.servers;
        }

        private string GetReadableTime(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            if (t.Days > 0) return t.Days + " days";
            if (t.Hours > 0) return t.Hours + " hours";
            if (t.Minutes > 0) return t.Minutes + " minutes";
            return seconds + " seconds";
        }

        private int GetCurrentUnixTime()
        {
            int unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            return unixTimestamp;
        }

        private FOGameStatus ProcessStatus(FOGameStatus status)
        {
            if (status.IsOffline())
            {
                if (status.Seen != -1)
                    status.PlayersStr = string.Format("Offline - Down for {0}", this.GetReadableTime(this.GetCurrentUnixTime() - status.Seen));
                else
                    status.PlayersStr = "Offline";
            }
            status.PlayersStr = status.Players.ToString();

            return status;
        }

        public void UpdateStatus()
        {
            JObject data = json.GetStatus();
            foreach(var server in this.servers)
            {
                server.Status = this.deserializer.GetServerStatus(data, server.Id);
                server.Status = ProcessStatus(server.Status);
            }
        }

        public void Update()
        {
            JObject data = json.GetCombined();

            var servers = new List<FOGameInfo>();
            foreach(var server in deserializer.GetServers(data))
            {
                if (string.IsNullOrEmpty(server.Host) || server.Port == 0 || server.Closed) { continue; } // Port 0 or empty host is usually placeholders.

                if (string.IsNullOrEmpty(server.Website))
                    server.Website = server.Link;

                server.Status = ProcessStatus(server.Status);
                servers.Add(server);
            }
            this.servers = servers;
        }
    }
}