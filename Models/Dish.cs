using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Restauracja.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }

        //cena
        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        //znizka
        public decimal discount { get; set; }


        //zdj
        [Display(Name = "Plakat")]
        [Required(ErrorMessage = "Add Poster")]
        [DataType(DataType.Upload)]
        public byte[] Poster { get; set; }
        //public int NumberOfOpinions { get; set; }  narazie nie potrzebne bo ilość opini sie weźmie z listy.count
        //public int NumberOfRatings { get; set; }
        
        //czy jest dostępny
        public bool isAvailable { get; set; }
        //czy polecane
        public bool isRecommended { get; set; }
        public virtual ICollection<Opinion> Opinions { get; set; }
        //public virtual ICollection<Sales> Sales { get; set; }

    }
}
