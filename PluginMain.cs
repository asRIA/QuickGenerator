//Abbreviation: create an instance CreateWords that’s the abbreviation itself.
//After insert the text, this function will move the mouse pointer inside the sensible area and between docs.
//CreateWords: it Analyze the text with the arguments and it returns the abbreviation full text.
//Plus it monitor the changes, self-deactivating when something is writing outside the sensible area. 

//ControlShortCutDoubleChar: it activate or deactivate the Autoclose’s functions because it is not able to do it alone.
//AutoClose: it check the insert between the brackets and the creation of the parameters.

//Some notices about the style of the code:

//Nomenclature:
//I’ve used ActionScript for a lot of time and I’m used to utilize its style for the nomenclature.
//For this reason some part of the code may be written in ActionScript style – most probably I forgot it.

//Style of the code:


// Aiming to improve the performance:
//- Usually I not create a property that encapsulate a field if this is enabled both in reading and writing with no checks applied on it or if the code that use it, is not based on the properties.
//- I use the For cycle instead of foreach in some cases.

//  Others:
//I changed the name of some classes or variables according to their real function/behavior and it’s probably that some wrong name are present.
//ES: Autoclose is the new name of the old DoubleChar.

//Some code choose were done base on my knowledge that I had or have about FlashDevelop and it may appear bizarre at the first look. 
//Some parts of the code may appear not necessary, but it was done to keep my aim to execute less instruction to FlashDevelop especially in the monitoring of the writing.

//My scope was to create a plug-in that will not rework any part of the code of FlashDevelop.
//I found myself in certain situations when I could use some functions due their level of visibility or because these functions not used events to hook my code.  
//In this case, I had to copy some part of FlashDevelp inside this plug-in. 
// In CreateWords I show an example.


/// ----------------------------- ITALIAN -----------------------------
/*
 * Abbreviation: Crea una instanza CreateWords che è in altre parole è l'abbreviazione stessa.
 *              Dopo aver inserito il testo si preoccupa dello spostamento dentro le aree sensibili  e fra documenti.
 * CreateWords: Analizza il testo con gli argomenti e ne restituisce il testo completo dell'abbreviazione.
 *              Poi ne monitora i cambiamenti disattivandosi quando viene scritto all'infuori delle aree sensibili. 
 * 
 * 
 * ControlShortCutDoubleChar: si preoccupa di Abilitare o Disabiltare le funzioni di AutoClose perchè questo non può farlo da solo.
 * AutoClose: Controlla l'inserimento delle parentesi e la creazione dei parametri.
 * 
 * Alcune note sullo stile del codice:
 * 
 * Nomeclatura:
 * Avendo usato ActionScript per un pò di tempo e mi ero abituato ad usare il suo stile di nomenclatura.
 * Per questo ci potrebbero  essere delle parti scritte in stile ActionScript che mi sono dimenticato di riscrivere.
 * 
 * Stile del codice:
 *
 *  
 *  Al fine di migliorare le prestazioni:
 *  - Di solito non creo una proprietà che incapsula un campo se questo è abilitato
 *  sia alla lettura che alla scrittura senza nessun tipo di controllo applicato ad esso
 *  o se il codice che ne fà utilizzo non si basa sulle proprietà.
 * - Uso il ciclo for al posto del foreach in alcuni casi.
 * 
 * 
 * Altro:
 * Ho cambiato il nome di alcune classi o varibili  per rispecchiare il loro attuale comportamento ed è probabile che ci siano ancora
 * dei nomi fuorvianti.
 * Per es. AutoClose prima era DoubleChar.
 * 
 * Alcune scelte sono state effettuate in base alle conoscenze che avevo o ho su FlashDevelop quindi possono apparire bizzarre.
 * Alcuni pezzi di codice appariranno non neccessarie, ma ciò sono state dettate dal mio intento di far eseguire meno istruzioni
 * possibili a FlashDevelop sopratutto nel monitoraggio della scrittura.
 * 
 * 
 * Il mio scopo era quello di creare un plugin che ovviamente non doveva toccare nessuna parte del codice di FLashDevelop.
 * Mi sono trovato però in delle situazioni che non potevo riutilizzare alcune funzioni  per via del loro livello di visibilità o perchè queste non 
 * era proviste di eventi a cui potevo agganciarmi.
 * In questi casi, ho dovuto allora drasticamente copiare alcune parti di FlashDevlop stesso in questo plugin.
 * In CreateWords ne mostro un esempio.
 *
 */

