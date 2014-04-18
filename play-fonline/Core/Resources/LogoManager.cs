namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NLog;
    using PlayFOnline.Core;
    using FOQuery.Data;
    using FOQuery.Json;

    public class LogoManager
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
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
            JsonFetcher jsonFetcher = new JsonFetcher(new NLogWrapper("FOQuery"));
            JObject o = jsonFetcher.DownloadJson(logoUrl);

            if (o == null)
            {
                logger.Error("Returned JSON node is null.");
                return;
            }

            foreach (JToken serverName in o["fonline"]["logo"].Children())
            {
                FOLogoInfo logo = JsonConvert.DeserializeObject<FOLogoInfo>(serverName.First.ToString());
                string id = ((JProperty)serverName).Name;
                if (this.logoInfo.ContainsKey(id))
                {
                    if (!File.Exists(this.logoInfo[id].Path))
                    {
                        this.logger.Error("{0} not found.", this.logoInfo[id].Path);
                    }
                    else
                    {
                        if (this.logoInfo[id].Hash == logo.Hash)
                            continue;
                    }
                }
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                string imagePath = savePath + Path.DirectorySeparatorChar + Utils.GetFilenameFromUrl("http://" + logo.Path);
                this.DownloadLogo("http://fodev.net/status/" + logo.Path, imagePath);
                logo.Path = imagePath;
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
                    this.logger.Error("Failed to download {0}: {1}: {2}", url, e.Message, e.InnerException.Message);
                    return;
                }
            }
        }
    }
}