using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using PluginCore;
using QuickGenerator.CustomCompletionList;
using QuickGenerator.QuickSettings;
using ProjectManager.Projects;
using System.ComponentModel;

namespace QuickGenerator.UI.form
{
    public partial class CreateClassfrm :baseSettings
    {

        private delegate void createFileClass();
        public delegate void InsertPackageHandleEvent(string package);
        public event InsertPackageHandleEvent OnInsertPackage;

        private createFileClass CreateFileClass;


        private CreateClassFrmSettings _settings;
        private customCompletionTextBox cst;
        private customCompletionTextBoxAndListBox compInterface;
        private Boolean dontSearchImports;
        private Boolean dontSearchInterface;
        private Boolean _isSCiContext;
        GenerateClass asc;
        private ObserverChange observerChange;
        private const int WM_KEYDOWN = 0x100;
        List<ICompletionListItem> listImports;
        

        // String
      //  private string  extensionLanguage;
        private string Package = "";
        private string extensionLanguage = "";
        private string filePath;
        public string package
        {
            get { return Package; }
        }

        #region "construct"
        public CreateClassfrm(CreateClassFrmSettings setting, PluginMain plugin)
        {

            InitWithSettings(setting);
            Project project = (Project)PluginBase.CurrentProject;
           
            if (project == null)
            {
                asc = new GenerateClass();
                SetLanguage("as3");
                return;
            }

            SetLanguage(project.Language.ToLower() );
            ScintillaNet.ScintillaControl sci = ASCompletion.Context.ASContext.CurSciControl;

            if (sci != null)
            {
                txtFilePath.Text = Path.GetDirectoryName(sci.FileName);
            }

            if (txtFilePath.Text.Length==0 )
            {
                txtFilePath.Text = project.AbsoluteClasspaths[0];
            }


            asc = new GenerateClass();
            txtNameClass.TabIndex = 0;
            
        }

        public CreateClassfrm(string className,Boolean isSCiContext, CreateClassFrmSettings setting, string language, PluginMain plugin)
        {
            _isSCiContext = isSCiContext;
            InitWithSettings(setting);
            txtNameClass.Text = className;
            txtNameClass.Enabled = false;
            btnCreateClass.TabIndex = 0;
            chkClose.Visible = false;
            btnSaveSetting.Visible = true;
            
            // Set current path 
            string path = Path.GetDirectoryName(ASCompletion.Context.ASContext.CurSciControl.FileName);
            //MessageBox.Show(path);
            txtFilePath.Text = path;


            asc = new GenerateClass();
            SetLanguage(language);

         
        }

        private void SetLanguage(string language)
        {
           
            if (language == "haxe")
            {

                CreateFileClass = new createFileClass(CreateFileClassHaxe);
                extensionLanguage = ".hx";
                this.Text = "Create Class haxe";
            }else
                {
                    extensionLanguage = ".as";
                    CreateFileClass = new createFileClass(CreateFileClassAS3);
                    this.Text = "Create Class AS3";
                }

        }


        private void CreateFileClassAS3()
        {

            asc.SaveAS3ClassFile(filePath, GenerateClassOptions());
           
        }

        private void CreateFileClassHaxe()
        {

            asc.SaveClassFileHaxe(filePath,  txtNameClass.Text, GenerateClassOptions());
          
        }


        private void InitWithSettings(CreateClassFrmSettings setting)
        {
            InitializeComponent();
            if (setting.as3ClassOption == null)
            {
                setting.as3ClassOption = new ClassSettings("as3", "", null, true, false, false, false, false, "");
            }
            this.settings = setting;
        }

        public CreateClassfrm()
        {  
            InitializeComponent();
        }
        #endregion


        
        private void saveSettings()
        {
            _settings.PathToSaveFile = txtFilePath.Text;
            _settings.closeAfterSave = chkClose.Checked;

            _settings.as3ClassOption = GenerateClassOptions();
            
        }


        /// <summary>
        /// Retrieve settings 
        /// </summary>
        public CreateClassFrmSettings settings
        {
            get
            {
                return _settings;
            }

            set
            {

                _settings = value;
                txtFilePath.Text = _settings.PathToSaveFile;
                chkClose.Checked = _settings.closeAfterSave;

                ClassSettings option = _settings.as3ClassOption;


                if (option.isPublic)
                {
                    rdbPublic.Checked = true;
                }
                else
                {
                    rdbInternal.Checked = true;
                }


                dynamicCheck.Checked = _settings.as3ClassOption.isDynamic;
                finalCheck.Checked = _settings.as3ClassOption.isFinal;

                if (_settings.as3ClassOption.superClass.Length!=0)
                {
                    txtExtends.Text = _settings.as3ClassOption.superClass;
                    constructorCheck.Enabled = true;
                    constructorCheck.Checked = _settings.as3ClassOption.createConstructor;
                }

                
                if(_settings.as3ClassOption.Interfaces!=null)
                {
                    foreach (String item in _settings.as3ClassOption.Interfaces)
                    {
                        lstInterface.Items.Add(item);
                    }
                }

                if (lstInterface.Items.Count > 0)
                {
                    superCheck.Enabled = true;
                    superCheck.Checked = _settings.as3ClassOption.createInheritedMethods;
                }

                btnSaveSetting.Enabled = false;
            }

        }


