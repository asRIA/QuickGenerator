//The modify of this word will trigger a change in all the other words linked to the original and they are represent inside the class <VarDuplicateWordRegion.cs>.
//When the user changes this word, it will trigger an event managed by the CreateWords through the function <ChangeWordsVar>.
//This function will recalculate the new position of the other words, replacing the text.

//Even the <VarDuplicateWordRegion.cs> can be modify trigging the same effects above listed.
//The <VarDuplicateWordRegion.cs> that will changed, is represent by <VarNotify>.

// ------------------------  ITALIAN --------------------------------
/*
 * La modifica di questa parola comporta anche la modifica di tutte le altre parole a questa legata rappresentate dalla classe <VarDuplicateWordRegion.cs>.
 * Quando questa parola viene modificate scatena un evento che sarà gestito in CreateWords con la funzione <ChangeWordsVar>.
 * Questo funzione ricalcolerà in pratica le nuove posizioni delle altre parole sostituendone il testo.
 * 
 * Anche le <VarDuplicateWordRegion.cs> possono essere modificate provocando gli stessi effeti descritti sopra.
 * La <VarDuplicateWordRegion.cs> che viene modificata è rappresentata da <varNotify>.
*/
using System;

namespace QuickGenerator.Words
{

     
    /// <summary>
    /// Represent a variabile. Modify it will change each varDuplicate associate with this
    /// </summary>
    class VarWordRegion : WordCustomList, IDisposable
    {
      
        public string ValueVar;
        public delegate void ChangeVarEventHandler(VarWordRegion vwr);
        public event ChangeVarEventHandler ChangeVarWord;
        private bool disactive;
        
        private VarDuplicateWordRegion lastVar;
        private VarDuplicateWordRegion secondLast;
        public VarDuplicateWordRegion  NextVarDuplicate;
        public int newLength;
        private int changeLength;
        private VarDuplicateWordRegion varNotify;

        public VarWordRegion()
        {
            
        }

 
        public override void addCharactersToRegion(int length)
        {
            //endWord += length;

            //newLength = length;
            //NextWord.addCharactersNextWord(length);
            //newLength = 0;
            endWord += length;
            changeLength += length;


            if (NextVarDuplicate == null)
                NextWord.addCharactersNextWord(length);
            else
            ChangeVarWord(this);
        }





        public void ChangePositionVarsLink()
        {
           // endWord += changeLength;

            if (changeLength > 0)
            {
                newLength = changeLength;

                NextWord.addCharactersNextWord(changeLength);
            }
            else
            {
                newLength = -changeLength;
                NextWord.removeCharactersNextWord(newLength);
            }
            
            changeLength = 0;
            newLength = 0;
        }



        public override void removeCharactersFromRegion(int length)
        {

            endWord -= length;
            changeLength -= length;

            if (NextVarDuplicate == null)
                NextWord.removeCharactersNextWord(length);
            else
            ChangeVarWord(this);

        }

       

        public override void addCharactersNextWord(int length)
        {
            startWord += length;
            endWord += length;

        
            NextWord.addCharactersNextWord(length);
        }

        public VarDuplicateWordRegion VarNotify
        {
            get { return varNotify; }
            set {

                if (varNotify != null && value != null) return;

                varNotify = value;
            }
        }


        public override void removeCharactersNextWord(int length)
        {

            startWord -= length;
            endWord -= length;

            NextWord.removeCharactersNextWord(length);

        }

        public void InsertVarDuplicate(VarDuplicateWordRegion var)
        {
            var.rootWord = this;
            if (NextVarDuplicate == null)
            {
                NextVarDuplicate = var;
                lastVar = var;
            }
            else
            {
                secondLast = lastVar;
                lastVar.NextVarDuplicate = var;
                lastVar = var;
                
            }


        }

        public int ChangeLength
        {
            get { return changeLength; }
        }

        public void SetLastVar(VarDuplicateWordRegion lastWord)
        {
            lastWord.rootWord = this;


            if (secondLast != null)
                secondLast.NextVarDuplicate = lastWord;
            else
            {
                NextVarDuplicate = lastWord;
                lastVar = lastWord;
            }
        }


        public override void Disable()
        {
            disactive = true;
        }


        public bool Disactive
        {
            get { return disactive; }
        }
        #region IDisposable Membri di

        void IDisposable.Dispose()
        {
            lastVar = null;
            NextWord = null;
            NextVarDuplicate = null;
            VarNotify = null;
            
        }

        #endregion
    }
}
