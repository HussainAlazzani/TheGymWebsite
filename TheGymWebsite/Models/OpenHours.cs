using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TheGymWebsite.Models
{
    public class OpenHours
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public DayOfWeek DayName { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan OpenTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan CloseTime { get; set; }

        public string Note { get; set; }
    }
}