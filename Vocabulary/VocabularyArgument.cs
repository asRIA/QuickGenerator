//It contains the Regular Expression used in the abbreviations.
//I don’t want to test every argument for all the possible regular expression, so I create this class that works like an index.
//When an argument is evaluated, this class use the first characters to obtain the regular expression to use.
//For the arguments that exist now, it works fine. In future it should be better write and tested.

//Note: Originally I created this class for improve a possibility given by the reformat code, but in this version of the plug-in I don’t already insert it.

// --------------------- ITALIAN ----------------------------------
/*
 * Contiene le Regular Expression usate nelle abbreviazioni.
 * Dato che nella creazione delle abbreviazioni in AbbrevationCompletionForm.cs, non voglio testare ogni argomento per tutte le possibili
 * regular expression (sopratutto se queste dovessero aumentare), ho fatto in modo che questa classe funzioni come una specie di indice.
 * Quando viene valutato un argomento questa classe utilizza le prime lettere per ricavarne la regular expression da usare.
 * Per gli argomenti che ci sono attualmente funziona, in futuro dovrebbe essere modificato e testato di più per sicurezza.
 * 
 * Nota: avevo creato questa classe anche per ampliare una altra possibilità data da reformat code,
 *      ma in questa versione del plugin non l'ho ancora inserita.
 */
using System.Text.RegularExpressions;
using QuickGenerator.Abbreviation;
using System.Collections.Generic;

namespace QuickGenerator.Vocabulary
{
    public  class VocabularyArgument
    {

        IndexArgument firstWord;

        public const string SafeZone = "${SafeZone}";
        public const string CLIPBOARD = "${clipboard}";
        public const string MbrName = "${MbrName}";
        public const string BROWSER = "${browser}";
        public const string CREATEPARAMATERS = "${createParameters}";


        public readonly Regex regVar;
        public readonly Regex regList;
        public readonly Regex regPlace;
        public readonly Regex regCompType;
        public readonly Regex regImport;
        public readonly Regex regEventHandler;
        public readonly Regex regAfterCurrentMember;
        public readonly Regex regArguments;

        public List<string> ListArguments;
        public List<string> ListDescriptionArguments; 
        public VocabularyArgument()
        {
            ListArguments = new List<string>(10);
            ListDescriptionArguments = new List<string>(10);

            ListArguments.Add("${SafeZone}");
            ListDescriptionArguments.Add("Only in this word is possible expand another abbreviation!!");
            ListArguments.Add("${var=\"\"}");
            ListDescriptionArguments.Add("Inside the bracket you write the name of the variable.\nIf you will insert more variables with the same name and you want to change the name of all of them,\n you just need to change the name of the first variable");
            ListArguments.Add("${var=\"\" showCmp}");
            ListDescriptionArguments.Add("It's as ${var=\"\"} only that it show a Completion List too");
            ListArguments.Add("${var=\"\" list=\"\"}");
            ListDescriptionArguments.Add("It's as ${var=\"\"} only that it show a Custom List too");
            ListArguments.Add("${clipboard}");
            ListDescriptionArguments.Add("Insert the text inside the clipboard");
            ListArguments.Add("${MbrName}");
            ListDescriptionArguments.Add("Insert Members's type name");
            ListArguments.Add("${browser}");
            ListDescriptionArguments.Add("Show a window for selecting file.");
            ListArguments.Add("${list=\"\"}");
            ListDescriptionArguments.Add("Show a custom completion list indicate by a specific name.\nFor create the List press 'Show Custom List'");
            ListArguments.Add("${cmp=\"\"}");
            ListDescriptionArguments.Add("It tries to show a completion list with a list of tips.\nIt use the character one the left as reference\nInside the brakets you can to insert the default value.");
            ListArguments.Add("${\"\"}");
            ListDescriptionArguments.Add("Inside the brackets there is the value that will be show.");
            ListArguments.Add("${Import=\"\"}");
            ListDescriptionArguments.Add("It tries to create the import for the text inside the brackets.");
            ListArguments.Add("${EventHandler=\"\"}");
            ListDescriptionArguments.Add("It tries to create a Event function with the text inside the brackets.\nThe word must be inside a Member at the moment of the creation");
            ListArguments.Add("${createParameters}");
            ListDescriptionArguments.Add("It tries to generate paramaters inside a function or new type\nDon't create paramaters if there of the text inside () ");
            ListArguments.Add("${AfterCurrentMember=\"\"}");
            ListDescriptionArguments.Add("It generate another abbreviation after current member if you are inside it.\n Inside the brackets there is the name of the abbreviation.");


            regVar = new Regex("(?<=var=\")[a-zA-Z0-9_]+(?=\")");
            regList = new Regex("(?<=list=\")[a-zA-Z0-9_]+(?=\")");
            regPlace = new Regex("(?<={\")[a-zA-Z0-9_.]*(?=\")");
            regCompType = new Regex("(?<=cmp=\")[@a-zA-Z0-9_]*(?=\")");
            regImport = new Regex("(?<=Import=\")[a-zA-Z0-9_.]+(?=\")");
            regEventHandler = new Regex("(?<=EventHandler=\")[a-zA-Z0-9_.]+(?=\")");
            regAfterCurrentMember = new Regex("(?<=AfterCurrentMember=\")[@a-zA-Z0-9_]+(?=\")");
            
            regArguments = new Regex(@"\${[^}]*}", RegexOptions.CultureInvariant & RegexOptions.ECMAScript);

            IMatch match = new RegTest(regVar, WordTypes.var, true);
            InsertWord("var=", match);
            match = new RegTest(regList, WordTypes.list, true);
            InsertWord("list=", match);
            match = new RegTest(regPlace, WordTypes.place, true);
            InsertWord("\"\"", match);
            match = new RegTest(regCompType, WordTypes.cmp, true);
            InsertWord("cmp=", match);
            match = new RegTest(regImport, WordTypes.import, false);
            InsertWord("Import=", match);
            match = new RegTest(regEventHandler, WordTypes.EventHandler, false);
            InsertWord("EventHandler=", match);
            match = new RegTest(regAfterCurrentMember, WordTypes.AfterCurrentMember, false);
            InsertWord("AfterCurrentMember=", match);

            match = new RegConst(SafeZone, WordTypes.SafeZone, true);
            InsertWord("SafeZone", match);

            match = new RegConst(CLIPBOARD, WordTypes.Clipboard, false);
            InsertWord("clipboard", match);

            match = new RegConst(MbrName, WordTypes.mbrname, false);
            InsertWord("MbrName", match);


            match = new RegConst(BROWSER, WordTypes.browser, true);
            InsertWord("browser", match);

            match = new RegConst(CREATEPARAMATERS, WordTypes.createParameters, true);
            InsertWord("createParameters", match);

        }



