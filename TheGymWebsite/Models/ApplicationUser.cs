using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static TheGymWebsite.Enums;

namespace TheGymWebsite.Models
{
    public class ApplicationUser : IdentityUser
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

        // Establishing a one-to-one relation between user and his attendance record. For a one-to-one relationship,
        // a reference navigation property is required in both classes.
        public ICollection<GymAttendance> AttendanceRecord { get; set; }
    }
}
