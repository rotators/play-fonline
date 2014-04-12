namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using FOQuery.Data;

    public class TestJson
    {

        public TestJson()
        {
            Dictionary<string, FOGameInstallInfo> installinfos = new Dictionary<string, FOGameInstallInfo>();
            FOGameInstallInfo install = new FOGameInstallInfo();
            install.Files = new List<string> { "FODEconfig.exe", "FOnline.exe", "FOUpdater.exe" };
            install.Updated = "2014-03-15";
            install.InstallScript = new FOScriptInfo();
            install.InstallScript.Url = "http://fodev.net/play/scripts/install-fode.cs";
            install.InstallScript.Version = "0.1";
            install.InstallScript.Path = @"C:\Users\Ghosthack\Documents\GitHub\play-fonline\play-fonline\bin\Debug\scripts\install-fode.cs";
            install.InstallScript.Checksum = Utils.GetSHA1Checksum(install.InstallScript.Path);
            install.LaunchPrograms = new List<FOGameLaunchProgram>();
            FOGameLaunchProgram program = new FOGameLaunchProgram();
            program.Name = "Game - Direct3D";
            program.File = "FOnlineD3D.exe";
            install.LaunchPrograms.Add(program);

            installinfos["fode"] = install;

            string json = JsonConvert.SerializeObject(installinfos, Formatting.Indented);
            File.WriteAllText(".//example.json", json);
        }
    }
}
