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
    // verwaltet registry, login, aktualisierung 
    internal class UserManager : IUserManager
    {
        private readonly IUserDao _userDao;

        // konstruktor init
        public UserManager(IUserDao userDao)
        {
            _userDao = userDao;
        }

        // benutzer mit token get
        public UserSchema GetUserByAuthToken(string authToken)
        {
            return _userDao.GetUserByAuthToken(authToken) ?? throw new UserNotFoundException();
        }

        // registriert user
        public void RegisterUser(UserSchema user)
        {
            try
            {
                // user in db add
                if (!_userDao.UserInsertion(user))
                {
                    throw new DuplicateUserException();
                }
                // user data in db add
                _userDao.DataInsertion(user);
            }
            catch (Npgsql.PostgresException ex)
            {
                // user vorhanden --> ex
                if (ex.SqlState == "23505")
                {
                    throw new DuplicateUserException();
                }
            }
        }

        // bearbeitet daten v user
        public void EditUser(UserDataSchema userData, string username)
        {
            // aktualisierung in db
            if (!_userDao.EditUserData(userData, username))
            {
                throw new UserNotFoundException();
            }
        }

        // loggt user ein
        public void LoginUser(UserSchema user)
        {
            // check login info, ex wenn nicht gefunden
            if (_userDao.UserLogin(user) == false)
            {
                throw new UserNotFoundException();
            }
        }

        // get daten eines users
        public UserDataSchema GetUserData(string username)
        {
            // get daten von user aus db, ex wenn nicht vorhanden
            var userData = _userDao.GetUserData(username);
            if (userData == null)
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