using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using PluginCore.Utilities;
using PluginCore.Helpers;
using PluginCore;
using ASCompletion.Context;
using QuickGenerator.UI.form;
using QuickGenerator.CustomCompletionList;
using QuickGenerator.Abbreviation;
using QuickGenerator.UI.MenuItem;
using System.Text;
using QuickGenerator.Command;
using QuickGenerator.QuickSettings;

namespace QuickGenerator
{
	public class PluginMain : IPlugin
	{
		private String pluginName = "QuickGenerator";
		private String pluginGuid = "21D1087C-998D-4466-A25C-8E6EA42F6F32";
		private String pluginHelp = "http://www.youtube.com/user/Alan271078";
		private String pluginDesc = "Add functionality for create code quickly";
		private String pluginAuth = "Alan Lucchese, update: trzeci";
		private int pluginApi = 1;

		internal static CustomCompletionScintillaControl custCompletion;


		private Dictionary<Keys, IPressKey> dictNotifyPressKey;
		private ManagerSettings managerSettings;
		ItemShowCreateClass showCreateClass;
		ToolStripMenuItem menuCompletition;
		ToolStripMenuItem _EnvelopMenu;
		IPressKey key;
		ManagerResources managerResource;
		SettingsLoader settingsLoader;


		private Vocabulary.VocabularyArgument vca;

		public Settings settingsQuickGenerator
		{
			get { return settingsLoader.GetQuickSettings(); }
		}


		#region Required Properties

		/// <summary>
		/// Name of the plugin
		/// </summary> 
		public String Name
		{
			get { return this.pluginName; }
		}

		/// <summary>
		/// GUID of the plugin
		/// </summary>
		public String Guid
		{
			get { return this.pluginGuid; }
		}

		/// <summary>
		/// Author of the plugin
		/// </summary> 
		public String Author
		{
			get { return this.pluginAuth; }
		}

		/// <summary>
		/// Description of the plugin
		/// </summary> 
		public String Description
		{
			get { return this.pluginDesc; }
		}

		/// <summary>
		/// Web address for help
		/// </summary> 
		public String Help
		{
			get { return this.pluginHelp; }
		}

		/// <summary>
		/// Web address for help
		/// </summary> 
		public int Api
		{
			get { return this.pluginApi; }
		}
		/// <summary>
		/// Object that contains the settings
		/// </summary>
		[Browsable(false)]
		public Object Settings
		{
			get { return this.settingsQuickGenerator; }
		}

		#endregion

		#region Required Methods

		/// <summary>
		/// Initializes the plugin
		/// </summary>
		public void Initialize()
		{

			managerResource = new ManagerResources();
			settingsLoader = new SettingsLoader();

			this.InitBasics();
			this.LoadSettings();
			this.CreateMenu();

			dictNotifyPressKey = new Dictionary<Keys, IPressKey>();
			managerSettings = new ManagerSettings(settingsQuickGenerator, dictNotifyPressKey, this, vca);
			managerSettings.ShortcutMenuChangeHandler += new ShortcutMenuChange(managerSettings_ShortcutMenuChangeHandler);

			custCompletion = new CustomCompletionScintillaControl();
			custCompletion.OnSelectItem += new CustomCompletionScintillaControl.ItemSelectedEventHandler(custCompletion_OnSelectItem);

		}

		void custCompletion_OnSelectItem(ICommandInterface cmd)
		{
			cmd.Execute();
		}

		void managerSettings_ShortcutMenuChangeHandler()
		{
			showCreateClass.ShortcutKeys = settingsQuickGenerator.CreateClassShortCut;
			menuCompletition.ShortcutKeys = settingsQuickGenerator.CreateClassFromNameShortCut;
		}


		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
			this.SaveSettings();

