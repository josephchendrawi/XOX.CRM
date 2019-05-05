using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class ProductModel
    {
        [Display(Name = "Product Id")]
        public long ProductId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public string Type { get; set; }
        [Display(Name = "Price (RM)")]
        public string Price { get; set; }
        public string PriceType { get; set; }
        public string Quota { get; set; }
        public bool VasFlag { get; set; }
        
        public List<ProductModel> Child { get; set; }
    }
}