namespace PlayFOnline.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json.Linq;

    public class FOServerJson
    {
        string configUrl;
        string statusUrl;
        string combinedUrl;

        public FOServerJson(string configUrl, string statusUrl, string combinedUrl)
        {
            this.configUrl = configUrl;
            this.statusUrl = statusUrl;
            this.combinedUrl = combinedUrl;
        }

        public JObject GetStatus()
        {
            JsonFetcher jsonFetch = new JsonFetcher();
            return jsonFetch.DownloadJson(this.statusUrl);
        }
        
        public JObject GetConfig()
        {
            JsonFetcher jsonFetch = new JsonFetcher();
            return jsonFetch.DownloadJson(this.configUrl);
        }

        public JObject GetCombined()
        {
            JsonFetcher jsonFetch = new JsonFetcher();
            return jsonFetch.DownloadJson(this.combinedUrl);
        }
    }
}
