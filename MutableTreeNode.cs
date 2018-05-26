using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WarBender {
    enum SyntheticNodeTag {
        LoadingHidden,
        LoadingVisible,
    }

    class MutableTreeNode : TreeNode {
        public Mutable Mutable { get; }

        public MutableTreeNode(TreeView treeView, string name, Mutable mutable)
            : base(name) {
            Mutable = mutable;

            ToolTipText = mutable.TypeName;
            ImageKey = SelectedImageKey =
                treeView.ImageList.Images.ContainsKey(mutable.TypeName) ? mutable.TypeName :
                mutable.IsArray ? "array" : "record";

            if (mutable.MutableCount > 0) {
                Nodes.Add(new LoadingTreeNode(treeView, this));
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
                     select new MutableTreeNode(treeView, child.Name, mut)
                    ).ToArray();
            } catch (WarbendShuttingDownException) {
                return;
            }
            owner.Nodes.AddRange(childNodes);

            Remove();
        }
    }
}
