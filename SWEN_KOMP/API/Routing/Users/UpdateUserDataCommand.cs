using SWEN_KOMP.BLL.Users;
using SWEN_KOMP.Exceptions;
using SWEN_KOMP.HttpServer.Response;
using SWEN_KOMP.HttpServer.Routing;
using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.API.Routing.Users
{
    // user data aktualisieren
    internal class UpdateUserDataCommand : IRouteCommand
    {
        private readonly IUserManager _userManager;
        private readonly string _username;
        private readonly UserSchema _authUser;
        private readonly UserDataSchema _userData;

        // Konstruktor init
        public UpdateUserDataCommand(IUserManager userManager, string username, UserSchema authUser, UserDataSchema userData)
        {
            _userManager = userManager;
            _username = username;
            _authUser = authUser;
            _userData = userData;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                // auth prüfen
                if (_username != _authUser.Username && _authUser.Username != "admin")
                {
                    throw new UserNotAuthenticatedException();
                }
                // user daten aktualisieren
                _userManager.EditUser(_userData, _username);
                response = new HttpResponse(StatusCode.Ok);
            }
            catch (UserNotFoundException)
            {
                // user not found
                response = new HttpResponse(StatusCode.NotFound);
            }
            catch (UserNotAuthenticatedException)
            {
                // nicht auth
                response = new HttpResponse(StatusCode.Unauthorized);
            }

            return response;
        }
    }
}
