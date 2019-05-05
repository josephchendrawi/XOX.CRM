using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class MobileNumModel
    {
        public long MobileNumId { get; set; }
        [Required]
        public string MSISDN { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "Batch Number")]
        public string BatchNum { get; set; }
        [Display(Name = "Creation Date")]
        public string CreatedDate { get; set; }
    }
    public class MobileNumListData : DataTableModel
    {
        public List<MobileNumModel> aaData;
    }

    public class BatchModel
    {
        [Display(Name = "Batch Number")]
        public string BatchNum { get; set; }
        [Display(Name = "Min Price")]
        public decimal PriceMin { get; set; }
        [Display(Name = "Max Price")]
        public decimal PriceMax { get; set; }
        [Display(Name = "Number Count")]
        public long NumberCount { get; set; }

        public List<AssignedAccount> Accounts { get; set; }
    }
    public class BatchListData : DataTableModel
    {
        public List<BatchModel> aaData;
    }

    public class AssignedAccount
    {
        public long AccountId { get; set; }
        public string Name { get; set; }
        public string MSISDN { get; set; }
        public bool Assigned { get; set; }
    }

}