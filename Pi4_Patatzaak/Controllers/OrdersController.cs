using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pi4_Patatzaak.Data;
using Pi4_Patatzaak.Logic;
using Pi4_Patatzaak.Models;

namespace Pi4_Patatzaak.Controllers
{
    [Authorize]

    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly OrderLogic _orderLogic;
        private readonly AuthLogic _authLogic;

        public OrdersController(AppDbContext context, OrderLogic orderLogic, AuthLogic authLogic)
        {
            _context = context;
            _orderLogic = orderLogic;
            _authLogic = authLogic;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Orders.Include(o => o.Customer);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        //public IActionResult Create()
        //{
        //    ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerName");
        //    return View();
        //}

        // POST: Orders/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("OrderID")] Order order)
        //{
        //    Order newOrder = new Order();
        //    string customerName = _authLogic.GetUserName(HttpContext.User);
        //    var customerField = _context.Customers
        //        .Where(c => c.CustomerName == customerName)
        //        .FirstOrDefault();
        //    int UserID = customerField.CustomerID;

        //    newOrder = _orderLogic.CreateOrder(newOrder, UserID);


        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerName", order.CustomerID);
        //    return View(order);
        //}


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var products = await _context.Products.ToListAsync();
            ViewBag.ProductList = new SelectList(products, "ProductID", "ProductName");

            var order = new Order
            {
                Orderlines = new List<OrderLine>()
            };

            var productsAsJson = JsonSerializer.Serialize(products);
            ViewData["Products"] = productsAsJson;

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                // Create the order and save it to the database
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // You will add order lines dynamically using JavaScript
                // No need to add order lines here

                return RedirectToAction(nameof(Index));
            }

            // If the model is not valid, redisplay the form with errors
            var products = await _context.Products.ToListAsync();
            ViewBag.ProductList = new SelectList(products, "ProductID", "ProductName");
            return View(order);
        }






        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerName", order.CustomerID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,CustomerID,TotalPrice,Status")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerName", order.CustomerID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'AppDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderID == id)).GetValueOrDefault();
        }
    }
}
