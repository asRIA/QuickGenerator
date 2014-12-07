using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using UI;
using PluginCore.Controls;
using PluginCore;

namespace QuickGenerator.CustomCompletionList
{
	public delegate Boolean beforeItemInsert(ICompletionListItem mitem);
	public delegate void startPopulateCompletionList();

	abstract public class CustomCompletionBase : IDisposable
	{

		// Controls UI
		public ListBox completionList;
		private customRichToolTip ToolTip;


		public abstract Form formDisplay
		{
			get;
			set;
		}
		// Timer
		private System.Timers.Timer tempo;
		private System.Timers.Timer tempoTip;

		// Collection
		private ICompletionListItem currentItem;
		/// <summary>
		/// What the listbox show
		/// </summary>
		public List<ICompletionListItem> allItems;
		private ICompletionListItem defaultItem;

		//Boolean
		private Boolean autoHideList;
		private Boolean fullList;
		private Boolean needResize;
		private Boolean isActive;
		private Boolean smartMatchInList;
		private bool disableSmartMatch;
		private bool faded;


		//String
		public String word;
		private String currentWord;
		private String widestLabel;

		//Int
		// private  Int32 currentPos;
		private Int32 lastIndex;

		//Event
		static public event InsertedTextHandler OnCancel;
		delegate void pointToScreen();
		public event beforeItemInsert BeforeItemInsert;
		public event startPopulateCompletionList StartPopulateCompletionList;



		//Ctr
		protected abstract Control curCont
		{
			get;
		}

		protected CustomCompletionBase()
		{
			CreateControl();
		}

		protected CustomCompletionBase(Form formDisplay)
		{
			this.formDisplay = formDisplay;
			CreateControl();

		}
		/// <summary>
		/// Initilaize Controlo
		/// </summary>
		public void CreateControl()
		{
			tempo = new System.Timers.Timer();
			// tempo.SynchronizingObject = (Form)mainForm;
			tempo.Elapsed += new System.Timers.ElapsedEventHandler(DisplayList);
			tempo.AutoReset = false;
			tempoTip = new System.Timers.Timer();
			tempoTip.SynchronizingObject = formDisplay;// (Form)mainForm;
			tempoTip.Elapsed += new System.Timers.ElapsedEventHandler(UpdateTip);
			tempoTip.AutoReset = false;
			tempoTip.Interval = 800;

			completionList = new ListBox();
			completionList.Font = new System.Drawing.Font(PluginBase.Settings.DefaultFont, FontStyle.Regular);
			completionList.Visible = false;
			completionList.Location = new Point(200, 200);
			completionList.ItemHeight = completionList.Font.Height + 2;
			completionList.Size = new Size(180, 100);
			completionList.DrawMode = DrawMode.OwnerDrawFixed;
			completionList.DrawItem += new DrawItemEventHandler(CLDrawListItem);
			//completionList.Click += new EventHandler(CLClick);
			completionList.DoubleClick += new EventHandler(CLDoubleClick);


			formDisplay.Controls.Add(completionList);

			ToolTip = new customRichToolTip(this);
		}

		protected void SetAutoHide(Boolean autoHideLostFocus, Boolean autoShow)
		{
			if (autoHideLostFocus)
			{
				curCont.Leave += new EventHandler(curTexBox_Leave);
				formDisplay.Click += new EventHandler(curForm_Click);
			}

			if (autoShow)
			{
				curCont.Enter += new EventHandler(curTexBox_Enter);
				curCont.Click += new EventHandler(curTexBox_Click);
			}
		}

		void curTexBox_Leave(object sender, EventArgs e)
		{
			if (completionList != null) // there is complete list?
			{
				if (!completionList.Focused)
				{
					Hide();
					// Remove handle

				}
			}
		}



		void curForm_Click(object sender, EventArgs e)
		{
			if (completionList.Items.Count != 0 && completionList.Visible)
			{
				Hide();
			}
		}


		void curTexBox_Enter(object sender, EventArgs e)
		{
			if (allItems == null) return;

			if (allItems.Count == 0) return;

			Show(allItems, false);

			FindWordStartingWith(curCont.Text);
		}

		void curTexBox_Click(object sender, EventArgs e)
		{
			if (!completionList.Visible)
			{

				if (allItems == null) return;
				Show(allItems, false);

				if (curCont.Text.Length > 0)
					FindWordStartingWith(curCont.Text);
			}
		}


		public abstract void CLDoubleClick(object sender, EventArgs e);

		//    if (DoubleClick != null)
		//        DoubleClick(sender, e);


		protected void DisplayList(Object sender, System.Timers.ElapsedEventArgs e)
		{
			ListBox cl = completionList;
			if (curCont == null) return;

			if (cl.Items.Count == 0) return;


			if (curCont.InvokeRequired)
			{
				pointToScreen pts = new pointToScreen(PointToScreenSafe);
				curCont.Invoke(pts);
			}
			else
			{
				PointToScreenSafe();

			}

		}

