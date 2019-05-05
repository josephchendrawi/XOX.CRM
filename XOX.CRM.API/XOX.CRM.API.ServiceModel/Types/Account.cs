using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.API.ServiceModel.Types
{
    public class PersonalInfo
    {
        public string FullName { get; set; }
        public string Salutation { get; set; }
        public string MotherMaidenName { get; set; }
        public int IdentityType { get; set; }
        public string IdentityNo { get; set; }
        public DateTime BirthDate { get; set; }
        public int Nationality { get; set; }
        public string ContactNumber { get; set; }
        public int PreferredLanguage { get; set; }
        public int Race { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public int CustomerStatus { get; set; }
        public int SponsorPersonnel { get; set; }
        public string CustomerAccountNumber { get; set; }
        public string MSISDNNumber { get; set; }
        public decimal CreditLimit { get; set; }
    }

    public class BankingInfo
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public int CardType { get; set; }
        public string CreditCardNo { get; set; }
        public string CardHolderName { get; set; }
        public string CardIssuerBank { get; set; }
        public int CardExpiryMonth { get; set; }
        public int CardExpiryYear { get; set; }
        public bool ThirdPartyFlag { get; set; }
        public string BankAccountName { get; set; }
    }

    public class AddressInfo
    {
        public string AddressLine1 { get; set; }
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
        public string FileURL { get; set; }
        public string Base64 { get; set; }
        public string FileName { get; set; }
    }

}
