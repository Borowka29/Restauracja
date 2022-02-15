using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restauracja.Models
{
    public class Opinion
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public int DishID { get; set; }
        public virtual Dish Dish { get; set; }

    }
}
