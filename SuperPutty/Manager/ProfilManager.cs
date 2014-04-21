using Microsoft.Win32;
using SuperPutty.Manager;
using SuperPutty.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPutty.Manager
{
    class ProfilManager
    {
        private static String REGISTRY_NAME = @"Software\SuperPuTTY\Profils";
        private enum REGISTRY_PROFIL_KEY
        {
            id, label, hash
        };


        private static ProfilManager instance;
        private ProfilManager() {}

        public static ProfilManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProfilManager();
                }
                return instance;
            }
        }


        public void createProfil(string label, string login, string password)
        {
            RegistryKey parentKey = getParentKey();
            int id = parentKey.GetSubKeyNames().Length;
            string profilName = getProfilName(id);
            RegistryKey subKey = parentKey.CreateSubKey(profilName);

            string salt = BCrypt.GenerateSalt(12);
            string hash = BCrypt.HashPassword(login + password, salt);

            subKey.SetValue(REGISTRY_PROFIL_KEY.id.ToString(), id);
            subKey.SetValue(REGISTRY_PROFIL_KEY.label.ToString(), label);
            subKey.SetValue(REGISTRY_PROFIL_KEY.hash.ToString(), hash);

            subKey.Close();
            parentKey.Close();
        }

        public void updateProfil(int id, string newlabel)
        {
            RegistryKey parentKey = getParentKey();
            RegistryKey subKey = parentKey.OpenSubKey(getProfilName(id), true);
            subKey.SetValue(REGISTRY_PROFIL_KEY.label.ToString(), newlabel);
            subKey.Close();
            parentKey.Close();
        }

        public List<Profil> getProfils() {
            List<Profil> result = new List<Profil>();

            RegistryKey parentKey = getParentKey();

            string[] subKeysName = parentKey.GetSubKeyNames();
            foreach (string subKeyName in subKeysName)
            {
                Profil profil = getProfil(subKeyName);
                result.Add(profil);
            }
            parentKey.Close();

            return result;
        }

        public Profil getProfil(int id)
        {
            return getProfil(getProfilName(id));
        }

        private Profil getProfil(string subKeyName)
        {
            RegistryKey parentKey = getParentKey();
            RegistryKey subKey = parentKey.OpenSubKey(subKeyName);
            int id = (int)subKey.GetValue("id");
            string hash = (string)subKey.GetValue("hash");
            string label = (string)subKey.GetValue("label");

            Profil profil = new Profil();
            profil.id = id;
            profil.label = label;
            profil.hash = hash;

            subKey.Close();
            parentKey.Close();
            
            return profil;
        }

        private string getProfilName(int id)
        {
            return String.Format("profil_%2$d", id);
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
