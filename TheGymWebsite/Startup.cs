using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using System;
using System.Collections.Generic;
using System.Globalization;
using TheGymWebsite.Middlewares;
using TheGymWebsite.Models;
using TheGymWebsite.Models.Repository;
using TheGymWebsite.Security;

namespace TheGymWebsite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding the services common to Web API and the services required for rendering Razor views.
            // Also enforcing authorization policy globally.
            services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            // Get the email from secrets json and bind it to the Email class.
            services.Configure<Email>(Configuration.GetSection("Email"));

            // Enabling authorization based on policy.
            services.AddAuthorization(options =>
            {
                // Roles.
                options.AddPolicy("AdminAccess", policy => policy.RequireRole("Admin"));

                // Role Claims.
                options.AddPolicy("ManageUsersPermission", policy => policy.RequireClaim("ManageUsers", true.ToString()));
                options.AddPolicy("ManageRolesPermission", policy => policy.RequireClaim("ManageRoles", true.ToString()));
                options.AddPolicy("ManageBusinessPermission", policy => policy.RequireClaim("ManageBusiness", true.ToString()));
                options.AddPolicy("IssueBansPermission", policy => policy.RequireClaim("IssueBans", true.ToString()));

                // Claims.
                options.AddPolicy("MinimumAge16", policy => policy.AddRequirements(new MinimumAgeRequirement(16)));
                // This checks if user's membership has not expired and that they are not banned.
                options.AddPolicy("MembershipActive", policy => policy.AddRequirements(new MembershipActiveRequirement()));
                // Access to employees only.
                options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("Employee"));

                // Access to employees OR users who are granted any of the following listed claims.
                options.AddPolicy("CanViewUsers", policy => policy.RequireAssertion
                (
                    context => context.User.HasClaim(c =>
                    (
                        c.Type == "ManageUsers"
                        || c.Type == "ManageRoles"
                        || c.Type == "ManageBusiness"
                        || c.Type == "IssueBans"
                        || c.Type == "Employee"
                    ))
                ));
            });

            // Adding the ASP.NET core Identity membership system along with the EF setup and the default tokens to generate for
            // password resets, change of email, etc. I am also configuring password requirements and enforcing unique emails.
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Email requirements
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

                // Password requirements.
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Setting lockout options
                options.Lockout.MaxFailedAccessAttempts = 5;
                // I am setting the lockout time span to 5 minutes which is the default setting.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddEntityFrameworkStores<GymDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                // This is setting the token life span to 1 day which is the default setting.
                // This affects all tokens generated including the email and password tokens.
                // To change the setting for each token, custom token provider must be created.
                options.TokenLifespan = TimeSpan.FromDays(1);
            });

            // This is where clients are redirected to when authorization is required.
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
                options.AccessDeniedPath = new PathString("/Error/AccessDenied");
                //options.Cookie.HttpOnly = true;
                //options.Cookie.Expiration = TimeSpan.FromDays(5);
            });

            // This sets the culture globally to UK.
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            // Use local SQL server when in development mode, otherwise use azure sql server
            // I cannot use env.IsDevelopment() because configureServices only takes one parameter.
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                // Reusing DbContext instances rather than creating new one for each request. This can increase throughput.
                services.AddDbContextPool<GymDbContext>(options =>
                {
                    // Setting up the connection string using the MS SQL provider.
                    options.UseSqlServer(Configuration.GetConnectionString("GymDBConnection"));
                });
            }
            else
            {
                //// Remote sql server
                //services.AddDbContextPool<GymDbContext>(options =>
                //        options.UseSqlServer(Configuration.GetConnectionString("....")));
            }

            // Setting the life time of the objects to scoped, ie. a new instance is created for every client request.
            services.AddScoped<IGymRepository, SqlGymRepository>();
            services.AddScoped<IOpenHoursRepository, SqlOpenHoursRepository>();
            services.AddScoped<IVacancyRepository, SqlVacancyRepository>();
            services.AddScoped<IFreePassRepository, SqlFreePassRepository>();
            services.AddScoped<IMembershipDealRepository, SqlMembershipDealRepository>();
            services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
            services.AddSingleton<IAuthorizationHandler, MembershipActiveHandler>();
            services.AddScoped<IAttendanceRepository, SqlAttendanceRepository>();
            services.AddScoped<IEmailService, EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRequestLocalization();
            app.UseStaticFiles();

            // API secret key for stripe getway service.
            StripeConfiguration.ApiKey = Configuration.GetValue<string>("Stripe:Secret");

            // Conforms the app to the EU General Data Protection Regulation (GDPR) regulations.
            app.UseCookiePolicy();

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseMiddleware<EndpointLoggingMiddleware>();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
