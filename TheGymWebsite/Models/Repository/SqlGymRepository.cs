using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models.Repository
{
    public class SqlGymRepository : IGymRepository
    {
        private readonly GymDbContext context;

        public SqlGymRepository(GymDbContext context)
        {
            this.context = context;
        }

        public void Add(Gym gym)
        {
            context.Gyms.Add(gym);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            Gym gym = context.Gyms.Find(id);
            if (gym != null)
            {
                context.Gyms.Remove(gym);
                context.SaveChanges();
            }
        }

        public IEnumerable<Gym> GetGyms()
        {
            return context.Gyms;
        }

        public Gym GetGym(int id)
        {
            return context.Gyms.Find(id);
        }

        public void Update(Gym changedGym)
        {
            var gym = context.Gyms.Attach(changedGym);
            gym.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }
    }
}
