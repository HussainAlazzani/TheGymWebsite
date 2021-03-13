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
using Microsoft.Extensions.Logging;

namespace TheGymWebsite.Tests.Controllers
{
    public class RoleControllerTest
    {
        // Mocking the identity services
        private Mock<IUserStore<ApplicationUser>> mockUserStore;
        private Mock<UserManager<ApplicationUser>> mockUserManager;

        // Mocking roles service
        private Mock<IRoleStore<IdentityRole>> mockRoleStore;
        private Mock<RoleManager<IdentityRole>> mockRoleManager;

        private Mock<ILogger<RoleController>> mockLogger;

        public RoleControllerTest()
        {
            // Instantiating the mocks for the identity objects
            mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            // Instantiating the mocks for the roles objects
            mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);

            mockLogger = new Mock<ILogger<RoleController>>();
        }

        private RoleController CreateRoleController()
        {
            return new RoleController(
                mockUserManager.Object,
                mockRoleManager.Object,
                mockLogger.Object);
        }

        [Fact]
        public async Task ListRoles_ReturnsAResultView()
        {
            // Arrange
            var roles = new List<IdentityRole>
            {
                new IdentityRole("test_role")
            };

            mockRoleManager.SetupGet(rm => rm.Roles).Returns(roles.AsQueryable()).Verifiable();
            mockRoleManager.Setup(rm => rm.GetClaimsAsync(It.IsAny<IdentityRole>())).ReturnsAsync(new List<Claim>()).Verifiable();

            var controller = CreateRoleController();

            // Act
            var result = await controller.ListRoles();

            // Assert
            mockRoleManager.Verify(rm => rm.Roles, Times.Once);
            mockRoleManager.Verify(rm => rm.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<RoleViewModel>>(resultView.ViewData.Model);
        }

        [Fact]
        public void CreateRole_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateRoleController();

            // Act
            var result = controller.CreateRole();

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<RoleViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task CreateRolePost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateRoleController();

            controller.ModelState.AddModelError("Name", "Role name required");

            // Act
            var result = await controller.CreateRole(new RoleViewModel());

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<RoleViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task CreateRolePost_WhenCreatingARoleFails_ReturnsAResultView()
        {
            // Arrange
            mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed()).Verifiable();

            var controller = CreateRoleController();

            // Act
            var result = await controller.CreateRole(new RoleViewModel());

            // Assert
            mockRoleManager.Verify(rm => rm.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<RoleViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task CreateRolePost_WhenCreatingARoleSucceeds_ReturnsARedirection()
        {
            // Arrange
            mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            mockRoleManager.Setup(rm => rm.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            var controller = CreateRoleController();

            var model = new RoleViewModel
            {
                Id = "1",
                Name = "test_role_name",
                RoleClaims = new List<RoleClaimViewModel>
                {
                    new RoleClaimViewModel
                    {
                        ClaimType = "test_claim",
                        ClaimValue = true
                    }
                }
            };

            // Act
            var result = await controller.CreateRole(model);

            // Assert
            mockRoleManager.Verify(rm => rm.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListRoles", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditRole_WhenRoleIsNotFound_ReturnsARedirection()
        {
            // Arrange
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null).Verifiable();

            var controller = CreateRoleController();

            // Act
            var result = await controller.EditRole("test_no_role");

            // Assert
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListRoles", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditRole_WhenRoleIsFound_ReturnsAResultView()
        {
            // Arrange
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityRole()).Verifiable();
            mockRoleManager.Setup(rm => rm.GetClaimsAsync(It.IsAny<IdentityRole>())).ReturnsAsync(new List<Claim>()).Verifiable();

            var controller = CreateRoleController();

            // Act
            var result = await controller.EditRole("test_no_role");

            // Assert
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockRoleManager.Verify(rm => rm.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<RoleViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task EditRolePost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateRoleController();

            controller.ModelState.AddModelError("Name", "Role name required");

            // Act
            var result = await controller.EditRole(new RoleViewModel());

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<RoleViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task EditRolePost_WhenUpdatingARoleSucceeds_ReturnsARedirection()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("test_claim_type","true")
            };

            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityRole()).Verifiable();
            mockRoleManager.Setup(rm => rm.GetClaimsAsync(It.IsAny<IdentityRole>())).ReturnsAsync(claims).Verifiable();
            mockRoleManager.Setup(rm => rm.UpdateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            mockRoleManager.Setup(rm => rm.RemoveClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()));
            mockRoleManager.Setup(rm => rm.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            var controller = CreateRoleController();

            var model = new RoleViewModel
            {
                Id = "1",
                Name = "test_role_name",
                RoleClaims = new List<RoleClaimViewModel>
                {
                    new RoleClaimViewModel
                    {
                        ClaimType = "test_claim",
                        ClaimValue = true
                    }
                }
            };

            // Act
            var result = await controller.EditRole(model);

            // Assert
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockRoleManager.Verify(rm => rm.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Once);
            mockRoleManager.Verify(rm => rm.UpdateAsync(It.IsAny<IdentityRole>()), Times.Once);
            mockRoleManager.Verify(rm => rm.RemoveClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()));
            mockRoleManager.Verify(rm => rm.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()));
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListRoles", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task DeleteRolePost_WhenRoleExists_ReturnsAResultView()
        {
            // Arrange
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityRole()).Verifiable();
            mockRoleManager.Setup(rm => rm.DeleteAsync(It.IsAny<IdentityRole>())).Verifiable();

            var controller = CreateRoleController();

            // Act
            var result = await controller.DeleteRole("role_id");

            // Assert
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockRoleManager.Verify(rm => rm.DeleteAsync(It.IsAny<IdentityRole>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.Equal("ListRoles", resultView.ViewName);
        }

        [Fact]
        public async Task UsersInRole_WhenRoleIsNotFound_ReturnsARedirection()
        {
            // Arrange
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null).Verifiable();

            var controller = CreateRoleController();

            // Act
            var result = await controller.UsersInRole("role_id");

            // Assert
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListRoles", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task UsersInRole_WhenRoleExists_ReturnsAResultView()
        {
            // Arrange
            mockRoleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityRole { Name = "role_name" })
                .Verifiable();
            mockUserManager.Setup(rm => rm.GetUsersInRoleAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<ApplicationUser>())
                .Verifiable();

            var controller = CreateRoleController();

            // Act
            var result = await controller.UsersInRole("role_id");

            // Assert
            mockRoleManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(rm => rm.GetUsersInRoleAsync(It.IsAny<string>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<ApplicationUser>>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task AddUsersToRole_WhenRoleIsNotFound_ReturnsARedirection()
        {
            // Arrange
            mockRoleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            var controller = CreateRoleController();

            // Act
            var result = await controller.AddUsersToRole("role_id");

            // Assert
            mockRoleManager.Verify(rm => rm.FindByNameAsync(It.IsAny<string>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListRoles", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task AddUsersToRole_WhenRoleIsFound_ReturnsAResultView()
        {
            // Arrange
            mockRoleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityRole { Name = "role_name" })
                .Verifiable();
            mockUserManager.Setup(rm => rm.GetUsersInRoleAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<ApplicationUser>())
                .Verifiable();

            mockUserManager.SetupGet(rm => rm.Users).Returns(new List<ApplicationUser>().AsQueryable()).Verifiable();

            var controller = CreateRoleController();

            // Act
            var result = await controller.AddUsersToRole("role_id");

            // Assert
            mockRoleManager.Verify(rm => rm.FindByNameAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(rm => rm.GetUsersInRoleAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(rm => rm.Users, Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<AddUsersToRoleViewModel>>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task AddUsersToRolePost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateRoleController();

            controller.ModelState.AddModelError("RoleName", "Required");

            // Act
            var result = await controller.AddUsersToRole(new List<AddUsersToRoleViewModel>(), "role_name");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListRoles", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task AddUsersToRolePost_WhenRoleIsFound_ReturnsARedirection()
        {
            // Arrange
            mockRoleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityRole { Name = "role_name" })
                .Verifiable();

            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser())
                .Verifiable();

            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            var controller = CreateRoleController();

            var models = new List<AddUsersToRoleViewModel>
            {
                new AddUsersToRoleViewModel{ IsChecked = true }
            };

            // Act
            var result = await controller.AddUsersToRole(models, "role_id");

            // Assert
            mockRoleManager.Verify(rm => rm.FindByNameAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(rm => rm.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListRoles", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task RemoveUserFromRolePost_WhenUserIsInRole_ReturnsARedirection()
        {
            // Arrange
            mockRoleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityRole { Name = "role_name" })
                .Verifiable();
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser())
                .Verifiable();
            mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true)
                .Verifiable();
            mockUserManager.Setup(um => um.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Verifiable();

            var controller = CreateRoleController();

            // Act
            var result = await controller.RemoveUserFromRole("user_id", "role_id");

            // Assert
            mockRoleManager.Verify(rm => rm.FindByNameAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(rm => rm.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(rm => rm.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()),Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("UsersInRole", redirectToActionResult.ActionName);
        }
    }
}
