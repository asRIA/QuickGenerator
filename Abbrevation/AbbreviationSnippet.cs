using System;

namespace QuickGenerator.Abbreviation
{
    [Flags]
    public enum WordTypes  
    {
        None ,
        SafeZone ,
        Clipboard ,
        var ,
        mbrname ,
        browser ,
        list ,
        cmp ,
        place,
        import, 
        EventHandler,
        createParameters,
        AfterCurrentMember,
        
        
        
    }

    [Serializable]
    public class AbbrevationSnippet
    {
        public string Snippet;
        public WordTypes[] Arguments;
        public bool HasImport;
        public bool HasAfterCurrentMember;
        public bool HasEventHandler;

        public AbbrevationSnippet(string Snippet)
        {
            this.Snippet = Snippet;
        }
       

    }
}
