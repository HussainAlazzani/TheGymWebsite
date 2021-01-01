using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models.Repository
{
    public class SqlOpenHoursRepository : IOpenHoursRepository
    {
        private readonly GymDbContext context;

        public SqlOpenHoursRepository(GymDbContext context)
        {
            this.context = context;
        }

        public void Add(OpenHours openHours)
        {
            context.OpenHours.Add(openHours);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            OpenHours openHours = context.OpenHours.Find(id);
            if (openHours != null)
            {
                context.OpenHours.Remove(openHours);
                context.SaveChanges();
            }
        }

        public IEnumerable<OpenHours> GetOpenHours()
        {
            return context.OpenHours;
        }

        public OpenHours GetOpenHours(int id)
        {
            return context.OpenHours.Find(id);
        }

        public void Update(OpenHours changedOpenHours)
        {
            var openHours = context.OpenHours.Attach(changedOpenHours);
            openHours.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();            
        }
    }
}
