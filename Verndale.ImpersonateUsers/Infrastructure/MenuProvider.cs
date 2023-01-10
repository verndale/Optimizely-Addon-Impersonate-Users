using EPiServer.Shell;
using EPiServer.Shell.Navigation;
using System.Collections.Generic;

namespace Verndale.ImpersonateUsers.Infrastructure
{
    [MenuProvider]
    public class MenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            var path = Paths.ToResource(GetType(), "ImpersonateUsers");
            var menuItem = new UrlMenuItem(
                "Impersonate Users",
                $"{MenuPaths.Global}/cms/impersonateusers",
                path)
            {
                SortIndex = SortIndex.Last + 10,
                AuthorizationPolicy = Constants.AuthorizationPolicyName
            };

            return new List<MenuItem> { menuItem };
        }
    }
}
