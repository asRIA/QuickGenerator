using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickGenerator.UI.form;
using QuickGenerator.QuickSettings;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using QuickGenerator.Abbreviation;

namespace QuickGenerator
{

    public delegate void ChangeClipBoardRingSettingsEventHanlder();
    public delegate void ChangeAbbrevationSettingsEventHanlder();
    public delegate void ChangeDoubleCharSettingsEventHanlder();
    public delegate void ChangeAutoCloseFunctionEventHanlder();
    public delegate void ChangeFormatterCodeSettingsEventHanlder();
    public delegate bool ChangeShortCutEventHanlder(settingsComponent nameComponent, Keys oldKeys, Keys newKeys, bool disable);
    
    

    public enum settingsComponent
    { 
        Abbrevation,
        FormatterCode,
        ClipBoardRing,
        DoubleChar,
        CreateClass
    }

    [Serializable]
    public class Settings 
    {

        private CreateClassFrmSettings _settingCreateClass;

        // Event
       
        public event ChangeClipBoardRingSettingsEventHanlder ChangeClipBoardRingSettings;
        public event ChangeAbbrevationSettingsEventHanlder ChangeAbbrevationSettings;
        public event ChangeDoubleCharSettingsEventHanlder ChangeDoubleCharSettings;
        public event ChangeFormatterCodeSettingsEventHanlder ChangeFormatterCodeSettings;
        public event ChangeShortCutEventHanlder ChangeShortCut;
        public event ChangeAutoCloseFunctionEventHanlder ChangeFunctionAutoClose;

        public Settings()
        {
            _settingCreateClass.closeAfterSave = true;
        }

        #region "Create Class AS"
        [Description("Settings for create class form"), Browsable(false)]
        public CreateClassFrmSettings createClassSettings
        {
            get { return this._settingCreateClass; }
            set
            {
                
                this._settingCreateClass = value;
            }
        }

        private Keys _CreateClassShortCut;
        [DisplayName("Show Create Class"), Category("Create Class Shortcuts")]
        public Keys CreateClassShortCut
        {
            get { return _CreateClassShortCut; }
            set
            {
               

                _CreateClassShortCut = value;
                if (ChangeShortCut(settingsComponent.CreateClass, _CreateClassShortCut, value, true))
                    _CreateClassShortCut = Keys.None;
           
            }
        }

        private Keys _CreateClassFromNameShortCut;
        [DisplayName("Show Create Class from word"), Category("Create Class Shortcuts")]
        public Keys CreateClassFromNameShortCut
        {
            get { return _CreateClassFromNameShortCut; }
            set
            {

                _CreateClassFromNameShortCut = value;

                if (ChangeShortCut(settingsComponent.CreateClass, _CreateClassFromNameShortCut, value, true))
                    _CreateClassFromNameShortCut = Keys.None;
              

            }
        }

        #endregion



        #region "Abbreviation"


        internal Dictionary<string, Dictionary<string, AbbrevationSnippet>> abbrevationDictList;
        private Dictionary<string, List<string>> list;

        internal Dictionary<string, List<string>> customList
        {
            get{return list;}
            set{list = value;}
        }

        [NonSerialized]
        settingAbbrevation settAbbr;

       [Description("List of Abbreviations"), Category("Abbreviation  Settings"),
       Editor(typeof(ListAbbrevationEditor), typeof(UITypeEditor))]
        public settingAbbrevation Abbreviations
        {
            get
            {

                if (settAbbr == null)
                {
                    settAbbr = new settingAbbrevation();
                    settAbbr.AbbrevationDictionary =abbrevationDictList;
                    settAbbr.CustomList = list;
                    settAbbr.ColorArgument = ColorArgument;
                }

                  abbrevationDictList =settAbbr.AbbrevationDictionary;
                  list = settAbbr.CustomList;
                  ColorArgument = settAbbr.ColorArgument;
                return  settAbbr;
            }
            set
            {
                
                settAbbr = value;
                abbrevationDictList = settAbbr.AbbrevationDictionary;
                list = settAbbr.CustomList;
                ColorArgument = settAbbr.ColorArgument;
            }
        }



