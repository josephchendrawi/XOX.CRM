using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class BillPaymentVO
    {
        public long BillPaymentId { get; set; }
        public string SubmissionDate { get; set; }
        public string CreditCardNo { get; set; }
        public string MSISDN { get; set; }
        public string SubscriberName { get; set; }
        public string CompanyName { get; set; }
        public decimal? AmountDue { get; set; }
        public string CreditCardCVV { get; set; }
        public int? CCExpiryMonth { get; set; }
        public int? CCExpiryYear { get; set; }
        public string CardIssuerBank { get; set; }

        //for reporting purpose
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