        public void InsertWord(string word, IMatch test)
        {
          
            char[] text = word.ToCharArray();

            if (text.Length == 0) return;

            IndexArgument wordArg = new IndexArgument(test);



            if (firstWord == null)
            {
                wordArg.ch = text[0];
                firstWord = wordArg;
                return;
            }




            IndexArgument wa = firstWord;



            int val = (int)text[0];
            int val2 = (int)firstWord.ch;

            if (val < val2)
            {
                wordArg.ch = text[0];
                wordArg.nextCharacter = firstWord;
                firstWord = wordArg;
                return;
            }

            int indexChar = 0;
            int valToIns = (int)text[indexChar];

            while (wa != null)
            {
                int currentVal = (int)wa.ch;
                // lettera succesiva
                if (valToIns > currentVal)
                {
                    // non ha caratteri quindi inserisco
                    if (wa.nextCharacter == null)
                    {
                        wordArg.ch = text[indexChar];
                        wa.nextCharacter = wordArg;
                        return;
                    }
                    else
                    {
                        // ci sono altri caratteri continua la mia ricerca
                        int value = (int)wa.nextCharacter.ch;

                        // la lettera successiva è maggiore di quella attuale quindi
                        // deve essere posizionata prima
                        if (valToIns < value)
                        {
                            wordArg.ch = text[indexChar];
                            wordArg.nextCharacter = wa.nextCharacter;
                            wa.nextCharacter = wordArg;
                            return;
                        }
                        else
                        {
                            // non essendo minore ritesto la lettera
                            wa = wa.nextCharacter;
                        }

                    }

                }
                else
                {
                    // le lettere hanno lo stesso valore
                    // quindi avanzo nel testo
                    indexChar++;

                    // non c'è più testo la parola è già usata
                    if (indexChar == text.Length)
                        return;

                    int siblingChar = (int)text[indexChar];

                    if (wa.siblingCharacter == null)
                    {
                        wordArg.ch = text[indexChar];
                        wa.siblingCharacter = wordArg;
                        return;
                    }

                    currentVal = (int)wa.siblingCharacter.ch;
                    if (siblingChar < currentVal)
                    {
                        wordArg.ch = text[indexChar];

                        wordArg.nextCharacter = wa.siblingCharacter;
                        wa.siblingCharacter = wordArg;

                        return;
                    }

                    valToIns = siblingChar;
                    wa = wa.siblingCharacter;
                }
            }

            //  wa.TestChar(wordArg, 0, text);


        }

        public IMatch SearchInfoByText(string textSearch, int startIndex)
        {
            if (firstWord == null) return null;

            char[] textArr = textSearch.ToCharArray();

            if (textArr.Length == 0) return null;

            int indexChar = startIndex;

            if (indexChar < 0) indexChar = 0;

            int valToSearch = (int)textArr[indexChar];

            IndexArgument ia = firstWord;
            IndexArgument iaFind = null;


            while (ia != null)
            {

                int value = (int)ia.ch;

                //stessa lettera
                if (value == valToSearch)
                {
                    indexChar++; //avanzo nel testo

                    // non ho più testo la ricerca è finita
                    if (indexChar == textArr.Length)
                        return ia.imatch;

                    // c'è ancora testo si continua
                    valToSearch = (int)textArr[indexChar];

                    // non ci sono più lettere da consultare
                    if (ia.siblingCharacter == null)
                        return ia.imatch;


                    iaFind = ia;
                    ia = ia.siblingCharacter;

                }
                else
                {
                    // non è la stessa lettera
                    //provo con quella sottostante


                    ia = ia.nextCharacter;

                }


            }


            if (iaFind != null)
                return iaFind.imatch;


            return null;

        }


 

    }
}
