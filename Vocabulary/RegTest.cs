using System.Text.RegularExpressions;
using QuickGenerator.Abbreviation;

namespace QuickGenerator.Vocabulary
{
    public class RegTest:IMatch
    {
        Regex reg;
        InfoArguments info;


        public RegTest(Regex reg, WordTypes regplace,bool hasNeedSpace)
        {
            this.reg = reg;

            info = new InfoArguments();
            info.regplace = regplace;
            info.hasNeedSpace = hasNeedSpace;
        }




        #region IMatch Membri di

        public InfoArguments Match(string input, int startPos)
        {
           // string go = input.Substring(2, input.Length-2);
            Match mc = reg.Match(input, startPos);

            if (mc.Success)
            {

                info.index = mc.Index;
                info.length = mc.Length;
                return info;
            }

            return null;
        }


        #endregion

        #region IMatch Membri di


        public InfoArguments IsMatch(string input, int startPos)
        {


            if (reg.IsMatch(input, startPos))
            {

                return info;
            }

            return null;
        }

        #endregion
    }
}
