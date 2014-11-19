
//This class encapsulates the data of a change word, keep tracking of its length, actual position and of the next word through <Nextword>.
//Each time that the user insert a new character, CreateWords recall the function <AddCharactersToRegion> of the specific word, changing in the meanwhile the length and notify the changes to the next word <addCharactersNextWord>.
//This one after been notified, will change its position and pass the information about the change to the next word.
//The notify process will continue until the last word will be reached <LastWordRegion.cs>.
//The same rule also applies for the elimination <removeCharacterFromRegion>.

//LastWord identify the last changeable word in the text and it has not any word to pass the notify, so its implement is a bit different.
//I created it for avoid to do checks ad each changes on the existence of the next word.
//If the user want to avoid to use this class, he will have to write each checks necessary for the insert or delete of the text.

//VarWordRegion is the class that represent the word that when changed, it reflects its change to the other words linked to it and it follow a different behavior.


///-------------------------  ITALIAN -------------------------------
/*
 * Questa classe incapsula i dati di una parola modificabile,tenendo traccia della sua lunghezza, posizione e della parola succesiva ad essa<NextWord>.
 * Ogni volta che viene inserita una lettera CreateWords  richiama la funzione <addCharactersToRegion> della parola interessata
 * che nè modificherà la lunghezza notificando la parola successiva  di questo cambiamento <addCharactersNextWord>.
 * Quest'ultima, dopo essere stata notificata, cambierà a sua volta la sua posizione e notificherà la parola presente dopo di essa.
 * Il procedimento di notifica continuerà così fino all'arrivo dell'ultima parola <LastWordRegion.cs>.
 * Ciò vale anche per l'eliminazione <removeCharactersFromRegion>.
 * 
 * LastWord rappresenta l'ultima parola modificabile del testo e non ha nessuna altra parola da notificare quindi la sua implementazione è differente.
 * L'avevo creata per evitare di fare controlli ad ogni cambiamento dell'esistenza della successiva parola.
 * Se si vuole evitare di usare questa classe si deve scrivere ogni tipo di controllo necessario nel inserimento o cancellazione del testo.
 * 
 * VarWordRegion è la classe che rappresenta la parola che modificata riflette il suo cambiamento alle altre parole ad essa legata
 * e si comporta in modo differente.
 * 
*/
using System;

namespace QuickGenerator.Words
{
    abstract public class WordRegionBase : IDisposable
    {
        public  int startWord;
        public  int endWord;
        public WordRegionBase NextWord;
        

        public abstract void addCharactersNextWord(int length);
        public abstract void removeCharactersNextWord(int length);
        public abstract void removeCharactersFromRegion(int length);
        public abstract void addCharactersToRegion(int length);

        

        public enum kind
        {
            cursor,
            browser,
            customList,
            showCompType,
            temporary,
            VarLink,
            place,
            createParameters,
            VarLinkAndComp,
            Import,
            Parameter
        }


        public kind type;


        public virtual WordRegionBase getLastWord()
        {
            LastWordRegion wr = new LastWordRegion();

            wr.startWord = this.startWord;
            wr.endWord = this.endWord;
            wr.type = this.type;
            return wr;
        }


        public virtual void Disable()
        {

        }

        #region IDisposable Membri di

        public void Dispose()
        {
            NextWord = null;
        }


       

        #endregion
    }
}
