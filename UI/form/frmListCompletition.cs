using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QuickGenerator.UI
{
    public partial class frmListCompletition : Form
    {
        public frmListCompletition()
        {
            InitializeComponent();
        }
        Dictionary<string, List<string>> listForAbbrevations;


        private bool isLsNameChange;

        private void frmListCompletition_Load(object sender, EventArgs e)
        {
            editBox = new TextBox();
            editBox.BackColor = Color.Beige;
            editBox.ForeColor = Color.Blue;
            editBox.BorderStyle = BorderStyle.FixedSingle;

            editBox.LostFocus += new EventHandler(editBox_LostFocus);
        }

        void editBox_LostFocus(object sender, EventArgs e)
        {
            editBox.Hide();
            if (editBox.Text.Trim().Length==0) return;

            if (!isLsNameChange)
            {
                string key = (string)lsName.SelectedItem;
                string text = (string)lstString.SelectedItem;

                if (text == editBox.Text) return;
                List<string> temp = listForAbbrevations[key];
                 int ind = lstString.SelectedIndex;

                temp.RemoveAt(ind);
                temp.Insert(ind,editBox.Text);

                lstString.Items.RemoveAt(ind);
                lstString.Items.Insert(ind, editBox.Text);
            }
            else
            {
                string key = (string)lsName.SelectedItem;

                if (editBox.Text == key) return;
                if (ControlField(editBox.Text)) return;

                int ind = lsName.SelectedIndex;



                List<string> temp = listForAbbrevations[key];
                listForAbbrevations.Remove(key);
                listForAbbrevations.Add(editBox.Text, temp);

                lsName.Items.RemoveAt(ind);

                lsName.Items.Insert(ind, editBox.Text);
                lsName.SelectedIndex = ind;
            }
           
           
     
        }


        public frmListCompletition(Dictionary<string, List<string>> lst)
        {
            InitializeComponent();


            listForAbbrevations = lst;

            showKeys();
            this.StartPosition = FormStartPosition.CenterScreen;
           
        }



        private void btnAddKey_Click(object sender, EventArgs e)
        {
            if (txtNameList.Text.Trim().Length==0)
            {
                MessageBox.Show("Name list Missing!!!");
                return;
            }
            addKey();
        }

        void addKey()
        {

            if (ControlField(txtNameList.Text)) return;

            if (listForAbbrevations.Count == 0) lsName.Items.Clear();

         

           
           lsName.Items.Add(txtNameList.Text);
           lsName.Enabled = true;
           lsName.SelectedIndex = lsName.Items.Count - 1;

           listForAbbrevations.Add(txtNameList.Text, new List<string>());


           lstString.Items.Clear();
        

        }



        void removeKey()
        {

            if (lsName.Items.Count == 0 || lsName.Enabled == false) return;

            if (MessageBox.Show("Are you sure delete select list?", "Delete select list", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }



            string key = (string)lsName.SelectedItem;

            lsName.Items.Remove(lsName.SelectedItem);
            listForAbbrevations.Remove(key);

            if (lsName.Items.Count == 0)
            {

                lsName.Enabled = false;
                lsName.Items.Add("Nothing");

            }


        }

        private bool ControlField(string text)
        {
            foreach (String item in lsName.Items)
            {

                if (item == text)
                {
                    MessageBox.Show("Name list already present!!!");
                    return true;
                }
            }

            return false;
        }

        void showKeys()
        {

            if (listForAbbrevations.Count == 0)
            {
                lsName.Items.Clear();
                lsName.Items.Add("Nothing");
                lsName.SelectedIndex = 0;
                lsName.Enabled = false;
                return;
            }


            lsName.Items.Clear();


            foreach (string name in listForAbbrevations.Keys)
            {
               lsName.Items.Add(name);
                
            }

            if (lsName.Items.Count > 0)
                lsName.SelectedIndex = lsName.Items.Count - 1;

            lsName.Enabled = true;


        }

        private void btnRemoveKey_Click(object sender, EventArgs e)
        {
            removeKey();
        }




        private void btnUP_Click(object sender, EventArgs e)
        {
            up(lstString.SelectedIndex);
        }


        void up(int ind)
        {
            if (ind == 0) return;
            if (lstString.Items.Count == 0) return;
             if(lsName.SelectedIndex==-1) return;
             if (lstString.SelectedIndex == -1) return;
            string current = (string)lstString.Items[ind];
            string newPos = (string)lstString.Items[ind - 1];

            List<string> ls = listForAbbrevations[(string)lsName.SelectedItem];

     

            ls.RemoveAt(ind - 1);
            ls.Insert(ind - 1, current);
            ls.RemoveAt(ind);
            ls.Insert(ind, newPos);


            lstString.Items.RemoveAt(ind - 1);
            lstString.Items.Insert(ind - 1, current);
            lstString.Items.RemoveAt(ind);
            lstString.Items.Insert(ind, newPos);


            lstString.SelectedIndex = ind -1;


        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            down(lstString.SelectedIndex);
        }

        void down(int ind)
        {

            if (lstString.SelectedIndex == -1) return;
            if (lstString.Items.Count == 0) return;
            if (ind == lstString.Items.Count - 1) return;

            string current = (string)lstString.Items[ind];
            string newPos = (string)lstString.Items[ind + 1];


            List<string> ls = listForAbbrevations[(string)lsName.SelectedItem];


            ls.RemoveAt(ind + 1);
            ls.Insert(ind + 1, current);
            ls.RemoveAt(ind);
            ls.Insert(ind, newPos);


            lstString.Items.RemoveAt(ind + 1);
            lstString.Items.Insert(ind + 1, current);
            lstString.Items.RemoveAt(ind);
            lstString.Items.Insert(ind, newPos);


            lstString.SelectedIndex = ind + 1;

        }

        private void btnInsertText_Click(object sender, EventArgs e)
        {
            addItemList();
        }

        void addItemList()
        {
       

            if (lsName.Enabled == false) return;
            if (textBox1.Text.Trim().Length==0)
            {
                MessageBox.Show("Insert text!!!");
                return;
            }


            listForAbbrevations[(string)lsName.SelectedItem].Add(textBox1.Text);
            lstString.Items.Add(textBox1.Text);
            

            textBox1.Text = "";
            lstString.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            removeItemList();
        }

        void removeItemList()
        {
            if (lsName.SelectedIndex == -1 || listForAbbrevations.Count==0 || lstString.SelectedIndex==-1) return;
           

            listForAbbrevations[(string)lsName.SelectedItem].RemoveAt(lstString.SelectedIndex);
            lstString.Items.RemoveAt(lstString.SelectedIndex);

       

        }



        public Dictionary<string, List<string>> getList()
        {
            return listForAbbrevations;
        }




        private void lstString_DoubleClick(object sender, EventArgs e)
        {
            isLsNameChange = false;
            CreateEditBox(lstString);
        }

        private TextBox editBox;

        private void CreateEditBox(object sender)
        {
            
            ListBox listbox1 = (ListBox) sender;
            if (listbox1.SelectedIndex == -1) return;
            int itemSelected = listbox1.SelectedIndex;
            Rectangle r = listbox1.GetItemRectangle(itemSelected);
            string itemText = (string)listbox1.Items[itemSelected];
            int delta = 10;
            editBox.Location = new System.Drawing.Point(r.X + delta, r.Y + delta);
            editBox.Size = new Size(r.Width - 10, r.Height - delta);
            editBox.Show();
            listbox1.Controls.Add(editBox);
            editBox.Text = itemText;
            editBox.Focus();
            


        }

        private void lsName_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (lsName.SelectedIndex == -1) return;
            List<string> ls = null;
            if (listForAbbrevations.TryGetValue((string)lsName.SelectedItem, out ls))
            {
               

                lstString.Items.Clear();

                foreach (string item in ls)
                {
                    lstString.Items.Add(item);    
                }


                lstString.Enabled = true;
            }
            else
            {
                lstString.Enabled = false;
            }
    
        }

        private void lsName_DoubleClick(object sender, EventArgs e)
        {

            isLsNameChange = true;
            CreateEditBox(lsName);
        }

        private void frmListCompletition_FormClosing(object sender, FormClosingEventArgs e)
        {



            bool errorList = false;
            int i = -1;
            foreach (KeyValuePair<string,List<string>> item in listForAbbrevations)
            {
                i++;

                if (item.Value.Count == 0)
                {
                    errorList = true;
                    lsName.SelectedIndex = i;
                    lstString.Items.Clear();
                    MessageBox.Show("List [" + (string) lsName.SelectedItem +  "] is empty!! Please insert a value!!");
                    break;
                }
            }

            e.Cancel = errorList;
        }

     

       
    }
}
