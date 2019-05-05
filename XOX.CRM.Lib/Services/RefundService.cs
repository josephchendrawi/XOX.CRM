using CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib
{
    public class RefundService : IRefundService
    {
        public IAccountService AccountService = new AccountService();
        public IOrderService OrderService = new OrderService();

        public long Add(AddRefundVO AddRefundVO, long UserId = 0)
        {
            using (var DBContext = new CRMDbContext())
            {
                long AccountId = 0;
                try
                {
                    AccountId = AccountService.GetIDByMSISDN(AddRefundVO.MSISDN, AddRefundVO.Name);
                }
                catch { }

                if (AccountId != 0)
                {
                    long OrderId = 0;

                    var Account = AccountService.Get(AccountId);

                    OrderId = OrderService.GetOrderIdByAccountId(AccountId, "New Registration");

                    if (OrderId == 0)
                    {
                        OrderId = OrderService.GetOrderIdByAccountId(AccountId, "Supplementary Registration");
                    }

                    if (OrderId != 0)
                    {
                        var Order = OrderService.Get(OrderId);

                        XOX_T_ACCNT_REFUND a = new XOX_T_ACCNT_REFUND();
                        a.CREATED = DateTime.Now;
                        a.CREATED_BY = UserId;

                        a.NAME = AddRefundVO.Name;
                        a.MSISDN = AddRefundVO.MSISDN;
                        a.ADVANCE_PAYMENT = AddRefundVO.AdvancePayment;
                        a.DEPOSIT = AddRefundVO.Deposit;
                        a.BILL_PAYMENT = AddRefundVO.BillPayment;
                        a.USAGE = AddRefundVO.Usage;
                        a.REFUND_DATE = AddRefundVO.RefundDate;

                        a.ACCNT_ID = AccountId;
                        a.PLAN = Order.SubscriptionPlan;
                        a.STATUS = 1;
                        a.TERMINATION_DATE = Account.TerminationDate;
                        a.TOTAL_REFUND = ((a.DEPOSIT + a.ADVANCE_PAYMENT + a.BILL_PAYMENT) - a.USAGE);

                        DBContext.XOX_T_ACCNT_REFUND.Add(a);
                        DBContext.SaveChanges();

                        return a.ROW_ID;

                    }
                    else { }
                }
                else { }
            }

            return 0;
        }
                
        public List<RefundVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", DateTime? From = null, DateTime? To = null)
        {
            List<RefundVO> List = new List<RefundVO>();
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_ACCNT_REFUND
                             select d;

                //filtering
                if (From != null && To != null)
                {
                    To = To.Value.AddDays(1);
                    result = result.Where(m => m.REFUND_DATE >= From && m.REFUND_DATE < To);
                }

                TotalCount = result.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "Name")
                        result = result.OrderBy(m => m.NAME);
                    else if (orderBy == "MSISDN")
                        result = result.OrderBy(m => m.MSISDN);
                    else if (orderBy == "PlanType")
                        result = result.OrderBy(m => m.PLAN);
                    else if (orderBy == "TerminationDate")
                        result = result.OrderBy(m => m.TERMINATION_DATE);
                    else if (orderBy == "Deposit")
                        result = result.OrderBy(m => m.DEPOSIT);
                    else if (orderBy == "AdvancePayment")
                        result = result.OrderBy(m => m.ADVANCE_PAYMENT);
                    else if (orderBy == "BillPayment")
                        result = result.OrderBy(m => m.BILL_PAYMENT);
                    else if (orderBy == "Usage")
                        result = result.OrderBy(m => m.USAGE);
                    else if (orderBy == "TotatRefund")
                        result = result.OrderBy(m => m.TOTAL_REFUND);
                    else if (orderBy == "RefundDate")
                        result = result.OrderBy(m => m.REFUND_DATE);
                    else
                        result = result.OrderBy(m => m.ROW_ID);
                }
                else
                {
                    if (orderBy == "Name")
                        result = result.OrderByDescending(m => m.NAME);
                    else if (orderBy == "MSISDN")
                        result = result.OrderByDescending(m => m.MSISDN);
                    else if (orderBy == "PlanType")
                        result = result.OrderByDescending(m => m.PLAN);
                    else if (orderBy == "TerminationDate")
                        result = result.OrderByDescending(m => m.TERMINATION_DATE);
                    else if (orderBy == "Deposit")
                        result = result.OrderByDescending(m => m.DEPOSIT);
                    else if (orderBy == "AdvancePayment")
                        result = result.OrderByDescending(m => m.ADVANCE_PAYMENT);
                    else if (orderBy == "BillPayment")
                        result = result.OrderByDescending(m => m.BILL_PAYMENT);
                    else if (orderBy == "Usage")
                        result = result.OrderByDescending(m => m.USAGE);
                    else if (orderBy == "TotatRefund")
                        result = result.OrderByDescending(m => m.TOTAL_REFUND);
                    else if (orderBy == "RefundDate")
                        result = result.OrderByDescending(m => m.REFUND_DATE);
                    else
                        result = result.OrderByDescending(m => m.ROW_ID);
                }

                if (length >= 0)
                    result = result.Skip(startIdx).Take(length);

                foreach (var v in result)
                {
                    List.Add(new RefundVO()
                    {
                        ACCNT_ID = v.ACCNT_ID,
                        ADVANCE_PAYMENT = v.ADVANCE_PAYMENT,
                        BILL_PAYMENT = v.BILL_PAYMENT,
                        CREATED = v.CREATED,
                        DEPOSIT = v.DEPOSIT,
                        MSISDN = v.MSISDN,
                        NAME = v.NAME,
                        PLAN = v.PLAN,
                        REFUND_DATE = v.REFUND_DATE,
                        REMARKS = v.REMARKS,
                        ROW_ID = v.ROW_ID,
                        STATUS = v.STATUS,
                        TERMINATION_DATE = v.TERMINATION_DATE,
                        TOTAL_REFUND = v.TOTAL_REFUND,
                        USAGE = v.USAGE,
                    });
                }
            }

            return List;
        }

    }
}
