using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class PaymentRecordVO
    {
        public decimal Deposit { get; set; }
        public decimal AdvancePayment { get; set; }
        public decimal ForeignDeposit { get; set; }
        public string Reference { get; set; }
        public long OrderId { get; set; }
        public long AccountId { get; set; }
    }
}