			if (custCompletion != null)
			{
				custCompletion.Dispose();
				custCompletion = null;
			}


			managerSettings.Dispose();
			managerResource.Dispose();

			if (_EnvelopMenu != null)
			{
				_EnvelopMenu.Dispose();
				_EnvelopMenu = null;
			}

		}

		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
		{
			switch (e.Type)
			{
				case EventType.Keys:
					KeyEvent k = e as KeyEvent;


					if (dictNotifyPressKey.TryGetValue(k.Value, out key))
					{
						key.EventKey(k);
						e.Handled = true;
					}

					break;

			}
		}

		#endregion


		#region Custom Methods
		/// <summary>
		/// Initializes important variables
		/// </summary>
		public void InitBasics()
		{
			String dataPath = Path.Combine(PathHelper.DataDir, pluginName);
			if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
			settingsLoader.SetPath(Path.Combine(dataPath, "Settings.fdb"));

			//  this.pluginImage = PluginBase.MainForm.FindImage("100");
		}

		/// <summary>
		/// Creates a menu item for the plugin and adds a ignored key
		/// </summary>
		public void CreateMenu()
		{

			ToolStripMenuItem generator = new ToolStripMenuItem("Generator");


			showCreateClass = new ItemShowCreateClass(this);
			// showCreateClass.ShortcutKeys = Keys.Alt | Keys.N;
			showCreateClass.ShortcutKeys = settingsQuickGenerator.CreateClassShortCut;

			menuCompletition = new ToolStripMenuItem("Completition");
			menuCompletition.Visible = false;
			// menuCompletition.ShortcutKeys = Keys.ControlKey | Keys.Alt | Keys.J;
			menuCompletition.ShortcutKeys = settingsQuickGenerator.CreateClassFromNameShortCut;
			menuCompletition.Click += new EventHandler(menuCompletiton);

			generator.DropDownItems.Add(showCreateClass);
			generator.DropDownItems.Add(menuCompletition);

			ASContext.MainForm.MenuStrip.Items.Add(generator);

			PluginBase.MainForm.EditorMenu.Opening += new CancelEventHandler(EditorMenu_Opening);


		}

		void menuCompletiton(object sender, EventArgs e)
		{
			string currentWord = ASContext.CurSciControl.GetWordFromPosition(ASContext.CurSciControl.CurrentPos);

			if (currentWord == null || currentWord.Length == 0)
				return;
			// Project project = (Project)PluginBase.CurrentProject;
			List<ICompletionListItem> lcomp = new List<ICompletionListItem>();

			ICompletionListItem cmd = null;

			if (ASContext.Context.CurrentModel.haXe)
			{
				cmd = new QuickGenerator.Command.CreateClassCmdHaxe(currentWord, this, ManagerResources.ClassImage);
			}
			else
			{
				cmd = new QuickGenerator.Command.CreateClassCmdAS3(currentWord, this, ManagerResources.ClassImage);
			}

			lcomp.Add(cmd);

			custCompletion.Show(lcomp, true);
		}


		ToolStripMenuItem addAbbrevationMenu;

