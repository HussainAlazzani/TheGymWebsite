using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TheGymWebsite.ViewModels
{
    public class ContactUsViewModel
    {
        public int GymId { get; set; }
        public string GymName { get; set; }
        public string GymEmail { get; set; }
        public string GymAddressLineOne { get; set; }
        public string GymAddressLineTwo { get; set; }
        public string GymTown { get; set; }
        public string GymPostCode { get; set; }
        public string GymTelephone { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The name is too long")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(500, ErrorMessage = "The last name is too long")]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
