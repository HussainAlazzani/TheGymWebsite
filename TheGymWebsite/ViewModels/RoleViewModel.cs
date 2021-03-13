using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheGymWebsite.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        public List<RoleClaimViewModel> RoleClaims { get; set; } = new List<RoleClaimViewModel>();
    }

    public class RoleClaimViewModel
    {
        public string ClaimType { get; set; }
        public bool ClaimValue { get; set; }
    }
}
