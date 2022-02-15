using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restauracja.Data;
using Restauracja.Models;

namespace Restauracja.Controllers
{
    public class OpinionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OpinionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var opinion = _context.Opinions.Where(z => z.Id == id).ToList() ;
            if (opinion.Count != 0)
            {
                _context.Opinions.Remove(opinion[0]);
                _context.SaveChanges();
                result = true;
            }
            return Json(result);
        }

        // GET: Opinions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Opinions.Include(o => o.Dish).Include(o => o.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Opinions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions
                .Include(o => o.Dish)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (opinion == null)
            {
                return NotFound();
            }

            return View(opinion);
        }

        // GET: Opinions/Create
        public IActionResult Create()
        {
            ViewData["DishID"] = new SelectList(_context.Dishes, "Id", "Id");
            //ViewData["UserID"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            return View();
        }

        // POST: Opinions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Date,Rating,UserID,DishID")] Opinion opinion)
        {
            //!!! warunek na zalogowanie
            if (ModelState.IsValid)
            {
                opinion.UserID= this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(opinion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DishID"] = new SelectList(_context.Dishes, "Id", "Id", opinion.DishID);
            ViewData["UserID"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", opinion.UserID);
            return View(opinion);
        }

        // GET: Opinions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions.FindAsync(id);
            if (opinion == null)
            {
                return NotFound();
            }
            ViewData["DishID"] = new SelectList(_context.Dishes, "Id", "Id", opinion.DishID);
            ViewData["UserID"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", opinion.UserID);
            return View(opinion);
        }

        // POST: Opinions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Date,Rating,UserID,DishID")] Opinion opinion)
        {
            if (id != opinion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(opinion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpinionExists(opinion.Id))
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
            ViewData["DishID"] = new SelectList(_context.Dishes, "Id", "Id", opinion.DishID);
            ViewData["UserID"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", opinion.UserID);
            return View(opinion);
        }

        // GET: Opinions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions
                .Include(o => o.Dish)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (opinion == null)
            {
                return NotFound();
            }

            return View(opinion);
        }

        // POST: Opinions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opinion = await _context.Opinions.FindAsync(id);
            _context.Opinions.Remove(opinion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OpinionExists(int id)
        {
            return _context.Opinions.Any(e => e.Id == id);
        }
    }
}
