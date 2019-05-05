using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class AccountVO
    {
        public long AccountId { get; set; }
        public PersonalInfoVO PersonalInfo { get; set; }
        public BankingInfoVO BankingInfo { get; set; }
        public AddressInfoVO AddressInfo { get; set; }
        public AddressInfoVO BillingAddressInfo { get; set; }
        public List<FileVO> Files { get; set; }
        public int AccountType { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public int Grade { get; set; }
        public string DealerCode { get; set; }
        public string SIMSerialNumber { get; set; }

        public long? ParentAccountId { get; set; }
    }

    public class AccountPaymentCardDetail
    {
        public string MSISDN { get; set; }
        public string Name { get; set; }
        public string AccountStatus { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string CardIssuerBank { get; set; }
        public int CardExpiryMonth { get; set; }
        public int CardExpiryYear { get; set; }
        public string CardType { get; set; }
    }
}
