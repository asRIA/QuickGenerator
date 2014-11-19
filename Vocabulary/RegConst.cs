using QuickGenerator.Abbreviation;

namespace QuickGenerator.Vocabulary
{
    class RegConst : IMatch
    {
        InfoArguments info;
      
        private string test;

        public RegConst(string text, WordTypes regplace, bool hasNeedSpace)
        {
            info = new InfoArguments();
            info.regplace = regplace;
            info.hasNeedSpace = hasNeedSpace;
            test = text;
        }




        #region IMatch Membri di

        public InfoArguments Match(string input, int startPos)
        {
            if (test == input)
            {
                return info;
            }

            return null;
        }




        public InfoArguments IsMatch(string input, int startPos)
        {
            if (test == input)
            {
                return info;
            }

            return null;
        }

        #endregion
    }
}
