using System.Collections.Generic;
using log4net;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using System;
using System.Xml;
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;
using SuperPuTTY.Utils;
using System.Collections;

namespace SuperPuTTY.Manager
{
    public class SessionFolderData : AbstractSessionData //, IXmlSerializable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SessionFolderData));

        private String _Name;
        private BindingList<SessionData> _SessionDataChildren = new BindingList<SessionData>();
        private BindingList<SessionFolderData> _SessionFolderDataChildren = new BindingList<SessionFolderData>();
        private bool _IsExpand = false;
        private int _ParentFolderId;
        private int _Position;

        public int Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        public int ParentFolderId
        {
            get { return _ParentFolderId; }
            set { _ParentFolderId = value; }
        }


        public bool IsExpand
        {
            get { return _IsExpand; }
            set { _IsExpand = value; }
        }

        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public BindingList<SessionData> SessionDataChildren
        {
            get { return _SessionDataChildren; }
            set { _SessionDataChildren = value; }
        }

        public BindingList<SessionFolderData> SessionFolderDataChildren
        {
            get { return _SessionFolderDataChildren; }
            set { _SessionFolderDataChildren = value; }
        }

        public SessionFolderData()
        {

        }
        public SessionFolderData(String name)
        {
            
            this.Name = name;
        }
        public void AddSession(SessionData sessionData) {
            this._SessionDataChildren.Add(sessionData);
        }

        public BindingList<SessionData> GetSessionsList(SessionFolderData parent)
        {
            List<SessionData> result = new List<SessionData>();
            foreach (SessionFolderData child in parent._SessionFolderDataChildren)
            {
                result.AddRange(GetSessionsList(child));
            }
            result.AddRange(parent.SessionDataChildren);
            return new BindingList<SessionData>(result);
        }

        public SessionFolderData AddChildFolderData(String folderDataName)
        {
            SessionFolderData child = new SessionFolderData(folderDataName);

            _SessionFolderDataChildren.Add(child);
            return child;
        }

        public void RenameSessionFolderName(SessionFolderData sessionFolderData, String newFolderName)
        {
            sessionFolderData.Name = newFolderName;
        }

        public SessionFolderData GetParent(SessionFolderData parent, SessionFolderData folderDataToSearch) {
            int index = parent._SessionFolderDataChildren.IndexOf(folderDataToSearch);
            if (index >= 0)
            {
                return parent;
            }
            foreach (SessionFolderData p in parent._SessionFolderDataChildren)
            {
                if (GetParent(p, folderDataToSearch) != null)
                {
                    return p;
                }
            }
            return null;
        }

        public SessionFolderData GetParent(SessionFolderData parent, SessionData sessionDataToSearch)
        {
            int index = parent._SessionDataChildren.IndexOf(sessionDataToSearch);
            if (index >= 0)
            {
                return parent;
            }
            foreach (SessionFolderData p in parent._SessionFolderDataChildren)
            {
                SessionFolderData pp = GetParent(p, sessionDataToSearch);
                if (pp != null)
                {
                    return pp;
                }
            }
            return null;
        }


        public void MoveTo(SessionFolderData parent, SessionFolderData folderDataToMove)
        {
            // remove it
            SessionFolderData pp = GetParent(this, folderDataToMove);
            pp._SessionFolderDataChildren.Remove(folderDataToMove);

            // add to new space
            parent._SessionFolderDataChildren.Add(folderDataToMove);
        }

        public void MoveTo(SessionFolderData parent, SessionData sessionDataToMove)
        {
            RemoveSession(sessionDataToMove);

            // add to new space
            parent._SessionDataChildren.Add(sessionDataToMove);
        }

        public void RemoveSession(SessionData sessionDataToRemove)
        {
            // remove it
            SessionFolderData pp = GetParent(this, sessionDataToRemove);
            pp._SessionDataChildren.Remove(sessionDataToRemove);
        }
        
        public void RemoveFolder(SessionFolderData sessionFolderDataToRemove)
        {
            // remove it
            SessionFolderData pp = GetParent(this, sessionFolderDataToRemove);
            pp._SessionFolderDataChildren.Remove(sessionFolderDataToRemove);
        }

        public int MoveUp(SessionFolderData parent, SessionFolderData folderDataToMove)
        {
            // TODO : move Up d'un sous dossier
            int index = _SessionFolderDataChildren.IndexOf(folderDataToMove);
            if (index >= 0)
            {
                if (index == 0)
                {
                    // nothing because it's already the first element
                }
                else
                {
                    _SessionFolderDataChildren.RemoveAt(index);
                    _SessionFolderDataChildren.Insert(index - 1, folderDataToMove);
                    return index - 1;
                }
            }
            return 0;
        }

        public void MoveDown(int index)
        {
            SessionFolderData sessionFolderData = _SessionFolderDataChildren[index];
            _SessionFolderDataChildren.RemoveAt(index);
            _SessionFolderDataChildren.Insert(index + 1, sessionFolderData);
        }
    }

}
