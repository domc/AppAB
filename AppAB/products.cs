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
    using System.ComponentModel.DataAnnotations;

    public partial class products
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public products()
        {
            this.order_items = new HashSet<order_items>();
        }
    
        public int id { get; set; }

        [Required]
        [Display(Name = "Ime izdelka")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Opis izdelka")]
        public string description { get; set; }

        [Required]
        [Display(Name = "Slika")]
        public string image { get; set; }

        [Required]
        [Display(Name = "Cena")]
        public decimal price { get; set; }

        [Display(Name = "Dodano")]
        public System.DateTime create_date { get; set; }

        [Required]
        [Display(Name = "Znamka")]
        public int brand { get; set; }

        [Required]
        [Display(Name = "Kategorija")]
        public int subcategory { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<order_items> order_items { get; set; }
        public virtual product_brands product_brands { get; set; }
        public virtual product_subcategories product_subcategories { get; set; }
    }
}
