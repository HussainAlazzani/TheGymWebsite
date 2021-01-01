using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models.Repository
{
    public interface IOpenHoursRepository
    {
        OpenHours GetOpenHours(int id);
        IEnumerable<OpenHours> GetOpenHours();
        void Add(OpenHours openHours);
        void Update(OpenHours changedOpenHours);
        void Delete(int id);
    }
}
