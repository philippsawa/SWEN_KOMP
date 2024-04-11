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
    internal class GetUserDataCommand : IRouteCommand
    {
        private readonly IUserManager _userManager;
        private readonly UserSchema _authUser;
        private readonly string _username;

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
                if(_authUser.Username != _username && _authUser.Username != "admin"){
                    throw new UserNotAuthenticatedException();
                }

                var userData = _userManager.GetUserData(_username);
                var jsonPayload = JsonConvert.SerializeObject(userData);
                response = new HttpResponse(StatusCode.Ok, jsonPayload);
            }
            catch (UserNotAuthenticatedException){
                response = new HttpResponse(StatusCode.Unauthorized);
            }
            catch (UserNotFoundException)
            {
                response = new HttpResponse(StatusCode.NotFound);
            }

            return response;
        }
    }
}