		private void PointToScreenSafe()
		{
			ListBox cl = completionList;

			if (needResize && widestLabel != null && widestLabel.Length > 0)
			{
				needResize = false;
				Graphics g = cl.CreateGraphics();
				SizeF size = g.MeasureString(widestLabel, cl.Font);
				completionList.Width = (int)Math.Min(Math.Max(size.Width + 40, 100), 400) + 10;
			}
			int newHeight = Math.Min(cl.Items.Count, 10) * cl.ItemHeight + 4;
			if (newHeight != cl.Height) cl.Height = newHeight;
			// place control

			Point coord = CalculatePositionCtr();

			cl.Top = coord.Y;
			cl.Left = coord.X;
			cl.Update();

			if (!cl.Visible)
			{
				cl.Show();
				cl.BringToFront();
			}
		}

		/// <summary>
		/// Position where the specific control must show
		/// </summary>
		/// <returns>Coordinate of the control</returns>
		public abstract Point CalculatePositionCtr();

		private void CLDrawListItem(Object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			ICompletionListItem item = completionList.Items[e.Index] as ICompletionListItem;
			e.DrawBackground();
			bool selected = (e.State & DrawItemState.Selected) > 0;
			Brush myBrush = (selected) ? Brushes.White : Brushes.Black;
			Rectangle tbounds = new Rectangle(18, e.Bounds.Top + 1, e.Bounds.Width, e.Bounds.Height);
			if (item != null)
			{
				e.Graphics.DrawImage(item.Icon, 1, e.Bounds.Top + ((e.Bounds.Height - item.Icon.Height) / 2));
				int p = item.Label.LastIndexOf('.');
				if (p > 0)
				{
					string package = item.Label.Substring(0, p + 1);
					e.Graphics.DrawString(package, e.Font, Brushes.Gray, tbounds, StringFormat.GenericDefault);
					SizeF dims = e.Graphics.MeasureString(package, e.Font, tbounds.Width, StringFormat.GenericDefault);
					int left = tbounds.Left + (int)dims.Width + 1;
					if (left < tbounds.Right) e.Graphics.DrawString(item.Label.Substring(p + 1), e.Font, myBrush, left, tbounds.Top, StringFormat.GenericTypographic);
				}
				else e.Graphics.DrawString(item.Label, e.Font, myBrush, tbounds, StringFormat.GenericDefault);
			}
			e.DrawFocusRectangle();
			if ((item != null) && ((e.State & DrawItemState.Selected) > 0))
			{
				UITools.Tip.Hide();
				currentItem = item;
				tempoTip.Stop();
				tempoTip.Start();
			}
		}



		public virtual void Show(List<ICompletionListItem> itemList, bool autoHide)
		{
			if (itemList == null) return;

			allItems = itemList;
			autoHideList = autoHide;
			word = "";
			if (currentWord != null)
			{
				word = currentWord;
				currentWord = null;
			}


			fullList = (word.Length == 0) || !autoHide || !PluginBase.MainForm.Settings.AutoFilterList;

			lastIndex = 0;


			defaultItem = null;
			// populate list
			needResize = true;


			tempo.Enabled = autoHide && (PluginBase.MainForm.Settings.DisplayDelay > 0);
			if (tempo.Enabled) tempo.Interval = PluginBase.MainForm.Settings.DisplayDelay;
			_FindWordStartingWith(word);

			isActive = true;
			tempoTip.Enabled = false;


			disableSmartMatch = PluginBase.MainForm.Settings.DisableSmartMatch;


			faded = false;
		}

		public void FindWordStartingWith(String word)
		{
			if (allItems == null) return;
			_FindWordStartingWith(word);
		}


