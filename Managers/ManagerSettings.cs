using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickGenerator.clipboardring;
using PluginCore.Managers;
using PluginCore;
using QuickGenerator.Double;
using QuickGenerator.Abbreviation;
using QuickGenerator.Vocabulary;
using QuickGenerator.Reformatter;

namespace QuickGenerator
{

	public delegate void ShortcutMenuChange();

	public class ManagerSettings : IDisposable
	{
		private Settings settings;
		private Dictionary<Keys, IPressKey> dictNotifyPressKey;
		private frmMonitor monitorClipBoard;
		private bool isActive;
		private IPlugin plugin;
		private ControlShortCutAutoClose ctrDoubleChar;
		private Abbreviations abbrevationCompletition;
		private VocabularyArgument vca;
		private ReformatterCode reformatterCode;

		public event ShortcutMenuChange ShortcutMenuChangeHandler;

		public ManagerSettings(Settings setting, Dictionary<Keys, IPressKey> dictNotifyPressKey, IPlugin plugin, VocabularyArgument vca)
		{
			this.settings = setting;
			this.dictNotifyPressKey = dictNotifyPressKey;
			this.plugin = plugin;
			this.vca = vca;
			Init();
		}

		private void Init()
		{



			if (settings.EnableClipBoardRing)
				EnableMonitor();


			if (settings.AbbrevationEnabled)
				EnableAbbreviation();

			// this is Auto Close  option enabled
			if (settings.DoubleCharEnabled)
			{

				ctrDoubleChar = new ControlShortCutAutoClose(settings, abbrevationCompletition);
				ctrDoubleChar.Enable();


				if (settings.SwitchEnableOrDisableDoubleCharShortCut != Keys.None)
				{
					dictNotifyPressKey.Add(settings.SwitchEnableOrDisableDoubleCharShortCut, ctrDoubleChar);
					ActivateEventKeys();
				}
			}


			if (settings.FormatterCodeEnabled)
				EnableReformatterCode();


			if (settings.FormatterCodeEnabled && settings.AbbrevationEnabled)
				abbrevationCompletition.reformatterCode = reformatterCode;


			settings.ChangeClipBoardRingSettings += new ChangeClipBoardRingSettingsEventHanlder(settings_ChangeClipBoardRingSettings);
			settings.ChangeAbbrevationSettings += new ChangeAbbrevationSettingsEventHanlder(settings_ChangeAbbrevationSettings);
			settings.ChangeDoubleCharSettings += new ChangeDoubleCharSettingsEventHanlder(settings_ChangeDoubleCharSettings);
			settings.ChangeFormatterCodeSettings += new ChangeFormatterCodeSettingsEventHanlder(settings_ChangeFormatterCodeSettings);
			settings.ChangeShortCut += new ChangeShortCutEventHanlder(settings_ChangeShortCut);
			settings.ChangeFunctionAutoClose += new ChangeAutoCloseFunctionEventHanlder(settings_ChangeFunctionAutoClose);
		}

		void settings_ChangeFunctionAutoClose()
		{
			if (ctrDoubleChar == null) return;
			if (settings.CloseFunctionAndNew)
			{
				ctrDoubleChar.EnableCloseFunction();
			}
			else
				ctrDoubleChar.DisableCloseFunction();

			if (settings.CreateParameters)
			{
				ctrDoubleChar.EnableCreateParameters(abbrevationCompletition);
			}
			else
				ctrDoubleChar.DisableCreateParameters();


		}

