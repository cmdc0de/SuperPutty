/* +
 * Created by SharpDevelop.
 * User: Paul Hendryx
 * Date: 11/27/2010
 * Time: 10:25 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using Microsoft.Win32;
using SuperPutty.Utils;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace SuperPutty.Manager
{
    public class DatabaseManager
    {
        private static String REGISTRY_NAME = @"Software\SuperPuTTY\Salts";
        private enum REGISTRY_DATABASE_SALT_KEY
        {
            salt
        };

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

           // return (db.ExecuteCountQuery("SELECT count(*) FROM sqlite_master;") > 0);
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
                _conn = new System.Data.SQLite.SQLiteConnection(connection_string);
                _conn.Open();              
                
                CreateOrUpdateTables();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return _conn;
        }


        public static string getFolderPath()
        {
            return Path.Combine(Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "SuperPutty"), "data");
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
                    string sql = "CREATE TABLE IF NOT EXISTS session (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, parent_folder_id int, position integer, label varchar(255), image_key varchar(255), host varchar(255), port integer, proto varchar(255), putty_session varchar(255), username varchar(255));";
                    SQLiteCommand cmd = new SQLiteCommand(sql, _conn);
                    cmd.ExecuteNonQuery();
                }

                {
                    string sql = "CREATE TABLE IF NOT EXISTS folders (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, parent_id integer, position integer, is_expand boolean, label varchar(255));";
                    SQLiteCommand cmd = new SQLiteCommand(sql, _conn);
                    cmd.ExecuteNonQuery();
                }
            }
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