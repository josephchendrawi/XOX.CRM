using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class BankingInfoVO
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
        public bool PrintedBillingFlg { get; set; }
    }
}
