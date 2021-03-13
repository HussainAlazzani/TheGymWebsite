using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using static TheGymWebsite.Enums;

namespace TheGymWebsite.ViewModels
{
    public class RegisterMemberViewModel
    {
        [Required]
        public Title Title { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [RegularExpression("[a-zA-Z/'/-]+")]
        [MaxLength(100, ErrorMessage = "The last name is too long")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [RegularExpression("[a-zA-Z/'/-]+")]
        [MaxLength(100, ErrorMessage = "The last name is too long")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password does not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public int DayOfBirth { get; set; }
        [Required]
        public int MonthOfBirth { get; set; }
        [Required]
        public int YearOfBirth { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Contact Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Address Line 1")]
        [MaxLength(250)]
        public string AddressLineOne { get; set; }

        [Display(Name = "Address Line 2")]
        [MaxLength(250)]
        public string AddressLineTwo { get; set; }

        [Required]
        [MaxLength(100)]
        public string Town { get; set; }

        [Required]
        [RegularExpression("^([A-Za-z][A-Ha-hJ-Yj-y]?[0-9][A-Za-z0-9]? ?[0-9][A-Za-z]{2}|[Gg][Ii][Rr] ?0[Aa]{2})$",
            ErrorMessage = "The post code you entered in not valid")]
        public string Postcode { get; set; }
    }
}
