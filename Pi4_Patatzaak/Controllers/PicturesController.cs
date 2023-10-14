using System;
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
    public class PicturesController : Controller
    {
        private readonly AppDbContext _context;

        public PicturesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Pictures
        public async Task<IActionResult> Index()
        {
              return _context.Pictures != null ? 
                          View(await _context.Pictures.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Pictures'  is null.");
        }

        // GET: Pictures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pictures == null)
            {
                return NotFound();
            }

            var pictures = await _context.Pictures
                .FirstOrDefaultAsync(m => m.PictureID == id);
            if (pictures == null)
            {
                return NotFound();
            }

            return View(pictures);
        }

        // GET: Pictures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pictures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PictureID,FileName,PictureDescription")] Pictures pictures)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pictures);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pictures);
        }

        // GET: Pictures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pictures == null)
            {
                return NotFound();
            }

            var pictures = await _context.Pictures.FindAsync(id);
            if (pictures == null)
            {
                return NotFound();
            }
            return View(pictures);
        }

        // POST: Pictures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PictureID,FileName,PictureDescription")] Pictures pictures)
        {
            if (id != pictures.PictureID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pictures);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PicturesExists(pictures.PictureID))
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
            return View(pictures);
        }

        // GET: Pictures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pictures == null)
            {
                return NotFound();
            }

            var pictures = await _context.Pictures
                .FirstOrDefaultAsync(m => m.PictureID == id);
            if (pictures == null)
            {
                return NotFound();
            }

            return View(pictures);
        }

        // POST: Pictures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pictures == null)
            {
                return Problem("Entity set 'AppDbContext.Pictures'  is null.");
            }
            var pictures = await _context.Pictures.FindAsync(id);
            if (pictures != null)
            {
                _context.Pictures.Remove(pictures);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PicturesExists(int id)
        {
          return (_context.Pictures?.Any(e => e.PictureID == id)).GetValueOrDefault();
        }
    }
}
