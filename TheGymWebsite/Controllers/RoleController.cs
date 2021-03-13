using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TheGymWebsite.Models;
using TheGymWebsite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace TheGymWebsite.Controllers
{
    [Authorize(Policy = "ManageRolesPermission")]
    public class RoleController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<RoleController> logger;

        public RoleController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RoleController> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }

        [Route("Role")]
        [Route("Role/ListRoles")]
        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            IEnumerable<IdentityRole> roles = roleManager.Roles.ToList();
            List<RoleViewModel> models = new List<RoleViewModel>();

            foreach (var role in roles)
            {
                IList<Claim> claims = await roleManager.GetClaimsAsync(role);

                RoleViewModel model = new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name
                };
                foreach (var claim in claims.Where(c => c.Value == "True"))
                {
                    model.RoleClaims.Add(new RoleClaimViewModel { ClaimType = claim.Type, ClaimValue = bool.Parse(claim.Value) });
                }
                models.Add(model);
            }

            return View(models);
        }

        /// <summary>
        /// This action method is used to create a role by users who fulfil the ManageRolesPermission policy.
        /// The user is presented with four claims to select from. These will be added to their newly created role.
        /// Initially, the values of these claims is set to false. Once the form is submitted, the selected claims will become true.
        /// </summary>
        [HttpGet]
        public IActionResult CreateRole()
        {
            RoleViewModel model = new RoleViewModel
            {
                RoleClaims = new List<RoleClaimViewModel>
                {
                    new RoleClaimViewModel{ ClaimType = "ManageUsers", ClaimValue = false },
                    new RoleClaimViewModel{ ClaimType = "ManageRoles", ClaimValue = false },
                    new RoleClaimViewModel{ ClaimType = "ManageBusiness", ClaimValue = false },
                    new RoleClaimViewModel{ ClaimType = "IssueBans", ClaimValue = false }
                }
            };

            return View(model);
        }

        /// <summary>
        /// A new role is created and given the name that is entered by the user. Four role claims are linked to the role.
        /// The value of the selected claims is set to true. The values of the unselected claims remain false.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityRole role = new IdentityRole(model.Name);

            // Create new role with the given name.
            IdentityResult roleResult = await roleManager.CreateAsync(role);

            if (roleResult.Succeeded)
            {
                // Add all claims selected by the user to the newly created role. Only selected role claims are set to the value true.
                foreach (var roleClaim in model.RoleClaims)
                {
                    Claim claim = new Claim(roleClaim.ClaimType, roleClaim.ClaimValue.ToString());
                    IdentityResult claimResult = await roleManager.AddClaimAsync(role, claim);
                    if (claimResult.Succeeded == false)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                return RedirectToAction(nameof(ListRoles));
            }
            else
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        /// <summary>
        /// This action method retrieves the four claims associated with the role along with their values to be edited by the user.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToAction(nameof(ListRoles));
            }

            RoleViewModel model = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
            };

            // Retrieve the list of claims associated with the role.
            List<Claim> claims = (await roleManager.GetClaimsAsync(role)).ToList();

            // Store each claim and its value in the model to be presented to the user in the form of a checkbox.
            foreach (var claim in claims)
            {
                model.RoleClaims.Add(new RoleClaimViewModel { ClaimType = claim.Type, ClaimValue = bool.Parse(claim.Value) });
            }

            return View(model);
        }

        /// <summary>
        /// Edit role name, and re-assign role claims to the role.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> EditRole(RoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityRole role = await roleManager.FindByIdAsync(model.Id);
            if (role != null)
            {
                role.Name = model.Name;

                IdentityResult roleResult = await roleManager.UpdateAsync(role);
                if (roleResult.Succeeded)
                {
                    // Removing all the claims associated with this role. 
                    foreach (var oldClaim in await roleManager.GetClaimsAsync(role))
                    {
                        await roleManager.RemoveClaimAsync(role, oldClaim);
                    }

                    // Adding the four role claims to the role. Selected claims are set to value true.
                    // Unselected and deselected role claims are set to false.
                    foreach (var newClaim in model.RoleClaims)
                    {
                        Claim claim = new Claim(newClaim.ClaimType, newClaim.ClaimValue.ToString());
                        IdentityResult claimResult = await roleManager.AddClaimAsync(role, claim);
                        if (claimResult.Succeeded == false)
                        {
                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return View(model);
                        }
                    }
                    return RedirectToAction(nameof(ListRoles));
                }


                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);

            }

            return RedirectToAction(nameof(ListRoles));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);

            if (role != null)
            {
                // Deleted the role.
                await roleManager.DeleteAsync(role);

                logger.LogInformation($"Role {role.Name} is deleted.");
            }

            return View(nameof(ListRoles), roleManager.Roles);
        }

        /// <summary>
        /// Display all the users that are assigned to the role.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> UsersInRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                // Retrieve all the users in the role.
                IEnumerable<ApplicationUser> users = await userManager.GetUsersInRoleAsync(role.Name);
                ViewBag.RoleName = role.Name;

                return View(users);
            }

            return RedirectToAction(nameof(ListRoles));
        }

        /// <summary>
        /// The user is presented with a list of users that are NOT already assigned to the role.
        /// He or she may select the users they wish to add to the role.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AddUsersToRole(string roleName)
        {
            IdentityRole role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return RedirectToAction(nameof(ListRoles));
            }

            ViewBag.roleName = role.Name;

            // Retrieve all the users in the role.
            IList<ApplicationUser> usersInRole = await userManager.GetUsersInRoleAsync(role.Name);

            // Get all the users except those that are already assigned to the role.
            // Must use AsEnumerable() to load users into local memory, otherwise the query provider is
            // unable to translate the linq expression into a query.
            // This operation is inefficient but simple and acceptable for a relatively small website like this one.
            IEnumerable<ApplicationUser> usersNotInRole = userManager.Users.AsEnumerable().Except(usersInRole);

            List<AddUsersToRoleViewModel> models = new List<AddUsersToRoleViewModel>();

            // All users are initially unchecked since they are not assigned to the role.
            foreach (var user in usersNotInRole)
            {
                models.Add(new AddUsersToRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.FirstName + " " + user.LastName,
                    UserEmail = user.Email,
                    IsChecked = false
                });
            }

            return View(models);
        }

        /// <summary>
        /// Users that were selected are added to the role.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddUsersToRole(List<AddUsersToRoleViewModel> models, string roleName)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ListRoles));
            }

            IdentityRole role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                // Loop through each user in the list.
                foreach (var modelUser in models)
                {
                    // If the user was selected, assign them to the role.
                    if (modelUser.IsChecked)
                    {
                        ApplicationUser user = await userManager.FindByIdAsync(modelUser.UserId);
                        IdentityResult result = await userManager.AddToRoleAsync(user, role.Name);
                        if (result.Succeeded == false)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
            }

            return RedirectToAction(nameof(ListRoles));

        }

        /// <summary>
        /// Remove the user from the role.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            IdentityRole role = await roleManager.FindByNameAsync(roleName);
            
            if (role != null)
            {
                ApplicationUser user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    // Before removing the user from the role, check that they are in fact in the role.
                    bool isInRole = await userManager.IsInRoleAsync(user, roleName);
                    if (isInRole)
                    {
                        await userManager.RemoveFromRoleAsync(user, roleName);
                        logger.LogInformation($"Removed {user.Id} from role {roleName}");
                    }
                }
            }

            return RedirectToAction(nameof(UsersInRole));
        }
    }
}

