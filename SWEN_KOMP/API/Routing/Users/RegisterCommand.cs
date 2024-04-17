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
    // User registrieren
    internal class RegisterCommand : IRouteCommand
    {
        private readonly IUserManager _userManager;
        private readonly IScoreManager _scoreManager;
        private readonly UserSchema _userSchema;

        // Konstruktor init
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
                // user registrieren + eintrag in users und userData
                _userManager.RegisterUser(_userSchema);
                // stats für user init
                _scoreManager.InsertUserStats(_userSchema.Token);
                response = new HttpResponse(StatusCode.Created);
            }
            catch (DuplicateUserException)
            {
                // Error wenn user existiert
                response = new HttpResponse(StatusCode.Conflict);
            }

            return response;
        }
    }
}
