using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WarBender.Properties;

namespace WarBender {
    public partial class MainForm : Form {
        readonly WarbendService warbend;
        readonly Progress<string> status;
        readonly Progress<int> progress;
        readonly ObservableCollection<MutableTreeNode> selection = new ObservableCollection<MutableTreeNode>();
        string initialTitle;

        public MainForm() {
            InitializeComponent();
            initialTitle = Text;

            buttonLoad.DefaultItem = buttonLoadBinary;
            buttonSave.DefaultItem = buttonSaveBinary;

            propertyGrid.MainForm = this;
            propertyGrid.Selection = selection;
            selection.CollectionChanged += Selection_CollectionChanged;

            openFileDialog.InitialDirectory = saveFileDialog.InitialDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Mount&Blade Warband Savegames");

            status = new Progress<string>(StatusChanged);
            progress = new Progress<int>(ProgressChanged);
            warbend = new WarbendService(status, progress);
            warbend.EnsureStarted();
            warbend.Console.VisibleChanged += Console_VisibleChanged;
        }

        void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            warbend.Shutdown();
        }

        void EnableUI(bool enable) {
            buttonLoad.Enabled = enable;
            buttonSave.Enabled = enable;
            splitContainer.Enabled = enable;
            UseWaitCursor = !enable;
        }

        static readonly Dictionary<string, string> statusLabels = new Dictionary<string, string>() {
            [""] = "",
            ["load"] = "Loading",
            ["save"] = "Saving",
            ["validate"] = "Validating",
        };

        void StatusChanged(string status) {
            if (IsDisposed) {
                return;
            }
            progressBar.Visible = statusLabel.Visible = (status != "");
            progressBar.Value = 0;
            statusLabel.Text = statusLabels[status];
        }

        void ProgressChanged(int percent) {
            if (IsDisposed) {
                return;
            }
            progressBar.Value = percent;
        }

