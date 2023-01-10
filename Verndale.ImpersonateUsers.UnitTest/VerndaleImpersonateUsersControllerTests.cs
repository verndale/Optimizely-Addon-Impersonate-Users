using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Shell.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Verndale.ImpersonateUsers.Controllers;
using Verndale.ImpersonateUsers.Models;
using Verndale.ImpersonateUsers.Services;

namespace Verndale.ImpersonateUsers.UnitTest
{
    [TestClass]
    public class VerndaleImpersonateUsersControllerTests
    {
        [TestMethod]
        public async Task Index_ReturnsAViewResult_WithAListOfUsers()
        {
            // Arrange
            var dummyUserList = new List<IUIUser> { new ApplicationUser() };
            var mockLogger = new Mock<ILogger<VerndaleImpersonateUsersController>>();
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockImpersonationService = new Mock<IImpersonationService>();
            mockImpersonationService
                .Setup(service => service.FindUsersAsync(
                    It.Is<int>(value => value >= 0),
                    null,
                    null))
                .ReturnsAsync(dummyUserList);
            var contentPageController = new VerndaleImpersonateUsersController(
                mockLogger.Object,
                mockAuthorizationService.Object,
                mockImpersonationService.Object);
            var emptyModel = new ImpersonateUserListViewModel();

            // Act
            var result = await contentPageController.Index(emptyModel);

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<ImpersonateUserListViewModel>()
                    .Which.Users.Should().BeSameAs(dummyUserList);
        }
    }
}
