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
    internal class RetrieveUserHistoryCommand : IRouteCommand
    {
        private readonly ITournamentManager _tournamentManager;
        private readonly UserSchema _user;

        public RetrieveUserHistoryCommand(ITournamentManager tournamentManager, UserSchema user)
        {
            _tournamentManager = tournamentManager;
            _user = user;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                List<HistorySchema> history = _tournamentManager.GetHistory(_user.Username);
                var jsonPayload = JsonConvert.SerializeObject(history);
                response = new HttpResponse(StatusCode.Ok, jsonPayload);
            }
            catch (EmptyHistoryException)
            {
                response = new HttpResponse(StatusCode.NoContent);
            }

            return response;
        }
    }
}
