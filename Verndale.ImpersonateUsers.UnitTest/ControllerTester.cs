using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verndale.ImpersonateUsers.Controllers;

namespace Verndale.ImpersonateUsers.UnitTest
{
    [TestClass]
    public class ControllerTester
    {
        [TestMethod]
        public void Will_index_action_impersonateUsersController_Load()
        {
            var contentPageController = new VerndaleImpersonateUsersController();
            var result = contentPageController.Index();
            
            result.Should().NotBeNull();
        }
    }
}
