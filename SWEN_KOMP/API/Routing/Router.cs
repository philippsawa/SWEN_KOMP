using Newtonsoft.Json;
using SWEN_KOMP.Models.Schemas;
using SWEN_KOMP.API.Routing.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN_KOMP.API;
using SWEN_KOMP.HttpServer.Request;
using SWEN_KOMP.HttpServer.Response;
using SWEN_KOMP.HttpServer.Routing;
using SWEN_KOMP.Exceptions;
using HttpMethod = SWEN_KOMP.HttpServer.Request.HttpMethod;
using SWEN_KOMP.BLL.Users;
using System.Net;
using System.Diagnostics.CodeAnalysis;
using SWEN_KOMP.API.Routing.Scores;
using SWEN_KOMP.BLL.Scores;
using SWEN_KOMP.BLL.Tournaments;
using SWEN_KOMP.API.Routing.Tournaments;

namespace SWEN_KOMP.API.Routing
{
    // router für http anfragen
    internal class Router : IRouter
    {
        private readonly IdentityProvider _identityProvider;
        private readonly IdRouteParser _routeParser;
        private readonly IUserManager _userManager;
        private readonly IScoreManager _scoreManager;
        private readonly ITournamentManager _tournamentManager;

        // Konstruktor init
        public Router(IUserManager userManager, IScoreManager scoreManager, ITournamentManager tournamentManager)
        {
            _identityProvider = new IdentityProvider(userManager);
            _routeParser = new IdRouteParser();
            _userManager = userManager;
            _scoreManager = scoreManager;
            _tournamentManager = tournamentManager;
        }

        // route anfragen lösen
        public IRouteCommand? Resolve(HttpRequest request)
        {
            // route matching
            var isUsersMatch = (string path) => _routeParser.IsMatch(path, "/users/{id}");
            var isStatsMatch = (string path) => _routeParser.IsMatch(path, "/stats/{id}");
            var parseUsersId = (string path) => _routeParser.ParseParameters(path, "/users/{id}")["id"];
            var parseStatsId = (string path) => _routeParser.ParseParameters(path, "/stats/{id}")["id"];
            var checkBody = (string? payload) => payload ?? throw new InvalidDataException();

            try
            {
                // richtige command class für verschiedene pfade / methoden
                return request switch
                {
                    { Method: HttpMethod.Post, ResourcePath: "/users" } => new RegisterCommand(_userManager, _scoreManager, Deserialize<UserSchema>(request.Payload)),
                    { Method: HttpMethod.Post, ResourcePath: "/sessions" } => new LoginCommand(_userManager, Deserialize<UserSchema>(request.Payload)),
                    { Method: HttpMethod.Get, ResourcePath: var path } when isUsersMatch(path) => new GetUserDataCommand(_userManager, parseUsersId(path), GetIdentity(request)),
                    { Method: HttpMethod.Put, ResourcePath: var path } when isUsersMatch(path) => new UpdateUserDataCommand(_userManager, parseUsersId(path), GetIdentity(request), Deserialize<UserDataSchema>(request.Payload)),

                    { Method: HttpMethod.Get, ResourcePath: "/stats" } => new RetrieveUserStatsCommand(_scoreManager, GetIdentity(request)),
                    { Method: HttpMethod.Get, ResourcePath: "/score" } => new RetrieveScoreBoardCommand(_scoreManager, GetIdentity(request)),
                    { Method: HttpMethod.Post, ResourcePath: var path } when isStatsMatch(path) => new AddEloToUserCommand(_scoreManager, parseStatsId(path), GetIdentity(request), Deserialize<EloCheatSchema>(request.Payload)),

                    { Method: HttpMethod.Get, ResourcePath: "/history" } => new RetrieveUserHistoryCommand(_tournamentManager, GetIdentity(request)),
                    { Method: HttpMethod.Get, ResourcePath: "/tournament" } => new ListCurrentTournamentInfoCommand(_tournamentManager, GetIdentity(request)),
                    { Method: HttpMethod.Post, ResourcePath: "/history" } => new AddHistoryEntryCommand(_tournamentManager, _scoreManager, GetIdentity(request), Deserialize<HistoryPayloadSchema>(request.Payload)),

                    _ => null
                };
            }
            catch (InvalidDataException)
            {
                return null; // ungültige daten --> null
            }
        }

        // Deserialisieren des bodys
        private T Deserialize<T>(string? body) where T : class
        {
            var data = body is not null ? JsonConvert.DeserializeObject<T>(body) : null;
            return data ?? throw new InvalidDataException();
        }

        // identity aus http anfrage extrahieren
        private UserSchema GetIdentity(HttpRequest request)
        {
            return _identityProvider.GetIdentityForRequest(request) ?? throw new RouteNotAuthenticatedException();
        }
    }
}
