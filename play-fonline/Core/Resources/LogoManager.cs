namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NLog;
    using FOQuery.Data;
    using FOQuery.Json;

    public class LogoManager
    {
        private Logger logger = LogManager.GetLogger("LogoManager");
        private Dictionary<string, FOLogoInfo> logoInfo;
        private string logoInfoFile = Environment.CurrentDirectory + "\\logos.json";

        public LogoManager(string path, string logoUrl)
        {
            this.logoInfo = this.LoadSettings();
            this.Update(path, logoUrl);
        }

        public string GetLogoPath(string gameId)
        {
            if (this.logoInfo.ContainsKey(gameId))
                return this.logoInfo[gameId].Path;
            return null;
        }

        public Dictionary<string, FOLogoInfo> LoadSettings()
        {
            if (!File.Exists(this.logoInfoFile))
                return new Dictionary<string, FOLogoInfo>();
            string json = File.ReadAllText(this.logoInfoFile);
            return JsonConvert.DeserializeObject<Dictionary<string, FOLogoInfo>>(json);
        }

        public void SaveSettings(Dictionary<string, FOLogoInfo> logos)
        {
            string json = JsonConvert.SerializeObject(logos, Formatting.Indented);
            File.WriteAllText(this.logoInfoFile, json);
        }
        public void Update(string savePath, string logoUrl)
        {
            JsonFetcher jsonFetcher = new JsonFetcher();
            JObject o = jsonFetcher.DownloadJson(logoUrl);

            foreach (JToken serverName in o["fonline"]["logo"].Children())
            {
                FOLogoInfo logo = JsonConvert.DeserializeObject<FOLogoInfo>(serverName.First.ToString());
                string id = ((JProperty)serverName).Name;
                if (this.logoInfo.ContainsKey(id))
                {
                    if (this.logoInfo[id].Hash == logo.Hash)
                        continue;
                }
                this.DownloadLogo("http://fodev.net/status/" + logo.Path, savePath + Path.DirectorySeparatorChar + Utils.GetFilenameFromUrl("http://" + logo.Path));
                logo.Path = savePath + Path.DirectorySeparatorChar + Utils.GetFilenameFromUrl("http://" + logo.Path);
                this.logoInfo[id] = logo;
            }

            this.SaveSettings(this.logoInfo);
        }

        private void DownloadLogo(string url, string savePath)
        {
            using (var webClient = new System.Net.WebClient())
            {
                webClient.Proxy = null;
                this.logger.Info("Downloading logo from {0}", url);
                try
                {
                    webClient.DownloadFile(new Uri(url), savePath);
                }
                catch (WebException e)
                {
                    this.logger.Error("Failed to download {0}: {1}", url, e.Message);
                    return;
                }
            }
        }
    }
}