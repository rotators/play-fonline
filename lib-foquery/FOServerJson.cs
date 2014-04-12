namespace FOQuery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json.Linq;
    using FOQuery.Json;

    public class FOServerJson
    {
        string configUrl;
        string statusUrl;
        string combinedUrl;

        JsonFetcher jsonFetch;

        public FOServerJson(string configUrl, string statusUrl, string combinedUrl, ILogger logger)
        {
            this.configUrl = configUrl;
            this.statusUrl = statusUrl;
            this.combinedUrl = combinedUrl;
            jsonFetch = new JsonFetcher(logger);
        }

        public JObject GetStatus()
        {
            return jsonFetch.DownloadJson(this.statusUrl);
        }
        
        public JObject GetConfig()
        {
            return jsonFetch.DownloadJson(this.configUrl);
        }

        public JObject GetCombined()
        {
            return jsonFetch.DownloadJson(this.combinedUrl);
        }
    }
}
