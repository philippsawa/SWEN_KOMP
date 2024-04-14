using SWEN_KOMP.API.Routing;
using SWEN_KOMP.BLL.Scores;
using SWEN_KOMP.BLL.Tournaments;
using SWEN_KOMP.BLL.Users;
using SWEN_KOMP.DAL.Scores;
using SWEN_KOMP.DAL.Tournaments;
using SWEN_KOMP.DAL.Users;
using System.Net;

namespace SWEN_KOMP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Host=localhost;Username=swen_sawa;Password=psawa123;Database=my_db";

            IUserDao userDao = new DataBaseUserDao(connectionString);
            IScoreDao scoreDao = new DataBaseScoreDao(connectionString);
            ITournamentDao tournamentDao = new TournamentDao(connectionString);

            IUserManager userManager = new UserManager(userDao);
            IScoreManager scoreManager = new ScoreManager(scoreDao);
            ITournamentManager tournamentManager = new TournamentManager(tournamentDao, scoreManager);

            var router = new Router(userManager, scoreManager, tournamentManager);
            var server = new HttpServer.HttpServer(router, IPAddress.Any, 10001);
            server.Start();
        }
    }
}