using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class OrderVO
    {
        public long CustomerAccountId { get; set; }
        public long ROW_ID { get; set; }

        public long? CREATED_BY { get; set; }

        public DateTime? CREATED { get; set; }

        public long? LAST_UPD_BY { get; set; }

        public DateTime? LAST_UPD { get; set; }

        public string ORDER_NUM { get; set; }

        public string ORDER_TYPE { get; set; }

        public string ORDER_STATUS { get; set; }

        public DateTime? ORDER_SUBMIT_DT { get; set; }

        public DateTime? PREF_INSTALL_DT { get; set; }

        public string ASSIGNEE { get; set; }

        public string ORDER_SOURCE { get; set; }

        public string CAMPAIGN_CD { get; set; }

        public string ORDER_SUBMITTED_BY { get; set; }

        public string CUST_REP_ID { get; set; }

        public string PLAN { get; set; }
        public string CATEGORY { get; set; }
        public string REMARKS { get; set; }
    }
}
