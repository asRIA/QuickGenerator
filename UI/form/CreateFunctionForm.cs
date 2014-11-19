using System;
using System.Windows.Forms;

namespace QuickGenerator.UI.form
{
    public partial class CreateFunctionForm : Form
    {
        public CreateFunctionForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (NameFunction.Trim().Length == 0)
            {
                MessageBox.Show("Name function missing!!");
                return;
            }

            this.Close();
            _cancel = false;
            
        }

        public string NameFunction
        {
            get { return txtNameFunction.Text; }
        }


        public bool  functionIsPrivate
        {
            get { return rbPrivate.Checked; }
        }


        public bool writeNameFunction
        {
            get { return chkInsertName.Checked; }
        }

        private bool _cancel;

        public bool cancel
        {
            get { return _cancel; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            
            this.Close();
            
        }

        private void CreateFunctionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cancel = true;
        }
    }
}
