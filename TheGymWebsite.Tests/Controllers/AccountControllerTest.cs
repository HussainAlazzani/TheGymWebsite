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
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using System.Collections;

namespace TheGymWebsite.Tests.Controllers
{
    public class AccountControllerTest
    {
        // Mocking the identity services
        private Mock<IUserStore<ApplicationUser>> mockUserStore;
        private Mock<UserManager<ApplicationUser>> mockUserManager;
        private Mock<IHttpContextAccessor> mockContextAccessor;
        private Mock<IUserClaimsPrincipalFactory<ApplicationUser>> mockUserPrincipalFactory;
        private Mock<SignInManager<ApplicationUser>> mockSignInManager;

        private Mock<IAuthorizationService> mockAuthorizationService;
        private Mock<ILogger<AccountController>> mockLogger;

        // Mocking roles service
        private Mock<IRoleStore<IdentityRole>> mockRoleStore;
        private Mock<RoleManager<IdentityRole>> mockRoleManager;

        // Mocking the models and the repositories
        private Mock<IEmailService> mockEmailService;
        private Mock<IOptions<Email>> mockIOptionsEmail;
        private Mock<IWebHostEnvironment> mockIWebHostEnvironment;
        private Mock<IMembershipDealRepository> mockMembershipDealRepo;

        public AccountControllerTest()
        {
            // Instantiating the mocks for the identity objects
            mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            mockContextAccessor = new Mock<IHttpContextAccessor>();
            mockUserPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            mockSignInManager = new Mock<SignInManager<ApplicationUser>>(mockUserManager.Object, mockContextAccessor.Object, mockUserPrincipalFactory.Object, null, null, null, null);

            mockAuthorizationService = new Mock<IAuthorizationService>();
            mockLogger = new Mock<ILogger<AccountController>>();

            // Instantiating the mocks for the roles objects
            mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);

            // Instantiating the mocks for the models and repositories
            mockMembershipDealRepo = new Mock<IMembershipDealRepository>();
            mockIWebHostEnvironment = new Mock<IWebHostEnvironment>();
            mockEmailService = new Mock<IEmailService>();
            mockIOptionsEmail = new Mock<IOptions<Email>>();
        }

        private AccountController CreateAccountController()
        {
            return new AccountController(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockRoleManager.Object,
                mockAuthorizationService.Object,
                mockEmailService.Object,
                mockIOptionsEmail.Object,
                mockIWebHostEnvironment.Object,
                mockMembershipDealRepo.Object,
                mockLogger.Object);
        }

        [Fact]
        public void Login_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAccountController();

            // Act
            var result = controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task LoginPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAccountController();

            controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await controller.Login(new LoginViewModel(), string.Empty);

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<LoginViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task LoginPost_WhenUserIsNotFound_ReturnsAResultView()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(() => null).Verifiable();

            var controller = CreateAccountController();

