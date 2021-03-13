using System.Collections.Generic;

namespace TheGymWebsite.ViewModels
{
    public class EditUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        // Roles in system.
        public IList<string> Roles { get; set; } = new List<string>();
        // Roles the user is assigned to.
        public List<bool> IsInRole { get; set; } = new List<bool>();

        // Role name to Add or Remove
        public string RoleName { get; set; }
    }
}
