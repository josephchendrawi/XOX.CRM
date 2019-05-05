using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class PaymentVO
    {
        public long PaymentId { get; set; }
        public long AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public DateTime? Created { get; set; }
        public string PaymentType { get; set; }
        public string PaymentMethod { get; set; }
        public string Name { get; set; }
        public string MSISDN { get; set; }
        public string CreatedBy { get; set; }

        public string From { get; set; }
        public string To { get; set; }
    }

    public class PaymentResponseList
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public List<GetPaymentResponse> PaymentList { get; set; }
        public string Message { get; set; }
    }
}
