using Microsoft.AspNetCore.Authorization;

namespace TheGymWebsite.Security
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int MinimumAge { get; }

        public MinimumAgeRequirement (int minimumAge)
        {
            MinimumAge = minimumAge;
        }
    }
}