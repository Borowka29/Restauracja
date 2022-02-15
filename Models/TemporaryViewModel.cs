using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restauracja.Models
{
    public class TemporaryViewModel
    {

    }

    public class DishDetailsViewModel
    {
        public Dish Dish { get; set; }
        public virtual List<Opinion> Opinions { get; set; }
        public virtual ApplicationUser User { get; set; }

    }

    public class DishCartViewModel
    {
        public Dish Dish { get; set; }
        public int ilość { get; set; }
        public decimal cenaPoZniżce { get; set; }
        public decimal cenaPrzedZniżka { get; set; }

    }
}
