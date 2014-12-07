using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PluginCore.Utilities;
using QuickGenerator.Abbreviation;

namespace QuickGenerator.QuickSettings
{
	class SettingsLoader
	{
		private string settingFilename;
		private Settings settingsQuickGenerator;

		public void LoadSettings()
		{
			this.settingsQuickGenerator = new Settings();

			if (!File.Exists(this.settingFilename))
			{
				this.SaveSettings();
			}
			else
			{
				Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingsQuickGenerator);
				this.settingsQuickGenerator = (Settings)obj;
			}

			if (settingsQuickGenerator.abbrevationDictList == null)
			{
				settingsQuickGenerator.abbrevationDictList = new Dictionary<string, Dictionary<string, AbbrevationSnippet>>();
				settingsQuickGenerator.abbrevationDictList.Add(".as", new Dictionary<string, AbbrevationSnippet>());
				settingsQuickGenerator.abbrevationDictList.Add(".other", new Dictionary<string, AbbrevationSnippet>());
				AbbrevationSnippet abr = new AbbrevationSnippet("for(var ${var=\"i\" list=\"ls3\"}:int=0; ${var=\"i\"} < ${\"length\"}; ${var=\"i\"}++)\r\n{\r\n\t${SafeZone}\r\n}");
				abr.Arguments = new WordTypes[5];
				abr.Arguments[0] = WordTypes.var;
				abr.Arguments[1] = WordTypes.var;
				abr.Arguments[2] = WordTypes.place;
				abr.Arguments[3] = WordTypes.var;
				abr.Arguments[4] = WordTypes.SafeZone;

				settingsQuickGenerator.abbrevationDictList[".as"].Add("fori", abr);

				abr = new AbbrevationSnippet("${list=\"ls1\"} function ${var=\"name\"}(${\"\"}):void\r\n{\r\n\t${SafeZone}\r\n}");
				abr.Arguments = new WordTypes[4];
				abr.Arguments[0] = WordTypes.list;
				abr.Arguments[1] = WordTypes.var;
				abr.Arguments[2] = WordTypes.place;
				abr.Arguments[3] = WordTypes.SafeZone;

				settingsQuickGenerator.abbrevationDictList[".as"].Add("fnc", abr);

				abr = new AbbrevationSnippet("var ${var=\"request\"}:${Import=\"URLRequest\"} = new URLRequest(${browser});\r\nvar ${var=\"loader\"}:Loader = new ${Import=\"Loader\"}();\r\n${var=\"loader\"}.contentLoaderInfo.addEventListener(${Import=\"Event\"}.COMPLETE, ${EventHandler=\"completeHandler\"});\r\n${var=\"loader\"}.load(${var=\"request\"});");
				abr.Arguments = new WordTypes[10];
				abr.Arguments[0] = WordTypes.var;
				abr.Arguments[1] = WordTypes.import;
				abr.Arguments[2] = WordTypes.browser;
				abr.Arguments[3] = WordTypes.var;
				abr.Arguments[4] = WordTypes.import;
				abr.Arguments[5] = WordTypes.var;
				abr.Arguments[6] = WordTypes.import;
				abr.Arguments[7] = WordTypes.EventHandler;
				abr.Arguments[8] = WordTypes.var;
				abr.Arguments[9] = WordTypes.var;
				abr.HasImport = true;
				abr.HasEventHandler = true;
				settingsQuickGenerator.abbrevationDictList[".as"].Add("load", abr);
				//dictAbbrevations[".as"].Add("vr", new AbbrevationSnippet("${list=0}var ${cursor}:${showCompType};"));



				abr = new AbbrevationSnippet("${list=\"ls2\"} ${\"a\"}:${cmp=\"Number\"} = ${\"0\"};");
				abr.Arguments = new WordTypes[4];
				abr.Arguments[0] = WordTypes.list;
				abr.Arguments[1] = WordTypes.place;
				abr.Arguments[2] = WordTypes.cmp;
				abr.Arguments[3] = WordTypes.place;

				settingsQuickGenerator.abbrevationDictList[".as"].Add("vr", abr);

				abr = new AbbrevationSnippet("var ${\"mc\"}:${var=\"MovieClip\" showCmp} = new ${var=\"MovieClip\"}(${createParameters});");
				abr.Arguments = new WordTypes[4];
				abr.Arguments[0] = WordTypes.place;
				abr.Arguments[1] = WordTypes.var;
				abr.Arguments[2] = WordTypes.var;
				abr.Arguments[3] = WordTypes.createParameters;

				settingsQuickGenerator.abbrevationDictList[".as"].Add("nw", abr);


				abr = new AbbrevationSnippet("${var=\"name\"}(${createParameters});${AfterCurrentMember=\"fnc\"}");
				abr.Arguments = new WordTypes[3];
				abr.Arguments[0] = WordTypes.var;
				abr.Arguments[1] = WordTypes.createParameters;
				abr.Arguments[2] = WordTypes.AfterCurrentMember;
				abr.HasAfterCurrentMember = true;
				settingsQuickGenerator.abbrevationDictList[".as"].Add("out", abr);

				if (settingsQuickGenerator.Abbreviations.CustomList == null)
				{
					settingsQuickGenerator.Abbreviations.CustomList = new Dictionary<string, List<string>>();
					List<string> ls = new List<string>();
					ls.Add("public");
					ls.Add("private");
					ls.Add("protected");
					settingsQuickGenerator.Abbreviations.CustomList.Add("ls1", ls);
					ls = new List<string>();
					ls.Add("var");
					ls.Add("public var");
					ls.Add("private var");
					settingsQuickGenerator.Abbreviations.CustomList.Add("ls2", ls);

					ls = new List<string>();
					ls.Add("x");
					ls.Add("y");
					ls.Add("z");
					settingsQuickGenerator.Abbreviations.CustomList.Add("ls3", ls);
				}


			}
		}


		public void SaveSettings()
		{
			System.Reflection.EventInfo[] evtInfo = settingsQuickGenerator.GetType().GetEvents();
			Dictionary<string, object> evt = new Dictionary<string, object>();
			for (int i = 0; i < evtInfo.GetLength(0); i++)
			{
				string fldName = evtInfo[i].Name;
				System.Reflection.FieldInfo fldinfo = settingsQuickGenerator.GetType().GetField(
					fldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

				evt.Add(fldName,
				fldinfo.GetValue(settingsQuickGenerator));

				fldinfo.SetValue(settingsQuickGenerator, null);


			}

			ObjectSerializer.Serialize(this.settingFilename, this.settingsQuickGenerator);
			for (int i = 0; i < evtInfo.GetLength(0); i++)
			{
				string fldName = evtInfo[i].Name;
				System.Reflection.FieldInfo fldinfo = settingsQuickGenerator.GetType().GetField(
				   fldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				fldinfo.SetValue(settingsQuickGenerator, evt[fldName]);
			}
		}


		public void SetPath(string p)
		{
			settingFilename = p;
		}

		public Settings GetQuickSettings()
		{
			return settingsQuickGenerator;
		}
	}
}
