using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using TheGymWebsite.Models;
using System.Security.Claims;
using TheGymWebsite.ViewModels;

namespace TheGymWebsite.Controllers
{
    [Authorize(Policy = "MinimumAge16")]
    public class MemberController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAttendanceRepository attendanceRepository;

        public MemberController(UserManager<ApplicationUser> userManager, IAttendanceRepository attendanceRepository)
        {
            this.userManager = userManager;
            this.attendanceRepository = attendanceRepository;
        }

        /// <summary>
        /// This action method displays the members page. It displays the name of the user, whether their membership is active or not
        /// their membership expiry date if applicable and a chart that represents their attendance over the last month.
        /// The membership page is personal and can only be viewed by the user themselves.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Find the user by their identity.
            ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);
            MemberViewModel model = new MemberViewModel
            {
                FullName = user.FirstName + " " + user.LastName
            };

            // Checking user's membership status and expiry.
            Claim claim = (await userManager.GetClaimsAsync(user)).Where(c => c.Type == "MembershipExpiry")
                .OrderByDescending(c => c.Value)
                .FirstOrDefault();

            // This will inform the user on the status of their membership.
            if (claim == null || Convert.ToDateTime(claim.Value) < DateTime.Today)
            {
                model.MembershipStatus = "Inactive";
            }
            else
            {
                model.MembershipStatus = "Active";
                model.MembershipExpiration = Convert.ToDateTime(claim.Value).ToShortDateString();
            }

            // Retrieve attendance record of the user over the past month.
            DateTime pastMonth = DateTime.Now.AddMonths(-1);
            List<DateTime> dateTimes = attendanceRepository.GetAttendanceFromDate(user.Id, pastMonth).ToList();

            // For each day of the past month, check if the user attended the gym. This will be plotted on a bar chart.
            int index = 0;
            while (pastMonth <= DateTime.Now)
            {
                int userAttended = 0;
                if (dateTimes.Count > 0 && pastMonth.ToShortDateString() == dateTimes[index].ToShortDateString())
                {
                    userAttended = 1;
                    if (index < dateTimes.Count - 1)
                    {
                        index++;
                    }
                }

                // Data for the bar chart representing the user's attendance over the past month.
                // The x-axis represents each day of the past month, and the y-axis indicate whether the user attended or not.
                model.Chart.Add(pastMonth.ToShortDateString().Substring(0, 5), userAttended);
                // Shift to the next day.
                pastMonth = pastMonth.AddDays(1);
            }

            return View(model);
        }

        /// <summary>
        /// This action method simply displays the user's details. This client user can only view their own details.
        /// </summary>
        public async Task<IActionResult> MemberDetails()
        {
            ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);
            return View(user);
        }
    }
}
