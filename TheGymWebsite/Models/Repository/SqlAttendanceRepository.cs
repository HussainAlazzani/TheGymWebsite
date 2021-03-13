using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models
{
    public class SqlAttendanceRepository : IAttendanceRepository
    {
        private readonly GymDbContext context;

        public SqlAttendanceRepository(GymDbContext context)
        {
            this.context = context;
        }

        public void Add(GymAttendance attendance)
        {
            context.AttendanceRecord.Add(attendance);
            context.SaveChanges();
        }

        /// <summary>
        /// This method retrieves the attendance record of a user by specifying their id. The search is further filtered by
        /// only returning the dates that the user attended.
        /// </summary>
        /// <param name="userId">The id of the user whose attendance record is sought</param>
        /// <returns>I have opted for the Iqueryable return type over the IEnumerable because I want the search results
        /// to be filtered by the database server before sending it to the applicaiton. This improves efficiency.</returns>
        public IQueryable<DateTime> GetAllAttendance(string userId)
        {
            return context.AttendanceRecord.Where(x => x.UserId == userId).Select(x => x.Date);
        }

        /// <summary>
        /// This method retrieves the attendance record of a user from a certain date by specifying their id and the chosen date.
        /// The search is further filtered by only returning the dates that the user attended.
        /// </summary>
        /// <param name="userId">The id of the user whose attendance record is sought</param>
        /// <param name="fromDateTime"></param>
        /// <returns>I have opted for the Iqueryable return type over the IEnumerable because I want the search results
        /// to be filtered by the database server before sending it to the applicaiton. This improves efficiency.</returns>
        public IQueryable<DateTime> GetAttendanceFromDate(string userId, DateTime fromDateTime)
        {
            return context.AttendanceRecord.Where(x => x.UserId == userId).Select(x => x.Date).Where(d => d > fromDateTime);
        }


        //public IEnumerable<DateTime> GetAttendanceRecord(string userId)
        //{
        //    return context.AttendanceRecord.Where(x => x.UserId == userId).Select(x => x.Date);
        //}
    }
}
