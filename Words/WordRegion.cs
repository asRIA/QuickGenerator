using System;

namespace QuickGenerator.Words
{
	class WordRegion : WordRegionBase, IDisposable
	{

		public WordRegion()
		{
			type = kind.cursor;
			//indList = -1;
		}

		public override void addCharactersToRegion(int length)
		{
			endWord += length;

			// if (NextWord != null)
			NextWord.addCharactersNextWord(length);
		}

		public override void removeCharactersFromRegion(int length)
		{
			endWord -= length;

			NextWord.removeCharactersNextWord(length);

		}

		public override void removeCharactersNextWord(int length)
		{

			startWord -= length;
			endWord -= length;
			//  if (NextWord != null)
			NextWord.removeCharactersNextWord(length);

		}

		public override void addCharactersNextWord(int length)
		{

			startWord += length;
			endWord += length;
			//  if (NextWord != null)
			NextWord.addCharactersNextWord(length);

		}


	}
}
