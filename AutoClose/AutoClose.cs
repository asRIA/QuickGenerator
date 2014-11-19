//SciControl_CharAdded: 
//It checks the inserted character.
//If the character is a bracket opening, it will insert the bracket closing.
//This will not happen if:
//- The bracket is inside the comments.
//- There are some characters on the right side.

//Due the fact that AutoClose will always close the bracket, if a user has the habit to manually close the bracket, a lot of time he will see a double closing bracket.
//The plug-in check when a closing bracket is digit, what is the next character. If the next character is the same, the plug-in will auto remove the eventual double closing bracket

   
//  SciControl_BeforeDelete:
//It checks if the opening bracket is deleted by the user.
//In this case if there is a closing bracket on the right side, it will be auto deleted.
//ES: the user push ‘(’ and the AutoClose insert the ‘)’. 
//if now the user delete ‘(’ also the ‘)’ will be deleted.

  
//  CompletionList_OnInsert:
//If one completionList is visible and the user press the Enter button, it will insert the characters ‘()’.
//Before doing that, the plug-in will check the validity of the inserted word using <currentNotifyEvent>, a value obtained from HandleEvent and after that it checks the presence of the characters on the right to evaluate if wirte the character ‘;’.

//If the user pressed the key ‘(’, this procedure will not be activated and the plug-in will handle it with SciControl_CharAdded.
  
//  <currentNotifyEvent>  it is used to understand if the word inserted is a function or a new type or other.
//If it is a function, something must check if it is a constructor and in this case, the plug-in check the presence of the prefix ‘new’.
//If it is not a constructor, something must check if the word inserted is used as a parameter for another function, looking at the first character (it not must be tab,space,newline, ect.) present on the right, if this character is a‘)’ or a ‘,’.
//If one of these 2 characters is present, it most probable that it is inside a function and so the plug-in will check the truth of this info and if positive it check which position it has as a parameter.
//If the truth check give a positive answer, the plug-in will not insert the ‘()’.

//Otherwise if the inserted word is a type, the plug-in will check the presence of the word ‘new’.
//In order to do this check, the plug-in must move the mouse pointer behind one word with the instruction WordLeft in order to manually check the presence of this keyword.

//After checking that the word inserted is a function or a type, the plug-in will generate the parameters.
//It will happen only if there is a parameter and this is valid.
  

//NOTE:
//inside HandleEvent the plug-in will store not only <currentNotifyEvent>, but also check the insert or the creation of a new document for monitoring purpose with AutoClose.

//Due the fact that I don’t know how to obtain the handle of this document, I used the following procedure:
//- At the creation of the document I register the plug-in for the event ‘FileSwitch’.
//- After the docs will be exchanged I obtain the handle.
//- After that I remove the ‘FileSwitch’ because I use it only in this case.
  

//--------------  ITALIAN --------------------------------

