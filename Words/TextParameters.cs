using ASCompletion.Model;

namespace QuickGenerator.Abbreviation
{
	public class TextParameters
	{
		public TextParameters(MemberModel member)
		{
			this.text = member.ToString();

			//if(member.Comments!=null)
			//this.comments = member.Comments.Split('@');
			this.comments = member.Comments;

		}

		public string text;
		//public string[] comments;
		public string comments;
		public int posParameters;

	}
}
