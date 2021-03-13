using Xunit;
using TheGymWebsite.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using TheGymWebsite.Models;


// TODO: Complete the Error action method

namespace TheGymWebsite.Tests.Controllers
{
    public class ErrorControllerTest
    {
        [Fact]
        public void AccessDenied_ReturnsAResultView()
        {
            // Arrange        
            var controller = new ErrorController();

            // Act
            var result = controller.AccessDenied();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void BannedUser_ReturnsAResultView()
        {
            // Arrange        
            var controller = new ErrorController();

            // Act
            var result = controller.BannedUser("email", "name");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void EmailConfirmationError_ReturnsAResultView()
        {
            // Arrange        
            var controller = new ErrorController();

            // Act
            var result = controller.EmailConfirmationError();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void PaymentError_ReturnsAResultView()
        {
            // Arrange        
            var controller = new ErrorController();

            // Act
            var result = controller.PaymentError();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
        
        [Fact]
        public void AlreadyAMember_ReturnsAResultView()
        {
            // Arrange        
            var controller = new ErrorController();

            // Act
            var result = controller.AlreadyAMember("Expiry_date");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AccountLocked_ReturnsAResultView()
        {
            // Arrange        
            var controller = new ErrorController();

            // Act
            var result = controller.AccountLocked();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void UserNotFound_ReturnsAResultView()
        {
            // Arrange        
            var controller = new ErrorController();

            // Act
            var result = controller.UserNotFound("Id_or_email");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void HttpStatusCodeHandler_ReturnsAResultView()
        {
            // Arrange        
            var controller = new ErrorController();

            // Act
            var result = controller.HttpStatusCodeHandler(404);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("NotFound", viewResult.ViewName);
        }

        [Fact(Skip = "Implementation not complete. Need to set the path features")]
        public void Error_ReturnsAResultView()
        {
            // Arrange        
            Mock<IExceptionHandlerPathFeature> mockExFeature = new Mock<IExceptionHandlerPathFeature>();
            mockExFeature.SetupGet(ef => ef.Path).Returns("").Verifiable();
            mockExFeature.SetupGet(ef => ef.Error.Message).Returns("").Verifiable();
            mockExFeature.SetupGet(ef => ef.Error.StackTrace).Returns("").Verifiable();

            var controller = new ErrorController();

            var httpContext = new DefaultHttpContext();

            

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(new FeatureCollection())
            };

            // Act
            var result = controller.Error();

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ApplicationUser>(resultView.ViewData.Model);
        }
    }
}