/*
 *  SciControl_CharAdded: Controlla il carattere inserito.Se è l'appertura di una 
 *  parenti ne inserisce la sua corrispettiva chiusura. Questo non avviene se:
 *  - è dentro ad un commento
 *  - ci sono delle lettere sul lato destro
 *  
 *  Dato che auto close chiude le parentesi, se si è abituati ad inserire le parentesi a mano, c'è il rischio di reinserire la chiusura
 *  per poi doverla ricancellare. Quindi ho fatto in modo che
 *  all'inserimento della chiusura viene controllato se il carattere succesivo non sia lo stesso carattere,
 *  che in caso affermativo con un procedimento di eliminazione ne impedisco l'inserimento.
 *  
 * SciControl_BeforeDelete :Controlla se la parentesi di apertura viene cancellata.
 * In questo caso se c'è una  parentesi di chiusura sulla sua parte destra anche questa viene cancellata.
 * Es. Premo '(' e autoclose inserisce anche ')'.Ora se cancello '(' anche la ')' sarà cancellata.
 * 
 * CompletionList_OnInsert :Se una completionList è visibile ed è stato premuto il tasto enter inserisce i caratteri '()' .
 * Prima però controllo la validità della parola inserita usando <currentNotifyEvent>  valore ricavato da HandleEvent
 * e succesivamente controlla la presenza di caratteri alla sua destra per valutare se deve inserire il carattere ';'.
 * 
 * Se è il tasto '(' a scatenare l'inserimento questo proccedimento non avviene e faccio gestire il tutto da SciControl_CharAdded.
 * 
 * <currentNotifyEvent>  serve per capire se la parola inserita sia una funzione o un nuovo tipo o altro.
 * Se è una funzione si deve controllare se è un construttore e in tal caso si deve controllare se è
 * preceduto dalla parola 'new'.
 * Se non è un costruttore allora si deve controllare se invece la parola inserita venga utilizzata come un parametro di un'altra funzione, 
 * analizzando  il primo carattere   (che non sia tab,space,newline, ect.) alla sua destra sia una ')'
 * o una ','. Se uno di questo carattere è presente è probabile che sia dentro una funzione quindi controllo se è
 * così e nel caso a quale posto si trova come parametro.Se la verifica da esito positivo non inserisco '()'.
 * 
 * Se è invece la parola inserita è un tipo  controllo la presenza della parola 'new'.
 * Per questa verifica devo spostare per forza il cursore indietro di un parola con l'istruzione
 * WordLeft per poi poter controllare manualmente la presenza di quella parola chiave.
 * 
 * Una volta verificato che sia una funzione o tipo genero i parametri.
 * Questo avviene solo se esiste almeno un parametro e che sia valido.
 * 
 * Nota:
 * In HandleEvent  non solo imaggazino <currentNotifyEvent> ma controllo l'inserimento o creazione di un nuovo documento da poter monitorato con AutoClose.
 * Dato che però non sapevo come ottenere l'handle di questo documento ho usato il seguente sistema:
 * - alla creazione del documento registro il plugin per l'evento 'FileSwitch'.
 * - una volta che il documento viene scambiato nè ricavo l'handle.
 * - dopo rimuovo 'FileSwitch' che mi serviva solo in questo caso.
 * 
 */

using System;
using System.Collections.Generic;
using ScintillaNet;
using FlashDevelop.Docking;
using PluginCore;
using PluginCore.Managers;
using ASCompletion.Completion;
using System.Collections;
using ASCompletion.Model;
using QuickGenerator.Abbreviation;

namespace QuickGenerator.Double
{
    class AutoClose  : IEventHandler ,IDisposable
    {
      //  Dictionary<IntPtr, MonitorSci> dict;
        DataEvent currentData;
        NotifyEvent currentNotifyEvent;
        private bool _isEnableFunctionClose;
        public Abbreviations abbreviations;
        private int[] newChars = new int[3];

        Dictionary<IntPtr, TabbedDocument> dict;
        delegateRemoveBracket HanldeRemoveBracket;
        public delegate void delegateRemoveBracket(ScintillaControl sci);

        const int bracketOpen = (int)'(';
        const int bracketClose = (int)')';
        const int quote = (int)'"';
        const int squareBracketOpen = (int)'[';
        const int squareBracketClose = (int)']';
        private readonly char[] arrBracketClose = new char[] { ')' };
        private readonly char[] arrSquareBracketClose = new char[] { ']' };
        private readonly char[] arrQuote = new char[] { '"' };
        char[] insert;


        public AutoClose()
        {
            //dict = new Dictionary<IntPtr, MonitorSci>();
            dict = new Dictionary<IntPtr, TabbedDocument>();
            activateEvent();
            newChars[0] = 'w';
            newChars[1] = 'e';
            newChars[2] = 'n';
            
             
        }



        public void EnableFunctionAndNewClose()
        {
            if (_isEnableFunctionClose) return;
            _isEnableFunctionClose = true;
            PluginCore.Controls.CompletionList.OnInsert += new PluginCore.Controls.InsertedTextHandler(CompletionList_OnInsert);
            EventManager.AddEventHandler(this, EventType.Command);
        }


