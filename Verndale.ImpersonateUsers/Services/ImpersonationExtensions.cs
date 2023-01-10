using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Verndale.ImpersonateUsers.Services
{
    public static class ImpersonationExtensions
    {
        public static bool IsImpersonating(this IPrincipal principal)
        {
            if (principal is not ClaimsPrincipal claimsPrincipal)
            {
                return false;
            }

            return claimsPrincipal.HasClaim(Constants.ImpersonationClaims.UserImpersonation, bool.TrueString);
        }

        public static string GetOriginalUsername(this IPrincipal principal)
        {
            if (principal is not ClaimsPrincipal claimsPrincipal || !claimsPrincipal.IsImpersonating())
            {
                return string.Empty;
            }

            var originalUsernameClaim = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == Constants.ImpersonationClaims.OriginalUsername);

            return originalUsernameClaim?.Value ?? string.Empty;
        }

        public static string GetImpersonatedUsername(this IPrincipal principal)
        {
            if (principal is not ClaimsPrincipal claimsPrincipal || !claimsPrincipal.IsImpersonating())
            {
                return string.Empty;
            }

            var impersonatedUsernameClaim = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == Constants.ImpersonationClaims.ImpersonatedUsername);

            return impersonatedUsernameClaim?.Value ?? string.Empty;
        }
    }
}
