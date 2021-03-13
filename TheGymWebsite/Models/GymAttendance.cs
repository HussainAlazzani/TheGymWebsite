using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TheGymWebsite.Models
{
    public class GymAttendance
    {
        public long Id { get; set; }

        public DateTime Date { get; set; }

        // This is the foreign key property which will reference the user's id.
        public string UserId { get; set; }
        
        // Establishing a one-to-many relationship between the user and his attendance record, whereby each user will have multiple
        // records that register his attendances. This navigation property will implement this by EF core convention.
        public ApplicationUser User { get; set; }
    }
}
