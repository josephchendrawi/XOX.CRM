using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class AccountActivityVO
    {
        public long ROW_ID { get; set; }

        public long? CREATED_BY { get; set; }

        public DateTime? CREATED { get; set; }

        public long? LAST_UPD_BY { get; set; }

        public DateTime? LAST_UPD { get; set; }

        public long ACCNT_ID { get; set; }
        public string ACT_DESC { get; set; }
        public string ASSIGNEE { get; set; }

        public string STATUS { get; set; }
        public string REASON { get; set; }
    }
}
