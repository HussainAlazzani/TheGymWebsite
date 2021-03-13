using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using TheGymWebsite.Controllers;
using TheGymWebsite.Models;
using TheGymWebsite.ViewModels;
using TheGymWebsite.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace TheGymWebsite.Tests.Controllers
{
    public class HomeControllerTest
    {
        // Mocking the models and the repositories
        private Mock<IOpenHoursRepository> mockOpenHoursRepo;
        private Mock<IGymRepository> mockGymRepo;
        private Mock<IVacancyRepository> mockVacancyRepo;
        private Mock<IFreePassRepository> mockFreePassRepo;
        private Mock<IMembershipDealRepository> mockMembershipDealRepo;
        private Mock<IWebHostEnvironment> mockIWebHostEnvironment;
        private Mock<IEmailService> mockEmailService;
        private Mock<IOptions<Email>> mockIOptionsEmail;

        public HomeControllerTest()
        {
            // Instantiating the mocks for the models and repositories
            mockOpenHoursRepo = new Mock<IOpenHoursRepository>();
            mockGymRepo = new Mock<IGymRepository>();
            mockVacancyRepo = new Mock<IVacancyRepository>();
            mockFreePassRepo = new Mock<IFreePassRepository>();
            mockMembershipDealRepo = new Mock<IMembershipDealRepository>();
            mockIWebHostEnvironment = new Mock<IWebHostEnvironment>();
            mockEmailService = new Mock<IEmailService>();
            mockIOptionsEmail = new Mock<IOptions<Email>>();
        }

        private HomeController CreateHomeController()
        {
            return new HomeController(
                mockGymRepo.Object,
                mockOpenHoursRepo.Object,
                mockVacancyRepo.Object,
                mockFreePassRepo.Object,
                mockMembershipDealRepo.Object,
                mockIWebHostEnvironment.Object,
                mockEmailService.Object,
                mockIOptionsEmail.Object);
        }

        [Fact]
        public void Index__ReturnsAViewResult()
        {
            // Arrange
            var controller = CreateHomeController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void OpenHours_ReturnsAViewResult_WithAListOfOpenHours()
        {
            // Arrange
            mockOpenHoursRepo.Setup(repo => repo.GetOpenHours()).Returns(TestData.GetTestOpenHours());

            var controller = CreateHomeController();

            // Act
            var result = controller.OpenHours();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<OpenHours>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void FreePass__ReturnsAViewResult()
        {
            var controller = CreateHomeController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void FreePassPost_WhenModelStateIsInvalid_ReturnsAViewResult()
        {
            // Arrange
            var controller = CreateHomeController();

            controller.ModelState.AddModelError("Email", "Required");
            var model = new FreePassViewModel();

            // Act
            var result = controller.FreePass(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<FreePassViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void FreePassPost_WhenModelStateIsValid_ReturnsARedirection()
        {
            // Arrange
            mockFreePassRepo.Setup(repo => repo.Add(It.IsAny<FreePass>())).Verifiable();
            mockFreePassRepo.Setup(repo => repo.GetFreePassIdFromEmail("test@testing.com")).Returns(It.IsAny<int>()).Verifiable();
            mockEmailService.Setup(es => es.Send(It.IsAny<Email>())).Verifiable();
            mockIOptionsEmail.SetupGet(s => s.Value).Returns(new Email());

            var controller = CreateHomeController();

            var model = new FreePassViewModel
            {
                Name = "Peter",
                Email = "test@testing.com",
                DateIssued = new DateTime()
            };

            // Act
            var result = controller.FreePass(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("ConfirmFreePass", redirectToActionResult.ActionName);
            mockFreePassRepo.Verify();
        }

        [Fact]
        public void ConfirmFreePass_ReturnsAViewResult()
        {
            // Arrange
            var controller = CreateHomeController();

            // Act
            var result = controller.ConfirmFreePass(It.IsAny<string>());

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void MembershipDeals_ReturnsAViewResult()
        {
            // Arrange
            mockMembershipDealRepo.Setup(repo => repo.GetMembershipDeals()).Returns(TestData.GetTestMembershipDeal());

            var controller = CreateHomeController();

            // Act
            var result = controller.MembershipDeals();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<MembershipDeal>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Vacancies_ReturnsAViewResult()
        {
            // Arrange
            mockVacancyRepo.Setup(repo => repo.GetVacancies()).Returns(TestData.GetTestVacancies());

            var controller = CreateHomeController();

            // Act
            var result = controller.Vacancies();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Vacancy>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void ContactUs_ReturnsAViewResult()
        {
            // Arrange
            mockGymRepo.Setup(repo => repo.GetGym(It.IsAny<int>())).Returns(new Gym()).Verifiable();

            var controller = CreateHomeController();

            // Act
            var result = controller.ContactUs();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ContactUsViewModel>(viewResult.ViewData.Model);
            mockGymRepo.Verify();
        }

        [Fact]
        public void ContactUsPost_WhenModelStateIsInvalid_ReturnsAViewResult()
        {
            // Arrange
            var controller = CreateHomeController();

            controller.ModelState.AddModelError("Email", "Required");
            var model = new ContactUsViewModel();

            // Act
            var result = controller.ContactUs(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ContactUsViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void ContactUsPost_WhenModelStateIsValid_ReturnsARedirection()
        {
            // Arrange
            mockEmailService.Setup(es => es.Send(It.IsAny<Email>())).Verifiable();
            mockIOptionsEmail.SetupGet(s => s.Value).Returns(new Email()).Verifiable();

            var controller = CreateHomeController();
            var model = new ContactUsViewModel();

            // Act
            var result = controller.ContactUs(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("ConfirmContactUs", redirectToActionResult.ActionName);
            mockEmailService.Verify();
            mockIOptionsEmail.Verify();
        }

        [Fact]
        public void ConfirmContactUs_ReturnsAViewResult()
        {
            // Arrange
            var controller = CreateHomeController();

            // Act
            var result = controller.ConfirmContactUs(It.IsAny<string>());

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
