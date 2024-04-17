using SWEN_KOMP.BLL.Scores;
using SWEN_KOMP.HttpServer.Response;
using SWEN_KOMP.HttpServer.Routing;
using SWEN_KOMP.Models.Schemas;
using SWEN_KOMP.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.API.Routing.Scores
{
    // Add Elo zu einem User
    internal class AddEloToUserCommand : IRouteCommand
    {
        private readonly IScoreManager _scoreManager;
        private readonly string _requestedUsername;
        private readonly UserSchema _requestingUserSchema;
        private readonly EloCheatSchema _eloCheat;

        // Konstruktor init
        public AddEloToUserCommand(IScoreManager scoreManager, string requestedUsername, UserSchema requestingUser, EloCheatSchema eloCheat)
        {
            _scoreManager = scoreManager;
            _requestedUsername = requestedUsername;
            _requestingUserSchema = requestingUser;
            _eloCheat = eloCheat;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                // benutzer = Admin?
                if (_requestingUserSchema.Token != "admin-sebToken")
                {
                    throw new UserNotAuthenticatedException();
                }
                // Fügt Elo hinzu (+sebtoken)
                _scoreManager.AddElo(_eloCheat.EloAmountToAdd, _requestedUsername + "-sebToken");
                response = new HttpResponse(StatusCode.Ok);
            }
            catch (UserNotAuthenticatedException)
            {
                // Nicht Auth --> Error
                response = new HttpResponse(StatusCode.Unauthorized);
            }

            return response;
        }
    }
}
