namespace WarBender {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.buttonMulti = new System.Windows.Forms.ToolStripButton();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.buttonLoad = new System.Windows.Forms.ToolStripSplitButton();
            this.buttonLoadBinary = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonLoadXml = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonSave = new System.Windows.Forms.ToolStripSplitButton();
            this.buttonSaveBinary = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonSaveXml = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonConsole = new System.Windows.Forms.ToolStripButton();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.propertyGrid = new WarBender.MutablePropertyGrid();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemShowRawIds = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.contextMenuTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonMulti,
            this.statusLabel,
            this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 485);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.ShowItemToolTips = true;
            this.statusStrip.Size = new System.Drawing.Size(734, 26);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.ClientSizeChanged += new System.EventHandler(this.statusStrip_ClientSizeChanged);
            // 
            // buttonMulti
            // 
            this.buttonMulti.CheckOnClick = true;
            this.buttonMulti.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMulti.Image = ((System.Drawing.Image)(resources.GetObject("buttonMulti.Image")));
            this.buttonMulti.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMulti.Name = "buttonMulti";
            this.buttonMulti.Size = new System.Drawing.Size(24, 24);
            this.buttonMulti.Text = "Enable multiple selection";
            this.buttonMulti.CheckedChanged += new System.EventHandler(this.buttonMulti_CheckedChanged);
            // 
            // statusLabel
            // 
            this.statusLabel.Margin = new System.Windows.Forms.Padding(2, 3, 5, 2);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.statusLabel.Size = new System.Drawing.Size(49, 21);
            this.statusLabel.Text = "Status";
            this.statusLabel.Visible = false;
            this.statusLabel.TextChanged += new System.EventHandler(this.statusLabel_TextChanged);
            // 
            // progressBar
            // 
            this.progressBar.AutoSize = false;
            this.progressBar.Margin = new System.Windows.Forms.Padding(1, 3, 6, 3);
            this.progressBar.Name = "progressBar";
            this.progressBar.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.progressBar.Size = new System.Drawing.Size(400, 20);
            this.progressBar.Visible = false;
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonLoad,
            this.buttonSave,
            this.buttonConsole});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(734, 27);
            this.toolStrip.TabIndex = 1;
            // 
            // buttonLoad
            // 
            this.buttonLoad.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonLoadBinary,
            this.buttonLoadXml});
            this.buttonLoad.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoad.Image")));
            this.buttonLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(81, 24);
            this.buttonLoad.Text = "Load";
            // 
            // buttonLoadBinary
            // 
            this.buttonLoadBinary.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoadBinary.Image")));
            this.buttonLoadBinary.Name = "buttonLoadBinary";
            this.buttonLoadBinary.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.buttonLoadBinary.Size = new System.Drawing.Size(187, 26);
            this.buttonLoadBinary.Text = "&Binary...";
            this.buttonLoadBinary.Click += new System.EventHandler(this.buttonLoadBinary_Click);
            // 
            // buttonLoadXml
            // 
            this.buttonLoadXml.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoadXml.Image")));
            this.buttonLoadXml.Name = "buttonLoadXml";
            this.buttonLoadXml.Size = new System.Drawing.Size(187, 26);
            this.buttonLoadXml.Text = "&XML...";
            this.buttonLoadXml.Click += new System.EventHandler(this.buttonLoadXml_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSaveBinary,
            this.buttonSaveXml});
            this.buttonSave.Image = ((System.Drawing.Image)(resources.GetObject("buttonSave.Image")));
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(79, 24);
            this.buttonSave.Text = "Save";
            // 
            // buttonSaveBinary
            // 
            this.buttonSaveBinary.Image = ((System.Drawing.Image)(resources.GetObject("buttonSaveBinary.Image")));
            this.buttonSaveBinary.Name = "buttonSaveBinary";
            this.buttonSaveBinary.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.buttonSaveBinary.Size = new System.Drawing.Size(184, 26);
            this.buttonSaveBinary.Text = "&Binary...";
            this.buttonSaveBinary.Click += new System.EventHandler(this.buttonSaveBinary_Click);
            // 
            // buttonSaveXml
            // 
            this.buttonSaveXml.Image = ((System.Drawing.Image)(resources.GetObject("buttonSaveXml.Image")));
            this.buttonSaveXml.Name = "buttonSaveXml";
            this.buttonSaveXml.Size = new System.Drawing.Size(184, 26);
            this.buttonSaveXml.Text = "&XML...";
            this.buttonSaveXml.Click += new System.EventHandler(this.buttonSaveXml_Click);
            // 
            // buttonConsole
            // 
            this.buttonConsole.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonConsole.CheckOnClick = true;
            this.buttonConsole.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonConsole.Image = ((System.Drawing.Image)(resources.GetObject("buttonConsole.Image")));
            this.buttonConsole.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonConsole.Name = "buttonConsole";
            this.buttonConsole.Size = new System.Drawing.Size(24, 24);
            this.buttonConsole.Text = "Show debug console";
            this.buttonConsole.CheckedChanged += new System.EventHandler(this.buttonConsole_CheckedChanged);
            // 
            // splitContainer
            // 
            this.splitContainer.DataBindings.Add(new System.Windows.Forms.Binding("SplitterDistance", global::WarBender.Properties.Settings.Default, "MainForm_SplitterDistance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 27);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeView);
            this.splitContainer.Panel1MinSize = 150;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.propertyGrid);
            this.splitContainer.Panel2MinSize = 150;
            this.splitContainer.Size = new System.Drawing.Size(734, 458);
            this.splitContainer.SplitterDistance = global::WarBender.Properties.Settings.Default.MainForm_SplitterDistance;
            this.splitContainer.SplitterWidth = 5;
            this.splitContainer.TabIndex = 2;
            // 
            // treeView
            // 
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView.ContextMenuStrip = this.contextMenuTree;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Margin = new System.Windows.Forms.Padding(4);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.ShowNodeToolTips = true;
            this.treeView.ShowRootLines = false;
            this.treeView.Size = new System.Drawing.Size(295, 458);
            this.treeView.TabIndex = 0;
            this.treeView.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeCheck);
            this.treeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCheck);
            this.treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "loading");
            this.imageList.Images.SetKeyName(1, "value");
            this.imageList.Images.SetKeyName(2, "array");
            this.imageList.Images.SetKeyName(3, "record");
            this.imageList.Images.SetKeyName(4, "troop");
            this.imageList.Images.SetKeyName(5, "quest");
            this.imageList.Images.SetKeyName(6, "event");
            this.imageList.Images.SetKeyName(7, "variable");
            this.imageList.Images.SetKeyName(8, "info_page");
            this.imageList.Images.SetKeyName(9, "faction");
            this.imageList.Images.SetKeyName(10, "template");
            this.imageList.Images.SetKeyName(11, "party");
            this.imageList.Images.SetKeyName(12, "party_template");
            this.imageList.Images.SetKeyName(13, "site");
            this.imageList.Images.SetKeyName(14, "trigger");
            this.imageList.Images.SetKeyName(15, "simple_trigger");
            this.imageList.Images.SetKeyName(16, "map_track");
            this.imageList.Images.SetKeyName(17, "item_kind");
            this.imageList.Images.SetKeyName(18, "player_party_stack");
            this.imageList.Images.SetKeyName(19, "game");
            this.imageList.Images.SetKeyName(20, "header");
            // 
            // propertyGrid
            // 
            this.propertyGrid.AllowDrop = true;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.MainForm = null;
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(5);
            this.propertyGrid.MultiSelect = false;
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.ShowMultiSelectTip = true;
            this.propertyGrid.Size = new System.Drawing.Size(434, 458);
            this.propertyGrid.TabIndex = 0;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "sav";
            this.openFileDialog.Filter = "Mount & Blade saved games (sg??.sav)|sg??.sav|All files (*.*)|*.*";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "sav";
            this.saveFileDialog.Filter = "Mount & Blade saved games (sg??.sav)|sg??.sav|All files (*.*)|*.*";
            // 
            // contextMenuTree
            // 
            this.contextMenuTree.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemShowRawIds});
            this.contextMenuTree.Name = "contextMenuTree";
            this.contextMenuTree.Size = new System.Drawing.Size(211, 56);
            this.contextMenuTree.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuTree_Opening);
            // 
            // menuItemShowRawIds
            // 
            this.menuItemShowRawIds.CheckOnClick = true;
            this.menuItemShowRawIds.Name = "menuItemShowRawIds";
            this.menuItemShowRawIds.Size = new System.Drawing.Size(210, 24);
            this.menuItemShowRawIds.Text = "Show raw IDs";
            this.menuItemShowRawIds.CheckedChanged += new System.EventHandler(this.menuItemShowRawIds_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = global::WarBender.Properties.Settings.Default.MainForm_Size;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::WarBender.Properties.Settings.Default, "MainForm_Location", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DataBindings.Add(new System.Windows.Forms.Binding("ClientSize", global::WarBender.Properties.Settings.Default, "MainForm_Size", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::WarBender.Properties.Settings.Default.MainForm_Location;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(527, 358);
            this.Name = "MainForm";
            this.Text = "WarBender";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.contextMenuTree.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripButton buttonMulti;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripButton buttonConsole;
        private MutablePropertyGrid propertyGrid;
        private System.Windows.Forms.ToolStripSplitButton buttonLoad;
        private System.Windows.Forms.ToolStripMenuItem buttonLoadBinary;
        private System.Windows.Forms.ToolStripMenuItem buttonLoadXml;
        private System.Windows.Forms.ToolStripSplitButton buttonSave;
        private System.Windows.Forms.ToolStripMenuItem buttonSaveBinary;
        private System.Windows.Forms.ToolStripMenuItem buttonSaveXml;
        private System.Windows.Forms.ContextMenuStrip contextMenuTree;
        private System.Windows.Forms.ToolStripMenuItem menuItemShowRawIds;
    }
}

