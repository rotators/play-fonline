using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlayFO
{
    class FOServerQuery
    {
        private List<FOGameInfo> Servers;

        public FOServerQuery(string configURL, string statusURL)
        {
            string jsonServers;
            string jsonStatus;

            using (var webClient = new System.Net.WebClient())
            {
                webClient.Proxy = null;
                jsonServers = webClient.DownloadString(configURL);
                jsonStatus = webClient.DownloadString(statusURL);
            }

            JObject o1 = JObject.Parse(jsonServers);
            JObject o2 = JObject.Parse(jsonStatus);

            Servers = new List<FOGameInfo>();

            foreach (JToken serverName in o1["fonline"]["config"]["server"].Children())
            {
                FOGameInfo server = JsonConvert.DeserializeObject<FOGameInfo>(serverName.First.ToString());
                if (!server.Closed && o2["fonline"]["status"]["server"][((JProperty)serverName).Name] != null)
                {
                    server.Status = JsonConvert.DeserializeObject<FOGameStatus>(o2["fonline"]["status"]["server"][((JProperty)serverName).Name].ToString());
                }
                this.Servers.Add(server);
            }
        }

        public List<FOGameInfo> GetOnlineServers()
        {
            return this.Servers.Where(x => x.Status != null && x.Status.Players != -1).ToList();
        }
            
        public List<FOGameInfo> GetServers(bool onlyOpen)
        {
            if (onlyOpen) return this.Servers.Where(x => !x.Closed && !x.Singleplayer).ToList();
            return this.Servers;
        }
    }
}
