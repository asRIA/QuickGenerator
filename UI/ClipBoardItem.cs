using PluginCore;
using System.Drawing;

namespace QuickGenerator.UI
{
    class ClipBoardItem : ICompletionListItem
    {
        #region ICompletionListItem Membri di

        private string label;
        private string description;
        protected string value;
        private Bitmap icon;

        public ClipBoardItem(string caption, string description, string value, Bitmap icon)
        {
            label = caption;
            this.description = description;
            this.value = value;
            this.icon = icon;
        }

        public string Label
        {
            get { return label; }
        }

        public virtual string Value
        {
            get
            {
                ScintillaNet.ScintillaControl sci = ASCompletion.Context.ASContext.CurSciControl;

                if (!string.IsNullOrEmpty(sci.SelText))
                    sci.ReplaceSel("");


                return value;
            }
        }

        public string Description
        {
            get { return description; }
        }




        public System.Drawing.Bitmap Icon
        {
            get
            {
                return icon;

            }
        }

        #endregion
    }
}
