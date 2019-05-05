using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class OrderModel
    {
        public long OrderId { get; set; }
        [Display(Name = "MSISDN Number")]
        public string MSISDN { get; set; }
        [Display(Name = "Subscription Plan")]
        public string SubscriptionPlan { get; set; }
        [Display(Name = "SIM Card Serial Number")]
        public string SIMCard { get; set; }
        public PersonalDetails PersonalDetails { get; set; }
        [Display(Name = "Permanent Address")]
        public AddressDetails PermanentAddress { get; set; }
        [Display(Name = "Billing Address")]
        public AddressDetails BillingAddress { get; set; }
        public BillingDetails BillingDetails { get; set; }
        public List<Document> Documents { get; set; }
        [Display(Name = "Status")]
        public string OrderStatus { get; set; }
        public string Category { get; set; }
        [Display(Name = "Submission Date")]
        public DateTime? SubmissionDate { get; set; }
        [Display(Name = "Registration Date")]
        public DateTime? RegistrationDate { get; set; }
        public string OrderNum { get; set; }
        [Display(Name = "Donor")]
        public string Remarks { get; set; }
        public long AccountId { get; set; }
        public long? ParentAccountId { get; set; }
        [Display(Name = "Payment Collected")]
        public string PaymentCollected { get; set; }
        [Display(Name = "Order Type")]
        public string OrderType { get; set; }
        public string PortReqFormId { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime? ChangePlanEffectiveDate { get; set; }

        public OrderPayment OrderPayment { get; set; }
        public List<OrderPayment> OrderSLPayment { get; set; }
    }

    public class OrderListVM
    {
        public long Idx { get; set; }
        public long OrderId { get; set; }
        [Display(Name = "Order Number")]
        public string OrderNum { get; set; }
        public string MSISDN { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Display(Name = "Registration Date")]
        public string RegistrationDate { get; set; }
        [Display(Name = "Submission Date")]
        public string SubmissionDate { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        [Display(Name = "CRP ID")]
        public string CRPId { get; set; }
        public string OrderType { get; set; }
    }
    public class OrderListVMData : DataTableModel
    {
        public List<OrderListVM> aaData;
    }

    public class PersonalDetails
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        public int Salutation { get; set; }
        [Display(Name = "Mother Maiden's Name")]
        public string MotherMaidenName { get; set; }
        [Display(Name = "Identity Type")]
        public int IdentityType { get; set; }
        [Display(Name = "Identity Number")]
        public string IdentityNo { get; set; }
        [Display(Name = "Date of Birth")]
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; }
        [Display(Name = "Mobile Number")]
        public string MobileNo { get; set; }
        [Display(Name = "Preferred Language")]
        public int PreferredLanguage { get; set; }
        public string Race { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        [Display(Name = "Received Itemised Billing")]
        public bool ReceivedItemisedBilling { get; set; }
    }

    public class AddressDetails
    {
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        [Required]
        public string Country { get; set; }
    }

    public class BillingDetails
    {
        [Display(Name = "Card Type")]
        public int CardType { get; set; }
        [Display(Name = "Card Number")]
        public string CardNo { get; set; }
        [Display(Name = "Card Holder Name")]
        public string CardHolderName { get; set; }
        [Display(Name = "Card Issuer Bank")]
        public string CardIssuerBank { get; set; }
        [Display(Name = "Card Expiry Date")]
        public int CardExpiryMonth { get; set; }
        public int CardExpiryYear { get; set; }
        [Display(Name = "Third Party Card")]
        public bool ThirdPartyFlag { get; set; }
        [Display(Name = "Itemised Billing")]
        public bool PrintedBillingFlg { get; set; }
    }

    public class Document
    {
        public long DocumentId { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string Type { get; set; }
    }
    
    public class Activity
    {
        public long OrderId { get; set; }
        public string Description { get; set; }
        public string CreatedDateTime { get; set; }
        public string DueDate { get; set; }
        public string Assignee { get; set; }
        public string Remarks { get; set; }
        public string CreatedDateTimeText { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
    }
    public class ActivityData : DataTableModel
    {
        public List<Activity> aaData;
    }

    public class AssigneeOrder
    {
        public long AssigneeId { get; set; }
        public string Assignee { get; set; }
        public List<OrderStatusCount> OrderStatusCount { get; set; }
    }

    public class OrderStatusCount
    {
        public string OrderStatus { get; set; }
        public long Count { get; set; }
    }


    public class OrderPayment
    {
        public long AccountId { get; set; }
        public decimal Deposit { get; set; }
        public decimal AdvancePayment { get; set; }
        public decimal ForeignDeposit { get; set; }
        public string Reference { get; set; }
    }

    public class ChangePlanVM
    {
        public long AccountId { get; set; }
        [Display(Name = "Current Plan")]
        public string CurrentPlan { get; set; }
        [Display(Name = "Next Cycle Billing Date")]
        public string NextCycleBillingDate { get; set; }
        [Display(Name = "New Plan")]
        public string NewPlan { get; set; }
    }

    public class TerminateVM
    {
        public long AccountId { get; set; }
        [Display(Name = "Subscription Activation Date")]
        public DateTime SubscriptionActivationDate { get; set; }
        [Display(Name = "Subscription Contract")]
        public DateTime SubscriptionContract { get; set; }
        [Display(Name = "Outstanding Amount")]
        public decimal OutstandingAmount { get; set; }
        [Display(Name = "Deposit Amount")]
        public decimal DepositAmount { get; set; }
        [Display(Name = "Settlement Amount")]
        public decimal SettlementAmount { get; set; }
    }

}