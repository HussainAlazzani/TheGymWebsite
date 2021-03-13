using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TheGymWebsite.Security
{
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            if (context.User.HasClaim (c => c.Type == "DateOfBirth") == false)
            {
                return Task.CompletedTask;
            }

            DateTime dateOfBirth = Convert.ToDateTime (context.User.FindFirst (c => c.Type == "DateOfBirth").Value);

            int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Today.AddYears (-calculatedAge))
            {
                calculatedAge--;
            }

            if (calculatedAge >= requirement.MinimumAge)
            {
                context.Succeed (requirement);
            }

            return Task.CompletedTask;
        }
    }
}