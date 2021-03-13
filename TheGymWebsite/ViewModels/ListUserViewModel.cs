using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TheGymWebsite.Enums;

namespace TheGymWebsite.ViewModels
{
    public class ListUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }

        public Title Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int DayOfBirth { get; set; }
        public int MonthOfBirth { get; set; }
        public int YearOfBirth { get; set; }

        public Gender Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
    }
}
