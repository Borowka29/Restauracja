using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restauracja.Data;
using Restauracja.Models;

namespace Restauracja.Controllers
{
    public class DishesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DishesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dishes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Dishes.ToListAsync());
        }

        // GET: Dishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes.Include(z=>z.Opinions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (dish == null)
            {
                return NotFound();
            }

            //czy już user dodał komentarz pod daniem
            var pom = _context.Opinions.Where(z => z.User.Email == HttpContext.User.Identity.Name && z.DishID == id).ToList().Count();

            if (pom == 0) ViewBag.CzymozeDodacKomentarz = true;
            else ViewBag.CzymozeDodacKomentarz = false;

            // opinie pod daniem
            List<Opinion> opinions = _context.Opinions.Include(z => z.User).Where(b => b.DishID == id).ToList();

            //widok dla details z opiniami
            DishDetailsViewModel data = new DishDetailsViewModel();
            data.Dish = dish;
            data.Opinions = opinions;
            //data.User=(ApplicationUser)_context.Users.Where(d=>d.Email== HttpContext.User.Identity.Name).FirstOrDefault();
            
            if(Request.Cookies[id.ToString()]!=null) ViewBag.CzymozeDodacdoKoszyka = false;
            else ViewBag.CzymozeDodacdoKoszyka = true;
            return View(data);


            // brakuje nie da sie wstawić opini jak sie nie kupiło produktu i jak niezalogowany
            //i jak jest niezalogowany to nie ma user
        }
        public List<SelectListItem> func()
        {
            List<SelectListItem> discountList = new List<SelectListItem>();
            discountList.Add(new SelectListItem { Text = "No Discount", Value = "1", Selected = true });
            discountList.Add(new SelectListItem { Text = "5 discount", Value = "0.05" });
            discountList.Add(new SelectListItem { Text = "10 discount", Value = "0.1" });
            discountList.Add(new SelectListItem { Text = "15 discount", Value = "0.15" });
            discountList.Add(new SelectListItem { Text = "20 discount", Value = "0.2" });
            return discountList;
        }
        // GET: Dishes/Create
        public IActionResult Create()
        {
            //Lista zniżek
            
            ViewBag.items = new SelectList(func(), "Value", "Text"); 

            return View();
        }

        // POST: Dishes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public IActionResult DeleteComment(int id)
        {
            var opinion = _context.Opinions.Where(z => z.Id == id).ToList();
            if (opinion.Count != 0)
            {
                _context.Opinions.Remove(opinion[0]);
                _context.SaveChanges();
            }

            return NoContent();
        }
        [HttpPost]
        public IActionResult AddComment(int id, string Name, string star)
        {
            Opinion opinion = new Opinion();
            opinion.Description = Name;
            opinion.Rating = int.Parse(star);
            opinion.DishID = id;
            opinion.UserID = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.Add(opinion);
             _context.SaveChanges();

            return NoContent();
        }
        [HttpPost]
        public IActionResult AddCart(int id)
        {
            //jesli nielalogowany to cookie
            if (!User.Identity.IsAuthenticated)
            {
                //jesli już istnieje koszyk dodajemy danie do koszyka
                if (Request.Cookies["cart"] != null)
                {
                    //spr czy napewno nie ma dania w koszyku
                    if (Request.Cookies[id.ToString()] == null)
                    {
                        CookieOptions options = new CookieOptions();
                        options.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Append(id.ToString(), "1", options);
                    }

                }
                else
                {
                    //jesli już istnieje koszyk tworzymy koszyk i ciastko dania
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Append("cart", "istnieje", options);
                    Response.Cookies.Append(id.ToString(), "1", options);


                }
                
            }
            //jek zalogowany to dodac do karty
            else
            {

            }
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Ingredients,Price,Poster,discount,isAvailable,isRecommended")] Dish dish, IFormFile Upload, string SelectedDiscount, string PriceDecimal)
        {

            //naliczanie zniżki jak jest
            dish.discount = System.Convert.ToDecimal(SelectedDiscount, new CultureInfo("en-US"));
            if (dish.discount == 1)
            {
                dish.Price = System.Convert.ToDecimal(PriceDecimal, new CultureInfo("en-US"));
            }
            else
            {
                var price= System.Convert.ToDecimal(PriceDecimal, new CultureInfo("en-US"));
                var operation = dish.discount * price;
                dish.Price =price- operation;
            }
            
            //spr czy dobrze załadowało obraz
            if (Upload != null)
            {
                if (Upload.Length > 0 && Upload.Length < 300000)
                {
                    using (var target = new MemoryStream())
                    {
                        Upload.CopyTo(target);
                        dish.Poster = target.ToArray();
                    }
                }

            }
            else
            {
                ViewBag.items = new SelectList(func(), "Value", "Text");
                return View(dish);
            }

            //dodanie dania
            _context.Add(dish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }
            return View(dish);
        }

        // POST: Dishes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Ingredients,Price,Poster,discount,isAvailable,isRecommended")] Dish dish)
        {
            if (id != dish.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.Id))
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
            return View(dish);
        }

        // GET: Dishes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.Id == id);
        }
    }
}
