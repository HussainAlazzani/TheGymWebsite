using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace TheGymWebsite.Models
{
    public class SeedUsers
    {
        public static void Seed(ModelBuilder builder, string name, Enums.Gender gender, DateTime date, int claimId)
        {
            string id = Guid.NewGuid().ToString();
            // Creating a new user.
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = id,
                Title = Enums.Title.Mr,
                FirstName = $"{name}FirstName",
                LastName = $"{name}LastName",
                DateOfBirth = date,
                Gender = gender,
                PhoneNumber = "00000000000",
                Email = $"{name}@yahoo.com",
                NormalizedEmail = $"{name}@yahoo.com".ToUpper(),
                UserName = $"{name}@yahoo.com",
                NormalizedUserName = $"{name}@yahoo.com".ToUpper(),
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, name),
                EmailConfirmed = true,

                AddressLineOne = $"1 {name} Road",
                AddressLineTwo = $"{name} Area",
                Town = $"{name}Town",
                Postcode = "AD1 2MN"

            });

            builder.Entity<IdentityUserClaim<string>>().HasData(
                    new IdentityUserClaim<string> { Id = claimId, UserId = id, ClaimType = "DateOfBirth", ClaimValue = date.ToShortDateString() }
            );
        }
    }
}
