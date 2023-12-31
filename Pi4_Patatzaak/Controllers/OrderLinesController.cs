﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pi4_Patatzaak.Data;
using Pi4_Patatzaak.Models;

namespace Pi4_Patatzaak.Controllers
{
    public class OrderLinesController : Controller
    {
        private readonly AppDbContext _context;

        public OrderLinesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrderLines
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.OrderLines.Include(o => o.Order).Include(o => o.Product);
            return View(await appDbContext.ToListAsync());
        }

        // GET: OrderLines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderLines == null)
            {
                return NotFound();
            }

            var orderLine = await _context.OrderLines
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderLineID == id);
            if (orderLine == null)
            {
                return NotFound();
            }

            return View(orderLine);
        }

        // GET: OrderLines/Create
        public IActionResult Create()
        {
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductName");
            return View();
        }

        // POST: OrderLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderLineID,OrderID,ProductID,Amount,ActualPrice")] OrderLine orderLine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderLine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderLine.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductName", orderLine.ProductID);
            return View(orderLine);
        }

        // GET: OrderLines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderLines == null)
            {
                return NotFound();
            }

            var orderLine = await _context.OrderLines.FindAsync(id);
            if (orderLine == null)
            {
                return NotFound();
            }
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderLine.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductName", orderLine.ProductID);
            return View(orderLine);
        }

        // POST: OrderLines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderLineID,OrderID,ProductID,Amount,ActualPrice")] OrderLine orderLine)
        {
            if (id != orderLine.OrderLineID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderLine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderLineExists(orderLine.OrderLineID))
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
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderLine.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductName", orderLine.ProductID);
            return View(orderLine);
        }

        // GET: OrderLines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderLines == null)
            {
                return NotFound();
            }

            var orderLine = await _context.OrderLines
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderLineID == id);
            if (orderLine == null)
            {
                return NotFound();
            }

            return View(orderLine);
        }

        // POST: OrderLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderLines == null)
            {
                return Problem("Entity set 'AppDbContext.OrderLines'  is null.");
            }
            var orderLine = await _context.OrderLines.FindAsync(id);
            if (orderLine != null)
            {
                _context.OrderLines.Remove(orderLine);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderLineExists(int id)
        {
          return (_context.OrderLines?.Any(e => e.OrderLineID == id)).GetValueOrDefault();
        }
    }
}
