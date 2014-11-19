//This class use an instance of AbbreviationSnippet.cs and it will obtain from it the text to insert and its sensible area.
//AbbreviationSnippet.cs contains the number, the position and the type of the argument to analyze.
//This will not allow to test all the regular expression for each argument present.

//During the argument’s analysis if CreateWords will find some area where the user will choose the value, a class derived from the WordRegionBase will be created.
//WordRegionBase contain the position of this word and it will contain a referring at the next word taken from its relative WordRegionBase.
//For more details look at WordRegionBase.cs
//These classes will be insert in a list called <Words> that will be used as index for the moving when the shortcut <TestInRegion> is used.

//After create these words, CreateWords will check every insert or delay of the text, crosschecking if this changes are done inside the area <InsertRegion> and <TestDeleteInRegion>.
//If the changes are done outside from these 2 area, the abbreviation will be deactivated.

//Due the fact that the external changes force the deactivation of the abbreviation and FlashDevelop generate the auto-import, I’ve to keep track of this words creating a temporary <WordRegion> called declarationWord.

//This <WordRegion> will be auto-eliminated after its usage.
//Even for the creation of the external variables I use the declarationWord, but these variables will be eliminated only if there are some changes outside them.
//Everytime that the user press ‘;’, the text will be reordered, changing the position of the words. This will force the recalculation of the WordRegionBase linked to them.
 
//For this specific event I didn’t find a way to hook up to an existing function/code, I’ve rewrote it in <sciMonitor_CharAdded>.
//Each time that I create an abbreviation I’ve to deactivate the pre-existing option inside FlashDevelop and run my custom version.
//I reactive the pre-existing version at each change of documents.


//------------------ ITALIAN --------------------
/*
 * Questa classe utilizza una istanza di AbbreviationSnippet.cs dalla quale
 * nè ricaverà il testo da inserire e le sue relative  aree sensibili.
 * AbbreviationSnippet.cs contiene il numero, la posizione e il tipo di argomento da analizzare.
 * Questo impedisce di testare tutte le regulare expression per ogni argomento presente.
 * 
 * Durante l'analisi degli argomenti se CreateWords troverà delle aree dove poi sarà l'utente a sceglierne il valore viene creata un classe derivata da WordRegionBase.
 * WordRegionBase contiene la posizione di questa parola situata dentro e conterrà inoltre  un riferimento alla parola succesiva rapresentata dalla sua relativa WordRegionBase.
 * Per dettagli guardare WordRegionBase.cs
 * Queste classi vengono inserite  in una lista chiamata <Words> che verrà usata come indice per lo spostamento
 * quando si userà il relativo shortcut <TestInRegion>.
 * 
 * Dopo aver creato queste parole CreateWords controllerà ogni inserimento o cancellazione del testo,
 * verificando se queste modifiche siano effettuate nelle aree interessate <InsertInRegion> <TestDeleteInRegion>.
 * Se sono effettuate fuori  da queste aree l'abbreviazione viene disattivata.
 * 
 * Dato che le modifiche esterne portano alla disattivazione dell'abbreviazione e FlashDevelop genera gli import automaticamente,
 * devo tenere traccia di queste parole creando una <WordRegion> temporanea chiamata declarationWord.
 * Questa <WordRegion> viene automaticamente eliminata dopo il suo utilizzo.
 * Anche per la creazione di variabili esterne uso declarationWord, ma queste vengono eliminate solo se 
 * solo se ci sono delle modifiche non effettuate all'interno di essa.
 * Ogni volta che si preme ';' il testo viene riordinato cambiando le posizioni delle parole,
 * questo comporta il ricalcolare le WordRegionBase associate ad esse.
 * Dato che per questo evento non ho trovato proprio un modo di aggangiarmi ho dovuto riscriverlo in <sciMonitor_CharAdded>.
 * Ogni volta che creo una abbreviazione devo disattivare questa opzione di flashdevelop ed eseguire la mia versione.
 * Ripristino questa opzione ogni cambio di documento.
 * 
 * */
using System;
using System.Collections.Generic;
using System.Text;
using ScintillaNet;
using System.Text.RegularExpressions;
using ASCompletion.Context;
using QuickGenerator.Abbreviation;
using ASCompletion.Model;
using QuickGenerator.Vocabulary;
using QuickGenerator.UI;
using PluginCore.Controls;

namespace QuickGenerator.Words
{

    public delegate void OnMonitorActiveEventHanlder(CreateWords createWords);
    public delegate void GotFocusHandler(CreateWords sender, ScintillaControl sci);

    public class CreateWords :  IDisposable
    {
        /// <summary>
        /// Monitor the changes of the words 
        /// </summary>
       public event OnMonitorActiveEventHanlder MonitorOnWordsActive;
       public event OnMonitorActiveEventHanlder MonitorOnWordsDeactive;
       public event GotFocusHandler GotFocus;

       delegate void delegateChangeWord();
       delegateChangeWord changeWord;
   
        
        private bool isMonitoring;
       // private bool isOpenDialog;
      

        private int semicolon = (int)';';

        int previousLenPlace;
        int curIndWord; // Word current
        int numCursors;
        int indexArgument;
       
        private ScintillaControl sciMonitor;
        
        
        private VarWordRegion vwr;
        private WordRegionBase curWord;
        private WordRegionBase secondLastWord;
        private WordRegionBase lastWord;
        private WordRegionBase wrTested;
        private WordRegionBase firstWord;

        /// <summary>
        ///  Temporary region for imports or other stuff
        /// </summary>
        private WordRegion declarationWord;
        private WordRegion importWord;

        public List<WordRegionBase> Words;
      // System.Windows.Forms.Timer tmChangeWordText;
        System.Windows.Forms.Timer highlightWord;
      

       private Dictionary<string, VarWordRegion> lsVar;
       private Dictionary<string,List<string>> _dictCustomList;
       
       public List<string> imports;
       public List<int> eventsHandler;
       public List<string> AfterCurrentMember;
        // Boolean

        private int isList;
        private bool retest;
        private bool reformater;
        private bool alreadyInvoke;
        private bool dontCreateLastWord;
        private string fileName;

        private VocabularyArgument _vocabularyArgument;
        private QuickGenerator.Settings _setting;
        FlashDevelop.Docking.TabbedDocument currentForm;

        public CreateWords(QuickGenerator.Settings settings, VocabularyArgument vocabularyArgument)
        {
           this._setting = settings;

           _vocabularyArgument = vocabularyArgument;

            changeWord = new delegateChangeWord(ChangeWordsVar);
            
         
            Words = new List<WordRegionBase>();
            lsVar = new Dictionary<string, VarWordRegion>();
            

            curIndWord = -1;

        }


        public void GenerateSensibleArea(int startPos,int[] positions, ScintillaControl sci)
        {
            if (isMonitoring)
                DeactiveMonitorWords();



            //if (sci.CodePage == 65001 && startPos!=0)
            //{

            //    int strLen = Encoding.UTF8.GetByteCount(
            //        sci.Text.Substring(0,startPos)
            //        );
            //    startPos = strLen;
            //}

             //startPos = sci.MBSafePosition(startPos);
             WordRegionBase last=null;
           WordRegionBase wr=null;
            for (int i = 0; i < positions.Length; i+=2)
            {

                

                if (i == positions.Length-2)
                {
                    wr = new LastWordRegion();
                }
                else
                {
                    wr = new WordRegion();
                }


                wr.type = WordRegionBase.kind.place;


                wr.startWord = startPos + positions[i];
                wr.endWord = startPos + positions[i + 1];

                if (last != null)
                {
                    last.NextWord = wr;
                }

                last = wr;
                Words.Add(wr);
            }

            firstWord = Words[0];

            lastWord = wr;
            fileName = ASContext.Context.CurrentClass.Name;

            if (MonitorOnWordsActive != null)
                MonitorOnWordsActive(this);

           

            ActiveMonitorWords();

            MoveNextWord();


        }
  

