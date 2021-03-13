using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models.Repository
{
    public interface IMembershipDealRepository
    {
        MembershipDeal GetMembershipDeal(int id);
        IEnumerable<MembershipDeal> GetMembershipDeals();
        void Add(MembershipDeal membershipDeal);
        void Update(MembershipDeal changedMembershipDeal);
        void Delete(int id);
        bool IsDurationOffered(Enums.MembershipDuration duration);
    }
}
