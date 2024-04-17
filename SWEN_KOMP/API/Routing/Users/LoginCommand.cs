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
    // user login
    internal class LoginCommand : IRouteCommand
    {
        private readonly IUserManager _userManager;
        private readonly UserSchema _userSchema;

        // Konstruktor init
        public LoginCommand(IUserManager userManager, UserSchema userSchema)
        {
            _userManager = userManager;
            _userSchema = userSchema;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                // einloggen
                _userManager.LoginUser(_userSchema);
                // token zurückgeben
                response = new HttpResponse(StatusCode.Ok, _userSchema.Token);
            }
            catch (UserNotFoundException)
            {
                // nicht gefunden
                response = new HttpResponse(StatusCode.Conflict);
            }

            return response;
        }
    }
}
