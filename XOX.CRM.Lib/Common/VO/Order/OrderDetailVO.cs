using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class OrderDetailVO
    {
        public long OrderId { get; set; }
        public string OrderNum { get; set; }
        public string OrderType { get; set; }
        public string MSISDN { get; set; }
        public string SubscriptionPlan { get; set; }
        public string SIMCard { get; set; }
        public AccountVO Account { get; set; }
        public string OrderStatus { get; set; }
        public string Category { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string Remarks { get; set; }
        public string CRPId { get; set; }
        public string PortReqFormId { get; set; }

        public string PaymentCollected { get; set; }

        public OrderPaymentVO OrderPayment { get; set; }
        public List<OrderPaymentVO> OrderSLPayment { get; set; }

        public DateTime? ChangePlanEffectiveDate { get; set; }
    }

    public class OrderPaymentVO
    {
        public long AccountId { get; set; }
        public decimal Deposit { get; set; }
        public decimal AdvancePayment { get; set; }
        public decimal ForeignDeposit { get; set; }
        public string Reference { get; set; }
    }

}
