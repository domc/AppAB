using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppAB.Models
{
    public class OrderViewModel
    {
        public List<order_items> items { get; set; }

        [Display(Name = "Skupna cena")]
        public decimal orderTotal { get; set; }
    }
}