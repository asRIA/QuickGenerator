//This specific class manage the different shortcut, allowing the user to  create the abbreviation ane move between the sensible area.
//It contain one instance of vocabularyArguments used as a container for the regular expression for analyze the arguments.

//How it works.
//- First of all it check the extension of the actual file which I recall the dictionary of the abbreviations.
//- If no dictionary is present, I use a default one called ‘other’.
//- After that it checks the word on the left and it checks the presence of this word inside the dictionary.
//- If present it create a new instance of <AbbreviationSnippet> that contains the text of the abbreviation and the number and the type of the arguments. The number and the type of arguments are flags already created in AbbreviationCompletionform.cs
//- It checks if in the open document is already present an abbreviation with sensible area and in positive case, it deactivate it, allowing the user to insert a new one.
//- It create an instance of CreateWords and it pass the value of the <AbbreviationSnippet> obtained before.
//- With this CreateWords create the text that will be insert and it generates the referred sensible area.
//- Abbreviation checks if there are some import, event or other snippet to insert in <DoBeforeMonitor()>.
//- After that it checks if there are some sensible area with the <TryActivationMonitor()> of CreateWords.
//- If inside the sensibile area there are some Abbreviation, it will insert a new instance of CreateWords inside dictionaryCreateWords in order to keep track of which Abbreviation are active.

// -------------------- ITALIAN --------------------
/*
 * Questa classe maneggia i vari shortcut per creare l'abbreviazione e per muoversi tra le relative aree sensibili .
 * Contiene all suo interno una istanza di vocabularyArguments che è  un contenitore delle regular expression
 * utilizzate per anallizare gli argomenti.
 * 
 * Come funziona.
 * - Prima controlla l'estensione dell file corrente con la quale ne ricavo il relativo dizionario di abbreviazioni.
 * -  Se nessun dizionario è presente ne uso uno di default chiamato 'other'
 * - Poi controlla la parola sulla sinistra e nè verifica la sua presenza in questo dizionario.
 * - Se presente ne ricava a sua volta una istanza di <AbbrevationSnippet> che a sua volta contiene il testo dell'abbreviazione e il numero e tipo di argomenti.
 *   Il numero e tipo di argomenti sono dei flag  già stati creati in AbbrevationCompletionForm.cs.
 * - Ora controllo se in questo documento vi sia già presente una abbreviazione con aree sensibili e nel caso la disattivo
 *   in modo da poterne inserire quella nuova.
 * -  Creo una istanza di CreateWords e gli dò <AbbrevationSnippet>  ricavato prima.
 *-  Con Questo CreateWords  crea il testo che deve essere inserito e ne genera le relative aree sensibili.
 *-  Abbreviation poi controlla se ci sono degli import,event o altri snippet da inserire in <DoBeforeMonitor()>.
 *  - Dopo prova a controllare se ci sono aree sensibili con <TryActivateMonitor()> di CreateWords.
 * - Se queste aree sensibili ci sono Abbreviation inserisce la nuova istanza di CreateWords in dictionaryCreateWords
 *   per poter tenere traccia di quali Abbreviazioni sono attive.
 * 
 *
 */
using System;
using System.Collections.Generic;
using ScintillaNet;
using PluginCore;
using ASCompletion.Completion;
using QuickGenerator.Words;
using QuickGenerator.Reformatter;
using ASCompletion.Context;
using System.Text.RegularExpressions;
using ASCompletion.Model;
using System.Threading;
using QuickGenerator.Vocabulary;
using QuickGenerator.UI;
using QuickGenerator.Command;

namespace QuickGenerator.Abbreviation
{
	class Abbreviations : IPressKey, IDisposable
	{
		#region IPressKey Membri di
		QuickGenerator.Settings settings;
		CreateWords currentCreateWords;
		private bool MonitorWordsActive;
		private bool listAbrrevationsVisible;
		private bool insertOnly;
		private bool isCursor;
		public ReformatterCode reformatterCode;
		private List<string> imports;
		private ScintillaControl currentSci;
		private Dictionary<string, AbbrevationSnippet> dictAbbreviations;
		private VocabularyArgument vocabularyArguments;
		private Dictionary<IntPtr, CreateWords> dictionaryCreateWords;


