namespace PlayFOnline.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using FOQuery.Data;
    using FOQuery;

    public class ServerManager
    {
        FOServerQuery query;
        InstallHandler installHandler;

        public ServerManager(FOServerJson foServerJson, InstallHandler installHandler)
        {
            this.query = new FOServerQuery(foServerJson, new NLogWrapper("FOQuery"));
            this.installHandler = installHandler;
        }

        private List<FOGameInfo> SetInstallInfo(List<FOGameInfo> servers)
        {
            foreach (var server in servers)
            {
                server.InstallPath = null;
                if (installHandler.IsInstalled(server.Id))
                    server.InstallPath = this.installHandler.GetInstallPath(server.Id);
            }
            return servers;
        }

        private List<FOGameInfo> GetOpenServers()
        {
            return SetInstallInfo(query.GetServers(true));
        }

        private bool IsInstalled(string gameId)
        {
            return installHandler.IsInstalled(gameId);
        }

        public List<FOGameInfo> GetOfflineServers()
        {
            var servers = this.GetOpenServers();
            servers = servers.Where(x => x.Status.IsOffline()).ToList();
            return SetInstallInfo(servers);
        }

        public List<FOGameInfo> GetServers(bool onlyOnline)
        {
            var servers = this.GetOpenServers();
            if (onlyOnline)
                servers = servers.Where(x => !x.Status.IsOffline() || this.IsInstalled(x.Id)).ToList(); // Always add installed, even if offline
            return servers;
        }

        public void UpdateStatus()
        {
            this.query.UpdateStatus();
        }
    }
}
