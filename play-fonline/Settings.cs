namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using PlayFOnline.Data;

    public class FOSettings
    {
        public string ConfigUrl { get; set; }

        public List<FOGameDependency> Dependencies { get; set; }
        public List<InstalledGame> Games { get; set; }

        public string InstallUrl { get; set; }
        public string StatusUrl { get; set; }

        public PathSettings Paths { get; set; }
        public UISettings UI { get; set; }

        public void AddDependency(FOGameDependency depend, string Path)
        {
            FOGameDependency newDepend = depend;
            newDepend.Path = Path;
            if (this.Dependencies == null) this.Dependencies = new List<FOGameDependency>();
            this.Dependencies.Add(newDepend);
        }

        public string GetInstallPath(string id)
        {
            return this.Games.Find(x => x.Id == id).Path;
        }

        public bool HasDependency(string name)
        {
            if (this.Dependencies == null) return false;
            return this.Dependencies.Exists(x => x.Name == name);
        }

        public void InstalledGame(string id, string path)
        {
            InstalledGame game = new PlayFOnline.InstalledGame();
            game.Id = id;
            game.Path = path;
            if (this.Games == null) this.Games = new List<PlayFOnline.InstalledGame>();
                this.Games.Add(game);
        }
        public bool IsInstalled(string id)
        {
            if (this.Games == null) return false;
            return this.Games.Exists(x => x.Id == id);
        }
    }

    public class InstalledGame
    {
        public string Id { get; set; }
        public string Path { get; set; }
    }

    public class PathSettings
    {
        public string DownloadTemp { get; set; }
        public string Scripts { get; set; }
    }

    public class UISettings
    {
        public int Height { get; set; }
        public bool ShowOffline { get; set; }
        public int Width { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    internal static class SettingsManager
    {
        public static readonly string SettingsPath = Path.Combine(Environment.CurrentDirectory, "settings.json");

        public static FOSettings LoadSettings()
        {
            string jsonSettings = File.ReadAllText(SettingsPath);
            return JsonConvert.DeserializeObject<FOSettings>(jsonSettings);
        }

        public static void SaveSettings(FOSettings settings)
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsPath, json);
        }
    }
}