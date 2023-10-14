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
using Microsoft.EntityFrameworkCore.Storage;
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
        private readonly PricingLogic _pricingLogic;

        public OrdersController(AppDbContext context, OrderLogic orderLogic, AuthLogic authLogic, PricingLogic pricingLogic)
        {
            _context = context;
            _orderLogic = orderLogic;
            _authLogic = authLogic;
            _pricingLogic = pricingLogic;
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



        [HttpGet]
        public IActionResult Create()
        {
            var products = _context.Products.ToList();

            foreach (var product in products)
            {
                product.Price = _pricingLogic.GetProductPrice(product.ProductID);
            }

            var productItems = products.Select(p => new SelectListItem
            {
                Value = p.ProductID.ToString(),
                Text = $"{p.ProductName} - Price: ${p.Price:F2}"
            }).ToList();

            ViewBag.ProductList = new SelectList(productItems, "Value", "Text");

            var order = new Order
            {
                Orderlines = new List<OrderLine>
                {
                    new OrderLine(),
                    new OrderLine(),
                    new OrderLine()
                }
            };

            return View(order);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            string customerName = _authLogic.GetUserName(HttpContext.User);
            var customerField = _context.Customers
                .Where(c => c.CustomerName == customerName)
                .FirstOrDefault();
            int UserID = customerField.CustomerID;

            order.CustomerID = UserID;

            if (ModelState.IsValid)
            {
                
                List<OrderLine> databaseorderlineList = new List<OrderLine>();

                decimal totalOrderPrice = 0;

                foreach (var orderLine in order.Orderlines)
                {
                    var product = _context.Products.FirstOrDefault(p => p.ProductID == orderLine.ProductID);
                    product.Price = _pricingLogic.GetProductPrice(product.ProductID);


                    if (product != null)
                    {
                        // Calculate the actual price
                        orderLine.ActualPrice = product.Price * orderLine.Amount;
                        totalOrderPrice += orderLine.ActualPrice;
                        orderLine.OrderID = order.OrderID;

                        // Generate Ordeline for database
                        OrderLine databaseOrderLine = new OrderLine
                        {
                            ProductID = orderLine.ProductID,
                            Amount = orderLine.Amount,
                            OrderID = order.OrderID,
                            ActualPrice = orderLine.ActualPrice
                        };
                        
                        databaseorderlineList.Add(databaseOrderLine);

                    }
                }

                
                _context.Orders.Add(order);
                order.TotalPrice = totalOrderPrice;
                await _context.SaveChangesAsync();

                //save all orderlines to db
                foreach (var i in databaseorderlineList)
                {
                    i.OrderID = order.OrderID;
                    _context.OrderLines.Add(i);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

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
