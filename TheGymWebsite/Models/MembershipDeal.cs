using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using static TheGymWebsite.Enums;

namespace TheGymWebsite.Models
{
    public class MembershipDeal
    {
        public int Id { get; set; }
        [Required]
        public MembershipDuration Duration { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
