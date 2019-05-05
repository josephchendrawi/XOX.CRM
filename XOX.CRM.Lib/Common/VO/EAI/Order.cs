using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class OrderReject : Request
    {
        public int UserId { get; set; }
        public string Reason { get; set; }
        public long OrderId { get; set; }
        public string Pass { get; set; }
        public List<AccountSupplementary> List { get; set; }
        public bool isSuppLine { get; set; }
        public long AccountId { get; set; }
    }

    public class AccountEdit
    {
        public string Pass { get; set; }
        public AccountInfo Data { get; set; }
        public bool isSupplementary { get; set; }
    }

    public class AccountInfo
    {
        public User User { get; set; }
        public PersonalInfoEAI PersonalInfo { get; set; }
        public BankingInfoVO BankingInfo { get; set; }
        public AddressInfoVO AddressInfo { get; set; }
        public AddressInfoVO BillingAddressInfo { get; set; }

        public string SIMSerialNumber { get; set; }
        public long IntegrationId { get; set; }
        
        public int FlgReceivedItemised { get; set; }
    }

    public class PersonalInfoEAI
    {
        public string FullName { get; set; }
        public string Salutation { get; set; }
        public string MotherMaidenName { get; set; }
        public int IdentityType { get; set; }
        public string IdentityNo { get; set; }
        public string BirthDate { get; set; }
        public int Nationality { get; set; }
        public string ContactNumber { get; set; }
        public int PreferredLanguage { get; set; }
        public int Race { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public int CustomerStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string SponsorPersonnel { get; set; }
        public string CustomerAccountNumber { get; set; }
        public string MSISDNNumber { get; set; }
        public decimal CreditLimit { get; set; }
        public string SimSerialNumber { get; set; }
    }

    public class OrderUpdateStatus
    {
        public User User { get; set; }
        public long IntegrationId { get; set; }
        public string OrderStatus { get; set; }
        public bool isSuppLine { get; set; }
    }

    public class AccountUpdateStatus
    {
        public User User { get; set; }
        public long IntegrationId { get; set; }
        public string AccountStatus { get; set; }
        public string SubscriberJoinDate { get; set; }
        public bool isSuppLine { get; set; }
        public string DeleteDate { get; set; }
    }

    public class AssignMSISDN
    {
        public List<AssignData> List { get; set; }
        public User User { get; set; }
        public string Pass { get; set; }
    }

    public class AssignData
    {
        public string MSISDN { get; set; }
        public string Members { get; set; }
        public decimal Price { get; set; }
    }

    public class AccountSupplementary
    {
        public long AccountId { get; set; }
        public string MSISDN { get; set; }
        public string Name { get; set; }
    }

    public class CRPAddPayment : Request
    {
        public long PaymentId { get; set; }
        public string MSISDN { get; set; }
        public decimal Amount { get; set; }
        public int ChargeType { get; set; }
        public string Desc { get; set; }
        public string PaymentDate { get; set; }
        public string Method { get; set; }
    }
}