		public Abbreviations(QuickGenerator.Settings settings, VocabularyArgument vocabularyArguments)
		{
			this.settings = settings;
			this.vocabularyArguments = vocabularyArguments;
			if (this.vocabularyArguments == null) this.vocabularyArguments = new VocabularyArgument();

			//CreateNewWords();
			dictionaryCreateWords = new Dictionary<IntPtr, CreateWords>(3);


		}



		private CreateWords CreateNewWords()
		{

			currentCreateWords = new CreateWords(settings, vocabularyArguments);
			currentCreateWords.MonitorOnWordsActive += new OnMonitorActiveEventHanlder(cw_MonitorOnWordsActive);
			currentCreateWords.MonitorOnWordsDeactive += new OnMonitorActiveEventHanlder(cw_MonitorOnWordsDeactive);

			return currentCreateWords;
		}



		void cw_MonitorOnWordsDeactive(CreateWords createWords)
		{
			dictAbbreviations = null;
			MonitorWordsActive = false;
			currentSci = null;

			ScintillaControl sci = ASContext.CurSciControl;

			dictionaryCreateWords.Remove(sci.Handle);


			createWords.MonitorOnWordsActive -= new OnMonitorActiveEventHanlder(cw_MonitorOnWordsActive);
			createWords.MonitorOnWordsDeactive -= new OnMonitorActiveEventHanlder(cw_MonitorOnWordsDeactive);
			createWords.GotFocus -= new GotFocusHandler(createWords_GotFocus);
			createWords.Dispose();
			createWords = null;

			currentCreateWords = null;

		}




		void cw_MonitorOnWordsActive(CreateWords createWords)
		{

			MonitorWordsActive = true;
			currentSci = ASContext.CurSciControl;

			CreateWords cw;
			if (dictionaryCreateWords.TryGetValue(currentSci.Handle, out cw))
			{
				cw_MonitorOnWordsDeactive(cw);
				currentSci = ASContext.CurSciControl;

			}

			dictionaryCreateWords.Add(currentSci.Handle, createWords);

			currentCreateWords = createWords;


			createWords.GotFocus += new GotFocusHandler(createWords_GotFocus);

		}

		void createWords_GotFocus(CreateWords sender, ScintillaControl sci)
		{
			currentCreateWords = sender;
			currentSci = sci;
		}

