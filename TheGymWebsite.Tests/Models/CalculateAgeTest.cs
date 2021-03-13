using System;
using Xunit;
using TheGymWebsite.Models;

namespace TheGymWebsite.Tests.Models
{
    public class CalculateAgeTest
    {
        [Fact]
        public void GetAge_ValidDateOfBirth_ReturnsAgeInInteger()
        {
            // Arrange
            DateTime dateOfBirth = new DateTime(2000, 1, 1);
            DateTime currentDate = DateTime.Now;
            int expected = currentDate.Year - dateOfBirth.Year;

            // Act
            int result = CalculateAge.GetAge(dateOfBirth);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetAge_InValidDateOfBirth_ThrowsException()
        {
            // Arrange
            DateTime invalidDateOfBirth = DateTime.Now.AddDays(1);

            // Act and Assert
            Assert.Throws<Exception>(() => CalculateAge.GetAge(invalidDateOfBirth));
        }
    }
}
