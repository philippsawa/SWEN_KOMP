using SWEN_KOMP.DAL.Users;
using SWEN_KOMP.Exceptions;
using SWEN_KOMP.Models.Schemas;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.BLL.Users
{
    internal class UserManager : IUserManager
    {
        private readonly IUserDao _userDao;

        public UserManager(IUserDao userDao)
        {
            _userDao = userDao;
        }

        public UserSchema GetUserByAuthToken(string authToken)
        {
            return _userDao.GetUserByAuthToken(authToken) ?? throw new UserNotFoundException();
        }

        public void RegisterUser(UserSchema user)
        {
            try
            {
                _userDao.UserInsertion(user);
                _userDao.DataInsertion(user);
            }catch (Npgsql.PostgresException ex)
            {
                if(ex.SqlState == "23505")
                {
                    throw new DuplicateUserException();
                }
            }
        }

        public void LoginUser(UserSchema user)
        {

            if (_userDao.UserLogin(user) == false)
            {
                throw new UserNotFoundException();
            }
        }

        public UserDataSchema GetUserData(string username)
        {
            var userData = _userDao.GetUserData(username);
            if(userData == null)
            {
                throw new UserNotFoundException();
            }
            else
            {
                return userData;
            }
        }
    }
}