		public void EventKey(PluginCore.KeyEvent k)
		{


			if (k.Value == settings.AbbrevationPlusFormatterShortCut)
			{
				if (reformatterCode == null) { System.Windows.Forms.MessageBox.Show("Reformat Code must be enable!!!"); return; }
				if (settings.abbrevationDictList == null)
				{
					System.Windows.Forms.MessageBox.Show("Insert abbreviations before!!");
					return;
				}



				ScintillaControl sci = ASCompletion.Context.ASContext.CurSciControl;
				if (sci == null) return;

				if (dictionaryCreateWords.TryGetValue(sci.Handle, out currentCreateWords))
				{

					if (currentCreateWords.IsMonitoring)
					{

						ControlCaretPosition();
						return;
					}
				}


				int linePos = sci.LineFromPosition(sci.CurrentPos);
				string lineText = sci.GetLine(linePos);
				string[] str = reformatterCode.DivideStringCode(lineText);

				insertOnly = true;
				// Nothing Divisor ; so do a normal abbrevation
				if (str == null)
				{
					ProcessAbbrevation(sci);
					return;
				}

				bool hasOperator = reformatterCode.SearchForOperators(str[1], true);


				if (!hasOperator)
				{
					ProcessAbbrevation(sci);
					return;
				}


				int startLine = sci.PositionFromLine(linePos);
				int pos = startLine + str[0].TrimEnd().Length;

				int start = 0;
				int end = 0;
				// sci.CurrentPos = pos;

				//string word = sci.GetWordFromPosition(pos);
				string word = GetWordFromPosition(sci, pos, ref start, ref end);
				// string expandeText = "";
				AbbrevationSnippet abbrSnippet = null;

				if (word != null)
				{
					String ext = System.IO.Path.GetExtension(ASContext.Context.CurrentFile).ToLower(); ;
					dictAbbreviations = null;


					if (!settings.abbrevationDictList.TryGetValue(ext, out dictAbbreviations))
					{
						dictAbbreviations = settings.abbrevationDictList[".other"];
					}



					dictAbbreviations.TryGetValue(word, out abbrSnippet);


				}

				if (abbrSnippet != null)
				{
					//string reformatterString = reformatterCode.Reformat(abbrSnippet.Snippet);
					AbbrevationSnippet reformatAbbreviation = new AbbrevationSnippet(reformatterCode.Reformat(abbrSnippet.Snippet));
					reformatAbbreviation.Arguments = abbrSnippet.Arguments;
					reformatAbbreviation.HasAfterCurrentMember = abbrSnippet.HasAfterCurrentMember;
					reformatAbbreviation.HasEventHandler = abbrSnippet.HasEventHandler;
					reformatAbbreviation.HasImport = abbrSnippet.HasImport;

					sci.GotoPos(start);

					sci.BeginUndoAction();

					if (currentCreateWords != null)
						cw_MonitorOnWordsDeactive(currentCreateWords);

					CreateWords cwNew = CreateNewWords();
					string elaborateText = currentCreateWords.MakeTextFromSnippet(sci, reformatAbbreviation);

					sci.SetSel(start, end);

					sci.ReplaceSel(elaborateText);
					sci.DelLineRight();

					if (abbrSnippet.Arguments == null) { sci.EndUndoAction(); return; }
					DoBeforeMonitor();

					if (!currentCreateWords.TryActivateMonitor())
					{
						cwNew.MonitorOnWordsActive -= new OnMonitorActiveEventHanlder(cw_MonitorOnWordsActive);
						cwNew.MonitorOnWordsDeactive -= new OnMonitorActiveEventHanlder(cw_MonitorOnWordsDeactive);
					}
					else
					{
						currentCreateWords = cwNew;
					}

					sci.EndUndoAction();

					return;


				}
				else
				{
					string newString = reformatterCode.Reformat(str[0] + ";");
					sci.GotoPos(startLine);
					sci.BeginUndoAction();
					sci.DelLineRight();
					sci.InsertText(sci.CurrentPos, newString);
					sci.EndUndoAction();
					sci.GotoPos(startLine + newString.Length);

				}

			}
			else if (k.Value == settings.AbbrevationShortCut)
			{

				ScintillaControl sci = ASCompletion.Context.ASContext.CurSciControl;
				if (sci == null) return;
				if (dictionaryCreateWords.TryGetValue(sci.Handle, out currentCreateWords))
				{

					if (currentCreateWords.IsMonitoring)
					{
						ControlCaretPosition();
						return;
					}

				}


				if (sci != null)
					if (sci.Focused)
						ProcessAbbrevation(sci);

			}
			else if (k.Value == settings.GotoAbbreviationShortCut)
			{

				if (dictionaryCreateWords != null && dictionaryCreateWords.Count > 0)
				{

					if (dictionaryCreateWords.Count == 1)
					{
						Dictionary<IntPtr, CreateWords>.ValueCollection.Enumerator enumerator = dictionaryCreateWords.Values.GetEnumerator();
						enumerator.MoveNext();

						CreateWords currentCreateWords = enumerator.Current;
						if (currentCreateWords != null)
						{
							currentCreateWords.GoToCurrentWord();
						}
					}
					else
					{
						List<PluginCore.ICompletionListItem> lcomp = new List<PluginCore.ICompletionListItem>();

						foreach (KeyValuePair<IntPtr, CreateWords> data in dictionaryCreateWords)
						{

							lcomp.Add(new GotoAbbreviationCmd(data.Value.FileName, data.Value.SciMonitor.FileName, ManagerResources.GoToAbbreviationBitmap, data.Value));

						}

						PluginMain.custCompletion.Show(lcomp, false);

					}
				}
			}
			else if (k.Value == settings.GenerateSensibleAreaShortCut)
			{

				if (PathExplorer.IsWorking)
				{
					WaitFinishPathExplorer();
				}

				if (ASContext.Context.CurrentMember != null)
				{

					MemberModel currentMember = ASContext.Context.CurrentMember;
					if (currentMember.Parameters == null || currentMember.Parameters.Count == 0) return;

					ScintillaControl sci = ASContext.CurSciControl;

					string declaration = sci.GetLine(currentMember.LineFrom);

					int startPos = declaration.IndexOf('(');
					int startLine = sci.PositionFromLine(currentMember.LineFrom);

					if (currentMember.Parameters.Count == 1)
					{
						int index = declaration.IndexOf(currentMember.Parameters[0].Name, startPos);
						int pos = startPos + startLine + 1;
						sci.SetSel(pos, pos + currentMember.Parameters[0].Name.Length);
						return;
					}

					int j = -1;

					int[] positionsSensibleArea = new int[currentMember.Parameters.Count * 2];

					int length = positionsSensibleArea.Length;
					for (int i = 0; i < length; i += 2)
					{
						j++;
						int index = declaration.IndexOf(currentMember.Parameters[j].Name, startPos);

						positionsSensibleArea[i] = index;
						positionsSensibleArea[i + 1] = index + currentMember.Parameters[j].Name.Length;



					}


					CreateWords cw;
					if (dictionaryCreateWords.TryGetValue(sci.Handle, out cw))
					{
						cw_MonitorOnWordsDeactive(cw);
						currentSci = ASContext.CurSciControl;

					}

					currentCreateWords = CreateNewWords();
					currentCreateWords.GenerateSensibleArea(startLine, positionsSensibleArea, sci);
				}

			}
		}



