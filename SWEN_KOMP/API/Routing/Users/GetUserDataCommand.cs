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
        private readonly UserSchema _userSchema;

        public GetUserDataCommand(IUserManager userManager, UserSchema userSchema)
        {
            _userManager = userManager;
            _userSchema = userSchema;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                var userData = _userManager.GetUserData(_userSchema.Username);
                var jsonPayload = JsonConvert.SerializeObject(userData);
                response = new HttpResponse(StatusCode.Ok, jsonPayload);
            }
            catch (UserNotFoundException)
            {
                response = new HttpResponse(StatusCode.NotFound);
            }

            return response;
        }
    }
}
