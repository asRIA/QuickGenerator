
namespace QuickGenerator.Words
{
    class LastWordRegionParameter : WordRegionParameter
    {
        public override void addCharactersNextWord(int length)
        {
            startWord += length;
            endWord += length;
        }

        public override void addCharactersToRegion(int length)
        {
            endWord += length;
        }

        public override void removeCharactersFromRegion(int length)
        {
            endWord -= length;

        }

        public override void removeCharactersNextWord(int length)
        {
            startWord -= length;
            endWord -= length;
        }
    }
}
