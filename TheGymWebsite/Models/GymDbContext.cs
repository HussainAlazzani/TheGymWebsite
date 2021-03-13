using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TheGymWebsite.Models
{
    public class GymDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// In order for the class to be useful, we must pass in the DbContextOptions.
        /// </summary>
        /// <param name="options">Carries configuration info such as connection string, database provider, etc.
        /// T is the context class which is passed to the base class.</param>
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

        /// <summary>
        /// Implementing a separate entity for the Gym. No relationships established between the users and the Gym as of yet.
        /// </summary>
        public DbSet<Gym> Gyms { get; set; }

        /// <summary>
        /// Implementing a separate entity for membership deals.
        /// </summary>
        public DbSet<MembershipDeal> MembershipDeals { get; set; }

        /// <summary>
        /// This will store the times and dates of the gym open hours.
        /// </summary>
        public DbSet<OpenHours> OpenHours { get; set; }

        /// <summary>
        /// This will store vacancy details.
        /// </summary>
        public DbSet<Vacancy> Vacancies { get; set; }

        /// <summary>
        /// This will store details about the Free one day pass.
        /// </summary>
        public DbSet<FreePass> FreePasses { get; set; }

        /// <summary>
        /// This will be used to acces the attendence record of each gym member.
        /// </summary>
        public DbSet<GymAttendance> AttendanceRecord { get; set; }

        /// <summary>
        /// This allows us to configure the database upon creation using Fluent API.
        /// </summary>
        /// <param name="builder">The object to configure the database.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Establishing a one-to-many relationship between the user and his attendance record.
            // An attendance record holds the information for every user; each user has multiple attendances.
            // A foreign key is added to reference the user by their id. This foreign key cannot be null.
            // The delete behaviour is set to cascade; therefore when a user is deleted, their attendance record is also deleted.
            builder.Entity<GymAttendance>()
                .HasOne<ApplicationUser>(x => x.User)
                .WithMany(x => x.AttendanceRecord)
                .HasForeignKey(x => x.UserId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            // These id's will be used to seed an 'admin' role and assign a user to it.
            string adminId = Guid.NewGuid().ToString();
            string roleId = Guid.NewGuid().ToString();

            // Creating the admin role.
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = roleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

            // Creating a new admin user.
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = adminId,
                Title = Enums.Title.Mr,
                FirstName = "AdminFirstName",
                LastName = "AdminLastName",
                DateOfBirth = new DateTime(1960, 01, 01),
                Gender = Enums.Gender.Male,
                PhoneNumber = "00000000000",
                Email = "admin@admin.com",
                NormalizedEmail = "admin@admin.com".ToUpper(),
                UserName = "admin@admin.com",
                NormalizedUserName = "admin@admin.com".ToUpper(),
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "admin"),
                EmailConfirmed = true,

                AddressLineOne = "1 Admin Road",
                AddressLineTwo = "Admin Area",
                Town = "AdminTown",
                Postcode = "AD1 2MN"

            });

            // Assigning the admin user to the admin role.
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = roleId,
                UserId = adminId
            });

            // Assigning four claims to the admin role.
            builder.Entity<IdentityRoleClaim<string>>().HasData(
                new IdentityRoleClaim<string> { Id = 1, RoleId = roleId, ClaimType = "ManageBusiness", ClaimValue = true.ToString() },
                new IdentityRoleClaim<string> { Id = 2, RoleId = roleId, ClaimType = "ManageRoles", ClaimValue = true.ToString() },
                new IdentityRoleClaim<string> { Id = 3, RoleId = roleId, ClaimType = "ManageUsers", ClaimValue = true.ToString() },
                new IdentityRoleClaim<string> { Id = 4, RoleId = roleId, ClaimType = "IssueBans", ClaimValue = true.ToString() }
            );

            // Assigning three claims to the admin user.
            builder.Entity<IdentityUserClaim<string>>().HasData(
                new IdentityUserClaim<string> { Id = 1, UserId = adminId, ClaimType = "DateOfBirth", ClaimValue = (new DateTime(2000, 01, 01).ToShortDateString()) },
                new IdentityUserClaim<string> { Id = 2, UserId = adminId, ClaimType = "Employee", ClaimValue = DateTime.Now.ToShortDateString() },
                new IdentityUserClaim<string> { Id = 3, UserId = adminId, ClaimType = "MembershipExpiry", ClaimValue = DateTime.MaxValue.ToShortDateString() }
            );

            // Creating fictional users for development
            SeedUsers.Seed(builder, "huss", Enums.Gender.Male, new DateTime(2000, 01, 01), 4);
            SeedUsers.Seed(builder, "beky", Enums.Gender.Female, new DateTime(1950, 01, 01), 5);
            SeedUsers.Seed(builder, "alice", Enums.Gender.Female, new DateTime(1960, 01, 01), 6);
            SeedUsers.Seed(builder, "seba", Enums.Gender.Female, new DateTime(1970, 01, 01), 7);
            SeedUsers.Seed(builder, "john", Enums.Gender.Male, new DateTime(1994, 01, 01), 8);
            SeedUsers.Seed(builder, "tom", Enums.Gender.Male, new DateTime(1993, 01, 01), 9);
            SeedUsers.Seed(builder, "jack", Enums.Gender.Male, new DateTime(1984, 01, 01), 10);
            SeedUsers.Seed(builder, "jam", Enums.Gender.Male, new DateTime(1982, 01, 01), 11);
            SeedUsers.Seed(builder, "mark", Enums.Gender.Male, new DateTime(2010, 01, 01), 12);

            // Seed data to initialise the Gym details. Other gyms may be added by the admins as the business grows.
            builder.Entity<Gym>().HasData(new Gym
            {
                Id = 1,
                GymName = "The Gym",
                Email = "thegymbirmingham@yahoo.com",
                AddressLineOne = "33 Oak road",
                AddressLineTwo = "Erdon",
                Town = "Birmingham",
                Postcode = "B20 1EZ",
                Telephone = "07739983984"
            });

            // Setting the Period property to be unique.
            builder.Entity<MembershipDeal>().HasIndex(m => m.Duration).IsUnique();
            // Seed data to initialise the membership deals.
            builder.Entity<MembershipDeal>().HasData(
                new MembershipDeal
                {
                    Id = 1,
                    Duration = Enums.MembershipDuration.OneWeek,
                    Price = 10m
                },
                new MembershipDeal
                {
                    Id = 2,
                    Duration = Enums.MembershipDuration.OneMonth,
                    Price = 20m
                },
                new MembershipDeal
                {
                    Id = 3,
                    Duration = Enums.MembershipDuration.SixMonths,
                    Price = 100m
                },
                new MembershipDeal
                {
                    Id = 4,
                    Duration = Enums.MembershipDuration.OneYear,
                    Price = 160m
                }
            );

            // Seed data to initialise the gym opening hours.
            builder.Entity<OpenHours>().HasData(
                new OpenHours
                {
                    Id = 1,
                    DayName = DayOfWeek.Monday,
                    OpenTime = new TimeSpan(6, 0, 0),
                    CloseTime = new TimeSpan(22, 0, 0)
                },
                new OpenHours
                {
                    Id = 2,
                    DayName = DayOfWeek.Tuesday,
                    OpenTime = new TimeSpan(6, 0, 0),
                    CloseTime = new TimeSpan(22, 0, 0)
                },
                new OpenHours
                {
                    Id = 3,
                    DayName = DayOfWeek.Wednesday,
                    OpenTime = new TimeSpan(6, 0, 0),
                    CloseTime = new TimeSpan(22, 0, 0)
                },
                new OpenHours
                {
                    Id = 4,
                    DayName = DayOfWeek.Thursday,
                    OpenTime = new TimeSpan(6, 0, 0),
                    CloseTime = new TimeSpan(22, 0, 0)
                },
                new OpenHours
                {
                    Id = 5,
                    DayName = DayOfWeek.Friday,
                    OpenTime = new TimeSpan(6, 0, 0),
                    CloseTime = new TimeSpan(22, 0, 0)
                },
                new OpenHours
                {
                    Id = 6,
                    DayName = DayOfWeek.Saturday,
                    OpenTime = new TimeSpan(8, 0, 0),
                    CloseTime = new TimeSpan(20, 0, 0)
                },
                new OpenHours
                {
                    Id = 7,
                    DayName = DayOfWeek.Sunday,
                    OpenTime = new TimeSpan(8, 0, 0),
                    CloseTime = new TimeSpan(20, 0, 0)
                }
            );
        }
    }
}