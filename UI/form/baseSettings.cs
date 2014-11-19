using System.Windows.Forms;

namespace QuickGenerator.UI.form
{
    public class baseSettings : Form
    {

        public bool OptionChange
        {
            get { return _optionChange; }
        }

        protected bool _optionChange;

        public baseSettings()
        {
            this.StartPosition = FormStartPosition.CenterScreen;

        }


    }
}
