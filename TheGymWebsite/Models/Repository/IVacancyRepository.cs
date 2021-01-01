using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models.Repository
{
    public interface IVacancyRepository
    {
        Vacancy GetVacancy(int id);
        IEnumerable<Vacancy> GetVacancies();
        void Add(Vacancy vacancy);
        void Update(Vacancy changedVacancy);
        void Delete(int id);
    }
}
