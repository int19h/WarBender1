using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using WarBender.Properties;

namespace WarBender {
    public partial class PropertyGridForm : Form {
        internal ObservableCollection<MutableTreeNode> Selection { get; }

        public PropertyGridForm() 
            : this(new MutableTreeNode[0]) {
        }

        internal PropertyGridForm(IEnumerable<MutableTreeNode> selection) {
            InitializeComponent();

            Selection = new ObservableCollection<MutableTreeNode>(selection);
            propertyGrid.Selection = Selection;
            Selection.CollectionChanged += Selection_CollectionChanged;

            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            UpdateTitle();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            Selection.CollectionChanged -= Selection_CollectionChanged;
            Settings.Default.PropertyChanged -= Settings_PropertyChanged;
        }

        protected override void OnShown(EventArgs e) {
            propertyGrid.MainForm = Owner;
            base.OnShown(e);
        }

        void UpdateTitle() {
            if (Selection.Count == 0) {
                Text = "No selection";
            } else if (Selection.Count == 1) {
                var mutable = Selection[0].Mutable;
                Text = (Settings.Default.UseRawIds ? null : mutable.Label) ?? mutable.Name ?? mutable.Path;
            } else {
                Text = string.Format("{0} objects", Selection.Count);
            }
        }

        void Selection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            UpdateTitle();
        }

        void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            UpdateTitle();
        }
    }
}