        public string MakeTextFromSnippet(ScintillaControl sci, AbbrevationSnippet abbrSnippet)
        {

         
            StringBuilder sbSnippet = new StringBuilder(abbrSnippet.Snippet);

            if (isMonitoring)
                DeactiveMonitorWords();

            if(abbrSnippet.HasImport)
            imports = new List<string>();
            if (abbrSnippet.HasEventHandler)
            eventsHandler = new List<int>();
            if (abbrSnippet.HasAfterCurrentMember) 
            AfterCurrentMember = new List<string>();



            _dictCustomList = _setting.customList;
            
            int pos = sci.CurrentPos;
            int CodePage = sci.CodePage;


            string nl = ASCompletion.Completion.ASComplete.GetNewLineMarker(sci.EOLMode);        
            int curLine = sci.LineFromPosition(pos);
            int startLine = sci.PositionFromLine(curLine);


            char ch = (char) sci.CharAt(startLine);
            string tabString = String.Empty;
            while ( Char.IsWhiteSpace( ch))
            {
                if(ch=='\n' || ch=='\r') break;
                tabString += ch;
             
                ch = (char)sci.CharAt(++startLine);
            }



             sbSnippet.Replace("\r","");
             sbSnippet.Replace("\n", nl + tabString);



             if (abbrSnippet.Arguments == null) return sbSnippet.ToString();

                previousLenPlace = 0;
                indexArgument = 0;
                numCursors = 0;



             MatchCollection mtc = _vocabularyArgument.regArguments.Matches(sbSnippet.ToString());
             int lenght = mtc.Count;
             Match m =null;
             Match var;
        
             string textClipBoard = "";
             int valueTextClipBoard = 0;


             char[] chars = null;
             int previousPos=0;
             for (int i = 0; i < lenght; i++)
             {
                  m = mtc[i];

                  /// if CodePage is 65001... pos= is position in scintilla text
                  /// and previousLen is position on abbrSnippet
                  if (indexArgument == abbrSnippet.Arguments.Length)
                  {
                      indexArgument = 0;

       
                          lsVar.Clear();
                   

                  }


                
                  switch (abbrSnippet.Arguments[indexArgument])
                  {
                      #region Place
                      case WordTypes.place:
                          var = _vocabularyArgument.regPlace.Match(m.Value);
                          if (var.Success)
                          {
                              WordRegion wr = new WordRegion();
 
                              int correctPosString = m.Index - previousLenPlace;

                              int count = correctPosString - previousPos;
                              chars = new char[count];

                              sbSnippet.CopyTo(previousPos, chars, 0, count);
                              previousLenPlace += m.Length - var.Length;
                              sbSnippet.Remove(correctPosString, m.Length);
                              sbSnippet.Insert(correctPosString, var.Value);


                              previousPos = correctPosString + var.Length;
               
                              
                              if (CodePage != 65001)
                              {
                                  wr.startWord = pos + chars.Length;
                                  wr.endWord = wr.startWord + var.Length;
                                  pos += chars.Length + var.Length;
                              }
                              else
                              {

                                
                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );
                                  int valueLen = Encoding.UTF8.GetByteCount(var.Value);
                                  wr.startWord = pos + strLen;
                                  wr.endWord = wr.startWord + valueLen;
                                  pos += strLen + valueLen;
                              }

                              wr.type = WordRegion.kind.place;

                              if (curWord != null)
                              {
                                  curWord.NextWord = wr;
                                  secondLastWord = curWord;
                              }

                              Words.Add(wr);

                              curWord = wr;
                              curIndWord++;
                              numCursors++;
                              indexArgument++;

                            //  sb.Append(var.Value);
                          }
                          break;
                      #endregion
                      #region cursor
                      case WordTypes.SafeZone:

                          if (m.Value == VocabularyArgument.SafeZone)
                          {
                              WordRegion wr = new WordRegion();
                              wr.type = WordRegionBase.kind.cursor;

                              //string previousTest = textSnippet.Substring(previousLenPlace, m.Index - previousLenPlace);
                              //sb.Append(previousTest);

                              int correctPosString = m.Index - previousLenPlace;

                              int count = correctPosString - previousPos;
                              chars = new char[count];

                              sbSnippet.CopyTo(previousPos, chars, 0, count);
                              previousLenPlace += m.Length;
                              sbSnippet.Remove(correctPosString, m.Length);
                              

                              previousPos = correctPosString;

                             

                              if (CodePage != 65001)
                              {
                                  wr.startWord = pos + chars.Length;
                                  wr.endWord = wr.startWord;
                                  pos += chars.Length;
                              }
                              else
                              {


                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );
                                 // int valueLen = Encoding.UTF8.GetByteCount(var.Value);
                                  wr.startWord = pos + strLen;
                                  wr.endWord = wr.startWord ;
                                  pos += strLen ;
                              }


                              if (curWord != null)
                              {
                                  curWord.NextWord = wr;
                                  secondLastWord = curWord;
                              }

                              Words.Add(wr);
                              curWord = wr;
                              curIndWord++;
                              numCursors++;
                              indexArgument++;
                              
                          }


                          break;
                      #endregion
                      #region createParameters
                      case WordTypes.createParameters:

                          if (m.Value == VocabularyArgument.CREATEPARAMATERS)
                          {
                              WordRegion wr = new WordRegion();
                              wr.type = WordRegionBase.kind.createParameters;



                              int correctPosString = m.Index - previousLenPlace;

                              int count = correctPosString - previousPos;
                              chars = new char[count];

                              sbSnippet.CopyTo(previousPos, chars, 0, count);
                              previousLenPlace += m.Length;
                              sbSnippet.Remove(correctPosString, m.Length);

                              previousPos = correctPosString;

                              if (CodePage != 65001)
                              {
                                  wr.startWord = pos + chars.Length;
                                  wr.endWord = wr.startWord;
                                  pos += chars.Length;
                              }
                              else
                              {


                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );
                                  wr.startWord = pos + strLen;
                                  wr.endWord = wr.startWord;
                                  pos += strLen;
                              }



                              if (curWord != null)
                              {
                                  curWord.NextWord = wr;
                                  secondLastWord = curWord;
                              }

                              Words.Add(wr);
                              curWord = wr;
                              curIndWord++;
                              numCursors++;
                              indexArgument++;
                             
                              
                          }


                          break;
                      #endregion
                      #region clipboard
                      case WordTypes.Clipboard:
                          if (m.Value == VocabularyArgument.CLIPBOARD)
                          {

                              if (textClipBoard.Length==0)
                              {
                                  textClipBoard = System.Windows.Forms.Clipboard.GetText().Trim();
                                  valueTextClipBoard = Encoding.UTF8.GetByteCount(textClipBoard);
                              }




                              int correctPosString = m.Index - previousLenPlace;

                              previousLenPlace += m.Length - textClipBoard.Length;
                              sbSnippet.Remove(correctPosString, m.Length);
                              sbSnippet.Insert(correctPosString, textClipBoard);

                              if (CodePage != 65001)
                              {
                                  pos += chars.Length + textClipBoard.Length;
                              }
                              else
                              {

                                  int count = correctPosString - previousPos;
                                  chars = new char[count];
                                  sbSnippet.CopyTo(previousPos, chars, 0, count);
                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );

                                  pos += strLen + valueTextClipBoard;
                              }

                              previousPos = correctPosString + textClipBoard.Length;



                              indexArgument++;
                             
                          }


                          break;
                      #endregion
                      #region showcomp
                      case WordTypes.cmp:


                          var = _vocabularyArgument.regCompType.Match(m.Value);

                          //if (var.Value.Length!=0)
                          if(var.Success)
                          {
                              WordRegion wr = new WordRegion();
                              wr.type = WordRegion.kind.showCompType;


                              int correctPosString = m.Index - previousLenPlace;

                              int count = correctPosString - previousPos;
                              chars = new char[count];

                              sbSnippet.CopyTo(previousPos, chars, 0, count);
                              previousLenPlace += m.Length - var.Length;
                              sbSnippet.Remove(correctPosString, m.Length);
                              sbSnippet.Insert(correctPosString, var.Value);


                              previousPos = correctPosString + var.Length;


                              if (CodePage != 65001)
                              {
                                  wr.startWord = pos + chars.Length;
                                  wr.endWord = wr.startWord + var.Length;
                                  pos += chars.Length + var.Length;
                              }
                              else
                              {


                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );
                                  int valueLen = Encoding.UTF8.GetByteCount(var.Value);
                                  wr.startWord = pos + strLen;
                                  wr.endWord = wr.startWord + valueLen;
                                  pos += strLen + valueLen;
                              }

                              if (curWord != null)
                              {
                                  curWord.NextWord = wr;
                                  secondLastWord = curWord;
                              }

                              Words.Add(wr);
                              curWord = wr;

