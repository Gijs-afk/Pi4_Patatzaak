using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pi4_Patatzaak.Data;
using Pi4_Patatzaak.Exceptions;
using Pi4_Patatzaak.Models;

namespace Pi4_Patatzaak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApiCustomersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPatch("changepassword")]
        public async Task<IActionResult> ChangePassword(string email, string oldPassword, string newPassword, string confirmNewPassword)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null) { throw new BadRequestException("Customer not found."); }

            if (newPassword != confirmNewPassword) { throw new BadRequestException("New password and confirm new password do not match."); }

            if (customer.Password != oldPassword) { throw new BadRequestException("Old password is incorrect."); }

            customer.Password = newPassword;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Password changed successfully.");

            }
            catch (DbUpdateException)
            {
                throw new InternalServerError("An error occurred while saving the new password.");
            }
        }
    }
}
