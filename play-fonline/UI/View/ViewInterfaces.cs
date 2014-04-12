namespace PlayFOnline.UI.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FOQuery.Data;
    using PlayFOnline.Core;
    using PlayFOnline.UI.Presenter;

    public interface IBaseView
    {
        void ShowInfo(string infoMsg);
        void ShowError(string errorMsg);
        string GetFolderPath();
        bool AskYesNoQuestion(string question, string title);
    }

    public interface IDownloadView
    {
        event EventHandler CancelDownload;

        void Load();
        void Close();
        void Show();
        void SetTitle(string text);
        void SetStatus(string text);
        void ShowError(string error);

        void SetDownloadUrl(string text);
        void UpdateProgress(int percentage, string statusText);
    }

    public interface IInstallView : IBaseView
    {
        event EventHandler NextStep;
        event EventHandler PreviousStep;
        event EventHandler InstallPathSelect;
        event ItemEventHandler<FOGameDependency> SelectDependencyPath;

        void Close();
        void Show();
        void Load();

        bool ReviewCode();
        void SetTitle(string text);
        void SetLogo(string fileName);
        void SetSetupText(string text);
        void SetInstallPath(string text);
        void SetDependencyPath(FOGameDependency dependency, string path);
        void SetDependencies(List<FOGameDependency> dependencies);
        void SetStep(SetupStep currentStep, SetupStep step);
        string SelectFileName(string name);
    }

    public interface IMainView : IBaseView
    {
        event EventHandler RefreshServers;
        event EventHandler FoDevLinkClicked;
        event EventHandler<FOGameInfo> InstallGame;
        event EventHandler<FOGameInfo> ChangedGame;
        event ItemEventHandler<string> LaunchProgram;
        event ItemEventHandler<bool> ShowOfflineChanged;

        new bool AskYesNoQuestion(string question, string title);
        new string GetFolderPath();

        new void ShowError(string errorMsg);
        new void ShowInfo(string infoMsg);
        void RefreshServerList(List<FOGameInfo> servers);

        void UpdateStatusBar(string text);
        UISettings GetWindowProperties();
        void ClearGameSelection();
        void AddInstallButton();
        void SelectGame(FOGameInfo game, List<FOGameLaunchProgram> programs);
        void SetWindowProperties(UISettings settings);
        void SetTitle(string title);

        void Load();
        void StartApplication();
    }


}
