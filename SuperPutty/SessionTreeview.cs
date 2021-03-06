﻿/*
 * Copyright (c) 2009 Jim Radford http://www.jimradford.com
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions: 
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using log4net;
using SuperPuTTY.Manager;
using SuperPuTTY.Utils;
using WeifenLuo.WinFormsUI.Docking;
using SuperPuTTY.Gui;
using System.IO;
using System.Text.RegularExpressions;


namespace SuperPuTTY
{
    public partial class SessionTreeview : ToolWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SessionTreeview));

        private static int MaxSessionsToOpen = Convert.ToInt32(ConfigurationManager.AppSettings["SuperPuTTY.MaxSessionsToOpen"] ?? "10");

        public const string SessionIdDelim = "/";
        public const string ImageKeySession = "computer";
        public const string ImageKeyFolder = "folder";


        private DockPanel m_DockPanel;
        private TreeNode nodeRoot;
        private ImageList imgIcons = new ImageList();
        private Func<SessionData, bool> filter;

        /// <summary>
        /// Instantiate the treeview containing the sessions
        /// </summary>
        /// <param name="dockPanel">The DockPanel container</param>
        /// <remarks>Having the dockpanel container is necessary to allow us to
        /// dock any terminal or file transfer sessions from within the treeview class</remarks>
        public SessionTreeview(DockPanel dockPanel)
        {
            m_DockPanel = dockPanel;

            this.Enabled = false;

          //  this.m_DockPanel.Paint += (a, b) => DrawWarningMessage(a, b);
            checkActivation();
        }

        public void checkActivation()
        {

            if (this.Enabled)
            {
                InitializeComponent();

                //this.treeView1.TreeViewNodeSorter = this;
                this.treeView1.ImageList = SuperPuTTY.Images;
                this.ApplySettings();

                // populate sessions in the treeview from the registry
                this.CreateTreeview();

                //SuperPuTTY.Sessions.ListChanged += new ListChangedEventHandler(Sessions_ListChanged);
                SuperPuTTY.Settings.SettingsSaving += new SettingsSavingEventHandler(Settings_SettingsSaving);

                m_DockPanel.ContextMenuStrip = this.contextMenuStripFolder;
            }
            else
            {
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(430, 503);
                this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.Margin = new System.Windows.Forms.Padding(5);
                this.Name = "SessionTreeview";
                this.ShowIcon = false;
                this.TabText = "Sessions";
                this.Text = "PuTTY Sessions";

                this.ResumeLayout(false);
            }
        }
        /*
        protected void DrawWarningMessage(object sender, PaintEventArgs e)
        {
            //this.BackColor = Color.White;
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            string drawString = "Open a database!";
            System.Drawing.Font drawFont = new System.Drawing.Font(
                "Arial", 11);
            System.Drawing.SolidBrush drawBrush = new
                System.Drawing.SolidBrush(System.Drawing.Color.Black);
            float x = 20;
            float y = 20;
            formGraphics.DrawString(drawString, drawFont, drawBrush, y, x);
            drawFont.Dispose();
            drawBrush.Dispose();
            formGraphics.Dispose();

        }*/

        void ExpandInitialTree() 
        {
            if (SuperPuTTY.Settings.ExpandSessionsTreeOnStartup)
            {
                nodeRoot.ExpandAll();
                this.treeView1.SelectedNode = nodeRoot;
            }
            else
            {
                // start with semi-collapsed view
                nodeRoot.Expand();
                foreach (TreeNode node in this.nodeRoot.Nodes)
                {
                    if (!IsSessionNode(node))
                    {
                        node.Collapse();
                    }
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

           // this.ExpandInitialTree();
        }

        void Settings_SettingsSaving(object sender, CancelEventArgs e)
        {
            this.ApplySettings();
        }

        void ApplySettings()
        {
            this.treeView1.ShowLines = SuperPuTTY.Settings.SessionsTreeShowLines;
            this.treeView1.Font = SuperPuTTY.Settings.SessionsTreeFont;
            this.panelSearch.Visible = SuperPuTTY.Settings.SessionsShowSearch;
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
        }

        protected override void OnClosed(EventArgs e)
        {
            //SuperPuTTY.Sessions.ListChanged -= new ListChangedEventHandler(Sessions_ListChanged);
            SuperPuTTY.Settings.SettingsSaving -= new SettingsSavingEventHandler(Settings_SettingsSaving);
            base.OnClosed(e);
        }

        /// <summary>
        /// Load the sessions from the registry and populate the treeview control
        /// </summary>
        public void CreateTreeview()
        {
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            SuperPuTTY.selectionFilter = (SelectionFilter)Enum.ToObject(typeof(SelectionFilter), sessionFilter.SelectedIndex);

            this.nodeRoot = treeView1.Nodes.Add("root", "PuTTY Sessions", ImageKeyFolder, ImageKeyFolder);
            this.nodeRoot.ContextMenuStrip = this.contextMenuStripFolder;

            SessionFolderData rootFolderData = SuperPuTTY.GetRootFolderData();
            
            this.nodeRoot.Tag = rootFolderData;
            CreateNodes(rootFolderData, this.nodeRoot);
            CreateNodesSession(rootFolderData.SessionDataChildren, this.nodeRoot);

            treeView1.SelectedNode = this.nodeRoot;
            nodeRoot.Expand();

            if (!SelectionFilter.ALL.Equals(SuperPuTTY.selectionFilter))
            {
                treeView1.ExpandAll();
            }

            treeView1.Invalidate();
            treeView1.EndUpdate();
        }

        private void CreateNodes(SessionFolderData folderData, TreeNode currentNode)
        {
            foreach (SessionFolderData child in folderData.SessionFolderDataChildren)
            {
                TreeNode childNode = AddFolderNode(currentNode, child);
                CreateNodes(child, childNode);
                CreateNodesSession(child.SessionDataChildren, childNode);

                if (child.IsExpand)
                {
                    childNode.Expand();
                }
                else
                {
                    childNode.Collapse();
                }
            }
        }

        private void CreateNodesSession(BindingList<SessionData> sessions, TreeNode currentNode)
        {
            foreach (SessionData session in sessions)
            {
                if (this.filter == null || this.filter(session))
                {
                    AddSessionNode(currentNode, session, true);
                }
            }
        }

        void Sessions_ListChanged(object sender, ListChangedEventArgs e)
        {
            /*if (isRenamingNode)
            {
                return;
            }*/
            BindingList<SessionData> sessions = (BindingList<SessionData>) sender;
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                SessionData session = sessions[e.NewIndex];
                //TryAddSessionNode(session);
            }
            else if (e.ListChangedType == ListChangedType.Reset)
            {
                // clear
                List<TreeNode> nodesToRemove = new List<TreeNode>();
                foreach(TreeNode node in nodeRoot.Nodes)
                {
                    nodesToRemove.Add(node);
                }
                foreach (TreeNode node in nodesToRemove)
                {
                    node.Remove();
                }
            }
            // @TODO: implement more later, note delete will be tricky...need a copy of the list
        }

        /// <summary>
        /// Opens the selected session when the node is double clicked in the treeview
        /// </summary>
        /// <param name="sender">The treeview control that was double clicked</param>
        /// <param name="e">An Empty EventArgs object</param>
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // e is null if this method is called from connectToolStripMenuItem_Click
            TreeNode node = (e != null) ? e.Node : treeView1.SelectedNode;

            if (IsSessionNode(node) && node == treeView1.SelectedNode)
            {
                SessionData sessionData = (SessionData)node.Tag;
                SuperPuTTY.OpenPuttySession(sessionData);
            }
        }


        /// <summary>
        /// Create/Update a session entry
        /// </summary>
        /// <param name="sender">The toolstripmenuitem control that was clicked</param>
        /// <param name="e">An Empty EventArgs object</param>
        private void CreateOrEditSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SessionData session = null;
            TreeNode node = null;
            TreeNode nodeRef = this.nodeRoot;
            bool isEdit = false;
            string title = null;
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                bool isFolderNode = IsFolderNode(treeView1.SelectedNode);
                if (menuItem.Text.ToLower().Equals("new") || isFolderNode)
                {
                    session = new SessionData();
                    nodeRef = isFolderNode ? treeView1.SelectedNode : treeView1.SelectedNode.Parent;
                    title = "Create New Session";
                }
                else if (menuItem == this.createLikeToolStripMenuItem)
                {
                    // copy as
                    session = (SessionData) ((SessionData) treeView1.SelectedNode.Tag).Clone();
                    //session.SessionId = SuperPuTTY.MakeUniqueSessionId(session.SessionId);
                    //session.SessionName = SessionData.GetSessionNameFromId(session.SessionId);
                    nodeRef = treeView1.SelectedNode.Parent;
                    title = "Create New Session Like " + session.SessionName;
                }
                else
                {
                    // edit, session node selected
                    session = (SessionData)treeView1.SelectedNode.Tag;
                    node = treeView1.SelectedNode;
                    nodeRef = node.Parent;
                    isEdit = true;
                    title = "Edit Session: " + session.SessionName;
                }
            }

            dlgEditSession form = new dlgEditSession(session, this.treeView1.ImageList);
            form.Text = title;
            form.SessionNameValidator += delegate(string txt, out string error)
            {
                error = String.Empty;
                bool isDupeNode = isEdit ? txt != node.Text && nodeRef.Nodes.ContainsKey(txt) : nodeRef.Nodes.ContainsKey(txt);
                if (isDupeNode)
                {
                    error = "Session with same name exists";
                }
                else if (txt.Contains(SessionIdDelim))
                {
                    error = "Invalid character ( " + SessionIdDelim + " ) in name";
                }
                else if (string.IsNullOrEmpty(txt) || txt.Trim() == String.Empty)
                {
                    error = "Empty name";
                }
                return string.IsNullOrEmpty(error);
            };
            
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                /* "node" will only be assigned if we're editing an existing session entry */


                if (node == null)
                {
                    SuperPuTTY.AddSession(session, (SessionFolderData) nodeRef.Tag);
                    //this.treeView1.SelectedNode = AddSessionNode(nodeRef, session, true);

                }
                else
                {
                    // handle renames
                    /*node.Text = session.SessionName;
                    node.Name = session.SessionName;
                    node.ImageKey = session.ImageKey;
                    node.SelectedImageKey = session.ImageKey;*/

                    /*try
                    {
                        this.isRenamingNode = true;
                        ((SessionFolderData)(nodeRef.Tag)).Name = session.SessionName;
                    }
                    finally
                    {
                        this.isRenamingNode = false;
                    }*/
                    //this.treeView1.SelectedNode = node;
                }

                DatabaseManager.Instance.Save();
                CreateTreeview();
                this.treeView1.SelectedNode = getTreeNode(nodeRoot, session);
            }
            
        }

        /// <summary>
        /// Forces a node to be selected when right clicked, this assures the context menu will be operating
        /// on the proper session entry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
            }          
        }

        /// <summary>
        /// Delete a session entry from the treeview and the registry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SessionData session = (SessionData)treeView1.SelectedNode.Tag;
            if (MessageBox.Show("Are you sure you want to delete " + session.SessionName + "?", "Delete Session?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SessionFolderData parent = SuperPuTTY.GetRootFolderData().GetParent(SuperPuTTY.GetRootFolderData(), session);
                SuperPuTTY.GetRootFolderData().RemoveSession(session);

                DatabaseManager.Instance.Save();
                CreateTreeview();
                treeView1.SelectedNode = getTreeNode(nodeRoot, parent);
                treeView1.SelectedNode.Expand();
            }
        }

        /// <summary>
        /// Open a directory listing on the selected nodes host to allow dropping files
        /// for drag + drop copy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SessionData session = (SessionData)treeView1.SelectedNode.Tag;
            SuperPuTTY.OpenScpSession(session);
        }

        /// <summary>
        /// Shortcut for double clicking an entries node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1_NodeMouseDoubleClick(null, null);
        }

        /// <summary>
        /// Open putty with args but as external process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectExternalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (IsSessionNode(node))
            {
                SessionData sessionData = (SessionData)node.Tag;
                PuttyStartInfo startInfo = new PuttyStartInfo(sessionData);
                startInfo.StartStandalone();
            }
        }

        private void connectInNewSuperPuTTYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (IsSessionNode(node))
            {
                SuperPuTTY.LoadSessionInNewInstance(((SessionData)node.Tag).SessionName);
            }
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null)
            {
                dlgRenameItem dialog = new dlgRenameItem();
                dialog.Text = "New Folder";
                dialog.ItemName = "New Folder";
                dialog.DetailName = "";
                dialog.ItemNameValidator = delegate(string txt, out string error)
                {
                    error = String.Empty;
                    if (node.Nodes.ContainsKey(txt))
                    {
                        error = "Node with same name exists";
                    }
                    else if (txt.Contains(SessionIdDelim))
                    {
                        error = "Invalid character ( " + SessionIdDelim + " ) in name";
                    }
                    else if (string.IsNullOrEmpty(txt) || txt.Trim() == String.Empty)
                    {
                        error = "Empty folder name";
                    }

                    return string.IsNullOrEmpty(error);
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    SessionFolderData parent = (SessionFolderData)node.Tag;
                    SessionFolderData newData = parent.AddChildFolderData(dialog.ItemName);
                   //SessionFolderData folderData = SuperPuTTY.GetRootFolderData().AddChildFolderData(dialog.ItemName);
                    //this.treeView1.SelectedNode = AddFolderNode(node, folderData);

                    DatabaseManager.Instance.Save();
                    CreateTreeview();
                    this.treeView1.SelectedNode = getTreeNode(nodeRoot, newData);
                }
            }
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            
            if (node != null)
            {
                TreeNode parent = node.Parent;
                int index = parent.Nodes.IndexOf(node);
                if (index > 0)
                {
                    //parent.Nodes.RemoveAt(index);
                    //parent.Nodes.Insert(index - 1, node);
                    SuperPuTTY.ReportStatus("Move up : {0}", node.Index);
                    SessionFolderData rootFolderData = SuperPuTTY.GetRootFolderData();
                    if (IsFolderNode(node))
                    {
                        rootFolderData.MoveUp(rootFolderData, (SessionFolderData)node.Tag);

                        DatabaseManager.Instance.Save();
                        CreateTreeview();
                        this.treeView1.SelectedNode = getTreeNode(this.treeView1.Nodes[0], (SessionFolderData)node.Tag);
                    }
                }
            }


        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null)
            {
                TreeNode parent = node.Parent;
                int index = parent.Nodes.IndexOf(node);
                if (index < parent.Nodes.Count - 1)
                {
                    SuperPuTTY.ReportStatus("Move down : {0}", index);

                    SessionFolderData rootFolderData = SuperPuTTY.GetRootFolderData();
                    rootFolderData.MoveDown(index);//, (SessionFolderData)node.Tag);

                    DatabaseManager.Instance.Save();
                    CreateTreeview();
                    this.treeView1.SelectedNode = getTreeNode(this.treeView1.Nodes[0], (SessionFolderData)node.Tag);
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null)
            {
                dlgRenameItem dialog = new dlgRenameItem();
                dialog.Text = "Rename Folder";
                dialog.ItemName = node.Text;
                dialog.DetailName = "";
                dialog.ItemNameValidator = delegate(string txt, out string error)
                {
                    error = String.Empty;
                    if (node.Parent.Nodes.ContainsKey(txt) && txt != node.Text)
                    {
                        error = "Node with same name exists";
                    }
                    else if (txt.Contains(SessionIdDelim))
                    {
                        error = "Invalid character ( " + SessionIdDelim + " ) in name";
                    }
                    return string.IsNullOrEmpty(error);
                };
                if (dialog.ShowDialog(this) == DialogResult.OK && node.Text != dialog.ItemName)
                {

                    SuperPuTTY.GetRootFolderData().RenameSessionFolderName((SessionFolderData) node.Tag, dialog.ItemName);
                    DatabaseManager.Instance.Save();
                    CreateTreeview();
                    this.treeView1.SelectedNode = getTreeNode(this.treeView1.Nodes[0], (SessionFolderData)node.Tag);
                }
            }
        }

        private void removeFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null)
            {
                if (node.Nodes.Count > 0)
                {
                    //List<SessionData> sessions = new List<SessionData>();
                    //GetAllSessions(node, sessions);
                    SessionFolderData sFolderData = (SessionFolderData) node.Tag;
                    if (DialogResult.Yes == MessageBox.Show(
                        "Remove Folder [" + node.Text + "] and [" + sFolderData.SessionDataChildren.Count + "] sessions?",
                        "Remove Folder?", 
                        MessageBoxButtons.YesNo))
                    {
                        SuperPuTTY.RemoveFolder(sFolderData);
                        DatabaseManager.Instance.Save();
                        CreateTreeview();
                    }
                }
                else
                {
                    node.Remove();
                    SuperPuTTY.ReportStatus("Removed Folder, {0}", node.Text);
                }
            }
        }

        private void connectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null && !IsSessionNode(node))
            {
                List<SessionData> sessions = new List<SessionData>();
                GetAllSessions(node, sessions);
                Log.InfoFormat("Found {0} sessions", sessions.Count);

                if (sessions.Count > MaxSessionsToOpen)
                {
                    if (DialogResult.Cancel == MessageBox.Show(
                        "Open All " + sessions.Count + " sessions?", 
                        "WARNING", 
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                    {
                        // bug out...too many sessions to open
                        return;
                    }
                }
                foreach (SessionData session in sessions)
                {
                        SuperPuTTY.OpenPuttySession(session);
                }
            }
        }

        private void contextMenuStripFolder_Opening(object sender, CancelEventArgs e)
        {
            bool isRootNode = this.treeView1.SelectedNode != this.nodeRoot;
            this.renameToolStripMenuItem.Enabled = isRootNode;
            // TODO: handle removing folder and nodes in it recursively
            this.removeFolderToolStripMenuItem.Enabled = isRootNode;// && this.treeView1.SelectedNode.Nodes.Count == 0;
        }

        private void contextMenuStripAddTreeItem_Opening(object sender, CancelEventArgs e)
        {
            // disable file transfers if pscp isn't configured.
            fileBrowserToolStripMenuItem.Enabled = SuperPuTTY.IsScpEnabled;

            connectInNewSuperPuTTYToolStripMenuItem.Enabled = !SuperPuTTY.Settings.SingleInstanceMode;

            SessionData sessionData = (SessionData) this.treeView1.SelectedNode.Tag;
            checkboxEnable.Checked = sessionData.Enabled;
        }

        #region Node helpers

        private void AddSessionNode(TreeNode parentNode, SessionData session, bool isInitializing)
        {
            if (SelectionFilter.ENABLED_ONLY.Equals(SuperPuTTY.selectionFilter))
            {
                if (!session.Enabled)
                {
                    return;
                }
            }
            else if (SelectionFilter.DISABLED_ONLY.Equals(SuperPuTTY.selectionFilter))
            {
                if (session.Enabled)
                {
                    return;
                }
            }
            else if (SelectionFilter.ACTIVE_ONLY.Equals(SuperPuTTY.selectionFilter))
            {
                // active only
                if (!session.IsActive)
                {
                    return;
                }
            }
            else if (SelectionFilter.UNACTIVE_ONLY.Equals(SuperPuTTY.selectionFilter))
            {
                // inactive only
                if (session.IsActive)
                {
                    return;
                }
            }

            TreeNode addedNode = parentNode.Nodes.Add(session.SessionName, session.SessionName, ImageKeySession, ImageKeySession);
                addedNode.Tag = session;
                addedNode.ContextMenuStrip = this.contextMenuStripAddTreeItem;
                addedNode.ToolTipText = GetSessionToolTip(session);
                if (!session.Enabled)
                {
                    addedNode.ForeColor = SystemColors.GrayText;
                }
                if (session.IsActive)
                {
                    Font boldFont = new Font(this.treeView1.Font, FontStyle.Bold);
                    addedNode.NodeFont = boldFont;
                }
                // Override with custom icon if valid
                if (IsValidImage(session.ImageKey))
                {
                    addedNode.ImageKey = session.ImageKey;
                    addedNode.SelectedImageKey = session.ImageKey;
                }
        }

        private TreeNode AddFolderNode(TreeNode parentNode, SessionFolderData folderData)
        {
            TreeNode nodeNew = null;
        
                SuperPuTTY.ReportStatus("Adding new folder, {1}.  parent={0}", parentNode.Text, folderData.Name);
                nodeNew = parentNode.Nodes.Add(folderData.Name, folderData.Name, ImageKeyFolder, ImageKeyFolder);
                nodeNew.ContextMenuStrip = this.contextMenuStripFolder;
                nodeNew.Tag = folderData;
            
            return nodeNew;
        }

        bool IsSessionNode(TreeNode node)
        {
            return node != null && node.Tag is SessionData;
        }

        bool IsFolderNode(TreeNode node)
        {
            return !IsSessionNode(node);
        }

        /*
        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;

            return -1; // string.Compare(tx.Text, ty.Text);

        }

        /*void ResortNodes()
        {
            this.treeView1.TreeViewNodeSorter = null;
            this.treeView1.TreeViewNodeSorter = this;
        }*/

        private void checkboxEnable_StateChanged(object sender, EventArgs e)
        {
            SessionData sessionData = (SessionData) this.treeView1.SelectedNode.Tag;
            sessionData.Enabled = checkboxEnable.Checked;

            if (sessionData.Enabled)
            {
                this.treeView1.SelectedNode.ForeColor = Color.Black;
            }
            else
            {
                this.treeView1.SelectedNode.ForeColor = SystemColors.GrayText;
            }
            
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
              TreeNode node = this.treeView1.SelectedNode;
              if (node != null)
              {
                  node.ExpandAll();
              }
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null)
            {
                node.Collapse();
                if (node == this.nodeRoot)
                {
                    nodeRoot.Expand();
                }
            }
        }

        void GetAllSessions(TreeNode nodeFolder, List<SessionData> sessions)
        {
            if (nodeFolder != null)
            {
                foreach (TreeNode child in nodeFolder.Nodes)
                {
                    if (IsSessionNode(child))
                    {
                        SessionData session = (SessionData) child.Tag;
                        sessions.Add(session);
                    }
                    else
                    {
                        GetAllSessions(child, sessions);
                    }
                }
            }
        }

        void GetAllNodes(TreeNode node, List<TreeNode> nodes)
        {
            if (node != null)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    if (child.Nodes.Count == 0)
                    {
                        nodes.Add(child);
                    }
                    else
                    {
                        GetAllNodes(child, nodes);
                    }
                }
            }
        }

        private static string GetSessionToolTip(SessionData session)
        {
            if (string.IsNullOrEmpty(session.PuttySession))
            {
                return session.ToString();
            }

            if (session.Proto != ConnectionProtocol.Auto && !string.IsNullOrEmpty(session.Host) && session.Port > 0)
            {
                return session.ToString();
            }

            var puttyProfile = PuttyDataHelper.GetSessionData(session.SessionName);

            var protocol = (session.Proto == ConnectionProtocol.Auto)
                ? puttyProfile.Proto
                : session.Proto;

            var host = string.IsNullOrEmpty(session.Host)
                ? puttyProfile.Host
                : session.Host;

            var port = (session.Port == 0)
                ? puttyProfile.Port
                : session.Port;

            return string.Format("{0}://{1}:{2}", protocol.ToString().ToLower(), host, port);
        }
        #endregion

        #region Drag Drop

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Get the tree
            TreeView tree = (TreeView)sender;

            // Get the node underneath the mouse.
            TreeNode node = e.Item as TreeNode;

            // Start the drag-and-drop operation with a cloned copy of the node.
            //if (node != null && IsSessionNode(node))
            if (node != null && tree.Nodes[0] != node)
            {
                this.treeView1.DoDragDrop(node, DragDropEffects.Copy);
            }
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            // Get the tree.
            TreeView tree = (TreeView)sender;

            // Drag and drop denied by default.
            e.Effect = DragDropEffects.None;

            // Is it a valid format?
            TreeNode nodePayload = (TreeNode) e.Data.GetData(typeof(TreeNode));
            if (nodePayload != null)
            {
                // Get the screen point.
                Point pt = new Point(e.X, e.Y);

                // Convert to a point in the TreeView's coordinate system.
                pt = tree.PointToClient(pt);

                TreeNode node = tree.GetNodeAt(pt);
                // Is the mouse over a valid node?
                if (node != null && node != nodePayload && nodePayload.Nodes.Find(node.Text, true).Length == 0)
                {
                    tree.SelectedNode = node;
                    // folder that is not the same parent and new node name is not already present
                    if (IsFolderNode(node) && !node.Nodes.ContainsKey(nodePayload.Text))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                }
            }
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            // Get the tree.
            TreeView tree = (TreeView)sender;

            // Get the screen point.
            Point pt = new Point(e.X, e.Y);

            // Convert to a point in the TreeView's coordinate system.
            pt = tree.PointToClient(pt);

            // Get the node underneath the mouse.
            TreeNode node = tree.GetNodeAt(pt);

            if (IsFolderNode(node))
            {
                Log.DebugFormat("Drag drop");

                TreeNode nodePayload = (TreeNode)e.Data.GetData(typeof(TreeNode));
                SessionFolderData newParent = (SessionFolderData)node.Tag;
                if (IsFolderNode(nodePayload))
                {
                    SessionFolderData nodeToMove = (SessionFolderData)nodePayload.Tag;
                    SuperPuTTY.GetRootFolderData().MoveTo(newParent, nodeToMove);
                }
                else {
                    SessionData nodeToMove = (SessionData)nodePayload.Tag;
                    SuperPuTTY.GetRootFolderData().MoveTo(newParent, nodeToMove);
                }

                DatabaseManager.Instance.Save();
                CreateTreeview();

                TreeNode selectedNodeParent = getTreeNode(nodeRoot, newParent);
                if (selectedNodeParent != null)
                {
                    selectedNodeParent.Expand();
                }
                

                TreeNode selectedNodeChild = null;
                if (IsFolderNode(nodePayload))
                {
                    selectedNodeChild = getTreeNode(nodeRoot, (SessionFolderData)nodePayload.Tag);    
                } else{
                    selectedNodeChild = getTreeNode(nodeRoot, (SessionData)nodePayload.Tag);
                }

                treeView1.SelectedNode = selectedNodeChild;

                // auto save settings...use timer to prevent excessive saves while dragging and dropping nodes
                timerDelayedSave.Stop();
                timerDelayedSave.Start();
                //SuperPuTTY.SaveSessions();
            }
        }

        public TreeNode getTreeNode(TreeNode treeNodeParent, SessionFolderData folderData)
        {
            foreach (TreeNode treeNode in treeNodeParent.Nodes)
            {
                if (IsFolderNode(treeNode))
                {
                    SessionFolderData tagData = (SessionFolderData) treeNode.Tag;
                    if (tagData == folderData)
                    {
                        return treeNode;
                    }
                }
                TreeNode childTreeNode = getTreeNode(treeNode, folderData);
                if (childTreeNode != null)
                {
                    return childTreeNode;
                }
            }
            return null;
        }

        public TreeNode getTreeNode(TreeNode treeNodeParent, SessionData sessionData)
        {
            foreach (TreeNode treeNode in treeNodeParent.Nodes)
            {
                if (IsSessionNode(treeNode))
                {
                    SessionData tagData = (SessionData)treeNode.Tag;
                    if (tagData == sessionData)
                    {
                        return treeNode;
                    }
                }

                TreeNode childTreeNode = getTreeNode(treeNode, sessionData);
                if (childTreeNode != null)
                {
                    return childTreeNode;
                }
            }
            return null;
        }

        #endregion

        private void timerDelayedSave_Tick(object sender, EventArgs e)
        {
            // stop timer
            timerDelayedSave.Stop();

            // do save
           // SuperPuTTY.SaveSessions();
            SuperPuTTY.ReportStatus("Saved Sessions after Drag-Drop @ {0}", DateTime.Now);
        }

        private void sessionFilterSelectionValue_Changed(object sender, EventArgs e) {
            //changed
            //int indexSelection = sessionFilter.SelectedIndex;

            CreateTreeview();
        }
        #region Icon
        bool IsValidImage(string imageKey)
        {
            bool valid = false;
            if (!string.IsNullOrEmpty(imageKey))
            {
                valid = this.treeView1.ImageList.Images.ContainsKey(imageKey);
                if (!valid)
                {
                    Log.WarnFormat("Missing icon, {0}", imageKey);
                }
            }
            return valid;
        }


        #endregion

        #region Search
        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            this.ApplySearch(this.txtSearch.Text);
            e.Handled = true;
            //e.SuppressKeyPress = true;
            /*
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    this.ApplySearch(this.txtSearch.Text);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Escape:
                    this.ClearSearch();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }*/
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.ApplySearch(this.txtSearch.Text);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.ClearSearch();
        }

        private void ClearSearch()
        {
            this.txtSearch.Text = "";
            this.ApplySearch("");
        }
        private void ApplySearch(string txt)
        {
            Log.InfoFormat("Applying Search: txt={0}.", txt);

            // define filter
            SearchFilter searchFilter = new SearchFilter(SuperPuTTY.Settings.SessionsSearchMode, txt);
            this.filter = searchFilter.IsMatch;

            // reload
            this.CreateTreeview();

            // if "clear" show init state otherwise expand all to show all matches
            /*if (string.IsNullOrEmpty(txt))
            {
                this.ExpandInitialTree();
            }
            else*/
            if (!string.IsNullOrEmpty(txt))
            {
                this.treeView1.ExpandAll();
            }
        }

        public enum SearchMode
        {
            CaseSensitive, CaseInSensitive, Regex
        }

        public class SearchFilter
        {
            public SearchFilter(string mode, string filter)
            {
                this.Mode = FormUtils.SafeParseEnum(mode, true, SearchMode.CaseSensitive);
                this.Filter = filter.Trim();
                if (this.Mode == SearchMode.Regex)
                {
                    try
                    {
                        this.Regex = new Regex(this.Filter);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Could not parse pattern: " + this.Filter, ex);
                    }
                }
            }
            public bool IsMatch(SessionData s)
            {
                if (this.Mode == SearchMode.CaseInSensitive)
                {
                    return s.SessionName.ToLower().Contains(this.Filter.ToLower());
                }
                else if (this.Mode == SearchMode.Regex)
                {
                    return this.Regex != null ? this.Regex.IsMatch(s.SessionName) : true;
                }
                else
                {
                    // case sensitive
                    return s.SessionName.Contains(this.Filter);
                }
            }
            public SearchMode Mode { get; set; }
            public string Filter { get; set; }
            public Regex Regex { get; set; }
        }
        #endregion

        #region Key Handling


        private void treeView1_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
                        // Get the tree
            TreeView tree = (TreeView)sender;

            // Get the node underneath the mouse.
            TreeNode node = e.Node;// e.Item as TreeNode;

            // Start the drag-and-drop operation with a cloned copy of the node.
            //if (node != null && IsSessionNode(node))
            if (node != null && tree.Nodes[0] != node)
            {
                if (IsFolderNode(node))
                {
                    SessionFolderData sessionFolderData = (SessionFolderData) node.Tag;
                    sessionFolderData.IsExpand = true;
                }
            }
        }

        private void treeView1_BeforeCollapse(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            // Get the tree
            TreeView tree = (TreeView)sender;

            // Get the node underneath the mouse.
            TreeNode node = e.Node;// e.Item as TreeNode;

            if (node == nodeRoot)
            {
                e.Cancel = true;
                return;
            }
            // Start the drag-and-drop operation with a cloned copy of the node.
            //if (node != null && IsSessionNode(node))
            if (node != null && tree.Nodes[0] != node)
            {
                if (IsFolderNode(node))
                {
                    SessionFolderData sessionFolderData = (SessionFolderData)node.Tag;
                    sessionFolderData.IsExpand = false;
                }
            }
        }

        private void treeView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13 && IsSessionNode(this.treeView1.SelectedNode))
            {
                if (Control.ModifierKeys == Keys.None)
                {
                    treeView1_NodeMouseDoubleClick(null, null);
                    e.Handled = true;
                }
                else if (Control.ModifierKeys == Keys.Shift)
                {
                    CreateOrEditSessionToolStripMenuItem_Click(this.settingsToolStripMenuItem, e);
                    e.Handled = true;
                }
            }
        }
        #endregion


        public enum SelectionFilter
        {
            ALL, ENABLED_ONLY, DISABLED_ONLY, ACTIVE_ONLY, UNACTIVE_ONLY
        };
    }

}
