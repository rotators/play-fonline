namespace PlayFOnline
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Windows.Forms;
    using NLog;
    using PlayFOnline.Data;
    using PlayFOnline.Scripts;

    public partial class frmInstallMain : Form
    {
        private Button btnBrowsePath;
        //private CheckBox chkReviewScript;
        private SetupStep currentStep;
        private FOGameInfo game;

        private InstallHandler installHandler;
        private Logger logger = LogManager.GetLogger("frmInstallMain");
        private LogoManager logoManager;
        private string scriptPath;
        private Dictionary<SetupStep, List<Control>> setupControls = new Dictionary<SetupStep, List<Control>>();
        private TextBox txtInstallPath;
        public frmInstallMain(FOGameInfo game, InstallHandler installHandler, LogoManager logoManager, string scriptPath)
        {
            this.game = game;
            this.scriptPath = scriptPath;
            this.installHandler = installHandler;
            this.logoManager = logoManager;
            this.InitializeComponent();

            pctGameLogo.Image = Image.FromFile(logoManager.GetLogoPath(game.Id));
        }

        private enum SetupStep
        {
            SelectPath,
            SelectDependencies,
            Install
        }

        public bool IsSuccess { get; set; }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (this.currentStep == SetupStep.SelectDependencies) this.SetStep(SetupStep.SelectPath);
            if (this.currentStep == SetupStep.Install) this.SetStep(SetupStep.SelectDependencies);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            TextBox txt = (TextBox)btn.Tag;
            FOGameDependency depend = (FOGameDependency)txt.Tag;

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = depend.Name + "|*.*";
            if (openFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            if (openFile.CheckFileExists)
            {
                if (string.IsNullOrEmpty(depend.Script.Path))
                {
                    if (!string.IsNullOrEmpty(depend.Script.Url))
                    {
                        using (var webClient = new System.Net.WebClient())
                        {
                            try
                            {
                                string fullPath = this.scriptPath + Path.DirectorySeparatorChar + Utils.GetFilenameFromUrl(depend.Script.Url);

                                if (!File.Exists(fullPath))
                                {
                                    webClient.Proxy = null;
                                    webClient.DownloadFile(depend.Script.Url, fullPath);
                                }
                                depend.Script.Path = fullPath;
                            }
                            catch (WebException ex)
                            {
                                this.MessageBoxError("Failed to download " + depend.Script.Url + ":" + ex.Message);
                                return;
                            }
                        }
                    }
                    else
                        this.MessageBoxError("No script available for verifying dependency " + depend.Name);
                }

                ResolveHost resolveHost = new ResolveHost();
                bool chooseNew = false;
                do
                {
                    if (chooseNew) openFile.ShowDialog();
                    chooseNew = false;
                    if (!resolveHost.RunResolveScript(depend.Script.Path, depend.Name, openFile.FileName))
                    {
                        chooseNew = MessageBox.Show(openFile.FileName + " doesn't seem to be a valid file for " + depend.Name + ", do you want to use it anyway?", "Play FOnline", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No;
                    }
                }
                while (chooseNew);
            }

            txt.Text = openFile.FileName;
        }

        private void btnBrowsePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = false;
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            this.txtInstallPath.Text = folderBrowser.SelectedPath;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (this.currentStep == SetupStep.SelectPath)
            {
                if (this.installHandler.GetDependencies(this.game.Id) == null)
                    this.SetStep(SetupStep.Install);
                else
                    this.SetStep(SetupStep.SelectDependencies);
            }
            else if (this.currentStep == SetupStep.SelectDependencies) this.SetStep(SetupStep.Install);
        }

        private Button CreateBrowseButton(TextBox attachedTo)
        {
            Button btnBrowse = new Button();
            btnBrowse.Text = "Browse...";
            btnBrowse.Tag = attachedTo;
            btnBrowse.Click += new EventHandler(this.btnBrowse_Click);
            return btnBrowse;
        }

        private TextBox CreatePathTextBox()
        {
            TextBox txtPath = new TextBox();
            txtPath.Width = 400;
            return txtPath;
        }

        private void frmInstallMain_Load(object sender, EventArgs e)
        {
            this.Text = this.Text.Replace("[Game]", this.game.Name);

            this.txtGameInfo.Text = string.Empty;
            this.txtGameInfo.Hide();
            this.txtInstallPath = new TextBox();
            this.txtInstallPath.Width = 400;
            this.btnBrowsePath = new Button();
            this.btnBrowsePath.Text = "Browse...";
            this.btnBrowsePath.Click += new EventHandler(this.btnBrowsePath_Click);

            lblSpaceRequired.Visible = false; // TODO

            this.SetStep(SetupStep.SelectPath);
        }

        private void MessageBoxError(string message)
        {
            MessageBox.Show(message);
            this.logger.Error(message);
        }

        private void SetStep(SetupStep step)
        {
            if (this.setupControls.ContainsKey(this.currentStep))
                foreach (Control ctrl in this.setupControls[this.currentStep])
                    this.flowArea.Controls.Remove(ctrl);

            if (step == SetupStep.SelectPath)
            {
                lblSetupText.Text = "Setup will install " + this.game.Name + " into the selected folder." + Environment.NewLine + Environment.NewLine +
                    "To continue, click next. If you want to select a different folder, click Browse.";

                this.btnBack.Enabled = false;

                if (!this.setupControls.ContainsKey(SetupStep.SelectPath))
                {
                    this.setupControls[step] = new List<Control>();
                    FlowLayoutPanel flow = new FlowLayoutPanel();
                    flow.AutoSize = true;
                    flow.Controls.Add(this.txtInstallPath);
                    flow.Controls.Add(this.btnBrowsePath);
                    this.setupControls[step].Add(flow);
                }

                foreach (Control ctrl in this.setupControls[step])
                    this.flowArea.Controls.Add(ctrl);
            }
            else if (step == SetupStep.SelectDependencies)
            {
                this.lblSetupText.Text = "This game has a few dependencies, this means you'll need to own this content to play the game. " + Environment.NewLine + Environment.NewLine + "This usually includes the original Fallout 2 content.";

                this.btnBack.Enabled = true;
                this.btnNext.Text = "Next >";

                if (!this.setupControls.ContainsKey(step))
                {
                    this.setupControls[step] = new List<Control>();
                    foreach (FOGameDependency depend in this.installHandler.GetDependencies(this.game.Id))
                    {
                        Label lbl = new Label();
                        lbl.Text = depend.Description;
                        lbl.AutoSize = true;
                        lbl.Margin = new Padding(0, 0, 0, 3);
                        this.setupControls[step].Add(lbl);
                        FlowLayoutPanel flow = new FlowLayoutPanel();
                        TextBox txt = this.CreatePathTextBox();
                        txt.ReadOnly = true;
                        txt.Margin = new Padding(0, 0, 3, 0);
                        txt.Tag = depend;
                        Button btn = this.CreateBrowseButton(txt);
                        btn.Margin = new Padding(3, 0, 3, 0);
                        flow.Margin = new Padding(0);
                        flow.AutoSize = true;
                        flow.Controls.Add(txt);
                        flow.Controls.Add(btn);
                        this.setupControls[step].Add(flow);
                    }
                }

                foreach (Control ctrl in this.setupControls[step])
                    this.flowArea.Controls.Add(ctrl);
            }
            else if (step == SetupStep.Install)
            {
                this.btnNext.Text = "Install";
            }

            this.currentStep = step;
        }
    }
}