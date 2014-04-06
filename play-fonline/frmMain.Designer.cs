﻿namespace PlayFOnline
{
    public partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public System.Windows.Forms.FlowLayoutPanel flowMenu;
        public System.Windows.Forms.Button btnOptions;
        public System.Windows.Forms.Button btnInstall;
        public System.Windows.Forms.CheckBox chkShowOffline;
        public BrightIdeasSoftware.ObjectListView lstGames;
        public BrightIdeasSoftware.OLVColumn olvName;
        public BrightIdeasSoftware.OLVColumn olvPlayers;
        public BrightIdeasSoftware.OLVColumn olvColumn2;
        public BrightIdeasSoftware.OLVColumn olvColumn3;
        public BrightIdeasSoftware.OLVColumn olvColumn4;
        public BrightIdeasSoftware.OLVColumn olvColumn5;
        public BrightIdeasSoftware.OLVColumn olvInstallPath;
        public BrightIdeasSoftware.OLVColumn olvPing;
        public System.Windows.Forms.StatusStrip statusBar;
        public System.Windows.Forms.Button btnRefresh;
        public System.Windows.Forms.ToolStripStatusLabel statusBarLabel;
        public System.Windows.Forms.LinkLabel linkFoDev;
        public System.Windows.Forms.BindingSource frmMainBindingSource;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.flowMenu = new System.Windows.Forms.FlowLayoutPanel();
            this.btnInstall = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.chkShowOffline = new System.Windows.Forms.CheckBox();
            this.lstGames = new BrightIdeasSoftware.ObjectListView();
            this.olvName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvPlayers = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvPing = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvInstallPath = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusBarLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.linkFoDev = new System.Windows.Forms.LinkLabel();
            this.frmMainBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.flowMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstGames)).BeginInit();
            this.statusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frmMainBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // flowMenu
            // 
            this.flowMenu.Controls.Add(this.btnInstall);
            this.flowMenu.Controls.Add(this.btnOptions);
            this.flowMenu.Location = new System.Drawing.Point(5, 12);
            this.flowMenu.Name = "flowMenu";
            this.flowMenu.Size = new System.Drawing.Size(119, 389);
            this.flowMenu.TabIndex = 6;
            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(3, 3);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(103, 28);
            this.btnInstall.TabIndex = 8;
            this.btnInstall.Text = "Install/Add";
            this.btnInstall.UseVisualStyleBackColor = true;
            // 
            // btnOptions
            // 
            this.btnOptions.Enabled = false;
            this.btnOptions.Location = new System.Drawing.Point(3, 37);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(103, 28);
            this.btnOptions.TabIndex = 6;
            this.btnOptions.Text = "Options";
            this.btnOptions.UseVisualStyleBackColor = true;
            // 
            // chkShowOffline
            // 
            this.chkShowOffline.AutoSize = true;
            this.chkShowOffline.Location = new System.Drawing.Point(130, 12);
            this.chkShowOffline.Name = "chkShowOffline";
            this.chkShowOffline.Size = new System.Drawing.Size(121, 17);
            this.chkShowOffline.TabIndex = 8;
            this.chkShowOffline.Text = "Show offline servers";
            this.chkShowOffline.UseVisualStyleBackColor = true;
            // 
            // lstGames
            // 
            this.lstGames.AllColumns.Add(this.olvName);
            this.lstGames.AllColumns.Add(this.olvPlayers);
            this.lstGames.AllColumns.Add(this.olvColumn2);
            this.lstGames.AllColumns.Add(this.olvColumn3);
            this.lstGames.AllColumns.Add(this.olvColumn4);
            this.lstGames.AllColumns.Add(this.olvPing);
            this.lstGames.AllColumns.Add(this.olvInstallPath);
            this.lstGames.AllColumns.Add(this.olvColumn5);
            this.lstGames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvName,
            this.olvPlayers,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4,
            this.olvPing,
            this.olvInstallPath,
            this.olvColumn5});
            this.lstGames.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstGames.FullRowSelect = true;
            this.lstGames.HasCollapsibleGroups = false;
            this.lstGames.Location = new System.Drawing.Point(130, 35);
            this.lstGames.Name = "lstGames";
            this.lstGames.ShowGroups = false;
            this.lstGames.Size = new System.Drawing.Size(662, 355);
            this.lstGames.TabIndex = 7;
            this.lstGames.UseCompatibleStateImageBehavior = false;
            this.lstGames.UseHyperlinks = true;
            this.lstGames.View = System.Windows.Forms.View.Details;
            // 
            // olvName
            // 
            this.olvName.AspectName = "Name";
            this.olvName.FillsFreeSpace = true;
            this.olvName.Text = "Name";
            // 
            // olvPlayers
            // 
            this.olvPlayers.AspectName = "Status.PlayersStr";
            this.olvPlayers.Text = "Players";
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Website";
            this.olvColumn2.FillsFreeSpace = true;
            this.olvColumn2.Hyperlink = true;
            this.olvColumn2.Text = "Website";
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Host";
            this.olvColumn3.Text = "Host";
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "Port";
            this.olvColumn4.Text = "Port";
            // 
            // olvPing
            // 
            this.olvPing.AspectName = "Status.Latency";
            this.olvPing.Text = "Ping";
            // 
            // olvInstallPath
            // 
            this.olvInstallPath.AspectName = "InstallPath";
            this.olvInstallPath.Text = "Install Path";
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "Source";
            this.olvColumn5.Hyperlink = true;
            this.olvColumn5.Text = "Source Code";
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 404);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(804, 22);
            this.statusBar.TabIndex = 9;
            this.statusBar.Text = "statusStrip1";
            // 
            // statusBarLabel
            // 
            this.statusBarLabel.Name = "statusBarLabel";
            this.statusBarLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(257, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(87, 23);
            this.btnRefresh.TabIndex = 10;
            this.btnRefresh.Text = "Update list";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // linkFoDev
            // 
            this.linkFoDev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkFoDev.AutoSize = true;
            this.linkFoDev.Location = new System.Drawing.Point(709, 12);
            this.linkFoDev.Name = "linkFoDev";
            this.linkFoDev.Size = new System.Drawing.Size(83, 13);
            this.linkFoDev.TabIndex = 14;
            this.linkFoDev.TabStop = true;
            this.linkFoDev.Text = "http://fodev.net";
            // 
            // frmMainBindingSource
            // 
            this.frmMainBindingSource.DataSource = typeof(PlayFOnline.frmMain);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 426);
            this.Controls.Add(this.linkFoDev);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.chkShowOffline);
            this.Controls.Add(this.lstGames);
            this.Controls.Add(this.flowMenu);
            this.MinimumSize = new System.Drawing.Size(460, 250);
            this.Name = "frmMain";
            this.Text = "Play FOnline [Version]";
            this.flowMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstGames)).EndInit();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frmMainBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }
}