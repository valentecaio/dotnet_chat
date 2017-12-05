using System;
using System.Collections.Generic;
using System.Text;

namespace Interface
{
    public class User
    {
        int port;
        string username;

        public User(int port, string username)
        {
            this.port = port;
            this.username = username;
        }
    }
}
