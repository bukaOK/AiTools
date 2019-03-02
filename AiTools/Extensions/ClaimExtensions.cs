using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AiTools.Extensions
{
    public static class ClaimExtensions
    {
        public static void AddUpdateClaim(this IPrincipal principal, string key, string value)
        {
            var currentIdentity = principal.Identity;
            if (!(currentIdentity is ClaimsIdentity identity))
                return;

            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            identity.AddClaim(new Claim(key, value));
        }

        public static string GetClaimValue(this IPrincipal principal, string key)
        {
            var identity = (ClaimsIdentity)principal.Identity;

            var claim = identity.FindFirst(x => x.Type == key);
            return claim?.Value;
        }
    }
}
