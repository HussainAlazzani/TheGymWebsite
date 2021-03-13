using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using static TheGymWebsite.Enums;

namespace TheGymWebsite.ViewModels
{
    public class VacancyViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "The job title is too long")]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }
        
        [Display(Name = "Job Type")]
        public JobType JobType { get; set; }
        
        [Display(Name = "Job Period")]
        public JobPeriod JobPeriod { get; set; }
        
        [DataType(DataType.Currency , ErrorMessage = "Enter appropriate salary figure")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal Salary { get; set; }
        
        [Display(Name = "Pay interval")]
        public PayInterval PayInterval { get; set; }

        [Display(Name = "Job Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }
}
