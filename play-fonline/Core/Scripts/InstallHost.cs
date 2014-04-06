namespace PlayFOnline.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSScriptLibrary;

    public interface IInstallScript
    {
        bool Install(string game, string tempDir, string installDir);
    }

    public class InstallHost
    {
        public bool RunInstallScript(string scriptFile, string game, string tempDir, string installDir)
        {
            IInstallScript script = (IInstallScript)CSScript.Load(scriptFile).CreateObject("Script");
            return script.Install(game, tempDir, installDir);
        }
    }
}