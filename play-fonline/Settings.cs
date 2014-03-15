using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace PlayFO
{
    static class SettingsManager
    {
        static string path = Environment.CurrentDirectory + "\\settings.json";

        static public FOSettings LoadSettings()
        {
            string jsonSettings = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<FOSettings>(jsonSettings);
        }

        static public void SaveSettings(FOSettings settings)
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }

    public class UISettings
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public bool showOffline { get; set; }
    }

    public class FOSettings
    {
        public string configURL { get; set; }
        public string statusURL { get; set; }

        public UISettings UI { get; set; }
        public List<Dependencies> Dependencies { get; set; }
        public List<InstalledGame> Games { get; set; }
    }

    public class InstalledGame
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    // .dat files and so on not supplied with game.
    public class Dependencies
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
