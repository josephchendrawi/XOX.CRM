using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class AssetVO
    {
        public long AssetId { get; set; }
        public string Status { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Plan { get; set; }
        public bool PrintedBilling { get; set; }
        public long AccountId { get; set; }
        public string SubscriberName { get; set; }
        public string MSISDN { get; set; }
    }
}
