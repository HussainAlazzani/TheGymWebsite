using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TheGymWebsite.ViewModels
{
    public class GymViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 letters")]
        [Display(Name = "Gym Name")]
        public string GymName { get; set; } = "The Gym";

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "1st line Address")]
        public string AddressLineOne { get; set; }

        [Display(Name = "2nd line Address")]
        public string AddressLineTwo { get; set; }

        [Required]
        public string Town { get; set; }

        [Required]
        public string Postcode { get; set; }

        [Required, Phone]
        public string Telephone { get; set; }
    }
}
