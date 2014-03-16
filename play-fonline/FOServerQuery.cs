using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace PlayFO
{
    class FOServerQuery
    {
        private List<FOGameInfo> Servers;

        private string configURL;
        private string statusURL;

        Logger logger = LogManager.GetLogger("FOServerQuery");

        public FOServerQuery(string configURL, string statusURL)
        {
            this.configURL = configURL;
            this.statusURL = statusURL;
            update();
        }

        public void update()
        {
            JsonFetcher jsonFetch = new JsonFetcher();

            JObject o1 = jsonFetch.downloadJson(configURL);
            //JObject o2 = jsonFetch.downloadJson(statusURL);

            Servers = new List<FOGameInfo>();

            foreach (JToken serverName in o1["fonline"]["config"]["server"].Children())
            {
                FOGameInfo server = JsonConvert.DeserializeObject<FOGameInfo>(serverName.First.ToString());
                if (!server.Closed && o1["fonline"]["status"]["server"][((JProperty)serverName).Name] != null)
                {
                    server.Status = JsonConvert.DeserializeObject<FOGameStatus>(o1["fonline"]["status"]["server"][((JProperty)serverName).Name].ToString());
                }
                server.Id = ((JProperty)serverName).Name;
                if (String.IsNullOrEmpty(server.Website))
                    server.Website = server.Link;

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
