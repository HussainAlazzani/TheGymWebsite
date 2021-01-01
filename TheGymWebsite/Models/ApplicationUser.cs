using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using static TheGymWebsite.Enums;

namespace TheGymWebsite.Models
{
    public class ApplicationUser
    {
        [Required]
        public Title Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public string AddressLineOne { get; set; }

        public string AddressLineTwo { get; set; }

        [Required]
        public string Town { get; set; }

        [Required]
        public string Postcode { get; set; }
    }
}
