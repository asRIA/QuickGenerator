using System.Windows.Forms;

using ProjectManager.Controls.TreeView;

namespace ASClassWizard.Controls.TreeView
{
    class SimpleDirectoryNode : GenericNode
    {
        public bool dirty;
        public string directoryPath;

        public SimpleDirectoryNode(string directory, string path) : base(directory)
        {
            this.dirty = true;
            this.directoryPath = path;
            this.Nodes.Add(new TreeNode(""));
        }
    }
}