		/// <summary>
		/// Filter the completion list with the letter typed
		/// </summary> 
		private void _FindWordStartingWith(String word)
		{

			Int32 len = word.Length;
			Int32 maxLen = 0;
			Int32 lastScore = 0;
			/// <summary>
			/// FILTER ITEMS
			/// </summary>
			if (PluginBase.MainForm.Settings.AutoFilterList || fullList)
			{
				List<ICompletionListItem> found;
				if (len == 0)
				{
					found = allItems;
					lastIndex = 0;

					smartMatchInList = true;
				}
				else
				{
					List<ItemMatch> temp = new List<ItemMatch>(allItems.Count);
					Int32 n = allItems.Count;
					Int32 i = 0;
					Int32 score;
					lastScore = 99;
					ICompletionListItem item;
					smartMatchInList = false;
					while (i < n)
					{
						item = allItems[i];
						// compare item's label with the searched word
						score = SmartMatch(item.Label, word, len);
						if (score > 0)
						{
							// first match found
							if (!smartMatchInList || score < lastScore
								/*|| (score == lastScore && item.Label.Length < matched.Length)*/)
							{
								lastScore = score;
								lastIndex = temp.Count;
								smartMatchInList = true;

							}
							temp.Add(new ItemMatch(score, item));
							if (item.Label.Length > maxLen)
							{
								widestLabel = item.Label;
								maxLen = widestLabel.Length;
							}
						}
						else if (fullList) temp.Add(new ItemMatch(0, item));
						i++;
					}
					// filter
					found = new List<ICompletionListItem>(temp.Count);
					for (int j = 0; j < temp.Count; j++)
					{
						if (j == lastIndex) lastIndex = found.Count;
						if (temp[j].Score - lastScore < 3) found.Add(temp[j].Item);
					}
				}
				// no match?
				if (!smartMatchInList)
				{
					if (autoHideList && PluginBase.MainForm.Settings.EnableAutoHide && (len == 0 || len > 255))
						Hide('\0');
					else
					{
						// smart match
						if (word.Length > 0)
							_FindWordStartingWith(word.Substring(0, len - 1));
						if (!smartMatchInList && autoHideList && PluginBase.MainForm.Settings.EnableAutoHide)
							Hide('\0');
					}
					return;
				}
				fullList = false;
				// reset timer
				if (tempo.Enabled)
				{
					tempo.Enabled = false;
					tempo.Enabled = true;
				}
				// is update needed?
				if (completionList.Items.Count == found.Count)
				{
					int n = completionList.Items.Count;
					bool changed = false;
					for (int i = 0; i < n; i++)
						if (completionList.Items[i] != found[i]) { changed = true; break; }
					if (!changed)
					{
						// preselected item
						if (lastScore > 2) lastIndex = TestDefaultItem(lastIndex, word, len);

						if (completionList.Items.Count > 0)
							completionList.SelectedIndex = lastIndex;
						return;
					}
				}
				// update
				try
				{
					if (StartPopulateCompletionList != null)
						StartPopulateCompletionList();

					completionList.BeginUpdate();
					completionList.Items.Clear();
					foreach (ICompletionListItem item in found)
					{

						if (BeforeItemInsert != null)
						{
							if (!BeforeItemInsert(item))
								continue;
						}

						completionList.Items.Add(item);
						if (item.Label.Length > maxLen)
						{
							widestLabel = item.Label;
							maxLen = widestLabel.Length;
						}
					}
					Int32 topIndex = lastIndex;
					if (lastScore > 2) lastIndex = TestDefaultItem(lastIndex, word, len);
					// select first item
					completionList.TopIndex = topIndex;

					if (completionList.Items.Count > 0)
						completionList.SelectedIndex = lastIndex;
				}
				catch (Exception ex)
				{
					Hide('\0');
					MessageBox.Show(ex.Message);
					//ErrorManager.ShowError(/*"Completion list populate error.", */ ex);
					return;
				}
				finally
				{
					completionList.EndUpdate();
				}
				// update list
				if (!tempo.Enabled) DisplayList(null, null);
			}
			/// <summary>
			/// NO FILTER
			/// </summary>
			else
			{
				int n = completionList.Items.Count;
				ICompletionListItem item;
				while (lastIndex < n)
				{
					item = completionList.Items[lastIndex] as ICompletionListItem;
					if (String.Compare(item.Label, 0, word, 0, len, true) == 0)
					{
						completionList.SelectedIndex = lastIndex;
						completionList.TopIndex = lastIndex;

						return;
					}
					lastIndex++;
				}
				// no match
				if (autoHideList && PluginBase.MainForm.Settings.EnableAutoHide) Hide('\0');

			}
		}


		private int SmartMatch(string label, string word, int len)
		{
			if (label.Length < len)
				return 0;

			// simple matching
			if (disableSmartMatch)
			{
				if (label.StartsWith(word, StringComparison.OrdinalIgnoreCase))
				{
					if (label.StartsWith(word)) return 1;
					else return 5;
				}
				return 0;
			}

			int p = label.IndexOf(word, StringComparison.OrdinalIgnoreCase);
			if (p >= 0)
			{
				int p2;
				if (Char.IsUpper(word[0])) // try case sensitive search
				{
					p2 = label.IndexOf(word);
					if (p2 >= 0)
					{
						int p3 = label.LastIndexOf("." + word); // in qualified type name
						if (p3 > 0)
						{
							if (p3 == label.LastIndexOf('.'))
							{
								if (label.EndsWith("." + word)) return 1;
								else return 3;
							}
							else return 4;
						}
					}
					if (p2 == 0)
					{
						if (word == label) return 1;
						else return 2;
					}
					else if (p2 > 0) return 4;
				}

				p2 = label.LastIndexOf("." + word, StringComparison.OrdinalIgnoreCase); // in qualified type name
				if (p2 > 0)
				{
					if (p2 == label.LastIndexOf('.'))
					{
						if (label.EndsWith("." + word, StringComparison.OrdinalIgnoreCase)) return 2;
						else return 4;
					}
					else return 5;
				}
				if (p == 0)
				{
					if (label.Equals(word, StringComparison.OrdinalIgnoreCase))
					{
						if (label.Equals(word)) return 1;
						else return 2;
					}
					else return 3;
				}
				else
				{
					int p4 = label.IndexOf(':');
					if (p4 > 0) return SmartMatch(label.Substring(p4 + 1), word, len);
					return 5;
				}
			}

			// loose
			int firstChar = label.IndexOf(word[0].ToString(), StringComparison.OrdinalIgnoreCase);
			int i = 1;
			p = firstChar;
			while (i < len && p >= 0)
			{
				p = label.IndexOf(word[i++].ToString(), p, StringComparison.OrdinalIgnoreCase);
			}
			return (p > 0) ? 7 : 0;
		}

