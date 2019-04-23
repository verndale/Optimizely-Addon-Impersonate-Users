using System.Web;
using System.Web.Mvc;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Microsoft.AspNet.Identity.Owin;
using Verndale.ImpersonateUsers.Repositories;

namespace Verndale.ImpersonateUsers.Controllers
{
    [AllowAnonymous]
    public class VerndaleRevertImpersonationController : Controller
    {
        #region Properties

        private static readonly ILogger Logger = LogManager.GetLogger();

        #endregion
        
        public ActionResult Index()
        {
            Logger.Debug("Begin Index()");

            var signInManager = Request.GetOwinContext().Get<ApplicationSignInManager<ApplicationUser>>();
            var userManager = signInManager.UserManager;
            var impersonationRepository = ServiceLocator.Current.GetInstance<IImpersonationRepository>();

            impersonationRepository.RevertImpersonation(userManager);

            return Redirect("/EPiServer/CMS/Admin/Default.aspx");
        }

    }
}