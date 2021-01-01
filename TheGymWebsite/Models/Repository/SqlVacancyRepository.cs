using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models.Repository
{
    public class SqlVacancyRepository : IVacancyRepository
    {
        private readonly GymDbContext context;

        public SqlVacancyRepository(GymDbContext context)
        {
            this.context = context;
        }

        public void Add(Vacancy vacancy)
        {
            context.Vacancies.Add(vacancy);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            Vacancy vacancy = context.Vacancies.Find(id);
            if (vacancy != null)
            {
                context.Vacancies.Remove(vacancy);
                context.SaveChanges();
            }
        }

        public IEnumerable<Vacancy> GetVacancies()
        {
            return context.Vacancies;
        }

        public Vacancy GetVacancy(int id)
        {
            return context.Vacancies.Find(id);
        }

        public void Update(Vacancy changedVacancy)
        {
            var vacancy = context.Vacancies.Attach(changedVacancy);
            vacancy.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }
    }
}
