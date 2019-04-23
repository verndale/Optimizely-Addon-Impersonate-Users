using EPiServer.Cms.UI.AspNetIdentity;
using Microsoft.AspNet.Identity;

namespace Verndale.ImpersonateUsers.Repositories
{
    public interface IImpersonationRepository
    {
        void ImpersonateUser(string userName, UserManager<ApplicationUser, string> userManager);

        void RevertImpersonation(UserManager<ApplicationUser, string> userManager);
    }
}