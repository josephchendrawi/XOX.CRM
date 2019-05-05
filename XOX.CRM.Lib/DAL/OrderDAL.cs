using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib.DAL
{
    public class OrderDAL
    {
        public XOX_T_ORDER GetOrder(long orderId)
        {
            using (var dbContext = new CRMDbContext())
            {
                var order = (from x in dbContext.XOX_T_ORDER
                             where x.ROW_ID == orderId
                             select x).FirstOrDefault();
                return order;
            }
        }

    }
}
