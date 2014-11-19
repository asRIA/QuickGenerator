using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ASCompletion.Context;
using ScintillaNet;
namespace QuickGenerator.Reformatter
{
    class ReformatterCode : IPressKey,IDisposable
    {
        public List<IFormater> lstFormater;
        QuickGenerator.Settings settings;
        private bool _forAbbreviations;
        public ReformatterCode(QuickGenerator.Settings settings)
        {
            this.settings = settings;
        }


        public string[] DivideStringCode(string text)
        {

            if (text.Trim().Length == 0) return null;
            string[] strings=null;

            int ind = text.LastIndexOf(";");
            if (ind != -1)
            {

                strings = new string[2];
                strings.SetValue(text.Substring(0, ind), 0);
                int ind2 = ind + 1;
                strings.SetValue(text.Substring(ind2, text.Length - ind2), 1);

            }
            else
                return strings;

            if (settings.BreakInLines)
            {
                if (strings != null)
                {

                    ScintillaControl sci = ASContext.CurSciControl;

                    int curLine = sci.LineFromPosition(sci.CurrentPos);
                    string tabString = new string('\t', sci.GetLineIndentation(curLine) / sci.Indent);
                   // string nl = ";" + ASCompletion.Completion.ASComplete.GetNewLineMarker(sci.EOLMode) + tabString;
                    string nl = ";" + PluginCore.Utilities.LineEndDetector.GetNewLineMarker(sci.EOLMode) + tabString;
                    strings[0] = strings[0].Replace(";", nl);
                }
            }
            return strings;
        }

        public bool  SearchForOperators(string textToAnalyze, bool forAbbreviations)
        {
            lstFormater = new List<IFormater>();
            _forAbbreviations = forAbbreviations;
            //(?:[*<])[^*<]+
            Regex rg = new Regex("[*<][^*<]+");
             MatchCollection mtc =  rg.Matches(textToAnalyze);

             if (mtc.Count == 0) return false;
             foreach (Match item in mtc)
             {
              IFormater f = null;
                
                 if (item.Value[0] == '*')
                 {
                  f  =  MoltiplyReformater(item.Value);
                     
                 }else if(item.Value[0] == '<')
                 {

                    f = InsertReformater(item.Value);
                 }

                 if (f != null)
                 {
                     lstFormater.Add(f);
                 }
             }

             return true;
        }


        public string Reformat(string str)
        {
            foreach (IFormater  item in lstFormater)
            {
              str =  item.ReformatString(str);
            }

            return str;
        }

        public IFormater InsertReformater(string text)
        {
            Regex rg = new Regex("[^\\s,]+");
            MatchCollection mc = rg.Matches(text,1);

            if (mc.Count == 0 )
            {
                return null;
            }
            else
            {


                return new InsertCode(mc, _forAbbreviations );
                //if(!_forAbbreviations)
                //    return new InsertCode(mc);
                //else
                //    return new InsertCodeWithoutArguments(mc);
            }
        }


        public  IFormater MoltiplyReformater(string text)
        {
            Regex rg = new Regex("\\d{1,2}");
            Match mc =  rg.Match(text);

            if (mc.Value.Length==0)
            {
                return null;
            }
            else
            {
                return new MoltiplyCode(int.Parse(mc.Value),settings.BaseNumber);
            }
        }






        #region IPressKey Membri di




        public void EventKey(PluginCore.KeyEvent k)
        {

                ScintillaNet.ScintillaControl currentSci = ASContext.CurSciControl;
                int linePos = currentSci.LineFromPosition(currentSci.CurrentPos);
                string line = currentSci.GetLine(linePos);
                string[] str = DivideStringCode(line);

                if (str == null)
                {
                   // MessageBox.Show("non è stato diviso niente");
                    return;
                }
                else
                {
                    if (SearchForOperators(str[1], false))
                    {
                       string newString =  Reformat(str[0] + ";");
                       currentSci.GotoPos(currentSci.PositionFromLine(linePos));
                       currentSci.BeginUndoAction();
                       currentSci.DelLineRight();
                       currentSci.InsertText(currentSci.CurrentPos, newString);
                       currentSci.EndUndoAction();
                       currentSci.GotoPos(currentSci.CurrentPos + newString.Length);
                       
                    }
                }
            
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            settings = null;
        }

        #endregion
    }


  
}
