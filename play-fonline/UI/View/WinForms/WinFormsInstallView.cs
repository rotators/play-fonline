namespace PlayFOnline.UI.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Drawing;
    using PlayFOnline.UI.Presenter;
    using FOQuery.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinFormsInstallView : WinFormsBaseView, IInstallView
    {
        frmInstallMain Form;
        Button btnBrowsePath;
        private Dictionary<SetupStep, List<Control>> setupControls = new Dictionary<SetupStep, List<Control>>();
        private TextBox txtInstallPath;

        public event EventHandler Cancel;
        public event EventHandler NextStep;
        public event EventHandler PreviousStep;
        public event EventHandler InstallPathSelect;
        public event ItemEventHandler<FOGameDependency> SelectDependencyPath;
        //event EventHandler<FOGameInfo> InstallGame;
        //event ItemEventHandler<string> LaunchProgram;
        //event ItemEventHandler<bool> ShowOfflineChanged;

        public void Load()
        {
            this.Form = new frmInstallMain();
            this.Form.txtGameInfo.Text = string.Empty;
            this.Form.txtGameInfo.Hide();
            this.txtInstallPath = new TextBox();
            this.txtInstallPath.Width = 400;
            this.btnBrowsePath = new Button();
            this.btnBrowsePath.Text = "Browse...";
            this.Form.lblSpaceRequired.Visible = false; // TODO

            this.btnBrowsePath.Click += InstallPathSelect;
            this.Form.btnNext.Click += NextStep;
            this.Form.btnBack.Click += PreviousStep;
            this.Form.btnCancel.Click += Cancel;
        }

        public void Close()
        {
            this.Form.Close();
            this.Form.Dispose();
        }

        public void Show()
        {
            this.Form.ShowDialog();
        }

        public string GetInstallPath()
        {
            return txtInstallPath.Text;
        }

        public void SetInstallPath(string text)
        {
            txtInstallPath.Text = text;
        }

        public void SetDependencyPath(FOGameDependency dependency, string path)
        {
            TextBox txt = this.FindByDependency(this.Form, dependency);
            txt.Text = path;
        }

        private TextBox FindByDependency(Control root, FOGameDependency dependency)
        {
            if (root == null)
                return null;
            if (root.Tag is FOGameDependency && (FOGameDependency)root.Tag == dependency)
                return (TextBox)root;
            return (from Control control in root.Controls select FindByDependency(control, dependency)).FirstOrDefault(x => x != null);
        }

        public void SetSetupText(string text)
        {
            Form.lblSetupText.Text = text;
        }

        public string SelectFileName(string name)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = name + "|*.*";
            if (openFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return string.Empty;
            return openFile.FileName;
        }

        private Button CreateBrowseButton(TextBox attachedTo)
        {
            Button btnBrowse = new Button();
            btnBrowse.Text = "Browse...";
            btnBrowse.Tag = attachedTo;
            btnBrowse.Click += new EventHandler(btnBrowse_Click);
            return btnBrowse;
        }

        void btnBrowse_Click(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)((Button)sender).Tag;
            SelectDependencyPath(this, new ItemEventArgs<FOGameDependency>((FOGameDependency)txt.Tag));
        }

        private TextBox CreatePathTextBox()
        {
            TextBox txtPath = new TextBox();
            txtPath.Width = 400;
            return txtPath;
        }

        public void SetDependencies(List<FOGameDependency> dependencies)
        {
            if (!this.setupControls.ContainsKey(SetupStep.SelectDependencies))
            {
                this.setupControls[SetupStep.SelectDependencies] = new List<Control>();
                foreach (FOGameDependency depend in dependencies)
                {
                    Label lbl = new Label();
                    lbl.Text = depend.Description;
                    lbl.AutoSize = true;
                    lbl.Margin = new Padding(0, 0, 0, 3);
                    this.setupControls[SetupStep.SelectDependencies].Add(lbl);
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
                    this.setupControls[SetupStep.SelectDependencies].Add(flow);
                }
            }
        }

        public void SetStep(SetupStep currentStep, SetupStep step)
        {
            if (this.setupControls.ContainsKey(currentStep))
                foreach (Control ctrl in this.setupControls[currentStep])
                    this.Form.flowArea.Controls.Remove(ctrl);

            this.Form.chkReviewCode.Visible = false;

            if (step == SetupStep.SelectPath)
            {
                this.Form.btnBack.Enabled = false;

                if (!this.setupControls.ContainsKey(SetupStep.SelectPath))
                {
                    this.setupControls[step] = new List<Control>();
                    FlowLayoutPanel flow = new FlowLayoutPanel();
                    flow.AutoSize = true;
                    flow.Controls.Add(this.txtInstallPath);
                    flow.Controls.Add(this.btnBrowsePath);
                    this.setupControls[step].Add(flow);
                }
            }
            else if (step == SetupStep.SelectDependencies)
            {
                this.Form.btnBack.Enabled = true;
                this.Form.btnNext.Text = "Next >";
            }
            else if (step == SetupStep.InstallPreview)
            {
                this.Form.btnNext.Text = "Install";
                this.Form.chkReviewCode.Visible = true;
            }

            if (this.setupControls.ContainsKey(step))
                foreach (Control ctrl in this.setupControls[step])
                    this.Form.flowArea.Controls.Add(ctrl);
        }
        
        public bool ReviewCode()
        {
            return this.Form.chkReviewCode.Checked;
        }

        public void SetTitle(string text)
        {
            this.Form.Text = text;
        }

        public void SetLogo(string fileName)
        {
            this.Form.pctGameLogo.Image = Image.FromFile(fileName);
        }
    }
}
