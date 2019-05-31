using System;
using System.Collections.Generic;
using EPiServer.Shell.Security;

namespace Verndale.ImpersonateUsers.Models
{
    public class ImpersonateUserViewModel
    {
        public ImpersonateUserViewModel()
        {
            PagingSize = 20;
        }

        public string FirstName { get; set; }
        public string Email { get; set; }
        public int PagingSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PagingSize);
        public IEnumerable<IUIUser> Users { get; set; }
        public string Message { get; set; }
    }
}