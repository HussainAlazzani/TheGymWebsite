using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models.Repository
{
    public interface IGymRepository
    {
        Gym GetGym(int id);
        IEnumerable<Gym> GetGyms();
        void Add(Gym gym);
        void Update(Gym changedGym);
        void Delete(int id);
    }
}
