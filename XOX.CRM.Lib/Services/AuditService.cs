using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.VO.Audit;
using XOX.CRM.Lib.DAL;
using XOX.CRM.Lib.DBContext;
using XOX.CRM.Lib.Interfaces;

namespace XOX.CRM.Lib.Services
{
    public class AuditService : IAuditService
    {
        private AuditDAL AuditDAL = new AuditDAL();

        private long _auditUserId;
        private string _auditModule;
        private string _auditAction;

        public AuditService(long userId, string module, string action)
        {
            _auditUserId = userId;
            _auditModule = module;
            _auditAction = action;
        }

        public void Check(ObjectStateEntry objectState, long auditRecordId)
        {
            IEnumerable<string> modifiedProperties = objectState.GetModifiedProperties();
            foreach (var item in modifiedProperties)
            {
                if (item == "CREATED" || item == "CREATED_BY" || item == "LAST_UPD_BY" || item == "LAST_UPD" || item == "CUSTOMER_NUM")
                    continue;

                AuditTrailVO auditTrail = new AuditTrailVO();
                auditTrail.CREATED_BY = _auditUserId;
                auditTrail.MODULE_NAME = _auditModule;
                auditTrail.AUDIT_ROW_ID = auditRecordId;
                auditTrail.ACTION_CD = _auditAction;
                auditTrail.OLD_VAL = objectState.OriginalValues[item].ToString();
                auditTrail.NEW_VAL = objectState.CurrentValues[item].ToString();
                auditTrail.FIELD_NAME = item;
                AuditDAL.Add(auditTrail);
            }
        }

        public List<AuditTrailVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", AuditTrailVO filterby = null)
        {
            List<AuditTrailVO> list = new List<AuditTrailVO>();
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_AUDIT_TRAIL
                             join e in DBContext.XOX_T_USER on d.CREATED_BY equals e.ROW_ID
                             select new
                             {
                                 ROW_ID = d.ROW_ID,
                                 ACTION_CD = d.ACTION_CD,
                                 OLD_VAL = d.OLD_VAL,
                                 NEW_VAL = d.NEW_VAL,
                                 CREATED = d.CREATED,
                                 CREATED_BY = d.CREATED_BY,
                                 MODULE_NAME = d.MODULE_NAME,
                                 FIELD_NAME = d.FIELD_NAME,
                                 CREATED_BY_USERNAME = e.USERNAME,
                             };

                //filtering
                if (filterby.CreatedByUserName != null && filterby.CreatedByUserName != "")
                {
                    result = result.Where(m => m.CREATED_BY_USERNAME == filterby.CreatedByUserName);
                }
                if (filterby.MODULE_NAME != null && filterby.MODULE_NAME != "")
                {
                    result = result.Where(m => m.MODULE_NAME == filterby.MODULE_NAME);
                }

                TotalCount = result.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "Action")
                        result = result.OrderBy(m => m.ACTION_CD);
                    else if (orderBy == "OldValue")
                        result = result.OrderBy(m => m.OLD_VAL);
                    else if (orderBy == "NewValue")
                        result = result.OrderBy(m => m.NEW_VAL);
                    else if (orderBy == "Created")
                        result = result.OrderBy(m => m.CREATED);
                    else if (orderBy == "CreatedBy")
                        result = result.OrderBy(m => m.CREATED_BY);
                    else if (orderBy == "Module")
                        result = result.OrderBy(m => m.MODULE_NAME);
                    else if (orderBy == "Field")
                        result = result.OrderBy(m => m.FIELD_NAME);
                    else
                        result = result.OrderBy(m => m.ROW_ID);
                }
                else
                {
                    if (orderBy == "Action")
                        result = result.OrderByDescending(m => m.ACTION_CD);
                    else if (orderBy == "OldValue")
                        result = result.OrderByDescending(m => m.OLD_VAL);
                    else if (orderBy == "NewValue")
                        result = result.OrderByDescending(m => m.NEW_VAL);
                    else if (orderBy == "Created")
                        result = result.OrderByDescending(m => m.CREATED);
                    else if (orderBy == "CreatedBy")
                        result = result.OrderByDescending(m => m.CREATED_BY);
                    else if (orderBy == "Module")
                        result = result.OrderByDescending(m => m.MODULE_NAME);
                    else if (orderBy == "Field")
                        result = result.OrderByDescending(m => m.FIELD_NAME);
                    else
                        result = result.OrderByDescending(m => m.ROW_ID);
                }

                if (length >= 0)
                    result = result.Skip(startIdx).Take(length);

                foreach (var v in result)
                {
                    list.Add(new AuditTrailVO()
                    {
                        ROW_ID = v.ROW_ID,
                        ACTION_CD = v.ACTION_CD,
                        CREATED = v.CREATED,
                        OLD_VAL = v.OLD_VAL,
                        NEW_VAL = v.NEW_VAL,
                        MODULE_NAME = v.MODULE_NAME,
                        FIELD_NAME = v.FIELD_NAME,
                        CREATED_BY = v.CREATED_BY,
                        CreatedByUserName = v.CREATED_BY_USERNAME
                    });
                }
            }

            return list;
        }

        public List<string> GetAllModule()
        {
            List<string> MODULE_NAMEs = new List<string>();
            using (var DBContext = new CRMDbContext())
            {
                var result = (from d in DBContext.XOX_T_AUDIT_TRAIL
                              select d.MODULE_NAME).Distinct();

                foreach (var v in result)
                {
                    MODULE_NAMEs.Add(v);
                }
            }

            return MODULE_NAMEs;
        }

        public void AddAuditLog(AuditLogVO AuditLogVO)
        {
            AuditLogVO.CREATED_BY = _auditUserId;
            AuditLogVO.COMMIT_BY = _auditUserId;

            AuditDAL.AddAuditLog(AuditLogVO);
        }

        public AuditLogVO GetLatestAuditLog(string EventType, long UserId = 0)
        {
            using (var DBContext = new CRMDbContext())
            {
                var AuditLog = from d in DBContext.XOX_T_AUDIT_LOG
                               where d.EVENT_TYPE == EventType
                               && d.COMMIT_BY == UserId
                               orderby d.CREATED descending
                               select d;

                if (AuditLog.Count() > 0)
                {
                    return new AuditLogVO()
                    {
                        EVENT_TYPE = AuditLog.First().EVENT_TYPE,
                        LOG_DETAILS = AuditLog.First().LOG_DETAILS,
                        RECORD_KEY = AuditLog.First().RECORD_KEY
                    };
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
