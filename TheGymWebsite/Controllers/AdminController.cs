using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheGymWebsite.Models;
using TheGymWebsite.Models.Repository;
using TheGymWebsite.ViewModels;

namespace TheGymWebsite.Controllers
{
    [Authorize(Policy = "CanViewUsers")]
    public class AdminController : Controller
    {
        private readonly IGymRepository gymRepository;
        private readonly IOpenHoursRepository openHoursRepository;
        private readonly IVacancyRepository vacancyRepository;
        private readonly IMembershipDealRepository dealRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IAttendanceRepository attendanceRepository;
        private readonly ILogger<AdminController> logger;

        public AdminController(
            IGymRepository gymRepository,
            IOpenHoursRepository openHoursRepository,
            IVacancyRepository vacancyRepository,
            IMembershipDealRepository dealRepository,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAttendanceRepository attendanceRepository,
            ILogger<AdminController> logger)
        {
            this.gymRepository = gymRepository;
            this.openHoursRepository = openHoursRepository;
            this.vacancyRepository = vacancyRepository;
            this.dealRepository = dealRepository;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.attendanceRepository = attendanceRepository;
            this.logger = logger;
        }

        /// <summary>
        /// This action method retrieves all users in the system. The view renders the list in a table that can be searched and filtered.
        /// </summary>
        [Route("Admin")]
        [Route("Admin/UserList")]
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            IEnumerable<ApplicationUser> users = userManager.Users;
            IList<UserListViewModel> models = new List<UserListViewModel>();

            // I applied a ToList() method for immediate execution so that GetRolesAsync(user) can run without throwing exception.
            // The exception reads "There is already an open DataReader associated with this Command which must be closed first."
            // This occurs when executing a query while iterating over the results from another query. In this case, I am accessing 
            // the roles via GetClaimsAsync(user) inside a foreach loop which is also reading data from users object.
            // Alternatively, I can set MARS (MultipleActiveResultSets=true;) in the connection string but this is not thread-safe.
            foreach (var user in users.ToList())
            {
                UserListViewModel model = new UserListViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                };

                // Checking user's membership status and expiry to be displayed in the view.
                // Claim type is the membership. The value is its expiry date.
                Claim membershipClaim = (await userManager.GetClaimsAsync(user))
                    .Where(c => c.Type == "MembershipExpiry")
                    .OrderByDescending(c => c.Value)
                    .FirstOrDefault();

                // Expired memberships are flagged as false. Non expired memberships are flagged as true. 
                if (membershipClaim == null || Convert.ToDateTime(membershipClaim.Value) < DateTime.Today)
                {
                    model.IsMembershipActive = false;
                }
                else
                {
                    model.IsMembershipActive = true;
                    model.MembershipExpiration = Convert.ToDateTime(membershipClaim.Value).ToShortDateString();
                }

                models.Add(model);
            }

