using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NLog;
using System.Net;
using System.IO;
using PlayFOnline.Scripts;

namespace PlayFOnline
{
    public partial class frmInstallMain : Form
    {
        FOGameInfo game;

        TextBox txtInstallPath;
        Button btnBrowsePath;
        CheckBox chkReviewScript;

        SetupStep currentStep;
        InstallHandler installHandler;
        LogoManager logoManager;

        string scriptPath;

        public bool IsSuccess { get; set; }

        Dictionary<SetupStep, List<Control>> SetupControls = new Dictionary<SetupStep, List<Control>>();

        Logger logger = LogManager.GetLogger("frmInstallMain");

        public frmInstallMain(FOGameInfo game, InstallHandler installHandler, LogoManager logoManager, string scriptPath)
        {
            this.game = game;
            this.scriptPath = scriptPath;
            this.installHandler = installHandler;
            this.logoManager = logoManager;
            InitializeComponent();

            pctGameLogo.Image = Image.FromFile(logoManager.getLogoPath(game.Id));
        }

        enum SetupStep
        {
            SelectPath,
            SelectDependencies,
            Install
        }

        private void SetStep(SetupStep Step)
        {
            if(SetupControls.ContainsKey(currentStep))
                foreach (Control ctrl in SetupControls[currentStep])
                    flowArea.Controls.Remove(ctrl);

            if (Step == SetupStep.SelectPath)
            {
                lblSetupText.Text = "Setup will install " + game.Name + " into the selected folder." + Environment.NewLine + Environment.NewLine +
                    "To continue, click next. If you want to select a different folder, click Browse.";

                btnBack.Enabled = false;

                if (!SetupControls.ContainsKey(SetupStep.SelectPath))
                {
                    SetupControls[Step] = new List<Control>();
                    FlowLayoutPanel flow = new FlowLayoutPanel();
                    flow.AutoSize = true;
                    flow.Controls.Add(txtInstallPath);
                    flow.Controls.Add(btnBrowsePath);
                    SetupControls[Step].Add(flow);
                }

                foreach (Control ctrl in SetupControls[Step])
                    flowArea.Controls.Add(ctrl);
            }
            else if (Step == SetupStep.SelectDependencies)
            {
                lblSetupText.Text = "This game has a few dependencies, this means you'll need to own this content to play the game. " + Environment.NewLine + Environment.NewLine +  "This usually includes the original Fallout 2 content.";

                btnBack.Enabled = true;
                btnNext.Text = "Next >";

                if (!SetupControls.ContainsKey(Step))
                {
                    SetupControls[Step] = new List<Control>();
                    foreach (FOGameDependency depend in installHandler.GetDependencies(game.Id))
                    {
                        Label lbl = new Label();
                        lbl.Text = depend.Description;
                        lbl.AutoSize = true;
                        lbl.Margin = new Padding(0, 0, 0, 3);
                        SetupControls[Step].Add(lbl);
                        FlowLayoutPanel flow = new FlowLayoutPanel();
                        TextBox txt = CreatePathTextBox();
                        txt.ReadOnly = true;
                        txt.Margin = new Padding(0, 0, 3, 0);
                        txt.Tag = depend;
                        Button btn = CreateBrowseButton(txt);
                        btn.Margin = new Padding(3, 0, 3, 0);
                        flow.Margin =  new Padding(0);
                        flow.AutoSize = true;
                        flow.Controls.Add(txt);
                        flow.Controls.Add(btn);
                        SetupControls[Step].Add(flow);
                    }
                }

                foreach (Control ctrl in SetupControls[Step])
                    flowArea.Controls.Add(ctrl);
            }
            else if (Step == SetupStep.Install)
            {
                btnNext.Text = "Install";
            }

            currentStep = Step;
        }

        private void MessageBoxError(string Message)
        {
            MessageBox.Show(Message);
            logger.Error(Message);
        }

        private TextBox CreatePathTextBox()
        {
            TextBox txtPath = new TextBox();
            txtPath.Width = 400;
            return txtPath;
        }

        private Button CreateBrowseButton(TextBox attachedTo)
        {
            Button btnBrowse = new Button();
            btnBrowse.Text = "Browse...";
            btnBrowse.Tag = attachedTo;
            btnBrowse.Click +=new EventHandler(btnBrowse_Click);
            return btnBrowse;
        }

        void  btnBrowse_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            TextBox txt = (TextBox)btn.Tag;
            FOGameDependency depend = (FOGameDependency)txt.Tag;

            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Filter = depend.Name + "|*.*";
            if (OpenFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            if (OpenFile.CheckFileExists)
            {
                if (string.IsNullOrEmpty(depend.Script.Path))
                {
                    if (!string.IsNullOrEmpty(depend.Script.Url))
                    {
                        using (var webClient = new System.Net.WebClient())
                        {
                            try
                            {
                                string fullPath = scriptPath + Path.DirectorySeparatorChar + Utils.GetFilenameFromUrl(depend.Script.Url);

                                if (!File.Exists(fullPath))
                                {
                                    webClient.Proxy = null;
                                    webClient.DownloadFile(depend.Script.Url, fullPath);
                                }
                                depend.Script.Path = fullPath;
                            }
                            catch (WebException ex)
                            {
                                MessageBoxError("Failed to download " + depend.Script.Url + ":" + ex.Message);
                                return;
                            }
                        }
                    }
                    else
                        MessageBoxError("No script available for verifying dependency " + depend.Name);
                }

                ResolveHost resolveHost = new ResolveHost();
                bool chooseNew = false;
                do
                {
                    if (chooseNew) OpenFile.ShowDialog();
                    chooseNew = false;
                    if (!resolveHost.RunResolveScript(depend.Script.Path, depend.Name, OpenFile.FileName))
                    {
                        chooseNew = (MessageBox.Show(OpenFile.FileName + " doesn't seem to be a valid file for " + depend.Name + ", do you want to use it anyway?", "Play FOnline",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No);
                    }

                } while (chooseNew);
            }

            txt.Text = OpenFile.FileName;
                //settings.AddDependency(depend, OpenFile.FileName);
        }

        private void frmInstallMain_Load(object sender, EventArgs e)
        {
            this.Text = this.Text.Replace("[Game]", game.Name);
            // Load logo
            txtGameInfo.Text = "";
            txtGameInfo.Hide();
            txtInstallPath = new TextBox();
            txtInstallPath.Width = 400;
            btnBrowsePath = new Button();
            btnBrowsePath.Text = "Browse...";
            btnBrowsePath.Click += new EventHandler(btnBrowsePath_Click);

            lblSpaceRequired.Visible = false; // TODO

            SetStep(SetupStep.SelectPath);
        }

        void btnBrowsePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FolderBrowser = new FolderBrowserDialog();
            FolderBrowser.ShowNewFolderButton = false;
            if (FolderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            txtInstallPath.Text = FolderBrowser.SelectedPath;
           // game.InstallPath = FolderBrowser.SelectedPath;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentStep == SetupStep.SelectPath)
            {
                if (installHandler.GetDependencies(game.Id) == null)
                    SetStep(SetupStep.Install);
                else
                    SetStep(SetupStep.SelectDependencies);
            }
            else if (currentStep == SetupStep.SelectDependencies) SetStep(SetupStep.Install);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (currentStep == SetupStep.SelectDependencies) SetStep(SetupStep.SelectPath);
            if (currentStep == SetupStep.Install) SetStep(SetupStep.SelectDependencies);
        }
    }
}
