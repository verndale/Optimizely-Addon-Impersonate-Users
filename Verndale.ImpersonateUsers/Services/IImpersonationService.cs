using EPiServer.Shell.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Verndale.ImpersonateUsers.Services
{
    public interface IImpersonationService
    {
        Task ImpersonateUserAsync(string userName);

        Task RevertImpersonationAsync();

        Task<IEnumerable<IUIUser>> FindUsersAsync(int pageSize, string nameQuery = null, string emailQuery = null);
    }
}
