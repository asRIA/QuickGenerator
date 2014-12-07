using System;
using System.Collections.Generic;
using PluginCore;
using ASCompletion.Model;
using ASCompletion.Completion;
using ASCompletion.Context;
using ScintillaNet;
using PluginCore.Utilities;
using ScintillaNet.Enums;

namespace QuickGenerator.CustomCompletionList
{
	class ExplorerProject
	{

		private ExplorerProject()
		{
		}

		public static MemberModel SearchFunction(int LinePos)
		{
			MemberModel found = null;
			FileModel fm = ASContext.Context.CurrentModel;
			FlagType mask = FlagType.Function;

			foreach (ClassModel item in fm.Classes)
			{
				foreach (MemberModel mm in item.Members)
				{

					if ((mm.Flags & mask) == FlagType.Function)
					{
						if (LinePos > mm.LineFrom && LinePos < mm.LineTo)
						{
							found = mm;
							return found;
						}

					}
				}
			}



			//if (md == null)
			//{
			//    msg = "null";
			//}
			//else
			//{
			//    msg = md.Name;
			//}

			return found;
		}


		//public static void AddHighlights(ScintillaControl sci, int posText, string textToHigh, System.Drawing.Color color)
		//{

		//        //Int32 start = sci.MBSafePosition(posText);
		//        Int32 start = posText;
		//        Int32 end = start + sci.MBSafeTextLength(textToHigh);
		//        Int32 position = start;
		//        Int32 mask = 1 << sci.StyleBits;

		//        sci.SetIndicStyle(0, (Int32)IndicatorStyle.Plain);

		//        sci.SetIndicFore(0, DataConverter.ColorToInt32(color));

		//        sci.StartStyling(position, mask);
		//        sci.SetStyling(end - start, mask);




		//}

		public static void AddHighlights(ScintillaControl sci, Int32 posText, Int32 length, System.Drawing.Color color, IndicatorStyle indicator)
		{


			if (length == 0) return;

			Int32 mask = 1 << sci.StyleBits;

			sci.SetIndicStyle(0, (Int32)indicator);
			sci.SetIndicFore(0, DataConverter.ColorToInt32(color));

			sci.StartStyling(posText, mask);
			sci.SetStyling(length, mask);



		}


		// <summary>
		/// Removes the highlights from the correct sci control
		/// </summary>
		public static void RemoveHighlights(ScintillaControl sci)
		{
			Int32 es = sci.EndStyled;
			Int32 mask = (1 << sci.StyleBits);
			sci.StartStyling(0, mask);
			sci.SetStyling(sci.TextLength, 0);
			sci.StartStyling(es, mask - 1);
		}


		/// <summary>
		/// Generate a List of classes for import
		/// </summary>
		/// <returns></returns>
		public static List<ICompletionListItem> RetrieveListImports()
		{
			ASCompletion.Model.MemberList ml = ASCompletion.Context.ASContext.Context.GetAllProjectClasses();

			FlagType mask = (true) ? FlagType.Class | FlagType.Interface | FlagType.Enum : (FlagType)uint.MaxValue;

			List<ICompletionListItem> list = new List<ICompletionListItem>();

			if (ml.Count == 0)
			{
				return null;
			}
			string prev = "";

			foreach (ASCompletion.Model.MemberModel member in ml)
			{
				if ((member.Flags & mask) == 0 || prev.Equals(member.Name))
					if (member.Type != "Class") continue;
				prev = member.Name;
				list.Add(new MemberItem(member));



			}

			return list;
		}




		public static List<ICompletionListItem> RetrieveListInterface()
		{
			List<ICompletionListItem> list = new List<ICompletionListItem>();
			if (PluginBase.CurrentProject == null) return list;
			MemberList known = null;
			IASContext context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
			// FlagType IncludeFlag = FlagType.Interface;
			//  FlagType ExcludeFlag = FlagType.Class;
			string prev = String.Empty;

			try
			{

				known = context.GetAllProjectClasses();
				known.Merge(ASContext.Context.GetVisibleExternalElements());
			}
			finally
			{
				//                Debug.WriteLine(error.StackTrace);
			}

			FlagType mask = FlagType.Interface;

			if (known != null)
			{
				foreach (MemberModel member in known)
				{

					//if (ExcludeFlag > 0) if ((item.Flags & ExcludeFlag) > 0) continue;
					//if (IncludeFlag > 0)
					//{
					//    if (!((item.Flags & IncludeFlag) > 0))
					//    {

					//        continue;
					//    }
					//}

					if ((member.Flags & mask) == 0 || prev.Equals(member.Name))
						if (member.Type != "Interface") continue;
					prev = member.Name;
					list.Add(new MemberItem(member));



					// list.Add(new MemberItem(item));

				}
			}




			return list;
		}
	}






}
