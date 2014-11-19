using System.Text;
using System.Text.RegularExpressions;

namespace QuickGenerator.Reformatter
{
    class MoltiplyCode : IFormater
    {
        #region IFormater Membri di
        int numberDuplicate;
        int baseNumber;
        static Regex rg = new Regex(@"§(?:\[-?\d+\])?");

        public MoltiplyCode(int n, int baseNumber)
        {
            if (n <= 0) n = 1;
            this.numberDuplicate = n;
            this.baseNumber = baseNumber;
        }

        public string ReformatString(string OriginalText)
        {
            Regex rg = MoltiplyCode.rg;   //new Regex(@"§(?:\[-?\d+\])?");
            MatchCollection mtc = rg.Matches(OriginalText);

            int countPlace = mtc.Count;

            int[] IntInd = new int[countPlace];
            Match place;
            int i = -1;
            int lengthString=0;
           // First retrieve value
            for (i = 0; i < countPlace; i++)
            {
                
                place = mtc[i];
                if (place.Value.Length == 1)
                {

                    IntInd[i] = baseNumber;
                    lengthString += 2;
                }
                else
                {
                    string valueInt = place.Value.Substring(2, place.Value.Length - 3);
                    lengthString += valueInt.Length + 1;
                    IntInd[i] = int.Parse(valueInt);
                }

             
            }




            string nl = ASCompletion.Completion.ASComplete.GetNewLineMarker(ASCompletion.Context.ASContext.CurSciControl.EOLMode);
            StringBuilder sb = new StringBuilder((OriginalText.Length + lengthString +nl.Length) * numberDuplicate);

           

            
            // insert to

            int pos=0;
            int initPos = 0;
            
            for (i = 0; i < numberDuplicate; i++)
            {
                sb.Append(OriginalText);


                
                for (int j = countPlace-1; j >= 0; j--)
                {

                  
                    place = mtc[j];
                    pos = initPos + place.Index;
                    sb.Remove(pos, place.Length);
                    sb.Insert(pos, ++IntInd[j]);
                    
                }


                 sb.Append(nl);

                initPos = sb.Length;
          
            }

            sb.Remove(sb.Length - nl.Length, nl.Length);
            return sb.ToString();

        }

        #endregion
    }
}
