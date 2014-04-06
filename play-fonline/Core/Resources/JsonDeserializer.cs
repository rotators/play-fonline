namespace PlayFOnline.Core.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using PlayFOnline.Data;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class FOJsonDeserializer
    {
        public Dictionary<string, FOGameInstallInfo> GetInstallData(JObject data)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, FOGameInstallInfo>>(data["fonline"]["install-data"].ToString());
        }

        public FOGameStatus GetServerStatus(JObject data, string gameId)
        {
            return JsonConvert.DeserializeObject<FOGameStatus>(data["fonline"]["status"]["server"][gameId].ToString());
        }

        public List<FOGameInfo> GetServers(JObject data)
        {
            var servers = new List<FOGameInfo>();
            foreach (JToken serverName in data["fonline"]["config"]["server"].Children())
            {
                FOGameInfo server = JsonConvert.DeserializeObject<FOGameInfo>(serverName.First.ToString());
                server.Id = ((JProperty)serverName).Name;
                if (!server.Closed && data["fonline"]["status"]["server"][server.Id] != null)
                    server.Status = this.GetServerStatus(data, server.Id);
                servers.Add(server);
            }
            return servers;
        }

    }
}
