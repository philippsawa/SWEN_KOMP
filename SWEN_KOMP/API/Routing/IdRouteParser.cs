using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SWEN_KOMP.API.Routing
{
    // route parser für url routes
    internal class IdRouteParser
    {
        // gegebener pfad = route muster?
        public bool IsMatch(string resourcePath, string routePattern)
        {
            // muster erstellen und {id} durch regex ersetzen
            var pattern = "^" + routePattern.Replace("{id}", ".*").Replace("/", "\\/") + "(\\?.*)?$";
            return Regex.IsMatch(resourcePath, pattern);
        }

        // parameter aus url extrahieren
        public Dictionary<string, string> ParseParameters(string resourcePath, string routePattern)
        {
            // query-Parameter extrahieren
            var parameters = ParseQueryParameters(resourcePath);

            // ID-Parameter extrahieren
            var id = ParseIdParameter(resourcePath, routePattern);
            if (id != null)
            {
                parameters["id"] = id;
            }

            return parameters;
        }

        // ID parameter aus ressourcenpfad
        private string? ParseIdParameter(string resourcePath, string routePattern)
        {
            // muster für extrahierung von id aus dem pfad
            var pattern = "^" + routePattern.Replace("{id}", "(?<id>[^\\?\\/]*)").Replace("/", "\\/") + "$";
            var result = Regex.Match(resourcePath, pattern);
            return result.Groups["id"].Success ? result.Groups["id"].Value : null;
        }

        // parse query parameter aus url
        private Dictionary<string, string> ParseQueryParameters(string route)
        {
            var parameters = new Dictionary<string, string>();

            // teil nach dem ? als query string abtrennen
            var query = route
                .Split("?", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .FirstOrDefault();

            if (query != null)
            {
                // schlüsse-wert paar extrahieren
                var keyValues = query
                    .Split("&", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Split("=", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                    .Where(p => p.Length == 2);

                foreach (var p in keyValues)
                {
                    parameters[p[0]] = p[1];
                }
            }

            return parameters;
        }

    }
}
