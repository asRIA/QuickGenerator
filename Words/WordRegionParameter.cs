using System;
using QuickGenerator.Abbreviation;

namespace QuickGenerator.Words
{
	class WordRegionParameter : WordRegion, IDisposable
	{

		public TextParameters textParameters;
		public int startToolTip;
		public int endToolTip;
		//  public int indexToolTip;
		public string textParameter;

		public override WordRegionBase getLastWord()
		{
			LastWordRegionParameter lw = new LastWordRegionParameter();
			lw.startWord = this.startWord;
			lw.endWord = this.endWord;
			lw.type = kind.Parameter;
			lw.textParameters = this.textParameters;
			lw.startToolTip = this.startToolTip;
			lw.endToolTip = this.endToolTip;
			//   lw.indexToolTip = this.indexToolTip;
			lw.textParameter = this.textParameter;
			return lw;
		}



		public void ShowTips(ScintillaNet.ScintillaControl sciMonitor)
		{

			if (PluginCore.Controls.CompletionList.Active)
				PluginCore.Controls.CompletionList.Hide();



			PluginCore.Controls.UITools.CallTip.Hide();

			if (textParameters.comments == null)
				PluginCore.Controls.UITools.CallTip.CallTipShow(sciMonitor, textParameters.posParameters, textParameters.text);
			else
				PluginCore.Controls.UITools.CallTip.CallTipShow(sciMonitor, textParameters.posParameters, textParameters.text + textParameter);
			PluginCore.Controls.UITools.CallTip.CallTipSetHlt(startToolTip, endToolTip);


		}




		#region IDisposable Membri di

		void IDisposable.Dispose()
		{
			textParameters = null;
			textParameter = null;
			NextWord = null;
		}

		#endregion
	}
}
