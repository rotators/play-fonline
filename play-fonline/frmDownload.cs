namespace PlayFOnline
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Windows.Forms;

    /// <summary>
    /// Form that displays downloading.
    /// Based on https://github.com/wipe2238/FOUpdaterEx/blob/master/FOUpdater/frmMain.cs
    /// Thanks Wipe!
    /// </summary>
    public partial class frmDownload : Form
    {
        public frmDownload()
        {
            this.InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams result = base.CreateParams;
                if (Environment.OSVersion.Platform == PlatformID.Win32NT
                    && Environment.OSVersion.Version.Major >= 6)
                {
                    result.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                }

                return result;
            }
        }
        
        private void buttonCancel_Click_1(object sender, EventArgs e)
        {

        }
    }
}