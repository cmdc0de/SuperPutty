﻿/* +
 * Created by SharpDevelop.
 * User: Paul Hendryx
 * Date: 11/27/2010
 * Time: 10:25 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using Microsoft.Win32;
using SuperPuTTY.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace SuperPuTTY.Manager
{
    public class DatabaseManager
    {
        private static String REGISTRY_NAME = @"Software\SuperPuTTY\Salts";
        private enum REGISTRY_DATABASE_SALT_KEY
        {
            salt
        };

        public bool isOpened = false;
        private SQLiteConnection _conn;
        private static DatabaseManager instance;
        private DatabaseManager() { }

        public static DatabaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseManager();
                }
                return instance;
            }
        }

        /*
        public DatabaseManager(string label, string hash)
        {
            this._label = label;
            this._passwordhash = hash;
        }*/

        public string CreateDatabase(string label, string password)
        {

            string db_folder = getFolderPath();
            if (!Directory.Exists(db_folder))
            {
                Directory.CreateDirectory(db_folder);
            }

            string dblocation = Path.Combine(db_folder, label + ".db");

            if (!File.Exists(dblocation))
            {
                SQLiteConnection.CreateFile(dblocation);
            }

            string salt = BCrypt.GenerateSalt(12);
            //backup salt in registry
            addSaltToRegistry(dblocation, salt);

            //DatabaseManager db = new DatabaseManager(label, hash);
            SQLiteConnection con = Open(dblocation, password);

            return dblocation;
        }



        public SQLiteConnection Open(string dblocation, string password) 
        {
            try
            {

                // get the hash
                string salt = getSalt(dblocation);
                if (salt == null)
                {
                    MessageBox.Show("Error salt not found in registry!");
                    throw new Exception();
                }

                string hash = BCrypt.HashPassword(password, salt);
                string connection_string = "Data Source=" + dblocation + ";Version=3;Password=" + hash + ";";
                _conn = new SQLiteConnection(connection_string);
                _conn.Open();              
                
                CreateOrUpdateTables();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }

            isOpened = true;
            LoadSessions();

            return _conn;
        }

        public void Close()
        {
            try { 
                _conn.Close();
            } catch(Exception ex) {
                Logger.Log(ex);
            }
            isOpened = false;
        }

        public static string getFolderPath()
        {
            return Path.Combine(Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "SuperPuTTY"), "data");
        }

        public DataTable FillDataTable(string sql)
        {
            DataTable dt = new DataTable();

            SQLiteCommand cmd = new SQLiteCommand(sql);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }


        private void CreateOrUpdateTables()
        {
            // Settings table
            if (_conn.GetSchema("Tables").Select("Table_Name = 'sessions'").Length == 0)
            {
                /*string sql = "create table if not exists sessions (id INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT, auto_start boolean, host varchar(255), last_dock integer, ";
                sql += "last_path varchar(255), port integer, proto varchar(255), putty_session varchar(255));";*/
                {
                    string sql = "CREATE TABLE IF NOT EXISTS session (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, parent_folder_id integer, position integer, label varchar(255), image_key varchar(255), host varchar(255), port integer, proto varchar(255), putty_session varchar(255), username varchar(255));";
                    SQLiteCommand cmd = new SQLiteCommand(sql, _conn);
                    cmd.ExecuteNonQuery();
                }

                {
                    string sql = "CREATE TABLE IF NOT EXISTS folder (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, parent_id integer, position integer, is_expand boolean, label varchar(255));";
                    SQLiteCommand cmd = new SQLiteCommand(sql, _conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Save()
        {
            ExecuteNonQuery("DELETE FROM folder");
            ExecuteNonQuery("DELETE FROM session");

            int position = 0;
            foreach (SessionData sd in SuperPuTTY.GetRootFolderData().GetSessionsList(SuperPuTTY.GetRootFolderData()))
            {
                InsertSession(SuperPuTTY.GetRootFolderData(), position, sd);
                position++;
            }
        }
        public void InsertFolder(SessionFolderData sessionFolderData)
        {
            string sql = "INSERT INTO folder (parent_id, position, is_expand, label) VALUES ";
            sql += "(@parent_id, @position, @is_expand, @label)";

            SQLiteCommand cmd = new SQLiteCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@parent_id", sessionFolderData.ParentFolderId);
            cmd.Parameters.AddWithValue("@position", sessionFolderData.Position);
            cmd.Parameters.AddWithValue("@is_expand", sessionFolderData.IsExpand);
            cmd.Parameters.AddWithValue("@label", sessionFolderData.Name);
            
            int result = cmd.ExecuteNonQuery();
            if (result == 1)
            {
                MessageBox.Show("session inserted");
            }
            else
            {
                MessageBox.Show("fail to insert session");
            }
            cmd.Dispose();
        }
        public void InsertSession(SessionFolderData sessionFolderData, int position, SessionData sessionData)
        {
            string sql = "INSERT INTO session (parent_folder_id, position, label, image_key, host, port, proto, putty_session, username) VALUES ";
            sql += "(@parent_folder_id, @position, @label, @image_key, @host, @port, @proto, @putty_session, @username)";

            SQLiteCommand cmd = new SQLiteCommand(sql, _conn);
            cmd.Parameters.AddWithValue("@parent_folder_id", sessionFolderData.ParentFolderId);
            cmd.Parameters.AddWithValue("@position", position);
            cmd.Parameters.AddWithValue("@label", sessionData.SessionName);
            cmd.Parameters.AddWithValue("@image_key", sessionData.ImageKey);
            cmd.Parameters.AddWithValue("@host", sessionData.Host);
            cmd.Parameters.AddWithValue("@port", sessionData.Port);
            cmd.Parameters.AddWithValue("@proto", sessionData.Proto);
            cmd.Parameters.AddWithValue("@putty_session", sessionData.PuttySession);
            cmd.Parameters.AddWithValue("@username", sessionData.Username);
            
            int result = cmd.ExecuteNonQuery();
            if (result == 1)
            {
                MessageBox.Show("session inserted");
            }
            else
            {
                MessageBox.Show("fail to insert session");
            }
            cmd.Dispose();
        }

        private void LoadSessions()
        {
            List<SessionData> result = getSessions();
            foreach(SessionData sd in result) {
                SuperPuTTY.GetRootFolderData().AddSession(sd);
            }
            
        }

        public List<SessionData> getSessions()
        {
            List<SessionData> result = new List<SessionData>();

            string mySelectQuery = "SELECT * FROM session";
            SQLiteCommand sqCommand = new SQLiteCommand(mySelectQuery, _conn);
            SQLiteDataReader sqReader = sqCommand.ExecuteReader();
            try
            {
                while (sqReader.Read())
                {
                    //Console.WriteLine(sqReader.GetInt32(0) + ", " + sqReader.GetString(1));
                    SessionData sessionData = new SessionData();
                    sessionData.SessionName = sqReader["label"].ToString();
                    sessionData.ImageKey = sqReader["image_key"].ToString();
                    sessionData.Host = sqReader["host"].ToString();
                    sessionData.Port = Convert.ToInt16(sqReader["port"].ToString());
                    sessionData.Proto = (ConnectionProtocol) Enum.Parse(typeof(ConnectionProtocol), (string) sqReader["proto"].ToString());
                    sessionData.PuttySession = sqReader["putty_session"].ToString();
                    sessionData.Username = sqReader["username"].ToString();
                    result.Add(sessionData);
                }
            }
            finally
            {
                // always call Close when done reading. 
                sqReader.Close();
            } 

            return result;
        }

        public int ExecuteNonQuery(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, _conn);
            return cmd.ExecuteNonQuery();
        }


        public int ExecuteCountQuery(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, _conn);
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) {
                return reader.GetInt32(0);
            }
            return -1;
        }


        private string getSalt(string dblocation)
        {
            RegistryKey parentKey = getParentKey();
            RegistryKey subKey = parentKey.OpenSubKey(dblocation.Replace("\\", "_"));
            string result = null;
            if (subKey != null)
            {
                result  =  (string) subKey.GetValue(REGISTRY_DATABASE_SALT_KEY.salt.ToString());
                subKey.Close();
            }
            parentKey.Close();
            return result;
        }

        private void addSaltToRegistry(string dblocation, string salt)
        {
            RegistryKey parentKey = getParentKey();
            RegistryKey subKey = parentKey.CreateSubKey(dblocation.Replace("\\", "_"));
            subKey.SetValue(REGISTRY_DATABASE_SALT_KEY.salt.ToString(), salt);
            subKey.Close();
            parentKey.Close();
        }

        private RegistryKey getParentKey()
        {
            RegistryKey parentKey = Registry.CurrentUser.OpenSubKey(REGISTRY_NAME, true);
            if (parentKey == null)
            {
                parentKey = Registry.CurrentUser.CreateSubKey(REGISTRY_NAME);
            }
            return parentKey;
        }
    }


       
}