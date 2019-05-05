using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOX.CRM.Lib.DBContext;
using XOX.CRM.Lib.WebServices;

namespace XOX.CRM.Lib
{
    public class PaymentCollectedService
    {
        public long Add(PaymentCollectedVO vo)
        {
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_PAYMENT_COLLECTED ett = new XOX_T_PAYMENT_COLLECTED();
                ett.AMOUNT = vo.Amount;
                ett.CHARGE_TYPE = vo.ChargeType;
                ett.DESCRIPTION = vo.Description;
                ett.METHOD = vo.Method;
                ett.MSISDN = vo.MSISDN;
                ett.PAYMENT_DATE = vo.PaymentDate;
                ett.PAYMENT_ID = vo.PaymentId;

                ett.CREATED = DateTime.Now;
                ett.CREATED_BY = 1;

                DBContext.XOX_T_PAYMENT_COLLECTED.Add(ett);
                DBContext.SaveChanges();

                return ett.ROW_ID;
            }
        }

        public decimal GetAmountSum(int? Month, int? Year)
        {
            using (var DBContext = new CRMDbContext())
            {
                var XOX_T_PAYMENT_COLLECTEDs = from d in DBContext.XOX_T_PAYMENT_COLLECTED
                                               where d.PAYMENT_DATE != null
                                               select d;

                if (Month != null && Year != null)
                {
                    XOX_T_PAYMENT_COLLECTEDs = XOX_T_PAYMENT_COLLECTEDs.Where(m => m.PAYMENT_DATE.Value.Month == Month && m.PAYMENT_DATE.Value.Year == Year);
                }

                return XOX_T_PAYMENT_COLLECTEDs.Sum(m => m.AMOUNT) ?? 0;
            }
        }

    }
}