        private bool _AbbrevationEnabled;
        [DisplayName("Enable"), Category("Abbreviation  Settings")]
        public bool AbbrevationEnabled
        {
            get { return _AbbrevationEnabled; }
            set
            {

                _AbbrevationEnabled = value;

                if (ChangeAbbrevationSettings != null)
                    ChangeAbbrevationSettings();

            }
        }

        internal  bool ColorArgument;
        internal System.Drawing.Color _highLightColor;
        [DisplayName("Sensible Area Color"), Category("Abbreviation  Settings"), Description("Color highlight")]
        public System.Drawing.Color HighLightColor
        {
            get { return _highLightColor; }
            set
            {

                _highLightColor = value;

            }
        }


        internal bool _showHighLight;
        [DisplayName("Show Words with a color"), Category("Abbreviation  Settings"), Description("Color the words")]
        public bool ShowHighLight
        {
            get { return _showHighLight; }
            set
            {

                _showHighLight = value;

            }
        }


        internal bool _AutomaticNextWordWithSemiColon;
        [DisplayName("Automatic Next Word with <;>"), Category("Abbreviation  Settings"), Description("After typing <;> if there is a next word the cursor go to it.")]
        public bool AutomaticNextWordWithSemiColon
        {
            get { return _AutomaticNextWordWithSemiColon; }
            set
            {

                _AutomaticNextWordWithSemiColon = value;

            }
        }

        internal   Keys abbrevationShortCut;
        [DisplayName("Expand test and move cursor"), Category("Abbreviation Shortcuts") ,Description("Expand text of a abbreviation and after it allow to move in the sensible aree of the expanded text.")]
        public Keys AbbrevationShortCut
        {
            get { return abbrevationShortCut; }
            set
            {
                
                if(ChangeShortCut(settingsComponent.Abbrevation, abbrevationShortCut, value, false))
                    value = Keys.None;

                abbrevationShortCut = value;
            }


        }


        internal Keys generateSensibleAreaShortCut;
        [DisplayName("Generate sensible area"), Category("Abbreviation Shortcuts"), Description("Generate sensible area for the paramaters in the current function.It don't move the cursor after generate!!")]
        public Keys GenerateSensibleAreaShortCut
        {
            get { return generateSensibleAreaShortCut; }
            set
            {

                if (ChangeShortCut(settingsComponent.Abbrevation, generateSensibleAreaShortCut, value, false))
                    value = Keys.None;

                generateSensibleAreaShortCut = value;
            }


        }



        internal Keys gotoAbbreviationShortCut;
        [DisplayName("Go to sensible area"), Category("Abbreviation Shortcuts"), Description("Switch to document where there is a sensible area.")]
        public Keys GotoAbbreviationShortCut
        {
            get { return gotoAbbreviationShortCut; }
            set
            {
                if (ChangeShortCut(settingsComponent.Abbrevation, gotoAbbreviationShortCut, value, false))
                    value = Keys.None;
                gotoAbbreviationShortCut = value;

             
            }
        }


