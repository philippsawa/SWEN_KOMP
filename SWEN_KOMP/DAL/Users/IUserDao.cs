using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.DAL.Users
{
    internal interface IUserDao
    {
        bool UserInsertion(UserSchema user);
        bool UserLogin(UserSchema user);

        UserDataSchema? GetUserData(string username);
        UserSchema? GetUserByAuthToken(string authToken);
        bool DataInsertion(UserSchema user);

    }

}
