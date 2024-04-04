using Newtonsoft.Json;
using SWEN_KOMP.BLL.Tournaments;
using SWEN_KOMP.Exceptions;
using SWEN_KOMP.HttpServer.Response;
using SWEN_KOMP.HttpServer.Routing;
using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.API.Routing.Tournaments
{
    internal class ListCurrentTournamentInfoCommand : IRouteCommand
    {
        private readonly ITournamentManager _tournamentManager;
        private readonly UserSchema _userSchema;

        public ListCurrentTournamentInfoCommand(ITournamentManager tournamentManager, UserSchema userSchema)
        {
            _tournamentManager = tournamentManager;
            _userSchema = userSchema;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                TournamentInfoSchema tournament = _tournamentManager.GetTournamentInfo(_userSchema.Username);
                var jsonPayload = JsonConvert.SerializeObject(tournament);
                response = new HttpResponse(StatusCode.Ok, jsonPayload);
            }
            catch (NoTournamentException)
            {
                response = new HttpResponse(StatusCode.NoContent);
            }

            return response;
        }


    }
}
