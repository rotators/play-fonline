namespace PlayFOnline.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSScriptLibrary;

    public interface IResolveHost { }

    /// <summary>
    /// Scripts for resolving dependencies
    /// </summary>
    public interface IResolveScript
    {
        bool IsValidResource(string name, string filePath);
    }

    public class ResolveHost : IResolveHost
    {
        public bool RunResolveScript(string scriptFile, string name, string filePath)
        {
            IResolveScript script = (IResolveScript)CSScript.Load(scriptFile).CreateObject("Script");
            return script.IsValidResource(name, filePath);
        }
    }
}
