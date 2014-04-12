namespace PlayFOnline.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using FOQuery.Data;
    using NLog;

    public class FOSettings
    {
        public string ConfigUrl { get; set; }

        public List<FOGameDependency> Dependencies { get; set; }
        public List<InstalledGame> Games { get; set; }

        public string CombinedUrl { get; set; }
        public string InstallUrl { get; set; }
        public string StatusUrl { get; set; }

        public PathSettings Paths { get; set; }
        public UISettings UI { get; set; }
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
        public string Logos { get; set; }
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
        public static Logger logger;
        public static readonly string SettingsPath = Path.Combine(Environment.CurrentDirectory, "settings.json");

        public static void Init()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        public static FOSettings LoadSettings()
        {
            if (!File.Exists(SettingsPath))
            {
                logger.Error("Unable to find settings on {0}", SettingsPath);
                return null;
            }

            logger.Info("Loading settings from {0}", SettingsPath);
            string jsonSettings = File.ReadAllText(SettingsPath);
            logger.Info("Read settings file.");
            var settings = JsonConvert.DeserializeObject<FOSettings>(jsonSettings);
            logger.Info("Deserialized settings.");
            return settings;
        }

        public static void SaveSettings(FOSettings settings)
        {
            logger.Info("Serializing settings.");
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            logger.Info("Saving settings to {0}.", SettingsPath);
            File.WriteAllText(SettingsPath, json);
        }
    }
}