        internal Keys abbrevationPlusFormatterShortCut;
        [DisplayName("Reformat code"), Category("Abbreviation Shortcuts"), Description("Combine Reformat code with Abbreviations. It work only with Reformat Code enable")]
        public Keys AbbrevationPlusFormatterShortCut
        {
            get { return abbrevationPlusFormatterShortCut; }
            set
            {
                if (ChangeShortCut(settingsComponent.Abbrevation, abbrevationPlusFormatterShortCut, value, false))
                    value = Keys.None;
                abbrevationPlusFormatterShortCut = value;

                if (value != Keys.None && !_FormatterCodeEnabled)
                {
                    if (MessageBox.Show("Reformat code is not active!!  Do you want activate it?", "Reformat code disabled", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        FormatterCodeEnabled = true;
                    }
                }
            }
        }

        #endregion

        #region "Formatter Code"
        private bool _FormatterCodeEnabled;
        [DisplayName("Enable"), Category("Reformat Code Settings")]
        public bool FormatterCodeEnabled
        {
            get { return _FormatterCodeEnabled; }
            set
            {

                _FormatterCodeEnabled = value;

                if (ChangeFormatterCodeSettings != null)
                    ChangeFormatterCodeSettings();
            }
        }


        private int baseNumber;
        [DisplayName("Base Number"), Category("Reformat Code Settings") ,Description("Base Number for §")]
        public int BaseNumber
        {
            get { return baseNumber; }
            set
            {

                baseNumber = value;

            }
        }



        internal bool _breakInLines;
        [DisplayName("Break In Lines"), Category("Reformat Code Settings"), Description("when encounter ; add a new line")]
        public bool BreakInLines
        {
            get { return _breakInLines; }
            set
            {
                _breakInLines = value;
            }
        }

        internal Keys formatterCodeShortCut;
        [DisplayName("Elaborate code"), Category("Reformat Code Shortcuts")]
        public Keys FormatterCodeShortCut
        {
            get { return formatterCodeShortCut; }
            set
            {
                if (ChangeShortCut(settingsComponent.FormatterCode, formatterCodeShortCut, value, false))
                    value = Keys.None;
                formatterCodeShortCut = value;
            }
        }
        #endregion

        #region ClipBoardRing

        private bool _enableClipBoardRing;
        [DisplayName("Enable"), Category("Clipboard Ring Settings")]
        public bool EnableClipBoardRing
        {
            get { return _enableClipBoardRing; }
            set
            {

                _enableClipBoardRing = value;

                if (ChangeClipBoardRingSettings != null)
                    ChangeClipBoardRingSettings();
            }
        }


        internal int capacityClipBoardRing;
        [DisplayName("Capacity"), Category("Clipboard Ring Settings"), DefaultValue(3)]
        public int CapacityClipBoardRing
        {
            get { return capacityClipBoardRing; }
            set
            {
                if (value < 3)
                    value = 3;
                else if (value > 50)
                    value = 50;

                capacityClipBoardRing = value;
                if (ChangeClipBoardRingSettings != null)
                    ChangeClipBoardRingSettings();
            }
        }


        internal Keys fowardShortCut;

        /// <summary>
        /// Show the next item of the clip board ring
        /// </summary>
        [DisplayName("Show next item"), Category("Clipboard Ring Shortcuts"), Description("Shortcut for the Next item in the ClipBoard")]
        public Keys FowardShortCut
        {
            get { return fowardShortCut; }
            set
            {

                if (ChangeShortCut(settingsComponent.ClipBoardRing, fowardShortCut, value, false))
                    value = Keys.None;

                fowardShortCut = value;
            }
        }

        internal Keys previousShortCut;

        /// <summary>
        /// Show the previous item of the clip board ring
        /// </summary>
        [DisplayName("Show previous item"), Category("Clipboard Ring Shortcuts"), Description("ShortCut for the Previous Item in the ClipboardRing")]
        public Keys PreviousShortCut
        {
            get { return previousShortCut; }
            set
            {

                if (ChangeShortCut(settingsComponent.ClipBoardRing, previousShortCut, value, false))
                    value = Keys.None;
                
                previousShortCut = value;
            }
        }

        internal Keys showClipBoardRingShortCut;

        /// <summary>
        /// Show the previous item of the clip board ring
        /// </summary>
        [DisplayName("Show Content of the ClipBoardRing"), Category("Clipboard Ring Shortcuts"), Description("Show content ClipBoardRing")]
        public Keys ShowClipBoardRingShortCut
        {
            get { return showClipBoardRingShortCut; }
            set
            {

                if (ChangeShortCut(settingsComponent.ClipBoardRing, showClipBoardRingShortCut, value, false))
                    value = Keys.None;

                showClipBoardRingShortCut = value;
            }
        }
        #endregion

        #region "AutoClose"
        private bool _doubleCharEnabled;
        [DisplayName("Enable"), Category("Auto Closing") ,Description("The characters \"[( are auto close when they are typing")]
        public bool DoubleCharEnabled
        {
            get { return _doubleCharEnabled; }
            set
            {

                _doubleCharEnabled = value;

                if (ChangeDoubleCharSettings != null)
                    ChangeDoubleCharSettings();
            }
        }


        private bool _createParameters;
        [DisplayName("Create Parameters"), Category("Auto Closing"), Description("Generate paramaters.This depend from Abbreviation enable and 'Function And New Close' enable.")]
        public bool CreateParameters
        {
            get { return _createParameters; }
            set
            {

                _createParameters = value;

                if (ChangeFunctionAutoClose != null)
                    ChangeFunctionAutoClose();

                if (value == true && !_doubleCharEnabled)
                {
                    if (MessageBox.Show("'AutoClose' is not active!!  Do you want activate it?", "Setting required", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        DoubleCharEnabled = true;
                    }
                }


                if (value == true && !_closeFunctionAndNew)
                {
                    if (MessageBox.Show("'Function And New Close' is not active!!  Do you want activate it?", "Setting required", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                       CloseFunctionAndNew  = true;
                    }
                }


                if (value == true && !_AbbrevationEnabled)
                {


                    if (MessageBox.Show("Abbreviation code is not active!!  Do you want activate it?", "Setting required", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            AbbrevationEnabled = true;
                        }
                   

                }

            }
        }


        private bool _closeFunctionAndNew;
        [DisplayName("Function And New Close"), Category("Auto Closing"), Description("Add () to function or type precede from word new")]
        public bool CloseFunctionAndNew
        {
            get { return _closeFunctionAndNew; }
            set
            {

                _closeFunctionAndNew = value;

                if (ChangeFunctionAutoClose != null)
                    ChangeFunctionAutoClose();
            }
        }


        private  bool _ShowStateChange;
        [DisplayName("Show State"), Category("Auto Closing"), Description("When switch Enable show a dialog!!")]
        public bool  ShowStateChange
        {
            get { return _ShowStateChange; }
            set
            {

                _ShowStateChange = value;
            }
        }
        

        internal Keys switchEnableOrDisableDoubleCharShortCut;
        [DisplayName("Switch enable state"), Category("Auto Closing Shortcuts"), Description("Temporary Disable or Enable Auto Closing\nThis ShorCut work only if ide start with Auto Closing enabled on true or when it is set on true ")]
        public Keys SwitchEnableOrDisableDoubleCharShortCut
        {
            get { return switchEnableOrDisableDoubleCharShortCut; }
            set
            {
                if (ChangeShortCut(settingsComponent.DoubleChar, switchEnableOrDisableDoubleCharShortCut, value, false))
                    value = Keys.None;
                switchEnableOrDisableDoubleCharShortCut = value;
            }
        }


        #endregion

     
      
        class ListAbbrevationEditor : UITypeEditor
        {
          

            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }


            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService iwfse = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (iwfse == null) return null;


                settingAbbrevation sta = (settingAbbrevation)value;

                if (sta == null)
                    sta = new settingAbbrevation();
                //QuickGenerator.se

                //frmAbbrevationCompletion form = new frmAbbrevationCompletion((Dictionary<string, Dictionary<string, string>>)value);
                AbbrevationCompletionForm form = new AbbrevationCompletionForm(sta, null);
                form.ShowDialog();
               // value = form.getDictAbbrevations();
               // sta._abbrevationDictList = form.getDictAbbrevations();

                value = form.getSettingAbbrevation();
                return base.EditValue(context, provider, value);
            }
        }

    }

    

}
