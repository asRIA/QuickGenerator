using System;
using System.Windows.Forms;

namespace QuickGenerator.UI.form
{
    public partial class NameExtensionForm : baseSettings
    {
        public NameExtensionForm()
        {
            InitializeComponent();
        }

        private void frmExtension_Load(object sender, EventArgs e)
        {
          // this.StartPosition = FormStartPosition.CenterScreen;
        }

        public string ext()
        {
            return textBoxOnlyWord1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
