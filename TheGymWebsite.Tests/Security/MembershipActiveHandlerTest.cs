using Xunit;
using TheGymWebsite.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using System;

namespace TheGymWebsite.Tests.Security
{
    public class MembershipActiveHandlerTest
    {
        /// <summary>
        /// These are users who have registered their details but have not activated their membership.
        /// There are no claims attached to them.
        /// </summary>
        [Fact]
        public async Task HandleRequirementAsync_WhenUserMembershipIsNotActive_ShouldFail()
        {
            // Arrange
            var requirements = new List<IAuthorizationRequirement>();
            requirements.Add(new MembershipActiveRequirement());

            var user = new ClaimsPrincipal();

            var context = new AuthorizationHandlerContext(requirements, user, null);
            var membershipActiveHandler = new MembershipActiveHandler();

            // Act
            await membershipActiveHandler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_WhenUserMembershipIsExpired_ShouldFail()
        {
            // Arrange
            var requirements = new List<IAuthorizationRequirement>();
            requirements.Add(new MembershipActiveRequirement());

            var userClaim = new Claim("MembershipExpiry", DateTime.Now.AddDays(-1).ToShortDateString());

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(userClaim);
            var user = new ClaimsPrincipal(claimsIdentity);

            var context = new AuthorizationHandlerContext(requirements, user, null);
            var membershipActiveHandler = new MembershipActiveHandler();

            // Act
            await membershipActiveHandler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_WhenUserIsBanned_ShouldFail()
        {
            // Arrange
            var requirements = new List<IAuthorizationRequirement>();
            requirements.Add(new MembershipActiveRequirement());

            IEnumerable<Claim> userClaims = new List<Claim>
            {
                new Claim("MembershipExpiry", DateTime.Now.AddDays(1).ToShortDateString()),
                new Claim("Banned", "")
            };

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaims(userClaims);
            var user = new ClaimsPrincipal(claimsIdentity);

            var context = new AuthorizationHandlerContext(requirements, user, null);
            var membershipActiveHandler = new MembershipActiveHandler();

            // Act
            await membershipActiveHandler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_WhenUserMembershipIsActiveAndIsNotBanned_ShouldSucceed()
        {
            // Arrange
            var requirements = new List<IAuthorizationRequirement>();
            requirements.Add(new MembershipActiveRequirement());

            var userClaim = new Claim("MembershipExpiry", DateTime.Now.AddDays(1).ToShortDateString());

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(userClaim);
            var user = new ClaimsPrincipal(claimsIdentity);

            var context = new AuthorizationHandlerContext(requirements, user, null);
            var membershipActiveHandler = new MembershipActiveHandler();

            // Act
            await membershipActiveHandler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }
    }
}
