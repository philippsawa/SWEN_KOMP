using SWEN_KOMP.HttpServer.Request;
using SWEN_KOMP.HttpServer.Response;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.HttpServer
{
    internal class HttpClientHandler
    {
        private enum ParseState
        {
            Base,
            Headers,
            Payload,
            Finished
        }

        private readonly TcpClient _client;

        public HttpClientHandler(TcpClient client)
        {
            _client = client;
        }

        // received und parst http anfrage von einem tcpclient
        public HttpRequest? ReceiveRequest()
        {
            try
            {
                // streamreader liest zeichen aus byte-stream
                using var reader = new StreamReader(_client.GetStream(), leaveOpen: true);

                Request.HttpMethod method = Request.HttpMethod.Get;
                string? path = null;
                string? version = null;
                Dictionary<string, string> header = new();
                int contentLength = 0;
                string? payload = null;

                ParseState state = ParseState.Base;
                string? line;

                // liest request zeile für zeile
                while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    line = line.Trim();
                    switch (state)
                    {
                        case ParseState.Base:
                            // parst startzeile des requests
                            var baseInfo = line.Split(' ');
                            if (baseInfo.Length != 3)
                            {
                                throw new InvalidDataException();
                            }
                            method = MethodUtilities.GetMethod(baseInfo[0]);
                            path = baseInfo[1];
                            version = baseInfo[2];

                            state = ParseState.Headers;
                            break;

                        case ParseState.Headers:
                            // parst header des requests
                            var headerInfo = line.Split(':', 2);
                            if (headerInfo.Length != 2)
                            {
                                throw new InvalidDataException();
                            }
                            var key = headerInfo[0].Trim();
                            var value = headerInfo[1].Trim();
                            header.Add(key, value);

                            // wenn content-length vorkommt --> payload handling
                            if (key == "Content-Length")
                            {
                                try
                                {
                                    contentLength = int.Parse(value);
                                }
                                catch (Exception e)
                                {
                                    throw new InvalidDataException("invalid content length", e);
                                }
                            }
                            break;
                    }
                }

                // checkt ob pfad und version vorhanden
                if (path is null || version is null)
                {
                    return null;
                }

                // entscheidet ob payload erwartet wird
                state = contentLength > 0 && header.ContainsKey("Content-Type") ? ParseState.Payload : ParseState.Finished;

                if (state == ParseState.Payload)
                {
                    // liest payload wenn content-length >0
                    var buffer = new char[contentLength];
                    var bytesReadTotal = reader.ReadBlock(buffer, 0, contentLength);

                    if (bytesReadTotal != contentLength)
                    {
                        throw new InvalidDataException();
                    }

                    payload = new string(buffer);
                    state = ParseState.Finished;
                }

                return state == ParseState.Finished ? new HttpRequest(method, path, version, header, payload) : null;
            }
            catch (Exception e) when (e is IOException || e is InvalidDataException)
            {
                return null;
            }
        }

        // sendet http antwort an client
        public void SendResponse(HttpResponse response)
        {
            using var writer = new StreamWriter(_client.GetStream());

            // schreibt statuscode und den status der antwort
            writer.Write($"HTTP/1.1 {(int)response.StatusCode} {response.StatusCode}\r\n");
            if (!string.IsNullOrEmpty(response.Payload))
            {
                var payload = Encoding.UTF8.GetBytes(response.Payload);
                writer.Write($"Content-Length: {payload.Length}\r\n");
                writer.Write("\r\n");
                writer.Write(Encoding.UTF8.GetString(payload));
            }
            else
            {
                writer.Write("\r\n");
            }
        }
    }
}
