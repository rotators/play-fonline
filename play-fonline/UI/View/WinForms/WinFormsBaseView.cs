namespace PlayFOnline.UI.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    public class WinFormsBaseView
    {
        public void ShowInfo(string infoMsg)
        {
            MessageBox.Show(infoMsg, "Play FOnline", MessageBoxButtons.OK, MessageBoxIcon.Information);
            NLog.LogManager.GetCurrentClassLogger().Info(infoMsg);
        }

        public void ShowError(string errorMsg)
        {
            MessageBox.Show(errorMsg, "Play FOnline", MessageBoxButtons.OK, MessageBoxIcon.Error);
            NLog.LogManager.GetCurrentClassLogger().Error(errorMsg);
        }

        public string GetFolderPath()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = false;
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return string.Empty;
            return folderBrowser.SelectedPath;
        }

        public bool AskYesNoQuestion(string question, string title)
        {
            return (MessageBox.Show(question, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
        }
    }
}
