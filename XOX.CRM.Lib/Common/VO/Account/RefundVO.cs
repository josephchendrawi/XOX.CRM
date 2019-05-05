using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class AddRefundVO
    {
        public string Name { get; set; }
        public string MSISDN { get; set; }
        public decimal Deposit { get; set; }
        public decimal AdvancePayment { get; set; }
        public decimal BillPayment { get; set; }
        public decimal Usage { get; set; }
        public DateTime RefundDate { get; set; }
    }
    public class RefundVO
    {
        public long ROW_ID { get; set; }
        public Nullable<long> CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public Nullable<long> LAST_UPD_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPD { get; set; }
        public Nullable<long> ACCNT_ID { get; set; }
        public string NAME { get; set; }
        public string MSISDN { get; set; }
        public string PLAN { get; set; }
        public Nullable<System.DateTime> TERMINATION_DATE { get; set; }
        public Nullable<decimal> DEPOSIT { get; set; }
        public Nullable<decimal> ADVANCE_PAYMENT { get; set; }
        public Nullable<decimal> BILL_PAYMENT { get; set; }
        public Nullable<decimal> USAGE { get; set; }
        public Nullable<decimal> TOTAL_REFUND { get; set; }
        public Nullable<System.DateTime> REFUND_DATE { get; set; }
        public string REMARKS { get; set; }
        public Nullable<int> STATUS { get; set; }
    }
}