		/// <summary>
		/// Hide completion list
		/// </summary> 	
		public virtual void Hide()
		{
			if (completionList != null && isActive)
			{
				tempo.Enabled = false;
				isActive = false;
				fullList = false;
				faded = false;
				completionList.Visible = false;
				if (completionList.Items.Count > 0) completionList.Items.Clear();
				currentItem = null;
				//  allItems = null;
				ToolTip.Hide();
				if (!UITools.CallTip.CallTipActive) UITools.Manager.UnlockControl();
			}
		}


		/// <summary>
		/// Cancel completion list with event
		/// </summary> 	
		public void Hide(char trigger)
		{
			if (completionList != null && isActive)
			{
				Hide();
				if (OnCancel != null)
				{
					ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
					if (!doc.IsEditable) return;
					OnCancel(doc.SciControl, 0, currentWord, trigger, null);
				}
			}
		}

		public void Hide(bool isActive)
		{
			this.isActive = isActive;
			Hide();
		}

		protected void UpList()
		{
			if (completionList == null) return;
			if (completionList.Items.Count > 1)
			{

				int act = -1;

				if (completionList.SelectionMode == SelectionMode.MultiExtended)
				{  //act = completionList.SelectedItems[completionList.SelectedItems.Count - 1];
					act = completionList.SelectedIndices[0];
					foreach (int index in completionList.SelectedIndices)
					{
						completionList.SetSelected(index, false);
					}
				}
				else
					act = completionList.SelectedIndex;

				act--;

				if (act < 0) act = 0;

				completionList.SelectedIndex = act;

			}
		}

		protected void downList()
		{
			if (completionList == null) return;


			if (completionList.Items.Count > 1)
			{
				int act = -1;

				if (completionList.SelectionMode == SelectionMode.MultiExtended)
				{  //act = completionList.SelectedItems[completionList.SelectedItems.Count - 1];
					act = completionList.SelectedIndices[completionList.SelectedIndices.Count - 1];
					foreach (int index in completionList.SelectedIndices)
					{
						completionList.SetSelected(index, false);
					}
				}
				else
					act = completionList.SelectedIndex;

				act++;
				if (act >= completionList.Items.Count)
					return;

				completionList.SelectedIndex = act;



			}
		}


		private int TestDefaultItem(Int32 index, String word, Int32 len)
		{
			if (defaultItem != null && completionList.Items.Contains(defaultItem))
			{
				Int32 score = (len == 0) ? 1 : SmartMatch(defaultItem.Label, word, len);
				if (score > 0 && score < 6) return completionList.Items.IndexOf(defaultItem);
			}
			return index;
		}


		/// <summary>
		/// Display item information in tooltip
		/// </summary> 
		private void UpdateTip(Object sender, System.Timers.ElapsedEventArgs e)
		{


			tempoTip.Stop();


			if (currentItem == null || faded) return;

			if (currentItem.Description != ToolTip.Text)
			{
				ToolTip.Text = currentItem.Description;
				ToolTip.Location = new Point(completionList.Right, completionList.Top);

				if (!ToolTip.AutoSize(completionList.Left) && ToolTip.Size.Width > 200)
					ToolTip.Location = new Point(completionList.Left - ToolTip.Size.Width, completionList.Top);
			}

			if (ToolTip.Visible == false)
				ToolTip.Show();



		}


		struct ItemMatch
		{
			public int Score;
			public ICompletionListItem Item;

			public ItemMatch(int score, ICompletionListItem item)
			{
				Score = score;
				Item = item;
			}
		}


		#region IDisposable Membri di

		public void Dispose()
		{
			allItems = null;
			tempoTip.Enabled = false;
			tempoTip.Stop();
			ToolTip.Dispose();
			completionList.Dispose();
			tempoTip.Dispose();
			tempo.Dispose();
		}

		#endregion
	}
}
