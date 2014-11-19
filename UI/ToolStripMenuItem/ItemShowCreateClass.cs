using System;
using System.Windows.Forms;
using QuickGenerator.UI.form;
using System.Drawing;
using System.Reflection;

namespace QuickGenerator.UI.MenuItem
{
    class ItemShowCreateClass : ToolStripMenuItem
    {
        QuickGenerator.PluginMain _plugin;

        public ItemShowCreateClass(QuickGenerator.PluginMain plugin):base("Class")
        {
            
            this.ToolTipText = "Generate Actionscript file Class";
            this.Click += new EventHandler(ItemShowCreateClass_Click);
            _plugin = plugin;


            Assembly assembly = Assembly.GetExecutingAssembly();
            // QuickGenerator 
            this.Image = new Bitmap(assembly.GetManifestResourceStream("QuickGenerator.Resources.ClassLarge.png"));

        }

        void ItemShowCreateClass_Click(object sender, EventArgs e)
        {
            CreateClassFrmSettings settings = _plugin.settingsQuickGenerator.createClassSettings;
            CreateClassfrm frm = new CreateClassfrm(settings, _plugin);
            frm.FormClosed += new FormClosedEventHandler(saveSettingsFrmCreateClass);
            frm.ShowDialog();
        }

        void saveSettingsFrmCreateClass(object sender, FormClosedEventArgs e)
        {
            CreateClassfrm frm = (CreateClassfrm)sender;

            if (frm.OptionChange)
            {
                _plugin.settingsQuickGenerator.createClassSettings = frm.settings;
               _plugin.SaveSettings();
            }
        }


    }
}
