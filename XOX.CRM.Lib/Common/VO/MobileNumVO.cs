using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class MobileNumVO
    {
        public long MobileNumId { get; set; }
        public string MSISDN { get; set; }
        public decimal Price { get; set; }
        public string BatchNum { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
