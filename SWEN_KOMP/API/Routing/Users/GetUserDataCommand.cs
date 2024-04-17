using Newtonsoft.Json;
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
    // user data aufruf
    internal class GetUserDataCommand : IRouteCommand
    {
        private readonly IUserManager _userManager;
        private readonly UserSchema _authUser;
        private readonly string _username;

        // Konstruktor init
        public GetUserDataCommand(IUserManager userManager, string username, UserSchema user)
        {
            _userManager = userManager;
            _authUser = user;
            _username = username;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                // authentifizierungs check
                if (_authUser.Username != _username && _authUser.Username != "admin")
                {
                    throw new UserNotAuthenticatedException();
                }

                // daten abrufen
                var userData = _userManager.GetUserData(_username);
                // als JSON
                var jsonPayload = JsonConvert.SerializeObject(userData);
                response = new HttpResponse(StatusCode.Ok, jsonPayload);
            }
            catch (UserNotAuthenticatedException)
            {
                // nicht auth
                response = new HttpResponse(StatusCode.Unauthorized);
            }
            catch (UserNotFoundException)
            {
                // nicht gefunden
                response = new HttpResponse(StatusCode.NotFound);
            }

            return response;
        }
    }
}
