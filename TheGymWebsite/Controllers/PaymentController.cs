using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Stripe;
using TheGymWebsite.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TheGymWebsite.Models.Repository;

namespace TheGymWebsite.Controllers
{

    [Authorize(Policy = "MinimumAge16")]
    public class PaymentController : Controller
    {
        private readonly IMembershipDealRepository dealRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public PaymentController(IMembershipDealRepository dealRepository, UserManager<ApplicationUser> userManager)
        {
            this.dealRepository = dealRepository;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            IEnumerable<MembershipDeal> membershipDeals = dealRepository.GetMembershipDeals();

            return View(membershipDeals);
        }

        [HttpPost]
        public async Task<IActionResult> Processing(string stripeToken, string stripeEmail, int amount, Enums.MembershipDuration membershipDuration)
        {

            ApplicationUser user = await userManager.FindByEmailAsync(User.Identity.Name);

            // Checking if user has membership.
            Claim claim = (await userManager.GetClaimsAsync(user)).Where(c => c.Type == "MembershipExpiry")
                .OrderByDescending(c => c.Value)
                .FirstOrDefault();

            // If the user was previously granted membership, then do not issue a new one if the membership is still valid.
            if (claim != null && Convert.ToDateTime(claim.Value) > DateTime.Today)
            {
                return RedirectToAction("AlreadyAMember", "Error", routeValues: new { expiry = Convert.ToDateTime(claim.Value) });
            }

            // This object contains information about the charge we will create.
            ChargeCreateOptions options = new ChargeCreateOptions
            {
                Amount = amount,
                Currency = "GBP",
                Description = Enums.GetDisplayName(membershipDuration) + " membership",
                Source = stripeToken,
                ReceiptEmail = stripeEmail // This sends email receipt to the customer based on the entered email.

                //// Metadata can be used to store more non-sensitive information for our own reference.
                //// Stripe does not use this information.
                //Metadata = new Dictionary<String, String>{ { "OrderId", "..." } }
            };

            // This creates the charge in which we pass in the information. The status of the charge will be pending until
            // Stripe processes it and returns a status of either succeeded of failed.
            ChargeService service = new ChargeService();
            // The charge request is made here. It connects to Stripe servers and processes the transaction.
            Charge charge = service.Create(options);

            // Checking the status of the charge. Succeeded or failed or Pending.
            try
            {
                if (charge.Status == "succeeded")
                {
                    // Create new membership for the user.
                    DateTime expiry = MembershipExpiry.SetExpiryDate(membershipDuration);
                    IdentityResult result = await userManager.AddClaimAsync(user, new Claim("MembershipExpiry", expiry.ToShortDateString()));

                    ViewBag.FirstName = user.FirstName;
                    ViewBag.LastName = user.LastName;
                    ViewBag.ExpiryDate = expiry.ToShortDateString();

                    return View(nameof(Success));
                }
                if (charge.Status == "failed")
                {
                    return View(nameof(Fail));
                }
            }
            catch (Exception)
            {
                return RedirectToAction("PaymentError", "Error");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Fail()
        {
            return View();
        }
    }
}
