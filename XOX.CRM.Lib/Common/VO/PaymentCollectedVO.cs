using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class PaymentCollectedVO
    {
        public long Id { get; set; }
        
        public Nullable<long> PaymentId { get; set; }
        public string MSISDN { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> ChargeType { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string Method { get; set; }
    }
}
