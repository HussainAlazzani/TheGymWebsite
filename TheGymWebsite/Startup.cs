using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheGymWebsite.Models;
using TheGymWebsite.Models.Repository;

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
            services.AddControllersWithViews();

            services.AddDbContextPool<GymDbContext>(options =>
            {
                // Setting up the connection string using the MS SQL provider.
                options.UseSqlServer(Configuration.GetConnectionString("GymDBConnection"));
            });

            // Setting the life time of the objects to scoped, ie. a new instance is created for every client request.
            services.AddScoped<IGymRepository, SqlGymRepository>();
            services.AddScoped<IOpenHoursRepository, SqlOpenHoursRepository>();
            services.AddScoped<IVacancyRepository, SqlVacancyRepository>();
            services.AddScoped<IFreePassRepository, SqlFreePassRepository>();
            services.AddScoped<IMembershipDealRepository, SqlMembershipDealRepository>();
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
            app.UseStaticFiles();

            app.UseRouting();

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
