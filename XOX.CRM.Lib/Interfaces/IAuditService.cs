using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.VO.Audit;

namespace XOX.CRM.Lib.Interfaces
{
    public interface IAuditService
    {
        void Check(ObjectStateEntry objectState, long auditRecord);
        List<AuditTrailVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", AuditTrailVO filterby = null);
        List<string> GetAllModule();
        void AddAuditLog(AuditLogVO AuditLogVO);
        AuditLogVO GetLatestAuditLog(string EventType, long UserId = 0);

    }
}
