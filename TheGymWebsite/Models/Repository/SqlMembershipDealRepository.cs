using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.Models.Repository
{
    public class SqlMembershipDealRepository : IMembershipDealRepository
    {
        private readonly GymDbContext context;

        public SqlMembershipDealRepository(GymDbContext context)
        {
            this.context = context;
        }

        public void Add(MembershipDeal membershipDeal)
        {
            context.MembershipDeals.Add(membershipDeal);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            MembershipDeal membershipDeal = context.MembershipDeals.Find(id);
            if (membershipDeal != null)
            {
                context.MembershipDeals.Remove(membershipDeal);
                context.SaveChanges();
            }
        }

        public IEnumerable<MembershipDeal> GetMembershipDeals()
        {
            return context.MembershipDeals;
        }

        public MembershipDeal GetMembershipDeal(int id)
        {
            return context.MembershipDeals.Find(id);
        }

        public bool IsDurationOffered(Enums.MembershipDuration duration)
        {
            int d = context.MembershipDeals.Where(m => m.Duration == duration).Select(m=>m.Id).FirstOrDefault();
            return d == 0 ? false : true;
        }

        public void Update(MembershipDeal changedMembershipDeal)
        {
            var membershipDeal = context.MembershipDeals.Attach(changedMembershipDeal);
            membershipDeal.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }
    }
}
