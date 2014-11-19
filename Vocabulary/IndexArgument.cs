
namespace QuickGenerator.Vocabulary
{
    class IndexArgument
    {
  

        public char ch;


        public IndexArgument siblingCharacter;
        public IndexArgument nextCharacter;
        IMatch _test;
        public  IndexArgument(IMatch test)
        {
            _test = test;
            
        }

        public IMatch imatch
        {
            get { return _test; }
        }


    
    }
}
