using EPiServer.Shell.Security;
using System.Collections.Generic;

namespace Verndale.ImpersonateUsers.Models
{
    public class ImpersonateUserListViewModel
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public int PagingSize { get; set; } = 20;
        public int PageIndex { get; set; } = 1;
        public IEnumerable<IUIUser> Users { get; set; }
        public string Message { get; set; }
    }
}
