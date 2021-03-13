using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;

namespace TheGymWebsite.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult BannedUser(string email, string name)
        {
            ViewBag.Email = email;
            ViewBag.Name = name;
            return View();
        }

        [HttpGet]
        public IActionResult EmailConfirmationError()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PaymentError()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AlreadyAMember(string expiry)
        {
            ViewBag.Expiry = expiry;
            return View();
        }

        [HttpGet]
        public IActionResult AccountLocked()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UserNotFound(string idOrEmail)
        {
            ViewBag.UserIdOrEmail = idOrEmail;
            return View();
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                    break;
                    // You can add more status codes from here...
            }

            return View("NotFound");
        }


        [Route("Error")]
        public IActionResult Error()
        {
            // Retrieve the exception Details
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.ExceptionPath = exceptionHandlerPathFeature.Path;
            ViewBag.ExceptionMessage = exceptionHandlerPathFeature.Error.Message;
            ViewBag.StackTrace = exceptionHandlerPathFeature.Error.StackTrace;

            return View();
        }
    }
}

