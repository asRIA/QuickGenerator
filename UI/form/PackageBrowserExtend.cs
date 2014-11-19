using System;
using ASClassWizard.Wizards;
//using System.Windows.Forms;

namespace QuickGenerator.UI.form
{
    public partial class PackageBrowserExtend : 
        PackageBrowser
    {

        public bool externalProject;

        public PackageBrowserExtend()
        {
            InitializeComponent();
        }

        private void PackageBrowserExtend_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            externalProject = true;
            this.Close();
        }

    
    }
}