            // Act
            var result = await controller.Login(new LoginViewModel(), string.Empty);

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<LoginViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task LoginPost_WhenUserLoginIsSuccessful_ReturnsARedirection()
        {
            // Arrange
            var user = TestData.GetTestUsers().First();

            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user)
                .Verifiable();
            mockSignInManager.Setup(um => um.SignOutAsync()).Verifiable();
            mockSignInManager.Setup(um => um.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), true, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success)
                .Verifiable();

            var controller = CreateAccountController();

            //var user = TestData.GetTestUsers().First();
            LoginViewModel model = new LoginViewModel
            {
                Email = user.Email,
                Password = user.PasswordHash,
                RememberMe = true
            };

            // Act
            var result = await controller.Login(model, string.Empty);

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockSignInManager.Verify(um => um.SignOutAsync(), Times.Once);
            mockSignInManager.Verify(um => um.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), true, true), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Member", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task LoginPost_WhenPasswordIsIncorrect_ReturnsAResultView()
        {
            // Arrange
            var user = TestData.GetTestUsers().First();

            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user)
                .Verifiable();
            mockSignInManager.Setup(um => um.SignOutAsync()).Verifiable();
            mockSignInManager.Setup(um => um.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), true, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed)
                .Verifiable();
            mockUserManager.Setup(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false)
                .Verifiable();

            var controller = CreateAccountController();

            LoginViewModel model = new LoginViewModel
            {
                Email = user.Email,
                Password = user.PasswordHash,
                RememberMe = true
            };

            // Act
            var result = await controller.Login(model, string.Empty);

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockSignInManager.Verify(um => um.SignOutAsync(), Times.Once);
            mockSignInManager.Verify(um => um.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), true, true), Times.Once);
            mockUserManager.Verify(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<LoginViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task LoginPost_WhenEmailRequiresVerification_ReturnsAResultView()
        {
            // Arrange
            var user = TestData.GetTestUsers().First();

            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user)
                .Verifiable();
            mockSignInManager.Setup(um => um.SignOutAsync()).Verifiable();
            mockSignInManager.Setup(um => um.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), true, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut)
                .Verifiable();
            mockUserManager.Setup(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true)
                .Verifiable();

            var controller = CreateAccountController();

            //var user = TestData.GetTestUsers().First();
            LoginViewModel model = new LoginViewModel
            {
                Email = user.Email,
                Password = user.PasswordHash,
                RememberMe = true
            };

            // Act
            var result = await controller.Login(model, string.Empty);

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockSignInManager.Verify(um => um.SignOutAsync(), Times.Once);
            mockSignInManager.Verify(um => um.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), true, true), Times.Once);
            mockUserManager.Verify(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<LoginViewModel>(resultView.ViewData.Model);
        }

        [Fact(Skip = "Need to set up so that LockOut and TwoFactorRequired are both true")]
        public async Task LoginPost_WhenTheAccountIsLockedOut_ReturnsARedirection()
        {
            // Arrange
            var user = TestData.GetTestUsers().First();

            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user)
                .Verifiable();
            mockSignInManager.Setup(um => um.SignOutAsync()).Verifiable();
            mockSignInManager.Setup(um => um.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), true, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut)
                .Verifiable();
            mockUserManager.Setup(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true)
                .Verifiable();

            var controller = CreateAccountController();

            //var user = TestData.GetTestUsers().First();
            LoginViewModel model = new LoginViewModel
            {
                Email = user.Email,
                Password = user.PasswordHash,
                RememberMe = true
            };

            // Act
            var result = await controller.Login(model, string.Empty);

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockSignInManager.Verify(um => um.SignOutAsync(), Times.Once);
            mockSignInManager.Verify(um => um.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), true, true), Times.Once);
            mockUserManager.Verify(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ControllerName);
            Assert.Equal("AccountLocked", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task LogoutPost_ReturnsARedirection()
        {
            // Arrange
            mockSignInManager.Setup(sm => sm.SignOutAsync()).Verifiable();

            var controller = CreateAccountController();

            // Act
            var result = await controller.Logout();

            // Assert
            mockSignInManager.Verify(sm => sm.SignOutAsync(), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void ForgotPassword_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAccountController();

            // Act
            var result = controller.ForgotPassword();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ForgotPasswordPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            var controller = CreateAccountController();

            controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await controller.ForgotPassword(new ForgotPasswordViewModel());

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ForgotPasswordViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task ForgotPasswordPost_WhenUserIsNotFound_ReturnsAResultView()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(() => null).Verifiable();
            mockUserManager.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(false).Verifiable();

            var controller = CreateAccountController();

            // Act
            var result = await controller.ForgotPassword(new ForgotPasswordViewModel());

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>()), Times.Never);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ForgotPasswordPost_WhenUserEmailIsNotConfirmed_ReturnsAResultView()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser()).Verifiable();
            mockUserManager.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(false).Verifiable();

            var controller = CreateAccountController();

            // Act
            var result = await controller.ForgotPassword(new ForgotPasswordViewModel());

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.Equal("ForgotPasswordConfirmation", resultView.ViewName);
        }

        [Fact]
        public async Task ForgotPasswordPost_WhenUserIsFound_ReturnsAResultView()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(TestData.GetTestUsers().First()).Verifiable();
            mockUserManager.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(true).Verifiable();
            mockUserManager.Setup(um => um.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("test_Token").Verifiable();
            mockEmailService.Setup(es => es.Send(It.IsAny<Email>())).Verifiable();
            mockIOptionsEmail.SetupGet(s => s.Value).Returns(new Email());

            var controller = CreateAccountController();

            // Extension methods cannot be mocked, hence I am Mocking url.Action(actionName,controllerName,routeValues,protocol)
            // extension method by mocking the method urls.Action(UrlActionContext) since they all call it.
            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(url => url.Action(It.IsAny<UrlActionContext>())).Returns("/testing").Verifiable();
            controller.Url = mockUrl.Object;

            // Mocking http request. Setting the Request scheme is not necessary since an empty string will be passed by default.
            // However I must instantiate the ControllerContext but not necessarily the HttpContext property when mocking.
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "testing_scheme";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.ForgotPassword(new ForgotPasswordViewModel());

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>()), Times.Once);
            mockUserManager.Verify(um => um.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()), Times.Once);
            mockEmailService.Verify(es => es.Send(It.IsAny<Email>()), Times.Once);
            mockIOptionsEmail.Verify(s => s.Value, Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.Equal("ForgotPasswordConfirmation", resultView.ViewName);
        }

        [Fact]
        public void ResetPassword_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAccountController();

            // Act
            var result = controller.ResetPassword("Test_token", "test_email");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ResetPasswordPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAccountController();

            controller.ModelState.AddModelError(string.Empty, "Error in the form");

            // Act
            var result = await controller.ResetPassword(new ResetPasswordViewModel());

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ResetPasswordViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task ResetPasswordPost_WhenUserIsNotFound_ReturnsAResultView()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(() => null).Verifiable();

            var controller = CreateAccountController();

            // Act
            var result = await controller.ResetPassword(new ResetPasswordViewModel());

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.Equal("ResetPasswordConfirmation", resultView.ViewName);
        }

        [Fact]
        public async Task ResetPasswordPost_WhenResetingPasswordSucceeds_ReturnsAResultView()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(TestData.GetTestUsers().First()).Verifiable();
            mockUserManager.Setup(um => um.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            var controller = CreateAccountController();

            // Act
            var result = await controller.ResetPassword(new ResetPasswordViewModel());

            // Assert
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(
                um => um.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())
                , Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.Equal("ResetPasswordConfirmation", resultView.ViewName);
        }

        [Fact]
        public void RegisterMember_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAccountController();

            // Act
            var result = controller.RegisterMember();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task RegisterMemberPost_WhenModelStateIsInvalid_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAccountController();

            controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await controller.RegisterMember(new RegisterMemberViewModel());

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task RegisterMemberPost_WhenDateOfBirthIsInvalid_ThrowsAnArgumentOutOfRangeExceptionAndReturnsAResultView()
        {
            // Arrange
            var controller = CreateAccountController();

            var model = new RegisterMemberViewModel
            {
                // Invalid date of birth to raise an ArgumentOutOfRangeException
                DayOfBirth = 30,
                MonthOfBirth = 2,
                YearOfBirth = 2000
            };

            // Act
            var result = await controller.RegisterMember(model);

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<RegisterMemberViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task RegisterMemberPost_WhenCreatingTheUserSucceeds_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            mockUserManager.Setup(um => um.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Verifiable();
            mockUserManager.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("test_token")
                .Verifiable();
            mockIOptionsEmail.SetupGet(e => e.Value).Returns(new Email()).Verifiable();
            mockEmailService.Setup(es => es.Send(It.IsAny<Email>())).Verifiable();

            var controller = CreateAccountController();

            // Extension methods cannot be mocked, hence I am Mocking url.Action(actionName,controllerName,routeValues,protocol)
            // extension method by mocking the method urls.Action(UrlActionContext) since they all call it.
            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(url => url.Action(It.IsAny<UrlActionContext>())).Returns("/testing").Verifiable();
            controller.Url = mockUrl.Object;

            // Mocking http request. Setting the Request scheme is not necessary since an empty string will be passed by default.
            // However I must instantiate the ControllerContext but not necessarily the HttpContext property when mocking.
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "testing_scheme";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var user = TestData.GetTestUsers().First();

            var model = new RegisterMemberViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = "Te5tpa55w0rD",
                DayOfBirth = 1,
                MonthOfBirth = 1,
                YearOfBirth = 2000,
                Gender = Enums.Gender.Male,
                AddressLineOne = user.AddressLineOne,
                Town = user.Town,
                Postcode = user.Postcode
            };

            // Act
            var result = await controller.RegisterMember(model);

            // Assert
            mockUserManager.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
            mockUserManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()), Times.Once);
            mockUrl.Verify(x => x.Action(It.IsAny<UrlActionContext>()), Times.Once);
            mockIOptionsEmail.Verify(e => e.Value, Times.Once);
            mockEmailService.Verify(es => es.Send(It.IsAny<Email>()), Times.Once);
            var resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("RequestToConfirmEmail", resultView.ActionName);
        }

        [Fact]
        public void RequestToConfirmEmail_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAccountController();

            // Act
            var result = controller.RequestToConfirmEmail();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ConfirmEmail_WhenConfirmationSucceedsButUserIsBelowMinimumAge_ReturnsARedirection()
        {
            // Arrange
            var userBelowAge = TestData.GetTestUsers().First();
            // Setting user age to 10 years old.
            userBelowAge.DateOfBirth = DateTime.Now.AddYears(-10);

            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(userBelowAge).Verifiable();
            mockUserManager.Setup(um => um.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();
            mockSignInManager.Setup(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), false, null)).Verifiable();

            var controller = CreateAccountController();

            // Act
            var result = await controller.ConfirmEmail("test_userId", "test_token");

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mockSignInManager.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), false, null), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Account", redirectToActionResult.ControllerName);
            Assert.Equal("AgeRestriction", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ConfirmEmail_WhenConfirmationSucceedsAndUserIsAboveMinimumAge_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(TestData.GetTestUsers().First()).Verifiable();
            mockUserManager.Setup(um => um.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();
            mockSignInManager.Setup(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), false, null)).Verifiable();

            var controller = CreateAccountController();

            // Act
            var result = await controller.ConfirmEmail("test_userId", "test_token");

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mockSignInManager.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), false, null), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Payment", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ConfirmEmail_WhenConfirmationFails_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(TestData.GetTestUsers().First()).Verifiable();
            mockUserManager.Setup(um => um.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed())
                .Verifiable();
            mockSignInManager.Setup(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), false, null)).Verifiable();

            var controller = CreateAccountController();

            // Act
            var result = await controller.ConfirmEmail("test_userId", "test_token");

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mockSignInManager.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), false, null), Times.Never);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ControllerName);
            Assert.Equal("EmailConfirmationError", redirectToActionResult.ActionName);
        }

        [Fact]
        public void AgeRestriction_ReturnsAResultView()
        {
            // Arrange
            var controller = CreateAccountController();

            // Act
            var result = controller.AgeRestriction("test_user");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task EditUserDetails_WhenUserHasNoPermissionToManageOtherUsers_ReturnsAResultView()
        {
            // Arrange
            mockAuthorizationService.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed())
                .Verifiable();

            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(TestData.GetTestUsers().First())
                .Verifiable();

            var controller = CreateAccountController();

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal();
            httpContext.User.AddIdentity(new ClaimsIdentity());
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.EditUserDetails("test_email", string.Empty);

            // Assert
            mockAuthorizationService.Verify(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<EditUserDetailsViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task EditUserDetails_WhenUserHasPermissionToManageOtherUsers_ReturnsAResultView()
        {
            // Arrange
            mockAuthorizationService.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success)
                .Verifiable();

            mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(TestData.GetTestUsers().First())
                .Verifiable();

            var controller = CreateAccountController();

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal();
            //httpContext.User.AddIdentity(new ClaimsIdentity());
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.EditUserDetails("test_email", string.Empty);

            // Assert
            mockAuthorizationService.Verify(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()), Times.Once);
            mockUserManager.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<EditUserDetailsViewModel>(resultView.ViewData.Model);
        }

        [Fact]
        public async Task EditUserRoles_WhenUserIsNotFound_ReturnsARedirection()
        {
            // Arrange
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null).Verifiable();

            var controller = CreateAccountController();

            // Act
            var result = await controller.EditUserRoles(It.IsAny<string>());

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Admin", redirectToActionResult.ControllerName);
            Assert.Equal("UserList", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditUserRoles_WhenUserIsFound_ReturnsAResultView()
        {
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser()).Verifiable();
            mockRoleManager.SetupGet(rm => rm.Roles).Returns(TestData.GetTestIdentityRoles().AsQueryable()).Verifiable();
            mockUserManager.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<bool>())
                .Verifiable();

            var controller = CreateAccountController();

            // Act
            var result = await controller.EditUserRoles(It.IsAny<string>());

            // Assert
            mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mockRoleManager.Verify(rm => rm.Roles, Times.Once);
            mockUserManager.Verify(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Exactly(2));
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<EditUserRolesViewModel>(resultView.ViewData.Model);
        }

    }
}