		private void DoBeforeMonitor()
		{

			if (currentCreateWords.AfterCurrentMember != null) GenerateAfterCurrentMember();
			if (currentCreateWords.eventsHandler == null && currentCreateWords.imports == null) return;

			/// Generate a temporary WordRegionBase for insert imports.
			/// These word move the other words.
			currentCreateWords.CreateTemporaryVar();

			if (currentCreateWords.eventsHandler != null) GenerateEventHandler();
			if (currentCreateWords.imports != null)
			{
				/// Imports not yet generated
				if (PathExplorer.IsWorking)
				{
					WaitFinishPathExplorer();
				}

				GenerateImports();
			}

			currentCreateWords.RemoveTemporaryVar();

		}


		/// <summary>
		/// FlashDevlop is running PathExplorer.Generate imports o other stuff now lead to a Error.
		/// Abbreviation must wait finish it. 
		/// </summary>
		public static void WaitFinishPathExplorer()
		{
			bool go = PathExplorer.IsWorking;
			int i = 0;
			ScintillaControl sci = ASContext.CurSciControl;

			PluginCore.Controls.UITools.CallTip.CallTipShow(sci, sci.CurrentPos, "Please wait until the the abbreviation is generate!!");

			while (go)
			{

				//Thread.Sleep(100);
				Thread.CurrentThread.Join(100);

				go = PathExplorer.IsWorking;
				i++;
				// Limit of attempts
				if (i > 100)
				{
					go = false; // for security   

				}
			}

			PluginCore.Controls.UITools.CallTip.Hide();
		}

