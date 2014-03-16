using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSScriptLibrary;

// Scripts for resolving dependencies.
namespace PlayFO.Scripts
{
    public interface IResolveHost 
    { 
    }
    public interface IResolveScript
    {
        bool IsValidResource(string name, string filePath);
    }

    class ResolveHost : IResolveHost
    {
        public bool RunResolveScript(string scriptFile, string name, string filePath)
        {
            IResolveScript script = (IResolveScript)CSScript.Load(scriptFile).CreateObject("Script");
            return script.IsValidResource(name, filePath);
        }
    }
}
