using System;
using PluginCore;
using PluginCore.Managers;
using QuickGenerator.UI.form;
using System.Drawing;

namespace QuickGenerator.Command
{
	class CreateClassCmdHaxe : ICompletionListItem, ICommandInterface
	{
		#region ICompletionListItem Membri di
		private String _className;
		private PluginMain _plugin;
		private Bitmap icon;

		public CreateClassCmdHaxe(String className, PluginMain plugin, Bitmap icon)
		{
			_className = className;
			_plugin = plugin;
			this.icon = icon;

		}

		public string Label
		{
			get { return "Create class with [" + _className + "]"; }
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
			ITabbedDocument td = ASCompletion.Context.ASContext.MainForm.CurrentDocument;
			CreateClassFrmSettings settings = _plugin.settingsQuickGenerator.createClassSettings;

			CreateClassfrm frm = new CreateClassfrm(_className, true, settings, "haxe", _plugin);
			frm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(_plugin.SaveSettingsFrmCreateClass);


			EventManager.AddEventHandler(_plugin, EventType.ProcessArgs);
			frm.ShowDialog();
			EventManager.RemoveEventHandler(_plugin);
			td.Activate();
			if (frm.package.Length == 0) return;

			ASCompletion.Model.MemberModel mm = new ASCompletion.Model.MemberModel();
			mm.Type = frm.package;
			ASCompletion.Completion.ASGenerator.InsertImport(mm, true);


		}

		#endregion
	}
}
