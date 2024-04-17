using SWEN_KOMP.BLL.Scores;
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
    // Entry in history hinzufügen
    internal class AddHistoryEntryCommand : IRouteCommand
    {
        private readonly ITournamentManager _tournamentManager;
        private readonly IScoreManager _scoreManager;
        private readonly UserSchema _userSchema;
        private readonly HistoryPayloadSchema _historyPayloadSchema;

        // Konstruktor init
        public AddHistoryEntryCommand(ITournamentManager tournamentManager, IScoreManager scoreManager, UserSchema userSchema, HistoryPayloadSchema historyPayloadSchema)
        {
            _tournamentManager = tournamentManager;
            _scoreManager = scoreManager;
            _userSchema = userSchema;
            _historyPayloadSchema = historyPayloadSchema;
        }

        public HttpResponse Execute()
        {
            HttpResponse response;

            try
            {
                // turnier start (mit historyschema)
                _tournamentManager.StartTournament(new HistorySchema(_historyPayloadSchema.Count, _historyPayloadSchema.DurationInSeconds, _userSchema.Username), _historyPayloadSchema.Name);
                // pushup count add
                _scoreManager.AddPushUpCount(_historyPayloadSchema.Count, _userSchema.Token);
                response = new HttpResponse(StatusCode.Ok);
            }
            catch (Exception ex)
            {
                // ex auf console ausgeben
                Console.WriteLine(ex.Message);
                response = new HttpResponse(StatusCode.BadRequest);
            }

            return response;
        }
    }
}
