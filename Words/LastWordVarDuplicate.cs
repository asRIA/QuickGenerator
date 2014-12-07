using System;

namespace QuickGenerator.Words
{
	class LastWordVarDuplicate : VarDuplicateWordRegion, IDisposable
	{



		public LastWordVarDuplicate()
			: base()
		{

		}


		public override void addCharactersNextWord(int length)
		{

			//int newEndWord = length + newLength;
			//startWord += length;
			//endWord += newEndWord;
			//newLength = 0;

			int newEndWord = length + rootWord.newLength;
			startWord += length;
			endWord += newEndWord;

		}


		public override void removeCharactersNextWord(int length)
		{
			//int newEndWord = length + newLength;
			//startWord -= length;
			//endWord -= newEndWord;
			//newLength = 0;

			int newEndWord = length + rootWord.newLength;
			base.startWord -= length;
			endWord -= newEndWord;

		}

		#region IDisposable Membri di

		void IDisposable.Dispose()
		{
			NextWord = null;
			rootWord = null;
		}

		#endregion
	}
}
