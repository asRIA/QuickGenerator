using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using QuickGenerator.Abbreviation;
using System.Text.RegularExpressions;
using ASCompletion.Context;
using QuickGenerator.CustomCompletionList;
using PluginCore;
using QuickGenerator.Vocabulary;
using System.Drawing;

namespace QuickGenerator.UI.form
{
    public partial class AbbrevationCompletionForm :baseSettings
    {
        private bool isAdd;
        private bool isModify;
        private ListViewItem curItem;
        private string actLanguage;
        private CustomCompletionListTextBoxAbbreviation cst;
  
        private Dictionary<string, Dictionary<string, AbbrevationSnippet>> dictAbbrevations;
        QuickSettings.settingAbbrevation _settings;
        int indexCmbLanguage = -1;
        private List<ICompletionListItem> CompletionListArguments;
        private List<ICompletionListItem> CompletionListCustomList;
        private List<ICompletionListItem> CompletionListAbbreviations;
        VocabularyArgument vocabularyArgument;
        private Font fontArguments;
        private Font fontTxtExpanded;
        private bool isActiveList;

        public AbbrevationCompletionForm(QuickSettings.settingAbbrevation settings, VocabularyArgument vocabulary)
        {
            
            InitializeComponent();

            
            vocabularyArgument = vocabulary;
            if (vocabularyArgument == null) vocabularyArgument = new VocabularyArgument();

        
            foreach (string item in vocabularyArgument.ListArguments)
            {

                lstPlaceHolder.Items.Add(item);

            }

            fontArguments = new Font("Verdana", 8, FontStyle.Bold);
            fontTxtExpanded = txtExpandesText.SelectionFont;
            
            Dictionary<string, Dictionary<string, AbbrevationSnippet>> abbrevation = settings.AbbrevationDictionary;
           
            cst = new CustomCompletionListTextBoxAbbreviation(txtExpandesText, this);
         
            
            if (abbrevation != null)
            {
                cmbLanguage.Items.Clear();
                String ext="";
              
                int i=-1;
                if (ASContext.Context.CurrentFile != null)
                {
                    ext = System.IO.Path.GetExtension(ASContext.Context.CurrentFile).ToLower(); ;
                }
                foreach (KeyValuePair<string, Dictionary<string, AbbrevationSnippet>> dict in abbrevation)
                {
                  
                    if (dict.Key == ".other") continue;
                    i++;
                    if (dict.Key == ext) indexCmbLanguage = i;
                    cmbLanguage.Items.Add(dict.Key.Substring(1));
                }


                cmbLanguage.Items.Add("other");

                if (indexCmbLanguage != -1)
                { cmbLanguage.SelectedIndex = indexCmbLanguage; }
                else
                {
                    indexCmbLanguage = cmbLanguage.Items.Count - 1;
                }
                dictAbbrevations = abbrevation;
            }
            //else
            //{

            //    dictAbbrevations = new Dictionary<string, Dictionary<string, AbbrevationSnippet>>();

            //    for (int i = 0; i < cmbLanguage.Items.Count; i++)
            //    {
            //        Dictionary<string, AbbrevationSnippet> abbreviatonsTemp = new Dictionary<string, AbbrevationSnippet>();
            //        dictAbbrevations.Add("." + ((string)cmbLanguage.Items[i]).ToLower(), abbreviatonsTemp);
            //    }

            //    indexCmbLanguage = 0;

            //    //AbbrevationSnippet abr = new AbbrevationSnippet("for(var ${var=\"i\" list=\"ls3\"}:int=0; ${var=\"i\"} < ${\"length\"}; ${var=\"i\"}++)\r\n{\r\n\t${SafeZone}\r\n}");
            //    //abr.Arguments = new WordTypes[5];
            //    //abr.Arguments[0] = WordTypes.var;
            //    //abr.Arguments[1] = WordTypes.var;
            //    //abr.Arguments[2] = WordTypes.place;
            //    //abr.Arguments[3] = WordTypes.var;
            //    //abr.Arguments[4] = WordTypes.SafeZone;

            //    //dictAbbrevations[".as"].Add("fori", abr);

            //    //abr = new AbbrevationSnippet("${list=\"ls1\"} function ${var=\"name\"}(${\"\"}):void\r\n{\r\n\t${SafeZone}\r\n}");
            //    //abr.Arguments = new WordTypes[4];
            //    //abr.Arguments[0] = WordTypes.list;
            //    //abr.Arguments[1] = WordTypes.var;
            //    //abr.Arguments[2] = WordTypes.place;
            //    //abr.Arguments[3] = WordTypes.SafeZone;
                
            //    //dictAbbrevations[".as"].Add("fnc", abr);

            //    //abr = new AbbrevationSnippet("var ${var=\"request\"}:${Import=\"URLRequest\"} = new URLRequest(${browser});\r\nvar ${var=\"loader\"}:Loader = new ${Import=\"Loader\"}();\r\n${var=\"loader\"}.contentLoaderInfo.addEventListener(${Import=\"Event\"}.COMPLETE, ${EventHandler=\"completeHandler\"});\r\n${var=\"loader\"}.load(${var=\"request\"});");
            //    //abr.Arguments = new WordTypes[10];
            //    //abr.Arguments[0] = WordTypes.var;
            //    //abr.Arguments[1] = WordTypes.import;
            //    //abr.Arguments[2] = WordTypes.browser;
            //    //abr.Arguments[3] = WordTypes.var;
            //    //abr.Arguments[4] = WordTypes.import;
            //    //abr.Arguments[5] = WordTypes.var;
            //    //abr.Arguments[6] = WordTypes.import;
            //    //abr.Arguments[7] = WordTypes.EventHandler;
            //    //abr.Arguments[8] = WordTypes.var;
            //    //abr.Arguments[9] = WordTypes.var;
            //    //abr.HasImport = true;
            //    //abr.HasEventHandler = true;
            //    //dictAbbrevations[".as"].Add("load", abr);
            //    ////dictAbbrevations[".as"].Add("vr", new AbbrevationSnippet("${list=0}var ${cursor}:${showCompType};"));



            //    //abr = new AbbrevationSnippet("${list=\"ls2\"} ${\"a\"}:${cmp=\"Number\"} = ${\"0\"};");
            //    //abr.Arguments = new WordTypes[4];
            //    //abr.Arguments[0] = WordTypes.list;
            //    //abr.Arguments[1] = WordTypes.place;
            //    //abr.Arguments[2] = WordTypes.cmp;
            //    //abr.Arguments[3] = WordTypes.place;
              
            //    //dictAbbrevations[".as"].Add("vr", abr);

            //    //abr = new AbbrevationSnippet("var ${\"mc\"}:${var=\"MovieClip\" showCmp} = new ${var=\"MovieClip\"}(${createParameters});");
            //    //abr.Arguments = new WordTypes[4];
            //    //abr.Arguments[0] = WordTypes.place;
            //    //abr.Arguments[1] = WordTypes.var;
            //    //abr.Arguments[2] = WordTypes.var;
            //    //abr.Arguments[3] = WordTypes.createParameters;
              
            //    //dictAbbrevations[".as"].Add("nw", abr);


            //    //abr = new AbbrevationSnippet("${var=\"name\"}(${createParameters});${AfterCurrentMember=\"fnc\"}");
            //    //abr.Arguments = new WordTypes[3];
            //    //abr.Arguments[0] = WordTypes.var;
            //    //abr.Arguments[1] = WordTypes.createParameters;
            //    //abr.Arguments[2] = WordTypes.AfterCurrentMember;
            //    //abr.HasAfterCurrentMember = true;
            //    //dictAbbrevations[".as"].Add("out", abr);


            //    //settings.AbbrevationDictionary = dictAbbrevations;
            //}

            //if (settings.CustomList == null)
            //{
            //    settings.CustomList = new Dictionary<string, List<string>>();
            //    List<string> ls = new List<string>();
            //    ls.Add("public");
            //    ls.Add("private");
            //    ls.Add("protected");
            //    settings.CustomList.Add("ls1", ls);
            //    ls = new List<string>();
            //    ls.Add("var");
            //    ls.Add("public var");
            //    ls.Add("private var");
            //    settings.CustomList.Add("ls2", ls);

            //     ls = new List<string>();
            //    ls.Add("x");
            //    ls.Add("y");
            //    ls.Add("z");
            //    settings.CustomList.Add("ls3", ls);
            //}
            _settings = settings;

            chkColorArgument.Checked = settings.ColorArgument;
            
            if(settings.ColorArgument)
                this.txtExpandesText.TextChanged += new System.EventHandler(this.txtExpandesText_TextChanged);
               
        }

        
        private void btnAdd_Click(object sender, EventArgs e)
        {

            ResetTexts();

            btnAdd.Visible = false;
            btnRemove.Visible = false;
            btnCancel.Visible = true;
            btnConfirm.Visible = true;
            txtExpandesText.Enabled = true;
            txtAbbrevation.Enabled = true;
            lsvCodeTemplate.Enabled = false;
            cmbLanguage.Enabled = false;
            btnModify.Visible = false;
            btnAddLanguageExt.Enabled = false;
            btnRemoveLanguage.Enabled = false;
            isAdd = true;

            
        }

