using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.Common.VO.Audit;
using XOX.CRM.Lib.DBContext;
using XOX.CRM.Lib.Services;
using XOX.CRM.Lib.WebServices;

namespace XOX.CRM.Lib
{
    public class OrderActivityService : IOrderActivityService
    {
        public long AddActivity(OrderActivityVO _newOrderAct)
        {
            if (_newOrderAct.CREATED_BY == 0 || _newOrderAct.CREATED_BY == null)
            {
                _newOrderAct.CREATED_BY = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            long newOrderActivityId = 0;
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_ORDER_ACT orderActivityEntry = new XOX_T_ORDER_ACT
                {
                    CREATED = DateTime.Now,
                    ORDER_ID = _newOrderAct.ORDER_ID,
                    ACT_DESC = _newOrderAct.ACT_DESC,
                    ASSIGNEE = _newOrderAct.ASSIGNEE,
                    ACT_REMARKS = _newOrderAct.ACT_REMARKS,
                    DUE_DATE = _newOrderAct.DUE_DATE,
                    CREATED_BY = _newOrderAct.CREATED_BY,
                    ORDER_STATUS = _newOrderAct.ORDER_STATUS ?? string.Empty
                };
                DBContext.XOX_T_ORDER_ACT.Add(orderActivityEntry);
                DBContext.SaveChanges();

                newOrderActivityId = orderActivityEntry.ROW_ID;

                /*AuditTrailVO auditTrail = new AuditTrailVO();
                auditTrail.CREATED_BY = _newOrderAct.CREATED_BY;
                auditTrail.MODULE_NAME = OhanaConstants.MODULE_ORDER_ACTIVITIES;
                auditTrail.AUDIT_ROW_ID = newOrderActivityId;
                auditTrail.ACTION_CD = "Create";
                AuditTrailDAL.InsertAuditTrail(auditTrail);*/
            }
            return newOrderActivityId;
        }

        public List<OrderActivityVO> GetActivities(long orderId)
        {
            List<OrderActivityVO> orderActivities = new List<OrderActivityVO>();
            using (var DBContext = new CRMDbContext())
            {
                /*
                List<long> orderactivitylist = new List<long>();
                orderactivitylist.Add(orderId);

                var order = from d in DBContext.XOX_T_ORDER
                            where d.ORDER_STATUS == "Resubmitted" && d.ROW_ID == orderId
                            select d;
                if (order.Count() > 0)
                {
                    var AccountId = AccountService.GetAccountIdByOrderId(orderId);

                    var rejectedorders = (from d in DBContext.XOX_T_ORDER_ITEM
                                         join e in DBContext.XOX_T_ORDER on d.ORDER_ID equals e.ROW_ID
                                         where d.CUST_ID == AccountId && e.ORDER_STATUS == "Rejected"
                                         select e.ROW_ID).Distinct();

                    foreach (var v in rejectedorders)
                    {
                        orderactivitylist.Add(v);
                    }
                }       
                */

                var activityList = from x in DBContext.XOX_T_ORDER_ACT
                                   where x.ORDER_ID == orderId
                                   select x;
                if (activityList.Count() > 0)
                    orderActivities = Mapper.OrderMapper.Map(activityList.ToList());
            }
            return orderActivities;
        }

    }
}
