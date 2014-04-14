﻿using SuperPutty.Utils;
using System.Drawing;
using System.Windows.Forms;
namespace SuperPutty
{
    partial class SessionTreeview
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SessionTreeview));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStripAddTreeItem = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createLikeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectExternalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectInNewSuperPuTTYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            // Adding a button to Menu
            checkboxEnable = new CheckBox();
            checkboxEnable.Text = "Enabled";
            checkboxEnable.Checked = true;
            this.checkboxEnableToolStripMenuItem = new ToolStripControlHost(checkboxEnable);
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.fileBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripFolder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newSessionToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.connectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerDelayedSave = new System.Windows.Forms.Timer(this.components);
            this.panelSearch = new System.Windows.Forms.Panel();
            this.sessionFilter = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.contextMenuStripAddTreeItem.SuspendLayout();
            this.contextMenuStripFolder.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 25);
            this.treeView1.Margin = new System.Windows.Forms.Padding(5);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowLines = false;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.ShowRootLines = false;
            this.treeView1.Size = new System.Drawing.Size(430, 478);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeCollapse);
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
            this.treeView1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeView1_KeyPress);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.png");
            this.imageList1.Images.SetKeyName(1, "computer.png");
            // 
            // contextMenuStripAddTreeItem
            // 
            this.contextMenuStripAddTreeItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSessionToolStripMenuItem,
            this.createLikeToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.connectToolStripMenuItem,
            this.connectExternalToolStripMenuItem,
            this.connectInNewSuperPuTTYToolStripMenuItem,
            this.toolStripSeparator2,
            this.checkboxEnableToolStripMenuItem,
            this.toolStripSeparator2,
            this.fileBrowserToolStripMenuItem});
            this.contextMenuStripAddTreeItem.Name = "contextMenuStripAddTreeItem";
            this.contextMenuStripAddTreeItem.ShowImageMargin = false;
            this.contextMenuStripAddTreeItem.Size = new System.Drawing.Size(182, 192);
            this.contextMenuStripAddTreeItem.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripAddTreeItem_Opening);
            // 
            // newSessionToolStripMenuItem
            // 
            this.newSessionToolStripMenuItem.Name = "newSessionToolStripMenuItem";
            this.newSessionToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.newSessionToolStripMenuItem.Text = "New";
            this.newSessionToolStripMenuItem.Click += new System.EventHandler(this.CreateOrEditSessionToolStripMenuItem_Click);
            // 
            // createLikeToolStripMenuItem
            // 
            this.createLikeToolStripMenuItem.Name = "createLikeToolStripMenuItem";
            this.createLikeToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.createLikeToolStripMenuItem.Text = "Copy As";
            this.createLikeToolStripMenuItem.Click += new System.EventHandler(this.CreateOrEditSessionToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.settingsToolStripMenuItem.Text = "Edit";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.CreateOrEditSessionToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // connectExternalToolStripMenuItem
            // 
            this.connectExternalToolStripMenuItem.Name = "connectExternalToolStripMenuItem";
            this.connectExternalToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.connectExternalToolStripMenuItem.Text = "Connect External";
            this.connectExternalToolStripMenuItem.Click += new System.EventHandler(this.connectExternalToolStripMenuItem_Click);
            // 
            // connectInNewSuperPuTTYToolStripMenuItem
            // 
            this.connectInNewSuperPuTTYToolStripMenuItem.Name = "connectInNewSuperPuTTYToolStripMenuItem";
            this.connectInNewSuperPuTTYToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.connectInNewSuperPuTTYToolStripMenuItem.Text = "Connect in New Instance";
            this.connectInNewSuperPuTTYToolStripMenuItem.Click += new System.EventHandler(this.connectInNewSuperPuTTYToolStripMenuItem_Click);

            // 
            // enableToolStripMenuItem
            // 
            this.checkboxEnableToolStripMenuItem.Name = "checkboxEnableToolStripMenuItem";
            this.checkboxEnableToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.checkboxEnableToolStripMenuItem.AutoSize = true;
            this.checkboxEnableToolStripMenuItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkboxEnableToolStripMenuItem.BackColor = Color.White;
            this.checkboxEnable.CheckStateChanged += new System.EventHandler(this.checkboxEnable_StateChanged);

            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
            // 
            // fileBrowserToolStripMenuItem
            // 
            this.fileBrowserToolStripMenuItem.Name = "fileBrowserToolStripMenuItem";
            this.fileBrowserToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.fileBrowserToolStripMenuItem.Text = "File Transfer";
            this.fileBrowserToolStripMenuItem.Click += new System.EventHandler(this.fileBrowserToolStripMenuItem_Click);
            // 
            // contextMenuStripFolder
            // 
            this.contextMenuStripFolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSessionToolStripMenuItem1,
            this.toolStripSeparator3,
            this.newFolderToolStripMenuItem,
            this.removeFolderToolStripMenuItem,
            this.toolStripMenuItem1,
            this.renameToolStripMenuItem,
            this.toolStripMenuItem3,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.toolStripMenuItem4,
            this.connectAllToolStripMenuItem,
            this.toolStripMenuItem2,
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem});
            this.contextMenuStripFolder.Name = "contextMenuStripAddTreeItem";
            this.contextMenuStripFolder.ShowImageMargin = false;
            this.contextMenuStripFolder.Size = new System.Drawing.Size(129, 232);
            this.contextMenuStripFolder.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripFolder_Opening);
            // 
            // newSessionToolStripMenuItem1
            // 
            this.newSessionToolStripMenuItem1.Name = "newSessionToolStripMenuItem1";
            this.newSessionToolStripMenuItem1.Size = new System.Drawing.Size(128, 22);
            this.newSessionToolStripMenuItem1.Text = "New";
            this.newSessionToolStripMenuItem1.Click += new System.EventHandler(this.CreateOrEditSessionToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(125, 6);
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.newFolderToolStripMenuItem.Text = "New Folder";
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
            // 
            // removeFolderToolStripMenuItem
            // 
            this.removeFolderToolStripMenuItem.Name = "removeFolderToolStripMenuItem";
            this.removeFolderToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.removeFolderToolStripMenuItem.Text = "Remove Folder";
            this.removeFolderToolStripMenuItem.Click += new System.EventHandler(this.removeFolderToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(125, 6);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(125, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.moveUpToolStripMenuItem.Text = "Move up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.moveDownToolStripMenuItem.Text = "Move down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(125, 6);
            // 
            // connectAllToolStripMenuItem
            // 
            this.connectAllToolStripMenuItem.Name = "connectAllToolStripMenuItem";
            this.connectAllToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.connectAllToolStripMenuItem.Text = "Connect All";
            this.connectAllToolStripMenuItem.Click += new System.EventHandler(this.connectAllToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(125, 6);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
            // 
            // timerDelayedSave
            // 
            this.timerDelayedSave.Interval = 3000;
            this.timerDelayedSave.Tick += new System.EventHandler(this.timerDelayedSave_Tick);
            // 
            // panelSearch
            // 
            this.panelSearch.Controls.Add(this.sessionFilter);
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Controls.Add(this.btnSearch);
            this.panelSearch.Controls.Add(this.btnClear);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(2, 2);
            this.panelSearch.Margin = new System.Windows.Forms.Padding(4);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Padding = new System.Windows.Forms.Padding(0, 2, 1, 2);
            this.panelSearch.Size = new System.Drawing.Size(431, 25);
            this.panelSearch.TabIndex = 2;
            // 
            // sessionFilter
            // 
            this.sessionFilter.BackColor = System.Drawing.SystemColors.ControlLight;
            this.sessionFilter.Dock = System.Windows.Forms.DockStyle.Left;
            this.sessionFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sessionFilter.Items.AddRange(new object[] {
            "All sessions", "Sessions enabled only", "Sessions disabled only", "Sessions opened only", "Sessions closed only"});
            this.sessionFilter.SelectedIndex = 0;
            this.sessionFilter.Location = new System.Drawing.Point(0, 2);
            this.sessionFilter.Margin = new System.Windows.Forms.Padding(0);
            this.sessionFilter.Name = "sessionFilter";
            this.sessionFilter.Size = new System.Drawing.Size(132, 21);
            this.sessionFilter.TabIndex = 0;
            this.sessionFilter.SelectedValueChanged += new System.EventHandler(this.sessionFilterSelectionValue_Changed);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(137, 2);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(245, 19);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            Win32Utility.SetCueText(this.txtSearch, "Search...");

            // 
            // btnSearch
            // 
            this.btnSearch.AutoSize = true;
            this.btnSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSearch.Image = global::SuperPutty.Properties.Resources.find;
            this.btnSearch.Location = new System.Drawing.Point(385, 2);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(0);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(22, 21);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.AutoSize = true;
            this.btnClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClear.Image = global::SuperPutty.Properties.Resources.cross_black;
            this.btnClear.Location = new System.Drawing.Point(407, 2);
            this.btnClear.Margin = new System.Windows.Forms.Padding(0);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(22, 21);
            this.btnClear.TabIndex = 3;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // SessionTreeview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 503);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.panelSearch);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "SessionTreeview";
            this.ShowIcon = false;
            this.TabText = "Sessions";
            this.Text = "PuTTY Sessions";
            this.contextMenuStripAddTreeItem.ResumeLayout(false);
            this.contextMenuStripFolder.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAddTreeItem;
        private System.Windows.Forms.ToolStripMenuItem newSessionToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFolder;
        private System.Windows.Forms.ToolStripMenuItem newSessionToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeFolderToolStripMenuItem;
        private System.Windows.Forms.Timer timerDelayedSave;
        private System.Windows.Forms.ToolStripMenuItem createLikeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectExternalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectInNewSuperPuTTYToolStripMenuItem;
        private System.Windows.Forms.ToolStripControlHost checkboxEnableToolStripMenuItem;
        private CheckBox checkboxEnable;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem connectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ComboBox sessionFilter;
    }
}