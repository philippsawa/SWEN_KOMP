using SWEN_KOMP.HttpServer.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.HttpServer.Routing
{
    internal interface IRouteCommand
    {
        HttpResponse Execute();
    }
}
