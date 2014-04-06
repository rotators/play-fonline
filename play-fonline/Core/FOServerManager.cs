namespace PlayFOnline.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using PlayFOnline.Data;

    public class FOServerManager
    {
        FOServerQuery query;


        public FOServerManager(string ConfigUrl, string StatusUrl)
        {
            this.query = new FOServerQuery(ConfigUrl, StatusUrl);
        }

        private List<FOGameInfo> GetOpenServers()
        {
            return query.GetServers(true);
        }

        private bool IsInstalled(string gameId)
        {
            return false;
        }

        public List<FOGameInfo> GetOfflineServers()
        {
            var servers = this.GetOpenServers();
            return servers.Where(x => x.Status.IsOffline()).ToList();
        }

        public List<FOGameInfo> GetServers(bool onlyOnline)
        {
            var servers = this.GetOpenServers();
            if (onlyOnline)
                servers = servers.Where(x => !x.Status.IsOffline() || this.IsInstalled(x.Id)).ToList(); // Always add installed, even if offline
            return servers;
        }
    }
}