        private void retrieveCurrentItem()
        {
            curItem = null;

            foreach (ListViewItem item in lsvCodeTemplate.SelectedItems)
            {
                if (item != null)
                {
                    curItem = item;
                   
                }
            }

        }

        private void ResetTexts()
        {
            txtAbbrevation.Text = "";
            txtExpandesText.Text = "";
        }

        private bool ControlField()
        {
            if (txtExpandesText.Text.Trim().Length == 0)
            {
                MessageBox.Show("Text Missing!!");
                return false;
            }

            if (txtAbbrevation.Text.Trim().Length == 0)
            {
                MessageBox.Show("Abbrevation Missing!!");
                return false;
            }


         
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
           
            EnabledControl();
            isAdd = false;
            isModify = false;
            txtExpandesText.Text = "";
            txtAbbrevation.Text = "";
            //if(lsvCodeTemplate.Items.Count>0)
                
        }

        private void EnabledControl()
        {
            btnAdd.Visible = true;
            btnRemove.Visible = true;
            btnModify.Visible = true;
            btnConfirm.Visible = false;
            btnCancel.Visible = false;
            lsvCodeTemplate.Enabled = true;
            txtAbbrevation.Enabled = false ;
            txtExpandesText.Enabled = false;
            cmbLanguage.Enabled = true;
            btnAddLanguageExt.Enabled = true;
            btnRemoveLanguage.Enabled = true;
            checkRemoved();

            lsvCodeTemplate.Select();
        }
       
