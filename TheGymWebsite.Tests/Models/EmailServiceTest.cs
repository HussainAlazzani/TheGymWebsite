using Xunit;
using Moq;
using TheGymWebsite.Models;

namespace TheGymWebsite.Tests.Models
{
    public class EmailServiceTest
    {
        [Fact]
        public void Send_InputEmailObject_SendsEmail()
        {
            // Arrange
            var mockEmailService = new Mock<IEmailService>();
            mockEmailService.Setup(p => p.Send(It.IsAny<Email>())).Verifiable();
            
            Email email = new Email
            {
                Address = "test@testing.com",
                Password = "pa55worD",
                Name = "Michael Brown",
                Subject = "Testing Email",
                Message = "This email is a test"
            };

            // Act
            mockEmailService.Object.Send(email);

            // Assert
            mockEmailService.Verify(x => x.Send(email), Times.Once);
        }
    }
}
