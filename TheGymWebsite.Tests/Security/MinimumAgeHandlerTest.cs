using Xunit;
using TheGymWebsite.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;

namespace TheGymWebsite.Tests.Security
{
    public class MinimumAgeHandlerTest
    {
        [Fact]
        public async Task HandleRequirementAsync_WhenUserAgeIsBelowMinimumAge_ShouldFail()
        {
            // Arrange
            var requirements = new List<IAuthorizationRequirement>();
            requirements.Add(new MinimumAgeRequirement(16));

            var claim = new Claim("DateOfBirth", "01/01/2010");
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(claim);
            var user = new ClaimsPrincipal(claimsIdentity);

            var context = new AuthorizationHandlerContext(requirements, user, null);
            var minimumAgeHandler = new MinimumAgeHandler();

            // Act
            await minimumAgeHandler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_WhenUserAgeIsAboveMinimumAge_ShouldSucceed()
        {
            // Arrange
            var requirements = new List<IAuthorizationRequirement>();
            requirements.Add(new MinimumAgeRequirement(16));

            var claim = new Claim("DateOfBirth", "01/01/2000");
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(claim);
            var user = new ClaimsPrincipal(claimsIdentity);

            var context = new AuthorizationHandlerContext(requirements, user, null);
            var minimumAgeHandler = new MinimumAgeHandler();

            // Act
            await minimumAgeHandler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }
    }
}
