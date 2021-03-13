using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using TheGymWebsite.Controllers;
using TheGymWebsite.Models;
using TheGymWebsite.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace TheGymWebsite.Tests.Controllers
{
    public class MemberControllerTest
    {
        // Mocking the identity services
        private Mock<IUserStore<ApplicationUser>> mockUserStore;
        private Mock<UserManager<ApplicationUser>> mockUserManager;

        // Mocking attendance repository
        private Mock<IAttendanceRepository> mockAttendanceRepository;

        public MemberControllerTest()
        {
            // Instantiating the mocks for the identity objects
            mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            // Instantiating the attendance repository object
            mockAttendanceRepository = new Mock<IAttendanceRepository>();
        }

        [Fact]
        public async Task Index_ReturnsAResultView()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(TestData.GetTestUsers().First())
                .Verifiable();
            
            var claims = new List<Claim>
            {
                new Claim("MembershipExpiry", DateTime.Now.AddDays(1).ToShortDateString())
            };
            
            mockUserManager.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(claims)
                .Verifiable();


            var controller = new MemberController(
                mockUserManager.Object,
                mockAttendanceRepository.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.Index();

            // Assert
            mockUserManager.Verify(um => um.FindByNameAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<MemberViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task MemberDetails_ReturnsAResultView()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser()).Verifiable();
            
            var controller = new MemberController(
                mockUserManager.Object,
                mockAttendanceRepository.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.MemberDetails();

            // Assert
            mockUserManager.Verify(um => um.FindByNameAsync(It.IsAny<string>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ApplicationUser>(resultView.ViewData.Model);
        }

    }
}
