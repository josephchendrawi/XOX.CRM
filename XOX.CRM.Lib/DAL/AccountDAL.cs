using CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib.DAL
{
    public class AccountDAL
    {
        #region Insert
        public long AddBillPayment(XOX_T_BILL_PAYMENT billItem)
        {
            using (var dbContext = new CRMDbContext())
            {
                dbContext.XOX_T_BILL_PAYMENT.Add(billItem);
                dbContext.SaveChanges();
            }
            return billItem.ROW_ID;
        }
        #endregion

        public XOX_T_ACCNT GetAccountIDByMsisdnByName(string msisdn, string name)
        {
            XOX_T_ACCNT returnAccountId = null;
            using (var DBContext = new CRMDbContext())
            {
                var PrincipalLineType = ((int)AccountType.PrincipalLine).ToString();
                var result = from d in DBContext.XOX_T_ACCNT
                             where d.MSISDN == msisdn && d.NAME == name
                             && d.ACCNT_TYPE_CD == PrincipalLineType
                             && d.ACCNT_STATUS != XOXConstants.STATUS_WITHDRAW_CD
                             orderby d.ROW_ID descending
                             select d;

                if (result.Count() == 1)
                {
                    returnAccountId = result.First();
                }
                else if (result.Count() > 0)
                {
                    returnAccountId = result.First();
                    //throw new Exception(XOXExceptions.ACCNT02_DUPLICATE_ACCOUNTS);
                }
                else
                {
                    throw new Exception(XOXExceptions.ACCNT01_MSISDN_NOT_FOUND);
                }
            }
            return returnAccountId;
        }
    }
}
