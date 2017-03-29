using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppAB.Models
{
    public class RemoveFromOrderViewModel
    {

        public string message { get; set; }
        public int itemCount { get; set; }
        public decimal totalPrice { get; set; }
        public int deleteId { get; set; }
    }
}