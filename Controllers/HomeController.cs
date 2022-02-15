using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Restauracja.Data;
using Restauracja.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Restauracja.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            

            return NoContent();
        }
        public IActionResult Index()
        {
            List<DishCartViewModel> dishes = new List<DishCartViewModel>();
            int liczba;
            if (Request.Cookies["cart"] != null)
            {
                var allCookies = Request.Cookies.Keys;
                foreach (string cookie in allCookies)
                {
                    bool res = int.TryParse(cookie, out liczba);
                    if (res)
                    {
                        var dis = _context.Dishes.Where(z => z.Id == liczba).FirstOrDefault();
                        int ile = int.Parse(Request.Cookies[cookie.ToString()]) ;
                        decimal cena = (dis.Price - dis.Price * dis.discount) * ile;
                        DishCartViewModel s = new DishCartViewModel();
                        s.Dish = dis;
                        s.ilość = ile;
                        s.cenaPoZniżce = cena;
                        s.cenaPrzedZniżka = dis.Price * ile;
                        dishes.Add(s);
                    }
                }
            }
            ViewBag.lista = dishes;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
