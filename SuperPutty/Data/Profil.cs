using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperPuTTY.Manager
{
    class Profil
    {
        private int _id;
        public int id
        {
            get { return _id; }
            set { _id = value; }
        }
        
        private string _label;
        public string label
        {
            get { return _label; }
            set { _label = value; }
        }

        private string _hash;
        public string hash
        {
            get { return _hash; }
            set { _hash = value; }
        }

    }
}
