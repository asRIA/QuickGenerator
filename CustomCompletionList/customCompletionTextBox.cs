using System;
using System.Windows.Forms;
using System.Drawing;

namespace QuickGenerator.CustomCompletionList
{
    public delegate void enterWithoutCompletition();


    class customCompletionTextBox : CustomCompletionBase
    {
        protected TextBox curTexBox;
        public Boolean insertTextAfterDoubleClick;
        private Form _curForm;


        public event enterWithoutCompletition EnterWithoutCompletition;


      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curTexBox">TextBox that show listbox</param>
        /// <param name="curForm">Form that contain Textbox</param>
        /// <param name="autoHideLostFocus">If true hide the list box if textbox lost focus</param>
        /// <param name="autoShow">if true Show the listbox if mouse enter in the textbox</param>
        public customCompletionTextBox(TextBox curTexBox, Form curForm, Boolean autoHideLostFocus, Boolean autoShow)
            : base(curForm)
        {

            init(curTexBox, curForm);

            SetAutoHide(autoHideLostFocus, autoShow);


        }
       
       


        

        private void init(TextBox curTexBox, Form curForm)
        {

            this.curTexBox = curTexBox;
            this._curForm = curForm;
            insertTextAfterDoubleClick = true;
            curTexBox.TextChanged += new EventHandler(curTexBox_TextChanged);
            curTexBox.KeyDown += new KeyEventHandler(curTexBox_KeyDown);
        }

        void curTexBox_KeyDown(object sender, KeyEventArgs e)
        {


            switch (e.KeyCode)
            {
                case Keys.Enter:
                    EnterSelectItem();
                    break;
                case Keys.Up:
                    UpList();
                    break;
                case Keys.Down:
                    downList();
                    break;
            }
        }


       

        protected virtual  void EnterSelectItem()
        {
            ASCompletion.Completion.MemberItem mi = (ASCompletion.Completion.MemberItem)completionList.SelectedItem;

            if (mi == null)
            {
                if (EnterWithoutCompletition != null)
                    EnterWithoutCompletition();
                return;

            }
            curTexBox.Text = mi.Value;

            Hide(true);
            curTexBox.SelectionStart = curTexBox.Text.Length;

        }



      



        protected virtual  void curTexBox_TextChanged(object sender, EventArgs e)
        {
            if (allItems == null) return;
            if (allItems.Count == 0) return;
            if (completionList == null) return;


            FindWordStartingWith(curTexBox.Text);
        }

        



        public override System.Drawing.Point CalculatePositionCtr()
        {
            Point coord = new Point();

            this.completionList.Width = curTexBox.Width;
            coord.X = curTexBox.Left;
            coord.Y = curTexBox.Top + curTexBox.Height;

            return coord;
        }

        public override void CLDoubleClick(object sender, EventArgs e)
        {
            if (insertTextAfterDoubleClick)
            {
                if (completionList.SelectedIndex != -1)
                {
                    EnterSelectItem();
                    Hide();
                }
            }
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
            get { return (Control)curTexBox; }
        }
    }
}