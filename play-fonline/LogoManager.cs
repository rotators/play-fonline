using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using NLog;

namespace PlayFO
{
    public class LogoManager
    {
        private Dictionary<string, FOLogoInfo> logoInfo;
        private string logoInfoFile = Environment.CurrentDirectory + "\\logos.json";

        Logger logger = LogManager.GetLogger("LogoManager");

        public LogoManager(string path, string logoUrl)
        {
            this.logoInfo = LoadSettings();
            update(path, logoUrl);
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

        public string getLogoPath(string gameId)
        {
            if (logoInfo.ContainsKey(gameId))
                return logoInfo[gameId].Path;
            return null;
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
                downloadLogo("http://fodev.net/status/" + logo.Path, savePath + Path.DirectorySeparatorChar + Utils.GetFilenameFromUrl("http://"+logo.Path));
                logo.Path = savePath + Path.DirectorySeparatorChar + Utils.GetFilenameFromUrl("http://" + logo.Path);
                logoInfo[id] = logo;
            }

            SaveSettings(logoInfo);
        }
    }
}
