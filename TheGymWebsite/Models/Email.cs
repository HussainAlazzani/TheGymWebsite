using System.ComponentModel.DataAnnotations;

namespace TheGymWebsite.Models
{
    public class Email
    {
        [EmailAddress]
        public string Address { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
