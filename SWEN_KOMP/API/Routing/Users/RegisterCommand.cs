using Npgsql;
using SWEN_KOMP.BLL.Scores;
using SWEN_KOMP.BLL.Users;
using SWEN_KOMP.Exceptions;
using SWEN_KOMP.HttpServer.Response;
using SWEN_KOMP.HttpServer.Routing;
using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.API.Routing.Users
{
    internal class RegisterCommand : IRouteCommand
    {
        private readonly IUserManager _userManager;
        private readonly IScoreManager _scoreManager;
        private readonly UserSchema _userSchema;

        public RegisterCommand(IUserManager userManager, IScoreManager scoreManager, UserSchema userSchema)
        {
            _userManager = userManager;
            _scoreManager = scoreManager;
            _userSchema = userSchema;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                _userManager.RegisterUser(_userSchema); // -> users neuer Eintrag, userData neuer Eintrag
                _scoreManager.InsertUserStats(_userSchema.Token);
                response = new HttpResponse(StatusCode.Created);
            }
            catch (DuplicateUserException)
            {
                response = new HttpResponse(StatusCode.Conflict);
            }

                return response;
        }
    }
}
