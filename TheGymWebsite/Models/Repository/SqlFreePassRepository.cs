using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models.Repository
{
    public class SqlFreePassRepository : IFreePassRepository
    {
        private readonly GymDbContext context;

        public SqlFreePassRepository(GymDbContext context)
        {
            this.context = context;
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
    }
}
