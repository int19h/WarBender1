using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using WarBender.Properties;

namespace WarBender {
    public partial class MutablePropertyGrid : UserControl, IDisposable {
        readonly ToolStripItem[] toolStripMultiItems;
        bool multiSelect, showMultiSelectTip;
        ObservableCollection<MutableTreeNode> selection;

        public MutablePropertyGrid() {
            InitializeComponent();
            Mutable.Invalidated += Mutable_Invalidated;
            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            toolStripMultiItems = toolStrip.Items.Cast<ToolStripItem>().ToArray();
            var propertyGridToolStrip = propertyGrid.Controls.OfType<ToolStrip>().Single();
            propertyGridToolStrip.AllowMerge = true;
            propertyGridToolStrip.Items[propertyGridToolStrip.Items.Count - 1].Visible = false;
            ToolStripManager.Merge(toolStripMerged, propertyGridToolStrip);
        }

        void IDisposable.Dispose() {
            Selection = null;
            Mutable.Invalidated -= Mutable_Invalidated;
            Settings.Default.PropertyChanged -= Settings_PropertyChanged;

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Form MainForm { get; set; }

        public bool MultiSelect {
            get => multiSelect;
            set {
                if (multiSelect != value) {
                    multiSelect = value;
                    OnMultiSelectChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnMultiSelectChanged(EventArgs e) {
            MultiSelectChanged?.Invoke(this, e);
            UpdateToolStrip();
        }

        public event EventHandler MultiSelectChanged;

        public bool ShowMultiSelectTip {
            get => showMultiSelectTip;
            set {
                if (showMultiSelectTip != value) {
                    showMultiSelectTip = value;
                    OnShowMultiSelectTipChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnShowMultiSelectTipChanged(EventArgs e) {
            ShowMultiSelectTipChanged?.Invoke(this, e);
            UpdateToolStrip();
        }

        public event EventHandler ShowMultiSelectTipChanged;

        internal ObservableCollection<MutableTreeNode> Selection {
            get => selection;
            set {
                if (selection != null) {
                    selection.CollectionChanged -= Selection_CollectionChanged;
                }
                selection = value;
                if (selection != null) {
                    selection.CollectionChanged += Selection_CollectionChanged;
                }
                UpdatePropertyGrid();
            }
        }

        void UpdatePropertyGrid() {
            UseWaitCursor = true;
            if (Selection != null) {
                propertyGrid.SelectedObjects = Selection.Select(n => n.Mutable).ToArray();
            } else {
                propertyGrid.SelectedObject = null;
            }
            UseWaitCursor = false;
            UpdateToolStrip();
        }

        void UpdateToolStrip() {
            var selection = Selection?.ToArray() ?? new MutableTreeNode[0];
            var isEmpty = selection.Length == 0;
            labelTip.Visible = showMultiSelectTip && isEmpty;
            buttonClearSelection.Visible = !isEmpty;
            toolStrip.Visible = MultiSelect;

            var buttons = new List<ToolStripItem>();
            foreach (var node in selection) {
                var mutable = node.Mutable;
                var treeView = node.TreeView;

                var text = (Settings.Default.UseRawIds ? null : mutable.Label) ?? mutable.Name ?? mutable.Path;
                var button = new ToolStripButton(text, treeView.ImageList.Images[node.ImageKey]);
                button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                button.ToolTipText = mutable.Path;
                button.Click += delegate {
                    treeView.SelectedNode = node;
                };
                button.MouseDown += (sender, e) => {
                    switch (e.Button) {
                        case MouseButtons.Middle:
                            Selection.Remove(node);
                            break;
                        case MouseButtons.Right:
                            treeView.SelectedNode = node;
                            break;
                        case MouseButtons.Left:
                            if (DoDragDrop(node, DragDropEffects.Copy | DragDropEffects.Move) == DragDropEffects.Move) {
                                Selection.Remove(node);
                            }
                            break;
                    }
                };
                buttons.Add(button);
            }

            toolStrip.Items.Clear();
            toolStrip.Items.AddRange(toolStripMultiItems);
            toolStrip.Items.AddRange(buttons.ToArray());
        }

        void Selection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            UpdatePropertyGrid();
        }

        void buttonClearSelection_Click(object sender, EventArgs e) {
            Selection.Clear();
        }

        void buttonPopOut_Click(object sender, EventArgs e) {
            new PropertyGridForm(Selection).Show(MainForm);
        }

        void MutablePropertyGrid_DragEnter(object sender, DragEventArgs e) {
            if (!e.AllowedEffect.HasFlag(DragDropEffects.Copy) &&
                !e.AllowedEffect.HasFlag(DragDropEffects.Move)) {
                return;
            }

            var node = (MutableTreeNode)e.Data.GetData(typeof(MutableTreeNode));
            if (node == null) {
                return;
            }

            e.Effect = e.AllowedEffect;
        }

        void MutablePropertyGrid_DragOver(object sender, DragEventArgs e) {
            const int ShiftKeyState = 4;
            if (e.AllowedEffect.HasFlag(DragDropEffects.Move) && (e.KeyState & ShiftKeyState) != 0) {
                e.Effect = DragDropEffects.Move;
            } else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        void MutablePropertyGrid_DragDrop(object sender, DragEventArgs e) {
            var node = (MutableTreeNode)e.Data.GetData(typeof(MutableTreeNode));
            if (node != null) {
                if (!MultiSelect) {
                    Selection.Clear();
                }
                if (!Selection.Contains(node)) {
                    Selection.Add(node);
                }
            }
        }

        void Mutable_Invalidated(object sender, ISet<Mutable> invalidated) {
            BeginInvoke((Action)delegate {
                if (Selection.Any(node => invalidated.Contains(node.Mutable))) {
                    propertyGrid.Refresh();
                }
            });
        }

        void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Settings.Default.UseRawIds)) {
                propertyGrid.Refresh();
                UpdateToolStrip();
            }
        }
    }
}

