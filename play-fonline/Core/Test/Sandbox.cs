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

    public class Sandbox
    {
        public Sandbox()
        {
            
        }

        public bool TestInstallReloaded(string game, string tempDir, string installDir)
        {
            string filename = tempDir + Path.DirectorySeparatorChar + "FOnlineReloaded-Full.zip";
            frmDownload download = new frmDownload(game, "http://www.fonline-reloaded.net/files/dl_fullclient.php", filename);
            if (!download.IsDisposed)
                download.ShowDialog();
            
            if (!File.Exists(filename))
            {
                MessageBox.Show(filename + " not found after download!");
                return false;
            }

            ZipFile zip;
            //zip.ExtractSelectedEntries("*.*", "", installDir, ExtractExistingFileAction.OverwriteSilently)
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

        public bool TestInstallFode(string tempDir, string installDir)
        {
            string filename = tempDir + Path.DirectorySeparatorChar + "FOUpdater.zip";
            frmDownload download = new frmDownload("FODE", "http://fode.eu/files/download/2-fonline-desert-europe-game-client/", filename);
            if (!download.IsDisposed)
                download.ShowDialog();

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

        public bool TestResolve(string name, string filePath)
        {
           DatReaderError status;
           string datPath = filePath;
           DAT loadedDat = DATReader.ReadDat(datPath, out status);
           if (status.Error != DatError.Success)
           {
               MessageBox.Show("Error loading " + datPath + ": " + Environment.NewLine + status.Message);
           }
           bool valid = false;
           if (name.ToLower() == "master.dat")
           {
               valid = loadedDat.FileList.Exists(x => x.FileName.ToLower() == "ai.txt") &&
                       loadedDat.FileList.Exists(x => x.FileName.ToLower() == "vault13.gam") &&
                       loadedDat.FileList.Exists(x => x.FileName.ToLower() == "maps.txt") &&
                       loadedDat.FileList.Exists(x => x.FileName.ToLower() == "adb001.frm") &&
                       loadedDat.FileList.Exists(x => x.FileName.ToLower() == "acavcol1.frm");
           }
           if (name.ToLower() == "critter.dat")
           {
               valid = loadedDat.FileList.Exists(x => x.FileName.ToLower() == "critters.lst") &&
                       loadedDat.FileList.Exists(x => x.FileName.ToLower() == "hanpwraa.frm") &&
                       loadedDat.FileList.Exists(x => x.FileName.ToLower() == "hanpwral.frm") &&
                       loadedDat.FileList.Exists(x => x.FileName.ToLower() == "hanpwrbi.frm") &&
                       loadedDat.FileList.Exists(x => x.FileName.ToLower() == "hanpwrbl.frm");
           }
           loadedDat.Close();
           return valid;
        }
    }
}
