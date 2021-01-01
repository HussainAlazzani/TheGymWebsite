using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace TheGymWebsite
{
    public class Enums
    {
        public enum Title
        {
            Mr,
            Mrs,
            Miss,
            Ms
        }

        public enum Gender
        {
            Male,
            Female
        }

        public enum MembershipDuration
        {
            [Display (Name = "1 Day")]
            OneDay, [Display (Name = "1 Week")]
            OneWeek, [Display (Name = "2 Weeks")]
            TwoWeeks, [Display (Name = "1 Month")]
            OneMonth, [Display (Name = "2 Months")]
            TwoMonth, [Display (Name = "3 Months")]
            ThreeMonths, [Display (Name = "4 Months")]
            FourMonths, [Display (Name = "6 Months")]
            SixMonths, [Display (Name = "1 Year")]
            OneYear, [Display (Name = "2 Years")]
            TwoYears, [Display (Name = "Unlimited")]
            Unlimited
        }

        public enum JobType
        {
            [Display (Name = "Full-time")]
            Fulltime, [Display (Name = "Part-time")]
            Parttime,
            Weekends
        }

        public enum JobPeriod
        {
            Permanent,
            Temporary
        }

        public enum PayInterval
        {
            [Display (Name = "Per annum")]
            PerAnnum, [Display (Name = "Per month")]
            PerMonth, [Display (Name = "Per week")]
            PerWeek, [Display (Name = "Per hour")]
            PerHour
        }

        // Helper method to display the name of the enum values.
        public static string GetDisplayName (Enum value)
        {
            return value.GetType () ?
                .GetMember (value.ToString ())?.First () ?
                .GetCustomAttribute<DisplayAttribute> () ?
                .Name;
        }
    }
}