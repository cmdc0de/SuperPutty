﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperPuTTY.Manager;
using System.ComponentModel;
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;

namespace SuperPuTTY.Utils
{
    class SessionRegistryHelper
    {
        private static String REGISTER_BASENAME = @"Software\SuperPuTTY\Sessions";


        private static void CreateSubKeySessionFolderData(SessionFolderData folderData, String prefixRegister) {
            String registerName = prefixRegister + "\\" + folderData.Name;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(registerName, true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(registerName);
            }

            if (key != null)
            {
                key.SetValue("Name", folderData.Name);
                key.Close();
            }
            else
            {
                Logger.Log("Unable to create registry key [folderData] with name = " + folderData.Name);
            }

            foreach (SessionData sessionData in folderData.SessionDataChildren)
            {
                CreateSubKeySessionData(sessionData, registerName);
            }
            foreach (SessionFolderData sessionFolderData in folderData.SessionFolderDataChildren)
            {
                CreateSubKeySessionFolderData(sessionFolderData, registerName);
            }
        }

        private static void CreateSubKeySessionData(SessionData sessionData, String prefixRegister) {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(prefixRegister + "\\" + sessionData.SessionName, true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(prefixRegister + "\\" + sessionData.SessionName);
            }

            if (key != null)
            {
                key.SetValue("Name", sessionData.SessionName);
                key.SetValue("Host", sessionData.Host);
                key.SetValue("Port", sessionData.Port);
                key.SetValue("Proto", sessionData.Proto);
                key.SetValue("PuttySession", sessionData.PuttySession);

                if (!String.IsNullOrEmpty(sessionData.Username)) {
                    key.SetValue("Login", sessionData.Username);
                }
                if (sessionData.LastDockstate != DockState.Hidden && sessionData.LastDockstate != DockState.Unknown) {
                    key.SetValue("Last Dock", (int)sessionData.LastDockstate);
                }

                key.SetValue("Auto Start", sessionData.AutoStartSession);

                key.Close();
            }
            else
            {
                Logger.Log("Unable to create registry key [sessionData] with name = " + sessionData.SessionName);
            }
        }

        public static void SaveAllSessionsToRegistry() {
            SessionFolderData rootFolderData = SuperPuTTY.GetRootFolderData();
            foreach(SessionFolderData folderChild in rootFolderData.SessionFolderDataChildren)
            {
                CreateSubKeySessionFolderData(folderChild, REGISTER_BASENAME);
            }

            foreach (SessionData sessionData in rootFolderData.SessionDataChildren)
            {
                CreateSubKeySessionData(sessionData, REGISTER_BASENAME);
            }
        }


        public static void LoadAllSessionsFromRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTER_BASENAME);
            if (key != null)
            {
                string[] sessionKeys = key.GetSubKeyNames();
                foreach (string sessionKey in sessionKeys)
                {
                    Logger.Log(sessionKey);
                }
            }
        }
    }
}
