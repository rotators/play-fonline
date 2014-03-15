using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PlayFO
{
    public partial class frmMain : Form
    {
        FOSettings settings;
        FOServerQuery query;

        public frmMain()
        {
            InitializeComponent();

            settings = SettingsManager.LoadSettings();
            LoadFormSettings();

            query = new FOServerQuery(settings.configURL, settings.statusURL);

            lstGames.SetObjects(query.GetOnlineServers());

            this.olvPlayers.AspectToStringConverter = delegate(object x)
            {
                int Players = (int)x;
                if (Players == -1)
                    return "Offline";
                return Players.ToString();
            };

            this.olvInstallPath.AspectToStringConverter = delegate(object x)
            {
                string Path = (string)x;
                if (String.IsNullOrWhiteSpace(Path))
                    return "Not installed/added";
                return Path;
            };

        }

        private void LoadFormSettings()
        {
            if (settings.UI == null) return;

            this.DesktopLocation = new Point(settings.UI.x, settings.UI.y);
            this.Width = settings.UI.width;
            this.Height = settings.UI.height;
        }

        private void Exit()
        {
            settings.UI = new UISettings();
            settings.UI.x = this.DesktopLocation.X;
            settings.UI.y = this.DesktopLocation.Y;
            settings.UI.width = this.Width;
            settings.UI.height = this.Height;

            SettingsManager.SaveSettings(settings);
            Environment.Exit(0);
        }

        private void chkShowOffline_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowOffline.Checked)
                lstGames.SetObjects(query.GetServers(true));
            else
                lstGames.SetObjects(query.GetOnlineServers());
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Exit();
        }

        private void lstGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            FOGameInfo Game = (FOGameInfo)lstGames.SelectedObject;

            bool installed = !String.IsNullOrWhiteSpace(Game.InstallPath);

            btnPlay.Enabled = installed;
            btnInstall.Enabled = !installed;
            btnGameConfig.Enabled = installed;
            btnOptions.Enabled = installed;
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            Script script = new Script();
        }
    }
}
