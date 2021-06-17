using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Logging;
using EPiServer.PlugIn;
using EPiServer.Shell.Security;
using Microsoft.AspNet.Identity.Owin;
using Verndale.ImpersonateUsers.Models;
using Verndale.ImpersonateUsers.Repositories;
using Shell = EPiServer.Shell;
using PlugInArea = EPiServer.PlugIn.PlugInArea;

namespace Verndale.ImpersonateUsers.Controllers
{
    [Authorize(Roles = "Administrators, WebAdmins, ImpersonateUsers")]
    [GuiPlugIn(Area = PlugInArea.AdminMenu, UrlFromModuleFolder = "ImpersonateUsers", DisplayName = "Impersonate Users")]
    public class VerndaleImpersonateUsersController : Controller
    {
        #region Properties

        private static readonly ILogger Logger = LogManager.GetLogger();

        #endregion

        public ActionResult Index()
        {
            Logger.Debug("Begin Index()");
            return View(GetViewLocation("Index"), new ImpersonateUserViewModel());
        }

        [HttpPost]
        public ActionResult IndexPost(ImpersonateUserViewModel model)
        {
            Logger.Debug("Begin Index(ImpersonateUserViewModel model)");
            Logger.Debug(
                $"FirstName: {model.FirstName}, Email: {model.Email}, PageIndex: {model.PageIndex}, PagingSize: {model.PagingSize}");

            try
            {
                int totalRecords = 0;
                var userProvider = ServiceLocator.Current.GetInstance<UIUserProvider>();

                if (model.Email == null) model.Email = "";
                if (model.FirstName == null) model.FirstName = "";

                model.Users = !string.IsNullOrEmpty(model.Email)
                    ? userProvider.FindUsersByEmail(model.Email.Trim().ToLower(), model.PageIndex, model.PagingSize, out totalRecords)
                    : userProvider.FindUsersByName(model.FirstName.Trim().ToLower(), model.PageIndex, model.PagingSize, out totalRecords);

                if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.FirstName))
                {
                    var usersByName = userProvider.FindUsersByName(model.FirstName.Trim().ToLower(), 0, 1000, out totalRecords);
                    var usersByEmail = userProvider.FindUsersByEmail(model.Email.Trim().ToLower(), 0, 1000, out totalRecords);
                    var users = new List<IUIUser>();
                    
                    foreach (var user in usersByName)
                    {
                        Logger.Debug(user.Username);

                        if (usersByEmail.Any(p => p.Username == user.Username))
                        {
                            users.Add(user);
                        }
                    }

                    model.Users = users.Skip(model.PagingSize * model.PageIndex).Take(model.PagingSize);

                    totalRecords = model.Users.Count();
                }

                Logger.Debug($"Total records: {totalRecords}");

                model.TotalRecords = totalRecords;
            }
            catch (Exception ex)
            {
                model.Message = ex.Message;
                Logger.Error(ex.Message, ex);
            }

            return View(GetViewLocation("Index"), model);
        }

        [HttpPost]
        public ActionResult Impersonate(string user)
        {
            Logger.Debug("Begin Impersonate(string user)");

            try
            {
                if (Request == null)
                {
                    throw new Exception("Request cannot be null");
                }

                var owinContext = Request.GetOwinContext();
                if (owinContext == null)
                {
                    throw new Exception("No Owin Context.");
                }

                var signInManager = owinContext.Get<ApplicationSignInManager<ApplicationUser>>();
                if (signInManager == null)
                {
                    throw new Exception("SignInManager could not be retrieved.");
                }

                var userManager = signInManager.UserManager;
                var impersonationRepository = ServiceLocator.Current.GetInstance<IImpersonationRepository>();

                impersonationRepository.ImpersonateUser(user, userManager);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return Content(ex.Message);
            }

            return View(GetViewLocation("Impersonation"), model: user);
        }

        private static string GetViewLocation(string viewName)
        {
            // Catch null exception in unit testing
            try
            {
                var rootPath = Shell.Paths.ProtectedRootPath;
            }
            catch (Exception)
            {
                return viewName;
            }

            return $"{Shell.Paths.ProtectedRootPath}Verndale.ImpersonateUsers/Views/ImpersonateUsers/{viewName}.cshtml";
        }
    }
}