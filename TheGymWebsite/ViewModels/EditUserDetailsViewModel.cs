using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using static TheGymWebsite.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TheGymWebsite.ViewModels
{
    public class EditUserDetailsViewModel
    {
        public string Email { get; set; }

        public Title Title { get; set; }

        [Display(Name = "First Name")]
        [RegularExpression("[a-zA-Z/'/-]+")]
        [MaxLength(100, ErrorMessage = "The last name is too long")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [RegularExpression("[a-zA-Z/'/-]+")]
        [MaxLength(100, ErrorMessage = "The last name is too long")]
        public string LastName { get; set; }

        //[DataType(DataType.Date)]
        //[Display(Name = "Date of Birth")]
        //public DateTime DateOfBirth { get; set; }

        public int DayOfBirth { get; set; }
        
        public int MonthOfBirth { get; set; }
        
        public int YearOfBirth { get; set; }

        public Gender Gender { get; set; }

        [Phone]
        [Display(Name = "Contact Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Address Line 1")]
        [MaxLength(250)]
        public string AddressLineOne { get; set; }

        [Display(Name = "Address Line 2")]
        [MaxLength(250)]
        public string AddressLineTwo { get; set; }

        [MaxLength(100)]
        public string Town { get; set; }

        [RegularExpression("^([A-Za-z][A-Ha-hJ-Yj-y]?[0-9][A-Za-z0-9]? ?[0-9][A-Za-z]{2}|[Gg][Ii][Rr] ?0[Aa]{2})$")]
        public string Postcode { get; set; }
    }
}
