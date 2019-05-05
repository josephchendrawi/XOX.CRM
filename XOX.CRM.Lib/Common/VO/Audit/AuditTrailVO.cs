using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib.Common.VO.Audit
{
    public class AuditTrailVO
    {
        public long ROW_ID { get; set; }

        public long? CREATED_BY { get; set; }

        public DateTime? CREATED { get; set; }

        public long? LAST_UPD_BY { get; set; }

        public DateTime? LAST_UPD { get; set; }

        public long? AUDIT_ROW_ID { get; set; }

        public string ACTION_CD { get; set; }

        public string OLD_VAL { get; set; }

        public string NEW_VAL { get; set; }

        public string MODULE_NAME { get; set; }

        public string SCREEN_NAME { get; set; }

        public string FIELD_NAME { get; set; }

        public string CreatedByUserName { get; set; }
    }

    public class AuditLogVO
    {
        public long ROW_ID { get; set; }
        public Nullable<long> CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public Nullable<long> LAST_UPD_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPD { get; set; }
        public string ENTITY { get; set; }
        public string EVENT_TYPE { get; set; }
        public Nullable<long> COMMIT_BY { get; set; }
        public Nullable<long> RECORD_KEY { get; set; }
        public string LOG_DETAILS { get; set; }
    }
}
