using System;

namespace TheGymWebsite.Models
{
    public class MembershipExpiry
    {
        public static DateTime SetExpiryDate(Enums.MembershipDuration duration)
        {
            DateTime today = DateTime.Now;

            switch (duration)
            {
                case Enums.MembershipDuration.OneDay:
                    return today.AddDays(1);
                case Enums.MembershipDuration.OneWeek:
                    return today.AddDays(8);
                case Enums.MembershipDuration.TwoWeeks:
                    return today.AddDays(15);
                case Enums.MembershipDuration.OneMonth:
                    return today.AddMonths(1);
                case Enums.MembershipDuration.TwoMonth:
                    return today.AddMonths(2);
                case Enums.MembershipDuration.ThreeMonths:
                    return today.AddMonths(3);
                case Enums.MembershipDuration.FourMonths:
                    return today.AddMonths(4);
                case Enums.MembershipDuration.SixMonths:
                    return today.AddMonths(6);
                case Enums.MembershipDuration.OneYear:
                    return today.AddYears(1);
                case Enums.MembershipDuration.TwoYears:
                    return today.AddYears(2);
                case Enums.MembershipDuration.Unlimited:
                    return DateTime.MaxValue;
                default:
                    return DateTime.MinValue;
            }
        }
    }
}
