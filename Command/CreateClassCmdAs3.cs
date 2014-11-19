using System;
using PluginCore;
using QuickGenerator.UI.form;
using System.Drawing;
using ASCompletion.Context;

namespace QuickGenerator.Command
{
    class CreateClassCmdAS3 : ICompletionListItem, ICommandInterface   
    {
        #region ICompletionListItem Membri di
        private String _className;
        private PluginMain _plugin;
        private Bitmap icon;

        public CreateClassCmdAS3(String className, PluginMain plugin, Bitmap icon)
        {
            _className = className;
            _plugin = plugin;

            this.icon = icon;
        }

        public string Label
        {
            get { return "Create class with " + _className; }
        }

        public string Value
        {
            get { return "Create class "; }
        }

        public string Description
        {
            get { return "Create a class "; }
        }

        public System.Drawing.Bitmap Icon
        {
            get { return icon; }
        }

        #endregion

        #region ICommandInterface Membri di

        public void Execute()
        {
            if (_plugin == null) return;
       
            CreateClassFrmSettings settings = _plugin.settingsQuickGenerator.createClassSettings;


            CreateClassfrm frm = new CreateClassfrm(_className ,true, settings , "as3", _plugin);
            frm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(_plugin.SaveSettingsFrmCreateClass);
            frm.OnInsertPackage += new CreateClassfrm.InsertPackageHandleEvent(frm_OnInsertPackage);
            frm.ShowDialog();

            ITabbedDocument newTabbedDocument = ASContext.MainForm.CurrentDocument;

            newTabbedDocument.Activate();

            frm.FormClosed -= new System.Windows.Forms.FormClosedEventHandler(_plugin.SaveSettingsFrmCreateClass);
            frm.OnInsertPackage -= new CreateClassfrm.InsertPackageHandleEvent(frm_OnInsertPackage);

          
        }

        void frm_OnInsertPackage(string package)
        {
            if (package.Length == 0) return;


            ASCompletion.Model.MemberList ml = ASContext.Context.CurrentClass.InFile.Imports;


            bool findImport = false;
            foreach (ASCompletion.Model.MemberModel item in ml)
            {
                if (item.Name == package)
                {
                    findImport = true;
                    break;
                }
            }

            if (findImport) return;
            ASCompletion.Model.MemberModel mm = new ASCompletion.Model.MemberModel();
            mm.Type = package;
            ASCompletion.Completion.ASGenerator.InsertImport(mm, true);
        }

        #endregion
    }
}
