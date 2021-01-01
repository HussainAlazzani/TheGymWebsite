using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using static TheGymWebsite.Enums;

namespace TheGymWebsite.Models
{
    public class Vacancy
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200, ErrorMessage = "The job title is too long")]
        public string JobTitle { get; set; }
        public JobType JobType { get; set; }
        public JobPeriod JobPeriod { get; set; }
        public Decimal Salary { get; set; }
        public PayInterval PayInterval { get; set; }
        public string Description { get; set; }
    }
}
