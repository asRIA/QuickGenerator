using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace QuickGenerator.Reformatter
{
    class InsertCode :IFormater
    {
        #region IFormater Membri di

        protected List<string> WordsToSubstitute;
        List<string> Parameters;

        protected int textLength;

        private bool isAbbreviationAndReformat;

        public InsertCode(MatchCollection mc, bool isAbbreviationAndReformat)
        {
            WordsToSubstitute = new List<string>();

            this.isAbbreviationAndReformat = isAbbreviationAndReformat;

            foreach (Match item in mc)
            {
                textLength += item.Value.Length;
                WordsToSubstitute.Add(item.Value);
            }
           
        }

        public InsertCode()
        {
        }


        public string ReformatString(string originalText)
        {
            
            Regex rg = new Regex("@\\d?");

            
             MatchCollection mtc =  rg.Matches(originalText);
             int length = mtc.Count;
             if (length == 0) return originalText;


             Parameters = new List<string>();
             Match param;

             for (int i = 0; i < length; i++)
			{
                param = mtc[i];
                if (!Parameters.Contains(param.Value))
                 {

                     Parameters.Add(param.Value);

                 }
             }


             int countParam = Parameters.Count;
             int countListWord = WordsToSubstitute.Count;
             int pos = 0;
             int initPos = 0;
             int indexWord = 0;
             Dictionary<string, string> newInsert;

             int numberLines = countListWord / countParam;
             string nl = PluginCore.Utilities.LineEndDetector.GetNewLineMarker(ASCompletion.Context.ASContext.CurSciControl.EOLMode);
             StringBuilder sbNewString = new StringBuilder((originalText.Length + (textLength * countParam)) * numberLines);

            
             if (numberLines == 0) numberLines = 1;


             
            
             for (int indexLine = 0; indexLine < numberLines; indexLine++)
             {
                 sbNewString.Append(originalText);

                 newInsert = new Dictionary<string, string>(countParam);

                 for (int indexPar = 0; indexPar < Parameters.Count; indexPar++)
                 {
                     
                     newInsert.Add(Parameters[indexPar],WordsToSubstitute[indexWord]);

                     indexWord++;

                     if (countListWord == indexWord)
                     {
                         break;
                     }
                 }


                 for (int j = length - 1; j >= 0; j--)
                 {


                     param = mtc[j];
                     pos = initPos + param.Index;
                     string insertString = null;

                     newInsert.TryGetValue(param.Value, out insertString);

                     if (insertString == null) continue;
                     sbNewString.Remove(pos, param.Length);
                     sbNewString.Insert(pos, insertString);

                     if (isAbbreviationAndReformat)
                     {
                         int start = pos -1;
                         if(start<0) start =0;
                         sbNewString.Replace("${", "{", start, insertString.Length + 1);
                     }
                     
                 }



                 sbNewString.Append(nl);

                 initPos = sbNewString.Length;

             }

             sbNewString.Remove(sbNewString.Length - nl.Length, nl.Length);

             return sbNewString.ToString();

        }

        #endregion
    }


}
