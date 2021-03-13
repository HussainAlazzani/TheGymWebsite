using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using TheGymWebsite.Models;

namespace TheGymWebsite.Tests
{
    public class TestData
    {
        public static IEnumerable<Gym> GetTestGyms()
        {
            IEnumerable<Gym> gyms = new List<Gym>
            {
                new Gym()
                {
                    GymName = "TheGym",
                    Email = "gym@thegyms.com",
                    AddressLineOne = "1st street",
                    AddressLineTwo = "West Heath",
                    Town = "London",
                    Postcode = "SE2 1RT",
                    Telephone = "02219902856"
                },
                new Gym()
                {
                    GymName = "TheSecondGym",
                    Email = "gym2@thegyms.com",
                    AddressLineOne = "2nd street",
                    AddressLineTwo = "North End",
                    Town = "Bristol",
                    Postcode = "BR2 1QZ",
                    Telephone = "03919103558"
                }
            };
            return gyms;
        }

        public static IEnumerable<OpenHours> GetTestOpenHours()
        {
            IEnumerable<OpenHours> openHours = new List<OpenHours>
            {
                new OpenHours
                {
                    Id = 1,
                    Date = new DateTime(2020,1,1),
                    DayName = DayOfWeek.Monday,
                    OpenTime = new TimeSpan(8,0,0),
                    CloseTime = new TimeSpan(22,0,0),
                    Note = string.Empty
                },
                new OpenHours
                {
                    Id = 2,
                    Date = new DateTime(2020,2,1),
                    DayName = DayOfWeek.Sunday,
                    OpenTime = new TimeSpan(10,0,0),
                    CloseTime = new TimeSpan(16,0,0),
                    Note = "Testing open hours notes"
                }
            };
            return openHours;
        }

        public static IEnumerable<Vacancy> GetTestVacancies()
        {
            IEnumerable<Vacancy> vacancies = new List<Vacancy>
            {
                new Vacancy
                {
                    Id = 1,
                    JobTitle = "Training instructor",
                    JobType = Enums.JobType.Fulltime,
                    JobPeriod = Enums.JobPeriod.Permanent,
                    PayInterval = Enums.PayInterval.PerAnnum,
                    Salary = 20000m,
                    Description = string.Empty
                },
                new Vacancy
                {
                    Id = 2,
                    JobTitle = "Maintenance staff",
                    JobType = Enums.JobType.Parttime,
                    JobPeriod = Enums.JobPeriod.Temporary,
                    PayInterval = Enums.PayInterval.PerWeek,
                    Salary = 500m,
                    Description = "Experienced maintenance person required."
                }
            };
            return vacancies;
        }

        public static IEnumerable<FreePass> GetTestFreePass()
        {
            IEnumerable<FreePass> freePasses = new List<FreePass>
            {
                new FreePass
                {
                    Id = 1,
                    Name = "Nelish",
                    Email = "nelish@testing.com",
                    DateIssued = new DateTime(2021,1,1),
                    DateUsed = new DateTime(2021, 1, 20)
                },
                new FreePass
                {
                    Id = 2,
                    Name = "Julie",
                    Email = "Julie@testing.com",
                    DateIssued = new DateTime(2021,2,1),
                    DateUsed = new DateTime(2021, 2, 20)
                }
            };
            return freePasses;
        }

        public static IEnumerable<MembershipDeal> GetTestMembershipDeal()
        {
            IEnumerable<MembershipDeal> membershipDeals = new List<MembershipDeal>
            {
                new MembershipDeal
                {
                    Id = 1,
                    Duration = Enums.MembershipDuration.OneDay,
                    Price = 4m
                },
                new MembershipDeal
                {
                    Id = 2,
                    Duration = Enums.MembershipDuration.Unlimited,
                    Price = 500m
                }
            };
            return membershipDeals;
        }

        public static IEnumerable<ApplicationUser> GetTestUsers()
        {
            IEnumerable<ApplicationUser> users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = Enums.Title.Mr,
                    FirstName = "Johan",
                    LastName = "Singh",
                    NormalizedUserName = "TESTING@TEST.COM",
                    Gender = Enums.Gender.Male,
                    DateOfBirth = new DateTime(1990,3,4),
                    UserName = "testing@test.com",
                    Email = "testing@test.com",
                    NormalizedEmail = "TESTING@TEST.COM",
                    EmailConfirmed = true,
                    PasswordHash = "haSheDPa55worD",
                    PhoneNumber = "0887878822",
                    AddressLineOne = "12 baker street",
                    AddressLineTwo = string.Empty,
                    Town = "Brighton",
                    Postcode = "BR12 4RQ",
                    TwoFactorEnabled = false
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = Enums.Title.Ms,
                    FirstName = "Katie",
                    LastName = "Pearson",
                    NormalizedUserName = "TESTING2@TEST.COM",
                    Gender = Enums.Gender.Female,
                    DateOfBirth = new DateTime(1980,2,14),
                    UserName = "testing2@test.com",
                    Email = "testing2@test.com",
                    NormalizedEmail = "TESTING2@TEST.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "0771878835",
                    AddressLineOne = "44 Parkor street",
                    AddressLineTwo = string.Empty,
                    Town = "Manchester",
                    Postcode = "M12 8PL",
                    TwoFactorEnabled = false
                }
            };

            return users;
        }
    
        public static IEnumerable<IdentityRole> GetTestIdentityRoles()
        {
            IEnumerable<IdentityRole> identityRoles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin"
                },
                new IdentityRole
                {
                    Name = "Director"
                }
            };
            return identityRoles;
        }
    }
}