		private void GenerateAfterCurrentMember()
		{
			AbbrevationSnippet abbrevationSnippet;
			int TotalLenghtArray = 0;
			int index = 0;
			QuickGenerator.Abbreviation.WordTypes[] regPlace = null;
			System.Text.StringBuilder sb = null;

			ScintillaControl sci = ASCompletion.Context.ASContext.CurSciControl;
			string nl = PluginCore.Utilities.LineEndDetector.GetNewLineMarker(sci.EOLMode);
			bool hasImport = false;
			bool hasAfterCurrentMember = false;
			bool hasEvent = false;

			foreach (String afterName in currentCreateWords.AfterCurrentMember)
			{
				if (dictAbbreviations.TryGetValue(afterName, out abbrevationSnippet))
				{
					if (abbrevationSnippet.HasAfterCurrentMember)
						hasAfterCurrentMember = true;

					if (abbrevationSnippet.HasEventHandler)
						hasEvent = true;

					if (abbrevationSnippet.HasImport)
						hasImport = true;

					if (abbrevationSnippet.Arguments == null)
						TotalLenghtArray += 0;
					else
						TotalLenghtArray += abbrevationSnippet.Arguments.Length;


					if (regPlace == null)
					{
						if (abbrevationSnippet.Arguments == null)
							regPlace = new WordTypes[0];
						else
							regPlace = new WordTypes[abbrevationSnippet.Arguments.Length];
						sb = new System.Text.StringBuilder(abbrevationSnippet.Snippet.Length);
						sb.Append(abbrevationSnippet.Snippet);
					}
					else
					{
						sb.Append(nl);
						sb.Append(abbrevationSnippet.Snippet);
						index = regPlace.Length;
						Array.Resize(ref regPlace, TotalLenghtArray);
					}

					if (abbrevationSnippet.Arguments != null)
						abbrevationSnippet.Arguments.CopyTo(regPlace, index);

				}
			}

			if (sb == null)
			{
				currentCreateWords.ConvertLastWord();
				return;
			}

			AbbrevationSnippet newAbbreviation = new AbbrevationSnippet(sb.ToString());

			if (regPlace.Length != 0)
				newAbbreviation.Arguments = regPlace;
			else
				newAbbreviation.Arguments = null;


			newAbbreviation.HasImport = hasImport;
			newAbbreviation.HasEventHandler = hasEvent;
			newAbbreviation.HasAfterCurrentMember = hasAfterCurrentMember;


			MemberModel mm = ASContext.Context.CurrentMember;
			ClassModel cm = ASContext.Context.CurrentClass;



			int numBraces = 0;

			if (cm.LineTo == mm.LineTo) numBraces++;

			MemberModel item;

			int length = cm.Members.Count;
			for (int i = 0; i < length; i++)
			{
				item = cm.Members[i];
				if (item.Name != mm.Name)
				{
					if (item.LineTo == mm.LineTo) numBraces++;
				}
			}
			int pos = 0;
			int currentPos = sci.CurrentPos;
			if (numBraces > 0)
			{
				string linestr = sci.GetLine(mm.LineTo);
				int indexBrace = 0;
				for (int i = 0; i < numBraces; i++)
				{
					indexBrace = linestr.LastIndexOf('}');
				}

				pos = sci.PositionFromLine(mm.LineTo);
				pos += indexBrace;


			}
			else
			{
				pos = sci.PositionFromLine(mm.LineTo + 1);
			}


			string tabString = new string('\t', sci.GetLineIndentation(mm.LineFrom) / sci.Indent);


			sci.InsertText(pos, nl + tabString + nl);

			pos = pos + tabString.Length + nl.Length;
			//  sci.GotoPos(pos + tabString.Length +nl.Length  );
			sci.GotoPos(pos);


			// sci.MBSafeGotoPos(pos + tabString.Length + nl.Length);


			// string  tabString2 = new string('\t', sci.GetLineIndentation(sci.LineFromPosition(pos)) / sci.Indent);
			string newSnippet = currentCreateWords.MakeTextFromSnippet(sci, newAbbreviation);


			sci.InsertText(pos, newSnippet);
			//sci.MBSafeInsertText(sci.CurrentPos, newSnippet);


			sci.GotoPos(currentPos);

			currentCreateWords.ConvertLastWord();




		}



