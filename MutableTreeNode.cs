using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WarBender.Properties;

namespace WarBender {
    class MutableTreeNode : TreeNode {
        public Mutable Mutable { get; }

        public MutableTreeNode(TreeView treeView, Mutable mutable) {
            Mutable = mutable;

            ToolTipText = mutable.TypeName;
            ImageKey = SelectedImageKey =
                treeView.ImageList.Images.ContainsKey(mutable.TypeName) ? mutable.TypeName :
                mutable.IsArray ? "array" : "record";

            if (mutable.MutableCount > 0) {
                Nodes.Add(new LoadingTreeNode(treeView, this));
            }

            ComputeText();
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Settings.Default.UseRawIds)) {
                ComputeText();
            }
        }

        void ComputeText() {
            var index = Mutable.Parent?.GetIndexString(Mutable.Index);
            if (Settings.Default.UseRawIds) {
                Text = index + Mutable.Name;
                if (Mutable.Label != null) {
                    ToolTipText = Mutable.Label;
                }
            } else {
                Text = index + (Mutable.Label ?? Mutable.Name);
                if (Mutable.Label != null) {
                    ToolTipText = Mutable.Name;
                }
            }
        }
    }

    class LoadingTreeNode : TreeNode {
        readonly TreeView treeView;
        readonly MutableTreeNode owner;
        bool isLoading;

        public LoadingTreeNode(TreeView treeView, MutableTreeNode owner)
            : base("Loading ...") {

            this.treeView = treeView;
            this.owner = owner;

            ForeColor = SystemColors.GrayText;
            ImageKey = SelectedImageKey = "loading";

            treeView.AfterExpand += TreeView_AfterExpand;
        }

        void TreeView_AfterExpand(object sender, TreeViewEventArgs e) {
            if (isLoading || e.Node != owner) {
                return;
            }
            treeView.AfterExpand -= TreeView_AfterExpand;
            isLoading = true;

            TreeNode[] childNodes;
            try {
                childNodes =
                    (from child in owner.Mutable.Children
                     let mut = child.Value as Mutable
                     where mut != null
                     select new MutableTreeNode(treeView, mut)
                    ).ToArray();
            } catch (WarbendShuttingDownException) {
                return;
            }
            owner.Nodes.AddRange(childNodes);

            Remove();
        }
    }
}