        //public bool checkTooMuchCursor()
        //{
        //    // for moment disable it
        //    // for use future
        //    return false;
        //    //int ind = txtExpandesText.Text.IndexOf(cursor);


        //    //if (ind!=-1)
        //    //{
        //    //    ind = txtExpandesText.Text.IndexOf(cursor, ind + cursor.Length);
        //    //    if (ind != -1)
        //    //        return true;
        //    //}

        //    //return false;
        //}

        private void btnConfirm_Click(object sender, EventArgs e)
        {

             if (!ControlField())
                    return;

            if (isAdd)
            {

                if (AbbrevationExist(txtAbbrevation.Text.Trim()))
                {
                    MessageBox.Show("Abbrevation already present!!");
                    return;
                }

                if (ErrorCreateParameters())
                {
                    MessageBox.Show("No surround bracket on ${createParameters}!!!");
                    return;
                }



                ListViewItem item = new ListViewItem(txtAbbrevation.Text.Trim());
                ListViewItem.ListViewSubItem sub = new ListViewItem.ListViewSubItem();

                //if (checkTooMuchCursor())
                //{
                //    MessageBox.Show("Only a ${cursor} is permitted!!!");
                //    return;
                //}

                AbbrevationSnippet abr = new AbbrevationSnippet(txtExpandesText.Text.Trim());
                ScanAbbreviation( abr);

                sub.Text = abr.Snippet ;
                item.SubItems.Add(sub);
                lsvCodeTemplate.Items.Add(item);
                //sub = testo
              

                _settings.AbbrevationDictionary["." +(string) cmbLanguage.SelectedItem].Add(item.Text, abr);
                txtExpandesText.Text = abr.Snippet;
                isAdd = false;
               
                EnabledControl();


                item.Selected = true;

                CompletionListAbbreviations = null;
              
            }
            else if (isModify)
            {


                if (ErrorCreateParameters())
                {
                    MessageBox.Show("No surround bracket on ${createParameters}!!!");
                    return;
                }


                foreach (ListViewItem item in lsvCodeTemplate.SelectedItems)
                {
                    if (item != null)
                    {
                        string text = txtAbbrevation.Text.Trim();

                        foreach (ListViewItem itm in lsvCodeTemplate.Items)
                        {
                            if (itm.Text == text && item.Index != itm.Index)
                            {
                                MessageBox.Show("Abbrevation already present!!");
                                return;
                            }
                        }
                        string oldValue = item.Text;

                        item.Text = text;


                        AbbrevationSnippet abr = new AbbrevationSnippet(txtExpandesText.Text.Trim());
                        _settings.AbbrevationDictionary["." + (string)cmbLanguage.SelectedItem].Remove(oldValue);
                        ScanAbbreviation( abr);
                        _settings.AbbrevationDictionary["." + (string)cmbLanguage.SelectedItem].Add(item.Text,abr);

                        if (item.SubItems.Count > 0)
                            item.SubItems[1].Text = abr.Snippet;

                        txtExpandesText.Text = abr.Snippet;
                        //   .Add(item.Text, abr);
                        item.Selected = true;

                        CompletionListAbbreviations = null;
                    }
                }




                isModify = false;
                EnabledControl();
            }

          
            
        }

