using EPiServer.Shell;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Verndale.ImpersonateUsers.Models;
using Verndale.ImpersonateUsers.Services;

namespace Verndale.ImpersonateUsers.Controllers
{
    [Authorize(Policy = Constants.AuthorizationPolicyName)]
    public class VerndaleImpersonateUsersController : Controller
    {
        #region Properties

        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IImpersonationService _impersonationService;

        #endregion

        public VerndaleImpersonateUsersController(
            ILogger<VerndaleImpersonateUsersController> logger,
            IAuthorizationService authorizationService,
            IImpersonationService impersonationService)
        {
            _logger = logger;
            _authorizationService = authorizationService;
            _impersonationService = impersonationService;
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<ActionResult> Index(ImpersonateUserListViewModel model)
        {
            _logger.LogDebug("Begin Index(ImpersonateUserViewModel model)");
            _logger.LogDebug(
                "FirstName: {FirstName}, Email: {Email}, PageIndex: {PageIndex}, PagingSize: {PagingSize}",
                model.FirstName,
                model.Email,
                model.PageIndex,
                model.PagingSize);

            try
            {
                var resultSize = Math.Max(model.PageIndex, 1) * model.PagingSize;
                model.Users = await _impersonationService.FindUsersAsync(resultSize, model.FirstName, model.Email);
            }
            catch (Exception ex)
            {
                model.Message = ex.Message;
                _logger.LogError(ex, ex.Message);
            }

            return View(model);
        }

        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public async Task<ActionResult> Impersonate(ImpersonateUserViewModel model)
        {
            _logger.LogDebug("Begin Impersonate(ImpersonateUserViewModel model)");

            if (!await IsUserAuthorized())
            {
                return Redirect(Paths.ToResource(GetType(), $"{nameof(ImpersonateUsers)}/{nameof(RevertImpersonation)}"));
            }

            try
            {
                if (Request == null)
                {
                    throw new Exception("Request cannot be null");
                }

                await _impersonationService.ImpersonateUserAsync(model.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

                return Content(ex.Message);
            }

            return View(model);
        }

        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public async Task<ActionResult> RevertImpersonation()
        {
            _logger.LogDebug("Begin RevertImpersonation()");

            await _impersonationService.RevertImpersonationAsync();

            return Redirect(Paths.ToResource(GetType(), nameof(ImpersonateUsers)));
        }

        private async Task<bool> IsUserAuthorized()
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, Constants.AuthorizationPolicyName);

            return authResult.Succeeded;
        }
    }
}
