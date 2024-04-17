using SWEN_KOMP.BLL.Users;
using SWEN_KOMP.HttpServer.Request;
using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.API.Routing
{
    // identity provider für http requests
    internal class IdentityProvider
    {
        private readonly IUserManager _userManager;

        // Konstruktor init
        public IdentityProvider(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public UserSchema? GetIdentityForRequest(HttpRequest request)
        {
            UserSchema? currentUser = null;

            // token aus header extrahieren
            if (request.Header.TryGetValue("Authorization", out var authToken))
            {
                const string prefix = "Basic ";
                // check ob token "basic" prefix hat
                if (authToken.StartsWith(prefix))
                {
                    try
                    {
                        // user anhand des tokens identifizieren
                        currentUser = _userManager.GetUserByAuthToken(authToken.Substring(prefix.Length));
                    }
                    catch { } // error handling nicht nötig --> currentUser bleibt null
                }
            }

            return currentUser;
        }
    }
}
