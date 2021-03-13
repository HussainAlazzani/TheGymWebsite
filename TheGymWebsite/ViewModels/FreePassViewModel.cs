using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TheGymWebsite.ViewModels
{
    public class FreePassViewModel
    {
        [Required]
        [MaxLength(200, ErrorMessage = "The name is too long")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime DateIssued { get; set; }
    }
}