		/// <summary>
		/// Try generate relative imports
		/// </summary>
		private void GenerateImports()
		{



			/// First check if there are already imports
			ASCompletion.Model.FileModel fm = ASContext.Context.CurrentModel;

			imports = new List<string>();
			bool foundImport;

			foreach (String imp in currentCreateWords.imports)
			{
				foundImport = false;
				foreach (ASCompletion.Model.MemberModel import in fm.Imports)
				{
					if (imp == import.Name)
					{
						foundImport = true;
						break;
					}
				}

				if (foundImport == false)
					imports.Add("." + imp);
			}

			// no new imports
			if (imports.Count == 0) return;
			//  ASContext.CurSciControl.BeginInvoke(change);

			ASCompletion.Model.MemberList allClasses = ASContext.Context.GetAllProjectClasses();

			if (allClasses != null)
			{


				foreach (ASCompletion.Model.MemberModel member in allClasses)
				{
					int index = 0;

					while (index < imports.Count)
					{
						if (member.Name.EndsWith(imports[index]))
						{
							//  ASGenerator.GenerateJob(GeneratorJobType.AddImport, member, null, null);
							ASGenerator.InsertImport(member, false);

							imports.Remove(imports[index]);
							break;
						}

						index++;
					}

					if (imports.Count == 0) break;
				}

			}

			return;

		}

		private void ControlCaretPosition()
		{

			if (listAbrrevationsVisible)
			{
				RemoveListenerAbbrevationsList();
				currentCreateWords.MoveNextWord();
				return;
			}

			WordRegionBase wr = currentCreateWords.TestInRegion(ASCompletion.Context.ASContext.CurSciControl.CurrentPos);

			if (wr != null)
			{
				if (wr.type == WordRegionBase.kind.cursor)
				{
					isCursor = true;
					ProcessAbbrevation(currentSci);
				}
				else
					currentCreateWords.MoveNextWord();
			}
			else
			{
				ShowListAbbrevations();
				//ProcessAbbrevation(ASCompletion.Context.ASContext.CurSciControl);
			}

		}

		private bool ProcessAbbrevation(ScintillaControl sci)
		{
			//if (_settings.abbrevationDictList == null)
			//{
			//    System.Windows.Forms.MessageBox.Show("Insert abbreviations before!!");
			//    return false;
			//}

			int start = 0;
			int end = 0;

			string left = GetWordFromPosition(sci, ref start, ref end);



			if (left != null)
			{
				String ext = System.IO.Path.GetExtension(ASContext.Context.CurrentFile).ToLower(); ;
				dictAbbreviations = null;

				if (!settings.abbrevationDictList.TryGetValue(ext, out dictAbbreviations))
				{
					dictAbbreviations = settings.abbrevationDictList[".other"];
				}

				AbbrevationSnippet abbrevationSnippet;
				if (dictAbbreviations.TryGetValue(left, out abbrevationSnippet))
				{



					sci.GotoPos(start);
					sci.BeginUndoAction();



					CreateWords cwNew;
					// c'è una abbreviazione lo creo


					if (currentCreateWords != null)
						cw_MonitorOnWordsDeactive(currentCreateWords);

					cwNew = CreateNewWords();

					string elaborateText = cwNew.MakeTextFromSnippet(sci, abbrevationSnippet);


					sci.SetSel(start, end);
					sci.ReplaceSel(elaborateText);

					if (abbrevationSnippet.Arguments == null)
					{
						cwNew.MonitorOnWordsActive -= new OnMonitorActiveEventHanlder(cw_MonitorOnWordsActive);
						cwNew.MonitorOnWordsDeactive -= new OnMonitorActiveEventHanlder(cw_MonitorOnWordsDeactive);
						sci.EndUndoAction();
						return true;
					}


					DoBeforeMonitor();
					// nessuna activazione quindi nessun monitor
					if (!currentCreateWords.TryActivateMonitor())
					{
						cwNew.MonitorOnWordsActive -= new OnMonitorActiveEventHanlder(cw_MonitorOnWordsActive);
						cwNew.MonitorOnWordsDeactive -= new OnMonitorActiveEventHanlder(cw_MonitorOnWordsDeactive);
					}
					else
					{
						currentCreateWords = cwNew;
					}

					sci.EndUndoAction();

					return true;
				}

			}


			//non ha trovato nessuna parola

			if (isCursor)
			{
				isCursor = false;
				ShowListAbbrevations();
				return false;
			}


			if (MonitorWordsActive && currentSci.Focused)
				currentCreateWords.MoveNextWord();
			else
				ShowListAbbrevations();


			return false;
		}

