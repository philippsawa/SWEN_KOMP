using Newtonsoft.Json;
using SWEN_KOMP.BLL.Scores;
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
    internal class RetrieveScoreBoardCommand : IRouteCommand
    {
        private readonly IScoreManager _scoreManager;
        private readonly UserSchema _userSchema;

        public RetrieveScoreBoardCommand(IScoreManager scoreManager, UserSchema userSchema)
        {
            _scoreManager = scoreManager;
            _userSchema = userSchema;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

           List<UserStatsSchema> scoreboard = _scoreManager.GetScoreboard();
            var jsonPayload = JsonConvert.SerializeObject(scoreboard);
            response = new HttpResponse(StatusCode.Ok, jsonPayload);

            return response;
        }
    }
}
