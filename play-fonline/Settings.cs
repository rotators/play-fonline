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
        public static string path = Environment.CurrentDirectory + "\\settings.json";

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
       // public string bootstrapURL { get; set; }
        public string installURL { get; set; }
        public string configURL { get; set; }
        public string statusURL { get; set; }

        public UISettings UI { get; set; }
        public List<FOGameDependency> Dependencies { get; set; }
        public List<InstalledGame> Games { get; set; }

        public void InstalledGame(string Id, string Path)
        {
            InstalledGame game = new PlayFO.InstalledGame();
            game.Id = Id;
            game.Path = Path;
            if (Games == null) Games = new List<PlayFO.InstalledGame>();
            Games.Add(game);
        }

        public void AddDependency(string Name, string Path)
        {
            FOGameDependency dep =  new FOGameDependency();
            dep.Name = Name;
            dep.Path = Path;
            if (Dependencies == null) Dependencies = new List<FOGameDependency>();
            Dependencies.Add(dep);
        }

        public bool HasDependency(string Name)
        {
            if (Dependencies == null) return false;
            return (Dependencies.Exists(x => x.Name == Name));
        }

        public string GetInstallPath(string Id)
        {
            return (Games.Find(x => x.Id == Id).Path);
        }

        public bool IsInstalled(string Id)
        {
            if (Games == null) return false;
            return (Games.Exists(x => x.Id == Id));
        }

    }

    public class InstalledGame
    {
        public string Id { get; set; }
        public string Path { get; set; }
    }
}