        public void disableFunctionAndNewClose()
        {
            if (!_isEnableFunctionClose) return;
            _isEnableFunctionClose = false;
            PluginCore.Controls.CompletionList.OnInsert -= new PluginCore.Controls.InsertedTextHandler(CompletionList_OnInsert);
            EventManager.RemoveEventHandler(this);
            activateEvent();
            
        }
        

        void CompletionList_OnInsert(ScintillaControl sender, int position, string text, char trigger, ICompletionListItem item)
        {


            if (trigger == '(' || trigger == '.') return;
            if (!(item is MemberItem)) return; // Generate Event
      //      if (item is EventItem) return;
             currentData = (DataEvent)currentNotifyEvent;
             Hashtable table = currentData.Data as Hashtable;
             if (table==null) return; 

            
            ASResult res = (table)["context"] as ASResult;

            if (res == null) return;

            MemberModel member = res.Member;
            int posAdd = 0;

                    if (member != null)
                    {
                        if ((member.Flags & FlagType.Function) == 0) { return; }
                        
                           

                            int pos = sender.CurrentPos;
                            int insertPos = pos;
                            if (((member.Flags & FlagType.Constructor) > 0))
                            {

                                if (!thereIsNewWord(sender))
                                {
                                    sender.GotoPos(pos);
                                    return;
                                }
                            }


                          //  sender.ReplaceSel
                                bool hasParameters = false;

                                char lastChar=' ';
                                posAdd = SearchNextNewLineWithoutChar(sender, position, text, ref lastChar);

                                if (lastChar == '(')
                                {
                                    return;
                                }

                            // Search if is a parameter of a function
                                if (lastChar == ',' || lastChar == ')')
                                {
                                    if (IsFunctionParameter(sender, position - 1))
                                    {
                                        return;
                                    };
                                }


                                sender.BeginUndoAction();

                                if (posAdd > 0)
                                {
                                    sender.InsertText(pos, "();");
                                    posAdd = 1;

                                }
                                else
                                    sender.InsertText(pos, "()");


                             


                                pos++;

                            
                                if (!(trigger == '[' || trigger == '"'))
                                {
                                    if (member.Parameters != null)
                                    {
                                        if (member.Parameters.Count == 0)
                                        {
                                            pos += 1 + posAdd;

                                        }
                                        else
                                        {
                                            hasParameters = true;
                                        }

                                    }
                                    else
                                    {
                                        pos += 1 + posAdd;
                                    }


                                }

                           

                                sender.GotoPos(pos);

                                if (hasParameters)
                                {
                                    if (abbreviations != null &&  member.Parameters[0].Value == null && member.Parameters[0].Name != "...rest")
                                    {

                                       // string str = res.Member.ToString();
                                        TextParameters tp = new TextParameters(res.Member);
                                         

                                        if (member.Name.EndsWith("EventListener"))
                                        {
                                            if (text.EndsWith("Event"))
                                            {
                                                sender.GotoPos(insertPos+2);
                                                sender.DeleteBack();
                                                sender.DeleteBack();
                                                sender.EndUndoAction();
                                                return;
                                            }
                                            abbreviations.CreateParameters(member.Parameters, true,tp);
                                         

                                        }
                                        else
                                            abbreviations.CreateParameters(member.Parameters, false,tp);


                                        sender.EndUndoAction();
                                        return;
                                    }

                                  

                                    ASComplete.HandleFunctionCompletion(sender, true);
                                }


                                sender.EndUndoAction();
                       
                       
                    }
                    else if (res.Type != null)
                    {

                    
                        int pos2 = sender.CurrentPos;
         

                        bool hasParameters = false;
                        MemberModel mlConstructor = null;

                        if (!thereIsNewWord(sender)) { sender.GotoPos(pos2); return; }
                        
                            char lastChar=' ';
                            posAdd = SearchNextNewLineWithoutChar(sender, pos2, "",ref lastChar);

                            if (lastChar == '(') { sender.GotoPos(pos2); return; }
                          
                            if (res.Type.Members.Count > 0)
                            {

                                foreach (MemberModel ml in res.Type.Members)
                                {
                                    if ((ml.Flags & FlagType.Constructor)>0)
                                    {
                                        if (ml.Parameters!=null && ml.Parameters.Count > 0)
                                        {
                                            mlConstructor = ml;
                                            hasParameters = true;
                                        }
                                        break;
                                    }
                                }

                                
                            }
                           
                            if(posAdd>0)
                                sender.InsertText(pos2, "();");
                            else
                            sender.InsertText(pos2, "()");

                            if (trigger == '[' || trigger == '"' || hasParameters)
                              pos2++;
                            else
                            pos2 += 2 + posAdd;
                        


                        sender.GotoPos(pos2);
                        if (hasParameters)
                        {
                            sender.BeginUndoAction();
                            if (abbreviations != null && mlConstructor.Parameters[0].Value == null && mlConstructor.Parameters[0].Name != "...rest")
                            {
                                TextParameters tp = new TextParameters(mlConstructor);
                           

                               abbreviations.CreateParameters(mlConstructor.Parameters, false, tp);
                            }
                            else
                                ASComplete.HandleFunctionCompletion(sender, true);
                            sender.EndUndoAction();
                        }


                        
                    }

               
              
        }
        /// <summary>
        /// Search the word new left current word
        /// </summary>
        /// <param name="sci"></param>
        /// <returns></returns>
        private bool thereIsNewWord(ScintillaControl sci)
        {

            sci.WordLeft();   
            int pos = sci.CurrentPos;
            bool find = true;

            while(true)
            {
                pos--;

                
                if (!char.IsWhiteSpace((char) sci.CharAt(pos)))
                {
                    pos++;
                    break;
                }
                
            }


            for (int i = 0; i < 3; i++)
            {
                pos--;

                if (sci.CharAt(pos) != newChars[i])
                {
                    find = false;
                    break;
                }
            }



            return find;
        }

