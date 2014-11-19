using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QuickGenerator.CustomCompletionList
{

    delegate void listItemAdded();

    class customCompletionTextBoxAndListBox : customCompletionTextBox
    {

        //UI
        private ListBox _lstBox;
        //Event
        public event listItemAdded ItemAfterAdded;
        private List<string> listSearchIn;

        public  customCompletionTextBoxAndListBox(TextBox textBox, ListBox lstBox, Form frm):base(textBox, frm, true,true)
        {
            this.curTexBox = textBox;
            this._lstBox = lstBox;
            completionList.SelectionMode = SelectionMode.MultiExtended;
            completionList.ValueMemberChanged += new EventHandler(completionList_ValueMemberChanged);            
            this.BeforeItemInsert += new beforeItemInsert(customCompletionTextBoxAndListBox_BeforeItemInsert);
            this.StartPopulateCompletionList += new startPopulateCompletionList(customCompletionTextBoxAndListBox_StartPopulateCompletionList);
        }

        void customCompletionTextBoxAndListBox_StartPopulateCompletionList()
        {
            if (_lstBox.Items.Count > 0)
            {
                listSearchIn = new List<string>(_lstBox.Items.Count);

                foreach (String value in _lstBox.Items)
                {
                    listSearchIn.Add(value);
                }

            }
            else
            {
                listSearchIn = null;
            }

        }

        bool customCompletionTextBoxAndListBox_BeforeItemInsert(PluginCore.ICompletionListItem mitem)
        {
            if (listSearchIn  == null) return true;

            foreach (String value in listSearchIn)
            {
                if (mitem.Value == value)
                {

                    listSearchIn.Remove(value);
                   return false;

                }
            }


         //   listSearchIn = null;

            return true;
        }

 

        void completionList_ValueMemberChanged(object sender, EventArgs e)
        {
            MessageBox.Show("value change");
        }

 

      

        public override void CLDoubleClick(object sender, EventArgs e)
        {
            if (insertTextAfterDoubleClick)
            {
                if (completionList.SelectedIndex != -1)
                {

                    foreach (ASCompletion.Completion.MemberItem  mi in completionList.SelectedItems)
	                {

                        if (!_lstBox.Items.Contains(mi.Value))
                        {

                            _lstBox.Items.Add(mi.Value);
                            if (ItemAfterAdded != null)
                            {
                                ItemAfterAdded();
                            }
                        }

	                } 
                    
                    Hide();
                }
            }
        }

    }
}
