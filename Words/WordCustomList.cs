
namespace QuickGenerator.Words
{
	class WordCustomList : WordRegion
	{
		public string indList;


		public override WordRegionBase getLastWord()
		{

			LastWordCustomList wb = new LastWordCustomList();
			wb.indList = this.indList;
			wb.type = this.type;
			wb.endWord = this.endWord;
			wb.startWord = this.startWord;

			return wb;
		}
	}
}
