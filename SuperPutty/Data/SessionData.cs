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

namespace SuperPuTTY.Manager
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

        private string _SessionName;
        public string SessionName
        {
            get { return _SessionName; }
            set {
                _SessionName = value;
            }
        }

        private string _ImageKey;
        public string ImageKey
        {
            get { return _ImageKey; }
            set { _ImageKey = value; }
        }

        private string _Host;
        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }

        private int _Port;
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        private ConnectionProtocol _Proto;
        
        public ConnectionProtocol Proto
        {
            get { return _Proto; }
            set { _Proto = value; }
        }

        private string _PuttySession;
        
        public string PuttySession
        {
            get { return _PuttySession; }
            set { _PuttySession = value; }
        }

        private string _Username;
        
        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private string _ExtraArgs;
        
        public string ExtraArgs
        {
            get { return _ExtraArgs; }
            set { _ExtraArgs = value; }
        }

        private DockState m_LastDockstate = DockState.Document;
        public DockState LastDockstate
        {
            get { return m_LastDockstate; }
            set { m_LastDockstate = value; }
        }

        private bool m_AutoStartSession = false;
        public bool AutoStartSession
        {
            get { return m_AutoStartSession; }
            set { m_AutoStartSession = value; }
        
        }

        private bool _Enabled;
        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }

        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; }
        }


        public SessionData(string sessionName, string hostName, int port, ConnectionProtocol protocol, string sessionConfig)
        {
            SessionName = sessionName;
            Host = hostName;
            Port = port;
            Proto = protocol;
            PuttySession = sessionConfig;
            this.Enabled = true;
        }
        
        public SessionData()
        {
            Proto = ConnectionProtocol.Auto;
            this.Enabled = true;
        }

        public int CompareTo(object obj)
        {
            SessionData s = obj as SessionData;
            return s == null ? 1 : this.SessionName.CompareTo(s.SessionName);
        }

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
