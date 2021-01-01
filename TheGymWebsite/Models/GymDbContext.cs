using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TheGymWebsite.Models
{
    public class GymDbContext : DbContext
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
        /// This allows us to configure the database upon creation using Fluent API.
        /// </summary>
        /// <param name="builder">The object to configure the database.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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