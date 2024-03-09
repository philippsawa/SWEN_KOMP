using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.BLL.Users
{
    internal interface IUserManager
    {
        void RegisterUser(UserSchema user);
        void LoginUser(UserSchema user);

        UserSchema GetUserByAuthToken(string authToken);

        UserDataSchema GetUserData(string username);
    }
}
