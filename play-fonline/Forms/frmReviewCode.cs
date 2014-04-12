namespace PlayFOnline.Forms
{
    using System;
    using System.Windows.Forms;

    public partial class frmReviewCode : Form
    {
        public bool Cancelled { get; set; }

        public frmReviewCode(string Code)
        {
            InitializeComponent();
            txtScriptCode.Text = Code;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Cancelled = false;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Cancelled = true;
            this.Close();
        }
    }
}
