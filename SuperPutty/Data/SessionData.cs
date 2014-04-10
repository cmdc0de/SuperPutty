/*
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
using System.Collections.Generic;
using System.Text;
using System.Net;
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;
using log4net;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Xml;

namespace SuperPutty.Data
{
    public enum ConnectionProtocol
    {
        SSH,
        SSH2,
        Telnet,
        Rlogin,
        Raw,
        Serial,
        Cygterm,
        Mintty,
        Auto
    }

    public class SessionData : IComparable, ICloneable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SessionData));


        /// <summary>
        /// Full session id (includes path for session tree)
        /// </summary>
        /*private string _SessionId;
        [XmlAttribute]
        public string SessionId
        {
            get { return this._SessionId; }
            set
            {
                this.OldSessionId = SessionId;
                this._SessionId = value;
            }
        }
        internal string OldSessionId { get; set; }*/

        private string _OldName;
        [XmlIgnore]
        public string OldName
        {
            get { return _OldName; }
            set { _OldName = value; }
        }

        private string _SessionName;
        [XmlAttribute]
        public string SessionName
        {
            get { return _SessionName; }
            set { OldName = _SessionName; 
                _SessionName = value;
               /* if (SessionId == null)
                {
                    SessionId = value;
                }*/
            }
        }

        private string _ImageKey;
        [XmlAttribute]
        public string ImageKey
        {
            get { return _ImageKey; }
            set { _ImageKey = value; }
        }

        private string _Host;
        [XmlAttribute]
        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }

        private int _Port;
        [XmlAttribute]
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        private ConnectionProtocol _Proto;
        [XmlAttribute]
        public ConnectionProtocol Proto
        {
            get { return _Proto; }
            set { _Proto = value; }
        }

        private string _PuttySession;
        [XmlAttribute]
        public string PuttySession
        {
            get { return _PuttySession; }
            set { _PuttySession = value; }
        }

        private string _Username;
        [XmlAttribute]
        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }

        private string _Password;
        [XmlIgnore]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private string _ExtraArgs;
        [XmlAttribute]
        public string ExtraArgs
        {
            get { return _ExtraArgs; }
            set { _ExtraArgs = value; }
        }

        /* Unused...ignore for now
        private string _LastPath = ".";
        public string LastPath
        {
            get { return _LastPath; }
            set { _LastPath = value; }
        }*/

        private DockState m_LastDockstate = DockState.Document;
        [XmlIgnore]
        public DockState LastDockstate
        {
            get { return m_LastDockstate; }
            set { m_LastDockstate = value; }
        }

        private bool m_AutoStartSession = false;
        [XmlIgnore]
        public bool AutoStartSession
        {
            get { return m_AutoStartSession; }
            set { m_AutoStartSession = value; }
        }

        public SessionData(string sessionName, string hostName, int port, ConnectionProtocol protocol, string sessionConfig)
        {
            SessionName = sessionName;
            Host = hostName;
            Port = port;
            Proto = protocol;
            PuttySession = sessionConfig;
        }
        
        public SessionData()
        {
            Proto = ConnectionProtocol.Auto;
        }

        /*internal void SaveToRegistry()
        {
            if (!String.IsNullOrEmpty(this.SessionName)
                && !String.IsNullOrEmpty(this.Host)
                && this.Port >= 0)
            {

                // detect if session was renamed
                if (!String.IsNullOrEmpty(this.OldName) && this.OldName != this.SessionName)
                {
                    RegistryRemove(this.OldName);
                }

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SuperPuTTY\Sessions\" + this.SessionName, true);
                if (key == null)
                {
                    key = Registry.CurrentUser.CreateSubKey(@"Software\SuperPuTTY\Sessions\" + this.SessionName);
                }

                if (key != null)
                {                   
                    key.SetValue("Host", this.Host);
                    key.SetValue("Port", this.Port);
                    key.SetValue("Proto", this.Proto);
                    key.SetValue("PuttySession", this.PuttySession);
                    
                    if(!String.IsNullOrEmpty(this.Username))
                        key.SetValue("Login", this.Username);

                    //key.SetValue("Last Path", this.LastPath);

                    if(this.LastDockstate != DockState.Hidden && this.LastDockstate != DockState.Unknown)
                        key.SetValue("Last Dock", (int)this.LastDockstate);                    

                    key.SetValue("Auto Start", this.AutoStartSession);

                    *if (this.SessionId != null)
                    {
                        key.SetValue("SessionId", this.SessionId);
                    }
                    key.Close();
                }
                else
                {
                    Logger.Log("Unable to create registry key for " + this.SessionName);
                }
            }
        }

        void RegistryRemove(string sessionName)
        {
            if (!String.IsNullOrEmpty(sessionName))
            {
                Log.DebugFormat("Removing session, name={0}", sessionName);
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SuperPuTTY\Sessions", true);
                try
                {
                    if (key.OpenSubKey(sessionName) != null)
                    {
                        key.DeleteSubKeyTree(sessionName);
                        key.Close();
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    Logger.Log(e);
                }
            }
        }*/

        public int CompareTo(object obj)
        {
            SessionData s = obj as SessionData;
            return s == null ? 1 : this.SessionName.CompareTo(s.SessionName);
        }

        /*public static string CombineSessionIds(string parent, string child) 
        {
            if (parent == null && child == null)
            {
                return null;
            } 
            else if (child == null) 
            {
                return parent;
            }
            else if (parent == null)
            {
                return child;
            }
            else
            {
                return parent + "/" + child;
            }
        }

        public static string GetSessionNameFromId(string sessionId)
        {
            string[] parts = GetSessionNameParts(sessionId);
            return parts.Length > 0 ? parts[parts.Length - 1] : sessionId;
        }

        public static string[] GetSessionNameParts(string sessionId)
        {
            return sessionId.Split('/');
        }

        public static string GetSessionParentId(string sessionId)
        {
            string parentPath = null;
            if (sessionId != null)
            {
                int idx = sessionId.LastIndexOf('/');
                if (idx != -1)
                {
                    parentPath = sessionId.Substring(0, idx);
                }
            }
            return parentPath;
        }*/


        public object Clone()
        {
            SessionData session = new SessionData();
            foreach (PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(session, pi.GetValue(this, null), null);
                }
            }
            return session;
        }

        public override string ToString()
        {

            if (this.Proto == ConnectionProtocol.Cygterm || this.Proto == ConnectionProtocol.Mintty)
            {
                return string.Format("{0}://{1}", this.Proto.ToString().ToLower(), this.Host);
            }
            else
            {
                return string.Format("{0}://{1}:{2}", this.Proto.ToString().ToLower(), this.Host, this.Port);
            }
        }

    }
}
