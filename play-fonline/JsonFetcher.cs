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
    class JsonFetcher
    {
        private WebException exWeb;
        private JsonReaderException exJson;

        Logger logger = LogManager.GetLogger("JsonFetcher");

        public JObject downloadJson(string URL)
        {
            string jsonContent = this.fetchJson(URL);
            return parseJson(jsonContent);
        }

        private JObject parseJson(string json)
        {
            try
            {
                return JObject.Parse(json);
            }
            catch (JsonReaderException e)
            {
                logger.Error("Failed to parse {0}: {1}", json, e.Message);
                exJson = e;
            }
            return null;
        }

        private string fetchJson(string URL)
        {
            string jsonContent;
            using (var webClient = new System.Net.WebClient())
            {
                webClient.Proxy = null;
                logger.Info("Downloading JSON from {0}", URL);
                try
                {
                    jsonContent = webClient.DownloadString(URL);
                }
                catch (WebException e)
                {
                    logger.Error("Failed to download {0}: {1}", URL, e.Message);
                    exWeb = e;
                    return "";
                }
            }
            return jsonContent;
        }
    }
}
