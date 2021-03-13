using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models
{
    public interface IAttendanceRepository
    {
        IQueryable<DateTime> GetAllAttendance(string userId);
        IQueryable<DateTime> GetAttendanceFromDate(string userId, DateTime fromDateTime);
        void Add(GymAttendance attendance);
    }
}

