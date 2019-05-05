using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib.Mapper
{
    public static class MobileNumMapper
    {
        public static MobileNumVO Map(XOX_T_REV_NUM db) 
        {
            return new MobileNumVO
            {
                BatchNum = db.BATCH_NUM,
                CreatedDate = (DateTime)db.CREATED,
                MobileNumId = db.ROW_ID,
                MSISDN = db.MAIN_NUM,
                Price = (decimal)db.PRICE
            };
        }

        public static List<MobileNumVO> Map(List<XOX_T_REV_NUM> dbRecords)
        {
            var result = new List<MobileNumVO>();
            foreach (var v in dbRecords)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }

    }
}
