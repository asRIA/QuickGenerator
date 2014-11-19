using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace QuickGenerator.UI.form
{

    public delegate void validateName();
   

    class TextBoxOnlyWord :TextBox 
    {
        public const string REG_IDENTIFIER = "^[a-zA-Z_$][a-zA-Z0-9_$]*$";

        public event validateName ValidateName;


        private bool _hasError;
        public bool hasError { get { return _hasError;}  }

        public TextBoxOnlyWord()
        {
            this.KeyPress += new KeyPressEventHandler(TextBoxOnlyWord_KeyPress);
            this.TextChanged += new EventHandler(TextBoxOnlyWord_TextChanged);
        }

        void TextBoxOnlyWord_TextChanged(object sender, EventArgs e)
        {
             if (ValidateName != null)
            {
                ValidateClass();
             }
        }

        private void ValidateClass()
        {

            if (Regex.Match(this.Text, REG_IDENTIFIER, RegexOptions.Singleline).Success == false)
            {
                _hasError = true;
            }
            else
            {
                _hasError = false;
            }

           
                ValidateName();
    
        }


        void TextBoxOnlyWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.Text.Length==0)
            {
                if (!(e.KeyChar >= 'a' && e.KeyChar <= 'z') && !(e.KeyChar >= 'A' && e.KeyChar <= 'Z') && !(e.KeyChar == '_'))
                {
                    e.Handled = true;
                }
            }

            if (!(e.KeyChar >= 'a' && e.KeyChar <= 'z') && !(e.KeyChar >= 'A' && e.KeyChar <= 'Z') && !(e.KeyChar == '_') && !(e.KeyChar == 8) && !(e.KeyChar >= '0' && e.KeyChar <= '9'))
            {
                e.Handled = true;
            }
        }
    }
}
