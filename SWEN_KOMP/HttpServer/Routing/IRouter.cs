using SWEN_KOMP.HttpServer.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.HttpServer.Routing
{
    internal interface IRouter
    {
        IRouteCommand? Resolve(HttpRequest request);
    }
}
