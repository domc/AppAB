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

    public class OrderDetailViewModel
    {
        public List<order_items> items { get; set; }

        [Display(Name = "Skupna cena")]
        public decimal orderTotal { get; set; }

        [Display(Name = "Ustvarjeno")]
        public string created { get; set; }

        [Display(Name = "Naročilo oddano")]
        public string confirmed { get; set; }
    }

    public class OrdersListViewModel
    {
        public string id { get; set; }

        [Display(Name = "Skupna cena")]
        public decimal total_price { get; set; }

        [Display(Name = "Uporabnik")]
        public string userName { get; set; }

        [Display(Name = "Št. artiklov v košarici")]
        public int numberOfItems { get; set; }

        [Display(Name = "Ustvarjeno")]
        public string created { get; set; }

        [Display(Name = "Naročilo oddano")]
        public string confirmed { get; set; }
    }

    public class RemoveFromOrderViewModel
    {
        public string message { get; set; }
        public int itemCount { get; set; }
        public decimal totalPrice { get; set; }
        public int deleteId { get; set; }
    }
}