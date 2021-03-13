using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TheGymWebsite.ViewModels
{
    public class OpenHoursViewModel
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public int Day { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }

        public DayOfWeek DayName { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Opening time")]
        public TimeSpan OpenTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Closing time")]
        public TimeSpan CloseTime { get; set; }

        [MaxLength(200, ErrorMessage = "Name cannot exceed 50 letters")]
        public string Note { get; set; }

        public bool IsChecked { get; set; }
    }
}