		bool settings_ChangeShortCut(settingsComponent nameComponent, Keys oldKeys, Keys newKeys, bool disable)
		{
			switch (nameComponent)
			{
				case settingsComponent.CreateClass:

					if (dictNotifyPressKey.ContainsKey(newKeys))
					{
						MessageBox.Show("Shorcut already in use in QuickGenerator!!\nPlease choose another Shorcut!!");
						return true;
					}


					ShortcutMenuChangeHandler();

					return false;
				case settingsComponent.FormatterCode:
					if (reformatterCode == null) return false;
					if (dictNotifyPressKey.ContainsKey(oldKeys))
					{
						if (dictNotifyPressKey[oldKeys] is ReformatterCode)
							dictNotifyPressKey.Remove(oldKeys);
					}

					if (newKeys != Keys.None)
						if (settings.FormatterCodeEnabled)
						{
							if (!dictNotifyPressKey.ContainsKey(newKeys))
							{
								dictNotifyPressKey.Add(newKeys, reformatterCode);
							}
							else
							{
								MessageBox.Show("Shorcut already in use in QuickGenerator!!\nPlease choose another Shorcut!!");
								return true;
							}
						}


					break;
				case settingsComponent.ClipBoardRing:
					if (monitorClipBoard == null) return false;

					if (dictNotifyPressKey.ContainsKey(oldKeys))
					{
						if (dictNotifyPressKey[oldKeys] is frmMonitor)
							dictNotifyPressKey.Remove(oldKeys);
					}

					if (newKeys != Keys.None)
						if (settings.EnableClipBoardRing)
						{
							if (!dictNotifyPressKey.ContainsKey(newKeys))
							{

								dictNotifyPressKey.Add(newKeys, monitorClipBoard);
							}
							else
							{
								MessageBox.Show("Shorcut already in use in QuickGenerator!!\nPlease choose another Shorcut!!");
								return true;
							}
						}



					break;
				case settingsComponent.Abbrevation:
					if (abbrevationCompletition == null) return false;
					if (dictNotifyPressKey.ContainsKey(oldKeys))
					{
						if (dictNotifyPressKey[oldKeys] is Abbreviations)
							dictNotifyPressKey.Remove(oldKeys);
					}

					if (newKeys != Keys.None)
						if (settings.AbbrevationEnabled)
						{
							if (!dictNotifyPressKey.ContainsKey(newKeys))
							{
								dictNotifyPressKey.Add(newKeys, abbrevationCompletition);
							}
							else
							{
								MessageBox.Show("Shorcut already in use in QuickGenerator!!\nPlease choose another Shorcut!!");
								return true;
							}
						}

					break;
				case settingsComponent.DoubleChar:
					if (dictNotifyPressKey.ContainsKey(oldKeys))
					{
						if (dictNotifyPressKey[oldKeys] is ControlShortCutAutoClose)
							dictNotifyPressKey.Remove(oldKeys);
					}

					if (newKeys != Keys.None)
					{
						if (settings.DoubleCharEnabled)
						{
							if (!dictNotifyPressKey.ContainsKey(newKeys))
							{

								if (ctrDoubleChar == null)
									ctrDoubleChar = new ControlShortCutAutoClose(settings, abbrevationCompletition);

								dictNotifyPressKey.Add(newKeys, ctrDoubleChar);
							}
							else
							{
								MessageBox.Show("Shorcut already in use in QuickGenerator!!\nPlease choose another Shorcut!!");
								return true;
							}

						}
					}
					else
					{
						if (!settings.DoubleCharEnabled)
						{
							ctrDoubleChar.Dispose();
							ctrDoubleChar = null;
						}
					}

					break;
			}

			ActivateEventKeys();


			DisactivateEventKey();

			return false;
		}

		void settings_ChangeFormatterCodeSettings()
		{
			if (settings.FormatterCodeEnabled)
			{
				EnableReformatterCode();
			}
			else
			{
				if (reformatterCode != null)
				{
					DisableReformatterCode();


				}
			}
		}

		void settings_ChangeDoubleCharSettings()
		{
			if (!settings.DoubleCharEnabled)
			{
				if (ctrDoubleChar == null) return;

				ctrDoubleChar.Disable();

				if (settings.SwitchEnableOrDisableDoubleCharShortCut == Keys.None)
				{
					ctrDoubleChar.Dispose();
					ctrDoubleChar = null;
				}

			}
			else
			{
				// activate Double Char
				if (ctrDoubleChar == null)
				{
					ctrDoubleChar = new ControlShortCutAutoClose(settings, abbrevationCompletition);
				}


				ctrDoubleChar.EnabledAutoCloseAndListenOpenDocuments();
				ctrDoubleChar.EnableCreateParameters(abbrevationCompletition);

				if (settings.SwitchEnableOrDisableDoubleCharShortCut != Keys.None)
				{

					if (dictNotifyPressKey.ContainsKey(settings.SwitchEnableOrDisableDoubleCharShortCut))
					{
						settings.switchEnableOrDisableDoubleCharShortCut = Keys.None;
						MessageBox.Show("Shorcut already in use in QuickGenerator!!\nPlease choose another Shorcut!!");
					}
					else
						dictNotifyPressKey.Add(settings.SwitchEnableOrDisableDoubleCharShortCut, ctrDoubleChar);

					ActivateEventKeys();
				}
			}
		}

