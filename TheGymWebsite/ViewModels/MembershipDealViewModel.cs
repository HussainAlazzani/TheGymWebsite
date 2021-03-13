using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static TheGymWebsite.Enums;

namespace TheGymWebsite.ViewModels
{
    public class MembershipDealViewModel
    {
        public int Id { get; set; }
        [Required]
        public MembershipDuration Duration { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
