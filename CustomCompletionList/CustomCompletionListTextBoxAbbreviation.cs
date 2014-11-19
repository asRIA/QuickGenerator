using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace QuickGenerator.CustomCompletionList
{

    public delegate void delegateInsertText(string textInserted);
    class CustomCompletionListTextBoxAbbreviation : CustomCompletionBase
    {

        public event delegateInsertText OnCompletionDifferentInsertText;
        public event delegateInsertText OnInsertText;
        private bool insertCompletionText;
        int startPos;
        int endPos = -1; // position for control
        int add;
        int posEnd; //real position text
        private RichTextBox richTextBox;
        private Form _curForm;
        private string search;
        public CustomCompletionListTextBoxAbbreviation(RichTextBox richTextBox, Form curForm)
            : base(curForm)
        {
            this.richTextBox = richTextBox;
            _curForm = curForm;
           
        }

  
        //protected override void init(System.Windows.Forms.TextBox curTexBox, System.Windows.Forms.Form curForm)
        //{
           
        //    curTexBox.KeyPress += new KeyPressEventHandler(curTexBox_KeyPress);
        //}
        
        void curTexBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (completionList.Visible)
            {

                if (e.KeyChar == '\b')
                    add = -1;
                else
                     add = 1;
                
               
            }
        }
        protected void richTextBox_TextChanged(object sender, EventArgs e)
        {

            if (!completionList.Visible) return;
            int pos = richTextBox.SelectionStart;
            
            if (pos >= startPos && pos <= endPos)
            {
                posEnd = endPos;
                endPos += add;
            }
            else
            {
                Hide();
                return;
            }

            if (posEnd > richTextBox.Text.Length) posEnd = richTextBox.Text.Length;
            search = richTextBox.Text.Substring(startPos, posEnd - startPos);
            FindWordStartingWith(search);

        }


        public override void Show(List<PluginCore.ICompletionListItem> itemList, bool autoHide)
        {
         
            startPos = endPos = posEnd = richTextBox.SelectionStart;
            endPos++;

            
            richTextBox.MouseClick += new MouseEventHandler(curTexBox_MouseClick);
            richTextBox.KeyPress += new KeyPressEventHandler(curTexBox_KeyPress);
            richTextBox.KeyDown += new KeyEventHandler(richTextBox_KeyDown);
            richTextBox.TextChanged+=new EventHandler(richTextBox_TextChanged);
           
           
            base.Show(itemList, autoHide);
        }

        void richTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (completionList.Visible)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    EnterSelectItem();
                    e.SuppressKeyPress = true;
                }
                if (e.KeyCode == Keys.Delete)
                {
                    add = -1;
                }else
                if (e.KeyCode == Keys.Up )
                {
                    UpList();
                    e.SuppressKeyPress = true;
                }else if (e.KeyCode == Keys.Down )
                {
                    downList();
                    e.SuppressKeyPress = true;
                }
            }
            else if (insertCompletionText)
            {
                insertCompletionText = false;
                if (e.KeyCode == Keys.Enter)
                    e.SuppressKeyPress = true;
            }
        }

        public override void Hide()
        {
            richTextBox.KeyDown -= new KeyEventHandler(richTextBox_KeyDown);
            richTextBox.MouseClick -= new MouseEventHandler(curTexBox_MouseClick);
            richTextBox.KeyPress -= new KeyPressEventHandler(curTexBox_KeyPress);
            richTextBox.TextChanged -= new EventHandler(richTextBox_TextChanged);
            endPos = -1;
            base.Hide();
        }


        void curTexBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (completionList.Visible)
                Hide();
        }



        public override System.Drawing.Point CalculatePositionCtr()
        {
            Point coord = new Point();

          //  this.completionList.Width = 150;
            coord = richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart);
            
            //curTexBox.ge
            //coord.Y += curTexBox.Top;
            //coord.X = curTexBox.Left;
            //coord.Y = curTexBox.Top + curTexBox.Height;
         //   coord = curTexBox.PointToScreen(curTexBox.Location);

            
            Point locationOnForm = richTextBox.FindForm().PointToClient(
             richTextBox.Parent.PointToScreen(richTextBox.Location));
            locationOnForm.X += coord.X;
            locationOnForm.Y += coord.Y + 20;
            //coord = curTexBox.Location;
            return locationOnForm;
        }
       
        protected  void EnterSelectItem()
        {
            if (!completionList.Visible) return;
            PluginCore.ICompletionListItem mi = (PluginCore.ICompletionListItem)completionList.SelectedItem;

           
            //if (mi == null)
            //{
            //    if (EnterWithoutCompletition != null)
            //        EnterWithoutCompletition();
            //    return;

            //}
            insertCompletionText = true;
            richTextBox.TextChanged -= new EventHandler(richTextBox_TextChanged);
           
            Hide(true);

            if(search!=null)
            if (search.Length > 0)
            {
                richTextBox.SelectionStart -= search.Length;
                richTextBox.SelectionLength = search.Length;
                richTextBox.SelectedText = String.Empty;
                search = String.Empty;
            }

            if (OnCompletionDifferentInsertText != null)
            {
               
                OnCompletionDifferentInsertText(mi.Value);
            }
            else
            {
                //int pos = richTextBox.SelectionStart;
                //richTextBox.Text = richTextBox.Text.Insert(pos, mi.Value);
                //richTextBox.SelectionStart = pos + mi.Value.Length;
                if (endPos != -1 && richTextBox.SelectionLength==0)
                {
                    richTextBox.SelectionStart = startPos;
                    richTextBox.SelectionLength = posEnd - startPos;
                }

               richTextBox.SelectedText = mi.Value;
            }



            
            if (OnInsertText != null)
                OnInsertText(mi.Value);
           
                
        }




        public override Form formDisplay
        {
            get
            {
               return _curForm;
            }
            set
            {
               _curForm = value;
            }
        }

        protected override Control curCont
        {
            get { return richTextBox; }
        }

        public override void CLDoubleClick(object sender, EventArgs e)
        {
            EnterSelectItem();
        }
    }
}