		void settings_ChangeAbbrevationSettings()
		{
			if (abbrevationCompletition == null)
			{
				EnableAbbreviation();
			}
			else
				DisableAbbrevation();
		}

		void settings_ChangeClipBoardRingSettings()
		{
			if (settings.EnableClipBoardRing)
			{
				if (monitorClipBoard == null)
				{
					EnableMonitor();
				}

				monitorClipBoard.SetCapacity(settings.CapacityClipBoardRing);
			}
			else
			{
				if (monitorClipBoard != null)
					DisableMonitor();
			}
		}



		#region Reformat Code State
		private void EnableReformatterCode()
		{
			reformatterCode = new ReformatterCode(settings);

			if (abbrevationCompletition != null)
				abbrevationCompletition.reformatterCode = reformatterCode;

			bool alreadyPresent = false;
			if (settings.FormatterCodeShortCut != Keys.None)
			{
				if (dictNotifyPressKey.ContainsKey(settings.FormatterCodeShortCut))
				{
					settings.formatterCodeShortCut = Keys.None;
					alreadyPresent = true;
				}
				else
					dictNotifyPressKey.Add(settings.FormatterCodeShortCut, reformatterCode);

				if (alreadyPresent)
					MessageBox.Show("Shorcut already in use in QuickGenerator!!\nPlease choose another Shorcut!!");

				ActivateEventKeys();
			}



			// insertShortCutHandle(reformatterCode);

		}


		private void DisableReformatterCode()
		{
			//removeShortCutHandle(reformatterCode);
			reformatterCode.Dispose();

			reformatterCode = null;

			if (abbrevationCompletition != null)
				abbrevationCompletition.reformatterCode = null;

			if (settings.FormatterCodeShortCut != Keys.None)
			{
				if (dictNotifyPressKey.ContainsKey(settings.FormatterCodeShortCut))
					dictNotifyPressKey.Remove(settings.FormatterCodeShortCut);

				DisactivateEventKey();
			}



		}
		#endregion

		#region Abbreviation State
		/// <summary>
		/// Enable Abbrevation 
		/// </summary>
		private void EnableAbbreviation()
		{
			abbrevationCompletition = new Abbreviations(settings, vca);


			if (ctrDoubleChar != null)
				if (settings.CreateParameters)
				{
					ctrDoubleChar.EnableCreateParameters(abbrevationCompletition);
				}


			// Active only if there is at least a short cut is active
			if (settings.AbbrevationShortCut != Keys.None || settings.AbbrevationPlusFormatterShortCut != Keys.None)
			{
				bool alreadyPresent = false;

				if (settings.AbbrevationShortCut != Keys.None)
				{
					if (dictNotifyPressKey.ContainsKey(settings.AbbrevationShortCut))
					{
						alreadyPresent = true;
						settings.abbrevationShortCut = Keys.None;

					}
					else
						dictNotifyPressKey.Add(settings.AbbrevationShortCut, abbrevationCompletition);
				}


				if (settings.AbbrevationPlusFormatterShortCut != Keys.None)
				{
					if (dictNotifyPressKey.ContainsKey(settings.AbbrevationPlusFormatterShortCut))
					{
						alreadyPresent = true;
						settings.abbrevationPlusFormatterShortCut = Keys.None;

					}
					else
						dictNotifyPressKey.Add(settings.AbbrevationPlusFormatterShortCut, abbrevationCompletition);
				}

				if (settings.GotoAbbreviationShortCut != Keys.None)
				{
					if (dictNotifyPressKey.ContainsKey(settings.GotoAbbreviationShortCut))
					{
						alreadyPresent = true;
						settings.gotoAbbreviationShortCut = Keys.None;

					}
					else
						dictNotifyPressKey.Add(settings.GotoAbbreviationShortCut, abbrevationCompletition);
				}


				if (settings.GenerateSensibleAreaShortCut != Keys.None)
				{
					if (dictNotifyPressKey.ContainsKey(settings.GenerateSensibleAreaShortCut))
					{
						alreadyPresent = true;
						settings.generateSensibleAreaShortCut = Keys.None;

					}
					else
						dictNotifyPressKey.Add(settings.GenerateSensibleAreaShortCut, abbrevationCompletition);
				}




				//insertShortCutHandle(abbrevationCompletition);


				if (alreadyPresent)
					MessageBox.Show("Shorcut already in use in QuickGenerator!!\nPlease choose another Shorcut!!");

				ActivateEventKeys();
			}
		}