        private void CreateClassAS3frm_Load(object sender, EventArgs e)
        {
            cst = new customCompletionTextBox(txtExtends, this, true, true);
            compInterface = new customCompletionTextBoxAndListBox(txtInterface, lstInterface, this);
            compInterface.ItemAfterAdded += new listItemAdded(compInterface_ListItemAdd);


            observerChange = new ObserverChange((Control) txtExtends );
            observerChange.PutUnderObservation((Control) chkClose  );
            observerChange.PutUnderObservation((Control)superCheck);
            observerChange.PutUnderObservation((Control) constructorCheck );
            observerChange.PutUnderObservation((Control)finalCheck );
            observerChange.PutUnderObservation((Control) rdbInternal);
            observerChange.PutUnderObservation((Control) dynamicCheck);
            observerChange.PutUnderObservation((Control) rdbPublic);
            observerChange.PutUnderObservation((Control)txtFilePath);

            observerChange.UIChange += new uiChange(observerChange_UIChange);


            txtNameClass.ValidateName += new validateName(txtNameClass_ValidateName);
        }

        void txtNameClass_ValidateName()
        {
            if (txtNameClass.hasError)
            {
                errorLabel.Text = "Invalid name!!!";
                errorLabel.ForeColor = System.Drawing.Color.Red;
       
            }
            else
            {
                errorLabel.Text = "Correct!!!";
                errorLabel.ForeColor = System.Drawing.Color.Green;
            }
        }



        void observerChange_UIChange()
        {
            EnableSaveSetting();
        }

        private void EnableSaveSetting()
        {
            btnSaveSetting.Enabled = true; 
        }

      

        private void compInterface_ListItemAdd()
        {
            superCheck.Enabled = lstInterface.Items.Count > 0;
      

            observerChange.RemoveAllHandle();
            EnableSaveSetting();
        }


      

        private void button1_Click(object sender, EventArgs e)
        {

             Project project = (Project)PluginBase.CurrentProject;


             if (project == null)
             {
                 ShowBrowserExternalProject();
                 return;
             }
          
            PackageBrowserExtend pbe = new PackageBrowserExtend();
            pbe.Project = project;


          //  bool showExtBrowser=false;

            foreach (string item in project.AbsoluteClasspaths)
            {
                //if (txtFilePath.Text.IndexOf(item) == -1) showExtBrowser = true;
                pbe.AddClassPath(item);
            }


            if (pbe.ShowDialog(this) == DialogResult.OK)
            {
                if (pbe.Package != null)
                {

                    txtFilePath.Text = pbe.Package;
                    GetPackageName();
                }
               
            }

            if (pbe.externalProject)
            {
                ShowBrowserExternalProject();

            }
        }

        private void ShowBrowserExternalProject()
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.SelectedPath = txtFilePath.Text;

          //  fb.RootFolder = Environment.SpecialFolder.MyComputer;
            fb.ShowDialog();
           
            if (fb.SelectedPath.Trim().Length==0) return;
            txtFilePath.Text = fb.SelectedPath;

            if (PluginBase.CurrentProject == null) {
                
                return; 
            }

            GetPackageName();

            ProjectManager.Controls.TreeView.ProjectTreeView.Instance.RebuildTree(true);
      
        }

        private void GetPackageName()
        {


     
            string DirectoryName = Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
            string PathSource = "";

            foreach (String item in PluginBase.CurrentProject.SourcePaths)
            {
                PathSource = Path.Combine(DirectoryName, item);
                if (txtFilePath.Text.IndexOf(PathSource, 0) != -1)
                {
                    Package = txtFilePath.Text.Replace(PathSource, "");
                    if (Package.Length != 0)
                    {
                        Package = Package.Substring(1);
                        Package = Package.Replace("\\", ".");
                    }

                  
                }
            }
        }

        

        private void btnCreateClass_Click(object sender, EventArgs e)
        {
            CreateClass();
        }


