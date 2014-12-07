//This class deactivate or reactive the AutoClose and it manage its referred options.
//If the AutoClose is turned on and there are more than 1 document open, it adds a listener ad each of those documents, otherwise it remove the listeners.


//----   ITALIAN -------------
/**
 * Questa classe disattiva o riattiva AutoClose  e nè amministra le sue relative opzioni.
 * Se si attiva autoclose e ci sono più documenti aperti aggiungo un listener ad ognuno di questi documenti,
 * in caso contrario rimuove questi listener.
 */
using System;
using QuickGenerator.Abbreviation;

namespace QuickGenerator.Double
{
	class ControlShortCutAutoClose : IPressKey, IDisposable
	{
		QuickGenerator.Settings settings;
		AutoClose autoClose;
		private bool enabled;
		private Abbreviations abbreviations;

		public ControlShortCutAutoClose(QuickGenerator.Settings settings, Abbreviations abbreviations)
		{
			this.settings = settings;
			this.abbreviations = abbreviations;
		}


		#region IPressKey Membri di

		public void EventKey(PluginCore.KeyEvent k)
		{


			if (enabled)
			{

				Disable();
				if (settings.ShowStateChange)
				{
					System.Windows.Forms.MessageBox.Show("AutoClose Disable");
				}
			}
			else
			{
				EnabledAutoCloseAndListenOpenDocuments();
				if (settings.ShowStateChange)
				{
					System.Windows.Forms.MessageBox.Show("AutoClose Enable");
				}
			}

		}

		#endregion

		public void Enable()
		{
			if (autoClose != null) return;
			autoClose = new AutoClose();
			if (settings.CloseFunctionAndNew)
				autoClose.EnableFunctionAndNewClose();

			enabled = true;

			if (settings.CreateParameters)
				autoClose.abbreviations = this.abbreviations;

		}

		public void Disable()
		{
			if (autoClose == null) return;
			autoClose.Dispose();
			autoClose = null;
			enabled = false;

		}

		public void EnableCreateParameters(Abbreviation.Abbreviations abr)
		{
			if (autoClose == null) return;
			if (settings.CreateParameters)
			{
				autoClose.abbreviations = abr;
			}

		}

		public void DisableCreateParameters()
		{
			if (autoClose == null) return;
			autoClose.abbreviations = null;
		}

		public void EnableCloseFunction()
		{
			if (autoClose == null) return;
			autoClose.EnableFunctionAndNewClose();
		}

		public void DisableCloseFunction()
		{
			if (autoClose == null) return;
			autoClose.disableFunctionAndNewClose();
		}
		// Enable DoubleChar and add Listener to Open documents
		public void EnabledAutoCloseAndListenOpenDocuments()
		{
			if (autoClose != null) return;
			autoClose = new AutoClose();

			autoClose.InsertAllDocument();

			if (settings.CloseFunctionAndNew)
				autoClose.EnableFunctionAndNewClose();

			if (settings.CreateParameters)
				autoClose.abbreviations = abbreviations;


			enabled = true;
		}


		#region IDisposable Membri di

		public void Dispose()
		{
			if (autoClose != null)
			{
				Disable();
			}

			settings = null;
			enabled = false;
			abbreviations = null;
		}

		#endregion
	}
}
