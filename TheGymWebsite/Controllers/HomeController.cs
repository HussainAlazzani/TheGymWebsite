using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using TheGymWebsite.Models;
using TheGymWebsite.Models.Repository;
using TheGymWebsite.ViewModels;

namespace TheGymWebsite.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IGymRepository gymRepository;
        private readonly IOpenHoursRepository openHoursRepository;
        private readonly IVacancyRepository vacancyRepository;
        private readonly IFreePassRepository freePassRepository;
        private readonly IMembershipDealRepository dealRepository;
        private readonly IWebHostEnvironment env;
        private readonly IEmailService emailService;
        private readonly Email email;

        public HomeController(
            IGymRepository gymRepository,
            IOpenHoursRepository openHoursRepository,
            IVacancyRepository vacancyRepository,
            IFreePassRepository freePassRepository,
            IMembershipDealRepository dealRepository,
            IWebHostEnvironment env,
            IEmailService emailService,
            IOptions<Email> email)
        {
            this.gymRepository = gymRepository;
            this.openHoursRepository = openHoursRepository;
            this.vacancyRepository = vacancyRepository;
            this.freePassRepository = freePassRepository;
            this.dealRepository = dealRepository;
            this.env = env;
            this.emailService = emailService;
            this.email = email.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OpenHours()
        {
            List<OpenHours> openHours = openHoursRepository.GetOpenHours().ToList();

            return View(openHours);
        }

        [RequireHttps]
        [HttpGet]
        public IActionResult FreePass()
        {
            return View();
        }

        [HttpPost]
        [RequireHttps]
        public IActionResult FreePass(FreePassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Adding entry to database.
            FreePass freePass = new FreePass
            {
                Name = model.Name,
                Email = model.Email,
                DateIssued = model.DateIssued,
                DateUsed = null
            };
            freePassRepository.Add(freePass);

            // Getting the Id of the free pass. This will be sent to the user as the pass code.
            int freePassId = freePassRepository.GetFreePassIdFromEmail(freePass.Email);

            email.Name = freePass.Name;
            if (env.IsProduction())
            {
                email.Address = model.Email;
            }
            email.Subject = "Your Day Pass number";
            email.Message = $"Hello {freePass.Name}. Your pass number is: {freePassId}.";
            emailService.Send(email);

            return RedirectToAction(nameof(ConfirmFreePass), "Home", model.Name);
        }

        [HttpGet]
        public IActionResult ConfirmFreePass(string name)
        {
            ViewBag.Name = name;

            return View();
        }

        [HttpGet]
        public IActionResult MembershipDeals()
        {
            IEnumerable<MembershipDeal> membershipDeals = dealRepository.GetMembershipDeals();

            return View(membershipDeals);
        }

        [HttpGet]
        public IActionResult Vacancies()
        {
            IEnumerable<Vacancy> vacancies = vacancyRepository.GetVacancies();

            return View(vacancies);
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            // Hard-coding the id value 1 because this application is only serving one gym at the initial stage.
            Gym gym = gymRepository.GetGym(1);

            ContactUsViewModel model = new ContactUsViewModel
            {
                GymId = gym.Id,
                GymName = gym.GymName,
                GymEmail = gym.Email,
                GymAddressLineOne = gym.AddressLineOne,
                GymAddressLineTwo = gym.AddressLineTwo,
                GymTown = gym.Town,
                GymPostCode = gym.Postcode,
                GymTelephone = gym.Telephone
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ContactUs(ContactUsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            email.Subject = "A message from website visitor";
            email.Message = $"Name: {model.Name}\nEmail: {model.Email}\n\nSubject: {model.Subject}\n{model.Message}\n\nSent on: {DateTime.Now}.";
            emailService.Send(email);

            return RedirectToAction(nameof(ConfirmContactUs), "Home", model.Name);
        }

        [HttpGet]
        public IActionResult ConfirmContactUs(string name)
        {
            ViewBag.Name = name;
            return View();
        }
    }
}
