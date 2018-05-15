namespace WarBender {
    partial class MutablePropertyGrid {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MutablePropertyGrid));
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.labelTip = new System.Windows.Forms.ToolStripLabel();
            this.buttonClearSelection = new System.Windows.Forms.ToolStripButton();
            this.toolStripMerged = new System.Windows.Forms.ToolStrip();
            this.buttonPopOut = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.toolStripMerged.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.AllowDrop = true;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(0, 25);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGrid.Size = new System.Drawing.Size(451, 392);
            this.propertyGrid.TabIndex = 3;
            this.propertyGrid.DragDrop += new System.Windows.Forms.DragEventHandler(this.MutablePropertyGrid_DragDrop);
            this.propertyGrid.DragEnter += new System.Windows.Forms.DragEventHandler(this.MutablePropertyGrid_DragEnter);
            this.propertyGrid.DragOver += new System.Windows.Forms.DragEventHandler(this.MutablePropertyGrid_DragOver);
            // 
            // toolStrip
            // 
            this.toolStrip.AllowDrop = true;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelTip,
            this.buttonClearSelection});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip.Location = new System.Drawing.Point(0, 394);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(451, 23);
            this.toolStrip.TabIndex = 4;
            this.toolStrip.Visible = false;
            this.toolStrip.DragDrop += new System.Windows.Forms.DragEventHandler(this.MutablePropertyGrid_DragDrop);
            this.toolStrip.DragEnter += new System.Windows.Forms.DragEventHandler(this.MutablePropertyGrid_DragEnter);
            this.toolStrip.DragOver += new System.Windows.Forms.DragEventHandler(this.MutablePropertyGrid_DragOver);
            // 
            // labelTip
            // 
            this.labelTip.Name = "labelTip";
            this.labelTip.Size = new System.Drawing.Size(262, 15);
            this.labelTip.Text = "Use checkboxes in the tree to select items to edit";
            // 
            // buttonClearSelection
            // 
            this.buttonClearSelection.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonClearSelection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonClearSelection.Image = ((System.Drawing.Image)(resources.GetObject("buttonClearSelection.Image")));
            this.buttonClearSelection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonClearSelection.Name = "buttonClearSelection";
            this.buttonClearSelection.Size = new System.Drawing.Size(23, 20);
            this.buttonClearSelection.Text = "Clear selection";
            this.buttonClearSelection.Click += new System.EventHandler(this.buttonClearSelection_Click);
            // 
            // toolStripMerged
            // 
            this.toolStripMerged.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonPopOut});
            this.toolStripMerged.Location = new System.Drawing.Point(0, 0);
            this.toolStripMerged.Name = "toolStripMerged";
            this.toolStripMerged.Size = new System.Drawing.Size(451, 25);
            this.toolStripMerged.TabIndex = 5;
            this.toolStripMerged.Visible = false;
            // 
            // buttonPopOut
            // 
            this.buttonPopOut.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonPopOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPopOut.Image = ((System.Drawing.Image)(resources.GetObject("buttonPopOut.Image")));
            this.buttonPopOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPopOut.Name = "buttonPopOut";
            this.buttonPopOut.Size = new System.Drawing.Size(23, 22);
            this.buttonPopOut.Text = "Open in separate window";
            this.buttonPopOut.Click += new System.EventHandler(this.buttonPopOut_Click);
            // 
            // MutablePropertyGrid
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.toolStripMerged);
            this.Name = "MutablePropertyGrid";
            this.Size = new System.Drawing.Size(451, 417);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MutablePropertyGrid_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MutablePropertyGrid_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.MutablePropertyGrid_DragOver);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.toolStripMerged.ResumeLayout(false);
            this.toolStripMerged.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel labelTip;
        private System.Windows.Forms.ToolStripButton buttonClearSelection;
        private System.Windows.Forms.ToolStrip toolStripMerged;
        private System.Windows.Forms.ToolStripButton buttonPopOut;
    }
}
