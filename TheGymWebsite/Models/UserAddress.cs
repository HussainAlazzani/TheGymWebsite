using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TheGymWebsite.Models
{
    public class UserAddress
    {
        public int Id { get; set; }

        [Required]
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        [Required]
        public string Town { get; set; }
        [Required]
        public string Postcode { get; set; }
    }
}
