using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheGymWebsite.ViewModels
{
    public class MemberViewModel
    {
        public string FullName { get; set; }
        public string MembershipStatus { get; set; }
        public string MembershipExpiration { get; set; }

        public Dictionary<string, int> Chart { get; set; } = new Dictionary<string, int>();
    }
}
