using SWEN_KOMP.API.Routing;
using SWEN_KOMP.BLL.Users;
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

            IUserManager userManager = new UserManager(userDao);

            var router = new Router(userManager);
            var server = new HttpServer.HttpServer(router, IPAddress.Any, 10001);
            server.Start();
        }
    }
}