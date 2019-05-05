using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class OverallStatus
    {
        public int ActiveSubscriberCount { get; set; }
        public decimal BillPaymentSum { get; set; }
        public int TerminatedSubscriberinThisMonthCount { get; set; }
        public decimal PaymentCollectedSum { get; set; }
    }

    public class ReportPaymentModel
    {
        public long PaymentId { get; set; }
        public long AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public DateTime? Created { get; set; }
        public string Date { get; set; }
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        public string Name { get; set; }
        public string MSISDN { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        public string From { get; set; }
        public string To { get; set; }
    }
    public class PaymentData : DataTableModel
    {
        public List<ReportPaymentModel> aaData;
    }
    
    public class PaymentCSV
    {
        public string MSISDN { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string PaymentType { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public string CreatedBy { get; set; }
    }

    
    public class AssetModel
    {
        public long AssetId { get; set; }
        public string Status { get; set; }
        [Display(Name = "Activation Date")]
        public string EffectiveDate { get; set; }
        public string Plan { get; set; }
        [Display(Name = "Printed Billing")]
        public string PrintedBilling { get; set; }
        public long AccountId { get; set; }
        [Display(Name = "Subscriber Name")]
        public string SubscriberName { get; set; }
        public string MSISDN { get; set; }
    }
    public class AssetData : DataTableModel
    {
        public List<AssetModel> aaData;
    }

    public class BillPaymentModel
    {
        public long BillPaymentId { get; set; }
        [Display(Name = "Submission Date")]
        public string SubmissionDate { get; set; }
        [Display(Name = "Credit Card No")]
        public string CreditCardNo { get; set; }
        public string MSISDN { get; set; }
        [Display(Name = "Subscriber Name")]
        public string SubscriberName { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Amount Due")]
        public decimal? AmountDue { get; set; }
        [Display(Name = "Credit Card CVV")]
        public string CreditCardCVV { get; set; }
        [Display(Name = "Credit Card Expiry")]
        public string CCExpiry { get; set; }
        [Display(Name = "Card Issuer Bank")]
        public string CardIssuerBank { get; set; }
    }
    public class BillPaymentData : DataTableModel
    {
        public List<BillPaymentModel> aaData;
    }

    public class BillPaymentReport
    {
        public string SubmissionDate { get; set; }
        public string CreditCardNumber { get; set; }
        public string MSISDN { get; set; }
        public string SubscriberName { get; set; }
        public string CompanyName  { get; set; }
        public string Amount { get; set; }
        public string CVV { get; set; }
        public string CreditCardExpiry { get; set; }
    }

    public class AssetReportModel
    {
        public string MSISDN { get; set; }
        public string SubscriberName { get; set; }
        public string Status { get; set; }
        public string ActivationDate { get; set; }
        public string Plan { get; set; }
        public string PrintedBilling { get; set; }
    }


    public class AuditTrailModel
    {
        public long AuditTrailId { get; set; }
        [Display(Name = "Username")]
        public string CreatedBy { get; set; }
        public string Created { get; set; }
        public string Action { get; set; }
        [Display(Name = "Old Value")]
        public string OldValue { get; set; }
        [Display(Name = "New Value")]
        public string NewValue { get; set; }
        public string Module { get; set; }
        public string Field { get; set; }
    }
    public class AuditTrailData : DataTableModel
    {
        public List<AuditTrailModel> aaData;
    }


    public class RefundModel
    {
        public long RefundId { get; set; }
        public Nullable<long> AccountId { get; set; }
        public string Name { get; set; }
        public string MSISDN { get; set; }
        [Display(Name = "Plan Type")]
        public string PlanType { get; set; }
        [Display(Name = "Termination Date")]
        public string TerminationDate { get; set; }
        public Nullable<decimal> Deposit { get; set; }
        [Display(Name = "Advance Payment")]
        public Nullable<decimal> AdvancePayment { get; set; }
        [Display(Name = "Bill Payment")]
        public Nullable<decimal> BillPayment { get; set; }
        public Nullable<decimal> Usage { get; set; }
        [Display(Name = "Total Refund")]
        public Nullable<decimal> TotalRefund { get; set; }
        [Display(Name = "Refund Date")]
        public string RefundDate { get; set; }
    }
    public class RefundListVMData : DataTableModel
    {
        public List<RefundModel> aaData;
    }

    public class RefundExcel
    {
        public string Name { get; set; }
        public string MSISDN { get; set; }
        public string Plan { get; set; }
        [Display(Name = "Termination Date")]
        public string TerminationDate { get; set; }
        public Nullable<decimal> Deposit { get; set; }
        [Display(Name = "Advance Payment")]
        public Nullable<decimal> AdvancePayment { get; set; }
        [Display(Name = "Bill Payment")]
        public Nullable<decimal> BillPayment { get; set; }
        public Nullable<decimal> Usage { get; set; }
        [Display(Name = "Total Refund")]
        public Nullable<decimal> TotalRefund { get; set; }
        [Display(Name = "Refund Date")]
        public string RefundDate { get; set; }
    }

}