                              curIndWord++;
                              numCursors++;
                              indexArgument++;
                            
                          }


                          break;
                      #endregion
                      #region variable link
                      case WordTypes.var:


                          var = _vocabularyArgument.regVar.Match(m.Value);

                          if (var.Value.Length!=0)
                          {
                              VarWordRegion varRootWR = null;
                              VarDuplicateWordRegion vdwr = null;
                              VarWordRegion vwr = null;
                              WordRegionBase wr;
                             
                              // there is a root?
                              if (lsVar.TryGetValue(var.Value, out varRootWR))
                              {
                                  vdwr = new VarDuplicateWordRegion();
                                  wr = vdwr;
                                  vdwr.type = WordRegionBase.kind.VarLink;
                              }
                              else
                              {

                                  vwr = new VarWordRegion();
                                  vwr.ValueVar = var.Value;
                                  curIndWord++;


                                  string option = m.Value.Substring(var.Index + var.Length);
                                  Match mc = _vocabularyArgument.regList.Match(option);

                                  if (mc.Value.Length!=0)
                                  {
                                      vwr.indList = mc.Value;
                                      vwr.type = WordRegionBase.kind.customList;
                                  }
                                  else if (option.IndexOf("showCmp") != -1)
                                      vwr.type = WordRegion.kind.VarLinkAndComp;
                                  else
                                      vwr.type = WordRegionBase.kind.VarLink;

                                  wr = vwr;

                                  vwr.ChangeVarWord += new VarWordRegion.ChangeVarEventHandler(vwr_ChangeVarWord);
                              }

                              //string previousTest = textSnippet.Substring(previousLenPlace, m.Index - previousLenPlace);
                              //sb.Append(previousTest);

                              // questo serve per cancellare index
                              int correctPosString = m.Index - previousLenPlace;

                              int count = correctPosString - previousPos;
                              chars = new char[count];

                              sbSnippet.CopyTo(previousPos, chars, 0, count);
                              previousLenPlace += m.Length - var.Length;
                              sbSnippet.Remove(correctPosString, m.Length);
                              sbSnippet.Insert(correctPosString, var.Value);


                              previousPos = correctPosString +  var.Length;


                              if (CodePage != 65001)
                              {
                                  wr.startWord = pos + chars.Length;
                                  wr.endWord = wr.startWord + var.Length;
                                  pos += chars.Length + var.Length;
                              }
                              else
                              {


                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );
                                  int valueLen = Encoding.UTF8.GetByteCount(var.Value);
                                  wr.startWord = pos + strLen;
                                  wr.endWord = wr.startWord + valueLen;
                                  pos += strLen + valueLen;
                              }


                              if (curWord != null)
                              {
                                  curWord.NextWord = wr;
                                  secondLastWord = curWord;
                              }

                              curWord = wr;


                              if (varRootWR != null)
                              {
                                  varRootWR.InsertVarDuplicate(vdwr);
                                  //varRootWR.lstWords.Add(vdwr);
                              }
                              else
                              {
                                  lsVar.Add(var.Value, vwr);
                                  Words.Add(wr);
                              }
                              indexArgument++;

                              numCursors++;

                           //   sb.Append(var.Value);
                          }


                          break;
                      #endregion
                      #region mbrname
                      case WordTypes.mbrname:
                          if (m.Value == VocabularyArgument.MbrName)
                          {

                              string strMbrName = "";

                              if (ASContext.Context.CurrentMember != null)
                              {
                                  strMbrName = ASContext.Context.CurrentMember.Name;
                              }




                              int correctPosString = m.Index - previousLenPlace;

                              previousLenPlace += m.Length - strMbrName.Length;
                              sbSnippet.Remove(correctPosString, m.Length);
                              sbSnippet.Insert(correctPosString, strMbrName);

                              if (CodePage != 65001)
                              {
                                  pos += chars.Length + strMbrName.Length;
                              }
                              else
                              {

                                  int count = correctPosString - previousPos;
                                  chars = new char[count];
                                  sbSnippet.CopyTo(previousPos, chars, 0, count);
                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );

                                  pos += strLen + strMbrName.Length;
                              }

                              previousPos = correctPosString + strMbrName.Length;

                              indexArgument++;
                          }
                          break;
                      #endregion
                      #region "Browser"
                      case WordTypes.browser:
                          // --------------  browser
                          if (m.Value == VocabularyArgument.BROWSER)
                          {
                              WordRegion wr = new WordRegion();
                              wr.type = WordRegion.kind.browser;

                              int correctPosString = m.Index - previousLenPlace;

                              int count = correctPosString - previousPos;
                              chars = new char[count];

                              sbSnippet.CopyTo(previousPos, chars, 0, count);
                              previousLenPlace += m.Length;
                              sbSnippet.Remove(correctPosString, m.Length);
    

                              previousPos = correctPosString;


                              if (CodePage != 65001)
                              {
                                  wr.startWord = pos + chars.Length;
                                  wr.endWord = wr.startWord;
                                  pos += chars.Length;
                              }
                              else
                              {


                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );
                                  wr.startWord = pos + strLen;
                                  wr.endWord = wr.startWord;
                                  pos += strLen ;
                              }



                              //  wr.lineNumber = lineNumber;

                              if (curWord != null)
                              {
                                  curWord.NextWord = wr;
                                  secondLastWord = curWord;
                              }

                              Words.Add(wr);
                              curWord = wr;
                              curIndWord++;
                              //  wr.index = curIndWord;
                              numCursors++;
                              indexArgument++;
                             
                          }
                          break;
                      #endregion
                      #region AfterCurrentMember
                      case WordTypes.AfterCurrentMember:

                          var = _vocabularyArgument.regAfterCurrentMember.Match(m.Value);
                          if (var.Value.Length!=0)
                          {

                              if (ASContext.Context.CurrentMember != null)
                              {
                                  if (!AfterCurrentMember.Contains(var.Value))
                                  {
                                      AfterCurrentMember.Add(var.Value);
                                      dontCreateLastWord = true;
                                  }
                                  //strMbrName = ASContext.Context.CurrentMember.Name;
                              }

                              int correctPosString = m.Index - previousLenPlace;

                              int count = correctPosString - previousPos;
                              chars = new char[count];

                              sbSnippet.CopyTo(previousPos, chars, 0, count);
                              previousLenPlace += m.Length;
                              sbSnippet.Remove(correctPosString, m.Length);

                              previousPos = correctPosString;

                              if (CodePage != 65001)
                              {

                                  pos += chars.Length;
                              }
                              else
                              {
                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );

                                  pos += strLen;
                              }


                              indexArgument++;
                             
                          }
                          break;
                      #endregion
                      #region Custom list
                      case WordTypes.list:
                          //-------- CUSTOM LIST COMPLETITION

                          var = _vocabularyArgument.regList.Match(m.Value);

                          if (var.Value.Length!=0)
                          {
                              WordCustomList wr = new WordCustomList();
                              wr.type = WordRegion.kind.customList;
                              string value = "";
                              List<string> ls;
                              if (_dictCustomList.TryGetValue(var.Value, out ls))
                              {

                                  if(ls.Count>0)
                                  value = ls[0];
                              }


                              int correctPosString = m.Index - previousLenPlace;

                              int count = correctPosString - previousPos;
                              chars = new char[count];

                              sbSnippet.CopyTo(previousPos, chars, 0, count);
                              previousLenPlace += m.Length - value.Length;
                              sbSnippet.Remove(correctPosString, m.Length);
                              sbSnippet.Insert(correctPosString, value);


                              previousPos = correctPosString + value.Length;


                              if (CodePage != 65001)
                              {
                                  wr.startWord = pos + chars.Length;
                                  wr.endWord = wr.startWord + value.Length;
                                  pos += chars.Length + value.Length;
                              }
                              else
                              {


                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );
                                  int valueLen = Encoding.UTF8.GetByteCount(value);
                                  wr.startWord = pos + strLen;
                                  wr.endWord = wr.startWord + valueLen;
                                  pos += strLen + valueLen;
                              }

                              //  wr.lineNumber = lineNumber;
                              wr.indList = var.Value;
                              if (curWord != null)
                              {
                                  curWord.NextWord = wr;
                                  secondLastWord = curWord;
                              }

                              Words.Add(wr);
                              curWord = wr;
                              curIndWord++;
                              //   wr.index = curIndWord;
                              numCursors++;
                              indexArgument++;
                             
                          }

                          break;
                      #endregion
                      #region import
                      case WordTypes.import:
                          var = _vocabularyArgument.regImport.Match(m.Value);

                          

                          if (var.Value.Length!=0)
                          {

                            

                              int correctPosString = m.Index - previousLenPlace;
                             
                              previousLenPlace += m.Length - var.Length;
                              sbSnippet.Remove(correctPosString, m.Length);
                              sbSnippet.Insert(correctPosString, var.Value);

                              if (CodePage != 65001)
                              {
                                  pos += chars.Length + var.Length;
                              }
                              else
                              {

                                  int count = correctPosString - previousPos + var.Value.Length;
                                  chars = new char[count];
                                  sbSnippet.CopyTo(previousPos, chars, 0, count);
                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );
                                 
                                  pos += strLen ;
                              }

                              previousPos = correctPosString + var.Length;


                              if (!imports.Contains(var.Value))
                                  imports.Add(var.Value);

                              indexArgument++;
                             
                          }
                          break;
                      #endregion
                      #region EventHandler
                      case WordTypes.EventHandler:
                          var = _vocabularyArgument.regEventHandler.Match(m.Value);



                          if (var.Value.Length!=0)
                          {


                              int correctPosString = m.Index - previousLenPlace;

                              previousLenPlace += m.Length - var.Length;
                              sbSnippet.Remove(correctPosString, m.Length);
                              sbSnippet.Insert(correctPosString, var.Value);

                              if (CodePage != 65001)
                              {
                                  pos += chars.Length + var.Length;
                              }
                              else
                              {

                                  int count = correctPosString - previousPos + var.Value.Length;
                                  chars = new char[count];
                                  sbSnippet.CopyTo(previousPos, chars, 0, count);
                                  int strLen = Encoding.UTF8.GetByteCount(
                                      chars
                                      );

                                  pos += strLen;
                                  
                              }
                              eventsHandler.Add(pos);
                              previousPos = correctPosString + var.Length;

                              indexArgument++;
                    
                          }
                          break;
                      #endregion
                  }

             }


            if (!dontCreateLastWord) 
            ConvertLastWord();


           

            return sbSnippet.ToString();

        }




        /// <summary>
        /// Convert Last word in list 
        /// </summary>
        public void ConvertLastWord()
        {
            if (secondLastWord != null)
            {
                VarDuplicateWordRegion vdw  =curWord as VarDuplicateWordRegion;

                if (vdw!=null)
                {

                    VarDuplicateWordRegion lwr = (VarDuplicateWordRegion)vdw.getLastWord();

                    secondLastWord.NextWord = lwr;

                    vdw.rootWord.SetLastVar(lwr);
                    lastWord = lwr;

                }
                else
                {

                    WordRegionBase lwr = curWord.getLastWord();
          
                    //    lwr.vwr = ((VarDuplicateWordRegion)curWord).vwr;

                    secondLastWord.NextWord = lwr;
                    // Words[Words.Count - 2].NextWord = lwr;
                    Words[Words.Count - 1] = lastWord = lwr;
                }

                

            }
            else if (imports!=null)
            {
                if (Words.Count == 1)
                {
                    WordRegionBase lwr = curWord.getLastWord();

                    Words[0] = firstWord = lwr;
                }
            }




            secondLastWord = null;
            curWord = null;
            dontCreateLastWord = false;
            if (Words.Count > 0)
            {
                firstWord = Words[0];
            }

            curIndWord = -1;
        }

        // string RemoveDiacritics(string stIn)
        //{
        //    string stFormD = stIn.Normalize(NormalizationForm.FormD);
        //    StringBuilder sb = new StringBuilder();

        //    for (int ich = 0; ich < stFormD.Length; ich++)
        //    {
                
        //        UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
        //        if (uc != UnicodeCategory.NonSpacingMark)
        //        {
        //            sb.Append(stFormD[ich]);
        //        }
        //    }

        //    return (sb.ToString().Normalize(NormalizationForm.FormC));
        //}

        public bool TryActivateMonitor()
        {

            if (numCursors > 1)
            {

                if (MonitorOnWordsActive != null)
                    MonitorOnWordsActive(this);

                ActiveMonitorWords();

                MoveNextWord();

                return true;
            }else if(numCursors==1)
            {

                sciMonitor = ASContext.CurSciControl;
                MoveNextWord();
                // retest if has generate new word
                if (numCursors == 1)
                    Words.Clear(); // no new word 
                else
                    return true;
            }

            return false;
            
        }
       
        
        public void CreateTemporaryVar()
        {

            sciMonitor = ASContext.CurSciControl;

            if (Words.Count == 0)
            {
                LastWordRegion wb = new LastWordRegion();
                wb.startWord = wb.endWord = sciMonitor.CurrentPos;
                Words.Add(wb);
                firstWord = wb;
                numCursors = 1;
            }
            
                declarationWord = new WordRegion();
                
                sciMonitor.TextInserted += new TextInsertedHandler(insertTemporaryText);
                sciMonitor.TextDeleted += new TextDeletedHandler(deleteTemporaryText);
            // it became firstword in the list

                declarationWord.startWord = declarationWord.endWord = firstWord.startWord;
                declarationWord.NextWord = firstWord;
                firstWord = declarationWord; 


            
        }

        public void RemoveTemporaryVar()
        {
            if (declarationWord != null)
            {
                firstWord = declarationWord.NextWord;
                sciMonitor.TextInserted -= new TextInsertedHandler(insertTemporaryText);
                sciMonitor.TextDeleted -= new TextDeletedHandler(deleteTemporaryText);
                declarationWord.Dispose();
                declarationWord = null;
               

            }

        }

        void deleteTemporaryText(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (position <= declarationWord.startWord)
                declarationWord.removeCharactersNextWord(length);
        }

        void insertTemporaryText(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (position <= declarationWord.startWord)
                declarationWord.addCharactersNextWord(length);
        }

        private void ActiveMonitorWords()
        {
            fileName = ASContext.Context.CurrentClass.Name;
            sciMonitor = ASContext.CurSciControl;
           
            sciMonitor.TextInserted += new TextInsertedHandler(sciMonitor_TextInserted);
            sciMonitor.TextDeleted += new TextDeletedHandler(sciMonitor_TextDeleted);

            sciMonitor.CharAdded += new CharAddedHandler(sciMonitor_CharAdded);
            sciMonitor.FocusChanged += new FocusHandler(sciMonitor_FocusChanged);
           

            currentForm = (FlashDevelop.Docking.TabbedDocument)ASContext.MainForm.CurrentDocument;
            currentForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(currentForm_FormClosed);


            reformater =   ASContext.CommonSettings.DisableCodeReformat;
            ASContext.CommonSettings.DisableCodeReformat = true;
            isMonitoring = true;
            
            //CompletionList.OnInsert += new InsertedTextHandler(insert);

            if (_setting._showHighLight && highlightWord==null)
            {
                highlightWord = new System.Windows.Forms.Timer();
                highlightWord.Interval = 700;
                highlightWord.Tick += new EventHandler(highlightWord_Tick);
                
            }
            else if (!_setting._showHighLight && highlightWord != null)
            {
                highlightWord.Tick -= new EventHandler(highlightWord_Tick);
                highlightWord = null;

            }

            if(highlightWord!=null)
                highlightWord.Start();
 
        }

    

        void currentForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            DeactiveMonitorWords();
        }

       
        
        

        void sciMonitor_CharAdded(ScintillaControl sender, int ch)
        {
            
            // Manually reformat
            if (ch == semicolon)
            {

                if (!reformater)
                {
                    ASCompletion.Completion.ReformatOptions options = new ASCompletion.Completion.ReformatOptions();
                    options.Newline = ASCompletion.Completion.ASComplete.GetNewLineMarker(sciMonitor.EOLMode);
                    options.CondenseWhitespace = ASContext.CommonSettings.CondenseWhitespace;
                    options.BraceAfterLine = ASContext.CommonSettings.ReformatBraces
                        && PluginCore.PluginBase.MainForm.Settings.CodingStyle == PluginCore.CodingStyle.BracesAfterLine;
                    options.CompactChars = ASContext.CommonSettings.CompactChars;
                    options.SpacedChars = ASContext.CommonSettings.SpacedChars;
                    options.SpaceBeforeFunctionCall = ASContext.CommonSettings.SpaceBeforeFunctionCall;
                    options.AddSpaceAfter = ASContext.CommonSettings.AddSpaceAfter.Split(' ');
                    options.IsPhp = ASContext.Context.Settings.LanguageId == "PHP";
                    options.IsHaXe = ASContext.Context.Settings.LanguageId == "HAXE";

                    int curPos = sciMonitor.CurrentPos;
                    int line = sciMonitor.LineFromPosition(curPos);


                    string oldtxt = sciMonitor.GetLine(line);



                    int startPos = sciMonitor.PositionFromLine(line);
                    int offset = sciMonitor.MBSafeLengthFromBytes(oldtxt, curPos - startPos);

                    int newOffset = offset;

                    string replace = ASCompletion.Completion.Reformater.ReformatLine(oldtxt, options, ref newOffset);
                    int position;


                    if (replace != oldtxt)
                    {



                        position = curPos + newOffset - offset;

                        List<WordReformat> wordsRf = wordsReformatFromLine(line, startPos);

                        if (wordsRf.Count > 0)
                        {
                            sciMonitor.TextInserted -= new TextInsertedHandler(sciMonitor_TextInserted);
                            sciMonitor.TextDeleted -= new TextDeletedHandler(sciMonitor_TextDeleted);

                            int j = 0;


                            WordReformat wrf = wordsRf[0];
                            int indexword = 0;
                            int newCharacters = 0;
                            bool enterWord = false;
                            WordRegionBase wbFinal = null;
                            for (int i = 0; i < oldtxt.Length; i++)
                            {
                                char c = oldtxt[i];
                                char cr = replace[j];

                                if (i == wrf.endWord && enterWord)
                                {
                                    enterWord = false;
                                    indexword++;
                                    //  next word

                                    if (newCharacters != 0)
                                        wrf.addCharactersNextWord(newCharacters);



                                    newCharacters = 0;
                                    if (indexword != wordsRf.Count)
                                    {
                                        wrf = wordsRf[indexword];

                                    }
                                    else
                                    {
                                        wbFinal = wrf.word.NextWord;
                                        // no other word to update
                                        if (wbFinal == null) break;
                                    }

                                    // exit word
                                }


                                while (cr != c)
                                {
                                    j++;
                                    cr = replace[j];
                                    newCharacters++;
                                }


                                if (i >= wrf.startWord && !enterWord)
                                {

                                    // enter to word
                                    enterWord = true;
                                    if (newCharacters != 0)
                                        wrf.shiftWord(newCharacters);
                                    newCharacters = 0;
                                }

                                j++;
                            }
                            if (newCharacters != 0 && wbFinal != null)
                                wbFinal.addCharactersNextWord(newCharacters);

                        }


                        sciMonitor.SetSel(startPos, startPos + sciMonitor.MBSafeTextLength(oldtxt));
                        sciMonitor.ReplaceSel(replace);
                        sciMonitor.SetSel(position, position);


                        if (wordsRf.Count > 0)
                        {


                            sciMonitor.TextInserted += new TextInsertedHandler(sciMonitor_TextInserted);
                            sciMonitor.TextDeleted += new TextDeletedHandler(sciMonitor_TextDeleted);

                            if (highlightWord != null)
                            {
                                highlightWord.Stop();
                                highlightWord.Start();
                            }
                        }
                    }
                }

                if (_setting._AutomaticNextWordWithSemiColon)
                {
                    if (curIndWord != (Words.Count - 1))
                        MoveNextWord();
                }
            }
        }


        private List<WordReformat> wordsReformatFromLine(int currentLine,int startPos)
        {
            List<WordReformat> wordsRf =  new List<WordReformat>();

            //bool hasVar = false;
            //int length = Words.Count;

            WordRegionBase wb = firstWord;
            int ln = 0;

            while (wb != null)
            {
                ln = sciMonitor.LineFromPosition(wb.endWord);
                if (ln == currentLine)
                {
                    WordReformat wr = new WordReformat();
                    wr.startWord = wb.startWord - startPos;
                    wr.endWord = wb.endWord - startPos;
                    wr.word = wb;
                    wordsRf.Add(wr);

                }
                else if (ln > currentLine)
                    break;
                


                wb = wb.NextWord;
            }

  

            return wordsRf;
        }


        private void DeactiveMonitorWords()
        {



            if (isList>-1)
            {
                CompletionList.OnInsert -= new InsertedTextHandler(CompletionList_OnInsert);
                isList = -1;
            }

            if (highlightWord != null)
            {
                QuickGenerator.CustomCompletionList.ExplorerProject.RemoveHighlights(sciMonitor);
                highlightWord.Stop();

                if (!_setting._showHighLight)
                {
                    highlightWord.Tick -= new EventHandler(highlightWord_Tick);
                    highlightWord = null;
                }
            }

            alreadyInvoke = false;


          
            ASContext.CommonSettings.DisableCodeReformat = reformater;


            if (declarationWord != null)
            {
                sciMonitor.TextInserted -= new TextInsertedHandler(DeclarationWordTextChange);
                sciMonitor.TextDeleted -= new TextDeletedHandler(DeclarationWordTextChange);
                declarationWord.NextWord = null;
                declarationWord = null;
            }

            if (importWord != null)
            {
                sciMonitor.TextInserted -= new TextInsertedHandler(DeclarationWordTextChange);
                sciMonitor.TextDeleted -= new TextDeletedHandler(DeclarationWordTextChange);
                importWord.NextWord = null;
                importWord = null;
            }

            if (sciMonitor != null)
            {

                sciMonitor.TextInserted -= new TextInsertedHandler(sciMonitor_TextInserted);
                sciMonitor.TextDeleted -= new TextDeletedHandler(sciMonitor_TextDeleted);
                sciMonitor.CharAdded -= new CharAddedHandler(sciMonitor_CharAdded);
                sciMonitor.FocusChanged -= new FocusHandler(sciMonitor_FocusChanged);
            //    sciMonitor.FocusChanged -= new FocusHandler(sciMonitor_FocusChanged);
               
                sciMonitor = null;

            }
            //if (tmChangeWordText != null)
            //{
            //    tmChangeWordText.Stop();
            //    tmChangeWordText.Tick -= new EventHandler(tmChangeWordText_Tick);
            //    tmChangeWordText = null;
            //}

   

            lastWord = null;
            curWord = null;
            secondLastWord = null;

            
            WordRegionBase wb = firstWord;
            
          

            WordRegionBase disp = null;

            while (wb != null)
            {
                
                disp = wb;
                wb = wb.NextWord;
                disp.Dispose();
            }

            Words.Clear();

            if(imports!=null)
            imports.Clear();
            if (eventsHandler != null)
            eventsHandler.Clear();

            if (AfterCurrentMember != null)
            AfterCurrentMember.Clear();


            lsVar.Clear();
            isMonitoring = false;
            firstWord = null;
           
            numCursors = 0;
           
            curIndWord = -1;


            if(currentForm!=null)
            {
                currentForm.FormClosed -= new System.Windows.Forms.FormClosedEventHandler(currentForm_FormClosed);
                currentForm=null;
            }
            if (MonitorOnWordsDeactive != null)
                MonitorOnWordsDeactive(this);
            return;
        }


        void HightLightWords()
        {
            if (ASContext.Panel == null) return;
            if (ASContext.Panel.InvokeRequired)
            {
                ASContext.Panel.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate { HightLightWords(); });
                return;
            }
            if (!ASContext.Context.IsFileValid) return;

            ScintillaNet.ScintillaControl sci = ASContext.CurSciControl;
            if (sci == null) return;


            QuickGenerator.CustomCompletionList.ExplorerProject.RemoveHighlights(sciMonitor);

            foreach (WordRegion item in Words)
            {

                if (item.type != WordRegion.kind.cursor)
                    QuickGenerator.CustomCompletionList.ExplorerProject.AddHighlights(sciMonitor, item.startWord, item.endWord - item.startWord, _setting._highLightColor, ScintillaNet.Enums.IndicatorStyle.RoundBox);
            }
        }


        void highlightWord_Tick(object sender, EventArgs e)
        {

            if (highlightWord == null) return;

            highlightWord.Stop();

            if (Words.Count == 0 || !isMonitoring)
                return;


            HightLightWords();
            
        }



        private void ChangeWordsVar()
        {
            
            if (sciMonitor == null) return;
            alreadyInvoke = false;
            sciMonitor.TextInserted -= new TextInsertedHandler(sciMonitor_TextInserted);
            sciMonitor.TextDeleted -= new TextDeletedHandler(sciMonitor_TextDeleted);


          
            int start;
            int end;
            //  sciMonitor.SetSel(wr.startWord, wr.endWord);

  
            VarDuplicateWordRegion varNotify = vwr.VarNotify;


            int actPos = sciMonitor.CurrentPos;
           
            if (varNotify == null)
            {
                //must be before
                vwr.ChangePositionVarsLink();

                start = sciMonitor.MBSafeCharPosition(vwr.startWord);
                end = sciMonitor.MBSafeCharPosition(vwr.endWord);
             


            }
            else
            {

                start = sciMonitor.MBSafeCharPosition(varNotify.startWord);
                end = sciMonitor.MBSafeCharPosition(varNotify.endWord + vwr.ChangeLength);

                actPos = actPos - varNotify.startWord;
                // muste be after

                // this change the position of the word
                vwr.ChangePositionVarsLink();
                actPos += varNotify.startWord; // new position
            }

            //vwr.newValueVar = sciMonitor.SelText; 
            string newValue = sciMonitor.Text.Substring(start, end - start);
           
           
            sciMonitor.IsUndoCollection = false;

  

            Int32 es = sciMonitor.EndStyled;
            Int32 mask = (1 << sciMonitor.StyleBits);
            sciMonitor.StartStyling(0, mask);
            sciMonitor.SetStyling(sciMonitor.TextLength, 0);
            sciMonitor.StartStyling(es, mask - 1);
  

            int lengthText = sciMonitor.MBSafeTextLength(vwr.ValueVar);
            int lengthHighLight = sciMonitor.MBSafeTextLength(newValue);



              VarDuplicateWordRegion varDup = vwr.NextVarDuplicate;


            
         

            if (varNotify != null)
            {

                sciMonitor.SetSel(vwr.startWord, vwr.startWord + lengthText);
                sciMonitor.ReplaceSel(newValue);

            }
       
                
   

            QuickGenerator.CustomCompletionList.ExplorerProject.AddHighlights(sciMonitor, vwr.startWord, lengthHighLight, System.Drawing.Color.MidnightBlue, ScintillaNet.Enums.IndicatorStyle.Box);

            while (varDup != null)
            {
                if (varDup != varNotify)
                {
                    sciMonitor.SetSel(varDup.startWord, varDup.startWord + lengthText);
                    sciMonitor.ReplaceSel(newValue);
                }

                QuickGenerator.CustomCompletionList.ExplorerProject.AddHighlights(sciMonitor, varDup.startWord, lengthHighLight, System.Drawing.Color.MidnightBlue, ScintillaNet.Enums.IndicatorStyle.Box);
                varDup = varDup.NextVarDuplicate;
            }


            sciMonitor.IsUndoCollection = true;
            sciMonitor.TextInserted += new TextInsertedHandler(sciMonitor_TextInserted);
            sciMonitor.TextDeleted += new TextDeletedHandler(sciMonitor_TextDeleted);
            vwr.ValueVar = newValue;
            vwr.VarNotify = null;
            


            sciMonitor.GotoPos(actPos);

            if (vwr.Disactive)
            {
                vwr.NextVarDuplicate = null;
            }
           if (retest)
           {

               retest = false;

               if (isList>-1)
               {
                   isList = -1;
                   if (curIndWord < Words.Count - 1)
                       MoveNextWord();

                   return;
               }
               
               MoveNextWord();
           }

     
        }


         ///<summary>
         ///Deactive Monitor if sci lost control
         ///</summary>
         ///<param name="sender"></param>
        void sciMonitor_FocusChanged(ScintillaControl sender)
        {

            if (sender.Focused)
            {
         
                ASContext.CommonSettings.DisableCodeReformat = true;

                if (isList>-1)
                {
                    CompletionList.OnInsert += new InsertedTextHandler(CompletionList_OnInsert);
                }

                if (GotFocus != null)
                    GotFocus(this, sciMonitor);
            
            }
            else
            {  
                ASContext.CommonSettings.DisableCodeReformat = reformater;

                if (isList>-1)
                {
                    CompletionList.OnInsert -= new InsertedTextHandler(CompletionList_OnInsert);
                }
            }

        }

       

        void sciMonitor_TextDeleted(ScintillaControl sender, int position, int length, int linesAdded)
        {
   
            if (position > lastWord.endWord) {DeactiveMonitorWords(); return;}
           
            if (length > 1)
            {

                WordRegionBase first = TestDeleteInRegion(position);
                //WordRegion second = TestInRegion(position + length);

                if (first == null || first.endWord < (position + length))
                {
                    DeactiveMonitorWords();

                    return;
                }
                else
                {
                    first.removeCharactersFromRegion(length);
                    if (highlightWord != null)
                    {
                        highlightWord.Stop();
                        highlightWord.Start();
                    }
                    return;
                }

            }



            WordsDeleteChange(position, length);

            if (highlightWord != null)
            {
                highlightWord.Stop();
                highlightWord.Start();
            }
          
          
        }


        void sciMonitor_TextInserted(ScintillaControl sender, int position, int length, int linesAdded)
        {
        
            if (linesAdded > 0)
            {
                
                // Control import/declaration generator
              
                    if (length > 4)
                    {
                       
                        // only a unique temporary world

                        string strControl = sciMonitor.GetLine(sciMonitor.LineFromPosition(position));
                        // is a snippet ,FlashDevelop has generated code


                        int indexImport = -1;
                      
                        // FlashDevelop generate snippet
                        if (strControl.IndexOf("$(Boundary)") == -1 && (indexImport= strControl.IndexOf("import")) == -1)
                        {
                            DeactiveMonitorWords();
                            return;
                        }

                        WordRegion newStatmentWord = new WordRegion();

                            newStatmentWord.endWord = newStatmentWord.startWord = position;
                        // is import
                            if (indexImport != -1)
                            {
                                newStatmentWord.type = WordRegionBase.kind.Import;

                                if (importWord != null)
                                    RemoveDeclarationWord(sender, importWord);

                                importWord = newStatmentWord;

                            }
                            else
                            {
                               
                                newStatmentWord.type = WordRegion.kind.temporary;
                                if (declarationWord != null)
                                    RemoveDeclarationWord(sender, declarationWord);

                                declarationWord = newStatmentWord;
                            }


                            newStatmentWord.NextWord = firstWord;
                            firstWord = newStatmentWord;
                      


                            sender.TextInserted += new TextInsertedHandler(DeclarationWordTextChange);
                            sender.TextDeleted += new TextDeletedHandler(DeclarationWordTextChange);
                          

                            if (indexImport == -1)
                            {
                                int posInsert = curIndWord;
                                if (posInsert >= Words.Count)
                                { posInsert = Words.Count - 1; }
                                else if (posInsert < 0) posInsert = 0;

                                Words.Insert(posInsert, newStatmentWord);
                            }


                            wrTested = newStatmentWord;
                            newStatmentWord.addCharactersToRegion(length);

                            if (highlightWord != null)
                            {
                                highlightWord.Stop();
                                highlightWord.Start();
                            }
                        
                            return;
                        
                    }


                    DeactiveMonitorWords();
                    return;
               
            }



          
            // Write before abbreviation
            if (position > lastWord.endWord) { DeactiveMonitorWords(); return; }



            WordsInsertChange(position, length);

            if (highlightWord != null)
            {
                highlightWord.Stop();
                highlightWord.Start();
            }
            
        }

        void DeclarationWordTextChange(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if(wrTested!=null && isMonitoring)           
            if ( wrTested.type != WordRegionBase.kind.Import)
                {
                    if (importWord != null)
                    {
                        RemoveDeclarationWord(sender, importWord);
                        
                        importWord = null;
                        if (isList>-1)
                        {
                            //curIndWord--;
                            curIndWord = isList;
                           // MoveNextWord();
                        }
                    }

                    if (declarationWord != null && wrTested.type!= WordRegionBase.kind.temporary)
                    {
                        RemoveDeclarationWord(sender, declarationWord);
                        declarationWord = null;
                    }
                }
        }

        private void RemoveDeclarationWord(ScintillaControl sender, WordRegion wordToDelete)
        {
            firstWord = wordToDelete.NextWord;
            if (wordToDelete.type != WordRegionBase.kind.Import)
                Words.Remove(wordToDelete);
            wordToDelete.NextWord = null;
            wordToDelete = null;
            sender.TextInserted -= new TextInsertedHandler(DeclarationWordTextChange);
            sender.TextDeleted -= new TextDeletedHandler(DeclarationWordTextChange);
        }


        private void WordsInsertChange(int position, int length)
        {

            wrTested = InsertInRegion(position);

            if (wrTested == null) { DeactiveMonitorWords(); return; }

             wrTested.addCharactersToRegion(length);
            
        }

        private void WordsDeleteChange(int position, int length)
        {

            wrTested = TestDeleteInRegion(position);

            if (wrTested == null) { DeactiveMonitorWords(); return; }

             wrTested.removeCharactersFromRegion(length);


        }


     
        void vwr_ChangeVarWord(VarWordRegion vwr)
        {
            if (!alreadyInvoke)
            {

                this.vwr = vwr;
                sciMonitor.BeginInvoke(changeWord);
                alreadyInvoke = true;
            }
        }


        

        /// <summary>
        /// Control if the text is write inside in a word region
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public WordRegionBase InsertInRegion(int position)
        {
          
            //int pos = -1;

         

            WordRegionBase wr = firstWord;
            int i = 0;

            while (wr != null)
            {
                if (position <= wr.endWord)
                {
                    if ((wr.startWord <= position))
                    {
                        curIndWord = i;

                        //curIndWord = word.index;
                        return wr;
                    }
                    else
                        return null;
                }


                wr = wr.NextWord;
                i++;
            }



            return null;
        }


        /// <summary>
        /// Control the position caret
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public WordRegionBase TestInRegion(int position)
        {


            int length = Words.Count;

            for (int i = 0; i < length; i++)
            {
                if (position <= Words[i].endWord)
                {
                    if ((Words[i].startWord <= position))
                    {
                        curIndWord = i;

                        //curIndWord = word.index;
                        return Words[i];
                    }
                    else
                    {
                        if (declarationWord != null) continue;
                        return null;
                    }
                }
            }


            return null;
        }

        /// <summary>
        /// Control if the text is delete inside in a word region
        /// Is different from TestInRegion
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private WordRegionBase TestDeleteInRegion(int position)
        {


            WordRegionBase wr = firstWord;
            int i = 0;

            while (wr != null)
            {
                if (position < wr.endWord)
                {
                    if ((wr.startWord <= position))
                    {
                        curIndWord = i;

                        return wr;
                    }
                    else
                    return null;
                }


                wr = wr.NextWord;
                i++;
            }

            return null;
        }

        public void GoToCurrentWord()
        {
            if (currentForm == null) return;
            if (!currentForm.Focused)
            {
                currentForm.Activate();
                currentForm.Focus();

            }

            curIndWord--;
            MoveNextWord();
            
        }
      
        /// <summary>
        /// Place the cursor a the end of the next word
        /// </summary>
        public void  MoveNextWord()
        {
        

            if (isList>-1)
            {

                CompletionList.OnInsert -= new InsertedTextHandler(CompletionList_OnInsert);
                if (CompletionList.Active)
                    CompletionList.Hide();
                isList = -1;
            }


            if (UITools.CallTip.CallTipActive)
                UITools.CallTip.Hide();

            curIndWord++;

            if (curIndWord >= Words.Count ) curIndWord = 0;

             WordRegionBase wr =  Words[curIndWord];


               if (wr.type == WordRegion.kind.place || wr.type== WordRegionBase.kind.VarLink)
               {
                   sciMonitor.SetSel(wr.startWord, wr.endWord);
          
                   return;
               }
               else if (wr.type == WordRegionBase.kind.Parameter)
               {
                   sciMonitor.SetSel(wr.startWord, wr.endWord);
                   
                   ((WordRegionParameter)wr).ShowTips(sciMonitor);
             
                  
               }
               else if (wr.type == WordRegionBase.kind.customList)
               {
                   sciMonitor.SetSel(wr.startWord, wr.endWord);
                   WordCustomList wc = (WordCustomList)wr;
                   List<PluginCore.ICompletionListItem> lcomp = new List<PluginCore.ICompletionListItem>();
                   List<string> ls = null;
                   if (_dictCustomList.ContainsKey(wc.indList))
                   {

                       ls = _dictCustomList[wc.indList];

                       if (ls.Count == 0)
                       {
                           UITools.CallTip.CallTipShow(ASContext.CurSciControl,ASContext.CurSciControl.CurrentPos, "This List is empty!!");
                           return;
                       }
                       else
                       {
                           foreach (string item in ls)
                           {
                               lcomp.Add(new CompletionListItem(item, ManagerResources.EmptyBitmap));
                           }
                       }
                   }
                   else
                   {
                       UITools.CallTip.CallTipShow(ASContext.CurSciControl, ASContext.CurSciControl.CurrentPos, "No list at current index!!");
                       return;
                   }

                   CompletionList.OnInsert += new InsertedTextHandler(CompletionList_OnInsert);
                   CompletionList.Show(lcomp, false);

                 


                   isList = curIndWord;

               }
               else if (wr.type == WordRegion.kind.showCompType || wr.type == WordRegionBase.kind.VarLinkAndComp)
               {
                   sciMonitor.SetSel(wr.startWord, wr.endWord);
             
                   if (ASContext.HasContext && ASContext.Context.IsFileValid)
                   {
                       int pos = wr.startWord - 1;
                       char c = (char)sciMonitor.CharAt(pos);

                       if (c == ' ')
                           c = '.';


                   
                    //   CompletionList.OnChar(sciMonitor, (int)c);
                      UITools.Manager.SendChar(sciMonitor, c);
                      if (!CompletionList.Active) return;

                    
                       CompletionList.OnInsert += new InsertedTextHandler(CompletionList_OnInsert);
                       isList = curIndWord;
                       
                   }

               }
               else if (wr.type == WordRegion.kind.temporary )
               {
                   //if (wr.type == WordRegionBase.kind.Import)
                   //{

                   //    MoveNextWord();
                   //    // eliminate import word
                   //    Words.Remove(declarationWord);
                   //    firstWord = declarationWord.NextWord;
                   //    declarationWord.NextWord = null;
                   //    declarationWord = null;

                   //    return;
                   //}

                   sciMonitor.GotoPos(wr.endWord - 1);

               }
               else if (wr.type == WordRegion.kind.cursor)
               {
                   sciMonitor.GotoPos(wr.endWord);
                   //isCursorWord = true;
               }
               else if (wr.type == WordRegionBase.kind.createParameters)
               {
                   sciMonitor.GotoPos(wr.startWord);
                   ASCompletion.Completion.ASResult result = null;

                   // sciMonitor.GotoPos(wr.startWord);

                   

                  int pos = sciMonitor.CurrentPos;
                  int pos2 = pos;

                  char c =(char) sciMonitor.CharAt(--pos);


                   while(char.IsWhiteSpace(c))
                   {
                       c = (char)sciMonitor.CharAt(--pos);
                   }

                   if (c != '(') return;

                   char c2 = (char)sciMonitor.CharAt(pos2);


                   while (char.IsWhiteSpace(c2))
                   {
                       c2 = (char)sciMonitor.CharAt(++pos2);
                   }

                   if (c2 != ')') return;


                  if(PathExplorer.IsWorking)
                  {
                      Abbreviations.WaitFinishPathExplorer();
                  }

                   c = (char)sciMonitor.CharAt(--pos);


                  while (char.IsWhiteSpace(c))
                  {
                      c = (char)sciMonitor.CharAt(--pos);
                  }
                  pos++;

                   
                   //sciMonitor.WordLeft();
                   //string str = sciMonitor.GetWordFromPosition(pos);
                   //string str1 = sciMonitor.GetLine(sciMonitor.LineFromPosition(wr.startWord));
                     result = ASCompletion.Completion.ASComplete.GetExpressionType(sciMonitor, pos);
                  

                   
                   if (result.Member != null)
                   {
                       
                       if ( result.Member.Parameters!=null &&  result.Member.Parameters.Count > 0)
                       {
                           if (result.Member.Parameters[0].Value == null &&  result.Member.Parameters[0].Type!=null && result.Member.Parameters[0].Name != "...rest")
                           {

                               TextParameters tp = new TextParameters(result.Member);

                               CreateParameters(result.Member.Parameters, tp);
                               return;
                           }
                           else
                           {
                               sciMonitor.GotoPos(wr.endWord);
                               ASCompletion.Completion.ASComplete.HandleFunctionCompletion(sciMonitor, true);
                           }
                       }
                       else
                       {
                           sciMonitor.GotoPos(wr.endWord);
                       }
                   }
                   else if (result.Type != null)
                   {
                       if (result.Type.Members.Count > 0)
                       {

                           foreach (MemberModel ml in result.Type.Members)
                           {
                               if ((ml.Flags & FlagType.Constructor) > 0)
                               {
                                   if (ml.Parameters != null && ml.Parameters.Count > 0)
                                   {
                                       if (ml.Parameters[0].Value == null && ml.Parameters[0].Name != "...rest")
                                       {
                                           TextParameters tp = new TextParameters(ml);


                                           CreateParameters(ml.Parameters, tp);

                                       }
                                       else
                                       {
                                           sciMonitor.GotoPos(wr.endWord);
                                           ASCompletion.Completion.ASComplete.HandleFunctionCompletion(sciMonitor, true);
                                       }

                                   }


                                   break;
                               }
                           }
                       }
                       else
                       {
                           sciMonitor.GotoPos(wr.endWord);
                       }

                   }
                   else
                   {
                       sciMonitor.GotoPos(wr.endWord);
                   }



               }
               else if (wr.type == WordRegion.kind.browser)
               {
                   sciMonitor.SetSel(wr.startWord, wr.endWord);
                   System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog();

                   //  isOpenDialog = true;
                   of.ShowDialog();
                   //isOpenDialog = false;
                   if (of.FileName.Length!=0)
                   {

                       int ind = of.FileName.IndexOf(ASContext.Context.CurrentModel.BasePath);


                       if (ind == -1)
                       {
                           // is extern to project
                           sciMonitor.ReplaceSel("\"" + of.FileName + "\"");
                       }
                       else
                       {

                           string relPath = ProjectManager.Projects.ProjectPaths.GetRelativePath(
                               System.IO.Path.GetDirectoryName(ASContext.Context.CurrentFile), of.FileName).Replace('\\', '/');

                           sciMonitor.ReplaceSel("\"" + relPath + "\"");
                       }

                       if (curIndWord != Words.Count - 1)
                           MoveNextWord();
                   }

               }

               

          
        
        }

        /// <summary>
        /// This function is create only for addEventListener
        /// </summary>
        public void ActivateList()
        {
            CompletionList.OnInsert += new InsertedTextHandler(CompletionList_OnInsert);
            isList = curIndWord;
            Words[curIndWord].type = WordRegionBase.kind.showCompType;
        }


        void CompletionList_OnInsert(ScintillaControl sender, int position, string text, char trigger, PluginCore.ICompletionListItem item)
        {
 
            CompletionList.OnInsert -= new InsertedTextHandler(CompletionList_OnInsert);

            
            if (trigger == '.' || trigger == ';' || trigger == '=') { isList = -1; return; }
            if (Words == null) return;

            if (alreadyInvoke)
            {
                retest = true;
                return;
            }

           
            if (curIndWord==isList && curIndWord < Words.Count - 1 )
            {
                isList = -1;
                MoveNextWord();
            }
            else
                isList = -1;
        }


        public void CreateParameters(List<ASCompletion.Model.MemberModel> parameters, TextParameters textParameters)
        {

            WordRegionBase previous = null;
            WordRegionBase nextWord = null;
            StringBuilder sb = new StringBuilder(10);
            int actpos = ASContext.CurSciControl.CurrentPos;
            int varpos = actpos;
            int index =0;
            WordRegionBase wrCurrent = null;
            if (!isMonitoring)
            {
                Words = new List<WordRegionBase>();
                curIndWord = index;

               
            }
            else
            {
               // if (curIndWord == Words.Count) curIndWord--;
                if (highlightWord != null)
                    highlightWord.Stop();

                 wrCurrent = firstWord;
                int position = sciMonitor.CurrentPos;

                TestInRegion(position);

                // La parola nella quale viene inserito/rimosso il carattere
                while (wrCurrent != null)
                {

                    if (position <= wrCurrent.endWord)
                    {
                        if ((wrCurrent.startWord <= position))
                        {
  
                            break;
                        }
                       
                    }

                    previous = wrCurrent;
                    wrCurrent = wrCurrent.NextWord;
                   
                }

                
                nextWord = wrCurrent.NextWord;
                

                sciMonitor.TextInserted -= new TextInsertedHandler(sciMonitor_TextInserted);
                index = curIndWord;
                Words.RemoveAt(curIndWord);
               
                
            }

            int startToolTip = textParameters.text.IndexOf("(") +1;
            textParameters.posParameters = actpos - startToolTip + 1;
          //  int indexToolTip = 0;

            WordRegionParameter wordParamater=null;
            WordRegionBase realPrevious = null;
            foreach (ASCompletion.Model.MemberModel mm in parameters)
            {
                if (mm.Value != null || mm.Name == "...rest") break;
               

                wordParamater = new WordRegionParameter();
                wordParamater.startWord = varpos;
                wordParamater.endWord = varpos + mm.Name.Length;
                wordParamater.type = WordRegionBase.kind.Parameter;
                wordParamater.textParameters = textParameters;
                wordParamater.startToolTip = startToolTip;
                //es              ( X              :    number)

                int typeLength = (mm.Type != null) ? mm.Type.Length : 0;

                wordParamater.endToolTip = startToolTip + mm.Name.Length + 1 + typeLength;
                // (, )
                startToolTip = wordParamater.endToolTip + 2;
                //indexToolTip++;

                //wr.indexToolTip = indexToolTip;
                sb.Append(mm.Name);
                sb.Append(", ");
                varpos = actpos + sb.Length;


                if (textParameters.comments != null)
                {

                    Match mParam = Regex.Match(textParameters.comments, "@param\\s+" + Regex.Escape(mm.Name) + "[ \t:]+(?<desc>[^\r\n]*)");
                    
					if (mParam.Success)
					{
                        wordParamater.textParameter = "\n[B]" + mm.Name + ":[/B] " + mParam.Groups["desc"].Value.Trim();
					}

                }
                

                Words.Insert(index, wordParamater);
                index++;
                if (previous == null)
                {
                    previous = wordParamater;
                }
                else
                {
                    previous.NextWord = wordParamater;
                    realPrevious = previous;
                    previous = wordParamater;
                }


              
            }


            sb.Remove(sb.Length - 2, 2);

            ASContext.CurSciControl.InsertText(actpos, sb.ToString());

            if (Words.Count > 1)
            {
                numCursors = 2;



                // last word
                if (nextWord == null)
                {
                    index--;
                   // WordRegionBase wb = Words[index];
                   // LastWordRegionParameter lw = (LastWordRegionParameter)wb.getLastWord();
                    WordRegionBase lw = wordParamater.getLastWord();
                   // Words[index - 1].NextWord = lw;
                    if (realPrevious != null)
                    {
                        realPrevious.NextWord = lw;
                    }
                    
                    Words[index] = lw;
                    lastWord = lw;
                }
                else
                {
                    previous.NextWord = nextWord;
                }

                firstWord = Words[0];


                if (!isMonitoring)
                {
                    curIndWord = -1;
                    TryActivateMonitor();

                    if (highlightWord != null)
                        highlightWord.Start();

                    return;
                }
            }
            else
            {
                if(!isMonitoring)
                ASContext.CurSciControl.SetSel(previous.startWord, previous.endWord);
                numCursors = 1;
            }


            if (isMonitoring)
            {
                if (highlightWord != null)
                    highlightWord.Start();

                if (nextWord != null)
                {
                    nextWord.addCharactersNextWord(sb.Length);
                    wrCurrent.Disable();

                }
                sciMonitor.TextInserted += new TextInsertedHandler(sciMonitor_TextInserted);
                curIndWord--;
                MoveNextWord();
            
            }
        }


        public ScintillaControl SciMonitor
        {
            get { return sciMonitor; }
        }

        public bool IsMonitoring
        {
            get { return isMonitoring; }
        }

         public string FileName
        {
            get { return fileName; }
        }

        #region IDisposable Membri di

        public void Dispose()
        {



            if (declarationWord != null)
            {
                declarationWord.Dispose();
                declarationWord = null;
            }
            _vocabularyArgument = null;

            if (sciMonitor != null)
            {
                DeactiveMonitorWords();
            }

            _setting = null;

            if (Words == null ) return;




            foreach (WordRegion item in Words)
            {
                item.Dispose();
            }

            if (highlightWord != null)
            {
                highlightWord.Stop();
                highlightWord.Dispose();
                highlightWord = null;
                
            }


            Words.Clear();

            Words = null;
            imports = null;
            
            AfterCurrentMember = null;

            
           
        }

        #endregion

       


       
    }


}
