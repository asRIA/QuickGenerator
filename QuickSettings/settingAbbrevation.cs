using System.Collections.Generic;
using QuickGenerator.Abbreviation;

namespace QuickGenerator.QuickSettings
{
	public class settingAbbrevation
	{
		public Dictionary<string, Dictionary<string, AbbrevationSnippet>> AbbrevationDictionary;
		public Dictionary<string, List<string>> CustomList;
		public bool ColorArgument;



		public override string ToString()
		{
			return string.Empty;
		}
	}
}
