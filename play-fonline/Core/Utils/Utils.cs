namespace PlayFOnline
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Collections.Generic;
    using Microsoft.Win32;

    public class NetVersion
    {
        public string Name { get; set; }
        public string RegKeyInstall { get; set; }
        public string RegKeySP { get; set; }
        public string RegKeyVersion { get; set; }
        public bool Installed { get; set; }
        public string Version { get; set; }
        public string SP { get; set; }
    }

    public static class Utils
    {
        public static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            if (!Directory.Exists(sourceDirName))
                return false;

            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (File.Exists(temppath))
                    continue;
                if (!Directory.Exists(temppath))
                    continue;
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }

            return true;
        }

        public static string GetFilenameFromUrl(string url)
        {
            Uri uri = new Uri(url);
            return uri.Segments[uri.Segments.Length - 1];
        }

        private static object GetRegKeyValue(string KeyValuePath)
        {
            RegistryKey Key = Registry.LocalMachine.OpenSubKey(KeyValuePath.Remove(KeyValuePath.LastIndexOf('\\'), KeyValuePath.Length - KeyValuePath.LastIndexOf('\\')));
            if (Key == null) return null;
            string Value = KeyValuePath.Substring(KeyValuePath.LastIndexOf('\\') + 1, KeyValuePath.Length - (KeyValuePath.LastIndexOf('\\') + 1));
            return Key.GetValue(Value);
        }

        public static string GetCLRInfo()
        {
            List<NetVersion> Versions = new List<NetVersion>();
            Versions.Add(new NetVersion()
            {
                Name = "1.0",
                RegKeyInstall = @"Software\Microsoft\.NETFramework\Policy\v1.0\3705",
                RegKeySP = @"Software\Microsoft\Active Setup\Installed Components\{78705f0d-e8db-4b2d-8193-982bdda15ecd}\Version",
                RegKeyVersion = @"Software\Microsoft\Active Setup\Installed Components\{78705f0d-e8db-4b2d-8193-982bdda15ecd}\Version"
            });

            Versions.Add(new NetVersion()
            {
                Name = "1.1",
                RegKeyInstall = @"Software\Microsoft\NET Framework Setup\NDP\v1.1.4322\Install",
                RegKeySP = @"Software\Microsoft\NET Framework Setup\NDP\v1.1.4322\SP",
                RegKeyVersion = @"Software\Microsoft\NET Framework Setup\NDP\v1.1.4322"
            });

            Versions.Add(new NetVersion()
            {
                Name = "2.0",
                RegKeyInstall = @"Software\Microsoft\NET Framework Setup\NDP\v2.0.50727\Install",
                RegKeySP = @"Software\Microsoft\NET Framework Setup\NDP\v2.0.50727\SP",
                RegKeyVersion = @"Software\Microsoft\NET Framework Setup\NDP\v2.0.50727\Version"
            });

            Versions.Add(new NetVersion()
            {
                Name = "3.0",
                RegKeyInstall = @"Software\Microsoft\NET Framework Setup\NDP\v3.0\Setup\InstallSuccess",
                RegKeySP = @"Software\Microsoft\NET Framework Setup\NDP\v3.0\SP",
                RegKeyVersion = @"Software\Microsoft\NET Framework Setup\NDP\v3.0\Version"
            });

            Versions.Add(new NetVersion()
            {
                Name = "3.5",
                RegKeyInstall = @"Software\Microsoft\NET Framework Setup\NDP\v3.5\Install",
                RegKeySP = @"Software\Microsoft\NET Framework Setup\NDP\v3.5\SP",
                RegKeyVersion = @"Software\Microsoft\NET Framework Setup\NDP\v3.5\Version"
            });

            Versions.Add(new NetVersion()
            {
                Name = "4.0 Client Profile",
                RegKeyInstall = @"Software\Microsoft\NET Framework Setup\NDP\v4\Client\Install",
                RegKeySP = @"Software\Microsoft\NET Framework Setup\NDP\v4\Client\Servicing",
                RegKeyVersion = @"Software\Microsoft\NET Framework Setup\NDP\v4\Version"
            });

            Versions.Add(new NetVersion()
            {
                Name = "4.0 Full Profile",
                RegKeyInstall = @"Software\Microsoft\NET Framework Setup\NDP\v4\Full\Install",
                RegKeySP = @"Software\Microsoft\NET Framework Setup\NDP\v4\Full\Servicing",
                RegKeyVersion = @"Software\Microsoft\NET Framework Setup\NDP\v4\Version"
            });

            string Info = "";
            Info += "OS version: " + Environment.OSVersion.VersionString + (IntPtr.Size == 4 ? " 32-bit" : " 64-bit") + Environment.NewLine;
            Info += ".NET CLR version: " + Environment.Version + Environment.NewLine;

            if (Directory.Exists(@Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework"))
            {
                Info += "   .NET installed versions:" + Environment.NewLine;

                foreach (NetVersion version in Versions)
                {
                    version.Installed = (GetRegKeyValue(version.RegKeyInstall) != null);
                    if (!version.Installed)
                        continue;
                    version.Version = (string)GetRegKeyValue(version.RegKeyVersion);
                    try { version.SP = ((int)GetRegKeyValue(version.RegKeySP)).ToString(); }
                    catch (Exception) { version.SP = (string)GetRegKeyValue(version.RegKeySP).ToString(); }
                }
                foreach (NetVersion version in Versions.FindAll(x => x.Installed))
                {
                    Info += "       " + version.Name + (version.SP != "0" ? " (SP " + version.SP + ")" : "") + (!String.IsNullOrEmpty(version.Version) ? " - " + version.Version : "") + Environment.NewLine;
                }

            }
            return Info;
        }

        public static string GetSHA1Checksum(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            using (BufferedStream bs = new BufferedStream(fs))
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    byte[] hash = sha1.ComputeHash(bs);
                    StringBuilder formatted = new StringBuilder(2 * hash.Length);
                    foreach (byte b in hash)
                    {
                        formatted.AppendFormat("{0:X2}", b);
                    }
                    return formatted.ToString();
                }
            }
        }

        public static DateTime GetUnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return time.AddSeconds(unixTimeStamp).ToLocalTime();
        }
    }
}