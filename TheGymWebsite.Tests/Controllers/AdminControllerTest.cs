using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using TheGymWebsite.Controllers;
using TheGymWebsite.Models;
using TheGymWebsite.ViewModels;
using TheGymWebsite.Models.Repository;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TheGymWebsite.Tests.Controllers
{
    public class AdminControllerTest
    {
        // Mocking the models and the repositories
        private Mock<IOpenHoursRepository> mockOpenHoursRepo;
        private Mock<IGymRepository> mockGymRepo;
        private Mock<IVacancyRepository> mockVacancyRepo;
        private Mock<IMembershipDealRepository> mockMembershipDealRepo;
        private Mock<IAttendanceRepository> mockAttendanceRepo;

        // Mocking the identity services
        private Mock<IUserStore<ApplicationUser>> mockUserStore;
        private Mock<UserManager<ApplicationUser>> mockUserManager;
        private Mock<IHttpContextAccessor> mockContextAccessor;
        private Mock<IUserClaimsPrincipalFactory<ApplicationUser>> mockUserPrincipalFactory;
        private Mock<SignInManager<ApplicationUser>> mockSignInManager;

        private Mock<ILogger<AdminController>> mockLogger;

        public AdminControllerTest()
        {
            // Instantiating the mocks for the repositories
            mockOpenHoursRepo = new Mock<IOpenHoursRepository>();
            mockGymRepo = new Mock<IGymRepository>();
            mockVacancyRepo = new Mock<IVacancyRepository>();
            mockMembershipDealRepo = new Mock<IMembershipDealRepository>();
            mockAttendanceRepo = new Mock<IAttendanceRepository>();

            // Instantiating the mocks for the identity objects
            mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            mockContextAccessor = new Mock<IHttpContextAccessor>();
            mockUserPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            mockSignInManager = new Mock<SignInManager<ApplicationUser>>(mockUserManager.Object, mockContextAccessor.Object, mockUserPrincipalFactory.Object, null, null, null, null);

            mockLogger = new Mock<ILogger<AdminController>>();
        }

        private AdminController CreateAdminController()
        {
            return new AdminController(
                mockGymRepo.Object,
                mockOpenHoursRepo.Object,
                mockVacancyRepo.Object,
                mockMembershipDealRepo.Object,
                mockUserManager.Object,
                mockSignInManager.Object,
                mockAttendanceRepo.Object,
                mockLogger.Object);
        }

        [Fact]
        public async Task UserList_ReturnsAResultView()
        {
            // Arrange
            mockUserManager.SetupGet(um => um.Users).Returns(TestData.GetTestUsers().AsQueryable()).Verifiable();
            mockUserManager.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<Claim>()).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = await controller.UserList();

            // Assert
            mockUserManager.Verify(um => um.Users, Times.Once);
            mockUserManager.Verify(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Exactly(2));
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IList<UserListViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task GymSignInPost_WhenUserIsNotFound_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = await controller.GymSignIn(It.IsAny<string>());

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ControllerName);
            Assert.Equal("UserNotFound", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task GymSignInPost_WhenUserIsBanned_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(TestData.GetTestUsers().First()).Verifiable();
            mockUserManager.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<Claim> { new Claim("Banned", "") }).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = await controller.GymSignIn(It.IsAny<string>());

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ControllerName);
            Assert.Equal("BannedUser", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task GymSignInPost_WhenUserIsAccepted_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(TestData.GetTestUsers().First()).Verifiable();
            mockUserManager.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<Claim> { new Claim("", "") }).Verifiable();
            mockAttendanceRepo.Setup(repo => repo.Add(new GymAttendance { UserId = "1", Date = DateTime.Now })).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = await controller.GymSignIn(It.IsAny<string>());

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Once);
            mockAttendanceRepo.Verify(repo => repo.Add(It.IsAny<GymAttendance>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("UserList", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task UserProfile_WhenUserIsNotFound_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(() => null).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = await controller.UserProfile(It.IsAny<string>());

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ControllerName);
            Assert.Equal("UserNotFound", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task UserProfile_WhenUserIsFound_ReturnsAResultView()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(TestData.GetTestUsers().First()).Verifiable();
            mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>()).Verifiable();
            mockUserManager.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<Claim> { new Claim("", "") }).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = await controller.UserProfile(It.IsAny<string>());

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.GetRolesAsync(It.IsAny<ApplicationUser>()), Times.Once);
            mockUserManager.Verify(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Once);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<UserProfileViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task AddEmployeePost_WhenEmployeeIsNotFound_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = await controller.AddEmployee(It.IsAny<string>(), string.Empty);

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Admin", redirectToActionResult.ControllerName);
            Assert.Equal("UserList", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task AddEmployeePost_WhenEmployeeIsFound_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(TestData.GetTestUsers().First()).Verifiable();
            mockUserManager.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<Claim> { new Claim("", "") }).Verifiable();
            mockUserManager.Setup(um => um.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = await controller.AddEmployee(It.IsAny<string>(), string.Empty);

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Once);
            mockUserManager.Verify(um => um.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()), Times.Exactly(2));
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Admin", redirectToActionResult.ControllerName);
            Assert.Equal("UserList", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task RemoveEmployeePost_WhenEmployeeIsNotFound_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = await controller.RemoveEmployee(It.IsAny<string>(), string.Empty);

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Admin", redirectToActionResult.ControllerName);
            Assert.Equal("UserList", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task RemoveEmployeePost_WhenEmployeeIsFound_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(TestData.GetTestUsers().First()).Verifiable();
            mockUserManager.Setup(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<Claim> { new Claim("Employee", "") }).Verifiable();
            mockUserManager.Setup(um => um.RemoveClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(IdentityResult.Success).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = await controller.RemoveEmployee(It.IsAny<string>(), string.Empty);

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Once);
            mockUserManager.Verify(um => um.RemoveClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Admin", redirectToActionResult.ControllerName);
            Assert.Equal("UserList", redirectToActionResult.ActionName);
        }

        [Fact]
        public void AddGym_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            // Act
            var result = controller.AddGym();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<GymViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void AddGymPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = controller.AddGym(new GymViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<GymViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void AddGymPost_WhenModelStateIsValid_ReturnsARedirection()
        {
            // Arrange
            mockGymRepo.Setup(repo => repo.Add(It.IsAny<Gym>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.AddGym(new GymViewModel());

            // Assert
            mockGymRepo.Verify(repo => repo.Add(It.IsAny<Gym>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListGyms", redirectToActionResult.ActionName);
        }

        [Fact]
        public void EditGym_WhenGymIsNotFound_ReturnsARedirection()
        {
            mockGymRepo.Setup(repo => repo.GetGym(It.IsAny<int>())).Returns(() => null).Verifiable();

            // Arrange
            var controller = CreateAdminController();

            // Act
            var result = controller.EditGym(It.IsAny<int>());

            // Assert
            mockGymRepo.Verify(repo => repo.GetGym(It.IsAny<int>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListGyms", redirectToActionResult.ActionName);
        }

        [Fact]
        public void EditGym_WhenGymIsFound_ReturnsAResultView()
        {
            mockGymRepo.Setup(repo => repo.GetGym(It.IsAny<int>())).Returns(new Gym()).Verifiable();

            // Arrange
            var controller = CreateAdminController();

            // Act
            var result = controller.EditGym(It.IsAny<int>());

            // Assert
            mockGymRepo.Verify(repo => repo.GetGym(It.IsAny<int>()), Times.Once);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<GymViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void EditGymPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = controller.EditGym(new GymViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<GymViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void EditGymPost_WhenModelStateIsValid_ReturnsARedirection()
        {
            // Arrange
            mockGymRepo.Setup(repo => repo.GetGym(It.IsAny<int>())).Returns(new Gym()).Verifiable();
            mockGymRepo.Setup(repo => repo.Update(It.IsAny<Gym>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.EditGym(new GymViewModel());

            // Assert
            mockGymRepo.Verify(repo => repo.GetGym(It.IsAny<int>()), Times.Once);
            mockGymRepo.Verify(repo => repo.Update(It.IsAny<Gym>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListGyms", redirectToActionResult.ActionName);
        }

        [Fact]
        public void DeleteGymPost_ReturnsARedirection()
        {
            // Arrange
            mockGymRepo.Setup(repo => repo.Delete(It.IsAny<int>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.DeleteGym(It.IsAny<int>());

            // Assert
            mockGymRepo.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ListGyms", redirectToActionResult.ActionName);
        }

        [Fact]
        public void ListGyms_ReturnsAResultView()
        {
            mockGymRepo.Setup(repo => repo.GetGyms()).Returns(TestData.GetTestGyms()).Verifiable();

            // Arrange
            var controller = CreateAdminController();

            // Act
            var result = controller.ListGyms();

            // Assert
            mockGymRepo.Verify(repo => repo.GetGyms(), Times.Once);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Gym>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void AddMembershipDeal_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            // Act
            var result = controller.AddMembershipDeal();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AddMembershipDealPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            controller.ModelState.AddModelError("Price", "Required");

            // Act
            var result = controller.AddMembershipDeal(new MembershipDealViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<MembershipDealViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void AddMembershipDealPost_WhenDurationSelectedIsAlreadyOffered_ReturnsAResultView()
        {
            // Arrange
            mockMembershipDealRepo.Setup(repo => repo.IsDurationOffered(It.IsAny<Enums.MembershipDuration>())).Returns(true).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.AddMembershipDeal(new MembershipDealViewModel());

            // Assert
            mockMembershipDealRepo.Verify(repo => repo.IsDurationOffered(It.IsAny<Enums.MembershipDuration>()), Times.Once);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<MembershipDealViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void AddMembershipDealPost_WhenModelStateIsValid_ReturnsARedirection()
        {
            // Arrange
            mockMembershipDealRepo.Setup(repo => repo.Add(It.IsAny<MembershipDeal>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.AddMembershipDeal(new MembershipDealViewModel());

            // Assert
            mockMembershipDealRepo.Verify(repo => repo.Add(It.IsAny<MembershipDeal>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("MembershipDeals", redirectToActionResult.ActionName);
        }

        [Fact]
        public void EditMembershipDeal_WhenMembershipDealIsNotFound_ReturnsARedirection()
        {
            // Arrange
            mockMembershipDealRepo.Setup(repo => repo.GetMembershipDeal(It.IsAny<int>())).Returns(() => null).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.EditMembershipDeal(It.IsAny<int>());

            // Assert
            mockMembershipDealRepo.Verify(repo => repo.GetMembershipDeal(It.IsAny<int>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("MembershipDeals", redirectToActionResult.ActionName);
        }

        [Fact]
        public void EditMembershipDeal_WhenMembershipDealIsFound_ReturnsAResultView()
        {
            mockMembershipDealRepo.Setup(repo => repo.GetMembershipDeal(It.IsAny<int>())).Returns(new MembershipDeal()).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.EditMembershipDeal(It.IsAny<int>());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<MembershipDealViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void EditMembershipDealPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            controller.ModelState.AddModelError("Price", "Required");

            // Act
            var result = controller.EditMembershipDeal(new MembershipDealViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<MembershipDealViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void EditMembershipDealPost_WhenModelStateIsValid_ReturnsARedirection()
        {
            // Arrange
            mockMembershipDealRepo.Setup(repo => repo.GetMembershipDeal(It.IsAny<int>())).Returns(new MembershipDeal()).Verifiable();
            mockMembershipDealRepo.Setup(repo => repo.Update(It.IsAny<MembershipDeal>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.EditMembershipDeal(new MembershipDealViewModel());

            // Assert
            mockMembershipDealRepo.Verify(repo => repo.GetMembershipDeal(It.IsAny<int>()), Times.Once);
            mockMembershipDealRepo.Verify(repo => repo.Update(It.IsAny<MembershipDeal>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("MembershipDeals", redirectToActionResult.ActionName);
        }

        [Fact]
        public void DeleteMembershipDealPost_ReturnsARedirection()
        {
            // Arrange
            mockMembershipDealRepo.Setup(repo => repo.Delete(It.IsAny<int>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.DeleteMembershipDeal(It.IsAny<int>());

            // Assert
            mockMembershipDealRepo.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("MembershipDeals", redirectToActionResult.ActionName);
        }

        [Fact]
        public void AddBankHoliday_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            // Act
            var result = controller.AddBankHoliday();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AddBankHolidayPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            controller.ModelState.AddModelError("Day", "Required");

            // Act
            var result = controller.AddBankHoliday(new OpenHoursViewModel());

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<OpenHoursViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public void AddBankHolidayPost_WhenDateIsInvalid_ThrowsAnArgumentOutOfRangeExceptionAndReturnsAResultView()
        {
            // Arrange
            mockOpenHoursRepo.Setup(repo => repo.Add(It.IsAny<OpenHours>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var model = new OpenHoursViewModel
            {
                // Invalid date
                Day = 30,
                Month = 2,
                Year = 2021
            };
            var result = controller.AddBankHoliday(model);

            // Assert
            mockOpenHoursRepo.Verify(repo => repo.Add(It.IsAny<OpenHours>()), Times.Never);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<OpenHoursViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public void AddBankHolidayPost_WhenModelStateIsValid_ReturnsARedirection()
        {
            // Arrange
            mockOpenHoursRepo.Setup(repo => repo.Add(It.IsAny<OpenHours>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var model = new OpenHoursViewModel
            {
                Day = 1,
                Month = 1,
                Year = 2021
            };
            var result = controller.AddBankHoliday(model);

            // Assert
            mockOpenHoursRepo.Verify(repo => repo.Add(It.IsAny<OpenHours>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("OpenHours", redirectToActionResult.ActionName);
        }

        [Fact]
        public void EditOpenHours_ReturnsAResultView()
        {
            // Arrange
            mockOpenHoursRepo.Setup(repo => repo.GetOpenHours()).Returns(TestData.GetTestOpenHours()).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.EditOpenHours();

            // Assert
            mockOpenHoursRepo.Verify(repo => repo.GetOpenHours(), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<OpenHoursViewModel>>(resultView.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void EditOpenHoursPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            controller.ModelState.AddModelError("Day", "Required");

            // Act
            var result = controller.EditOpenHours(new List<OpenHoursViewModel>());

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<OpenHoursViewModel>>(resultView.ViewData.Model);
        }

        [Fact]
        public void EditOpenHoursPost_WhenDateIsInvalid_ThrowsAnArgumentOutOfRangeExceptionAndReturnsAResultView()
        {
            // Arrange
            mockOpenHoursRepo.Setup(repo => repo.GetOpenHours(It.IsAny<int>())).Returns(new OpenHours()).Verifiable();

            var controller = CreateAdminController();

            // Act
            List<OpenHoursViewModel> models = new List<OpenHoursViewModel>
            {
                // Id must be greater than 7 because those are reserved for the regular week days.
                new OpenHoursViewModel
                {
                    Id = 8,
                    // Invalid date
                    Day = 30,
                    Month = 2,
                    Year = 2021
                }
            };

            var result = controller.EditOpenHours(models);

            // Assert
            mockOpenHoursRepo.Verify(repo => repo.GetOpenHours(It.IsAny<int>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<OpenHoursViewModel>>(resultView.ViewData.Model);
        }

        [Fact]
        public void EditOpenHoursPost_WhenModelStateIsValid_ReturnsARedirection()
        {
            // Arrange
            mockOpenHoursRepo.Setup(repo => repo.GetOpenHours(It.IsAny<int>())).Returns(new OpenHours()).Verifiable();
            mockOpenHoursRepo.Setup(repo => repo.Update(It.IsAny<OpenHours>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var models = new List<OpenHoursViewModel>
            {
                new OpenHoursViewModel
                {
                    Day = 1,
                    Month = 1,
                    Year = 2021
                }
            };
            var result = controller.EditOpenHours(models);

            // Assert
            mockOpenHoursRepo.Verify(repo => repo.GetOpenHours(It.IsAny<int>()), Times.Once);
            mockOpenHoursRepo.Verify(repo => repo.Update(It.IsAny<OpenHours>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("OpenHours", redirectToActionResult.ActionName);
        }

        [Fact]
        public void DeleteBankHoliday_ReturnsAResultView()
        {
            // Arrange
            mockOpenHoursRepo.Setup(repo => repo.GetOpenHours()).Returns(TestData.GetTestOpenHours()).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.DeleteBankHoliday();

            // Assert
            mockOpenHoursRepo.Verify(repo => repo.GetOpenHours(), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<OpenHoursViewModel>>(resultView.ViewData.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void DeleteBankHolidayPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            controller.ModelState.AddModelError("Day", "Required");

            // Act
            var result = controller.DeleteBankHoliday(new List<OpenHoursViewModel>());

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<List<OpenHoursViewModel>>(resultView.ViewData.Model);
        }

        [Fact]
        public void DeleteBankHolidayPost_WhenModelStateIsValid_ReturnsARedirection()
        {
            // Arrange
            mockOpenHoursRepo.Setup(repo => repo.Delete(It.IsAny<int>())).Verifiable();

            var controller = CreateAdminController();

            var models = new List<OpenHoursViewModel>()
            {
                new OpenHoursViewModel
                {
                    Id = 8,
                    IsChecked = true
                }
            };

            // Act
            var result = controller.DeleteBankHoliday(models);

            // Assert
            mockOpenHoursRepo.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("OpenHours", redirectToActionResult.ActionName);
        }

        [Fact]
        public void AddVacancy_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            // Act
            var result = controller.AddVacancy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AddVacancyPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            controller.ModelState.AddModelError("Job Title", "Required");

            // Act
            var result = controller.AddVacancy(new VacancyViewModel());

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<VacancyViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public void AddVacancyPost_WhenModelStateIsValid_ReturnsARedirection()
        {
            // Arrange
            mockVacancyRepo.Setup(repo => repo.Add(It.IsAny<Vacancy>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.AddVacancy(new VacancyViewModel());

            // Assert
            mockVacancyRepo.Verify(repo => repo.Add(It.IsAny<Vacancy>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Vacancies", redirectToActionResult.ActionName);
        }

        [Fact]
        public void EditVacancy_WhenVacancyIsNotFound_ReturnsARedirection()
        {
            // Arrange
            mockVacancyRepo.Setup(repo => repo.GetVacancy(It.IsAny<int>())).Returns(() => null).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.EditVacancy(It.IsAny<int>());

            // Assert
            mockVacancyRepo.Verify(repo => repo.GetVacancy(It.IsAny<int>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Vacancies", redirectToActionResult.ActionName);
        }

        [Fact]
        public void EditVacancy_WhenVacancyIsFound_ReturnsAResultView()
        {
            // Arrange
            mockVacancyRepo.Setup(repo => repo.GetVacancy(It.IsAny<int>())).Returns(new Vacancy()).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.EditVacancy(It.IsAny<int>());

            // Assert
            mockVacancyRepo.Verify(repo => repo.GetVacancy(It.IsAny<int>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<VacancyViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public void EditVacancyPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAdminController();

            controller.ModelState.AddModelError("Job Title", "Required");

            // Act
            var result = controller.EditVacancy(new VacancyViewModel());

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<VacancyViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public void EditVacancyPost_WhenModelStateIsValid_ReturnsARedirection()
        {
            // Arrange
            mockVacancyRepo.Setup(repo => repo.Update(It.IsAny<Vacancy>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.EditVacancy(new VacancyViewModel());

            // Assert
            mockVacancyRepo.Verify(repo => repo.Update(It.IsAny<Vacancy>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Vacancies", redirectToActionResult.ActionName);
        }

        [Fact]
        public void DeleteVacancyPost_ReturnsARedirection()
        {
            // Arrange
            mockVacancyRepo.Setup(repo => repo.Delete(It.IsAny<int>())).Verifiable();

            var controller = CreateAdminController();

            // Act
            var result = controller.DeleteVacancy(It.IsAny<int>());

            // Assert
            mockVacancyRepo.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Vacancies", redirectToActionResult.ActionName);
        }
    }
}
