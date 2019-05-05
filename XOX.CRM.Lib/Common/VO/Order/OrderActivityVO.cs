using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class OrderActivityVO
    {
        public long ROW_ID { get; set; }

        public long? CREATED_BY { get; set; }

        public DateTime? CREATED { get; set; }

        public long? LAST_UPD_BY { get; set; }

        public DateTime? LAST_UPD { get; set; }

        public long ORDER_ID { get; set; }
        public string ACT_DESC { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public string ASSIGNEE { get; set; }

        public string ACT_REMARKS { get; set; }
        public string REJECTED_REASON { get; set; }
        public string ORDER_STATUS { get; set; }
    }
}
