using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.VO.Audit;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib.DAL
{
    public class AuditDAL
    {
        public void Add(AuditTrailVO auditTrailVO)
        {
            auditTrailVO.CREATED = DateTime.Now;
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_AUDIT_TRAIL auditRecord = new XOX_T_AUDIT_TRAIL();
                auditRecord.ACTION_CD = auditTrailVO.ACTION_CD;
                auditRecord.AUDIT_ROW_ID = auditTrailVO.AUDIT_ROW_ID;
                auditRecord.CREATED = auditTrailVO.CREATED;
                auditRecord.CREATED_BY = auditTrailVO.CREATED_BY;
                auditRecord.FIELD_NAME = auditTrailVO.FIELD_NAME;
                auditRecord.MODULE_NAME = auditTrailVO.MODULE_NAME;
                auditRecord.NEW_VAL = auditTrailVO.NEW_VAL;
                auditRecord.OLD_VAL = auditTrailVO.OLD_VAL;
                auditTrailVO.SCREEN_NAME = auditTrailVO.SCREEN_NAME;

                DBContext.XOX_T_AUDIT_TRAIL.Add(auditRecord);
                DBContext.SaveChanges();
            }
        }
        public void AddAuditLog(AuditLogVO AuditLogVO)
        {
            AuditLogVO.CREATED = DateTime.Now;
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_AUDIT_LOG auditRecord = new XOX_T_AUDIT_LOG();
                auditRecord.EVENT_TYPE = AuditLogVO.EVENT_TYPE;
                auditRecord.CREATED = AuditLogVO.CREATED;
                auditRecord.CREATED_BY = AuditLogVO.CREATED_BY;
                auditRecord.COMMIT_BY = AuditLogVO.COMMIT_BY;
                auditRecord.ENTITY = AuditLogVO.ENTITY;
                auditRecord.LOG_DETAILS = AuditLogVO.LOG_DETAILS;
                auditRecord.RECORD_KEY = AuditLogVO.RECORD_KEY;

                DBContext.XOX_T_AUDIT_LOG.Add(auditRecord);
                DBContext.SaveChanges();
            }
        }
    }
}