		private void ShowListAbbrevations()
		{

			String ext = System.IO.Path.GetExtension(ASContext.Context.CurrentFile).ToLower(); ;
			Dictionary<string, AbbrevationSnippet> Abbrevations = null;


			if (!settings.abbrevationDictList.TryGetValue(ext, out Abbrevations))
			{
				Abbrevations = settings.abbrevationDictList[".other"];
			}


			List<PluginCore.ICompletionListItem> lcomp = new List<PluginCore.ICompletionListItem>();

			if (Abbrevations.Count == 0)
			{
				PluginCore.Controls.UITools.CallTip.CallTipShow(ASContext.CurSciControl, ASContext.CurSciControl.CurrentPos, "No Abbreviations for this file extension!!");
				return;
			}
			else
			{
				foreach (KeyValuePair<string, AbbrevationSnippet> item in Abbrevations)
				{
					CompletionListItem comp = new CompletionListItem(item.Key, item.Value.Snippet, ManagerResources.AbbreviationBitmap);
					// QuickGenerator.UI.QuickGeneratorCompletionListItem comp = new QuickGenerator.UI.QuickGeneratorCompletionListItem(item.Key, "");
					lcomp.Add(comp);
				}
			}
			if (listAbrrevationsVisible)
			{
				RemoveListenerAbbrevationsList();
			}
			ScintillaControl sci = ASContext.CurSciControl;
			string word = sci.GetWordFromPosition(sci.CurrentPos);

			if (word == null) word = "";
			PluginCore.Controls.CompletionList.Show(lcomp, false, word);

			PluginCore.Controls.CompletionList.OnInsert += new PluginCore.Controls.InsertedTextHandler(CompletionList_OnInsert);
			PluginCore.Controls.CompletionList.OnCancel += new PluginCore.Controls.InsertedTextHandler(CompletionList_OnCancel);

			listAbrrevationsVisible = true;

		}

		void CompletionList_OnCancel(ScintillaControl sender, int position, string text, char trigger, ICompletionListItem item)
		{
			RemoveListenerAbbrevationsList();
		}


		private void GenerateEventHandler()
		{
			if (ASContext.Context.CurrentMember == null) return;

			ASCompletion.Model.ClassModel cm = ASContext.Context.CurrentClass;
			ScintillaControl sci = ASContext.CurSciControl;
			string contextToken = "";
			string linetext = "";
			const string patternEvent = "Listener\\s*\\(\\s*(?<event>[a-z_0-9.\\\"']+)\\s*,\\s*{0}";
			List<string> contextTokens = new List<string>();
			List<string> linestext = new List<string>();
			foreach (int pos in currentCreateWords.eventsHandler)
			{

				sci.GotoPos(pos);
				//System.Console.WriteLine(ASContext.Context.CurrentMember);
				contextToken = sci.GetWordFromPosition(pos);
				bool found = false;

				foreach (ASCompletion.Model.MemberModel mi in cm.Members)
				{
					if (mi.Name == contextToken)
					{
						found = true;
						break;
					}
				}

				if (found)
					continue;

				contextTokens.Add(contextToken);
				linetext = sci.GetLine(sci.LineFromPosition(pos));
				linestext.Add(linetext);

			}

			if (contextTokens.Count == 0) return;
			for (int i = 0; i < contextTokens.Count; i++)
			{
				Match m = Regex.Match(linestext[i], String.Format(patternEvent, contextTokens[i]), RegexOptions.IgnoreCase);

				string name = "Event";

				if (m.Groups.Count > 0)
				{
					int index = m.Groups[1].Value.IndexOf(".");

					if (index == -1)
					{
						name = m.Groups[1].Value;
					}
					else
					{
						name = m.Groups[1].Value.Substring(0, index);
					}
				}

				if (name.Length == 0) continue;
				ASGenerator.SetJobContext(contextTokens[i], name, null, m);
				ASGenerator.GenerateJob(GeneratorJobType.ComplexEvent, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, "Generate Event handler", null);
			}


		}

