using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models.Repository
{
    public interface IFreePassRepository
    {
        FreePass GetFreePass(int id);
        IEnumerable<FreePass> GetFreePasses();
        void Add(FreePass freePass);
        void Update(FreePass changedFreePass);
        void Delete(int id);
    }
}