        private void CreateClass()
        {
            if (txtNameClass.hasError)
            {
               if( MessageBox.Show("Class Name not valid, continue??", "Error", MessageBoxButtons.YesNo)==DialogResult.No) return;
            }

            if (!Directory.Exists(txtFilePath.Text)){ MessageBox.Show("The directory don't exist!!!"); return;}

            if (txtFilePath.Text.Trim().Length==0){ MessageBox.Show("Insert Save Path!!!"); return; }

            if (txtNameClass.Text.Trim().Length == 0){ MessageBox.Show("Insert Name Class!!!");return; }

            // Control existence file
            filePath = Path.ChangeExtension(Path.Combine(txtFilePath.Text, txtNameClass.Text), extensionLanguage);

            if(!GenerateClass.CheckFileExistence(filePath)) return;

            if (Package.Length != 0)
                Package += "." + txtNameClass.Text;
            else
                Package = txtNameClass.Text;

            if (OnInsertPackage != null)
                OnInsertPackage(Package);


            CreateFileClass();

            ProjectManager.Controls.TreeView.ProjectTreeView.Instance.RebuildTree(true);
           // PluginCore.Managers.EventManager.DispatchEvent(plugin, new NotifyEvent(EventType.FileNew));


            if (chkClose.Checked || _isSCiContext) this.Close();
        }


        private ClassSettings GenerateClassOptions()
        {
            string language = "as";
            string baseClass = txtExtends.Text.Trim();
            List<string> _interfaces = new List<string>(lstInterface.Items.Count);
            foreach (string item in lstInterface.Items)
            {
                _interfaces.Add(item);
            }


            ClassSettings as3
            = new ClassSettings(language,
            baseClass,_interfaces,rdbPublic.Checked, dynamicCheck.Checked,
            finalCheck.Checked,superCheck.Checked , constructorCheck.Checked
            , this.Package

            )
            ;

            return as3;
        }


        private void btnSaveSetting_Click(object sender, EventArgs e)
        {
            saveSettings();
            this._optionChange = true;
            btnSaveSetting.Enabled = false;
            btnCreateClass.Focus();
            
            observerChange.AddAllHandle();
           
        }



        private void CreateClassAS3frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            cst.Dispose();
            observerChange.Dispose();

            cst = null;
            observerChange = null;
        }

        private void txtInterface_Enter(object sender, EventArgs e)
        {
            if (dontSearchInterface) return;

            if (compInterface.allItems == null)
            {
                compInterface.allItems = ExplorerProject.RetrieveListInterface();
                if (compInterface.allItems == null)
                    dontSearchInterface = true;
            }
        }


     


        private void btnRemoveInterface_Click(object sender, EventArgs e)
        {
            if (lstInterface.SelectedItems.Count > 0)
            {
                foreach (int item in lstInterface.SelectedIndices)
	            {
                    lstInterface.Items.RemoveAt(item);
	            }

                superCheck.Enabled = lstInterface.Items.Count>0;  
                observerChange.PutUnderObservation((Control) chkClose  );
                
                observerChange.RemoveAllHandle();
                EnableSaveSetting();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (txtInterface.Text.Trim().Length > 0)
            {

                if (!lstInterface.Items.Contains(txtInterface.Text))
                {
                    lstInterface.Items.Add(txtInterface.Text);

                    compInterface_ListItemAdd();

                    observerChange.RemoveAllHandle();
                    EnableSaveSetting();
                }
            }
            else
            {
                txtInterface.Focus();                
            }
        }


        private void txtExtends_TextChanged_1(object sender, EventArgs e)
        {
            
            if (txtExtends.Text.Length == 0)
            {
                constructorCheck.Checked = false;
                constructorCheck.Enabled = false;
            }
            else
            {
                
                constructorCheck.Enabled = true;
            }

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(msg.Msg==WM_KEYDOWN)
            switch (keyData)
            {
               
                case Keys.Enter:

                    if (txtExtends.Focused)
                    {
                        if (!cst.completionList.Visible)
                        {
                            CreateClass();
                        }

                        break;
                    }

                    if (txtInterface.Focused)
                    {
                        if (!compInterface.completionList.Visible)
                        {
                            CreateClass();
                        }

                        break;
                    }

                    if (!btnPackage.Focused && !btnCreateClass.Focused && !btnSaveSetting.Focused && !btnAddInterface.Focused && !btnRemoveInterface.Focused)
                        CreateClass();

                    break;
                default:
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    

        private void txtExtends_Enter(object sender, EventArgs e)
        {
            if (txtExtends.Text.Length > 0)
            {
                txtExtends.SelectionStart = 0;
                txtExtends.SelectionLength = txtExtends.Text.Length;
            }
            // Dont want repeat the research
            if (dontSearchImports) return;

            if (cst.allItems == null)
            {
                cst.allItems = ExplorerProject.RetrieveListImports();
                if (cst.allItems == null)
                    dontSearchImports = true;
            }
            // No importscst.allItems = ExplorerProject.RetrieveListImports();
            if (listImports == null)
                return;

              cst.Show(listImports,false);
        }

        private void CreateClassfrm_KeyDown(object sender, KeyEventArgs e)
        {

        }


      
    }

    [Serializable]
    public struct CreateClassFrmSettings
    {
        public string PathToSaveFile;
       /// public string className;
       
        [DefaultValue(true)]
        public bool closeAfterSave;
        public ClassSettings as3ClassOption;
    }
}
