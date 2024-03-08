using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.HttpServer.Request
{
    internal enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete,
        Patch
    }

    internal static class MethodUtilities
    {
        public static HttpMethod GetMethod(string method)
        {
            return method.ToLower() switch
            {
                "get" => HttpMethod.Get,
                "post" => HttpMethod.Post,
                "put" => HttpMethod.Put,
                "delete" => HttpMethod.Delete,
                "patch" => HttpMethod.Patch,
                _ => throw new InvalidDataException()
            };
        }
    }
}
