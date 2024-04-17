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
    // aktuelle tournament infos listen
    internal class ListCurrentTournamentInfoCommand : IRouteCommand
    {
        private readonly ITournamentManager _tournamentManager;
        private readonly UserSchema _userSchema;

        // Konstruktor init
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
                // tournament info für spez user aufrufen
                TournamentInfoSchema tournament = _tournamentManager.GetTournamentInfo(_userSchema.Username);
                // in JSON
                var jsonPayload = JsonConvert.SerializeObject(tournament);
                response = new HttpResponse(StatusCode.Ok, jsonPayload);
            }
            catch (NoTournamentException)
            {
                // kein tournament
                response = new HttpResponse(StatusCode.NoContent);
            }

            return response;
        }
    }
}