        public async Task LoadAsync(string fileName, string format = "binary") {
            foreach (var form in OwnedForms) {
                if (form is PropertyGridForm) {
                    form.Close();
                }
            }
            selection.Clear();

            EnableUI(false);
            bool loaded = false;
            try {
                Text = Path.GetFileName(fileName) + " - " + initialTitle;
                treeView.BeginUpdate();
                treeView.Nodes.Clear();
                try {
                    await warbend.LoadAsync(fileName, format);
                } catch (WarbendOperationException ex) {
                    var msg = $"Error loading \"{fileName}\":\r\n\r\n{ex.Message}";
                    MessageBox.Show(this, msg, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                loaded = true;

                var game = new Mutable(warbend);
                var rootNode = new MutableTreeNode(treeView, game);
                treeView.Nodes.Add(rootNode);
                treeView.EndUpdate();
                rootNode.Expand();
            } catch (WarbendShuttingDownException) {
                loaded = false;
            } finally {
                EnableUI(true);
                if (!loaded) {
                    Text = initialTitle;
                }
            }
        }

        public async Task SaveAsync(string fileName, string format = "binary") {
            EnableUI(false);
            try {
                var tempFileName = Path.GetTempFileName();
                try {
                    await warbend.SaveAsync(tempFileName, format);
                } catch (WarbendOperationException ex) {
                    var msg = $"Error saving \"{fileName}\":\r\n\r\n{ex.Message}";
                    MessageBox.Show(this, msg, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string backupFileName = null;
                if (File.Exists(fileName)) {
                    while (true) {
                        backupFileName = fileName + "~" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        try {
                            File.Move(fileName, backupFileName);
                        } catch (Exception ex) {
                            var msg =
                                $"Couldn't back up \"{fileName}\":\r\n\r\n{ex.Message}\r\n\r\n" +
                                $"Try again, or ignore the problem and overwrite it?";
                            var res = MessageBox.Show(this, msg, null, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                            switch (res) {
                                case DialogResult.Abort:
                                    return;
                                case DialogResult.Retry:
                                    continue;
                            }
                        }
                        break;
                    }
                }

                try {
                    File.Move(tempFileName, fileName);
                } catch (Exception ex) {
                    var msg = $"Error writing to \"{fileName}\":\r\n\r\n{ex.Message}";
                    if (backupFileName != null) {
                        msg += $"\r\n\r\nYour original save was backed up to \"{backupFileName}\"";
                    }
                    MessageBox.Show(this, msg, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } catch (WarbendShuttingDownException) {
                return;
            } finally {
                EnableUI(true);
            }
        }

        void PrepareFileDialog(FileDialog dlg, string format) {
            switch (format) {
                case "binary":
                    dlg.DefaultExt = "sav";
                    dlg.Filter = "Mount & Blade saved games (sg??.sav)|sg??.sav|All files(*.*)|*.*";
                    break;
                case "xml":
                    dlg.DefaultExt = "xml";
                    dlg.Filter = "XML files (*.xml)|*.xml|All files(*.*)|*.*";
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        async void PickAndLoad(string format) {
            PrepareFileDialog(openFileDialog, format);
            if (openFileDialog.ShowDialog() != DialogResult.OK) {
                return;
            }
            await LoadAsync(openFileDialog.FileName, format);
        }

        async void PickAndSave(string format) {
            PrepareFileDialog(saveFileDialog, format);
            if (saveFileDialog.ShowDialog() != DialogResult.OK) {
                return;
            }
            await SaveAsync(saveFileDialog.FileName, format);
        }

        void buttonLoadBinary_Click(object sender, EventArgs e) =>
            PickAndLoad("binary");

        void buttonLoadXml_Click(object sender, EventArgs e) =>
            PickAndLoad("xml");

        void buttonSaveBinary_Click(object sender, EventArgs e) =>
            PickAndSave("binary");

        void buttonSaveXml_Click(object sender, EventArgs e) =>
            PickAndSave("xml");

        MutableTreeNode[] oldSelection = new MutableTreeNode[0];

        void Selection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            var deselected = oldSelection.Except(selection).ToArray();
            foreach (var node in deselected) {
                node.Checked = false;
            }

            var selected = selection.Except(oldSelection).ToArray();
            foreach (var node in selected) {
                node.Checked = true;
            }

            oldSelection = selection.ToArray();
        }

        void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
            if (!buttonMulti.Checked && e.Node is MutableTreeNode node) { 
                selection.Clear();
                selection.Add(node);
            }
        }

        void treeView_BeforeCheck(object sender, TreeViewCancelEventArgs e) {
            e.Cancel = !(e.Node is MutableTreeNode);
        }

        void treeView_AfterCheck(object sender, TreeViewEventArgs e) {
            var node = (MutableTreeNode)e.Node;
            if (node.Checked) {
                if (!selection.Contains(node)) {
                    selection.Add(node);
                }
            } else {
                if (selection.Contains(node)) {
                    selection.Remove(node);
                }
            }
        }

        void treeView_ItemDrag(object sender, ItemDragEventArgs e) {
            DoDragDrop(e.Item, DragDropEffects.Copy);
        }

        void buttonMulti_CheckedChanged(object sender, EventArgs e) {
            var multiSelect = buttonMulti.Checked;

            treeView.BeginUpdate();
            treeView.CheckBoxes = multiSelect;
            treeView.EndUpdate();

            propertyGrid.MultiSelect = multiSelect;
            selection.Clear();
            if (!multiSelect && treeView.SelectedNode is MutableTreeNode node) {
                selection.Add(node);
            }
        }

        void ResizeProgressBar() {
            int width = statusStrip.ClientSize.Width + progressBar.Width - statusStrip.SizeGripBounds.Width;
            foreach (ToolStripItem child in statusStrip.Items) {
                if (child.Visible) {
                    width -= child.Width;
                    width -= child.Margin.Horizontal;
                }
            }
            progressBar.Width = width;
        }

        void statusStrip_ClientSizeChanged(object sender, EventArgs e) {
            ResizeProgressBar();
        }

        void statusLabel_TextChanged(object sender, EventArgs e) {
            ResizeProgressBar();
        }

        bool consoleChanging = false;

        void buttonConsole_CheckedChanged(object sender, EventArgs e) {
            if (warbend.Console.IsDisposed || consoleChanging) {
                return;
            }
            consoleChanging = true;
            warbend.Console.MakeVisible(buttonConsole.Checked);
            consoleChanging = false;
        }

        void Console_VisibleChanged(object sender, EventArgs e) {
            if (consoleChanging) {
                return;
            }
            consoleChanging = true;

            bool visible = warbend.Console.Visible;
            IAsyncResult ar = null;
            Action update = delegate {
                if (buttonConsole.Checked != visible) {
                    buttonConsole.Checked = visible;
                }
                consoleChanging = false;
            };

            try {
                ar = BeginInvoke(update);
            } catch (InvalidOperationException) {
            }
        }

        void contextMenuTree_Opening(object sender, CancelEventArgs e) {
            menuItemShowRawIds.Checked = Settings.Default.UseRawIds;
        }

        void menuItemShowRawIds_CheckedChanged(object sender, EventArgs e) {
            if (Settings.Default.UseRawIds != menuItemShowRawIds.Checked) {
                treeView.BeginUpdate();
                Settings.Default.UseRawIds = menuItemShowRawIds.Checked;
                treeView.EndUpdate();
            }
        }
    }
}
