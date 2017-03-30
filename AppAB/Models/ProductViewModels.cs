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
        public string price { get; set; }   
    }

    public class ProductCreateEditViewModel
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "Artikel")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Opis artikla")]
        public string description { get; set; }

        [Required]
        [Display(Name = "Slika")]
        public string image { get; set; }

        [Required]
        [Display(Name = "Cena")]
        [RegularExpression(@"^\d+\,\d{0,2}$", ErrorMessage = "Za vnos cene uporabi format brez presledkov in z vejico za decimalko: 110,00")]
        public string price { get; set; }

        [Required]
        [Display(Name = "Znamka")]
        public int brand { get; set; }

        [Required]
        [Display(Name = "Kategorija")]
        public int subcategory { get; set; }
    }

    public class ProductDetailsDeleteViewModel
    {
        public int id { get; set; }
        
        [Display(Name = "Artikel")]
        public string name { get; set; }
        
        [Display(Name = "Opis artikla")]
        public string description { get; set; }
        
        [Display(Name = "Slika")]
        public string image { get; set; }
        
        [Display(Name = "Cena")]
        public string price { get; set; }
        
        [Display(Name = "Znamka")]
        public string brand { get; set; }
    }
}