		private void DisableAbbrevation()
		{
			abbrevationCompletition.Dispose();

			if (settings.AbbrevationShortCut != Keys.None)
			{
				dictNotifyPressKey.Remove(settings.AbbrevationShortCut);
			}

			if (settings.AbbrevationPlusFormatterShortCut != Keys.None)
			{
				dictNotifyPressKey.Remove(settings.AbbrevationPlusFormatterShortCut);
			}

			if (settings.GenerateSensibleAreaShortCut != Keys.None)
			{
				dictNotifyPressKey.Remove(settings.GenerateSensibleAreaShortCut);
			}

			if (settings.GotoAbbreviationShortCut != Keys.None)
			{
				dictNotifyPressKey.Remove(settings.GotoAbbreviationShortCut);
			}

			DisactivateEventKey();
			//removeShortCutHandle(abbrevationCompletition);

			abbrevationCompletition = null;

			if (ctrDoubleChar == null) return;
			ctrDoubleChar.DisableCreateParameters();

		}

		#endregion

		#region ClipBoardRing State
		private void EnableMonitor()
		{
			monitorClipBoard = new frmMonitor(settings);
			monitorClipBoard.ActiveMonitorClipBoard();
			bool alreadyPresent = false;
			if (settings.FowardShortCut != Keys.None || settings.PreviousShortCut != Keys.None || settings.ShowClipBoardRingShortCut != Keys.None)
			{
				// insertShortCutHandle(monitorClipBoard);
				if (settings.FowardShortCut != Keys.None)
				{
					if (dictNotifyPressKey.ContainsKey(settings.FowardShortCut))
					{
						settings.fowardShortCut = Keys.None;
						alreadyPresent = true;
					}
					else
						dictNotifyPressKey.Add(settings.FowardShortCut, monitorClipBoard);
				}

				if (settings.PreviousShortCut != Keys.None)
				{
					if (dictNotifyPressKey.ContainsKey(settings.PreviousShortCut))
					{
						settings.previousShortCut = Keys.None;
						alreadyPresent = true;
					}
					else
						dictNotifyPressKey.Add(settings.PreviousShortCut, monitorClipBoard);
				}

				if (settings.ShowClipBoardRingShortCut != Keys.None)
				{
					if (dictNotifyPressKey.ContainsKey(settings.ShowClipBoardRingShortCut))
					{
						settings.showClipBoardRingShortCut = Keys.None;
						alreadyPresent = true;
					}
					else
						dictNotifyPressKey.Add(settings.ShowClipBoardRingShortCut, monitorClipBoard);
				}


				if (alreadyPresent)
					MessageBox.Show("Shorcut already in use in QuickGenerator!!\nPlease choose another Shorcut!!");
				ActivateEventKeys();
			}
		}

		private void DisableMonitor()
		{
			if (monitorClipBoard != null)
			{
				// removeShortCutHandle(monitorClipBoard);


				if (settings.FowardShortCut != Keys.None)
				{
					dictNotifyPressKey.Remove(settings.FowardShortCut);
				}

				if (settings.PreviousShortCut != Keys.None)
				{
					dictNotifyPressKey.Remove(settings.PreviousShortCut);
				}

				if (settings.ShowClipBoardRingShortCut != Keys.None)
				{
					dictNotifyPressKey.Remove(settings.ShowClipBoardRingShortCut);
				}

				DisactivateEventKey();

				monitorClipBoard.Dispose();
				monitorClipBoard = null;
			}
		}
		#endregion

		#region Event Keys
		private void ActivateEventKeys()
		{
			if (dictNotifyPressKey.Count >= 1 && !isActive)
			{
				isActive = true;
				EventManager.AddEventHandler(plugin, EventType.Keys);
			}
		}

		private void DisactivateEventKey()
		{
			if (dictNotifyPressKey.Count == 0)
			{
				isActive = false;
				EventManager.RemoveEventHandler(plugin);
			}
		}
		#endregion


		#region IDisposable Membri di

		public void Dispose()
		{
			DisableMonitor();

			if (reformatterCode != null)
			{
				reformatterCode.Dispose();
				reformatterCode = null;
			}

			if (abbrevationCompletition != null)
			{
				abbrevationCompletition.Dispose();
				abbrevationCompletition = null;
			}



			if (ctrDoubleChar != null)
				ctrDoubleChar.Dispose();

		}

		#endregion


	}
}
