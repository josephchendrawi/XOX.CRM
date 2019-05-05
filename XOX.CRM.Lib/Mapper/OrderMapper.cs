using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib.Mapper
{
    public static class OrderMapper
    {
        public static OrderActivityVO Map(XOX_T_ORDER_ACT db) 
        {
            return new OrderActivityVO
            {
                ROW_ID = db.ROW_ID,
                CREATED = db.CREATED,
                CREATED_BY = db.CREATED_BY,
                ACT_DESC = db.ACT_DESC,
                ACT_REMARKS = db.ACT_REMARKS,
                ASSIGNEE = db.ASSIGNEE == null ? "" : db.ASSIGNEE,
                DUE_DATE = db.DUE_DATE,
                ORDER_ID = db.ORDER_ID ?? 0,
                ORDER_STATUS = db.ORDER_STATUS == null ? "" : db.ORDER_STATUS,
            };
        }

        public static List<OrderActivityVO> Map(List<XOX_T_ORDER_ACT> dbRecords)
        {
            var result = new List<OrderActivityVO>();
            foreach (var v in dbRecords)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }

    }
}
