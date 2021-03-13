using System;
using Xunit;
using TheGymWebsite.Models;

namespace TheGymWebsite.Tests.Models
{
    public class MembershipExpiryTest
    {
        [Fact]
        public void SetMembershipExpiryDate_InputDurationOfMembership_ReturnsExpiryDate()
        {
            // Arrange
            DateTime expected = DateTime.Now.AddDays(1);

            // Act
            DateTime result = MembershipExpiry.SetExpiryDate(Enums.MembershipDuration.OneDay);

            // Assert
            Assert.Equal(expected.Date, result.Date);
        }

        [Fact]
        public void SetMembershipExpiryDate_InputUnlimitedDuration_ReturnsMaximumDate()
        {
            // Arrange
            DateTime expected = DateTime.MaxValue;

            // Act
            DateTime result = MembershipExpiry.SetExpiryDate(Enums.MembershipDuration.Unlimited);

            // Assert
            Assert.Equal(expected.Date, result.Date);
        }
    }
}
