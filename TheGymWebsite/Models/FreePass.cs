using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace TheGymWebsite.Models
{
    public class FreePass
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        public DateTime DateIssued { get; set; }
        public DateTime? DateUsed { get; set; }
    }
}
