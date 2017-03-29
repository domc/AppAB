using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppAB.Models
{
    public class ProductListViewModel
    {
        public int id { get; set; }

        [Display(Name = "Artikel")]
        public string name { get; set; }
        
        [Display(Name = "Opis artikla")]
        public string description { get; set; }
        
        [Display(Name = "Slika")]
        public string image { get; set; }
        
        [Display(Name = "Cena")]
        [RegularExpression(@"^\d+\,\d{0,2}$")]
        public string price { get; set; }   
    }
}