		void EditorMenu_Opening(object sender, CancelEventArgs e)
		{

			bool hasTextToInsert = (ASContext.CurSciControl.SelText.Trim().Length != 0);

			ContextMenuStrip contextMenu = (ContextMenuStrip)sender;

			if (_EnvelopMenu == null)
			{
				_EnvelopMenu = new ToolStripMenuItem("Wrap");

				_EnvelopMenu.DropDownItems.Add("in Function", ManagerResources.MethodImage, new EventHandler(this.EnvelopInFunction));
				_EnvelopMenu.DropDownItems.Add("in if()", ManagerResources.ImportImage, new EventHandler(this.EnvelopInIF));

			}

			if (addAbbrevationMenu == null)
			{
				addAbbrevationMenu = new ToolStripMenuItem("Add to Abbreviation", null, new EventHandler(addAbrevation));
			}



			if (hasTextToInsert)
			{
				if (!contextMenu.Items.Contains(_EnvelopMenu))
					contextMenu.Items.Add(_EnvelopMenu);

				if (!contextMenu.Items.Contains(addAbbrevationMenu))
					contextMenu.Items.Add(addAbbrevationMenu);
			}
			else
			{
				if (contextMenu.Items.Contains(_EnvelopMenu))
					contextMenu.Items.Remove(_EnvelopMenu);

				if (contextMenu.Items.Contains(addAbbrevationMenu))
					contextMenu.Items.Remove(addAbbrevationMenu);
			}

		}
		private void addAbrevation(object sender, EventArgs e)
		{
			QuickSettings.settingAbbrevation sta = new QuickGenerator.QuickSettings.settingAbbrevation();
			sta.AbbrevationDictionary = settingsQuickGenerator.abbrevationDictList;
			sta.CustomList = settingsQuickGenerator.customList;
			sta.ColorArgument = settingsQuickGenerator.ColorArgument;

			AbbrevationCompletionForm frm = new AbbrevationCompletionForm(sta, vca);
			ScintillaNet.ScintillaControl sci = ASContext.CurSciControl;

			string text = ASContext.CurSciControl.SelText;
			int pos = sci.SelectionStart;
			int end = sci.SelectionEnd;




			int initLine = sci.LineFromPosition(pos);
			int endLine = sci.LineFromPosition(end);


			int indent = sci.GetLineIndentation(initLine);

			if (initLine != endLine)
			{
				while (initLine < endLine)
				{
					initLine++;
					int newIdent = sci.GetLineIndentation(initLine);
					if (newIdent < indent)
						indent = newIdent;
				}
			}

			string tabString = new string('\t', indent / sci.Indent);

			StringBuilder sb = new StringBuilder(text.Length);


			if (tabString.Length != 0)
			{
				string[] str = text.Split('\n');


				//sb.Append(str[0]);
				//sb.Append("\n");
				int ind = -1;
				int lenght = tabString.Length;
				for (int i = 0; i < str.Length; i++)
				{
					ind = str[i].IndexOf(tabString, 0);
					if (ind != -1)
					{
						//sb.Append(str[i].Replace(tabString, ""));
						// string sg = str[i].Substring(ind + lenght);
						sb.Append(str[i].Substring(ind + lenght));
					}
					else
					{

						//sb.Append(str[i].TrimStart('\t'));
						sb.Append(str[i]);
					}
					sb.Append("\n");
				}


				sb.Remove(sb.Length - 1, 1);
			}
			else
				sb.Append(text);


			frm.getAbbrevationExternal(sb.ToString());
			frm.ShowDialog();
			sta = frm.getSettingAbbrevation();
			settingsQuickGenerator.abbrevationDictList = sta.AbbrevationDictionary;
			settingsQuickGenerator.customList = sta.CustomList;
			settingsQuickGenerator.ColorArgument = sta.ColorArgument;
		}

		/// <summary>
		/// Invoked when the user selects the "Envelop" command
		/// </summary>
		private void EnvelopInFunction(object sender, EventArgs e)
		{
			Wrap.EnvelopInFunction();
		}

		/// <summary>
		/// Invoked when the user selects the "Envelop" command
		/// </summary>
		private void EnvelopInIF(object sender, EventArgs e)
		{
			Wrap.EnvelopInIF();
		}



		/// <summary>
		/// The form is closed.The new settings now begin save.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void SaveSettingsFrmCreateClass(object sender, FormClosedEventArgs e)
		{
			CreateClassfrm frm = (CreateClassfrm)sender;

			if (frm.OptionChange)
			{
				settingsQuickGenerator.createClassSettings = frm.settings;
				SaveSettings();
			}

		}



		/// <summary>
		/// Loads the plugin settings
		/// </summary>
		public void LoadSettings()
		{
			settingsLoader.LoadSettings();
		}

		/// <summary>
		/// Saves the plugin settings
		/// </summary>
		public void SaveSettings()
		{
			settingsLoader.SaveSettings();

		}

		#endregion


	}

}
