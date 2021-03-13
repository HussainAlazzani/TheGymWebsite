using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using TheGymWebsite.Models;
using TheGymWebsite.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using TheGymWebsite.Models.Repository;
using Microsoft.Extensions.Logging;

namespace TheGymWebsite.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        // To manage users in Identity system.
        private readonly UserManager<ApplicationUser> userManager;
        // To perform the authentication.
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IAuthorizationService authorizationService;
        private readonly IEmailService emailService;
        private readonly Email email;
        private readonly IWebHostEnvironment env;
        private readonly IMembershipDealRepository dealRepository;
        private readonly ILogger<AccountController> logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IAuthorizationService authorizationService,
            IEmailService emailService,
            IOptions<Email> email,
            IWebHostEnvironment env,
            IMembershipDealRepository dealRepository,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.authorizationService = authorizationService;
            this.emailService = emailService;
            this.email = email.Value;
            this.env = env;
            this.dealRepository = dealRepository;
            this.logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]      // Prevent cross-site request forgery.
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email is not found");
                return View(model);
            }

            // Signing out any logged in user.
            await signInManager.SignOutAsync();
            // This logs in the user by providing the user object and the password entered.
            // The 4th param will enable the login lockout feature as configured in the Startup.cs
            var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

            if (result.Succeeded)
            {
                // The user is redirected to their previous page if they have been redirected.
                // The return address is checked to make sure it is a local url.
                if (string.IsNullOrEmpty(returnUrl) == false && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Member");
            }
            if (await userManager.CheckPasswordAsync(user, model.Password) == false)
            {
                // When the user enters an invalid password.
                ModelState.AddModelError(nameof(model.Password), "Invalid password");
                return View(model);
            }
            if (result.RequiresTwoFactor == false)
            {
                // When the user's email has not been verified
                ModelState.AddModelError(nameof(model.Email), "Your email has not been verified");
                return View(model);
            }
            if (result.IsLockedOut)
            {
                // When the user account has been locked for reaching the maximum number of attempts.
                // The account will be unlocked when the lockout period expires.
                return RedirectToAction("AccountLocked", "Error");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Log out the user.
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Users who click on the forgot password link are directed here. They must submit their email .
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find the user by email
            var user = await userManager.FindByEmailAsync(model.Email);
            // If the user is found AND Email is confirmed
            if (user != null && await userManager.IsEmailConfirmedAsync(user))
            {
                // Generate the reset password token
                string token = await userManager.GeneratePasswordResetTokenAsync(user);

                // Build the password reset link
                string passwordResetLink = Url.Action("ResetPassword", "Account",
                        new { email = model.Email, token = token }, Request.Scheme);

                // An email is sent to the user with the confirmation link to verify their email.
                email.Name = user.FirstName + " " + user.LastName;
                if (env.IsProduction())
                {
                    email.Address = user.Email;
                }
                email.Subject = "Reset password request";
                email.Message = $"You have made a request to reset your password. Please click on the link below if you still wish to reset your password.\n{passwordResetLink}\n";

                emailService.Send(email);

                // Send the user to Forgot Password Confirmation view
                return View("ForgotPasswordConfirmation");
            }

            // To avoid account enumeration and brute force attacks, don't
            // reveal that the user does not exist or is not confirmed
            return View("ForgotPasswordConfirmation");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Error in the form");
                return View(model);
            }

            // Find the user by email
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }

            // reset the user password
            IdentityResult result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors. For example, password reset token already
            // used to change the password or password complexity rules not met
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterMember()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterMember(RegisterMemberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            DateTime date;

            try
            {
                // Possible exception. The date selected by the client may not correctly represent calender date. Eg. 31st February.
                date = new DateTime(model.YearOfBirth, model.MonthOfBirth, model.DayOfBirth);
            }
            catch (ArgumentOutOfRangeException)
            {
                logger.LogError("Invalid date entered for date of birth on registration");

                // If the user selects an invalid date such as 31st February, the user is given an error message.
                ModelState.AddModelError("DayOfBirth", "Your date of birth is invalid");
                return View(model);
            }

            // Copy data from RegisterMemberViewModel to ApplicationUser to be stored in the database.
            ApplicationUser user = new ApplicationUser
            {
                UserName = model.Email,
                NormalizedUserName = model.Email,
                Email = model.Email,
                NormalizedEmail = model.Email,

                Title = model.Title,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = date,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,

                AddressLineOne = model.AddressLineOne,
                AddressLineTwo = model.AddressLineTwo,
                Town = model.Town,
                Postcode = model.Postcode
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Adding the following claims to new users.
                IEnumerable<Claim> claims = new List<Claim>
                    {
                        new Claim("DateOfBirth", user.DateOfBirth.ToShortDateString()),
                        new Claim("RegistrationDate", DateTime.Now.ToShortDateString())
                    };

                await userManager.AddClaimsAsync(user, claims);

                // A token is generated which will be used to verify the email of the user.
                string token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                // A confirmation link is created using the id of the user and the token.
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                // An email is sent to the user with the confirmation link to verify their email.
                email.Name = user.FirstName + " " + user.LastName;
                if (env.IsProduction())
                {
                    email.Address = user.Email;
                }
                email.Subject = "verify your email";
                email.Message = $"Hello {model.FirstName} {model.LastName}.\nThank your for registering with us. Please click on the link below to vaidate your email. The link expires after 1 day.\n{confirmationLink}\n";

                emailService.Send(email);

                return RedirectToAction("RequestToConfirmEmail");
            }

            // When creating the user fails, collect the errors and rerender the view
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        /// <summary>
        /// Gives message to the user to confirm their email by clicking the
        /// verification link that is sent to their email.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult RequestToConfirmEmail()
        {
            return View();
        }

        /// <summary>
        /// Pressing the verification link will bring the user to this action method.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ApplicationUser user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("UserNotFound", "Error", new { idOrEmail = userId });
            }

            // The email is confirmed if the token corresponds to the right user.
            IdentityResult result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // If email is successfully confirmed, sign the user in and direct them to membership payment options.
                await signInManager.SignInAsync(user, isPersistent: false);

                // If user is below 16 years, direct them to age-restriction page.
                if ((CalculateAge.GetAge(user.DateOfBirth)) < 16)
                {
                    return RedirectToAction("AgeRestriction", "Account", new { userName = user.FirstName + " " + user.LastName });
                }

                return RedirectToAction("Index", "Payment");
            }
            else
            {
                // Invalid confirmation links are rejected.
                return RedirectToAction("EmailConfirmationError", "Error");
            }
        }

        /// <summary>
        /// Users below the age of 16 are not allowed to purchase memberships.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult AgeRestriction(string userName)
        {
            ViewBag.UserName = userName;
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditUserDetails(string email, string returnUrl)
        {
            // To edit user details, the following conditions must be satisfied:
            // 1. An email must be provided. A link without an email query string is set to the clients email.
            // 2. Clients must be authorised. Anonymous clients are denied access.
            // 3. Clients must have permission to manage other users. Clients without this permission are redirected to their own edit page.
            if (email == null || (await authorizationService.AuthorizeAsync(User, "ManageUsersPermission")).Succeeded == false)
            {
                // If query string is null or client doesn't have permission to manage users, direct them to their own edit page.
                email = User.Identity.Name;
            }

            ApplicationUser user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return RedirectToAction("UserList", "Admin");
            }

            EditUserDetailsViewModel model = new EditUserDetailsViewModel
            {
                Email = user.Email,
                Title = user.Title,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DayOfBirth = user.DateOfBirth.Day,
                MonthOfBirth = user.DateOfBirth.Month,
                YearOfBirth = user.DateOfBirth.Year,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                AddressLineOne = user.AddressLineOne,
                AddressLineTwo = user.AddressLineTwo,
                Town = user.Town,
                Postcode = user.Postcode,
            };

            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserDetails(EditUserDetailsViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                DateTime date;

                try
                {
                    // Possible exception. The date selected by the client may not correctly represent calender date. Eg. 31st February.
                    date = new DateTime(model.YearOfBirth, model.MonthOfBirth, model.DayOfBirth);
                }
                catch (Exception)
                {
                    logger.LogError($"Invalid date of birth entered when editiing user details");

                    // If the user selects an invalid date such as 31st February, the user is given an error message.
                    ModelState.AddModelError("DayOfBirth", "Your date of birth is invalid");
                    return View(model);
                }

                // Retrieve user details.
                ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Copy user details from the submitted form.
                    user.Email = model.Email;
                    user.Title = model.Title;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.DateOfBirth = date;
                    user.Gender = model.Gender;
                    user.PhoneNumber = model.PhoneNumber;
                    user.AddressLineOne = model.AddressLineOne;
                    user.AddressLineTwo = model.AddressLineTwo;
                    user.Town = model.Town;
                    user.Postcode = model.Postcode;

                    // Update user details.
                    IdentityResult result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        // Update the Date of Birth claim of the user to reflect the changes made to their date of birth.
                        Claim oldDoBClaim = (await userManager.GetClaimsAsync(user)).Where(c => c.Type == "DateOfBirth").FirstOrDefault();
                        Claim newDoBClaim = new Claim("DateOfBirth", user.DateOfBirth.ToShortDateString());
                        await userManager.ReplaceClaimAsync(user, oldDoBClaim, newDoBClaim);

                        // Redirect the user back to their previous page if applicable.
                        if (string.IsNullOrEmpty(returnUrl) == false && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("UserList", "Admin");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditUserPassword(string email)
        {
            // A user can only edit their own password.
            // If the user's email does not match the email in the link, they are not granted access.
            if (email == User.Identity.Name)
            {
                return View(new EditUserPasswordViewModel { Email = email });
            }

            return RedirectToAction("AccessDenied", "Error");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditUserPassword(EditUserPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the user by their email.
                ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
                // Change the users password.
                IdentityResult result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    // Refresh sign-in cookie if password is successfully changed.
                    await signInManager.RefreshSignInAsync(user);
                    return RedirectToAction("Index", "Member");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("Password", "Error with password");
            }
            return View(model);
        }

        /// <summary>
        /// Deleting users is only performed via a post method to guard against malicious links.
        /// </summary>
        [Authorize(Policy = "ManageUsersPermission")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                // If user exists in the database then delete their record.
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded == false)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return RedirectToAction("UserList", "Admin");
        }

        [Authorize(Policy = "IssueBansPermission")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BanUser(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Check if user is already banned. If they are banned, do not proceed to ban them.
                Claim claim = (await userManager.GetClaimsAsync(user)).Where(c => c.Type == "Banned").FirstOrDefault();
                if (claim == null)
                {
                    // Ban user by adding a "banned" claim against their record.
                    IdentityResult result = await userManager.AddClaimAsync(user, new Claim("Banned", DateTime.Now.ToShortDateString()));
                    if (result.Succeeded)
                    {
                        // Send an email to the user to notify them of the decision to ban them.
                        email.Name = user.FirstName + " " + user.LastName;
                        if (env.IsProduction())
                        {
                            email.Address = user.Email;
                        }
                        email.Subject = "Ban imposed";
                        email.Message = $"Hello {user.FirstName} {user.LastName}." +
                            $"\nA decision has been made to ban you from our gym. You no longer have access to our facilities.";

                        emailService.Send(email);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User is already banned");
                }
            }

            return RedirectToAction("UserProfile", "Admin", new { email = user.Email });
        }

        [Authorize(Policy = "IssueBansPermission")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBan(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Retrieve the user's ban.
                Claim claim = (await userManager.GetClaimsAsync(user)).Where(c => c.Type == "Banned").FirstOrDefault();
                // If the user is banned
                if (claim != null)
                {
                    // Lift the ban.
                    IdentityResult result = await userManager.RemoveClaimAsync(user, claim);
                    if (result.Succeeded)
                    {
                        // Sending an email to the user to notify them of the decision to lift their ban.
                        email.Name = user.FirstName + " " + user.LastName;
                        if (env.IsProduction())
                        {
                            email.Address = user.Email;
                        }
                        email.Subject = "Ban lifted";
                        email.Message = $"Congratulations, {user.FirstName} {user.LastName}." +
                            $"\nYour ban has been lifted. You may now access our facilities.";

                        emailService.Send(email);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            return RedirectToAction("UserProfile", "Admin", new { email = user.Email });
        }

        [Authorize(Policy = "ManageRolesPermission")]
        [HttpGet]
        public async Task<IActionResult> EditUserRoles(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ModelState.AddModelError("", $"User selected is not found");
                return RedirectToAction("UserList", "Admin");
            }

            EditUserRolesViewModel model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.FirstName + " " + user.LastName,
                Email = user.Email,
            };

            // Retrieve all the roles in the system.
            IEnumerable<IdentityRole> roles = roleManager.Roles;

            // Sort the roles by name then Loop through each role 
            foreach (var role in roles.OrderBy(x => x.Name).ToList())
            {
                model.Roles.Add(role.Name);
                // Check if the user is in the role.
                model.IsInRole.Add(await userManager.IsInRoleAsync(user, role.Name));
            }

            return View(model);
        }

        [Authorize(Policy = "ManageRolesPermission")]
        [HttpPost]
        public async Task<IActionResult> AddUserToRole(EditUserRolesViewModel model)
        {
            ApplicationUser user = await userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                // Add user to the selected role.
                var result = userManager.AddToRoleAsync(user, model.RoleName);
                if (result.Result.Succeeded == false)
                {
                    ModelState.AddModelError("", $"Could not add user to role {model.RoleName}");
                }
            }

            // Redirect the user back to the edit role page to continue editing the user's roles.
            return RedirectToAction(nameof(EditUserRoles), "Account", routeValues: new { id = model.UserId });
        }

        [Authorize(Policy = "ManageRolesPermission")]
        [HttpPost]
        public async Task<IActionResult> RemoveUserFromRole(EditUserRolesViewModel model)
        {
            ApplicationUser user = await userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                // Remove user from the selected role.
                var result = userManager.RemoveFromRoleAsync(user, model.RoleName);
                if (result.Result.Succeeded == false)
                {
                    ModelState.AddModelError("", $"Could not remove user from role {model.RoleName}");
                }
            }

            // Redirect the user back to the edit role page to continue editing the user's roles.
            return RedirectToAction(nameof(EditUserRoles), "Account", routeValues: new { id = model.UserId });
        }
    }
}
