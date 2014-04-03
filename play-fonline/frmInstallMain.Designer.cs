namespace PlayFOnline
{
    public partial class frmInstallMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.PictureBox pctGameLogo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label lblSeperator;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSpaceRequired;
        private System.Windows.Forms.TextBox txtGameInfo;
        private System.Windows.Forms.FlowLayoutPanel flowArea;
        private System.Windows.Forms.Label lblSetupText;

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
            this.panelInfo = new System.Windows.Forms.Panel();
            this.txtGameInfo = new System.Windows.Forms.TextBox();
            this.lblSeperator = new System.Windows.Forms.Label();
            this.pctGameLogo = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.lblSpaceRequired = new System.Windows.Forms.Label();
            this.flowArea = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSetupText = new System.Windows.Forms.Label();
            this.panelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctGameLogo)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelInfo
            // 
            this.panelInfo.BackColor = System.Drawing.Color.White;
            this.panelInfo.Controls.Add(this.txtGameInfo);
            this.panelInfo.Controls.Add(this.pctGameLogo);
            this.panelInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelInfo.Location = new System.Drawing.Point(0, 0);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(555, 100);
            this.panelInfo.TabIndex = 1;
            // 
            // txtGameInfo
            // 
            this.txtGameInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtGameInfo.Location = new System.Drawing.Point(106, 3);
            this.txtGameInfo.Multiline = true;
            this.txtGameInfo.Name = "txtGameInfo";
            this.txtGameInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGameInfo.Size = new System.Drawing.Size(437, 94);
            this.txtGameInfo.TabIndex = 9;
            this.txtGameInfo.Text = "[Game Text]";
            // 
            // lblSeperator
            // 
            this.lblSeperator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeperator.Location = new System.Drawing.Point(0, 100);
            this.lblSeperator.Name = "lblSeperator";
            this.lblSeperator.Size = new System.Drawing.Size(555, 2);
            this.lblSeperator.TabIndex = 8;
            // 
            // pctGameLogo
            // 
            this.pctGameLogo.Dock = System.Windows.Forms.DockStyle.Left;
            this.pctGameLogo.Location = new System.Drawing.Point(0, 0);
            this.pctGameLogo.Name = "pctGameLogo";
            this.pctGameLogo.Size = new System.Drawing.Size(100, 100);
            this.pctGameLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pctGameLogo.TabIndex = 1;
            this.pctGameLogo.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnBack);
            this.panel1.Controls.Add(this.btnNext);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 281);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(555, 39);
            this.panel1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(555, 2);
            this.label1.TabIndex = 9;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(457, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(86, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(262, 8);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(86, 28);
            this.btnBack.TabIndex = 6;
            this.btnBack.Text = "< Back ";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(354, 8);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(86, 28);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = "Next >";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // lblSpaceRequired
            // 
            this.lblSpaceRequired.AutoSize = true;
            this.lblSpaceRequired.Location = new System.Drawing.Point(10, 265);
            this.lblSpaceRequired.Name = "lblSpaceRequired";
            this.lblSpaceRequired.Size = new System.Drawing.Size(90, 13);
            this.lblSpaceRequired.TabIndex = 7;
            this.lblSpaceRequired.Text = "[Space Required]";
            // 
            // flowArea
            // 
            this.flowArea.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowArea.Location = new System.Drawing.Point(12, 162);
            this.flowArea.Name = "flowArea";
            this.flowArea.Size = new System.Drawing.Size(531, 100);
            this.flowArea.TabIndex = 8;
            // 
            // lblSetupText
            // 
            this.lblSetupText.AutoSize = true;
            this.lblSetupText.Location = new System.Drawing.Point(13, 113);
            this.lblSetupText.Name = "lblSetupText";
            this.lblSetupText.Size = new System.Drawing.Size(65, 13);
            this.lblSetupText.TabIndex = 9;
            this.lblSetupText.Text = "[Setup Text]";
            // 
            // frmInstallMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 320);
            this.Controls.Add(this.lblSeperator);
            this.Controls.Add(this.lblSetupText);
            this.Controls.Add(this.flowArea);
            this.Controls.Add(this.lblSpaceRequired);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmInstallMain";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Installing - [Game]";
            this.Load += new System.EventHandler(this.frmInstallMain_Load);
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctGameLogo)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }
}