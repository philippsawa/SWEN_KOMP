using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.Models.Schemas
{
    internal class UserSchema
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Token => $"{Username}-sebToken";

        public UserSchema(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
