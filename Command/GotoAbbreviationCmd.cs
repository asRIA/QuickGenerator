using PluginCore;
using System.Drawing;
using QuickGenerator.Words;

namespace QuickGenerator.Command
{
    class GotoAbbreviationCmd : ICompletionListItem, ICommandInterface 
    {
        #region ICommandInterface Membri di

        private Bitmap icon;
        private string value;
        private CreateWords cw;
        private string description;
        public GotoAbbreviationCmd(string text,string description, Bitmap icon, CreateWords createWords)
        {
            this.icon = icon;
            this.value = text;
            this.cw = createWords;
            this.description = description;
        }


        public void Execute()
        {
            cw.GoToCurrentWord();
        }

        #endregion

        #region ICompletionListItem Membri di

        public string Label
        {
            get { return value; }
        }

        public string Value
        {
            get { return value; }
        }

        public string Description
        {
            get { return description; }
        }

        public System.Drawing.Bitmap Icon
        {
            get { return icon; }
        }

        #endregion
    }
}
