//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppAB
{
    using System;
    using System.Collections.Generic;
    
    public partial class custumer_addresses
    {
        public int id { get; set; }
        public int customer { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
    
        public virtual customers customers { get; set; }
    }
}
