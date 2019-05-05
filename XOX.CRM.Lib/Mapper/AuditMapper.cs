using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.VO.Audit;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib.Mapper
{
    public static class Audit
    {
        public static AuditTrailVO Map(XOX_T_AUDIT_TRAIL a)
        {
            return new AuditTrailVO()
            {

            };
        }
    }
}
