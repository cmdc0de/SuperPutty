﻿using System.Collections.Generic;
using log4net;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using System;
using System.Xml;

namespace SuperPutty.Data
{
    public class FolderData : IXmlSerializable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FolderData));

        private String _Name;
        private BindingList<SessionData> sessionsList = new BindingList<SessionData>();
        private BindingList<FolderData> folderChildren = new BindingList<FolderData>();

        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public FolderData()
        {

        }
        public FolderData(String name)
        {
            Name = name;
        }
        public void AddSession(SessionData sessionData) {
            sessionsList.Add(sessionData);
        }

        public BindingList<SessionData> GetSessions()
        {
            return sessionsList;
        }

        public BindingList<FolderData> GetChildren()
        {
            return folderChildren;
        }
        public FolderData AddChildFolderData(String folderDataName)
        {
            FolderData child = new FolderData(folderDataName);

            folderChildren.Add(child);
            return child;
        }

        /// <summary>
        /// Read any existing saved sessions from the registry, decode and populate a list containing the data
        /// </summary>
        /// <returns>A list containing the entries retrieved from the registry</returns>
        public static List<SessionData> LoadSessionsFromRegistry()
        {
            Log.Info("LoadSessionsFromRegistry...");
            List<SessionData> sessionList = new List<SessionData>();
            /*RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Jim Radford\SuperPuTTY\Sessions");
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
                        sessionData.SessionId = (string)itemKey.GetValue("SessionId", session);
                        sessionData.Username = (string)itemKey.GetValue("Login", "");
                        sessionData.LastDockstate = (DockState)itemKey.GetValue("Last Dock", DockState.Document);
                        sessionData.AutoStartSession = bool.Parse((string)itemKey.GetValue("Auto Start", "False"));
                        sessionData.Position = (int)itemKey.GetValue("Position", 0);
                        sessionList.Add(sessionData);
                    }
                }
            }*/
            return sessionList;
        }

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


        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            folderChildren.Clear();
            while (!reader.EOF)
            {
                if (reader.ReadToFollowing("folder")) {
                    FolderData subFolderData = new FolderData(reader.ReadString());
                    folderChildren.Add(subFolderData);
                }
                if (reader.ReadToFollowing("sessions"))
                {
                    SessionData sessionData = new SessionData();
                    sessionsList.Add(sessionData);
                }
            }
            reader.Close();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("folders");

            foreach (FolderData myObject in folderChildren)
            {
                writer.WriteElementString("folder", myObject.Name);
                WriteSessionData(writer, myObject.GetSessions());
            }

            WriteSessionData(writer, sessionsList);
           
            writer.WriteEndElement(); // close Items tag
            writer.WriteEndDocument();
            writer.Close();
        }

        private void WriteSessionData(XmlWriter writer, BindingList<SessionData> sList)
        {
            foreach (SessionData sessionData in sList)
            {
                writer.WriteStartElement("sessions");
                XmlSerializer xs = new XmlSerializer(sessionData.GetType());
                xs.Serialize(writer, sessionData, null);
                writer.WriteEndElement();
            }
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
