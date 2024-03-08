using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.HttpServer.Request
{
    internal class HttpRequest
    {
        public HttpMethod Method { get; init; }
        public string ResourcePath { get; init; }
        public string HttpVersion { get; init; }
        public Dictionary<string, string> Header { get; init; }
        public string? Payload { get; init; }

        public HttpRequest(HttpMethod method, string resourcePath, string httpVersion, Dictionary<string, string> header, string? payload = null)
        {
            Method = method;
            ResourcePath = resourcePath;
            HttpVersion = httpVersion;
            Header = header;
            Payload = payload;
        }
    }
}
