

namespace QuickGenerator.Vocabulary
{
	public interface IMatch
	{

		InfoArguments Match(string input, int startPos);
		InfoArguments IsMatch(string test, int startPos);
	}
}
