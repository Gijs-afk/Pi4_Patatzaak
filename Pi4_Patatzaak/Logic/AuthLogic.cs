using Microsoft.EntityFrameworkCore;
using Pi4_Patatzaak.Data;
using Pi4_Patatzaak.Exceptions;
using Pi4_Patatzaak.Models;
using BCrypt.Net;
using Microsoft.CodeAnalysis.Scripting;
using Humanizer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System;

namespace Pi4_Patatzaak.Logic
{
    public class AuthLogic
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthLogic(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool CheckLoginCredentials(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) { return false; }

            var customer = _dbContext.Customers.FirstOrDefault(c => c.Email == email);
            if (customer == null) { return false; }

            if (password == customer.Password)
            {
                return true;
            }

            return false;
        }
        public async Task SignInUserAsync(Customer customer)
        {

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, customer.Email)
            

        };
            var customerWithNames = _dbContext.Customers
                .Where(c => c.Email == customer.Email)
                .FirstOrDefault();

            if (customerWithNames != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, customerWithNames.CustomerName));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),authProperties);
        }

        public string GetUserName(ClaimsPrincipal user)
        {
            var userNameClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userNameClaim == null) { throw new NotFoundException("No user found"); }
            return userNameClaim.Value;
            
        }







    }
}
