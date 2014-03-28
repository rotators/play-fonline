namespace PlayFO
{
    partial class frmDownload
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDownload));
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.groupMain = new System.Windows.Forms.GroupBox();
            this.progressFile = new System.Windows.Forms.ProgressBar();
            this.labelDetailed = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.labelDownloading = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupMain
            // 
            this.groupMain.AutoSize = true;
            this.groupMain.Controls.Add(this.labelDownloading);
            this.groupMain.Controls.Add(this.progressFile);
            this.groupMain.Controls.Add(this.labelDetailed);
            this.groupMain.Location = new System.Drawing.Point(6, 6);
            this.groupMain.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.groupMain.Name = "groupMain";
            this.groupMain.Padding = new System.Windows.Forms.Padding(10, 2, 10, 10);
            this.groupMain.Size = new System.Drawing.Size(330, 83);
            this.groupMain.TabIndex = 0;
            this.groupMain.TabStop = false;
            // 
            // progressFile
            // 
            this.progressFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressFile.ForeColor = System.Drawing.Color.Green;
            this.progressFile.Location = new System.Drawing.Point(10, 33);
            this.progressFile.Name = "progressFile";
            this.progressFile.Size = new System.Drawing.Size(310, 18);
            this.progressFile.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressFile.TabIndex = 3;
            this.progressFile.Value = 25;
            // 
            // labelDetailed
            // 
            this.labelDetailed.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelDetailed.Location = new System.Drawing.Point(10, 15);
            this.labelDetailed.Margin = new System.Windows.Forms.Padding(0);
            this.labelDetailed.Name = "labelDetailed";
            this.labelDetailed.Size = new System.Drawing.Size(310, 18);
            this.labelDetailed.TabIndex = 3;
            this.labelDetailed.Text = "[Download status]";
            this.labelDetailed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelButtons
            // 
            this.panelButtons.AutoSize = true;
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(6, 128);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(10);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(330, 0);
            this.panelButtons.TabIndex = 4;
            // 
            // labelDownloading
            // 
            this.labelDownloading.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelDownloading.Enabled = false;
            this.labelDownloading.Location = new System.Drawing.Point(10, 60);
            this.labelDownloading.Name = "labelDownloading";
            this.labelDownloading.Size = new System.Drawing.Size(310, 13);
            this.labelDownloading.TabIndex = 7;
            this.labelDownloading.Text = "[URL]...";
            this.labelDownloading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(6, 97);
            this.buttonCancel.MaximumSize = new System.Drawing.Size(75, 23);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // frmDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(342, 128);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupMain);
            this.Controls.Add(this.panelButtons);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 166);
            this.Name = "frmDownload";
            this.Padding = new System.Windows.Forms.Padding(6, 6, 6, 0);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FOPlay Downloader";
            this.Load += new System.EventHandler(this.frmUpdater_Load);
            this.groupMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.GroupBox groupMain;
        private System.Windows.Forms.ProgressBar progressFile;
        private System.Windows.Forms.Label labelDetailed;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Label labelDownloading;
        private System.Windows.Forms.Button buttonCancel;
    }
}