		private void RemoveListenerAbbrevationsList()
		{

			PluginCore.Controls.CompletionList.OnInsert -= new PluginCore.Controls.InsertedTextHandler(CompletionList_OnInsert);
			PluginCore.Controls.CompletionList.OnCancel -= new PluginCore.Controls.InsertedTextHandler(CompletionList_OnCancel);

			listAbrrevationsVisible = false;
		}

		void CompletionList_OnInsert(ScintillaControl sender, int position, string text, char trigger, ICompletionListItem item)
		{

			RemoveListenerAbbrevationsList();


			if (!(item is QuickGenerator.UI.CompletionListItem))
			{
				insertOnly = false;
				return;
			}


			if (insertOnly)
			{
				insertOnly = false;
				return;
			}
			//sender.WordLeftExtend();
			//sender.ReplaceSel(text);


			ProcessAbbrevation(sender);
		}

		/// <summary>
		/// Gets a word from the specified position
		/// </summary>
		public static string GetWordFromPosition(ScintillaControl sci, ref int start, ref int end)
		{
			int position = sci.CurrentPos;
			try
			{
				start = sci.WordStartPosition(position, true);
				end = sci.WordEndPosition(position, true);
				int startPosition = sci.MBSafeCharPosition(start);
				int endPosition = sci.MBSafeCharPosition(end);
				string keyword = sci.Text.Substring(startPosition, endPosition - startPosition);

				if (keyword.Length == 0 || keyword.Equals(" ")) return null;


				//startPosition = sci.WordStartPosition(position, true);
				//endPosition = sci.WordEndPosition(position, true);
				return keyword.Trim(); ;
			}
			catch
			{
				return null;
			}
		}


		/// <summary>
		/// Gets a word from the specified position
		/// </summary>
		public static string GetWordFromPosition(ScintillaControl sci, int position, ref int start, ref int end)
		{

			try
			{
				//startPosition = sci.MBSafeCharPosition(sci.WordStartPosition(position, true));
				//endPosition = sci.MBSafeCharPosition(sci.WordEndPosition(position, true));
				//string keyword = sci.Text.Substring(startPosition, endPosition - startPosition);

				start = sci.WordStartPosition(position, true);
				end = sci.WordEndPosition(position, true);
				int startPosition = sci.MBSafeCharPosition(start);
				int endPosition = sci.MBSafeCharPosition(end);

				string keyword = sci.Text.Substring(startPosition, endPosition - startPosition);

				if (keyword.Length == 0 || keyword.Equals(" ")) return null;


				//startPosition = sci.WordStartPosition(position, true);
				//endPosition = sci.WordEndPosition(position, true);
				return keyword.Trim();
			}
			catch
			{
				return null;
			}
		}


		public void CreateParameters(List<MemberModel> list, bool isEventListener, TextParameters textParameters)
		{


			ScintillaControl sci = ASContext.CurSciControl;

			if (!dictionaryCreateWords.TryGetValue(sci.Handle, out currentCreateWords))
			{

				currentCreateWords = CreateNewWords();
				currentSci = sci;

			}



			currentCreateWords.CreateParameters(list, textParameters);

			if (isEventListener)
			{
				// ScintillaControl sci = ASContext.CurSciControl;
				sci.ReplaceSel("");
				PluginCore.Controls.UITools.CallTip.Hide();
				ASComplete.HandleFunctionCompletion(sci, true);
				currentCreateWords.ActivateList();

			}

		}

		#endregion

		#region IDisposable Membri di

		public void Dispose()
		{
			settings = null;

			if (currentCreateWords != null)
				currentCreateWords.Dispose();
			if (listAbrrevationsVisible)
			{
				RemoveListenerAbbrevationsList();
			}

		}

		#endregion


	}


}
