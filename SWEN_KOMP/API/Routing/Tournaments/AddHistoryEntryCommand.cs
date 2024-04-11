using SWEN_KOMP.BLL.Tournaments;
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
    internal class AddHistoryEntryCommand : IRouteCommand
    {
        private readonly ITournamentManager _tournamentManager;
        private readonly UserSchema _userSchema;
        private readonly HistoryPayloadSchema _historyPayloadSchema;

        public AddHistoryEntryCommand(ITournamentManager tournamentManager, UserSchema userSchema, HistoryPayloadSchema historyPayloadSchema)
        {
            _tournamentManager = tournamentManager;
            _userSchema = userSchema;
            _historyPayloadSchema = historyPayloadSchema;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                _tournamentManager.StartTournament(new HistorySchema(_historyPayloadSchema.Count, _historyPayloadSchema.DurationInSeconds, _userSchema.Username), _historyPayloadSchema.Name);
                response = new HttpResponse(StatusCode.Ok);
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                response = new HttpResponse(StatusCode.BadRequest);
            }

            return response;
        }
    }
}
