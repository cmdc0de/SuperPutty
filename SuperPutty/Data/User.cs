using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperPuTTY.Manager
{
    class User
    {
        private string _login;
        public string login
        {
            get { return _login; }
            set {
                _login = value;
            }
        }
    }
}
