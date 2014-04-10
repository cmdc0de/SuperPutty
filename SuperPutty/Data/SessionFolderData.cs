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

        public BindingList<SessionData> GetSessions(SessionFolderData parent)
        {
            List<SessionData> result = new List<SessionData>();
            foreach (SessionFolderData child in parent._SessionFolderDataChildren)
            {
                result.AddRange(GetSessions(child));
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

        public void RenameSessionFolderName(String lastFolderName, String newFolderName)
        {
            BindingList<SessionFolderData> listSessionFolderData = GetSessionFolderDataChildren();
            foreach (SessionFolderData sessionFolderData in listSessionFolderData)
            {
                if (sessionFolderData.Name == lastFolderName)
                {
                    sessionFolderData.Name = newFolderName;
                    return;
                }
            }
        }

        /*
        /// <summary>
        /// Read any existing saved sessions from the registry, decode and populate a list containing the data
        /// </summary>
        /// <returns>A list containing the entries retrieved from the registry</returns>
        public static List<SessionData> LoadSessionsFromRegistry()
        {
            Log.Info("LoadSessionsFromRegistry...");
            SessionRegistryHelper.LoadAllSessionsFromRegistry();

            List<SessionData> sessionList = new List<SessionData>();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SuperPuTTY\Sessions");
            if (key != null)
            {
                string[] sessionKeys = key.GetSubKeyNames();
                foreach (string session in sessionKeys)
                {
                    SessionData sessionData = new SessionData();
                    RegistryKey itemKey = key.OpenSubKey(session);
                    if (itemKey != null)
                    {
                        sessionData.Host = (string)itemKey.GetValue("Host", "");
                        sessionData.Port = (int)itemKey.GetValue("Port", 22);
                        sessionData.Proto = (ConnectionProtocol)Enum.Parse(typeof(ConnectionProtocol), (string)itemKey.GetValue("Proto", "SSH"));
                        sessionData.PuttySession = (string)itemKey.GetValue("PuttySession", "Default Session");
                        sessionData.SessionName = session;
                        //sessionData.SessionId = (string)itemKey.GetValue("SessionId", session);
                        sessionData.Username = (string)itemKey.GetValue("Login", "");
                        sessionData.LastDockstate = (DockState)itemKey.GetValue("Last Dock", DockState.Document);
                        sessionData.AutoStartSession = bool.Parse((string)itemKey.GetValue("Auto Start", "False"));
                        sessionList.Add(sessionData);
                    }
                }
            }
            return sessionList;
        }*/

        public void LoadSessionsFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                /*XmlSerializer s = new XmlSerializer(sessions.GetType());
                using (TextReader r = new StreamReader(fileName))
                {
                    sessions = (FolderData)s.Deserialize(r);
                }*/

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

            // sort and save file
            //sessions.Sort();
            /*XmlSerializer s = new XmlSerializer(this.GetType());
            using (TextWriter w = new StreamWriter(fileName))
            {
                s.Serialize(w, this);
            }*/

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
                if (GetParent(p, sessionDataToSearch) != null)
                {
                    return p;
                }
            }
            return null;
        }


        public void moveTo(SessionFolderData parent, SessionFolderData folderDataToMove)
        {
            // remove it
            SessionFolderData pp = GetParent(this, folderDataToMove);
            pp._SessionFolderDataChildren.Remove(folderDataToMove);

            // add to new space
            parent._SessionFolderDataChildren.Add(folderDataToMove);
        }

        public void moveTo(SessionFolderData parent, SessionData sessionDataToMove)
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
        public void moveUp(SessionFolderData parent, SessionFolderData folderDataToMove)
        {
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
                }
            }
            /*foreach (FolderData f in folderChildren) {
                moveUp(f, folderDataToMove);
            }*/
        }

        public void moveDown(int index)
        {
            SessionData sessionData = _SessionDataChildren[index];
            _SessionDataChildren.RemoveAt(index);
            _SessionDataChildren.Insert(index, sessionData);
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
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("folder"))
                {
                    SessionFolderData subFolderData = new SessionFolderData(reader.GetAttribute("name"));
                    _SessionFolderDataChildren.Add(subFolderData);

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

                    stack.Peek().AddSession(sessionData);
                }
            }
            reader.Close();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("folders");

            foreach (SessionFolderData myObject in _SessionFolderDataChildren)
            {
                writer.WriteStartElement("folder");
                writer.WriteAttributeString("name", myObject.Name);
                WriteSessionData(writer, myObject.GetSessionDataChildren());
                writer.WriteEndElement();
            }

            WriteSessionData(writer, _SessionDataChildren);
           
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        private void WriteSessionData(XmlWriter writer, BindingList<SessionData> sList)
        {
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
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        #endregion
    }

    
    /*
        static void WorkaroundCygwinBug()
        {
            try
            {
                // work around known bug with cygwin
                Dictionary<string, string> envVars = new Dictionary<string, string>();
                foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
                {
                    string envVar = (string) de.Key;
                    if (envVars.ContainsKey(envVar.ToUpper()))
                    {
                        // duplicate found... (e.g. TMP and tmp)
                        Log.DebugFormat("Clearing duplicate envVariable, {0}", envVar);
                        Environment.SetEnvironmentVariable(envVar, null);
                        continue;
                    }
                    envVars.Add(envVar.ToUpper(), envVar);
                }

            }
            catch (Exception ex)
            {
                Log.WarnFormat("Error working around cygwin issue: {0}", ex.Message);
            }
        }*/


}
