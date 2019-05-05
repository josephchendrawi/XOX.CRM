using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class AccountModel
    {
        public long AccountId { get; set; }
        public PersonalInfo PersonalInfo { get; set; }
        public BankingInfo BankingInfo { get; set; }
        [Display(Name = "Permanent Address")]
        public AddressInfo AddressInfo { get; set; }
        [Display(Name = "Billing Address")]
        public AddressInfo BillingAddressInfo { get; set; }
        public List<File> Files { get; set; }
        [Display(Name = "Account Type")]
        public int AccountType { get; set; }
        [Display(Name = "Registration Date")]
        public DateTime? RegistrationDate { get; set; }
        [Display(Name = "Termination Date")]
        public DateTime? TerminationDate { get; set; }
        public int Grade { get; set; }
        [Display(Name = "Plan Info")]
        public string Plan { get; set; }
        public AccountUsage AccountUsage { get; set; }
        [Display(Name = "SIM Serial Number")]
        public string SIMSerialNumber { get; set; }

        public long? ParentAccountId { get; set; }
    }

    public class AccountListVM
    {
        public long AccountId { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        public string Salutation { get; set; }
        [Display(Name = "Identity Type")]
        public string IdentityType { get; set; }
        [Display(Name = "Identity Number")]
        public string IdentityNo { get; set; }
        public string Email { get; set; }
        public string MSISDN { get; set; }
        public int Idx { get; set; }
        public string Status { get; set; }
        public string AccountType { get; set; }
    }
    public class AccountListVMData : DataTableModel
    {
        public List<AccountListVM> aaData;
    }

    public class PersonalInfo
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
        
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }
        
        [Display(Name = "Preferred Language")]
        public int PreferredLanguage { get; set; }
        
        public string Race { get; set; }
        
        public int Gender { get; set; }
        
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        
        [Display(Name = "Customer Status")]
        public string CustomerStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "Sponsor Personnel")]
        public string SponsorPersonnel { get; set; }
        
        [Display(Name = "Customer Account Number")]
        public string CustomerAccountNumber { get; set; }
        
        [Display(Name = "MSISDN Number")]
        public string MSISDNNumber { get; set; }
        
        [Display(Name = "Credit Limit")]
        public decimal CreditLimit { get; set; }
    }

    public class BankingInfo
    {
        
        public int BankId { get; set; }
        
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        [Display(Name = "Bank Account Number")]
        public string BankAccountNumber { get; set; }
        
        [Display(Name = "Card Type")]
        public int CardType { get; set; }
        
        [Display(Name = "Credit Card Number")]
        public string CreditCardNo { get; set; }
        
        [Display(Name = "Card Holder Name")]
        public string CardHolderName { get; set; }
        
        [Display(Name = "Card Issuer Bank")]
        public string CardIssuerBank { get; set; }
        
        [Display(Name = "Card Expiry Date")]
        [Range(1, 12)]
        public int CardExpiryMonth { get; set; }
        [Range(1900, Int32.MaxValue)]
        public int CardExpiryYear { get; set; }
        
        [Display(Name = "Third Party")]
        public int ThirdPartyFlag { get; set; }
        [Display(Name = "Bank Account Name")]
        public string BankAccountName { get; set; }

        [Display(Name = "Itemised Billing")]
        public int PrintedBillingFlg { get; set; }
    }

    public class AddressInfo
    {
        public long AddressId { get; set; }
        
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        
        public string City { get; set; }
        
        public string State { get; set; }
        
        public string Postcode { get; set; }
        
        public string Country { get; set; }
        
        public int Status { get; set; }
        public int AddressType { get; set; }

    }

    public class File
    {
        public long FileId { get; set; }
        public long AccountId { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string Type { get; set; }
    }

    public class AccountActivity
    {
        public long AccountActivityId { get; set; }
        public long AccountId { get; set; }
        public string Description { get; set; }
        public string Assignee { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string CreatedDateTime { get; set; }
        public string CreatedDateTimeText { get; set; }
    }
    public class AccountActivityListData : DataTableModel
    {
        public List<AccountActivity> aaData;
    }

    public class CUGModel
    {
        public string CUGNumber { get; set; }
    }

    public class CUGListData : DataTableModel
    {
        public List<CUGModel> aaData;
    }

    public class PaymentModel
    {
        public long AccountId { get; set; }
        [Required]
        public string MSISDN { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Reference { get; set; }
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        [Display(Name = "Card Issuer Bank")]
        public string CardIssuerBank { get; set; }
        [Display(Name = "Card Type")]
        public string CardType { get; set; }
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }
        public int CardExpiryMonth { get; set; }
        public int CardExpiryYear { get; set; }
    }

    public class AccountPaymentVM
    {
        public string operationID { get; set; }
        public string paymentID { get; set; }
        public string vendorID { get; set; }
        public string buyerID { get; set; }
        public string amount { get; set; }
        public string paymentDate { get; set; }
        public string status { get; set; }
        public string paymentMethod { get; set; }
        public string paymentInstrumentID { get; set; }
        public string payType { get; set; }
        public string description { get; set; }
    }
    public class AccountPaymentVMData : DataTableModel
    {
        public List<AccountPaymentVM> aaData;
    }

    public class AccountUsage
    {
        [Display(Name = "Unbilled Amount")]
        public string UnbilledAmount { get; set; }
        [Display(Name = "Amount Due")]
        public string AmountDue { get; set; }
        [Display(Name = "Monthly Payments")]
        public string MonthlyPayments { get; set; }
        [Display(Name = "Remaining Deposit")]
        public string RemainingDeposit { get; set; }
        [Display(Name = "Data Expiration")]
        public string DataExpiration { get; set; }
        [Display(Name = "Contract Period")]
        public string ContractPeriod { get; set; }
        [Display(Name = "Data AddOn")]
        public string DataAddOn { get; set; }
        [Display(Name = "Data SMS")]
        public string DataSMS { get; set; }
        [Display(Name = "Deposit")]
        public string Deposit { get; set; }
        [Display(Name = "SignUpChannel")]
        public string SignUpChannel { get; set; }
        [Display(Name = "Original Billing Date")]
        public string OriginalBillingDate { get; set; }
        [Display(Name = "Previous Balance")]
        public string previousBalance { get; set; }

        [Display(Name = "Activation Date")]
        public string effectiveBillingDate { get; set; }


        [Display(Name = "Next Cycle Billing Date")]
        public string NextCycleBillingDate { get; set; }
    }
    
}