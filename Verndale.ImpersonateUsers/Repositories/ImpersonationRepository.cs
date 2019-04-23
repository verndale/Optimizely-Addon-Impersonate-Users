using System;
using System.Security.Claims;
using System.Web;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.ServiceLocation;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Verndale.ImpersonateUsers.Repositories
{
    [ServiceConfiguration(ServiceType = typeof(IImpersonationRepository))]
    public class ImpersonationRepository : IImpersonationRepository
    {
        public void ImpersonateUser(string userName, UserManager<ApplicationUser, string> userManager)
        {
            var context = HttpContext.Current;

            var originalUsername = context.User.Identity.Name;

            var impersonatedUser = userManager.FindByNameAsync(userName).Result;
            if (impersonatedUser == null)
            {
                throw new Exception("Unable to find the user " + userName);
            }

            var impersonatedIdentity =
                userManager.CreateIdentityAsync(impersonatedUser, DefaultAuthenticationTypes.ApplicationCookie).Result;

            impersonatedIdentity.AddClaim(new Claim("UserImpersonation", "true"));
            impersonatedIdentity.AddClaim(new Claim("OriginalUsername", originalUsername));

            var authenticationManager = context.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, impersonatedIdentity);
        }

        public void RevertImpersonation(UserManager<ApplicationUser, string> userManager)
        {
            var context = HttpContext.Current;

            if (!HttpContext.Current.User.IsImpersonating())
            {
                throw new Exception("Unable to remove impersonation because there is no impersonation");
            }

            var originalUsername = HttpContext.Current.User.GetOriginalUsername();

            var originalUser = userManager.FindByNameAsync(originalUsername).Result;

            var impersonatedIdentity = userManager
                .CreateIdentityAsync(originalUser, DefaultAuthenticationTypes.ApplicationCookie).Result;

            var authenticationManager = context.GetOwinContext().Authentication;

            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, impersonatedIdentity);
        }
    }
}