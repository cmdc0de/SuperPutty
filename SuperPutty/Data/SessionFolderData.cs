using System.Collections.Generic;
using log4net;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using System;
using System.Xml;
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;
using SuperPutty.Utils;
using System.Collections;

namespace SuperPutty.Data
{
    public class SessionFolderData : AbstractSessionData, IXmlSerializable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SessionFolderData));

        private String _Name;
        private BindingList<SessionData> _SessionDataChildren = new BindingList<SessionData>();
        private BindingList<SessionFolderData> _SessionFolderDataChildren = new BindingList<SessionFolderData>();
        private bool _IsExpand = false;

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
            result.AddRange(parent.GetSessionDataChildren());
            return new BindingList<SessionData>(result);
        }

        public BindingList<SessionData> GetSessionDataChildren()
        {
            return _SessionDataChildren;
        }

        public BindingList<SessionFolderData> GetSessionFolderDataChildren()
        {
            return _SessionFolderDataChildren;
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

        public void LoadSessionsFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                XmlTextReader reader = new XmlTextReader(fileName);
                this.ReadXml(reader);
            }
            else
            {
                Log.WarnFormat("Could not load sessions, file doesn't exist.  file={0}", fileName);
            }
            return;
        }


        private static void BackUpFiles(string fileName, int count)
        {
            if (File.Exists(fileName) && count > 0)
            {
                try
                {
                    // backup
                    string fileBaseName = Path.GetFileNameWithoutExtension(fileName);
                    string dirName = Path.GetDirectoryName(fileName);
                    string backupName = Path.Combine(dirName, string.Format("{0}.{1:yyyyMMdd_hhmmss}.XML", fileBaseName, DateTime.Now));
                    File.Copy(fileName, backupName, true);

                    // limit last count saves
                    List<string> oldFiles = new List<string>(Directory.GetFiles(dirName, fileBaseName + ".*.XML"));
                    oldFiles.Sort();
                    oldFiles.Reverse();
                    if (oldFiles.Count > count)
                    {
                        for (int i = 20; i < oldFiles.Count; i++)
                        {
                            Log.InfoFormat("Cleaning up old file, {0}", oldFiles[i]);
                            File.Delete(oldFiles[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error backing up files", ex);
                }
            }
        }

        public void SaveToFile(string fileName)
        {
            Log.InfoFormat("Saving sessions to {0}", fileName);

            BackUpFiles(fileName, 20);

            XmlTextWriter writer = new XmlTextWriter(fileName, null);
            writer.Formatting = Formatting.Indented;
            this.WriteXml(writer);
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

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            _SessionFolderDataChildren.Clear();
            _SessionDataChildren.Clear();

            SessionFolderData parent = SuperPuTTY.GetRootFolderData();
            Stack<SessionFolderData> stack = new Stack<SessionFolderData>();
            stack.Push(parent);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("folders"))
                {

                } 
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("folders"))
                {

                } 
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("folder"))
                {
                    String name = reader.GetAttribute("name");
                    SessionFolderData subFolderData = new SessionFolderData(name);
                    stack.Peek().GetSessionFolderDataChildren().Add(subFolderData);

                    stack.Push(subFolderData);
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("folder"))
                {
                    stack.Pop();
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("session"))
                {

                    SessionData sessionData = new SessionData();
                    sessionData.SessionName = reader.GetAttribute("name");
                    sessionData.ImageKey = reader.GetAttribute("imagekey");
                    sessionData.Host = reader.GetAttribute("host");
                    String portStr = reader.GetAttribute("port");
                    if (portStr != null)
                    {
                        sessionData.Port = int.Parse(portStr);
                    }
                    String protoStr = reader.GetAttribute("proto");
                    if (protoStr != null) { 
                        sessionData.Proto = (ConnectionProtocol) Enum.Parse(typeof(ConnectionProtocol), reader.GetAttribute("proto"), true);
                    }
                    sessionData.PuttySession = reader.GetAttribute("puttysession");
                    sessionData.Username = reader.GetAttribute("username");
                    sessionData.ExtraArgs = reader.GetAttribute("extraargs");
                    sessionData.Enabled = (reader.GetAttribute("enabled") == "true");

                    stack.Peek().AddSession(sessionData);
                }
            }
            reader.Close();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("root");
            WriteSessionFolderData(writer, SuperPuTTY.GetRootFolderData());
            WriteSessionData(writer, SuperPuTTY.GetRootFolderData().GetSessionDataChildren());
            writer.WriteEndElement(); 
            writer.WriteEndDocument();
            writer.Close();
        }

        private void WriteSessionFolderData(XmlWriter writer, SessionFolderData parent) {
            if (parent.GetSessionFolderDataChildren().Count == 0 && parent.GetSessionDataChildren().Count == 0)
            {
                writer.WriteStartElement("folders");
                writer.WriteEndElement();
                return;
            } 
            writer.WriteStartElement("folders");

            foreach (SessionFolderData sessionFolderData in parent.GetSessionFolderDataChildren())
            {
                writer.WriteStartElement("folder");
                writer.WriteAttributeString("name", sessionFolderData.Name);
                WriteSessionFolderData(writer, sessionFolderData);
                WriteSessionData(writer, sessionFolderData.GetSessionDataChildren());
                writer.WriteEndElement();
            }

            //WriteSessionData(writer, parent.GetSessionDataChildren());
           
            writer.WriteEndElement();
        }

        private void WriteSessionData(XmlWriter writer, BindingList<SessionData> sList)
        {
            if (sList.Count == 0)
            {
                return;
            }
            writer.WriteStartElement("sessions");
            foreach (SessionData sessionData in sList)
            {
                
                //XmlSerializer xs = new XmlSerializer(sessionData.GetType());
                //xs.Serialize(writer, sessionData, null);
                writer.WriteStartElement("session");
                writer.WriteAttributeString("name", sessionData.SessionName);
                writer.WriteAttributeString("imagekey", sessionData.ImageKey);
                writer.WriteAttributeString("host", sessionData.Host);
                writer.WriteAttributeString("port", sessionData.Port.ToString());
                writer.WriteAttributeString("proto", sessionData.Proto.ToString());
                writer.WriteAttributeString("puttysession", sessionData.PuttySession);
                writer.WriteAttributeString("username", sessionData.Username);
                writer.WriteAttributeString("extraargs", sessionData.ExtraArgs);
                writer.WriteAttributeString("enabled", sessionData.Enabled.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        #endregion
    }

}
