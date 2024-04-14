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
    internal class AddEloToUserCommand : IRouteCommand
    {
        private readonly IScoreManager _scoreManager;
        private readonly string _requestedUsername;
        private readonly UserSchema _requestingUserSchema;
        private readonly EloCheatSchema _eloCheat;

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
                if(_requestingUserSchema.Token != "admin-sebToken")
                {
                    throw new UserNotAuthenticatedException();
                }
                _scoreManager.AddElo(_eloCheat.EloAmountToAdd, _requestedUsername+"-sebToken");
                response = new HttpResponse(StatusCode.Ok);
            }catch (UserNotAuthenticatedException)
            {
                response = new HttpResponse(StatusCode.Unauthorized);
            }
            
            return response;
        }
    }
}
