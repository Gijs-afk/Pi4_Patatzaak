using Microsoft.AspNetCore.Mvc;
using Pi4_Patatzaak.Models;
using Pi4_Patatzaak.Exceptions;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pi4_Patatzaak.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WelcomeMessageApiClient _apiClient;

        public HomeController(ILogger<HomeController> logger, WelcomeMessageApiClient client)
        {
            _logger = logger;
            _apiClient = client;
        }

        public async Task<IActionResult> Index()
        {
            var welcomeMessage = await _apiClient.GetRandomWelcomeMessageAsync();
            if(welcomeMessage == null) { ViewBag.WelcomeMessage = "Welcome"; return View(); }

            try
            {
                
                var jsonObject = JObject.Parse(welcomeMessage);
                string message = jsonObject["message"]?.ToString();

                ViewBag.WelcomeMessage = message;
            }
            catch (JsonReaderException)
            {
                // Als deserialisatie mislukt, gebruik dan de oorspronkelijke string
                ViewBag.WelcomeMessage = welcomeMessage;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult EigenError()
        {
            throw new BadRequestException("Je hebt op het knopje gedrukt en nu is de applicatie gecrashed");
        }

        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}