using Newtonsoft.Json;
using SWEN_KOMP.BLL.Scores;
using SWEN_KOMP.Exceptions;
using SWEN_KOMP.HttpServer.Response;
using SWEN_KOMP.HttpServer.Routing;
using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.API.Routing.Scores
{
    // Stats für spezifischen user
    internal class RetrieveUserStatsCommand : IRouteCommand
    {
        private readonly IScoreManager _scoreManager;
        private readonly UserSchema _requestingUser;

        // Konstruktor init
        public RetrieveUserStatsCommand(IScoreManager scoreManager, UserSchema requestingUser)
        {
            _scoreManager = scoreManager;
            _requestingUser = requestingUser;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                // Stats abrufen mit token
                var data = _scoreManager.GetSpecificUserStats(_requestingUser.Token);
                // Daten als JSON
                var jsonPayload = JsonConvert.SerializeObject(data);
                response = new HttpResponse(StatusCode.Ok, jsonPayload);
            }
            catch (UserNotFoundException)
            {
                // Benutzer nd gefunden
                response = new HttpResponse(StatusCode.Unauthorized);
            }

            return response;
        }
    }
}
