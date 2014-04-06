namespace PlayFOnline.Core.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using PlayFOnline.Data;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class JsonSerializer
    {
        public Dictionary<string, FOGameInstallInfo> GetInstallData(JObject JsonObject)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, FOGameInstallInfo>>(JsonObject["fonline"]["install-data"].ToString());
        }
    }
}
