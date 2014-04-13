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
    using SharpCompress;
    using SharpCompress.Reader;
    using SharpCompress.Common;

    public class Sandbox
    {
        public bool TestInstallGoonHaven(string game, string tempDir, string installDir)
        {
            string filename = Path.Combine(tempDir, "Goonhaven.rar");
            string url = "http://goon-haven.ru/downloads/GoonHaven.rar";

            ProgressDownloader downloader = new ProgressDownloader();
            downloader.Download(game, url, filename);

            if (!File.Exists(filename))
            {
                MessageBox.Show(filename + " not found after download!");
                return false;
            }

            using (Stream stream = File.OpenRead(filename))
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        string path = Path.Combine(installDir, reader.Entry.FilePath.Replace("Client\\", ""));
                        path = path.Replace(Path.GetFileName(path), "");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        reader.WriteEntryToDirectory(path, ExtractOptions.Overwrite);
                    }
                }
            }
            return true;
        }

        public bool TestInstallTLAMK2(string game, string tempDir, string installDir)
        {
            string filename = Path.Combine(tempDir, "TLA Mk2 Client.exe");
            string url = "";

            ProgressDownloader downloader = new ProgressDownloader();
            downloader.Download(game, url, filename);

            if (!File.Exists(filename))
            {
                MessageBox.Show(filename + " not found after download!");
                return false;
            }

            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.WorkingDirectory = tempDir;
            procInfo.FileName = filename;

            Process proc = Process.Start(procInfo);
            System.Threading.Thread.Sleep(1000);
            while (!proc.HasExited)
            {
                Win32.GetChildWindows((IntPtr)proc.MainWindowHandle);
                List<IntPtr> handles = (List<IntPtr>)Win32.GetChildWindows((IntPtr)proc.MainWindowHandle).ToList();
                if (handles.Count() == 16)
                {
                    // Set path
                    //const uint WM_SETTEXT = 0x000C;
                    //Win32.SendMessageEx(handles[3], WM_SETTEXT, 0, installDir);

                    // Click install
                    const int BN_CLICKED = 245;
                    Win32.SendMessageEx(handles[10], BN_CLICKED, 0, 0);
                }
            }

            string tempInstall = Path.Combine(tempDir, "TLA Mk2 Client");

            if (!Directory.Exists(tempInstall))
            {
                MessageBox.Show(tempInstall + " not found!");
                return false;
            }

            Utils.DirectoryCopy(tempInstall, installDir, true);
            Directory.Delete(tempInstall, true);

            return true;
        }

        public bool TestInstallFOnline2(string game, string tempDir, string installDir)
        {
            string filename = Path.Combine(tempDir, "Fonline2Season2.zip");
            string url = "http://www.mediafire.com/download/7k8l07d95ylpk8h/Fonline2Season2.zip";

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
                if (entry.FileName != "Fonline2/")
                {
                    entry.FileName = entry.FileName.Replace("Fonline2/", string.Empty);
                    entry.Extract(installDir, ExtractExistingFileAction.OverwriteSilently);
                }
            });
            zip.Dispose();

            try
            {
                File.Delete(filename);
            }
            catch (IOException ex)
            {
                MessageBox.Show(string.Format("Error when deleting {0}: {1}", filename, ex.Message));
            }
            return true;
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
                    entry.Extract(installDir, ExtractExistingFileAction.OverwriteSilently);
                }
            });
            zip.Dispose();

            string updater = Path.Combine(installDir, "Updater.exe");

            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.WorkingDirectory = installDir;
            procInfo.FileName = updater;
            if (!File.Exists(updater))
            {
                MessageBox.Show("{0} not found, can't update!", updater);
                return false;
            }

            Process proc = Process.Start(procInfo);
            while (!proc.HasExited)
            {
                IntPtr hwndChild = Win32.FindWindowEx((IntPtr)proc.MainWindowHandle, IntPtr.Zero, "TButton", "Check");

                const int BN_CLICKED = 245;
                Win32.SendMessageEx(hwndChild, BN_CLICKED, 0, 0);

                if (Win32.WindowContainsTextString(proc.MainWindowHandle, "Checking end."))
                    proc.Kill();
            }
            try
            {
                File.Delete(filename);
            }
            catch (IOException ex)
            {
                MessageBox.Show(string.Format("Error when deleting {0}: {1}", filename, ex.Message));
            }
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
