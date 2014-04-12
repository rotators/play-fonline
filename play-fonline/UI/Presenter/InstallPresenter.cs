namespace PlayFOnline.UI.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Net;
    using NLog;
    using FOQuery.Data;
    using PlayFOnline.UI.View;
    using PlayFOnline.Scripts;

    public enum SetupStep
    {
        SelectPath,
        SelectDependencies,
        InstallPreview,
        Install
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class InstallPresenter
    {
        IInstallView view;
        //private CheckBox chkReviewScript;
        private SetupStep currentStep;
        private FOGameInfo game;

        private InstallHandler installHandler;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private LogoManager logoManager;
        private string scriptPath;
        private string installPath;
        private string tempPath;

        private Dictionary<FOGameDependency, string> selectedDependencies = new Dictionary<FOGameDependency, string>();

        public bool IsSuccess { get; set; }

        public InstallPresenter(IInstallView view, FOGameInfo game, InstallHandler installHandler, LogoManager logoManager, string scriptPath, string tempPath)
        {
            this.view = view;
            this.game = game;
            this.scriptPath = scriptPath;
            this.installHandler = installHandler;
            this.logoManager = logoManager;
            this.tempPath = tempPath;
        }

        void OnInstallPathSelect(object sender, EventArgs e)
        {
            string path = this.view.GetFolderPath();
            if (String.IsNullOrEmpty(path))
                return;

            this.view.SetInstallPath(path);
            installPath = path;
        }

        void OnDependencyPathSelect(object sender, ItemEventArgs<FOGameDependency> dependency)
        {
            string fileName = this.view.SelectFileName(dependency.Item.Name);
            this.VerifyDependency(fileName, dependency.Item);
        }

        void OnNextStep(object sender, EventArgs e)
        {
            this.Next();
        }

        void OnPreviousStep(object sender, EventArgs e)
        {
            this.Back();
        }

        public string GetInstallPath() { return this.installPath; }

        public void Back()
        {
            if (this.currentStep == SetupStep.SelectDependencies) this.SetStep(SetupStep.SelectPath);
            if (this.currentStep == SetupStep.InstallPreview) this.SetStep(SetupStep.SelectDependencies);
        }

        public void Next()
        {
            if (this.currentStep == SetupStep.SelectPath)
            {
                if (this.installHandler.GetDependencies(this.game.Id) == null)
                    this.SetStep(SetupStep.InstallPreview);
                else
                    this.SetStep(SetupStep.SelectDependencies);
            }
            else if (this.currentStep == SetupStep.SelectDependencies) this.SetStep(SetupStep.InstallPreview);
            else if (this.currentStep == SetupStep.InstallPreview) this.SetStep(SetupStep.Install);
        }

        public void Show()
        {
            this.view.InstallPathSelect += OnInstallPathSelect;
            this.view.NextStep += OnNextStep;
            this.view.PreviousStep += OnPreviousStep;
            this.view.SelectDependencyPath += OnDependencyPathSelect;

            this.view.Load();
            this.view.SetLogo(this.logoManager.GetLogoPath(game.Id));
            this.view.SetTitle("Installing - " + game.Name);
            this.SetStep(SetupStep.SelectPath);
            this.view.Show();
        }

        private void SetStep(SetupStep step)
        {
            if (step == SetupStep.SelectPath)
            {
                this.view.SetSetupText("Setup will install " + this.game.Name + " into the selected folder." + Environment.NewLine + Environment.NewLine +
                    "To continue, click next. If you want to select a different folder, click Browse.");
            }
            else if (step == SetupStep.SelectDependencies)
            {
                this.view.SetSetupText("This game has a few dependencies, this means you'll need to own this content to play the game. " 
                    + Environment.NewLine + Environment.NewLine + "This usually includes the original Fallout 2 content.");
                this.view.SetDependencies(installHandler.GetDependencies(game.Id));
            }
            else if (step == SetupStep.InstallPreview)
            {
                this.view.SetSetupText("You are now ready to install. If you want to review the script that will be run, please do so now.");
            }
            else if (step == SetupStep.Install)
            {
                this.view.Close();
                if (!this.installHandler.InstallGame(this.game, this.scriptPath, this.tempPath, this.installPath, selectedDependencies.Values.ToList()))
                {
                    this.view.ShowError(this.installHandler.GetInstallError());
                    this.IsSuccess = false;
                    return;
                }
                this.IsSuccess = true;
            }

            this.view.SetStep(currentStep, step);
            this.currentStep = step;
        }

        public bool VerifyDependency(string fileName, FOGameDependency dependency)
        {
            if (!File.Exists(fileName))
            {
                this.view.ShowError(string.Format("{0} doesn't exist", fileName));
                return false;
            }
            if (string.IsNullOrEmpty(dependency.Script.Path))
            {
                if (!string.IsNullOrEmpty(dependency.Script.Url))
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        try
                        {
                            string fullPath = Path.Combine(this.scriptPath, Utils.GetFilenameFromUrl(dependency.Script.Url));

                            if (!File.Exists(fullPath))
                            {
                                webClient.Proxy = null;
                                webClient.DownloadFile(dependency.Script.Url, fullPath);
                            }
                            dependency.Script.Path = fullPath;
                        }
                        catch (WebException ex)
                        {
                            this.view.ShowError("Failed to download " + dependency.Script.Url + ":" + ex.Message);
                            return false;
                        }
                    }
                }
                else
                    this.view.ShowError("No script available for verifying dependency " + dependency.Name);
            }

            ResolveHost resolveHost = new ResolveHost();
            bool chooseNew = false;
            do
            {
                if (chooseNew)
                {
                    fileName = this.view.SelectFileName(dependency.Name);
                }
                chooseNew = false;
                if (!resolveHost.RunResolveScript(dependency.Script.Path, dependency.Name, fileName))
                {
                    chooseNew = !this.view.AskYesNoQuestion(fileName + " doesn't seem to be a valid file for " + dependency.Name + ", do you want to use it anyway?", "Play FOnline");
                }
            }
            while (chooseNew);
            this.view.SetDependencyPath(dependency, fileName);
            this.selectedDependencies[dependency] = fileName;
            return true;
        }
    }
}
