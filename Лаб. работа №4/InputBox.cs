using System.Windows.Forms;

namespace InputBox
{
    public partial class frmInputBox : Form
    {
        public frmInputBox()
        {
            InitializeComponent();
        }

        public string InputBox(string ACaption, string APrompt, string ADefault)
        {
            Text = ACaption;
            lblPrompt.Text = APrompt;
            tbValue.Text = ADefault;
            if (ShowDialog() == DialogResult.OK)
                return tbValue.Text;
            else
                return ADefault;
        }
    }
}