        // Control if there is a Abbrevation with same name
        private bool AbbrevationExist(string text)
        {
            foreach (ListViewItem item in lsvCodeTemplate.Items )
            {
                if (item.Text == text)
                    return true;
            }


            return false;
        }


        private void lsvCodeTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowItem();
            btnRemove.Visible = true;
            btnModify.Visible = true;
        }

        private void ShowItem()
        {
            foreach (ListViewItem item in lsvCodeTemplate.SelectedItems)
	            {
                    if (item != null)
                    {
                        txtAbbrevation.Text = item.Text;
                        if(item.SubItems.Count>0)
                        txtExpandesText.Text = item.SubItems[1].Text;
                    }
	            }
               
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            lsvCodeTemplate.Enabled = false;
            btnAdd.Visible = false;
            btnRemove.Visible = false;
            btnModify.Visible = false;
            btnConfirm.Visible = true;
            btnCancel.Visible = true;
            cmbLanguage.Enabled = false;
            btnAddLanguageExt.Enabled = false;
            btnRemoveLanguage.Enabled = false;
            isModify = true;
            txtAbbrevation.Enabled = true;
            txtExpandesText.Enabled = true;
            ShowItem();

            if (chkColorArgument.Checked)
                AddColor();
           
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {

            DialogResult dg = MessageBox.Show("Are you sure delete select item??","Delete?", MessageBoxButtons.YesNo);


            if (dg == DialogResult.No)
            {
                lsvCodeTemplate.Select();
                return;
            }
            retrieveCurrentItem();
            if (curItem != null)
            {
                int oldIndex = curItem.Index;
                lsvCodeTemplate.Items.Remove(curItem);

                _settings.AbbrevationDictionary["." + (string)cmbLanguage.SelectedItem].Remove(curItem.Text);

                if (lsvCodeTemplate.Items.Count > 0)
                {
                    if (oldIndex != 0)
                    {
                        oldIndex--;
                        lsvCodeTemplate.Items[oldIndex].Selected = true;
                    }

                }
            }

            checkRemoved();

            lsvCodeTemplate.Select();
        }


        private void checkRemoved()
        {
            if (lsvCodeTemplate.Items.Count == 0)
            {
                btnRemove.Visible = false;
                btnModify.Visible = false;
                txtAbbrevation.Text = "";
                txtExpandesText.Text = "";
            }
        }




     

        public QuickSettings.settingAbbrevation getSettingAbbrevation()
        {
            _settings.AbbrevationDictionary = dictAbbrevations;
            _settings.ColorArgument = chkColorArgument.Checked;
            return _settings;
        }




        private bool ErrorCreateParameters()
        {
            bool error=false;

            string createParameters = "${createParameters}";

            int pos = txtExpandesText.Text.IndexOf(createParameters);

           
            if (pos ==-1) return false;



            while (pos > -1)
            {
                int startBracket = pos;
                int endBracket = pos + createParameters.Length;

                --startBracket;
                  char c=' ';

                if(startBracket>-1)
                {
                    c = txtExpandesText.Text[startBracket];

                    while (char.IsWhiteSpace(c))
                    {
                        if (startBracket == 0) break;
                        c = txtExpandesText.Text[--startBracket];
                    }
                }

                if (c != '(')
                {
                    txtExpandesText.Focus();
                    txtExpandesText.SelectionStart = pos;
                    txtExpandesText.SelectionLength = createParameters.Length;
                    return true;
                }

                int length = txtExpandesText.Text.Length;


                if (endBracket < length)
                {
                    c = txtExpandesText.Text[endBracket];

                    while (char.IsWhiteSpace(c))
                    {
                        ++endBracket;
                        if (endBracket >= length) break;
                        c = txtExpandesText.Text[endBracket];
                    }

                    if (c != ')')
                    {
                        error = true;

                    }

                }
                else
                {
                    error = true;
                }


                if (error)
                {
                    txtExpandesText.Focus();
                    txtExpandesText.SelectionStart = pos;
                    txtExpandesText.SelectionLength = createParameters.Length;
                    return true;
                }


                pos = txtExpandesText.Text.IndexOf(createParameters, pos + createParameters.Length);
            }




            return error;
        }

        /// <summary>
        /// Control compisition of the text
        /// </summary>
        /// <param name="abbr"></param>
        private void ScanAbbreviation(AbbrevationSnippet abbr)
        {
           
            MatchCollection mc = vocabularyArgument.regArguments.Matches(abbr.Snippet);
           


            if (mc.Count == 0) return;

            abbr.Arguments = new WordTypes[mc.Count];
            int index = 0;
            bool hasNeedSpace = false;
            int previousIndex = 0;

            List<int> indexSpace = new List<int>();
            foreach (Match item in mc)
            {



                IMatch match = vocabularyArgument.SearchInfoByText(item.Value, 2);

                if (match != null)
                {
                    InfoArguments info = match.Match(item.Value,0);


                    if (info != null)
                    {
                        abbr.Arguments[index] |= info.regplace;

                        switch (info.regplace)
                        {
                            case WordTypes.import:
                                abbr.HasImport = true;
                                break;
                            case WordTypes.EventHandler:
                                abbr.HasEventHandler = true;
                                break;
                            case WordTypes.AfterCurrentMember:
                                abbr.HasAfterCurrentMember = true;
                                break;
                            default:
                                break;
                        }

                        index++;

                        if (info.hasNeedSpace)
                            previousIndex = CheckNeedSpace(hasNeedSpace, previousIndex, indexSpace, item);

                        hasNeedSpace = info.hasNeedSpace;
                    }
                }
               
            }



            if (indexSpace.Count > 0)
            {
                StringBuilder sb = new StringBuilder(abbr.Snippet, abbr.Snippet.Length + indexSpace.Count);

                for (int i = indexSpace.Count - 1; i > -1; i--)
                {
                    int indexs = indexSpace[i];
                    sb.Insert(indexs, " ");
                }
                abbr.Snippet = sb.ToString();

            }
        }

        private static int CheckNeedSpace(bool hasNeedSpace, int previousIndex, List<int> indexSpace, Match item)
        {
            if (hasNeedSpace && previousIndex == item.Index)
            {
                indexSpace.Add(item.Index);
            }
            previousIndex = item.Index + item.Length;
            return previousIndex;
        }



        private void lstPlaceHolder_DoubleClick(object sender, EventArgs e)
        {
            if (!txtExpandesText.Enabled) return;

            int index = lstPlaceHolder.SelectedIndex;
            if (index < 0) return;

            InsertIntoTextEspanded(vocabularyArgument.ListArguments[index]);
        }

        private void InsertIntoTextEspanded(string strPL)
        {
            
           // int old = txtExpandesText.SelectionStart;
           // txtExpandesText.Text = txtExpandesText.Text.Insert(txtExpandesText.SelectionStart,
            txtExpandesText.Focus();
            txtExpandesText.SelectedText = strPL;
      

            if (strPL == "${\"\"}")
            {
              //  txtExpandesText.SelectionStart += strPL.Length + old - 2;
                txtExpandesText.SelectionStart  -= 2;
            }
            else if (strPL == "${var=\"\"}")
            {

                //txtExpandesText.SelectionStart += strPL.Length + old - 2;
                txtExpandesText.SelectionStart -= 2;

            }
            else if (strPL == "${var=\"\" showCmp}")
            {

                //txtExpandesText.SelectionStart += strPL.Length + old - 10;
                txtExpandesText.SelectionStart -= 10;

            }
            else if (strPL == "${var=\"\" list=\"\"}")
            {

                //txtExpandesText.SelectionStart += strPL.Length + old - 8;
                txtExpandesText.SelectionStart -= 10;

            }
            else if (strPL == "${list=\"\"}")
            {
                //txtExpandesText.SelectionStart += strPL.Length + old - 1;
                txtExpandesText.SelectionStart -= 2;
                ShowIndexCustomList();


            }
            else if (strPL == "${cmp=\"\"}")
            {
                //txtExpandesText.SelectionStart += strPL.Length + old - 2;
                txtExpandesText.SelectionStart -= 2;

            }
            else if (strPL == "${Import=\"\"}")
            {
                //txtExpandesText.SelectionStart += strPL.Length + old - 2;
                txtExpandesText.SelectionStart -= 2;
            }
            else if (strPL == "${EventHandler=\"\"}")
            {
                //txtExpandesText.SelectionStart += strPL.Length + old - 2;
                txtExpandesText.SelectionStart -= 2;
            }
            else if (strPL == "${AfterCurrentMember=\"\"}")
            {
                //txtExpandesText.SelectionStart += strPL.Length + old - 2;
                txtExpandesText.SelectionStart -= 2;
                ShowAbbreviationList();
            }
          


            
        }

        private void frmAbbrevationCompletion_Load(object sender, EventArgs e)
        {
            cmbLanguage.SelectedIndex = indexCmbLanguage;
            actLanguage = (String)cmbLanguage.SelectedItem;

            if (dictAbbrevations.ContainsKey("." + actLanguage))
            {

                Dictionary<string, AbbrevationSnippet> abbreviations = dictAbbrevations["." + actLanguage];
                DictionaryInListBox(abbreviations);
            }
            
        }

        private void DictionaryInListBox(Dictionary<string, AbbrevationSnippet> abbrevation)
        {
            foreach (KeyValuePair<string, AbbrevationSnippet> item in abbrevation)
            {
                ListViewItem lvi = new ListViewItem(item.Key);
                ListViewItem.ListViewSubItem sub = new ListViewItem.ListViewSubItem();
                sub.Text = item.Value.Snippet;
                lvi.SubItems.Add(sub);
                lsvCodeTemplate.Items.Add(lvi);
            }
        }


        public void getAbbrevationExternal(string textAbbrevation)
        {
            btnAdd.Visible = false;
            btnRemove.Visible = false;
            btnCancel.Visible = true;
            btnConfirm.Visible = true;
            txtExpandesText.Enabled = true;
            txtAbbrevation.Enabled = true;
            lsvCodeTemplate.Enabled = false;
            cmbLanguage.Enabled = false;
            btnModify.Visible = false;
            btnAddLanguageExt.Enabled = false;
            btnRemoveLanguage.Enabled = false;
            isAdd = true;
            
            txtExpandesText.Text = textAbbrevation;
            txtAbbrevation.Focus();
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (actLanguage == null) return;
            String language = (String)cmbLanguage.SelectedItem;

            if (actLanguage != language)
            {


                lsvCodeTemplate.Items.Clear();

             
                if (dictAbbrevations.ContainsKey("." + language))
                {

                    Dictionary<string, AbbrevationSnippet> abbreviations = dictAbbrevations["." +  language];
                    DictionaryInListBox(abbreviations);
                }



                actLanguage = language;
            }
           // MessageBox.Show(language);
        }

        private void btnRemoveLanguage_Click(object sender, EventArgs e)
        {
            String language = (String)cmbLanguage.SelectedItem;
            if (language == "Other" || language == "other")
            {
                return;
            }

            if (MessageBox.Show("Delete file extension and relative abbreviations??", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;


            cmbLanguage.Items.RemoveAt(cmbLanguage.SelectedIndex);
            cmbLanguage.SelectedIndex = 0;
          
            dictAbbrevations.Remove("." + language.ToLower());

        }

    

        private void btnAddLanguageExt_Click(object sender, EventArgs e)
        {
            NameExtensionForm frm = new NameExtensionForm();
            
            frm.Left = this.Left + (this.Width / 2);
            frm.Top  = this.Top  + (this.Height  / 2);
            frm.ShowDialog(this);
            if (frm.DialogResult == DialogResult.OK)
            {
                if (frm.ext().Length > 0)
                {
                    cmbLanguage.Items.Add(frm.ext());
                    dictAbbrevations.Add("." + frm.ext().ToLower(), new Dictionary<string,AbbrevationSnippet>());
                }
            }

        }

   

        private void btnFormList_Click(object sender, EventArgs e)
        {
           
            frmListCompletition form = new frmListCompletition(_settings.CustomList);
            form.ShowDialog();
            _settings.CustomList =  form.getList();
            CompletionListCustomList = null;

        }

   

   

        private void ShowAbbreviationList()
        {



            if (isActiveList)
                cst.OnCompletionDifferentInsertText -= new delegateInsertText(cst_OnCompletionInsertText);


            isActiveList = false;

            if (lsvCodeTemplate.Items.Count == 0) return;

            if (CompletionListAbbreviations == null)
            {
                CompletionListAbbreviations = new List<ICompletionListItem>();

                QuickGenerator.UI.CompletionListItem item = null;

                foreach (ListViewItem lsv in lsvCodeTemplate.Items)
                {

                    item = new CompletionListItem(lsv.Text, lsv.SubItems[1].Text ,ManagerResources.AbbreviationBitmap);
                    CompletionListAbbreviations.Add(item);
                }
            }


            isActiveList = true;
            cst.Show(CompletionListAbbreviations, true);
            
        }


        private void ShowIndexCustomList()
        {

            if (isActiveList)
                cst.OnCompletionDifferentInsertText -= new delegateInsertText(cst_OnCompletionInsertText);

            isActiveList = false;
            if (_settings.CustomList.Count == 0) return;

            if (CompletionListCustomList == null)
            {
                CompletionListCustomList = new List<ICompletionListItem>();

                QuickGenerator.UI.CompletionListItem item = null;



                foreach (KeyValuePair<string, List<string>> value in _settings.CustomList)
                {
                    string desc = "";
                    for (int i = 0; i < value.Value.Count; i++)
                    {
                        desc += value.Value[i] + "\n";
                    }

                    item = new CompletionListItem(value.Key.ToString(), desc, ManagerResources.EmptyBitmap);
                    CompletionListCustomList.Add(item);
                }


            }

            isActiveList = true;
            cst.Show(CompletionListCustomList, true);

        }


        private void frmAbbrevationCompletion_KeyDown(object sender, KeyEventArgs e)
        {


            if (!txtExpandesText.Enabled) return;



            isActiveList = false;


            if (e.Alt && e.KeyCode == Keys.Q)
            {
                if (cst.completionList.Visible)
                    cst.Hide();

                ShowAbbreviationList();
            }
            else if (e.Alt && e.KeyCode == Keys.C)
            {
                if (cst.completionList.Visible)
                    cst.Hide();

                ShowIndexCustomList();

            }
            else if (e.Alt && e.KeyCode == Keys.A)
            {
                if (cst.completionList.Visible)
                    cst.Hide();

                ShowListArguments();

            }
        }

        private void ShowListArguments()
        {

            if (isActiveList)
                cst.OnCompletionDifferentInsertText -= new delegateInsertText(cst_OnCompletionInsertText);

            isActiveList = false;

            if (CompletionListArguments == null)
            {
                CompletionListArguments = new List<ICompletionListItem>();

                QuickGenerator.UI.CompletionListItem item = null;


                for (int i = 0; i < vocabularyArgument.ListArguments.Count; i++)
                {
                    item = new CompletionListItem(vocabularyArgument.ListArguments[i],vocabularyArgument.ListDescriptionArguments[i], ManagerResources.EmptyBitmap);
                    CompletionListArguments.Add(item);
                }

              
            }
            isActiveList = true;
            cst.OnCompletionDifferentInsertText += new delegateInsertText(cst_OnCompletionInsertText);
            cst.Show(CompletionListArguments, true);
          
           // sciCst.Show(CompletionListArguments, true);
            
        }

      

        void cst_OnCompletionInsertText(string textInserted)
        {
            isActiveList = false;
            cst.OnCompletionDifferentInsertText -= new delegateInsertText(cst_OnCompletionInsertText);
            InsertIntoTextEspanded(textInserted);
        }


        
        


        private void txtExpandesText_TextChanged(object sender, EventArgs e)
        {

            if (!txtAbbrevation.Enabled) return;
            int curLine = txtExpandesText.GetLineFromCharIndex(txtExpandesText.SelectionStart);


            if (curLine == txtExpandesText.Lines.Length) return;


            MatchCollection mc = vocabularyArgument.regArguments.Matches(txtExpandesText.Lines[curLine]);
            int pos = txtExpandesText.SelectionStart;

            if (mc.Count == 0)
            {

                if (txtExpandesText.SelectionColor != System.Drawing.Color.Black)
                {
                    if(txtExpandesText.SelectionStart>0)
                    txtExpandesText.SelectionStart--;

                    
                    txtExpandesText.SelectionLength = 1;
                    txtExpandesText.SelectionColor = System.Drawing.Color.Black;
                    txtExpandesText.SelectionStart = pos;
                }
                return;
            }

            NativeMethods.SendMessage(txtExpandesText.Handle, 0xb, (IntPtr)0, IntPtr.Zero); 

            int startPos = txtExpandesText.GetFirstCharIndexFromLine(curLine);
   

            txtExpandesText.SelectionStart = startPos + mc[0].Index;
            txtExpandesText.SelectionLength = txtExpandesText.Lines[curLine].Length;

            txtExpandesText.SelectionColor = System.Drawing.Color.Black;
            
            txtExpandesText.SelectionFont = fontTxtExpanded; // new Font("Times New Roman", 10, FontStyle.Regular);
            txtExpandesText.SelectionLength = 0;

            Match item;

          

            for (int i = 0; i < mc.Count; i++)
            {
                item = mc[i];
                bool wrong = true;
                


                IMatch match = vocabularyArgument.SearchInfoByText(item.Value, 2);

                if (match != null)
                {
                  InfoArguments info =  match.Match(item.Value, 0);
                  if (info != null)
                  {
                      wrong = false;

                      txtExpandesText.SelectionStart = startPos + item.Index;
                      txtExpandesText.SelectionLength = item.Length;
                     
                      txtExpandesText.SelectionColor = System.Drawing.Color.MediumBlue;
                      txtExpandesText.SelectionFont = fontArguments;

                      if (info.index > 0)
                      {
                          txtExpandesText.SelectionStart += info.index-1;
                          txtExpandesText.SelectionLength = info.length+2;
                          txtExpandesText.SelectionColor = System.Drawing.Color.Green;
                          
                      }
                      

                  }
                }

                if (wrong)
                {
                    txtExpandesText.SelectionStart = startPos + item.Index;
                    txtExpandesText.SelectionLength = item.Length;

                    txtExpandesText.SelectionColor = System.Drawing.Color.Red;
                }

                
            }


            txtExpandesText.SelectionStart = pos;
            txtExpandesText.SelectionLength = 0;

            NativeMethods.SendMessage(txtExpandesText.Handle, 0xb, (IntPtr)1, IntPtr.Zero);
            txtExpandesText.Invalidate(); 
        }

        private void chkColorArgument_CheckedChanged(object sender, EventArgs e)
        {
            _settings.ColorArgument = chkColorArgument.Checked;

            if (_settings.ColorArgument)
            {
                this.txtExpandesText.TextChanged += new System.EventHandler(this.txtExpandesText_TextChanged);
                AddColor();
            }
            else
            {
                this.txtExpandesText.TextChanged -= new System.EventHandler(this.txtExpandesText_TextChanged);
                RemoveColor();
            }
        }


        private void AddColor()
        {

            if (txtAbbrevation.Text.Length == 0) return;
            MatchCollection mc = vocabularyArgument.regArguments.Matches(txtExpandesText.Text);

            Match item;

            txtExpandesText.SelectionStart = 0;
            txtExpandesText.SelectionLength = txtExpandesText.Text.Length;

            txtExpandesText.SelectionColor = System.Drawing.Color.Black;

            for (int i = 0; i < mc.Count; i++)
            {
                item = mc[i];
                bool wrong = true;



                IMatch match = vocabularyArgument.SearchInfoByText(item.Value, 2);

                if (match != null)
                {
                    InfoArguments info = match.Match(item.Value, 0);
                    if (info != null)
                    {
                        wrong = false;

                        txtExpandesText.SelectionStart =  item.Index;
                        txtExpandesText.SelectionLength = item.Length;

                        txtExpandesText.SelectionColor = System.Drawing.Color.MediumBlue;
                        txtExpandesText.SelectionFont = fontArguments;

                        if (info.index > 0)
                        {
                            txtExpandesText.SelectionStart += info.index - 1;
                            txtExpandesText.SelectionLength = info.length + 2;
                            txtExpandesText.SelectionColor = System.Drawing.Color.Green;

                        }


                    }
                }

                if (wrong)
                {
                    txtExpandesText.SelectionStart =  item.Index;
                    txtExpandesText.SelectionLength = item.Length;

                    txtExpandesText.SelectionColor = System.Drawing.Color.Red;
                }


            }
        }

        private void RemoveColor()
        {
            txtExpandesText.SelectionStart = 0;
            txtExpandesText.SelectionLength = txtExpandesText.Text.Length;

            txtExpandesText.SelectionColor = System.Drawing.Color.Black;
            txtExpandesText.SelectionFont = fontTxtExpanded;
        }

 
        
    }
}