        private static bool IsFunctionParameter(ScintillaControl sender, int position)
        {
            char c;
            int openBrace = 0;
            int commasCount = 0;

            ASResult result = null;
            do
            {
                c = (char)sender.CharAt(position);

                if (c == ';' || c == '{' || c == '}') break;
                if (c == '(')
                {
                    openBrace--;
                    if (openBrace == -1)
                    {
                        result = ASComplete.GetExpressionType(sender, position);
                        break;
                    }
                }
                else if (c == ')')
                {
                    openBrace++;
                }
                else if (c == ',')
                {

                    int stylemask = (1 << sender.StyleBits) - 1;

                    bool isTextStyle = ASComplete.IsTextStyle(sender.StyleAt(position) & stylemask);
                    //int style = sender.StyleAt(position - 1) & stylemask;
                    if (isTextStyle)
                        if (openBrace == 0) commasCount++;
                }

                position--;
            }
            while (true);
            //while (c != ';' && c != '{' && c != '}');
         



            if (result == null) return false;
            if (result.Member == null) return false;

            MemberModel md = result.Member;

            if (md.Parameters.Count <= commasCount) return false;
            if (md.Parameters==null || md.Parameters.Count == 0) return false;

            
            if (md.Parameters[commasCount].Type=="Function")
            {
                return true;
            }

            return false;
        }

        private static int SearchNextNewLineWithoutChar(ScintillaControl sender, int position, string text, ref char firstChar)
        {
            char c;
            int safe = sender.MBSafeCharPosition(position);
            int search = safe + text.Length;
           // int pos = 0;
            bool findNewLineBefore = false;
            do
            {

                c = sender.Text[search];
                if(!char.IsWhiteSpace(c)) break;

                if (c == '\n' || c=='\r') findNewLineBefore = true;

                search++;
            } while (true);

            firstChar = c;

            if (findNewLineBefore)
            {
                return 1;
            }

            
            return 0;
        }
            
