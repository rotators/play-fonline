using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace PlayFOnline
{
    public class LogoManager
    {
        private Logger logger = LogManager.GetLogger("LogoManager");
        private Dictionary<string, FOLogoInfo> logoInfo;
        private string logoInfoFile = Environment.CurrentDirectory + "\\logos.json";

        public LogoManager(string path, string logoUrl)
        {
            this.logoInfo = LoadSettings();
            update(path, logoUrl);
        }

        public string getLogoPath(string gameId)
        {
            if (logoInfo.ContainsKey(gameId))
                return logoInfo[gameId].Path;
            return null;
        }

        public Dictionary<string, FOLogoInfo> LoadSettings()
        {
            if (!File.Exists(logoInfoFile))
                return new Dictionary<string, FOLogoInfo>();
            string json = File.ReadAllText(logoInfoFile);
            return JsonConvert.DeserializeObject<Dictionary<string, FOLogoInfo>>(json);
        }

        public void SaveSettings(Dictionary<string, FOLogoInfo> logoInfo)
        {
            string json = JsonConvert.SerializeObject(logoInfo, Formatting.Indented);
            File.WriteAllText(logoInfoFile, json);
        }
        public void update(string savePath, string logoUrl)
        {
            JsonFetcher jsonFetcher = new JsonFetcher();
            JObject o = jsonFetcher.downloadJson(logoUrl);

            foreach (JToken serverName in o["fonline"]["logo"].Children())
            {
                FOLogoInfo logo = JsonConvert.DeserializeObject<FOLogoInfo>(serverName.First.ToString());
                string id = ((JProperty)serverName).Name;
                if (logoInfo.ContainsKey(id))
                {
                    if (this.logoInfo[id].Hash == logo.Hash)
                        continue;
                }
                downloadLogo("http://fodev.net/status/" + logo.Path, savePath + Path.DirectorySeparatorChar + Utils.GetFilenameFromUrl("http://" + logo.Path));
                logo.Path = savePath + Path.DirectorySeparatorChar + Utils.GetFilenameFromUrl("http://" + logo.Path);
                logoInfo[id] = logo;
            }

            SaveSettings(logoInfo);
        }

        private void downloadLogo(string url, string savePath)
        {
            using (var webClient = new System.Net.WebClient())
            {
                webClient.Proxy = null;
                logger.Info("Downloading logo from {0}", url);
                try
                {
                    webClient.DownloadFile(new Uri(url), savePath);
                }
                catch (WebException e)
                {
                    logger.Error("Failed to download {0}: {1}", url, e.Message);
                    return;
                }
            }
        }
    }
}