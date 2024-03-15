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
    internal class UpdateUserDataCommand : IRouteCommand
    {
        private readonly IUserManager _userManager;
        private readonly string _username;
        private readonly UserSchema _authUser;
        private readonly UserDataSchema _userData;

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
                if(_username != _authUser.Username && _username != "admin")
                {
                    throw new UserNotAuthenticatedException();
                }
                _userManager.EditUser(_userData, _username);
                response = new HttpResponse(StatusCode.Ok);
            }
            catch (UserNotFoundException)
            {
                response = new HttpResponse(StatusCode.NotFound);
            }
            catch(UserNotAuthenticatedException)
            {
                response = new HttpResponse(StatusCode.Unauthorized);
            }

            return response;
        }
    }
}
