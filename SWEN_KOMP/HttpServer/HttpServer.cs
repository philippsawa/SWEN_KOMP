using SWEN_KOMP.Exceptions;
using SWEN_KOMP.HttpServer.Routing;
using SWEN_KOMP.HttpServer.Response;
using SWEN_KOMP.HttpServer.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.HttpServer
{
    internal class HttpServer
    {
        private readonly IRouter _router;
        private readonly TcpListener _listener;
        private bool _listening;

        public HttpServer(IRouter router, IPAddress address, int port)
        {
            _router = router;
            _listener = new TcpListener(address, port);
            _listening = false;
        }

        public void Start()
        {
            _listener.Start();
            _listening = true;

            while (_listening)
            {
                var client = _listener.AcceptTcpClient();
                var clientHandler = new HttpClientHandler(client);
                Task.Run(() => HandleClient(clientHandler));
            }
        }

        public void Stop() 
        { 
            _listening = false;
            _listener.Stop();
        }

        private void HandleClient(HttpClientHandler handler)
        {
            var request = handler.ReceiveRequest();
            HttpResponse response;

            if (request is null)
            {
                response = new HttpResponse(StatusCode.BadRequest);
            }
            else
            {
                try
                {
                    var command = _router.Resolve(request);
                    if (command is null)
                    {
                        response = new HttpResponse(StatusCode.BadRequest);
                    }
                    else
                    {
                        response = command.Execute();
                    }
                }
                catch (RouteNotAuthenticatedException)
                {
                    response = new HttpResponse(StatusCode.Unauthorized);
                }
            }

            handler.SendResponse(response);
        }
    }
}
