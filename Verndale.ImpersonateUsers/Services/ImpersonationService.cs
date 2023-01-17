using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Logging;
using EPiServer.Shell.Security;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Verndale.ImpersonateUsers.Services
{
    public class ImpersonationService<TUser> : IImpersonationService where TUser : IdentityUser, IUIUser, new()
    {
        #region Properties

        private static readonly ILogger Logger = LogManager.GetLogger();
        private readonly ApplicationSignInManager<TUser> _applicationSignInManager;
        private readonly UIUserProvider _userProvider;

        #endregion

        public ImpersonationService(ApplicationSignInManager<TUser> applicationSignInManager, UIUserProvider userProvider)
        {
            _applicationSignInManager = applicationSignInManager;
            _userProvider = userProvider;
        }

        public async Task ImpersonateUserAsync(string userName)
        {
            var originalUsername = _applicationSignInManager.Context.User.Identity?.Name;
            var impersonatedUser = await _applicationSignInManager.UserManager.FindByNameAsync(userName);

            if (impersonatedUser == null)
            {
                throw new Exception("Unable to find the user " + userName);
            }

            var additionalClaims = new List<Claim>
            {
                new Claim(Constants.ImpersonationClaims.UserImpersonation, bool.TrueString),
                new Claim(Constants.ImpersonationClaims.OriginalUsername, originalUsername),
                new Claim(Constants.ImpersonationClaims.ImpersonatedUsername, userName)
            };

            await _applicationSignInManager.SignOutAsync();
            await _applicationSignInManager.SignInWithClaimsAsync(impersonatedUser, false, additionalClaims);
        }

        public async Task RevertImpersonationAsync()
        {
            var userPrincipal = _applicationSignInManager.Context?.User;

            if (userPrincipal == null || !userPrincipal.IsImpersonating())
            {
                Logger.Warning("Unable to remove impersonation because there is no impersonation");
                return;
            }

            var originalUsername = userPrincipal.GetOriginalUsername();
            var originalUser = await _applicationSignInManager.UserManager.FindByNameAsync(originalUsername);

            await _applicationSignInManager.SignOutAsync();
            await _applicationSignInManager.SignInAsync(originalUser, false);
        }

        public async Task<IEnumerable<IUIUser>> FindUsersAsync(int pageSize, string nameQuery = null, string emailQuery = null)
        {
            var userResultSets = new List<List<IUIUser>>();

            if (!string.IsNullOrEmpty(emailQuery))
                userResultSets.Add(await LoadUserPage(_userProvider.FindUsersByEmailAsync(emailQuery.Trim().ToLower(), 0, pageSize)));

            if (!string.IsNullOrEmpty(nameQuery))
                userResultSets.Add(await LoadUserPage(_userProvider.FindUsersByNameAsync(nameQuery.Trim().ToLower(), 0, pageSize)));

            if (!userResultSets.Any())
                userResultSets.Add(await LoadUserPage(_userProvider.GetAllUsersAsync(0, pageSize)));

            return MergeUserResults(userResultSets, pageSize);
        }

        private static async Task<List<IUIUser>> LoadUserPage(IAsyncEnumerable<IUIUser> userEnumerable)
        {
            var userList = new List<IUIUser>();

            await foreach (var user in userEnumerable)
            {
                userList.Add(user);
            }

            return userList;
        }

        private static IEnumerable<IUIUser> MergeUserResults(List<List<IUIUser>> userResultSets, int pageSize)
        {
            var mergedUsers = new Dictionary<string, IUIUser>();

            for (var i = 0; i < pageSize; i++)
            {
                foreach (var userSet in userResultSets.ToArray())
                {
                    if (userSet.Count <= i)
                    {
                        userResultSets.Remove(userSet);
                        continue;
                    }

                    var user = userSet[i];

                    if (mergedUsers.Count < pageSize && !mergedUsers.ContainsKey(user.Username))
                        mergedUsers.Add(user.Username, user);
                }
            }

            return mergedUsers.Values;
        }
    }
}