       private void activateEvent()
       {
           EventManager.AddEventHandler(this, EventType.FileOpen);
           EventManager.AddEventHandler(this, EventType.FileNew);
           EventManager.AddEventHandler(this, EventType.FileEmpty);
           EventManager.AddEventHandler(this, EventType.FileTemplate);

          if( _isEnableFunctionClose)
              EventManager.AddEventHandler(this, EventType.Command);
       }
        /// <summary>
        /// Monitor all document open
        /// </summary>
       public void InsertAllDocument()
       {
           ITabbedDocument[] itab = PluginBase.MainForm.Documents;
        
           for (int i = 0; i <  itab.Length; i++)
           {
               //InsertTabbedDocument((TabbedDocument)itab[i]);
               
               InsertTabbedDocument((TabbedDocument)itab[i]);
               

           }
       }


       //public void InsertTabbedDocument(TabbedDocument tab)
       //{
       //    if (!dict.ContainsKey(tab.Handle))
       //    {
       //        MonitorSci ms = new MonitorSci(tab);
       //        ms.Close += new close(ms_Close);
       //        dict.Add(tab.Handle, ms);

       //    }

       //}

       public void InsertTabbedDocument(TabbedDocument tab)
       {
           if (!dict.ContainsKey(tab.Handle))
           {


               if (tab.SciControl != null)
               {
                   dict.Add(tab.Handle, tab);

                   tab.FormClosed += new System.Windows.Forms.FormClosedEventHandler(tab_FormClosed);
                   tab.SciControl.CharAdded += new CharAddedHandler(SciControl_CharAdded);

                   tab.SciControl.BeforeDelete += new BeforeDeleteHandler(SciControl_BeforeDelete);

                   tab.SciControl.ModEventMask |= (Int32)ScintillaNet.Enums.ModificationFlags.BeforeDelete; ;


                   //   _tabDoc.SciControl.ModEventMask ^= (Int32)ScintillaNet.Enums.ModificationFlags.BeforeDelete; ;
                   HanldeRemoveBracket = new delegateRemoveBracket(RemoveBracket);
               }


           }

       }

       void SciControl_BeforeDelete(ScintillaControl sender, int position, int length)
       {
           int c = sender.CharAt(position);
          // char c = sender.Text[position];
           
           if (c == bracketOpen)
           {
               if (sender.CharAt(position + length) != bracketClose)
               {
                   return;
               }
           }
           else if (c == squareBracketOpen)
           {
               if (sender.CharAt(position + length) != squareBracketClose)
               {
                   return;
               }
           }
           else if (c == quote)
           {
               if (sender.CharAt(position + length) != quote)
               {
                   return;
               }
           }
           else
               return;


           sender.BeginInvoke(HanldeRemoveBracket, new object[] { sender });
       }


