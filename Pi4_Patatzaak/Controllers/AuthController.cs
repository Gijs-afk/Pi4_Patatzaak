using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Pi4_Patatzaak.Models;
using Pi4_Patatzaak.Data;
using Pi4_Patatzaak.Logic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pi4_Patatzaak.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AuthLogic _authLogic;

        public AuthController(AppDbContext context, AuthLogic authLogic)
        {
            _context = context;
            _authLogic = authLogic;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated) { return RedirectToAction("Index", "Home"); }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Customer customer)
        {

            {
                bool isValidLogin = _authLogic.CheckLoginCredentials(customer.Email, customer.Password);

                if (isValidLogin)
                {
                    await _authLogic.SignInUserAsync(customer);
                    return RedirectToAction("Index", "Home");
                }

            }

            ViewData["VallidateMessage"] = "User not found";

            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,CustomerName,Email,Password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();

                // Sign in the user using AuthLogic
                await _authLogic.SignInUserAsync(customer);
                return RedirectToAction("Index", "Home");
            }
            return View(customer);
        }
    }
}



