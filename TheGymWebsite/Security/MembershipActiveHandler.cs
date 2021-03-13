using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TheGymWebsite.Security
{
    public class MembershipActiveHandler : AuthorizationHandler<MembershipActiveRequirement>
    {
        protected override Task HandleRequirementAsync (AuthorizationHandlerContext context, MembershipActiveRequirement requirement)
        {
            // Do not authorise users who are not members or are banned.
            if (context.User.HasClaim (c => c.Type == "MembershipExpiry") == false || context.User.HasClaim (c => c.Type == "Banned"))
            {
                return Task.CompletedTask;
            }

            // Computing the membership expiry date
            DateTime expiryDate = Convert.ToDateTime (context.User.FindFirst (c => c.Type == "MembershipExpiry").Value);

            // Grant permission to user whose membership is still active.
            if (expiryDate > DateTime.Today)
            {
                context.Succeed (requirement);
            }

            return Task.CompletedTask;
        }
    }
}