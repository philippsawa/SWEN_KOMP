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

        public HttpRequest? ReceiveRequest()
        {
            try
            {
                // "using" keyword -> immediately dispose and when going out of scope, leaving the socket stream open for sending the response
                using var reader = new StreamReader(_client.GetStream(), leaveOpen: true);

                Request.HttpMethod method = Request.HttpMethod.Get;
                string? path = null;
                string? version = null;
                Dictionary<string, string> header = new();
                int contentLength = 0;
                string? payload = null;

                ParseState state = ParseState.Base;
                string? line;

                while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    line = line.Trim();
                    switch (state)
                    {
                        case ParseState.Base:
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
                            var headerInfo = line.Split(':', 2);
                            if (headerInfo.Length != 2)
                            {
                                throw new InvalidDataException();
                            }
                            var key = headerInfo[0].Trim();
                            var value = headerInfo[1].Trim();
                            header.Add(key, value);

                            // special handling for content length, we need this for reading the payload
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

                // we need this to tell the compiler that the nullables are not null in the following code
                if (path is null || version is null)
                {
                    return null;
                }

                // check whether we need a payload step
                state = contentLength > 0 && header.ContainsKey("Content-Type") ? ParseState.Payload : ParseState.Finished;

                if (state == ParseState.Payload)
                {
                    // in a more complete implementation, we should consider the content type (i.e. for receiving binary data)
                    // we however only cover textual data

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

        public void SendResponse(HttpResponse response)
        {
            // https://stackoverflow.com/questions/5757290/http-header-line-break-style
            
            // "using" keyword -> immediately dispose and close stream when going out of scope
            using var writer = new StreamWriter(_client.GetStream());
            
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
