namespace FOQuery.Json
{
    using System.Net;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    //using NLog;

    public class JsonFetcher
    {
        private JsonReaderException jsonException;
        private WebException webException;
        private ILogger logger;
        private string jsonContent;

        public JsonFetcher(ILogger logger)
        {
            this.logger = logger;
        }

        private string GetJson()
        {
            return this.jsonContent;
        }

        public JObject DownloadJson(string url)
        {
            this.jsonContent = this.FetchJson(url);
            return this.ParseJson(jsonContent);
        }

        private string FetchJson(string url)
        {
            if (url == null || !url.Contains("http://"))
            {
                this.logger.Error("Invalid url: {0}", url);
                return string.Empty;
            }

            string jsonContent;
            using (var webClient = new System.Net.WebClient())
            {
                webClient.Proxy = null;
                this.logger.Info("Downloading JSON from {0}", url);
                try
                {
                    jsonContent = webClient.DownloadString(url);
                }
                catch (WebException e)
                {
                    this.logger.Error("Failed to download {0}: {1}", url, e.Message);
                    this.webException = e;
                    return string.Empty;
                }
            }
            return jsonContent;
        }

        private JObject ParseJson(string json)
        {
            try
            {
                return JObject.Parse(json);
            }
            catch (JsonReaderException e)
            {
                this.logger.Error("Failed to parse {0}: {1}", json, e.Message);
                this.jsonException = e;
            }
            return null;
        }
    }
}