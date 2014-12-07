using PluginCore;
using System.Drawing;

namespace QuickGenerator.UI
{
	public class CompletionListItem : ICompletionListItem
	{
		#region ICompletionListItem Membri di

		private string label;
		private string description;
		protected string value;
		private Bitmap icon;

		public CompletionListItem(string caption, Bitmap icon)
		{
			label = caption;
			this.description = "[No Description]";
			this.value = caption;
			this.icon = icon;

		}


		public CompletionListItem(string caption, string description, Bitmap icon)
		{
			label = caption;
			this.description = description;
			this.value = caption;
			this.icon = icon;
		}


		public CompletionListItem(string caption, string description, string value, Bitmap icon)
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
				else
				{
					int pos = sci.CurrentPos;

					pos--;
					if (pos < 1) pos = 0;

					if (char.IsLetter((char)sci.CharAt(pos)))
					{
						sci.DelWordLeft();
					}


				}


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
