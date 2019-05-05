using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.API.ServiceModel.Types;

namespace XOX.CRM.API.ServiceModel
{
    [Route("/account/add", "POST")]
    public class AccountAdd : IReturn<LongResponse>
    {
        public PersonalInfo PersonalInfo { get; set; }
        public BankingInfo BankingInfo { get; set; }
        public AddressInfo AddressInfo { get; set; }
        public AddressInfo BillingAddressInfo { get; set; }
        public long IntegrationId { get; set; }
        public string SIMSerialNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

    [Route("/account/files/add", "POST")]
    [Route("/account/{AccountId}/files/add", "POST")]
    public class AccountAddFiles : IReturn<BoolResponse>
    {
        public int AccountId { get; set; }
        public List<File> Files { get; set; }
    }

    [Route("/account/supplementary/add", "POST")]
    public class AccountSupplementaryAdd : IReturn<LongResponse>
    {
        public string MainAccountName { get; set; }
        public string MSISDN { get; set; }
        public PersonalInfo PersonalInfo { get; set; }
        public BankingInfo BankingInfo { get; set; }
        public AddressInfo AddressInfo { get; set; }
        public long AccountId { get; set; }
        public string SIMSerialNumber { get; set; }
        public long IntegrationId { get; set; }
    }

    [Route("/account/integration-id/check")]
    public class AccountIntegrationIdCheck : IReturn<LongResponse>
    {
        public long IntegrationId { get; set; }
    }

    [Route("/account/create-payment")]
    public class AccountCreatePayment : IReturn<BoolResponse>
    {
        public List<PaymentDetails> Payment { get; set; }
    }

    public class PaymentDetails
    {
        public long IntegrationId { get; set; }
        public decimal AdvancePayment { get; set; }
        public decimal Deposit { get; set; }
        public decimal ForeignDeposit { get; set; }
        public string Reference { get; set; }
        public bool SupplementaryLine { get; set; }
    }

    [Route("/account/redemption-payment")]
    public class AccountRedemptionPayment : IReturn<ObjResponse>
    {
        public long IntegrationId { get; set; }
        public decimal Amount { get; set; }
        public bool isSupplementaryLine { get; set; }
        public string AuthenticationKey { get; set; }
    }

    [Route("/account/crp/make-payment")]
    public class AccountCRPMakePayment : IReturn<ObjResponse>
    {
        public long IntegrationId { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public bool isSupplementaryLine { get; set; }
        public string AuthenticationKey { get; set; }
    }

}
