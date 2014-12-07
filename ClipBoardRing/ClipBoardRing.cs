using System;
using System.Collections.Generic;

namespace QuickGenerator.clipboardring
{
	class ClipBoardRing
	{
		List<String> clipboardRing;
		private int curItemIndex;



		public ClipBoardRing(int Capacity)
		{
			if (Capacity < 3) Capacity = 3;
			clipboardRing = new List<string>(Capacity);

		}

		public List<string> getClipBoardRing()
		{
			return clipboardRing;
		}
		public void SetCapacity(int capacity)
		{

			List<string> newClipboard = new List<string>(capacity);

			if (capacity >= clipboardRing.Capacity)
			{
				newClipboard.AddRange(clipboardRing);
			}
			else
			{

				int init = clipboardRing.Count - capacity;
				int max = capacity;

				if (clipboardRing.Count < capacity)
					max = clipboardRing.Count;

				if (init < 0) init = 0;
				newClipboard.AddRange(
					clipboardRing.GetRange(init, max)
					);

			}


			clipboardRing = newClipboard;

			curItemIndex = clipboardRing.Count - 1;
		}

		public void insert(string text)
		{

			/// If last copy is egual last insert text don't accept it
			int last = clipboardRing.Count - 1;
			if (clipboardRing.Count > 0)
				if (clipboardRing[last] == text)
					return;

			if (clipboardRing.Count == clipboardRing.Capacity)
			{
				clipboardRing.RemoveAt(0);
			}


			clipboardRing.Add(text);
			curItemIndex = clipboardRing.Count - 1;

		}

		public string ShowPrevious()
		{
			if (clipboardRing.Count == 0) return "";
			curItemIndex--;
			if (curItemIndex < 0)
				curItemIndex = clipboardRing.Count - 1;

			return clipboardRing[curItemIndex];
		}

		public string ShowCurrent()
		{
			if (clipboardRing.Count == 0) return "";
			return clipboardRing[curItemIndex];
		}



		public string ShowFoward()
		{
			if (clipboardRing.Count == 0) return "";
			curItemIndex++;
			if (curItemIndex == clipboardRing.Count)
				curItemIndex = 0;

			return clipboardRing[curItemIndex];
		}




		public void Clear()
		{
			if (clipboardRing != null)
				clipboardRing.Clear();
		}

	}
}
