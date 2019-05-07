using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Verndale.ImpersonateUsers.Repositories
{
    public static class ImpersonationHelper
    {
        public static bool IsImpersonating(this IPrincipal principal)
        {
            if (!(principal is ClaimsPrincipal claimsPrincipal))
            {
                return false;
            }

            return claimsPrincipal.HasClaim("UserImpersonation", "true");
        }

        public static string GetOriginalUsername(this IPrincipal principal)
        {
            if (!(principal is ClaimsPrincipal claimsPrincipal))
            {
                return string.Empty;
            }

            if (!claimsPrincipal.IsImpersonating())
            {
                return string.Empty;
            }

            var originalUsernameClaim = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == "OriginalUsername");

            if (originalUsernameClaim == null)
            {
                return string.Empty;
            }

            return originalUsernameClaim.Value;
        }

        public static string GetImpersonatedUsername(this IPrincipal principal)
        {
            if (!(principal is ClaimsPrincipal claimsPrincipal))
            {
                return string.Empty;
            }

            if (!claimsPrincipal.IsImpersonating())
            {
                return string.Empty;
            }

            var originalUsernameClaim = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == "ImpersonatedUsername");

            if (originalUsernameClaim == null)
            {
                return string.Empty;
            }

            return originalUsernameClaim.Value;
        }
    }
}