       unsafe void SciControl_CharAdded(ScintillaControl sender, int ch)
       {

           if (ch == bracketOpen)
           {
               insert = arrBracketClose;

           }
           else if (ch == quote)
           {

               insert = arrQuote;
           }
           else if (ch == squareBracketOpen)
           {

               insert = arrSquareBracketClose;

           }
           else if (ch == bracketClose)
           {
               if (sender.CharAt(sender.CurrentPos) == bracketClose)
               {
                   // temporary disable
                   sender.ModEventMask ^= (Int32)ScintillaNet.Enums.ModificationFlags.BeforeDelete;
                   sender.CharRight();
                   sender.DeleteBack();
                   sender.ModEventMask ^= (Int32)ScintillaNet.Enums.ModificationFlags.BeforeDelete;
                   return;
               }
               return;
           }
           else if (ch == squareBracketClose)
           {
               if (sender.CharAt(sender.CurrentPos) == squareBracketClose)
               {
                   // temporary disable
                   sender.ModEventMask ^= (Int32)ScintillaNet.Enums.ModificationFlags.BeforeDelete;
                   sender.CharRight();
                   sender.DeleteBack();
                   sender.ModEventMask ^= (Int32)ScintillaNet.Enums.ModificationFlags.BeforeDelete;
                   return;
               }
               return;
           }
           else
           {
               return;
           }

           int pos = sender.CurrentPos;
           uint actPos = (uint)pos;

           
           
           char character;
           do
           {
               character =(char) sender.CharAt(pos);
               if (character == '\n') break;
               if (!Char.IsWhiteSpace(character)) break;
               pos++;


           } while (true);

         //  if (Char.IsLetterOrDigit(character) || character == ch || character == insert[0]) return;
           if (Char.IsLetterOrDigit(character) || character == ch ) return;



           int stylemask = (1 << sender.StyleBits) - 1;

           bool isTextStyle = ASComplete.IsTextStyle(sender.StyleAt(sender.CurrentPos) & stylemask);
           int style = sender.StyleAt(sender.CurrentPos - 1) & stylemask;


           if (ch == quote)
           {
               if (!isTextStyle)
               {

                   fixed (byte* b = System.Text.Encoding.GetEncoding(sender.CodePage).GetBytes(insert))
                   {
                       sender.SPerform(2003, actPos, (uint)b);
                   }

               }

               return;
           }

           if (!ASComplete.IsTextStyle(style) && !isTextStyle)
           {

               return;
           }

           fixed (byte* b = System.Text.Encoding.GetEncoding(sender.CodePage).GetBytes(insert))
           {
               sender.SPerform(2003, actPos, (uint)b);
           }

       }



       public void RemoveBracket(ScintillaControl sci)
       {
           // temporary deactive
           sci.ModEventMask ^= (Int32)ScintillaNet.Enums.ModificationFlags.BeforeDelete;
           sci.CharRight();
           sci.DeleteBack();
           sci.ModEventMask |= (Int32)ScintillaNet.Enums.ModificationFlags.BeforeDelete;
       }


       void tab_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
       {
             TabbedDocument _tabDoc = (TabbedDocument)sender;
           
               _tabDoc.SciControl.CharAdded -= new CharAddedHandler(SciControl_CharAdded);
               _tabDoc.SciControl.BeforeDelete -= new BeforeDeleteHandler(SciControl_BeforeDelete);

               _tabDoc.SciControl.ModEventMask ^= (Int32)ScintillaNet.Enums.ModificationFlags.BeforeDelete; ;

               dict.Remove(_tabDoc.Handle);
               //   _tabDoc.SciControl.ModEventMask ^= (Int32)ScintillaNet.Enums.ModificationFlags.BeforeDelete; ;
          
       }

     

        #region IEventHandler Membri di

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            

            switch (e.Type)
            {
                case EventType.FileOpen:
                case EventType.FileNew:
                case EventType.FileTemplate:
                case EventType.FileEmpty:
                case EventType.FileOpening:
                    EventManager.RemoveEventHandler(this);

                    EventManager.AddEventHandler(this, EventType.FileSwitch);
                break;
                case EventType.FileSwitch:
                TabbedDocument tb = (TabbedDocument)PluginBase.MainForm.CurrentDocument;
                InsertTabbedDocument(tb);
                EventManager.RemoveEventHandler(this);
                activateEvent();
                break;
                case EventType.Command:
                    currentNotifyEvent = e;  
                break;
                
            }
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            EventManager.RemoveEventHandler(this);

            disableFunctionAndNewClose();

            foreach (KeyValuePair<IntPtr, TabbedDocument> item in dict)
            {
              
                item.Value.SciControl.CharAdded -= new CharAddedHandler(SciControl_CharAdded);
                item.Value.SciControl.BeforeDelete -= new BeforeDeleteHandler(SciControl_BeforeDelete);

                item.Value.SciControl.ModEventMask ^= (Int32)ScintillaNet.Enums.ModificationFlags.BeforeDelete; ;
            }


            dict.Clear();

            abbreviations = null;
           
        }

        #endregion

    }
}
