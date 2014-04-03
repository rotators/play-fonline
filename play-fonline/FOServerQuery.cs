namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NLog;
    using PlayFOnline.Data;

    internal class FOServerQuery
    {
        private string configURL;
        private Logger logger = LogManager.GetLogger("FOServerQuery");
        private List<FOGameInfo> servers;
        private string statusURL;

        public FOServerQuery(string configURL, string statusURL)
        {
            this.configURL = configURL;
            this.statusURL = statusURL;
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

        public void Update()
        {
            JsonFetcher jsonFetch = new JsonFetcher();

            JObject o1 = jsonFetch.DownloadJson(this.configURL);
            JObject o2 = jsonFetch.DownloadJson(this.statusURL);

            this.servers = new List<FOGameInfo>();

            foreach (JToken serverName in o1["fonline"]["config"]["server"].Children())
            {
                FOGameInfo server = JsonConvert.DeserializeObject<FOGameInfo>(serverName.First.ToString());

                if (string.IsNullOrEmpty(server.Host) || server.Port == 0) { continue; } // Usually placeholders.

                if (!server.Closed && o2["fonline"]["status"]["server"][((JProperty)serverName).Name] != null)
                {
                    server.Status = JsonConvert.DeserializeObject<FOGameStatus>(o2["fonline"]["status"]["server"][((JProperty)serverName).Name].ToString());
                    if (server.Status.IsOffline())
                    {
                        if (server.Status.Seen != -1)
                            server.Status.PlayersStr = "Offline - Down for " + Utils.GetReadableTime(Utils.GetCurrentUnixTime() - server.Status.Seen);
                        else
                            server.Status.PlayersStr = "Offline";
                    }
                    else
                        server.Status.PlayersStr = server.Status.Players.ToString();
                }
                server.Id = ((JProperty)serverName).Name;
                if (string.IsNullOrEmpty(server.Website))
                    server.Website = server.Link;

                this.servers.Add(server);
            }
        }
    }
}