using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TheGymWebsite.Models
{
    public class Gym
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 letters")]
        public string GymName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string AddressLineOne { get; set; }

        public string AddressLineTwo { get; set; }

        [Required]
        public string Town { get; set; }

        [Required]
        public string Postcode { get; set; }

        [Required, Phone]
        public string Telephone { get; set; }
    }
}
