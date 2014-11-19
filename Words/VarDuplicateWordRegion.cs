using System;


namespace QuickGenerator.Words
{
    class VarDuplicateWordRegion : WordRegion ,IDisposable
    {

        public VarWordRegion rootWord;
        public VarDuplicateWordRegion NextVarDuplicate;


        public override void addCharactersNextWord(int length)
        {

            int newEndWord = length + rootWord.newLength;
            startWord += length;
            endWord += newEndWord;


            NextWord.addCharactersNextWord(newEndWord);
            
        }


        public override void removeCharactersNextWord(int length)
        {
 

            int newEndWord = length + rootWord.newLength;
            startWord -= length;
            endWord -= newEndWord;


            NextWord.removeCharactersNextWord(newEndWord);

        }


        public override WordRegionBase getLastWord()
        {
            LastWordVarDuplicate wr = new LastWordVarDuplicate();

            wr.startWord = startWord;
            wr.endWord = endWord;
            wr.type = type;
            
            return wr;
        }

        public override void addCharactersToRegion(int length)
        {
            rootWord.VarNotify = this;
            rootWord.addCharactersToRegion(length);

        }

        public override void removeCharactersFromRegion(int length)
        {
            rootWord.VarNotify = this;
            rootWord.removeCharactersFromRegion(length);

        }

        public override void Disable()
        {
            rootWord.NextVarDuplicate = null;
        }


        #region IDisposable Membri di

        void IDisposable.Dispose()
        {
            NextWord = null;
            rootWord = null;
        }

        #endregion

        
    }
}
