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

        // startet server, akzeptiert client connections
        public void Start()
        {
            _listener.Start();
            _listening = true;

            // dauerschleife für annahme von client connections und parallele bearbeitung
            while (_listening)
            {
                var client = _listener.AcceptTcpClient();
                var clientHandler = new HttpClientHandler(client);
                Task.Run(() => HandleClient(clientHandler));
            }
        }

        // stoppt server
        public void Stop()
        {
            _listening = false;
            _listener.Stop();
        }

        // verarbeitet anfragen von einem client
        private void HandleClient(HttpClientHandler handler)
        {
            var request = handler.ReceiveRequest();
            HttpResponse response;

            // checkt ob anfrage korrekt empfangen wurde
            if (request is null)
            {
                response = new HttpResponse(StatusCode.BadRequest);
            }
            else
            {
                try
                {
                    // anfrage mit router befehl verarbeiten
                    var command = _router.Resolve(request);
                    if (command is null)
                    {
                        response = new HttpResponse(StatusCode.BadRequest);
                    }
                    else
                    {
                        // führt befehl aus + erzeugt antwort
                        response = command.Execute();
                    }
                }
                catch (RouteNotAuthenticatedException)
                {
                    // ex wenn route nicht auth
                    response = new HttpResponse(StatusCode.Unauthorized);
                }
            }

            // sendet antwort an client
            handler.SendResponse(response);
        }
    }

}
