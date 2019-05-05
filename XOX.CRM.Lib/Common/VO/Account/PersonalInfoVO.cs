using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class PersonalInfoVO
    {
        public string FullName { get; set; }
        public int Salutation { get; set; }
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
        public DateTime? CreatedDate { get; set; }
        public string SponsorPersonnel { get; set; }
        public string CustomerAccountNumber { get; set; }
        public string MSISDNNumber { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
