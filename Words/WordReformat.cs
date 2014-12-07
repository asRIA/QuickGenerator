using System;

namespace QuickGenerator.Words
{

	/// <summary>
	/// Word for when type key ";"
	/// </summary>
	class WordReformat : WordRegionBase
	{

		public WordRegionBase word;

		public override void addCharactersNextWord(int length)
		{
			word.endWord += length;

			if (word.NextWord != null)
				word.NextWord.addCharactersNextWord(length);
		}

		public override void removeCharactersNextWord(int length)
		{
			throw new NotImplementedException();
		}

		public void shiftWord(int length)
		{

			word.addCharactersNextWord(length);
		}


		public override void removeCharactersFromRegion(int length)
		{
			throw new NotImplementedException();
		}

		public override void addCharactersToRegion(int length)
		{
			throw new NotImplementedException();
		}
	}


}