            return View(models);
        }

        /// <summary>
        /// This post action method registers the gym member's attendance. When the  gym member attends the gym, they gym staff or admin
        /// signs them in by clicking the "sign in member". This feature is only enabled for members with active memberships.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GymSignIn(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction("UserNotFound", "Error", new { userId = id });
            }

            // Checking if user is banned. Banned users cannot be signed in to the gym.
            Claim bannedClaim = (await userManager.GetClaimsAsync(user))
                .Where(c => c.Type == "Banned")
                .FirstOrDefault();

            if (bannedClaim != null)
            {
                return RedirectToAction("BannedUser", "Error", new { email = user.Email, name = user.FirstName + " " + user.LastName });
            }

            // Sign the user in to the gym by registering the date and time they attended against their id.
            attendanceRepository.Add(new GymAttendance
            {
                Date = DateTime.Now,
                UserId = user.Id
            });

            return RedirectToAction(nameof(UserList));

        }

        /// <summary>
        /// This action method retrieves the details of a specific user by their email. The view displays the main details
        /// of the user along with their roles and claims.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> UserProfile(string email)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return RedirectToAction("UserNotFound", "Error", new { idOrEmail = email });
            }

            UserProfileViewModel model = new UserProfileViewModel
            {
                Id = user.Id,
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

                Roles = await userManager.GetRolesAsync(user)
            };

            IEnumerable<Claim> claims = await userManager.GetClaimsAsync(user);

            // Adding user claims to the model to be displayed while also checking if the user is an employee and is banned.
            // Knowing these two states will determine whether we can add or remove the user from the employee register, and
            // whether we can ban or lift the ban from the user.
            foreach (var claim in claims)
            {
                model.Claims.Add(new UserClaimViewModel { ClaimType = claim.Type, ClaimValue = claim.Value });

                // Banned users and employees are flagged as true. See previous comment.
                if (claim.Type == "Banned")
                {
                    model.IsBanned = true;
                }
                if (claim.Type == "Employee")
                {
                    model.IsEmployee = true;
                }
            }

            return View(model);
        }

        /// <summary>
        /// This post method adds users to the employee register by attaching the claim "Employee" to them. All employees are awarded
        /// free memberships as part of the company benefits. This handled by attaching a membership claim to them.
        /// </summary>
        [Authorize(Policy = "ManageUsersPermission")]
        [HttpPost]
        public async Task<IActionResult> AddEmployee(string id, string returnUrl)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Checking if the user is already an employee. Users with an Employee claim are already employees.
                Claim claim = (await userManager.GetClaimsAsync(user)).Where(c => c.Type == "Employee").FirstOrDefault();
                // If the user is not already an employee.
                if (claim == null)
                {
                    // Add employee claim to the user.
                    IdentityResult result = await userManager.AddClaimAsync(user, new Claim("Employee", DateTime.Now.ToShortDateString()));
                    if (result.Succeeded)
                    {
                        // If user is successfully added as employee, they automatically receive unlimited gym membership.
                        await userManager.AddClaimAsync(user, new Claim("MembershipExpiry", DateTime.MaxValue.ToShortDateString()));
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
                    ModelState.AddModelError("", "User is already an employee");
                }
            }

            // Users are redirected back to their previous page if the returnURL contains a valid url.
            if (string.IsNullOrEmpty(returnUrl) == false && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("UserList", "Admin");
        }

        [Authorize(Policy = "ManageUsersPermission")]
        [HttpPost]
        public async Task<IActionResult> RemoveEmployee(string id, string returnUrl)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Get the user's claim to employement and their entitlement to unlimited membership
                IEnumerable<Claim> claim = (await userManager.GetClaimsAsync(user))
                    .Where(c => c.Type == "Employee" ||
                        (c.Type == "MembershipExpiry" && c.Value == DateTime.MaxValue.ToShortDateString()));

                if (claim != null)
                {
                    // Remove the user's employee status and their unlimited membership entitlement.
                    // Previously paid memberships are unaffected.
                    IdentityResult result = await userManager.RemoveClaimsAsync(user, claim);
                    if (result.Succeeded == false)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            // Users are redirected back to their previous page if the returnURL contains a valid url.
            if (string.IsNullOrEmpty(returnUrl) == false && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("UserList", "Admin");
        }


        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpGet]
        public IActionResult AddGym()
        {
            // Passed in a new GymViewModel object to process the Gym Name which is initialised to "The Gym".
            return View(new GymViewModel());
        }

        /// <summary>
        /// This action method adds the new gym to the database.
        /// </summary>
        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        public IActionResult AddGym(GymViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Gym gym = new Gym
            {
                GymName = model.GymName,
                Email = model.Email,
                AddressLineOne = model.AddressLineOne,
                AddressLineTwo = model.AddressLineTwo,
                Town = model.Town,
                Postcode = model.Postcode,
                Telephone = model.Telephone
            };

            gymRepository.Add(gym);

            return RedirectToAction(nameof(ListGyms));

        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpGet]
        public IActionResult EditGym(int id)
        {
            // Retrieve the gym from the database to be edited by the user.
            Gym gym = gymRepository.GetGym(id);

            if (gym == null)
            {
                return RedirectToAction(nameof(ListGyms));
            }

            GymViewModel model = new GymViewModel
            {
                Id = gym.Id,
                GymName = gym.GymName,
                Email = gym.Email,
                AddressLineOne = gym.AddressLineOne,
                AddressLineTwo = gym.AddressLineTwo,
                Town = gym.Town,
                Postcode = gym.Postcode,
                Telephone = gym.Telephone
            };

            return View(model);
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        public IActionResult EditGym(GymViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Error in the form");
                return View(model);
            }

            Gym gym = gymRepository.GetGym(model.Id);
            gym.Id = model.Id;
            gym.GymName = model.GymName;
            gym.Email = model.Email;
            gym.AddressLineOne = model.AddressLineOne;
            gym.AddressLineTwo = model.AddressLineTwo;
            gym.Town = model.Town;
            gym.Postcode = model.Postcode;
            gym.Telephone = model.Telephone;

            gymRepository.Update(gym);

            return RedirectToAction(nameof(ListGyms));
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        public IActionResult DeleteGym(int id)
        {
            gymRepository.Delete(id);
            return RedirectToAction(nameof(ListGyms));
        }

        [HttpGet]
        public IActionResult ListGyms()
        {
            IEnumerable<Gym> gyms = gymRepository.GetGyms();
            return View(gyms);
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpGet]
        public IActionResult AddMembershipDeal()
        {
            return View();
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        public IActionResult AddMembershipDeal(MembershipDealViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Error found in the form");
                return View(model);
            }

            // The membership duration cannot be duplicated, hence, checking if the duration already exists.
            if (dealRepository.IsDurationOffered(model.Duration))
            {
                ModelState.AddModelError("Duration", "This duration is already offered.");
                return View(model);
            }

            MembershipDeal membershipDeal = new MembershipDeal
            {
                Duration = model.Duration,
                Price = model.Price
            };

            // Add new membership plan to the database. This will be displayed on the website.
            dealRepository.Add(membershipDeal);

            return RedirectToAction("MembershipDeals", "Home");
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpGet]
        public IActionResult EditMembershipDeal(int id)
        {
            // Retrieve the membership to be edited.
            MembershipDeal membershipDeal = dealRepository.GetMembershipDeal(id);

            // If membership does not exists
            if (membershipDeal == null)
            {
                return RedirectToAction("MembershipDeals", "Home");
            }

            MembershipDealViewModel model = new MembershipDealViewModel
            {
                Id = membershipDeal.Id,
                Duration = membershipDeal.Duration,
                Price = membershipDeal.Price
            };

            return View(model);
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        public IActionResult EditMembershipDeal(MembershipDealViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            MembershipDeal membershipDeal = dealRepository.GetMembershipDeal(model.Id);
            // Copy the information from the form to the database.
            membershipDeal.Price = model.Price;
            dealRepository.Update(membershipDeal);

            return RedirectToAction("MembershipDeals", "Home");
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        public IActionResult DeleteMembershipDeal(int id)
        {
            // Delete the chosen membership. The repository checks for the existence of the membership.
            dealRepository.Delete(id);

            return RedirectToAction("MembershipDeals", "Home");
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpGet]
        public IActionResult AddBankHoliday()
        {
            return View();
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        public IActionResult AddBankHoliday(OpenHoursViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Possible exception. The date selected by the client may not correctly represent calender date. Eg. 31st February.
                DateTime date = new DateTime(model.Year, model.Month, model.Day);

                OpenHours openHours = new OpenHours
                {
                    Date = date,
                    DayName = date.DayOfWeek,
                    OpenTime = model.OpenTime,
                    CloseTime = model.CloseTime,
                    Note = model.Note
                };

                openHoursRepository.Add(openHours);

                return RedirectToAction("OpenHours", "Home");
            }
            catch (ArgumentOutOfRangeException)
            {
                logger.LogError("Invalid date entered for bank holiday");


                // If the user selects an invalid date such as 31st February, the user is given an error message.
                ModelState.AddModelError("Date", "The date you selected is invalid");
                return View(model);
            }
        }

        /// <summary>
        /// This action method retrieves all the open hours and holidays information to be edited in one single page.
        /// Each day is displayed in a table row.
        /// </summary>
        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpGet]
        public IActionResult EditOpenHours()
        {
            IEnumerable<OpenHours> openHours = openHoursRepository.GetOpenHours();

            List<OpenHoursViewModel> models = new List<OpenHoursViewModel>();

            foreach (var item in openHours)
            {
                models.Add(new OpenHoursViewModel
                {
                    Id = item.Id,
                    Day = item.Date.Day,
                    Month = item.Date.Month,
                    Year = item.Date.Year,
                    DayName = item.DayName,
                    OpenTime = item.OpenTime,
                    CloseTime = item.CloseTime,
                    Note = item.Note
                });
            }

            return View(models);
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        public IActionResult EditOpenHours(List<OpenHoursViewModel> models)
        {
            if (!ModelState.IsValid)
            {
                return View(models);
            }

            foreach (var day in models)
            {
                OpenHours openHours = openHoursRepository.GetOpenHours(day.Id);

                try
                {
                    // Days beyond the 7 days are holidays, hence, they includes fields for date and day name.
                    if (day.Id > 7)
                    {
                        // Possible exception. The date selected by the client may not correctly represent calender date. Eg. 31st February.
                        DateTime date = new DateTime(day.Year, day.Month, day.Day);
                        openHours.Date = date;
                        openHours.DayName = date.DayOfWeek;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    logger.LogError("Invalid date entered for open hours.");

                    // If the user selects an invalid date such as 31st February, the user is given an error message.
                    ModelState.AddModelError("Date", "You have selected an invalid date.");
                    return View(models);
                }

                // Copy the form data to the object to be stored in the database.
                openHours.OpenTime = day.OpenTime;
                openHours.CloseTime = day.CloseTime;
                openHours.Note = day.Note;

                openHoursRepository.Update(openHours);
            }
            return RedirectToAction("OpenHours", "Home");

        }

        /// <summary>
        /// This action method allows holidays to be deleted. The regular 7 days (from monday to sunday) cannot be deleted from the system.
        /// The user selects the holidays to be deleted by clicking the checkbox.
        /// </summary>
        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpGet]
        public IActionResult DeleteBankHoliday()
        {
            List<OpenHours> openHours = openHoursRepository.GetOpenHours().ToList();
            List<OpenHoursViewModel> models = new List<OpenHoursViewModel>();

            // Loop through the holdays which begin from 7.
            for (int i = 7; i < openHours.Count; i++)
            {
                models.Add(new OpenHoursViewModel
                {
                    Id = openHours[i].Id,
                    Date = openHours[i].Date,
                    DayName = openHours[i].DayName,
                    OpenTime = openHours[i].OpenTime,
                    CloseTime = openHours[i].CloseTime,
                    Note = openHours[i].Note
                });
            }

            return View(models);
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBankHoliday(List<OpenHoursViewModel> models)
        {
            if (!ModelState.IsValid)
            {
                return View(models);
            }

            foreach (var day in models)
            {
                // Checking Id value is just a precautionary measure but otherwise unnecessary since only the days beyond 7 are renedered.
                if (day.Id > 7 && day.IsChecked)
                {
                    openHoursRepository.Delete(day.Id);
                }
            }

            return RedirectToAction("OpenHours", "Home");
        }

        /// <summary>
        /// The user can add a new vacancy to the system.
        /// </summary
        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpGet]
        public IActionResult AddVacancy()
        {
            return View();
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        public IActionResult AddVacancy(VacancyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Copy the information to the vacancy object to be stored in the database.
            Vacancy vacancy = new Vacancy
            {
                JobTitle = model.JobTitle,
                JobType = model.JobType,
                JobPeriod = model.JobPeriod,
                Salary = model.Salary,
                PayInterval = model.PayInterval,
                Description = model.Description
            };

            vacancyRepository.Add(vacancy);

            return RedirectToAction("Vacancies", "Home");
        }

        /// <summary>
        /// Vacancies can be edited by the user.
        /// </summary>
        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpGet]
        public IActionResult EditVacancy(int id)
        {
            Vacancy vacancy = vacancyRepository.GetVacancy(id);

            if (vacancy == null)
            {
                return RedirectToAction("Vacancies", "Home");
            }

            VacancyViewModel model = new VacancyViewModel
            {
                Id = vacancy.Id,
                JobTitle = vacancy.JobTitle,
                JobType = vacancy.JobType,
                JobPeriod = vacancy.JobPeriod,
                Salary = vacancy.Salary,
                PayInterval = vacancy.PayInterval,
                Description = vacancy.Description
            };

            return View(model);
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        public IActionResult EditVacancy(VacancyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Vacancy vacancy = new Vacancy
            {
                Id = model.Id,
                JobTitle = model.JobTitle,
                JobType = model.JobType,
                JobPeriod = model.JobPeriod,
                Salary = model.Salary,
                PayInterval = model.PayInterval,
                Description = model.Description,
            };

            vacancyRepository.Update(vacancy);

            return RedirectToAction("Vacancies", "Home");
        }

        [Authorize(Policy = "ManageBusinessPermission")]
        [HttpPost]
        public IActionResult DeleteVacancy(int id)
        {
            vacancyRepository.Delete(id);
            return RedirectToAction("Vacancies", "Home");
        }
    }
}
