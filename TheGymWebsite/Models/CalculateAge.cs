using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models
{
    public class CalculateAge
    {
        public static int GetAge(DateTime dateOfBirth)
        {
            if (dateOfBirth > DateTime.Now)
            {
                throw new Exception("Invalid date of birth");
            }

            int age;
            age = DateTime.Now.Year - dateOfBirth.Year;

            // If a whole year is not complete, then deduct 1 to get the age.
            // Example, DoB is 05/01/2010, and current date is 04/01/2020; then age should be 9 (not 10).
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age -= 1;

            return age;
        }
    }
}
