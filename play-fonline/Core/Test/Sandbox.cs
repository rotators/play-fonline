namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Windows.Forms;
    using DATLib;
    using Ionic.Zip;
    using PlayFOnline.Core;

    public class Sandbox
    {
        public Sandbox()
        {
            
        }

        public bool TestInstallReloaded(string game, string tempDir, string installDir)
        {
            string filename = Path.Combine(tempDir, "FOnlineReloaded-Full.zip");
            string url = "http://www.fonline-reloaded.net/files/dl_fullclient.php";

            ProgressDownloader downloader = new ProgressDownloader();
            downloader.Download(game, url, filename);

            if (!File.Exists(filename))
            {
                MessageBox.Show(filename + " not found after download!");
                return false;
            }

            ZipFile zip;
            try
            {
                zip = ZipFile.Read(filename);
            }
            catch (ZipException ex)
            {
                MessageBox.Show(string.Format("Error when loading {0}: {1}", filename, ex.Message));
                return false;
            }

            zip.ToList().ForEach(entry =>
            {
                if (entry.FileName != "FOnline Reloaded/")
                {
                    entry.FileName = entry.FileName.Replace("FOnline Reloaded/", string.Empty);
                    entry.Extract(installDir);
                }
            });

            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.WorkingDirectory = installDir;
            procInfo.FileName = installDir + "\\" + "Updater.exe";
            Process proc = Process.Start(procInfo);
            while (!proc.HasExited)
            {
                IntPtr hwndChild = Win32.FindWindowEx((IntPtr)proc.MainWindowHandle, IntPtr.Zero, "TButton", "Check");

                const int BN_CLICKED = 245;
                Win32.SendMessage(hwndChild, BN_CLICKED, 0, 0);

                if (Win32.WindowContainsTextString(proc.MainWindowHandle, "Checking end."))
                    proc.Kill();
            }

            File.Delete(filename);
            return true;
        }

        public bool TestInstallFode(string game, string tempDir, string installDir)
        {
            string filename = Path.Combine(tempDir, "FOUpdater.zip");
            string url = "http://fode.eu/files/download/2-fonline-desert-europe-game-client/";

            ProgressDownloader downloader = new ProgressDownloader();
            downloader.Download(game, url, filename);

            if (!File.Exists(filename))
            {
                MessageBox.Show(filename + " not found after download!");
                return false;
            }

            ZipFile zip = ZipFile.Read(filename);
            zip.ExtractSelectedEntries("*.*", string.Empty, installDir);

            Process proc = Process.Start(installDir + "\\" + "FOUpdater.exe");
            while (!proc.HasExited)
            {
                if (Win32.WindowContainsTextString("FOUpdater v0.2.1", "Update not needed") ||
                    Win32.WindowContainsTextString("FOUpdater v0.2.1", "Updated "))
                    proc.Kill();
            }

            // Delete temporary file
            File.Delete(filename);

            MessageBox.Show("Updater is done!");
            return true;
        }

        private bool FileExists(DAT dat, string fileName)
        {
            return dat.FileList.Exists(x => x.FileName.ToLower() == fileName);
        }

        public bool TestResolve(string name, string filePath)
        {
           DatReaderError status;
           string datPath = filePath;
           DAT loadedDat = DATReader.ReadDat(datPath, out status);
           if (status.Error != DatError.Success)
           {
               MessageBox.Show("Error loading " + datPath + ": " + Environment.NewLine + status.Message);
               return false;
           }
           bool valid = false;
           if (name.ToLower() == "master.dat")
           {
               valid = (this.FileExists(loadedDat, "ai.txt") &&
                        this.FileExists(loadedDat,  "vault13.gam") &&
                        this.FileExists(loadedDat,  "maps.txt") &&
                        this.FileExists(loadedDat,  "adb001.frm") &&
                        this.FileExists(loadedDat,  "acavcol1.frm")
                   );
           }
           if (name.ToLower() == "critter.dat")
           {
               valid = (this.FileExists(loadedDat,  "critters.lst") &&
                        this.FileExists(loadedDat,  "hanpwraa.frm") &&
                        this.FileExists(loadedDat,  "hanpwral.frm") &&
                        this.FileExists(loadedDat,  "hanpwrbi.frm") &&
                        this.FileExists(loadedDat,  "hanpwrbl.frm")
                   );
           }
           loadedDat.Close();
           return valid;
        }
    }
}
