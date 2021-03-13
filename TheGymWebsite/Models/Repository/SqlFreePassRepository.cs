using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace TheGymWebsite.Models.Repository
{
    public class SqlFreePassRepository : IFreePassRepository
    {
        private readonly GymDbContext context;
        private readonly IWebHostEnvironment env;

        public SqlFreePassRepository(GymDbContext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }

        public void Add(FreePass freePass)
        {
            context.FreePasses.Add(freePass);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            FreePass freePass = context.FreePasses.Find(id);
            if (freePass != null)
            {
                context.FreePasses.Remove(freePass);
                context.SaveChanges();
            }
        }

        public IEnumerable<FreePass> GetFreePasses()
        {
            return context.FreePasses;
        }

        public FreePass GetFreePass(int id)
        {
            return context.FreePasses.Find(id);
        }

        public void Update(FreePass changedFreePass)
        {
            var freePass = context.FreePasses.Attach(changedFreePass);
            freePass.State = EntityState.Modified;
            context.SaveChanges();
        }

        public int GetFreePassIdFromEmail(string email)
        {
            if (env.IsDevelopment())
            {
                // Linq-to-Entity does not support Last() and LastOrDefault() so I am ordering it by descending order then using first method.
                return  context.FreePasses
                        .Where(x => x.Email == email)
                        .Select(x => x.Id)
                        .OrderByDescending(x => x)
                        .FirstOrDefault();
            }

            return context.FreePasses
                .Where(x => x.Email == email)
                .Select(x => x.Id)
                .SingleOrDefault();
        }
    }
}
