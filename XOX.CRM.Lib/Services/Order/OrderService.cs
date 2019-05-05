using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOX.CRM.Lib.DBContext;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.WebServices;
using CRM;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace XOX.CRM.Lib
{
    public class OrderService : IOrderService
    {
        private IProductService productManager;
        private IAccountService AccountService;
        private IAddressService AddressService;
        private ICommonService CommonService;
        private IOrderActivityService OrderActivityService;
        private IAssetService AssetService;
        private IUserService UserService;
        public OrderService()
        {
            productManager = new ProductService();
            AccountService = new AccountService();
            AddressService = new AddressService();
            CommonService = new CommonService();
            OrderActivityService = new OrderActivityService();
            AssetService = new AssetService();
            UserService = new UserService();
        }

        public long CreateNewOrder(OrderVO _newOrder, List<long> _selectedProducts, string MSISDN, string SubmitBy, bool flgResubmitted = false, bool flgTermination = false)
        {
            #region Perform Account Checking
            using (var dbContext = new CRMDbContext())
            {
                var Account = from d in dbContext.XOX_T_ACCNT
                              where d.ROW_ID == _newOrder.CustomerAccountId
                              select d.ROW_ID;

                if (Account.Count() <= 0)
                {
                    throw new Exception("Account Id not found.");
                }

                //find CUST_REP_ID if _newOrder.CUST_REP_ID is in MSISDN
                var CustomerRepId = from d in dbContext.XOX_T_ACCNT
                                    where d.MSISDN == _newOrder.CUST_REP_ID
                                    select d.ROW_ID;

                if (CustomerRepId.Count() > 0)
                {
                    _newOrder.CUST_REP_ID = CustomerRepId.First().ToString();
                }
                else
                {
                    //_newOrder.CUST_REP_ID = "0";
                }
            }
            #endregion

            long newOrderId = this.AddOrderHeader(_newOrder, SubmitBy, flgResubmitted);
            long mainOrderItemId = 0;

            List<ProductVO> productList = productManager.GetAllProducts();
            productList = (from x in productList
                           where _selectedProducts.Contains(x.ROW_ID)
                           select x).ToList();

            #region Create Main MOLI
            List<KeyValuePair<long, long>> createdOrderParItemIds = new List<KeyValuePair<long, long>>();
            List<KeyValuePair<long, long>> createdOrderRootItemIds = new List<KeyValuePair<long, long>>();
            var productRefList = (from x in productList
                                  where x.PRD_LVL == XOXConstants.PRODUCT_LEVEL_MOLI
                                  select x).ToList();

            foreach (var pid in productRefList)
            {
                OrderItemVO orderItemHeader = new OrderItemVO();
                orderItemHeader.PROD_ID = pid.ROW_ID;
                orderItemHeader.ORDER_ID = newOrderId;
                orderItemHeader.STATUS_CD = XOXConstants.STATUS_ACTIVE_CD;
                orderItemHeader.CUST_ID = _newOrder.CustomerAccountId;
                orderItemHeader.CREATED_BY = _newOrder.CREATED_BY;
                orderItemHeader.CREATED = DateTime.Now;
                if (flgTermination == false)
                {
                    orderItemHeader.ACTION_TYPE = XOXConstants.ACTION_TYPE_ADD;
                }
                else
                {
                    orderItemHeader.ACTION_TYPE = XOXConstants.ACTION_TYPE_DELETE;
                }
                orderItemHeader.QTY = 1;
                mainOrderItemId = this.AddOrderItem(orderItemHeader, true);
                KeyValuePair<long, long> lookupPair = new KeyValuePair<long, long>(pid.ROW_ID, mainOrderItemId);
                createdOrderParItemIds.Add(lookupPair);
                KeyValuePair<long, long> lookupPair2 = new KeyValuePair<long, long>(pid.ROOT_ITEM_ID, mainOrderItemId);
                createdOrderRootItemIds.Add(lookupPair2);
            }
            #endregion

            #region Create OLI
            productList = (from x in productList
                           where _selectedProducts.Contains(x.ROW_ID) &&
                           x.PRD_LVL == XOXConstants.PRODUCT_LEVEL_OLI
                           select x).ToList();
            foreach (ProductVO pVo in productList)
            {
                //Serch Parent order item
                long parItemId = (from x in createdOrderParItemIds
                                  where x.Key == pVo.PAR_ITEM_ID
                                  select x.Value).FirstOrDefault();
                long rootItemId = (from x in createdOrderRootItemIds
                                   where x.Key == pVo.ROOT_ITEM_ID
                                   select x.Value).FirstOrDefault();
                long subOrderItem = 0;
                if (parItemId > 0 && rootItemId > 0)
                {
                    OrderItemVO newOrderItem = new OrderItemVO();
                    newOrderItem.PROD_ID = pVo.ROW_ID;
                    newOrderItem.ORDER_ID = newOrderId;
                    newOrderItem.STATUS_CD = XOXConstants.STATUS_ACTIVE_CD;
                    newOrderItem.CUST_ID = _newOrder.CustomerAccountId;
                    newOrderItem.CREATED_BY = long.Parse(_newOrder.CUST_REP_ID);
                    if (flgTermination == false)
                    {
                        newOrderItem.ACTION_TYPE = XOXConstants.ACTION_TYPE_ADD;
                    }
                    else
                    {
                        newOrderItem.ACTION_TYPE = XOXConstants.ACTION_TYPE_DELETE;
                    }
                    newOrderItem.PAR_ORDER_ITEM_ID = parItemId;
                    newOrderItem.ROOT_ORDER_ITEM_ID = rootItemId;
                    newOrderItem.QTY = 1;
                    if (pVo.PRD_PRICE_TYPE == "Recurring" && pVo.PRD_PRICE > 0)
                    {
                        newOrderItem.SERVICE_NUM = MSISDN;
                    }
                    subOrderItem = this.AddOrderItem(newOrderItem, false);
                }
            }
            #endregion

            return newOrderId;
        }
        private long AddOrderHeader(OrderVO _newOrder, string SubmitBy, bool flgResubmitted = false)
        {
            long newOrderId = 0;
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_ORDER orderHeader = new XOX_T_ORDER();
                orderHeader.CREATED = DateTime.Now;
                orderHeader.CREATED_BY = _newOrder.CREATED_BY;
                orderHeader.ASSIGNEE = _newOrder.ASSIGNEE;
                orderHeader.ORDER_SOURCE = _newOrder.ORDER_SOURCE;
                orderHeader.ORDER_TYPE = _newOrder.ORDER_TYPE;
                orderHeader.ORDER_NUM = "TEMP"; //Temp Value Before Generating Number.
                orderHeader.CUST_REP_ID = _newOrder.CUST_REP_ID;
                orderHeader.CATEGORY = _newOrder.CATEGORY;
                orderHeader.PLAN = _newOrder.PLAN;
                orderHeader.REMARKS = _newOrder.REMARKS;
                orderHeader.ORDER_SUBMIT_DT = _newOrder.ORDER_SUBMIT_DT;
                orderHeader.ORDER_SUBMITTED_BY = _newOrder.ORDER_SUBMITTED_BY;
                orderHeader.PREF_INSTALL_DT = _newOrder.PREF_INSTALL_DT;
                orderHeader.ORDER_STATUS = _newOrder.ORDER_STATUS == "0" ? "Terminated" : "New";
                if (flgResubmitted == true)
                {
                    orderHeader.ORDER_STATUS = "Resubmitted";
                }
                orderHeader.VERIFICATION_ID = _newOrder.CustomerAccountId;

                DBContext.XOX_T_ORDER.Add(orderHeader);
                DBContext.SaveChanges();
                newOrderId = orderHeader.ROW_ID;
                //Generate number for ORDER_NUM
                orderHeader.ORDER_NUM = "P" + newOrderId.ToString().PadLeft(8, '0');
                DBContext.SaveChanges();

                OrderActivityVO Activity = new OrderActivityVO();
                Activity.ACT_DESC = "CRP - Order submitted by " + SubmitBy;
                Activity.DUE_DATE = DateTime.Now; ///
                Activity.CREATED_BY = 1;
                Activity.ACT_REMARKS = "";
                Activity.REJECTED_REASON = "";
                Activity.ASSIGNEE = "abc@abc.com"; ///
                Activity.ORDER_ID = newOrderId;
                Activity.ORDER_STATUS = orderHeader.ORDER_STATUS;
                OrderActivityService.AddActivity(Activity);

                /* //Audit Trail
                AuditTrailVO auditTrail = new AuditTrailVO();
                auditTrail.CREATED_BY = _newOrder.CREATED_BY;
                auditTrail.MODULE_NAME = OhanaConstants.MODULE_ORDER;
                auditTrail.AUDIT_ROW_ID = newOrderId;
                auditTrail.ACTION_CD = "Create";
                AuditTrailDAL.InsertAuditTrail(auditTrail); */
            }
            return newOrderId;
        }
        private long AddOrderItem(OrderItemVO _newOrderItem, bool isRoot)
        {
            long orderItemId = 0;
            using (var dbContext = new CRMDbContext())
            {
                XOX_T_ORDER_ITEM orderItem = new XOX_T_ORDER_ITEM();
                orderItem.CREATED = DateTime.Now;
                orderItem.CREATED_BY = _newOrderItem.CREATED_BY;
                orderItem.ORDER_ASSET_NUM = Guid.NewGuid().ToString();
                orderItem.PROD_ID = _newOrderItem.PROD_ID;
                orderItem.ORDER_ID = _newOrderItem.ORDER_ID;
                orderItem.STATUS_CD = _newOrderItem.STATUS_CD;
                orderItem.CUST_ID = _newOrderItem.CUST_ID;
                orderItem.BILL_ID = _newOrderItem.BILL_ID;
                orderItem.SVC_AC_ID = _newOrderItem.SVC_AC_ID;
                orderItem.PAR_ORDER_ITEM_ID = _newOrderItem.PAR_ORDER_ITEM_ID;
                orderItem.ROOT_ORDER_ITEM_ID = _newOrderItem.ROOT_ORDER_ITEM_ID;
                orderItem.SERVICE_NUM = _newOrderItem.SERVICE_NUM;
                orderItem.QTY = _newOrderItem.QTY;
                orderItem.ACTION_TYPE = _newOrderItem.ACTION_TYPE;
                dbContext.XOX_T_ORDER_ITEM.Add(orderItem);
                dbContext.SaveChanges();
                orderItemId = orderItem.ROW_ID;

                _newOrderItem.ROW_ID = orderItemId;
                _newOrderItem.CREATED = orderItem.CREATED;
                if (isRoot)
                {
                    _newOrderItem.ROOT_ORDER_ITEM_ID = orderItemId;
                }
                this.UpdateOrderItem(_newOrderItem);
            }
            return orderItemId;
        }

        private void UpdateOrderItem(OrderItemVO orderItem)
        {
            using (var DBContext = new CRMDbContext())
            {
                var targetOrderItem = DBContext.XOX_T_ORDER_ITEM.FirstOrDefault(o => o.ROW_ID == orderItem.ROW_ID);
                XOX_T_ORDER_ITEM oOrderItem = new XOX_T_ORDER_ITEM
                {
                    CREATED = orderItem.CREATED,
                    CREATED_BY = orderItem.CREATED_BY,
                    LAST_UPD = DateTime.Now,
                    PROD_ID = orderItem.PROD_ID,
                    ORDER_ID = orderItem.ORDER_ID,
                    ORDER_ASSET_NUM = orderItem.ORDER_ASSET_NUM,
                    CUST_ID = orderItem.CUST_ID,
                    PAR_ORDER_ITEM_ID = orderItem.PAR_ORDER_ITEM_ID,
                    ROOT_ORDER_ITEM_ID = orderItem.ROOT_ORDER_ITEM_ID,
                    ACTION_TYPE = orderItem.ACTION_TYPE,
                    STATUS_CD = orderItem.STATUS_CD,
                    SERVICE_NUM = orderItem.SERVICE_NUM,
                    INTEGRATION_ID = orderItem.INTEGRATION_ID,
                    INSTALL_DT = orderItem.INSTALL_DT,
                    QTY = orderItem.QTY
                };

                //DBContext.ChangeTracker.DetectChanges();
                //var objectState = ((IObjectContextAdapter)DBContext).ObjectContext.ObjectStateManager.GetObjectStateEntry(targetOrderItem);
                //var modifiedProperties = objectState.GetModifiedProperties();
                DBContext.SaveChanges();
            }
        }

        public OrderDetailVO Get(long OrderId)
        {
            using (var dbContext = new CRMDbContext())
            {
                var ORDER = from d in dbContext.XOX_T_ORDER
                            where d.ROW_ID == OrderId
                            select d;

                if (ORDER.Count() > 0)
                {
                    var order = ORDER.First();

                    var ITEM = from d in dbContext.XOX_T_ORDER_ITEM
                               where d.ORDER_ID == OrderId
                               select d;

                    long AccountId = (long)ITEM.First().CUST_ID;

                    OrderDetailVO e = new OrderDetailVO();
                    e.OrderId = order.ROW_ID;
                    e.SubscriptionPlan = order.PLAN;
                    e.OrderStatus = order.ORDER_STATUS;
                    e.Category = order.CATEGORY;
                    e.SubmissionDate = order.CREATED;
                    e.OrderNum = order.ORDER_NUM;
                    e.Remarks = order.REMARKS;
                    e.OrderType = order.ORDER_TYPE;
                    e.PortReqFormId = order.PORT_REQ_FORM;

                    e.ChangePlanEffectiveDate = order.PREF_INSTALL_DT;

                    e.PaymentCollected = "";

                    e.OrderPayment = new OrderPaymentVO();
                    //Principal Payment Collected
                    var Payment = from d in dbContext.XOX_T_ACCNT_PAYMENT
                                  where d.ACCNT_ID == AccountId
                                  select d;
                    decimal PrincipalPayment = 0;
                    foreach (var v in Payment)
                    {
                        if (v.PAYMENT_TYPE != XOXConstants.PAYMENT_TYPE_BILLING)
                        {
                            PrincipalPayment += (v.AMOUNT == null ? 0 : (decimal)v.AMOUNT);
                        }

                        //add to OrderPaymentVO
                        if (v.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_DEPOSIT)
                        {
                            e.OrderPayment.Deposit = v.AMOUNT == null ? 0 : (decimal)v.AMOUNT;
                            e.OrderPayment.Reference = v.REFERENCE.Substring(v.REFERENCE.IndexOf(" - ") + 3);
                        }
                        else if (v.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_ADVANCE_PAYMENT)
                        {
                            e.OrderPayment.AdvancePayment = v.AMOUNT == null ? 0 : (decimal)v.AMOUNT;
                            e.OrderPayment.Reference = v.REFERENCE.Substring(v.REFERENCE.IndexOf(" - ") + 3);
                        }
                        else if (v.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_FOREIGN_DEPOSIT)
                        {
                            e.OrderPayment.ForeignDeposit = v.AMOUNT == null ? 0 : (decimal)v.AMOUNT;
                            e.OrderPayment.Reference = v.REFERENCE.Substring(v.REFERENCE.IndexOf(" - ") + 3);
                        }
                    }
                    e.PaymentCollected = "RM " + PrincipalPayment;

                    e.OrderSLPayment = new List<OrderPaymentVO>();
                    //Suppline Payment Collected
                    List<string> SuppLineCollected = new List<string>();
                    foreach (var v in AccountService.GetAllSupplementaryLine(AccountId))
                    {
                        var OrderPaymentVO = new OrderPaymentVO();
                        OrderPaymentVO.AccountId = v.AccountId;

                        var SLPayment = from d in dbContext.XOX_T_ACCNT_PAYMENT
                                        where d.ACCNT_ID == v.AccountId
                                        select d;
                        decimal SuppLinePayment = 0;
                        foreach (var y in SLPayment)
                        {
                            if (y.PAYMENT_TYPE != XOXConstants.PAYMENT_TYPE_BILLING)
                            {
                                SuppLinePayment += (y.AMOUNT == null ? 0 : (decimal)y.AMOUNT);
                            }

                            //add to OrderPaymentVO
                            if (y.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_DEPOSIT)
                            {
                                OrderPaymentVO.Deposit = y.AMOUNT == null ? 0 : (decimal)y.AMOUNT;
                            }
                            else if (y.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_ADVANCE_PAYMENT)
                            {
                                OrderPaymentVO.AdvancePayment = y.AMOUNT == null ? 0 : (decimal)y.AMOUNT;
                            }
                            else if (y.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_FOREIGN_DEPOSIT)
                            {
                                OrderPaymentVO.ForeignDeposit = y.AMOUNT == null ? 0 : (decimal)y.AMOUNT;
                            }
                        }
                        SuppLineCollected.Add("RM " + SuppLinePayment);

                        e.OrderSLPayment.Add(OrderPaymentVO);
                    }
                    foreach (var v in SuppLineCollected)
                    {
                        e.PaymentCollected += " + " + v;
                    }

                    //Principal and Total Supp Line
                    if (order.ORDER_TYPE == "Supplementary Registration")
                    {
                        e.PaymentCollected += " (1 supp line)";
                    }
                    else
                    {
                        if (SuppLineCollected.Count() <= 0)
                        {
                            e.PaymentCollected += " (1 principal)";
                        }
                        else if (SuppLineCollected.Count() == 1)
                        {
                            e.PaymentCollected += " (1 principal, 1 supp line)";
                        }
                        else
                        {
                            e.PaymentCollected += " (1 principal, " + SuppLineCollected.Count() + " supp lines)";
                        }
                    }



                    if (order.ORDER_STATUS == "Rejected")
                    {
                        e.Account = new AccountVO();
                        var OrderAudit = from d in dbContext.XOX_T_ORDER_AUDIT
                                         where d.ORDER_ID == OrderId
                                         orderby d.ROW_ID descending
                                         select d;
                        if (OrderAudit.Count() > 0)
                        {
                            var OAudit = OrderAudit.First();

                            e.Account.AccountId = AccountId;
                            e.Account.AddressInfo = new AddressInfoVO()
                            {
                                AddressLine1 = OAudit.ADDR_1,
                                AddressLine2 = OAudit.ADDR_2,
                                City = OAudit.CITY,
                                Country = OAudit.COUNTRY,
                                Postcode = OAudit.POSTCODE,
                                State = OAudit.STATE,
                            };
                            e.Account.BillingAddressInfo = new AddressInfoVO()
                            {
                                AddressLine1 = OAudit.BILL_ADDR_1,
                                AddressLine2 = OAudit.BILL_ADDR_2,
                                City = OAudit.BILL_CITY,
                                Country = OAudit.BILL_COUNTRY,
                                Postcode = OAudit.BILL_POSTCODE,
                                State = OAudit.BILL_STATE,
                            };
                            e.Account.BankingInfo = new BankingInfoVO()
                            {
                                BankAccountName = "",
                                BankAccountNumber = OAudit.BANK_ACCNT_NUMBER,
                                BankName = OAudit.BANK_NAME,
                                CardExpiryMonth = OAudit.BILL_EXPIRY_MONTH == null ? 0 : (int)OAudit.BILL_EXPIRY_MONTH,
                                CardExpiryYear = OAudit.BILL_EXPIRY_YEAR == null ? 0 : (int)OAudit.BILL_EXPIRY_YEAR,
                                CardHolderName = OAudit.BILL_ACCNT_NAME,
                                CardIssuerBank = OAudit.BILL_BANK_ISSUER,
                                CardType = OAudit.BILL_CARD_TYPE == null || OAudit.BILL_CARD_TYPE == "" ? 0 : EnumUtil.ParseEnumInt<CardType>(OAudit.BILL_CARD_TYPE),
                                CreditCardNo = OAudit.BILL_CARD_NUMBER,
                                ThirdPartyFlag = OAudit.BILL_THIRD_PARTY == null ? false : (bool)OAudit.BILL_THIRD_PARTY,
                                PrintedBillingFlg = OAudit.ITEMISED_BILLING == null ? false : (bool)OAudit.ITEMISED_BILLING,
                            };
                            e.Account.PersonalInfo = new PersonalInfoVO()
                            {
                                BirthDate = OAudit.DOB == null ? DateTime.MinValue : (DateTime)OAudit.DOB,
                                ContactNumber = OAudit.MOBILE_NO,
                                Email = OAudit.EMAIL,
                                FullName = OAudit.NAME,
                                Gender = OAudit.GENDER == null ? 0 : (int)OAudit.GENDER,
                                IdentityNo = OAudit.ID_NUM,
                                IdentityType = OAudit.ID_TYPE == null || OAudit.ID_TYPE == "" ? 0 : EnumUtil.ParseEnumInt<IdentityType>(OAudit.ID_TYPE),
                                MotherMaidenName = OAudit.MOTHER_MAIDEN_NAM,
                                MSISDNNumber = OAudit.MSISDN,
                                Nationality = OAudit.NATIONALITY == null || OAudit.NATIONALITY == "" ? 0 : int.Parse(OAudit.NATIONALITY),
                                PreferredLanguage = OAudit.PREF_LANG == null || OAudit.PREF_LANG == "" ? 0 : EnumUtil.ParseEnumInt<Language>(OAudit.PREF_LANG),
                                Race = OAudit.RACE == null || OAudit.RACE == "" ? 0 : int.Parse(OAudit.RACE),
                                Salutation = OAudit.SALUTATION == null || OAudit.SALUTATION == "" ? 0 : EnumUtil.ParseEnumInt<Salutation>(OAudit.SALUTATION),
                            };
                            e.Account.RegistrationDate = AccountService.Get(AccountId).RegistrationDate;
                            e.Account.SIMSerialNumber = OAudit.SIM_SERIAL_NUMBER;
                            e.MSISDN = OAudit.MSISDN;
                            e.SIMCard = OAudit.SIM_SERIAL_NUMBER;
                        }
                        else
                        {
                            e.Account = AccountService.Get(AccountId);

                            e.MSISDN = e.Account.PersonalInfo.MSISDNNumber;
                            e.SIMCard = e.Account.SIMSerialNumber;
                        }
                    }
                    else
                    {
                        e.Account = new AccountVO();
                        e.Account = AccountService.Get(AccountId);

                        e.MSISDN = e.Account.PersonalInfo.MSISDNNumber;
                        e.SIMCard = e.Account.SIMSerialNumber;

                        e.Account.BankingInfo.PrintedBillingFlg = this.GetPrintedBillingByOrder(OrderId);
                    }

                    return e;
                }
            }

            return null;
        }

        public OrderVO GetOrderVO(long OrderId)
        {
            using (var dbContext = new CRMDbContext())
            {
                var Order = (from d in dbContext.XOX_T_ORDER
                             where d.ROW_ID == OrderId
                             select d).First();

                OrderVO OrderVO = new OrderVO();

                OrderVO.ASSIGNEE = Order.ASSIGNEE;
                OrderVO.CUST_REP_ID = Order.CUST_REP_ID;
                OrderVO.ORDER_SOURCE = Order.ORDER_SOURCE;
                OrderVO.CATEGORY = Order.CATEGORY;
                OrderVO.ORDER_STATUS = Order.ORDER_STATUS;
                OrderVO.ROW_ID = Order.ROW_ID;
                OrderVO.PLAN = Order.PLAN;

                return OrderVO;
            }
        }

        public List<OrderDetailVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", string filterBy = "", string filterQuery = "", string UserName = "")
        {
            List<OrderDetailVO> List = new List<OrderDetailVO>();
            using (var dbContext = new CRMDbContext())
            {
                var ORDERs = from d in dbContext.XOX_T_ORDER
                             select new
                             {
                                 Order = d,
                                 Account = (from e in dbContext.XOX_T_ORDER_ITEM
                                            join f in dbContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                            where e.ORDER_ID == d.ROW_ID
                                            select f).FirstOrDefault()
                             };

                if (UserName != "")
                {
                    ORDERs = ORDERs.Where(m => m.Order.ROW_ID == (from d in dbContext.XOX_T_ORDER_ACT where d.ASSIGNEE == UserName && d.ORDER_ID == m.Order.ROW_ID && d.ASSIGNEE == (from e in dbContext.XOX_T_ORDER_ACT where e.ORDER_ID == d.ORDER_ID orderby e.ROW_ID descending select e.ASSIGNEE).FirstOrDefault() select d.ORDER_ID).FirstOrDefault());
                }

                //filtering
                if (filterBy != "" && filterQuery != "" && filterBy != null && filterQuery != null)
                {
                    if (filterBy == "OrderNum")
                        ORDERs = ORDERs.Where(m => m.Order.ORDER_NUM.ToLower().Contains(filterQuery.ToLower()));
                    else if (filterBy == "MSISDN")
                        ORDERs = ORDERs.Where(m => m.Account.MSISDN.ToLower().Contains(filterQuery.ToLower()));
                    else if (filterBy == "FullName")
                        ORDERs = ORDERs.Where(m => m.Account.NAME.ToLower().Contains(filterQuery.ToLower()));
                    else if (filterBy == "SubmissionDate")
                    {
                        DateTime date = DateTime.Parse(filterQuery);
                        ORDERs = ORDERs.Where(m => m.Order.ORDER_SUBMIT_DT.Value.Year == date.Year && m.Order.ORDER_SUBMIT_DT.Value.Month == date.Month && m.Order.ORDER_SUBMIT_DT.Value.Day == date.Day);
                    }
                    else if (filterBy == "Category")
                        ORDERs = ORDERs.Where(m => m.Order.CATEGORY.ToLower().Contains(filterQuery.ToLower()));
                    else if (filterBy == "Status")
                        ORDERs = ORDERs.Where(m => m.Order.ORDER_STATUS.ToLower().Contains(filterQuery.ToLower()));
                }

                TotalCount = ORDERs.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "OrderNum")
                        ORDERs = ORDERs.OrderBy(m => m.Order.ORDER_NUM);
                    else if (orderBy == "MSISDN")
                        ORDERs = ORDERs.OrderBy(m => m.Account.MSISDN);
                    else if (orderBy == "FullName")
                        ORDERs = ORDERs.OrderBy(m => m.Account.NAME);
                    else if (orderBy == "SubmissionDate")
                        ORDERs = ORDERs.OrderBy(m => m.Order.CREATED);
                    else if (orderBy == "Category")
                        ORDERs = ORDERs.OrderBy(m => m.Order.CATEGORY);
                    else if (orderBy == "Status")
                        ORDERs = ORDERs.OrderBy(m => m.Order.ORDER_STATUS);
                    else
                        ORDERs = ORDERs.OrderBy(m => m.Order.ROW_ID);
                }
                else
                {
                    if (orderBy == "OrderNum")
                        ORDERs = ORDERs.OrderByDescending(m => m.Order.ORDER_NUM);
                    else if (orderBy == "MSISDN")
                        ORDERs = ORDERs.OrderByDescending(m => m.Account.MSISDN);
                    else if (orderBy == "FullName")
                        ORDERs = ORDERs.OrderByDescending(m => m.Account.NAME);
                    else if (orderBy == "SubmissionDate")
                        ORDERs = ORDERs.OrderByDescending(m => m.Order.CREATED);
                    else if (orderBy == "Category")
                        ORDERs = ORDERs.OrderByDescending(m => m.Order.CATEGORY);
                    else if (orderBy == "Status")
                        ORDERs = ORDERs.OrderByDescending(m => m.Order.ORDER_STATUS);
                    else
                        ORDERs = ORDERs.OrderByDescending(m => m.Order.ROW_ID);
                }

                if (length >= 0)
                    ORDERs = ORDERs.Skip(startIdx).Take(length);

                foreach (var v in ORDERs)
                {
                    try
                    {
                        OrderDetailVO e = new OrderDetailVO();
                        e.OrderId = v.Order.ROW_ID;
                        e.SubscriptionPlan = v.Order.CATEGORY;
                        e.OrderStatus = v.Order.ORDER_STATUS;
                        e.Category = v.Order.CATEGORY;
                        e.SubmissionDate = v.Order.CREATED;
                        e.OrderNum = v.Order.ORDER_NUM;
                        e.Remarks = v.Order.REMARKS;

                        e.Account = new AccountVO();
                        e.Account = Mapper.Account.Map(v.Account);

                        e.MSISDN = e.Account.PersonalInfo.MSISDNNumber;
                        e.SIMCard = "SN???";//e.Account.PersonalInfo.;

                        List.Add(e);
                    }
                    catch { }
                }
            }

            return List;
        }

        public List<OrderDetailVO> GetAllBySearch(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", string MSISDN = "", string FullName = "", string From = "", string To = "", string Category = "", string Status = "", string OrderNum = "", string OrderType = "", long AccountId = 0)
        {
            List<OrderDetailVO> List = new List<OrderDetailVO>();
            using (var dbContext = new CRMDbContext())
            {
                var ORDERs = from d in dbContext.XOX_T_ORDER
                             select new
                             {
                                 Order = d,
                                 Account = (from e in dbContext.XOX_T_ORDER_ITEM
                                            join f in dbContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                            where e.ORDER_ID == d.ROW_ID
                                            select f).FirstOrDefault()
                             };
                if (AccountId > 0)
                {
                    var ORDER_ITEMs = (from d in dbContext.XOX_T_ORDER_ITEM
                                       where d.CUST_ID == AccountId
                                       select d.ORDER_ID).Distinct();

                    ORDERs = ORDERs.Where(m => ORDER_ITEMs.Contains(m.Order.ROW_ID));
                }

                //filtering string MSISDN, string FullName, string From, string To, string Category, string Status
                if (MSISDN != "")
                {
                    ORDERs = ORDERs.Where(m => m.Account.MSISDN.ToLower().Contains(MSISDN.ToLower()));
                }
                if (FullName != "")
                {
                    ORDERs = ORDERs.Where(m => m.Account.NAME.ToLower().Contains(FullName.ToLower()));
                }
                if (From != "" && To != "")
                {
                    DateTime DateFrom = DateTime.Parse(From);
                    DateTime DateTo = DateTime.Parse(To).AddDays(1);
                    ORDERs = ORDERs.Where(m => m.Order.CREATED.Value >= DateFrom && m.Order.CREATED.Value < DateTo);
                }
                if (Category != "")
                {
                    ORDERs = ORDERs.Where(m => m.Order.CATEGORY.ToLower().Contains(Category.ToLower()));
                }
                if (Status != "")
                {
                    var statusfilter = Status.Split('|').ToList().ConvertAll(d => d.ToLower());
                    ORDERs = ORDERs.Where(m => statusfilter.Contains(m.Order.ORDER_STATUS.ToLower()));
                }
                if (OrderNum != "")
                {
                    ORDERs = ORDERs.Where(m => m.Order.ORDER_NUM.ToLower().Contains(OrderNum.ToLower()));
                }
                if (OrderType != "")
                {
                    if (OrderType == "Registration")
                    {
                        ORDERs = ORDERs.Where(m => m.Order.ORDER_TYPE == XOXConstants.ORDER_TYPE_NEW || m.Order.ORDER_TYPE == XOXConstants.ORDER_TYPE_SUPPLEMENTARY);
                    }
                    else if (OrderType == "ChangePlan")
                    {
                        ORDERs = ORDERs.Where(m => m.Order.ORDER_TYPE == XOXConstants.ORDER_TYPE_CHANGE_PLAN);
                    }
                    else if (OrderType == "Termination")
                    {
                        ORDERs = ORDERs.Where(m => m.Order.ORDER_TYPE == XOXConstants.ORDER_TYPE_TERMINATE);
                    }
                    else if (OrderType == "SupplementaryRegistration")
                    {
                        ORDERs = ORDERs.Where(m => m.Order.ORDER_TYPE == XOXConstants.ORDER_TYPE_SUPPLEMENTARY);
                    }
                }

                TotalCount = ORDERs.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "OrderNum")
                        ORDERs = ORDERs.OrderBy(m => m.Order.ORDER_NUM);
                    else if (orderBy == "MSISDN")
                        ORDERs = ORDERs.OrderBy(m => m.Account.MSISDN);
                    else if (orderBy == "FullName")
                        ORDERs = ORDERs.OrderBy(m => m.Account.NAME);
                    else if (orderBy == "SubmissionDate")
                        ORDERs = ORDERs.OrderBy(m => m.Order.CREATED);
                    else if (orderBy == "Category")
                        ORDERs = ORDERs.OrderBy(m => m.Order.CATEGORY);
                    else if (orderBy == "Status")
                        ORDERs = ORDERs.OrderBy(m => m.Order.ORDER_STATUS);
                    else
                        ORDERs = ORDERs.OrderBy(m => m.Order.ROW_ID);
                }
                else
                {
                    if (orderBy == "OrderNum")
                        ORDERs = ORDERs.OrderByDescending(m => m.Order.ORDER_NUM);
                    else if (orderBy == "MSISDN")
                        ORDERs = ORDERs.OrderByDescending(m => m.Account.MSISDN);
                    else if (orderBy == "FullName")
                        ORDERs = ORDERs.OrderByDescending(m => m.Account.NAME);
                    else if (orderBy == "SubmissionDate")
                        ORDERs = ORDERs.OrderByDescending(m => m.Order.CREATED);
                    else if (orderBy == "Category")
                        ORDERs = ORDERs.OrderByDescending(m => m.Order.CATEGORY);
                    else if (orderBy == "Status")
                        ORDERs = ORDERs.OrderByDescending(m => m.Order.ORDER_STATUS);
                    else
                        ORDERs = ORDERs.OrderByDescending(m => m.Order.ROW_ID);
                }

                if (length >= 0)
                    ORDERs = ORDERs.Skip(startIdx).Take(length);

                foreach (var v in ORDERs)
                {
                    try
                    {
                        OrderDetailVO e = new OrderDetailVO();
                        e.OrderId = v.Order.ROW_ID;
                        e.SubscriptionPlan = v.Order.CATEGORY;
                        e.OrderStatus = v.Order.ORDER_STATUS;
                        e.Category = v.Order.CATEGORY;
                        e.SubmissionDate = v.Order.CREATED;
                        e.OrderNum = v.Order.ORDER_NUM;
                        e.Remarks = v.Order.REMARKS;

                        e.Account = new AccountVO();
                        e.Account = Mapper.Account.Map(v.Account);

                        e.MSISDN = e.Account.PersonalInfo.MSISDNNumber;
                        e.SIMCard = "SN???";//e.Account.PersonalInfo.;

                        if (v.Account.INTEGRATION_ID != null)
                        {
                            if (v.Order.ORDER_TYPE == "Supplementary Registration")
                            {
                                var ParAccount = (from d in dbContext.XOX_T_ACCNT
                                                  where d.ROW_ID == v.Account.PAR_ACCNT_ID
                                                  select d).First();

                                e.CRPId = 'D' + ParAccount.INTEGRATION_ID.ToString().PadLeft(6, '0');
                            }
                            else
                            {
                                e.CRPId = 'D' + v.Account.INTEGRATION_ID.ToString().PadLeft(6, '0');
                            }
                        }

                        List.Add(e);
                    }
                    catch { }
                }
            }

            return List;
        }

        public List<OrderDetailVO> GetAllOrderId(string UserName = "", int Length = -1, string FilterStatus = "")
        {
            List<OrderDetailVO> List = new List<OrderDetailVO>();
            using (var dbContext = new CRMDbContext())
            {
                var ORDERs = from d in dbContext.XOX_T_ORDER
                             select new
                             {
                                 Order = d
                             };

                if (UserName != "")
                {
                    ORDERs = ORDERs.Where(m => m.Order.ROW_ID == (from d in dbContext.XOX_T_ORDER_ACT where d.ASSIGNEE == UserName && d.ORDER_ID == m.Order.ROW_ID && d.ASSIGNEE == (from e in dbContext.XOX_T_ORDER_ACT where e.ORDER_ID == d.ORDER_ID orderby e.ROW_ID descending select e.ASSIGNEE).FirstOrDefault() select d.ORDER_ID).FirstOrDefault());
                }

                if (Length > -1)
                {
                    ORDERs = ORDERs.Take(Length);
                }

                if (FilterStatus != "")
                {
                    ORDERs = ORDERs.Where(m => m.Order.ORDER_STATUS.ToLower() == FilterStatus.ToLower());
                }

                foreach (var v in ORDERs)
                {
                    List.Add(new OrderDetailVO()
                    {
                        OrderId = v.Order.ROW_ID,
                        OrderStatus = v.Order.ORDER_STATUS
                    });
                }
            }

            return List;
        }

        public long GetAllCount(string filterBy = "", string filterQuery = "", string UserName = "")
        {
            long result = 0;
            using (var dbContext = new CRMDbContext())
            {
                var ORDERs = from d in dbContext.XOX_T_ORDER
                             select new
                             {
                                 Order = d,
                                 Account = (from e in dbContext.XOX_T_ORDER_ITEM
                                            join f in dbContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                            where e.ORDER_ID == d.ROW_ID
                                            select f).FirstOrDefault()
                             };

                if (UserName != "")
                {
                    ORDERs = ORDERs.Where(m => m.Order.ROW_ID == (from d in dbContext.XOX_T_ORDER_ACT where d.ASSIGNEE == UserName && d.ORDER_ID == m.Order.ROW_ID && d.ASSIGNEE == (from e in dbContext.XOX_T_ORDER_ACT where e.ORDER_ID == d.ORDER_ID orderby e.ROW_ID descending select e.ASSIGNEE).FirstOrDefault() select d.ORDER_ID).FirstOrDefault());
                }

                //filtering
                if (filterBy != "" && filterQuery != "" && filterBy != null && filterQuery != null)
                {
                    if (filterBy == "OrderNum")
                        ORDERs = ORDERs.Where(m => m.Order.ORDER_NUM.ToLower().Contains(filterQuery.ToLower()));
                    else if (filterBy == "MSISDN")
                        ORDERs = ORDERs.Where(m => m.Account.MSISDN.ToLower().Contains(filterQuery.ToLower()));
                    else if (filterBy == "FullName")
                        ORDERs = ORDERs.Where(m => m.Account.NAME.ToLower().Contains(filterQuery.ToLower()));
                    else if (filterBy == "RegistrationDate")
                    {
                        DateTime date = DateTime.Parse(filterQuery);
                        ORDERs = ORDERs.Where(m => m.Order.ORDER_SUBMIT_DT.Value.Year == date.Year && m.Order.ORDER_SUBMIT_DT.Value.Month == date.Month && m.Order.ORDER_SUBMIT_DT.Value.Day == date.Day);
                    }
                    else if (filterBy == "Category")
                        ORDERs = ORDERs.Where(m => m.Order.CATEGORY.ToLower().Contains(filterQuery.ToLower()));
                    else if (filterBy == "Status")
                        ORDERs = ORDERs.Where(m => m.Order.ORDER_STATUS.ToLower().Contains(filterQuery.ToLower()));
                }

                result = ORDERs.Count();
            }

            return result;
        }

        public List<OrderDocumentVO> GetAllAttachments(long OrderId)
        {
            List<OrderDocumentVO> List = new List<OrderDocumentVO>();
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_ORDER_ATT
                             where d.ORDER_ID == OrderId
                             //&& d.LAST_UPD_BY == null ///
                             select d;

                foreach (var p in result)
                {
                    OrderDocumentVO e = new OrderDocumentVO();
                    e.DocumentId = p.ROW_ID;
                    e.Path = p.FILE_PATH_NAME;

                    List.Add(e);
                }
            }
            return List;
        }

        public bool AddDocuments(List<String> filespath, long OrderId, long UserId = 0)
        {
            if (UserId == 0)
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var user = from d in DBContext.XOX_T_ORDER
                           where d.ROW_ID == OrderId
                           select d;

                if (user.Count() > 0)
                {
                    foreach (var filepath in filespath)
                    {
                        XOX_T_ORDER_ATT n = new XOX_T_ORDER_ATT();
                        n.ORDER_ID = OrderId;
                        n.FILE_PATH_NAME = filepath;
                        n.CREATED = DateTime.Now;
                        n.CREATED_BY = UserId;

                        DBContext.XOX_T_ORDER_ATT.Add(n);
                        DBContext.SaveChanges();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CheckAndAddDocument(List<String> filespath, long OrderId, long UserId = 0)
        {
            if (UserId == 0)
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var user = from d in DBContext.XOX_T_ORDER
                           join e in DBContext.XOX_T_ORDER_ATT on d.ROW_ID equals e.ROW_ID
                           where d.ROW_ID == OrderId
                           select e;

                if (user.Count() <= 0)
                {
                    foreach (var filepath in filespath)
                    {
                        XOX_T_ORDER_ATT n = new XOX_T_ORDER_ATT();
                        n.ORDER_ID = OrderId;
                        n.FILE_PATH_NAME = filepath;
                        n.CREATED = DateTime.Now;
                        n.CREATED_BY = UserId;

                        DBContext.XOX_T_ORDER_ATT.Add(n);
                        DBContext.SaveChanges();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool ResetDocuments(long OrderId)
        {
            using (var DBContext = new CRMDbContext())
            {
                var Order = from d in DBContext.XOX_T_ORDER
                            where d.ROW_ID == OrderId
                            select d;

                if (Order.Count() > 0)
                {
                    var Document = from d in DBContext.XOX_T_ORDER_ATT
                                   where d.ORDER_ID == OrderId
                                   select d;

                    foreach (var v in Document)
                    {
                        v.ORDER_ID = v.ORDER_ID * -1;
                    }
                    DBContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool UpdateStatus(OrderActivityVO Activity, string Status, string offerFrom = "")
        {
            using (var dbContext = new CRMDbContext())
            {
                string SubscriberJoinDate = "";
                bool flg = false;
                if (Status == "Rejected")
                {
                    var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                    var Account = (from d in dbContext.XOX_T_ACCNT
                                   join e in dbContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.CUST_ID
                                   where e.ORDER_ID == Activity.ORDER_ID //&& d.ACCNT_TYPE_CD == PrincipalLine
                                   select d).First();

                    if (Account.INTEGRATION_ID == null || Account.INTEGRATION_ID == "")
                    {
                        throw new Exception("An error occurred. INTEGRATION_ID is not found.");
                    }

                    var SupplementaryLine = ((int)AccountType.SupplementaryLine).ToString();
                    List<AccountSupplementary> SuppLines = new List<AccountSupplementary>();
                    var SuppAccount = from d in dbContext.XOX_T_ACCNT
                                      where d.ACCNT_TYPE_CD == SupplementaryLine && d.PAR_ACCNT_ID == Account.ROW_ID
                                      select d;
                    foreach (var v in SuppAccount)
                    {
                        SuppLines.Add(new AccountSupplementary()
                        {
                            AccountId = v.ROW_ID,
                            MSISDN = v.MSISDN,
                            Name = v.NAME
                        });
                    }

                    var parameter = new OrderReject()
                    {
                        UserId = int.Parse(Account.INTEGRATION_ID),
                        OrderId = Activity.ORDER_ID,
                        Reason = Activity.REJECTED_REASON,
                        User = new User()
                        {
                            Source = "CRM",
                        },
                        List = SuppLines,
                        isSuppLine = Account.ACCNT_TYPE_CD == "3" ? true : false,
                        AccountId = Activity.ORDER_ID ///
                    };
                    if (Activity.CREATED_BY == null || Activity.CREATED_BY == 0)
                    {
                        parameter.User.UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
                    }
                    else
                    {
                        parameter.User.UserId = (int)Activity.CREATED_BY;
                    }

                    var EAIResult = EAIService.RejectOrder(parameter);

                    if (EAIResult == "True")
                    {
                        if (Account.ACCNT_TYPE_CD != SupplementaryLine)
                        {
                            //Add to Order Audit
                            AddOrderAudit(Account.ROW_ID, Activity.ORDER_ID, Activity.CREATED_BY == null || Activity.CREATED_BY == 0 ? int.Parse(Thread.CurrentPrincipal.Identity.Name) : (long)Activity.CREATED_BY);
                        }

                        flg = true;
                    }
                }
                else if (Status == "Pending Verification")
                {
                    var Account = (from d in dbContext.XOX_T_ACCNT
                                   join e in dbContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.CUST_ID
                                   where e.ORDER_ID == Activity.ORDER_ID
                                   select d).First();

                    if (Account.INTEGRATION_ID == null || Account.INTEGRATION_ID == "")
                    {
                        throw new Exception("An error occurred. INTEGRATION_ID is not found.");
                    }

                    var EAIResult = EAIService.OrderUpdateStatus(new OrderUpdateStatus()
                    {
                        User = new User()
                        {
                            Source = "CRM",
                            UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name)
                        },
                        IntegrationId = long.Parse(Account.INTEGRATION_ID),
                        OrderStatus = "Pending Verification",
                        isSuppLine = Account.ACCNT_TYPE_CD == "3" ? true : false
                    });

                    if (EAIResult == "True")
                    {
                        flg = true;
                    }
                }
                else if (Status == "Withdraw")
                {
                    var Account = (from d in dbContext.XOX_T_ACCNT
                                   join e in dbContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.CUST_ID
                                   where e.ORDER_ID == Activity.ORDER_ID
                                   select d).First();

                    if (Account.INTEGRATION_ID == null || Account.INTEGRATION_ID == "")
                    {
                        throw new Exception("An error occurred. INTEGRATION_ID is not found.");
                    }

                    var EAIResult = EAIService.OrderUpdateStatus(new OrderUpdateStatus()
                    {
                        User = new User()
                        {
                            Source = "CRM",
                            UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name)
                        },
                        IntegrationId = long.Parse(Account.INTEGRATION_ID),
                        OrderStatus = "Withdraw",
                        isSuppLine = Account.ACCNT_TYPE_CD == "3" ? true : false
                    });

                    if (EAIResult == "True")
                    {
                        AccountActivityVO AccountActivity = new AccountActivityVO();
                        AccountActivity.ACCNT_ID = Account.ROW_ID;
                        AccountActivity.REASON = "";
                        AccountActivity.ASSIGNEE = Activity.ASSIGNEE;
                        AccountActivity.CREATED_BY = Activity.CREATED_BY;
                        AccountActivity.LAST_UPD_BY = Activity.LAST_UPD_BY;

                        AccountService.UpdateStatus(AccountActivity, "Withdraw");

                        flg = true;
                    }
                }
                else if (Status == "Active")
                {
                    var Account = (from d in dbContext.XOX_T_ACCNT
                                   join e in dbContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.CUST_ID
                                   where e.ORDER_ID == Activity.ORDER_ID
                                   select d).First();

                    if (Account.INTEGRATION_ID == null || Account.INTEGRATION_ID == "")
                    {
                        throw new Exception("An error occurred. INTEGRATION_ID is not found.");
                    }

                    var MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : '6' + Account.MSISDN;
                    var EAIResult = EAIService.GetSubscriberProfile(new GetSubscriberProfile()
                    {
                        MSISDN = MSISDN,
                        User = new User()
                        {
                            UserId = Activity.LAST_UPD_BY == null ? int.Parse(Thread.CurrentPrincipal.Identity.Name) : (int)Activity.LAST_UPD_BY
                        }
                    });
                    var r = JObject.Parse(EAIResult);

                    var OfferCode = "";
                    GetSubscriberProfileResponse response = null;
                    try
                    {
                        response = r["Result"].ToObject<GetSubscriberProfileResponse>();

                        SubscriberJoinDate = response.Customer.effective;
                        OfferCode = response.Customer.offerCode;
                        flg = true;
                    }
                    catch
                    {
                        throw new Exception(r["Result"].ToString());
                    }


                    //if (offerFrom == "Prepaid Offer")
                    //{
                    //    Activity.ACT_REMARKS += " OfferCode : " + OfferCode;
                    //    if (OfferCode == XOXConstants.OFFER_CODE_POSTPAID || OfferCode == XOXConstants.OFFER_CODE_LIFESTYLE || OfferCode == XOXConstants.OFFER_CODE_HYBRID)
                    //    {
                    //        Activity.ACT_REMARKS = "Response Offer Code cannot be " + XOXConstants.OFFER_CODE_POSTPAID + " / " + XOXConstants.OFFER_CODE_LIFESTYLE + " / " + XOXConstants.OFFER_CODE_HYBRID + " . Current Offer Code is " + OfferCode;
                    //        this.UpdateStatus(Activity, "Activation Failed");

                    //    }
                    //}
                    //else /*if (offerFrom == "Postpaid Offer")*/
                    if (offerFrom != "Prepaid Offer")
                    {
                        if (OfferCode != XOXConstants.OFFER_CODE_POSTPAID)
                        {
                            Activity.ACT_REMARKS = "Response Offer Code is not Consumer Postpaid Offer. Current Offer Code is " + OfferCode;
                            this.UpdateStatus(Activity, "Activation Failed");

                            return false;
                        }
                    }

                    //add asset
                    AssetService.ConvertAssetFromPlan(AccountService.Get(Account.ROW_ID), response);
                }
                else if (Status == "Pending Activation")
                {
                    var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                    var Account = (from d in dbContext.XOX_T_ACCNT
                                   join e in dbContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.CUST_ID
                                   where e.ORDER_ID == Activity.ORDER_ID //&& d.ACCNT_TYPE_CD == PrincipalLine
                                   select d).First();

                    if (Account.INTEGRATION_ID == null || Account.INTEGRATION_ID == "")
                    {
                        throw new Exception("An error occurred. INTEGRATION_ID is not found.");
                    }

                    //checking payment amount sufficient
                    if (AccountService.CheckPaymentSufficient(Account.ROW_ID, Activity.ORDER_ID) == false)
                    {
                        throw new Exception("Insufficient Collected Payment.");
                    }

                    var EAIResult = EAIService.OrderUpdateStatus(new OrderUpdateStatus()
                    {
                        User = new User()
                        {
                            Source = "CRM",
                            UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name)
                        },
                        IntegrationId = long.Parse(Account.INTEGRATION_ID),
                        OrderStatus = "Inactive",
                        isSuppLine = Account.ACCNT_TYPE_CD == "3" ? true : false
                    });

                    if (EAIResult == "True")
                    {
                        flg = true;
                    }
                    else
                    {
                        throw new Exception("Failed syncing Status to CRP");
                    }
                }
                else
                {
                    flg = true;
                }

                if (flg == true)
                {
                    var result = from d in dbContext.XOX_T_ORDER
                                 where d.ROW_ID == Activity.ORDER_ID
                                 select d;

                    if (result.Count() > 0)
                    {
                        Activity.ACT_DESC = "Changing Order Status from " + result.First().ORDER_STATUS + " to " + Status + ".";
                        if (Activity.REJECTED_REASON != "")
                        {
                            //Activity.ACT_DESC += "\nReason : " + Activity.REJECTED_REASON;
                            Activity.ACT_REMARKS += "\nReason : " + Activity.REJECTED_REASON;
                        }
                        Activity.DUE_DATE = DateTime.Now; ///
                        Activity.ORDER_STATUS = Status;
                        OrderActivityService.AddActivity(Activity);

                        result.First().ORDER_STATUS = Status;
                        result.First().REJECT_REASONS = Activity.REJECTED_REASON == "" ? null : Activity.REJECTED_REASON;
                        result.First().LAST_UPD = DateTime.Now;
                        result.First().LAST_UPD_BY = Activity.LAST_UPD_BY == null ? int.Parse(Thread.CurrentPrincipal.Identity.Name) : Activity.LAST_UPD_BY;
                        dbContext.SaveChanges();


                        if (Status == "Active")
                        {
                            var Account = (from d in dbContext.XOX_T_ACCNT
                                           join e in dbContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.CUST_ID
                                           where e.ORDER_ID == Activity.ORDER_ID
                                           select d).First();

                            AccountActivityVO AccountActivity = new AccountActivityVO();
                            AccountActivity.ACCNT_ID = Account.ROW_ID;
                            AccountActivity.REASON = "";
                            AccountActivity.ASSIGNEE = Activity.ASSIGNEE;
                            AccountActivity.CREATED_BY = Activity.CREATED_BY;
                            AccountActivity.LAST_UPD_BY = Activity.LAST_UPD_BY;

                            AccountService.UpdateStatus(AccountActivity, AccountStatus.Active.ToString(), SubscriberJoinDate);
                            
                            var SupplementaryLine = ((int)AccountType.SupplementaryLine).ToString();
                            var SuppAccount = from d in dbContext.XOX_T_ACCNT
                                              where d.PAR_ACCNT_ID == Account.ROW_ID
                                              && d.ACCNT_TYPE_CD == SupplementaryLine
                                              select d;

                            foreach (var v in SuppAccount)
                            {
                                AccountActivityVO SuppAccountActivity = new AccountActivityVO();
                                SuppAccountActivity.ACCNT_ID = v.ROW_ID;
                                SuppAccountActivity.REASON = "";
                                SuppAccountActivity.ASSIGNEE = Activity.ASSIGNEE;
                                SuppAccountActivity.CREATED_BY = Activity.CREATED_BY;
                                SuppAccountActivity.LAST_UPD_BY = Activity.LAST_UPD_BY;
                                AccountService.UpdateStatus(SuppAccountActivity, AccountStatus.Active.ToString(), SubscriberJoinDate);
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        private void AddOrderAudit(long ACCNT_ID, long ORDER_ID, long UserId)
        {
            using (var dbContext = new CRMDbContext())
            {
                string PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                string BillingLine = ((int)AccountType.BillingLine).ToString();
                string SupplementaryLine = ((int)AccountType.SupplementaryLine).ToString();

                var Account = (from d in dbContext.XOX_T_ACCNT
                               where d.ROW_ID == ACCNT_ID && d.ACCNT_TYPE_CD == PrincipalLine
                               select d).First();

                var Address = AddressService.Get((long)Account.ADDR_ID);

                var BillAccount = (from d in dbContext.XOX_T_ACCNT
                                   where d.PAR_ACCNT_ID == ACCNT_ID && d.ACCNT_TYPE_CD == BillingLine
                                   select d).First();

                var BillAddress = AddressService.Get((long)BillAccount.ADDR_ID);

                var SuppAccount = from d in dbContext.XOX_T_ACCNT
                                  where d.PAR_ACCNT_ID == ACCNT_ID && d.ACCNT_TYPE_CD == SupplementaryLine
                                  select d;

                string SuppMSISDN = "";
                if (SuppAccount.Count() > 0)
                {
                    SuppMSISDN = SuppAccount.First().MSISDN;
                }

                var Order = (from d in dbContext.XOX_T_ORDER
                             where d.ROW_ID == ORDER_ID
                             select d).First();

                XOX_T_ORDER_AUDIT et = new XOX_T_ORDER_AUDIT();
                et.ACCNT_ID = ACCNT_ID;
                et.ADDR_1 = Address.AddressLine1;
                et.ADDR_2 = Address.AddressLine2;
                et.BANK_ACCNT_NUMBER = Account.BANK_ACC_NUM;
                et.BANK_NAME = Account.BANK_NAME;
                et.BILL_ACCNT_NAME = Account.BILL_ACCNT_NAME;
                et.BILL_ADDR_1 = BillAddress.AddressLine1;
                et.BILL_ADDR_2 = BillAddress.AddressLine2;
                et.BILL_BANK_ISSUER = Account.BANK_ISSUER;
                et.BILL_CARD_NUMBER = Account.BILL_ACCNT_NUM;
                et.BILL_CARD_TYPE = Account.BILL_CARD_TYPE;
                et.BILL_CITY = BillAddress.City;
                et.BILL_COUNTRY = BillAddress.Country;
                et.BILL_EXPIRY_MONTH = Account.BANK_EXPIRY_MONTH;
                et.BILL_EXPIRY_YEAR = Account.BANK_EXPIRY_YEAR;
                et.BILL_POSTCODE = BillAddress.Postcode;
                et.BILL_STATE = BillAddress.State;
                et.BILL_THIRD_PARTY = Account.BANK_THIRD_PARTY;
                et.CATEGORY = Order.CATEGORY;
                et.CITY = Address.City;
                et.COUNTRY = Address.Country;
                et.CREATED = DateTime.Now;
                et.CREATED_BY = UserId;
                et.DOB = Account.BIRTH_DT;
                et.EMAIL = Account.EMAIL_ADDR;
                et.GENDER = Account.GENDER;
                et.ID_NUM = Account.ID_NUM;
                et.ID_TYPE = Account.ID_TYPE;
                et.MOBILE_NO = Account.MOBILE_NO;
                et.MOTHER_MAIDEN_NAM = Account.MOTHER_MAIDEN_NAME;
                et.MSISDN = Account.MSISDN;
                et.MSISDN_SUPPLINE = SuppMSISDN;
                et.NAME = Account.NAME;
                et.NATIONALITY = Account.NATIONALITY;
                et.ORDER_ID = ORDER_ID;
                et.PLAN = Order.PLAN;
                et.POSTCODE = Address.Postcode;
                et.PREF_LANG = Account.PREFERRED_LANG;
                et.RACE = Account.RACE;
                et.SALUTATION = Account.SALUTATION;
                et.SIM_SERIAL_NUMBER = Account.SIM_SERIAL_NUMBER;
                et.STATE = Address.State;
                et.SUBMISSION_DT = Order.CREATED;
                et.ITEMISED_BILLING = Account.BILL_LANG == null ? false : bool.Parse(Account.BILL_LANG);

                dbContext.XOX_T_ORDER_AUDIT.Add(et);
                dbContext.SaveChanges();
            }
        }

        public bool ActivateOrderRequest(OrderActivityVO OrderActivity)
        {
            var Deposit = productManager.GetDeposit(OrderActivity.ORDER_ID);
            var AdvancePayment = productManager.GetAdvancePayment(OrderActivity.ORDER_ID);

            if (Deposit > 0 && AdvancePayment > 0)
            {
                using (var dbContext = new CRMDbContext())
                {
                    var order = (from d in dbContext.XOX_T_ORDER
                                 where d.ROW_ID == OrderActivity.ORDER_ID
                                 select d).First();

                    var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                    var Account = (from d in dbContext.XOX_T_ACCNT
                                   join e in dbContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.CUST_ID
                                   where e.ORDER_ID == OrderActivity.ORDER_ID //&& d.ACCNT_TYPE_CD == PrincipalLine
                                   select d).First();

                    #region Check One-X Dealer
                    var MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : '6' + Account.MSISDN;

                    var EAIResult = EAIService.CheckOneXDealer(new CheckOneXDealer()
                    {
                        CustomerNRIC = Account.ID_NUM,
                        User = new User()
                        {
                            UserId = OrderActivity.LAST_UPD_BY == null ? int.Parse(Thread.CurrentPrincipal.Identity.Name) : (int)OrderActivity.LAST_UPD_BY
                        }
                    });
                    var r = JObject.Parse(EAIResult);

                    try
                    {
                        var response = r["Result"].ToObject<bool>();
                        if (response == true)
                        {
                            OrderActivity.ACT_REMARKS = "Subscriber's NRIC has registered as One-X dealer.";
                            OrderActivity.REJECTED_REASON = "Subscriber's NRIC has registered as One-X dealer.";
                            this.UpdateStatus(OrderActivity, "Rejected");

                            return false;
                        }
                    }
                    catch
                    {
                        OrderActivity.ACT_REMARKS = r["Result"].ToString();
                        this.UpdateStatus(OrderActivity, "Activation Failed");

                        return false;
                    }
                    #endregion

                    if (order.ORDER_TYPE == "New Registration")
                    {
                        if (order.CATEGORY == "NEW")
                        {
                            return this.ActivateOrder(OrderActivity);
                        }
                        else if (order.CATEGORY == "MNP")
                        {
                            return this.MNPPortInRequest(OrderActivity.ORDER_ID);
                        }
                        else if (order.CATEGORY == "COBP")
                        {
                            return this.ActivateCOBPOrder(OrderActivity);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (order.ORDER_TYPE == "Supplementary Registration")
                    {
                        if (order.CATEGORY == "NEW")
                        {
                            return this.ActivateSuppLineOrder(OrderActivity);
                        }
                        else if (order.CATEGORY == "MNP")
                        {
                            return this.MNPSuppLinePortInRequest(OrderActivity.ORDER_ID);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                throw new Exception("Failed to Activate. Deposit and Advance Payment must be filled.");
            }
        }

        public bool ActivateOrder(OrderActivityVO OrderActivity, int UserId = 0, string offerFrom = "Postpaid Offer")
        {
            if (UserId == 0)
            {
                UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var dbContext = new CRMDbContext())
            {
                var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                var Account = (from d in dbContext.XOX_T_ACCNT
                               join e in dbContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.CUST_ID
                               where e.ORDER_ID == OrderActivity.ORDER_ID && d.ACCNT_TYPE_CD == PrincipalLine
                               select d).First();

                var EAIResult = AccountService.AddSubcriberProfile(Account.ROW_ID, OrderActivity.ORDER_ID, UserId);
                var result = JObject.Parse(EAIResult);

                try
                {
                    var EAIResultMessage = "";
                    try
                    {
                        EAIResultMessage = result["Result"]["result"].ToString();
                    }
                    catch { }
                    if (EAIResultMessage.ToLower() == "true") //sucessfully created
                    {
                        Payment payment = GetAdvancePaymentRecord(Account.ROW_ID);

                        if (payment == null) //if payment got no record.
                        {
                            payment = new Payment()
                            {
                                Amount = "0",
                                Reference = "",
                            };
                        }
                        payment.Reference = Account.BANK_ISSUER + "-" + CommonService.MaskCreditCardDigit(Account.BILL_ACCNT_NUM);
                        payment.MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : '6' + Account.MSISDN;
                        payment.PaymentMethod = XOXConstants.PAYMENT_METHOD_CREDIT_CARD;
                        payment.User = new User()
                        {
                            Source = "CRM",
                            UserId = UserId
                        };

                        var MakePaymentResult = AccountService.MakePayment(Account.ROW_ID, payment, false, UserId);

                        if (MakePaymentResult != "true")
                        {
                            OrderActivity.ACT_REMARKS = MakePaymentResult;
                        }
                        else
                        {
                            AccountService.ChangeAdvancePaymentFlagResponse(Account.ROW_ID, true);
                        }

                        //advance payment supp lines
                        var SupplementaryLine = ((int)AccountType.SupplementaryLine).ToString();
                        var suppline = from d in dbContext.XOX_T_ACCNT
                                       where d.PAR_ACCNT_ID == Account.ROW_ID
                                       && d.ACCNT_TYPE_CD == SupplementaryLine
                                       select d;

                        foreach (var v in suppline)
                        {
                            try
                            {
                                Payment slpayment = GetAdvancePaymentRecord(v.ROW_ID);

                                if (slpayment == null) //if slpayment got no record.
                                {
                                    slpayment = new Payment()
                                    {
                                        Amount = "19.08",
                                        Reference = "",
                                    };
                                }
                                //reference from principal line
                                slpayment.Reference = payment.Reference + " (Supp)";
                                slpayment.MSISDN = v.MSISDN[0] == '6' ? v.MSISDN : '6' + v.MSISDN;
                                slpayment.PaymentMethod = XOXConstants.PAYMENT_METHOD_CREDIT_CARD;
                                slpayment.User = new User()
                                {
                                    Source = "CRM",
                                    UserId = UserId
                                };

                                var SLMakePaymentResult = AccountService.MakePayment(v.ROW_ID, slpayment, false, UserId);

                                if (SLMakePaymentResult == "true")
                                {
                                    AccountService.ChangeAdvancePaymentFlagResponse(v.ROW_ID, true);
                                }
                            }
                            catch
                            {
                                //////////
                            }
                        }

                        this.UpdateStatus(OrderActivity, "Active", offerFrom);

                        //send sms only for Actviation of "Premium 150" Plan
                        if (GetOrderVO(OrderActivity.ORDER_ID).PLAN.ToLower() == "premium 150")
                        {
                            try
                            {
                                EAIService.SendSMSWithShortCode(new SendSMSWithShortCode()
                                {
                                    User = new User()
                                    {
                                        Source = "CRM",
                                        UserId = 1
                                    },
                                    MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : ('6' + Account.MSISDN),
                                    Message = "RM0: Welcome to ONEXOX #postpaid! Thank you for your continuous support and you are now among the first to enjoy our new postpaid with UNLIMITED data, voice and text! Have any questions or feedback? Feel free to reach us at 12273 or send an email to enquiries@xox.com.my."
                                });
                            }
                            catch { }
                        }

                        return true;
                    }
                    else // failed
                    {
                        try
                        {
                            EAIResultMessage = result["Result"].ToString();
                        }
                        catch { }
                        OrderActivity.ACT_REMARKS = EAIResultMessage;
                        this.UpdateStatus(OrderActivity, "Activation Failed");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    OrderActivity.ACT_REMARKS = e.Message;
                    this.UpdateStatus(OrderActivity, "Activation Failed");
                    return false;
                }
            }
        }

        public bool ActivateSuppLineOrder(OrderActivityVO OrderActivity, int UserId = 0, string offerFrom = "Postpaid Offer")
        {
            if (UserId == 0)
            {
                UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var dbContext = new CRMDbContext())
            {
                var SupplementaryLine = ((int)AccountType.SupplementaryLine).ToString();
                var Account = (from d in dbContext.XOX_T_ACCNT
                               join e in dbContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.CUST_ID
                               where e.ORDER_ID == OrderActivity.ORDER_ID && d.ACCNT_TYPE_CD == SupplementaryLine
                               select d).First();

                var EAIResult = AccountService.AddSubcriberProfile(Account.ROW_ID, OrderActivity.ORDER_ID, UserId);
                var result = JObject.Parse(EAIResult);

                try
                {
                    var EAIResultMessage = "";
                    try
                    {
                        EAIResultMessage = result["Result"]["result"].ToString();
                    }
                    catch { }
                    if (EAIResultMessage.ToLower() == "true") //sucessfully created
                    {
                        var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                        var ParentAccount = (from d in dbContext.XOX_T_ACCNT
                                             where d.ROW_ID == Account.PAR_ACCNT_ID && d.ACCNT_TYPE_CD == PrincipalLine
                                             select d).First();

                        Payment payment = GetAdvancePaymentRecord(Account.ROW_ID);

                        if (payment == null) //if payment got no record.
                        {
                            payment = new Payment()
                            {
                                Amount = "19.08",
                                Reference = "",
                            };
                        }
                        payment.Reference = ParentAccount.BANK_ISSUER + "-" + CommonService.MaskCreditCardDigit(ParentAccount.BILL_ACCNT_NUM) + " (Supp)";
                        payment.MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : '6' + Account.MSISDN;
                        payment.PaymentMethod = XOXConstants.PAYMENT_METHOD_CREDIT_CARD;
                        payment.User = new User()
                        {
                            Source = "CRM",
                            UserId = UserId
                        };

                        var MakePaymentResult = AccountService.MakePayment(Account.ROW_ID, payment, false, UserId);

                        if (MakePaymentResult != "true")
                        {
                            OrderActivity.ACT_REMARKS = MakePaymentResult;
                        }
                        else
                        {
                            AccountService.ChangeAdvancePaymentFlagResponse(Account.ROW_ID, true);
                        }

                        this.UpdateStatus(OrderActivity, "Active", offerFrom);

                        return true;
                    }
                    else // failed
                    {
                        try
                        {
                            EAIResultMessage = result["Result"].ToString();
                        }
                        catch { }
                        OrderActivity.ACT_REMARKS = EAIResultMessage;
                        this.UpdateStatus(OrderActivity, "Activation Failed");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    OrderActivity.ACT_REMARKS = e.Message;
                    this.UpdateStatus(OrderActivity, "Activation Failed");
                    return false;
                }
            }
        }

        public bool ActivateCOBPOrder(OrderActivityVO OrderActivity, int UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var dbContext = new CRMDbContext())
            {
                bool addNewProfile = false;

                var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                var Account = (from d in dbContext.XOX_T_ACCNT
                               join e in dbContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.CUST_ID
                               where e.ORDER_ID == OrderActivity.ORDER_ID && d.ACCNT_TYPE_CD == PrincipalLine
                               select d).First();

                #region Check subscriber offer code
                var MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : '6' + Account.MSISDN;
                var EAIResult2 = EAIService.GetSubscriberProfile(new GetSubscriberProfile()
                {
                    MSISDN = MSISDN,
                    User = new User()
                    {
                        UserId = OrderActivity.LAST_UPD_BY == null ? int.Parse(Thread.CurrentPrincipal.Identity.Name) : (int)OrderActivity.LAST_UPD_BY
                    }
                });
                var r = JObject.Parse(EAIResult2);

                var OfferCode = "";
                try
                {
                    var response = r["Result"].ToObject<GetSubscriberProfileResponse>();
                    OfferCode = response.Customer.offerCode;
                }
                catch
                {
                    if (r["Result"].ToString().Contains("Subscription Not Found"))
                    {
                        addNewProfile = true;
                    }
                    else
                    {
                        throw new Exception(r["Result"].ToString());
                    }
                }

                if (addNewProfile == false)
                {
                    OrderActivity.ACT_REMARKS += " OfferCode : " + OfferCode;
                    if (OfferCode == XOXConstants.OFFER_CODE_LIFESTYLE || OfferCode == XOXConstants.OFFER_CODE_HYBRID)
                    {
                        //store Offer Code to Remarks
                        var Order = (from d in dbContext.XOX_T_ORDER
                                     where d.ROW_ID == OrderActivity.ORDER_ID
                                     select d).First();

                        Order.REMARKS = OfferCode;
                        dbContext.SaveChanges();

                        this.UpdateStatus(OrderActivity, "Activation Failed");

                        throw new Exception("Current Offer Code is " + OfferCode + ". The subscriber profile needs to be removed beforehand.");
                    }
                    else if (OfferCode == XOXConstants.OFFER_CODE_POSTPAID)
                    {
                        OrderActivity.ACT_REMARKS = "The Offer Code is " + OfferCode + ".";
                        OrderActivity.REJECTED_REASON = "The Offer Code is " + OfferCode + ".";
                        this.UpdateStatus(OrderActivity, "Rejected");

                        throw new Exception("Current Offer Code is " + OfferCode + ".");
                    }
                }
                #endregion

                if (addNewProfile == false)
                {
                    #region Perform Migration to Postpaid
                    var EAIResult = AccountService.migrateToPostpaid(Account.ROW_ID, OrderActivity.ORDER_ID, UserId);
                    var result = JObject.Parse(EAIResult);

                    try
                    {
                        var EAIResultMessage = "";
                        try
                        {
                            EAIResultMessage = result["Result"]["result"].ToString();
                        }
                        catch { }
                        if (EAIResultMessage.ToLower() == "true") //sucessfully created
                        {
                            var Balance = "";
                            try
                            {
                                Balance = result["Result"]["message"].ToString();
                            }
                            catch
                            {
                                throw new Exception("migrateToPostpaid response message doesn't have value.");
                            }

                            //advance payment
                            Payment payment = GetAdvancePaymentRecord(Account.ROW_ID);

                            if (payment == null) //if payment got no record.
                            {
                                payment = new Payment()
                                {
                                    Amount = "0",
                                    Reference = "",
                                };
                            }
                            payment.Reference = Account.BANK_ISSUER + "-" + CommonService.MaskCreditCardDigit(Account.BILL_ACCNT_NUM);
                            payment.MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : '6' + Account.MSISDN;
                            payment.PaymentMethod = XOXConstants.PAYMENT_METHOD_CREDIT_CARD;
                            payment.User = new User()
                            {
                                Source = "CRM",
                                UserId = UserId
                            };

                            var MakeAdvancePaymentResult = AccountService.MakePayment(Account.ROW_ID, payment, false, UserId);

                            if (MakeAdvancePaymentResult != "true")
                            {
                                OrderActivity.ACT_REMARKS = MakeAdvancePaymentResult;
                            }
                            else
                            {
                                AccountService.ChangeAdvancePaymentFlagResponse(Account.ROW_ID, true);
                            }

                            //billing payment
                            payment.Amount = Balance;
                            payment.PaymentMethod = XOXConstants.PAYMENT_METHOD_CASH;
                            payment.PaymentType = XOXConstants.PAYMENT_TYPE_BILLING;
                            payment.Reference = "Prepaid Balance";

                            var MakeBillingPaymentResult = AccountService.MakePayment(Account.ROW_ID, payment, true, UserId);

                            if (MakeBillingPaymentResult != "true")
                            {
                                OrderActivity.ACT_REMARKS += " | " + MakeBillingPaymentResult;
                            }

                            OrderActivity.ACT_REMARKS = "Prepaid Balance : " + Balance;
                            this.UpdateStatus(OrderActivity, "Active", "Prepaid Offer");

                            return true;
                        }
                        else // failed
                        {
                            try
                            {
                                EAIResultMessage = result["Result"].ToString();
                            }
                            catch { }
                            OrderActivity.ACT_REMARKS = EAIResultMessage;
                            this.UpdateStatus(OrderActivity, "Activation Failed");
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        OrderActivity.ACT_REMARKS = e.Message;
                        this.UpdateStatus(OrderActivity, "Activation Failed");
                        return false;
                    }
                    #endregion
                }
                else
                {
                    //check offer code in Remarks
                    var Order = (from d in dbContext.XOX_T_ORDER
                                 where d.ROW_ID == OrderActivity.ORDER_ID
                                 select d).First();

                    if (Order.REMARKS == XOXConstants.OFFER_CODE_HYBRID || Order.REMARKS == XOXConstants.OFFER_CODE_LIFESTYLE)
                    {
                        //if subscriber not found and offer code was Hybrid/Lifestyle,
                        //then addSubscriberProfile and Make Advance Payment
                        return ActivateOrder(OrderActivity, UserId, "Prepaid Offer");
                    }
                    else
                    {
                        OrderActivity.ACT_REMARKS = r["Result"].ToString();
                        this.UpdateStatus(OrderActivity, "Activation Failed");
                        return false;
                    }
                }
            }
        }

        public Payment GetAdvancePaymentRecord(long AccountId)
        {
            using (var DBContext = new CRMDbContext())
            {
                var ett = from d in DBContext.XOX_T_ACCNT_PAYMENT
                          where d.ACCNT_ID == AccountId
                          && d.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_ADVANCE_PAYMENT
                          select d;

                if (ett.Count() > 0)
                {
                    var v = ett.First();
                    return new Payment()
                    {
                        Amount = (v.AMOUNT == null ? 0 : (decimal)v.AMOUNT).ToString(),
                        Reference = v.REFERENCE
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public string GetPortReqFormId(XOX_T_ACCNT a, long OrderId)
        {
            string result = "";

            ////random id
            //string ORDER_ID = OrderId.ToString().PadLeft(8, '0');
            //string CURRENT_DT = DateTime.Now.ToString("yyMM");
            //string RANDOM = RandomString(2);
            //
            //result = "PP" + ORDER_ID + CURRENT_DT + RANDOM;

            ////fixed id
            //string ORDER_ID = OrderId.ToString().PadLeft(7, '0');
            //string MSISDN = a.MSISDN.Substring(a.MSISDN.Length - 7);

            //result = "PP" + ORDER_ID + MSISDN;

            //random id 2
            string ORDER_ID = OrderId.ToString().PadLeft(8, '0');
            string CURRENT_DT = DateTime.Now.ToString("yyMMdd");

            result = "PP" + ORDER_ID + CURRENT_DT;

            return result;
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool MNPPortInRequest(long OrderId, int UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_ORDER
                             where d.ROW_ID == OrderId
                             select d;

                var order = result.First();

                var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                var result2 = from d in DBContext.XOX_T_ORDER
                              join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                              join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                              where d.ROW_ID == order.ROW_ID && f.ACCNT_TYPE_CD == PrincipalLine
                              select f;

                var account = result2.First();

                bool foreigner = account.ID_TYPE == "PassportNo" ? true : false;

                var address = AddressService.Get((long)account.ADDR_ID);

                var dataPack = productManager.GetDataPack(order.PLAN);

                var MSISDN = account.MSISDN[0] == '6' ? account.MSISDN.Substring(1) : account.MSISDN;

                var printedBill = "No";
                printedBill = this.GetPrintedBillingByOrder(OrderId) == true ? "Yes" : "No";
                /*try{
                    printedBill = bool.Parse(account.BILL_LANG) == true ? "Yes" : "No";
                }
                catch{}*/

                int age = 0;
                if (account.BIRTH_DT != null)
                {
                    DateTime today = DateTime.Today;
                    age = today.Year - account.BIRTH_DT.Value.Year;
                    if (account.BIRTH_DT > today.AddYears(-age)) age--;
                }

                string StateCode = CommonService.GetLookupValueByName(address.State.ToUpper(), "State Code Mapping");

                string PortReqFormId = GetPortReqFormId(account, OrderId);
                order.PORT_REQ_FORM = PortReqFormId;
                DBContext.SaveChanges();

                List<NumbersToPort> NumbersToPorts = new List<NumbersToPort>();

                var NumbersToPortPrincipal = new NumbersToPort()
                {
                    AccountType = "POSTPAID",
                    AutoDebit = "Yes",
                    ContractPeriod = 24,
                    CreditLimit = (int)productManager.GetCreditLimit(order.PLAN),
                    DataPack = dataPack,
                    Deposit = (int)productManager.GetDeposit(order.ROW_ID),// + (foreigner == true ? XOXConstants.FOREIGNER_DEPOSIT : 0),
                    InitFnFData = (int)productManager.GetinitFnFData(order.PLAN),
                    InitFnFOnNetCalls = (int)productManager.GetinitFnFOnNetCalls(order.PLAN),
                    InitFnFOffNetCalls = (int)productManager.GetinitFnFOffNetCalls(order.PLAN),
                    InitFnFOnNetSms = (int)productManager.GetinitFnFOnNetSms(order.PLAN),
                    InitFnFOffNetSms = (int)productManager.GetinitFnFOffNetSms(order.PLAN),
                    InitFreeData = (int)productManager.GetinitFreeData(order.PLAN),
                    InitFreeOnNetCalls = (int)productManager.GetinitFreeOnNetCalls(order.PLAN),
                    InitFreeOffNetCalls = (int)productManager.GetinitFreeOffNetCalls(order.PLAN),
                    InitFreeOnNetSms = (int)productManager.GetinitFreeOnNetSms(order.PLAN),
                    InitFreeOffNetSms = (int)productManager.GetinitFreeOffNetSms(order.PLAN),
                    Prime = (int)productManager.GetPrime(order.PLAN),
                    Msisdn = MSISDN,
                    planInfo = order.PLAN,
                    PlanName = "Consumer Postpaid Offer", ///
                    PrintedBill = printedBill,
                    SignUpChannel = "CRP",
                    SimCardNumber = account.SIM_SERIAL_NUMBER,
                };
                NumbersToPorts.Add(NumbersToPortPrincipal);

                //suppline
                var SuppLineAccounts = AccountService.GetAllSupplementaryLine(account.ROW_ID);
                foreach (var v in SuppLineAccounts)
                {
                    var SL_MSISDN = v.PersonalInfo.MSISDNNumber[0] == '6' ? v.PersonalInfo.MSISDNNumber.Substring(1) : v.PersonalInfo.MSISDNNumber;

                    var sl_dataPack = "SUPP18"; /////////////

                    var sl_order_plan = "Supp 18"; //////////////////

                    var sl_deposit = 0;
                    try
                    {
                        sl_deposit = (int)productManager.GetDepositByAccountId(v.AccountId);
                    }
                    catch { }
                    sl_deposit = sl_deposit == 0 ? 18 : sl_deposit;

                    NumbersToPorts.Add(new NumbersToPort()
                    {
                        AccountType = "POSTPAID",
                        AutoDebit = "Yes",
                        ContractPeriod = 24,
                        CreditLimit = (int)productManager.GetCreditLimit(sl_order_plan),
                        DataPack = sl_dataPack,
                        Deposit = sl_deposit,
                        InitFnFData = (int)productManager.GetinitFnFData(sl_order_plan),
                        InitFnFOnNetCalls = (int)productManager.GetinitFnFOnNetCalls(sl_order_plan),
                        InitFnFOffNetCalls = (int)productManager.GetinitFnFOffNetCalls(sl_order_plan),
                        InitFnFOnNetSms = (int)productManager.GetinitFnFOnNetSms(sl_order_plan),
                        InitFnFOffNetSms = (int)productManager.GetinitFnFOffNetSms(sl_order_plan),
                        InitFreeData = (int)productManager.GetinitFreeData(sl_order_plan),
                        InitFreeOnNetCalls = (int)productManager.GetinitFreeOnNetCalls(sl_order_plan),
                        InitFreeOffNetCalls = (int)productManager.GetinitFreeOffNetCalls(sl_order_plan),
                        InitFreeOnNetSms = (int)productManager.GetinitFreeOnNetSms(sl_order_plan),
                        InitFreeOffNetSms = (int)productManager.GetinitFreeOffNetSms(sl_order_plan),
                        Prime = (int)productManager.GetPrime(sl_order_plan),
                        Msisdn = SL_MSISDN,
                        planInfo = sl_order_plan,
                        PlanName = "Consumer Postpaid Offer", ///
                        PrintedBill = printedBill,
                        SignUpChannel = "CRP",
                        SimCardNumber = v.SIMSerialNumber,
                    });
                }

                string r = "";
                if (SuppLineAccounts.Count() > 0)
                {
                    NumberLists NumberList = new NumberLists();
                    NumberList.NumberList = new List<NumbersToPort>();
                    NumberList.NumberList = NumbersToPorts;

                    r = ExternalService.CreateMNPSubscriberSubs(new AddSubscriberProfileMNPSubs()
                    {
                        User = new User()
                        {
                            Source = "CRM",
                            UserId = UserId
                        },
                        SubscriberData = new SubscriberData()
                        {
                            address1 = address.AddressLine1,
                            address2 = address.AddressLine2,
                            address3 = "",
                            age = age,
                            city = address.City,
                            contactPhone1 = account.MOBILE_NO.Trim(),
                            customerName = account.NAME,
                            dateOfBirth = int.Parse(account.BIRTH_DT.Value.ToString("yyyyMMdd")),
                            donorId = CommonService.GetLookupValueByName(order.REMARKS, "Donor Mapping"),
                            emailAddress = account.EMAIL_ADDR,
                            gender = account.GENDER == 1 ? "Male" : "Female",
                            motherMaidenName = account.MOTHER_MAIDEN_NAME,
                            nationality = account.NATIONALITY == CommonService.GetLookupValueByName("MALAYSIA", "Nationality") ? 1 : 2,
                            portReqFormId = PortReqFormId,
                            postCode = address.Postcode,
                            race = CommonService.GetLookupValueByName(account.RACE, "Race Mapping"),
                            regType = ((IdentityTypeMNP)EnumUtil.ParseEnumInt<IdentityType>(account.ID_TYPE)).ToString(),
                            stateCode = int.Parse(StateCode == "" ? "0" : StateCode),
                            subsId = account.ID_NUM,
                        },
                        NumbersToPort = new JavaScriptSerializer().Serialize(NumberList)
                    });
                }
                else
                {
                    r = ExternalService.CreateMNPSubscriber(new AddSubscriberProfileMNP()
                    {
                        User = new User()
                        {
                            Source = "CRM",
                            UserId = UserId
                        },
                        SubscriberData = new SubscriberData()
                        {
                            address1 = address.AddressLine1,
                            address2 = address.AddressLine2,
                            address3 = "",
                            age = age,
                            city = address.City,
                            contactPhone1 = account.MOBILE_NO.Trim(),
                            customerName = account.NAME,
                            dateOfBirth = int.Parse(account.BIRTH_DT.Value.ToString("yyyyMMdd")),
                            donorId = CommonService.GetLookupValueByName(order.REMARKS, "Donor Mapping"),
                            emailAddress = account.EMAIL_ADDR,
                            gender = account.GENDER == 1 ? "Male" : "Female",
                            motherMaidenName = account.MOTHER_MAIDEN_NAME,
                            nationality = account.NATIONALITY == CommonService.GetLookupValueByName("MALAYSIA", "Nationality") ? 1 : 2,
                            portReqFormId = PortReqFormId,
                            postCode = address.Postcode,
                            race = CommonService.GetLookupValueByName(account.RACE, "Race Mapping"),
                            regType = ((IdentityTypeMNP)EnumUtil.ParseEnumInt<IdentityType>(account.ID_TYPE)).ToString(),
                            stateCode = int.Parse(StateCode == "" ? "0" : StateCode),
                            subsId = account.ID_NUM,
                        },
                        NumbersToPort = NumbersToPortPrincipal
                    });
                }

                var EAIresult = JObject.Parse(r);

                var EAIResultMessage = "";
                try
                {
                    EAIResultMessage = EAIresult["Result"].ToString(); ///
                }
                catch { }
                if (EAIResultMessage.ToLower() == "success") //sucessfully requested
                {
                    ////store PortReqFormId
                    //order.PORT_REQ_FORM = PortReqFormId;
                    //DBContext.SaveChanges();

                    return true;
                }
                else //failed
                {
                    //if (EAIResultMessage.ToLower().Contains("timed out"))
                    //{
                    //    //store PortReqFormId when timed out
                    //    order.PORT_REQ_FORM = PortReqFormId;
                    //    DBContext.SaveChanges();
                    //}

                    var assignee = (from d in DBContext.XOX_T_USER
                                    where d.ROW_ID == UserId
                                    select d.USERNAME).First();
                    try
                    {
                        EAIResultMessage = EAIresult["Result"].ToString();
                    }
                    catch
                    {
                        EAIResultMessage = EAIresult.ToString();
                    }
                    OrderActivityVO Activity = new OrderActivityVO();
                    Activity.ORDER_ID = OrderId;
                    Activity.ASSIGNEE = assignee;
                    Activity.REJECTED_REASON = "";
                    Activity.ACT_REMARKS = EAIResultMessage;
                    this.UpdateStatus(Activity, "Activation Failed");

                    return false;
                }
            }
        }

        public bool MNPSuppLinePortInRequest(long OrderId, int UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_ORDER
                             where d.ROW_ID == OrderId
                             select d;

                var order = result.First();

                var SupplementaryLine = ((int)AccountType.SupplementaryLine).ToString();
                var result2 = from d in DBContext.XOX_T_ORDER
                              join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                              join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                              where d.ROW_ID == order.ROW_ID && f.ACCNT_TYPE_CD == SupplementaryLine
                              select f;

                var account = result2.First();

                bool foreigner = account.ID_TYPE == "PassportNo" ? true : false;

                var address = AddressService.Get((long)account.ADDR_ID);

                var dataPack = productManager.GetDataPack(order.PLAN);

                var MSISDN = account.MSISDN[0] == '6' ? account.MSISDN.Substring(1) : account.MSISDN;

                int age = 0;
                if (account.BIRTH_DT != null)
                {
                    DateTime today = DateTime.Today;
                    age = today.Year - account.BIRTH_DT.Value.Year;
                    if (account.BIRTH_DT > today.AddYears(-age)) age--;
                }

                string StateCode = CommonService.GetLookupValueByName(address.State.ToUpper(), "State Code Mapping");

                string PortReqFormId = GetPortReqFormId(account, OrderId);
                order.PORT_REQ_FORM = PortReqFormId;
                DBContext.SaveChanges();

                var printedBill = "No";
                printedBill = this.GetPrintedBillingByOrder(OrderId) == true ? "Yes" : "No";

                List<NumbersToPort> NumbersToPorts = new List<NumbersToPort>();

                var NumbersToPortPrincipal = new NumbersToPort()
                {
                    AccountType = "POSTPAID",
                    AutoDebit = "Yes",
                    ContractPeriod = 24,
                    CreditLimit = (int)productManager.GetCreditLimit(order.PLAN),
                    DataPack = dataPack,
                    Deposit = (int)productManager.GetDeposit(order.ROW_ID),// + (foreigner == true ? XOXConstants.FOREIGNER_DEPOSIT : 0),
                    InitFnFData = (int)productManager.GetinitFnFData(order.PLAN),
                    InitFnFOnNetCalls = (int)productManager.GetinitFnFOnNetCalls(order.PLAN),
                    InitFnFOffNetCalls = (int)productManager.GetinitFnFOffNetCalls(order.PLAN),
                    InitFnFOnNetSms = (int)productManager.GetinitFnFOnNetSms(order.PLAN),
                    InitFnFOffNetSms = (int)productManager.GetinitFnFOffNetSms(order.PLAN),
                    InitFreeData = (int)productManager.GetinitFreeData(order.PLAN),
                    InitFreeOnNetCalls = (int)productManager.GetinitFreeOnNetCalls(order.PLAN),
                    InitFreeOffNetCalls = (int)productManager.GetinitFreeOffNetCalls(order.PLAN),
                    InitFreeOnNetSms = (int)productManager.GetinitFreeOnNetSms(order.PLAN),
                    InitFreeOffNetSms = (int)productManager.GetinitFreeOffNetSms(order.PLAN),
                    Prime = (int)productManager.GetPrime(order.PLAN),
                    Msisdn = MSISDN,
                    planInfo = order.PLAN,
                    PlanName = "Consumer Postpaid Offer", ///
                    PrintedBill = printedBill,
                    SignUpChannel = "CRP",
                    SimCardNumber = account.SIM_SERIAL_NUMBER,
                };
                NumbersToPorts.Add(NumbersToPortPrincipal);

                string r = ExternalService.CreateMNPSubscriber(new AddSubscriberProfileMNP()
                {
                    User = new User()
                    {
                        Source = "CRM",
                        UserId = UserId
                    },
                    SubscriberData = new SubscriberData()
                    {
                        address1 = address.AddressLine1,
                        address2 = address.AddressLine2,
                        address3 = "",
                        age = age,
                        city = address.City,
                        contactPhone1 = account.MOBILE_NO.Trim(),
                        customerName = account.NAME,
                        dateOfBirth = int.Parse(account.BIRTH_DT.Value.ToString("yyyyMMdd")),
                        donorId = CommonService.GetLookupValueByName(order.REMARKS, "Donor Mapping"),
                        emailAddress = account.EMAIL_ADDR,
                        gender = account.GENDER == 1 ? "Male" : "Female",
                        motherMaidenName = account.MOTHER_MAIDEN_NAME,
                        nationality = account.NATIONALITY == CommonService.GetLookupValueByName("MALAYSIA", "Nationality") ? 1 : 2,
                        portReqFormId = PortReqFormId,
                        postCode = address.Postcode,
                        race = CommonService.GetLookupValueByName(account.RACE, "Race Mapping"),
                        regType = ((IdentityTypeMNP)EnumUtil.ParseEnumInt<IdentityType>(account.ID_TYPE)).ToString(),
                        stateCode = int.Parse(StateCode == "" ? "0" : StateCode),
                        subsId = account.ID_NUM,
                    },
                    NumbersToPort = NumbersToPortPrincipal
                });

                var EAIresult = JObject.Parse(r);

                var EAIResultMessage = "";
                try
                {
                    EAIResultMessage = EAIresult["Result"].ToString(); ///
                }
                catch { }
                if (EAIResultMessage.ToLower() == "success") //sucessfully requested
                {
                    ////store PortReqFormId
                    //order.PORT_REQ_FORM = PortReqFormId;
                    //DBContext.SaveChanges();

                    return true;
                }
                else //failed
                {
                    //if (EAIResultMessage.ToLower().Contains("timed out"))
                    //{
                    //    //store PortReqFormId when timed out
                    //    order.PORT_REQ_FORM = PortReqFormId;
                    //    DBContext.SaveChanges();
                    //}

                    var assignee = (from d in DBContext.XOX_T_USER
                                    where d.ROW_ID == UserId
                                    select d.USERNAME).First();
                    try
                    {
                        EAIResultMessage = EAIresult["Result"].ToString();
                    }
                    catch
                    {
                        EAIResultMessage = EAIresult.ToString();
                    }
                    OrderActivityVO Activity = new OrderActivityVO();
                    Activity.ORDER_ID = OrderId;
                    Activity.ASSIGNEE = assignee;
                    Activity.REJECTED_REASON = "";
                    Activity.ACT_REMARKS = EAIResultMessage;
                    this.UpdateStatus(Activity, "Activation Failed");

                    return false;
                }
            }
        }

        public string ActivateMNPOrderStatus(string portId, string statusMsg, string rejectCode)
        {
            try
            {
                using (var DBContext = new CRMDbContext())
                {
                    var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                    var Order = from d in DBContext.XOX_T_ORDER
                                join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                                join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                where f.ACCNT_TYPE_CD == PrincipalLine &&
                                (d.PORT_ID == portId)
                                select d;
                    var Account = (from d in DBContext.XOX_T_ORDER
                                   join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                                   join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                   where f.ACCNT_TYPE_CD == PrincipalLine &&
                                   (d.PORT_ID == portId)
                                   select f).First();

                    if (Order.Count() > 0)
                    {
                        var OrderId = Order.First().ROW_ID;

                        OrderActivityVO Activity = new OrderActivityVO();
                        Activity.ORDER_ID = OrderId;
                        Activity.ASSIGNEE = "abc@abc.com"; ///
                        Activity.ACT_REMARKS = "MPGS - " + statusMsg;
                        if (rejectCode != "")
                        {
                            Activity.ACT_REMARKS += " RejectCode : " + rejectCode;
                        }
                        Activity.REJECTED_REASON = "";
                        Activity.ORDER_STATUS = "";
                        Activity.CREATED_BY = 1;
                        Activity.LAST_UPD_BY = 1;
                        Activity.DUE_DATE = DateTime.Now;
                        Activity.ACT_DESC = "Activating MNP Status";

                        long result = 0;
                        if (statusMsg == XOXConstants.STATUS_MSG_ACTIVE)
                        {

                            //check if Order Status is Active
                            if (Order.First().ORDER_STATUS == "Active")
                            {
                                OrderActivityVO Act = new OrderActivityVO();
                                Act.ORDER_ID = OrderId;
                                Act.ASSIGNEE = "abc@abc.com"; ///
                                Act.ACT_REMARKS = "MPGS - " + statusMsg + " - Failed: Order is already Active";
                                Act.REJECTED_REASON = "";
                                Act.ORDER_STATUS = "";
                                Act.CREATED_BY = 1;
                                Act.LAST_UPD_BY = 1;
                                Act.DUE_DATE = DateTime.Now;
                                Act.ACT_DESC = "Activating MNP Status";
                                OrderActivityService.AddActivity(Act);

                                throw new Exception("This order is already Active.");
                            }

                            //send sms
                            try
                            {
                                EAIService.SendSMS(new SendSMS()
                                {
                                    User = new User()
                                    {
                                        Source = "CRM",
                                        UserId = 1
                                    },
                                    MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : ('6' + Account.MSISDN),
                                    Message = "Dear Customer, your porting request has been approved. XOX"
                                });
                            }
                            catch { }

                            Activity.ORDER_STATUS = "Active";
                            Payment payment = GetAdvancePaymentRecord(Account.ROW_ID);

                            if (payment == null) //if payment got no record.
                            {
                                throw new Exception("Advance Payment must be recorded in CRM side.");
                            }
                            payment.Reference = Account.BANK_ISSUER + "-" + CommonService.MaskCreditCardDigit(Account.BILL_ACCNT_NUM);
                            payment.MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : '6' + Account.MSISDN;
                            payment.PaymentMethod = XOXConstants.PAYMENT_METHOD_CREDIT_CARD;
                            payment.User = new User()
                            {
                                Source = "CRM",
                                UserId = 1
                            };

                            var MakePaymentResult = AccountService.MakePayment(Account.ROW_ID, payment, false, 1);

                            if (MakePaymentResult != "true")
                            {
                                Activity.ACT_REMARKS = MakePaymentResult;
                                var us = UpdateStatus(Activity, "Activation Failed");
                            }
                            else
                            {
                                AccountService.ChangeAdvancePaymentFlagResponse(Account.ROW_ID, true);

                                //advance payment supp lines
                                var SupplementaryLine = ((int)AccountType.SupplementaryLine).ToString();
                                var suppline = from d in DBContext.XOX_T_ACCNT
                                               where d.PAR_ACCNT_ID == Account.ROW_ID
                                               && d.ACCNT_TYPE_CD == SupplementaryLine
                                               select d;

                                foreach (var v in suppline)
                                {
                                    try
                                    {
                                        Payment slpayment = GetAdvancePaymentRecord(v.ROW_ID);

                                        if (slpayment == null) //if slpayment got no record.
                                        {
                                            slpayment = new Payment()
                                            {
                                                Amount = "19.08",
                                                Reference = "",
                                            };
                                        }
                                        //reference from principal line
                                        slpayment.Reference = payment.Reference + " (Supp)";
                                        slpayment.MSISDN = v.MSISDN[0] == '6' ? v.MSISDN : '6' + v.MSISDN;
                                        slpayment.PaymentMethod = XOXConstants.PAYMENT_METHOD_CREDIT_CARD;
                                        slpayment.User = new User()
                                        {
                                            Source = "CRM",
                                            UserId = 1
                                        };

                                        var SLMakePaymentResult = AccountService.MakePayment(v.ROW_ID, slpayment, false, 1);

                                        if (SLMakePaymentResult == "true")
                                        {
                                            AccountService.ChangeAdvancePaymentFlagResponse(v.ROW_ID, true);
                                        }
                                    }
                                    catch
                                    {
                                        //////////
                                    }
                                }

                                var us = UpdateStatus(Activity, "Active");

                                result = us == true ? 1 : 0;
                            }
                        }
                        else if (statusMsg == XOXConstants.STATUS_MSG_REJECT)
                        {
                            //send sms
                            try
                            {
                                EAIService.SendSMS(new SendSMS()
                                {
                                    User = new User()
                                    {
                                        Source = "CRM",
                                        UserId = 1
                                    },
                                    MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : ('6' + Account.MSISDN),
                                    Message = "Dear Customer, your porting request has been rejected. Please check CRP for rejection reason. XOX"
                                });
                            }
                            catch { }

                            Activity.ORDER_STATUS = "Rejected";
                            var RejectCodeDesc = CommonService.GetLookupValueByName(rejectCode, "MNP Reject Code");
                            if (RejectCodeDesc != "")
                            {
                                Activity.REJECTED_REASON = rejectCode + " - " + RejectCodeDesc;
                            }
                            else
                            {
                                Activity.REJECTED_REASON += rejectCode + " - Description not found.";
                            }

                            var us = UpdateStatus(Activity, "Rejected");

                            result = us == true ? 1 : 0;
                        }
                        else
                        {
                            result = OrderActivityService.AddActivity(Activity);
                        }

                        if (result > 0)
                        {
                            return "Successfully done.";
                        }
                        else
                        {
                            return "An error occured.";
                        }
                    }
                    else
                    {
                        return "PortId not found.";
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string ActivateMNPSuppLineOrderStatus(string portId, string statusMsg, string rejectCode)
        {
            try
            {
                using (var DBContext = new CRMDbContext())
                {
                    var SupplementaryLine = ((int)AccountType.SupplementaryLine).ToString();
                    var Order = from d in DBContext.XOX_T_ORDER
                                join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                                join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                where f.ACCNT_TYPE_CD == SupplementaryLine &&
                                (d.PORT_ID == portId)
                                select d;
                    var Account = (from d in DBContext.XOX_T_ORDER
                                   join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                                   join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                   where f.ACCNT_TYPE_CD == SupplementaryLine &&
                                   (d.PORT_ID == portId)
                                   select f).First();

                    if (Order.Count() > 0)
                    {
                        var OrderId = Order.First().ROW_ID;

                        //check if Order Status is Active
                        if (Order.First().ORDER_STATUS == "Active")
                        {
                            OrderActivityVO Act = new OrderActivityVO();
                            Act.ORDER_ID = OrderId;
                            Act.ASSIGNEE = "abc@abc.com"; ///
                            Act.ACT_REMARKS = "MPGS - " + statusMsg + " - Failed: Order is already Active";
                            Act.REJECTED_REASON = "";
                            Act.ORDER_STATUS = "";
                            Act.CREATED_BY = 1;
                            Act.LAST_UPD_BY = 1;
                            Act.DUE_DATE = DateTime.Now;
                            Act.ACT_DESC = "Activating MNP Status";
                            OrderActivityService.AddActivity(Act);

                            throw new Exception("This order is already Active.");
                        }

                        OrderActivityVO Activity = new OrderActivityVO();
                        Activity.ORDER_ID = OrderId;
                        Activity.ASSIGNEE = "abc@abc.com"; ///
                        Activity.ACT_REMARKS = "MPGS - " + statusMsg;
                        if (rejectCode != "")
                        {
                            Activity.ACT_REMARKS += " RejectCode : " + rejectCode;
                        }
                        Activity.REJECTED_REASON = "";
                        Activity.ORDER_STATUS = "";
                        Activity.CREATED_BY = 1;
                        Activity.LAST_UPD_BY = 1;
                        Activity.DUE_DATE = DateTime.Now;
                        Activity.ACT_DESC = "Activating MNP Status";

                        long result = 0;
                        if (statusMsg == XOXConstants.STATUS_MSG_ACTIVE)
                        {
                            //send sms
                            try
                            {
                                EAIService.SendSMS(new SendSMS()
                                {
                                    User = new User()
                                    {
                                        Source = "CRM",
                                        UserId = 1
                                    },
                                    MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : ('6' + Account.MSISDN),
                                    Message = "Dear Customer, your porting request has been approved. XOX"
                                });
                            }
                            catch { }

                            Activity.ORDER_STATUS = "Active";

                            var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                            var ParentAccount = (from d in DBContext.XOX_T_ACCNT
                                                 where d.ROW_ID == Account.PAR_ACCNT_ID && d.ACCNT_TYPE_CD == PrincipalLine
                                                 select d).First();

                            Payment payment = GetAdvancePaymentRecord(Account.ROW_ID);

                            if (payment == null) //if payment got no record.
                            {
                                throw new Exception("Advance Payment must be recorded in CRM side.");
                            }
                            payment.Reference = ParentAccount.BANK_ISSUER + "-" + CommonService.MaskCreditCardDigit(ParentAccount.BILL_ACCNT_NUM) + " (supp)";
                            payment.MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : '6' + Account.MSISDN;
                            payment.PaymentMethod = XOXConstants.PAYMENT_METHOD_CREDIT_CARD;
                            payment.User = new User()
                            {
                                Source = "CRM",
                                UserId = 1
                            };

                            var MakePaymentResult = AccountService.MakePayment(Account.ROW_ID, payment, false, 1);

                            if (MakePaymentResult != "true")
                            {
                                Activity.ACT_REMARKS = MakePaymentResult;
                                var us = UpdateStatus(Activity, "Activation Failed");
                            }
                            else
                            {
                                AccountService.ChangeAdvancePaymentFlagResponse(Account.ROW_ID, true);
                                var us = UpdateStatus(Activity, "Active");

                                result = us == true ? 1 : 0;
                            }
                        }
                        else if (statusMsg == XOXConstants.STATUS_MSG_REJECT)
                        {
                            //send sms
                            try
                            {
                                EAIService.SendSMS(new SendSMS()
                                {
                                    User = new User()
                                    {
                                        Source = "CRM",
                                        UserId = 1
                                    },
                                    MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : ('6' + Account.MSISDN),
                                    Message = "Dear Customer, your porting request has been rejected. Please check CRP for rejection reason. XOX"
                                });
                            }
                            catch { }

                            Activity.ORDER_STATUS = "Rejected";
                            var RejectCodeDesc = CommonService.GetLookupValueByName(rejectCode, "MNP Reject Code");
                            if (RejectCodeDesc != "")
                            {
                                Activity.REJECTED_REASON = rejectCode + " - " + RejectCodeDesc;
                            }
                            else
                            {
                                Activity.REJECTED_REASON += rejectCode + " - Description not found.";
                            }

                            var us = UpdateStatus(Activity, "Rejected");

                            result = us == true ? 1 : 0;
                        }
                        else
                        {
                            result = OrderActivityService.AddActivity(Activity);
                        }

                        if (result > 0)
                        {
                            return "Successfully done.";
                        }
                        else
                        {
                            return "An error occured.";
                        }
                    }
                    else
                    {
                        return "PortId not found.";
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string ActivateMNPOrderRequest(string PortReqFormId, string portId)
        {
            try
            {
                using (var DBContext = new CRMDbContext())
                {
                    var Order = from d in DBContext.XOX_T_ORDER
                                join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                                join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                where d.PORT_REQ_FORM == PortReqFormId
                                select new
                                {
                                    Order = d,
                                    Account = f
                                };

                    if (Order.First().Order.ORDER_TYPE == "New Registration")
                    {
                        return ActivateMNPOrder(PortReqFormId, portId);
                    }
                    else if (Order.First().Order.ORDER_TYPE == "Supplementary Registration")
                    {
                        return ActivateMNPOrderSuppLine(PortReqFormId, portId);
                    }
                }
            }
            catch { }

            return null;
        }

        public string ActivateMNPOrderStatusRequest(string portId, string statusMsg, string rejectCode)
        {
            try
            {
                using (var DBContext = new CRMDbContext())
                {
                    var Order = from d in DBContext.XOX_T_ORDER
                                join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                                join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                where d.PORT_ID == portId
                                select d;

                    if (Order.First().ORDER_TYPE == "New Registration")
                    {
                        return ActivateMNPOrderStatus(portId, statusMsg, rejectCode);
                    }
                    else if (Order.First().ORDER_TYPE == "Supplementary Registration")
                    {
                        return ActivateMNPSuppLineOrderStatus(portId, statusMsg, rejectCode);
                    }
                }
            }
            catch { }

            return null;
        }

        public string ActivateMNPOrder(string PortReqFormId, string portId)
        {
            try
            {
                using (var DBContext = new CRMDbContext())
                {
                    var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                    var Order = from d in DBContext.XOX_T_ORDER
                                join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                                join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                where f.ACCNT_TYPE_CD == PrincipalLine &&
                                (d.PORT_REQ_FORM == PortReqFormId)
                                select new
                                {
                                    Order = d,
                                    Account = f
                                };

                    if (Order.Count() > 0)
                    {
                        var OrderId = Order.First().Order.ROW_ID;

                        OrderActivityVO Activity = new OrderActivityVO();
                        Activity.ORDER_ID = OrderId;
                        Activity.ASSIGNEE = "abc@abc.com"; ///
                        Activity.ACT_REMARKS = "Received PortId - " + portId;
                        Activity.REJECTED_REASON = "";
                        Activity.ORDER_STATUS = "";
                        Activity.CREATED_BY = 1;
                        Activity.LAST_UPD_BY = 1;
                        Activity.DUE_DATE = DateTime.Now;
                        Activity.ACT_DESC = "Activating MNP Pack";

                        var result = OrderActivityService.AddActivity(Activity);
                        //var result = ActivateOrder(Activity, 1);

                        if (result != 0)
                        {
                            Order.First().Order.PORT_ID = portId;
                            DBContext.SaveChanges();

                            try
                            {
                                //send sms
                                EAIService.SendSMS(new SendSMS()
                                {
                                    User = new User()
                                    {
                                        Source = "CRM",
                                        UserId = 1
                                    },
                                    MSISDN = Order.First().Account.MSISDN[0] == '6' ? Order.First().Account.MSISDN : ('6' + Order.First().Account.MSISDN),
                                    Message = "Dear Customer, we have received your porting request dated " + DateTime.Now.ToString("dd MMM yyyy") + ". It will be processed shortly. XOX"
                                });
                            }
                            catch { }

                            return "Successfully Activated.";
                        }
                        else
                        {
                            string ACT_REMARKS = "Unknown error occured.";
                            try
                            {
                                ACT_REMARKS = (from d in DBContext.XOX_T_ORDER_ACT
                                               where d.ORDER_ID == OrderId
                                               orderby d.ROW_ID descending
                                               select d.ACT_REMARKS).First();
                            }
                            catch { }
                            return ACT_REMARKS;
                        }
                    }
                    else
                    {
                        return "PortReqFormId not found.";
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string ActivateMNPOrderSuppLine(string PortReqFormId, string portId)
        {
            try
            {
                using (var DBContext = new CRMDbContext())
                {
                    var SupplementaryLine = ((int)AccountType.SupplementaryLine).ToString();
                    var Order = from d in DBContext.XOX_T_ORDER
                                join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                                join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                where f.ACCNT_TYPE_CD == SupplementaryLine &&
                                (d.PORT_REQ_FORM == PortReqFormId)
                                select new
                                {
                                    Order = d,
                                    Account = f
                                };

                    if (Order.Count() > 0)
                    {
                        var OrderId = Order.First().Order.ROW_ID;

                        OrderActivityVO Activity = new OrderActivityVO();
                        Activity.ORDER_ID = OrderId;
                        Activity.ASSIGNEE = "abc@abc.com"; ///
                        Activity.ACT_REMARKS = "Received PortId - " + portId;
                        Activity.REJECTED_REASON = "";
                        Activity.ORDER_STATUS = "";
                        Activity.CREATED_BY = 1;
                        Activity.LAST_UPD_BY = 1;
                        Activity.DUE_DATE = DateTime.Now;
                        Activity.ACT_DESC = "Activating MNP Pack";

                        var result = OrderActivityService.AddActivity(Activity);
                        //var result = ActivateOrder(Activity, 1);

                        if (result != 0)
                        {
                            Order.First().Order.PORT_ID = portId;
                            DBContext.SaveChanges();

                            try
                            {
                                //send sms
                                EAIService.SendSMS(new SendSMS()
                                {
                                    User = new User()
                                    {
                                        Source = "CRM",
                                        UserId = 1
                                    },
                                    MSISDN = Order.First().Account.MSISDN[0] == '6' ? Order.First().Account.MSISDN : ('6' + Order.First().Account.MSISDN),
                                    Message = "Dear Customer, we have received your porting request dated " + DateTime.Now.ToString("dd MMM yyyy") + ". It will be processed shortly. XOX"
                                });
                            }
                            catch { }

                            return "Successfully Activated.";
                        }
                        else
                        {
                            string ACT_REMARKS = "Unknown error occured.";
                            try
                            {
                                ACT_REMARKS = (from d in DBContext.XOX_T_ORDER_ACT
                                               where d.ORDER_ID == OrderId
                                               orderby d.ROW_ID descending
                                               select d.ACT_REMARKS).First();
                            }
                            catch { }
                            return ACT_REMARKS;
                        }
                    }
                    else
                    {
                        return "PortReqFormId not found.";
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string FailActivateMNPOrder(string PortReqFormId, string MSISDN, string Remarks)
        {
            try
            {
                using (var DBContext = new CRMDbContext())
                {
                    var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                    var Order = from d in DBContext.XOX_T_ORDER
                                join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                                join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                                where f.ACCNT_TYPE_CD == PrincipalLine &&
                                (d.PORT_REQ_FORM == PortReqFormId || (f.MSISDN == "6" + MSISDN || f.MSISDN == MSISDN))
                                select d;

                    if (Order.Count() > 0)
                    {
                        var OrderId = Order.First().ROW_ID;
                        OrderActivityVO Activity = new OrderActivityVO();
                        Activity.ORDER_ID = OrderId;
                        Activity.ASSIGNEE = "abc@abc.com"; ///
                        Activity.ACT_REMARKS = Remarks;
                        Activity.REJECTED_REASON = "";
                        Activity.CREATED_BY = 1;
                        Activity.LAST_UPD_BY = 1;

                        var result = this.UpdateStatus(Activity, "Activation Failed");

                        if (result == true)
                        {
                            return "Successfully Failed Activation.";
                        }
                        else
                        {
                            return "An error occurred.";
                        }
                    }
                    else
                    {
                        return "PortReqFormId or MSISDN not found.";
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public long UpdateOrderSupplementary(string MSISDN, long SuppAccountId)
        {
            long OrderId = 0;
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_ORDER_ITEM
                             join e in dbContext.XOX_T_PROD on d.PROD_ID equals e.ROW_ID
                             join f in dbContext.XOX_T_ACCNT on d.CUST_ID equals f.ROW_ID
                             where f.ROW_ID == SuppAccountId && e.PRD_TYPE == "Supplementary"
                             select d;

                if (result.Count() > 0)
                {
                    bool isSameOrder = result.Select(x => x.ORDER_ID).Distinct().Count() < 2;

                    if (isSameOrder == true)
                    {
                        foreach (var v in result)
                        {
                            v.CUST_ID = SuppAccountId;

                            OrderId = v.ORDER_ID;
                        }

                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    var result2 = from d in dbContext.XOX_T_ORDER_ITEM
                                  join e in dbContext.XOX_T_PROD on d.PROD_ID equals e.ROW_ID
                                  join f in dbContext.XOX_T_ACCNT on d.CUST_ID equals f.ROW_ID
                                  where f.MSISDN == MSISDN && e.PRD_TYPE != "Supplementary"
                                  select d.ORDER_ID;

                    bool isSameOrder2 = result2.Select(x => x).Distinct().Count() < 2;

                    if (isSameOrder2 == true)
                    {
                        OrderId = result2.First();
                    }
                }
            }

            return OrderId;
        }

        public bool ResubmitOrder(long OrderId, OrderVO OrderVO, long UserId)
        {
            using (var dbContext = new CRMDbContext())
            {
                var Order = from d in dbContext.XOX_T_ORDER
                            where d.ROW_ID == OrderId
                            select d;

                if (Order.Count() > 0)
                {
                    var v = Order.First();

                    //add activity
                    OrderActivityVO Activity = new OrderActivityVO();
                    Activity.ACT_DESC = "Changing Order Status from " + v.ORDER_STATUS + " to Resubmitted by Dealer.";
                    Activity.DUE_DATE = DateTime.Now; ///
                    Activity.CREATED_BY = 1;
                    Activity.ACT_REMARKS = "";
                    Activity.REJECTED_REASON = "";
                    Activity.ASSIGNEE = "abc@abc.com"; ///
                    Activity.ORDER_ID = OrderId;
                    Activity.ORDER_STATUS = XOXConstants.ORDER_STATUS_RESUBMITTED;
                    OrderActivityService.AddActivity(Activity);

                    //change order properties
                    v.CUST_REP_ID = OrderVO.CUST_REP_ID;
                    v.ASSIGNEE = OrderVO.ASSIGNEE;
                    v.ORDER_SOURCE = OrderVO.ORDER_SOURCE;
                    v.ORDER_TYPE = OrderVO.ORDER_TYPE;
                    v.PLAN = OrderVO.PLAN;
                    v.CATEGORY = OrderVO.CATEGORY;
                    v.REMARKS = OrderVO.REMARKS;
                    v.ORDER_SUBMIT_DT = OrderVO.ORDER_SUBMIT_DT;

                    v.ORDER_STATUS = "Resubmitted";
                    v.LAST_UPD = DateTime.Now;
                    v.LAST_UPD_BY = UserId;

                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    throw new Exception("Order Id not found.");
                }
            }
        }

        public long GetOrderIdByAccountId(long AccountId, string OrderType = "New Registration")
        {
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_ORDER_ITEM
                             join e in dbContext.XOX_T_ORDER on d.ORDER_ID equals e.ROW_ID
                             where d.CUST_ID == AccountId
                             && e.ORDER_TYPE == OrderType
                             orderby d.ROW_ID descending
                             select e.ROW_ID;

                if (result.Count() > 0)
                {
                    return result.First();
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool AddFiles(List<String> filespath, long OrderId, long UserId = 0)
        {
            if (UserId == 0)
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var user = from d in DBContext.XOX_T_ORDER
                           where d.ROW_ID == OrderId
                           select d;

                if (user.Count() > 0)
                {
                    foreach (var filepath in filespath)
                    {
                        XOX_T_ORDER_ATT n = new XOX_T_ORDER_ATT();
                        n.ORDER_ID = OrderId;
                        n.FILE_PATH_NAME = filepath;
                        n.CREATED = DateTime.Now;
                        n.CREATED_BY = UserId;

                        DBContext.XOX_T_ORDER_ATT.Add(n);
                        DBContext.SaveChanges();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Edit(long OrderId, OrderDetailVO NewOrder, long UserId = 0)
        {
            if (UserId == 0)
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var order = from d in DBContext.XOX_T_ORDER
                            where d.ROW_ID == OrderId
                            select d;

                if (order.Count() > 0)
                {
                    order.First().REMARKS = NewOrder.Remarks;
                    order.First().CATEGORY = NewOrder.Category;
                    order.First().PLAN = NewOrder.SubscriptionPlan;
                    DBContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<OrderVO> GetRejectedOrder(long ResubmittedOrderId)
        {
            List<OrderVO> list = new List<OrderVO>();
            using (var DBContext = new CRMDbContext())
            {
                var AccountId = AccountService.GetAccountIdByOrderId(ResubmittedOrderId);
                var RejectedOrders = (from d in DBContext.XOX_T_ORDER_ITEM
                                      join e in DBContext.XOX_T_ORDER on d.ORDER_ID equals e.ROW_ID
                                      where d.CUST_ID == AccountId && e.ORDER_STATUS == "Rejected"
                                      select e).Distinct();
                foreach (var v in RejectedOrders)
                {
                    list.Add(new OrderVO()
                    {
                        ROW_ID = v.ROW_ID,
                        ORDER_NUM = v.ORDER_NUM
                    });
                }
            }

            return list;
        }

        public void TerminateOrder(long custAccountId, string TerminationDate)
        {
            using (var DBContext = new CRMDbContext())
            {
                var ASSET = from d in DBContext.XOX_T_ASSET
                            join e in DBContext.XOX_T_PROD_ITEM on d.PROD_ID equals e.ROW_ID
                            join f in DBContext.XOX_T_PROD on e.PROD_ID equals f.ROW_ID
                            join g in DBContext.XOX_T_ACCNT on d.CUST_ID equals g.ROW_ID
                            where d.CUST_ID == custAccountId
                            select new
                            {
                                ASSET = d,
                                PROD = f,
                                ACCNT = g
                            };

                if (ASSET.Count() > 0)
                {
                    var AssetDetail = ASSET.First();
                    var Plan = AssetDetail.PROD.EXT_PROD_NAME;

                    //change asset status first before create new termination order
                    var AssetsToBeChanged = from d in DBContext.XOX_T_ASSET
                                            where d.CUST_ID == custAccountId
                                            select d;
                    foreach (var v in AssetsToBeChanged)
                    {
                        v.STATUS_CD = "Inactive";
                    }
                    DBContext.SaveChanges();


                    var OrdersIdToBeTerminated = (from d in DBContext.XOX_T_ORDER_ITEM
                                                  where d.CUST_ID == custAccountId
                                                  select d.ORDER_ID).Distinct().ToList();

                    #region Termination order header
                    OrderVO _terminateOrder = new OrderVO();
                    _terminateOrder.CREATED_BY = 1;
                    _terminateOrder.ASSIGNEE = XOXConstants.SYSTEM_USER;
                    _terminateOrder.ORDER_SOURCE = XOXConstants.SYSTEM_NAME;
                    _terminateOrder.ORDER_TYPE = XOXConstants.ORDER_TYPE_TERMINATE;
                    _terminateOrder.CUST_REP_ID = string.Empty;
                    _terminateOrder.CATEGORY = XOXConstants.OFFER_CODE_POSTPAID;
                    _terminateOrder.PLAN = Plan;
                    _terminateOrder.REMARKS = string.Empty;
                    _terminateOrder.ORDER_SUBMIT_DT = DateTime.Now;
                    _terminateOrder.ORDER_STATUS = "0"; // 0 is Terminated, check AddOrderHeader function
                    _terminateOrder.CustomerAccountId = custAccountId;
                    //this.AddOrderHeader(_terminateOrder, null);
                    #endregion

                    #region Termination order item
                    var ProductItems = productManager.GetProductItemByPlan(Plan);

                    var selectedProds = ProductItems.Where(d => d.VAS_FLG == false).ToList();
                    //if (AssetDetail.ASSET.FlgReceivedItemised == true)
                    //{
                    //    selectedProds.Add(ProductItems.Where(d => d.EXT_PROD_NAME == "Printed Billing").First());
                    //}
                    if (AssetDetail.ACCNT.ID_TYPE == IdentityType.PassportNo.ToString())
                    {
                        selectedProds.Add(ProductItems.Where(d => d.EXT_PROD_NAME == "Foreigner Deposit").First());
                    }

                    List<long> _selectedProducts = new List<long>();
                    foreach (var v in selectedProds)
                    {
                        _selectedProducts.Add(v.ROW_ID);
                    }
                    #endregion

                    var NewTerminateOrderId = this.CreateNewOrder(_terminateOrder, _selectedProducts, AssetDetail.ACCNT.MSISDN, "", false, true);

                    //update status to CRP
                    AccountActivityVO AccountActivity = new AccountActivityVO();
                    AccountActivity.ACCNT_ID = custAccountId;
                    AccountActivity.REASON = "";
                    AccountActivity.ASSIGNEE = "abc@abc.com";
                    AccountActivity.CREATED_BY = 1;
                    AccountActivity.LAST_UPD_BY = 1;

                    AccountService.UpdateStatus(AccountActivity, AccountStatus.Terminated.ToString(), "", DateTime.ParseExact(TerminationDate.Trim(), "yyyy-MM-dd HH:mm:ss", null));

                    //add activity to new terminate order
                    OrderActivityVO Activity = new OrderActivityVO();
                    Activity.ACT_DESC = "Created New Terminated Order and Synced to CRP.";
                    Activity.DUE_DATE = DateTime.Now; ///
                    Activity.CREATED_BY = 1;
                    Activity.ACT_REMARKS = "";
                    Activity.REJECTED_REASON = "";
                    Activity.ASSIGNEE = "abc@abc.com"; ///
                    Activity.ORDER_ID = NewTerminateOrderId;
                    Activity.ORDER_STATUS = "";
                    OrderActivityService.AddActivity(Activity);
                }
                else
                {
                    throw new Exception("No Asset Found");
                }
            }
        }

        public long ChangePlanRequest(long AccountId, string NewPlan, DateTime RequestDate, long UserId = 0)
        {
            if (UserId == 0)
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            var Account = AccountService.Get(AccountId);
            #region Update to MPP
            //process to mpp
            var MSISDN = Account.PersonalInfo.MSISDNNumber[0] == '6' ? Account.PersonalInfo.MSISDNNumber : ('6' + Account.PersonalInfo.MSISDNNumber);

            var PlanUpdateRequestResult = EAIService.PlanUpdateRequest(new PlanUpdateRequest()
            {
                User = new User()
                {
                    Source = "CRM",
                    UserId = (int)UserId
                },

                MSISDN = MSISDN,
                DataPack = productManager.GetDataPack(NewPlan),
                Deposit = productManager.GetPrime(NewPlan), //same value as Prime
                InitFreeOnNetCalls = productManager.GetinitFreeOnNetCalls(NewPlan),
                InitFreeOffNetCalls = productManager.GetinitFreeOffNetCalls(NewPlan),
                InitFreeOnNetSMS = productManager.GetinitFreeOnNetSms(NewPlan),
                InitFreeOffNetSMS = productManager.GetinitFreeOffNetSms(NewPlan),
                InitFnFOffNetCalls = productManager.GetinitFnFOffNetCalls(NewPlan),
            });
            var r = JObject.Parse(PlanUpdateRequestResult);

            string response = "";
            string message = "";
            try
            {
                response = r["Result"]["status"].ToString();
                message = r["Result"]["message"].ToString();
            }
            catch { }
            #endregion

            var subProfile = AccountService.GetSubscriberProfile(AccountId);

            var parameter = subProfile.parameter.ToDictionary(v => v.name, v => v.value);
            bool flgReceivedItemisedBilling = parameter["PrintedBill"] == "No" ? false : true;

            bool flgForeigner = Account.PersonalInfo.IdentityType == (int)IdentityType.PassportNo ? true : false;

            var ProductItems = productManager.GetProductItemByPlan(NewPlan);

            var selectedProds = ProductItems.Where(d => d.VAS_FLG == false).ToList();
            if (flgReceivedItemisedBilling == true)
            {
                selectedProds.Add(ProductItems.Where(d => d.EXT_PROD_NAME == "Printed Billing").First());
            }
            if (flgForeigner == true)
            {
                selectedProds.Add(ProductItems.Where(d => d.EXT_PROD_NAME == "Foreigner Deposit").First());
            }

            List<long> _selectedProducts = new List<long>();
            foreach (var v in selectedProds)
            {
                _selectedProducts.Add(v.ROW_ID);
            }

            var OldPlan = "";
            OrderVO OrderVO = new OrderVO();
            using (var dbContext = new CRMDbContext())
            {
                var Order = (from d in dbContext.XOX_T_ACCNT
                             join e in dbContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.CUST_ID
                             join f in dbContext.XOX_T_ORDER on e.ORDER_ID equals f.ROW_ID
                             where d.ROW_ID == AccountId
                             orderby f.ROW_ID descending
                             select f).First();

                OldPlan = Order.PLAN;

                OrderVO.CUST_REP_ID = AccountId.ToString();
                OrderVO.CustomerAccountId = AccountId;
                OrderVO.ORDER_SOURCE = XOXConstants.ORDER_SOURCE_CRP;
                OrderVO.ORDER_TYPE = "Change Plan";
                OrderVO.CREATED_BY = UserId;
                OrderVO.PLAN = NewPlan;
                OrderVO.CATEGORY = "CP";
                OrderVO.REMARKS = OldPlan;
                OrderVO.ORDER_SUBMIT_DT = DateTime.Now;
                OrderVO.PREF_INSTALL_DT = RequestDate;
                OrderVO.ORDER_STATUS = "1";
            }

            var result = CreateNewOrder(OrderVO, _selectedProducts, Account.PersonalInfo.MSISDNNumber, "Changed Plan");

            using (var dbContext = new CRMDbContext())
            {
                var NewOrder = (from d in dbContext.XOX_T_ORDER
                                where d.ROW_ID == result
                                select d).First();

                NewOrder.ORDER_STATUS = "Incomplete";
                dbContext.SaveChanges();
            }

            OrderActivityService.AddActivity(new OrderActivityVO()
            {
                ACT_DESC = "Changing Plan from " + OldPlan + " to " + NewPlan,
                DUE_DATE = DateTime.Now, ///
                CREATED_BY = UserId,
                ACT_REMARKS = "",
                REJECTED_REASON = "",
                ASSIGNEE = UserService.UserGet((int)UserId).Username,
                ORDER_ID = result,
                ORDER_STATUS = "Incomplete",
            });

            return result;
        }

        public void ChangePlan(long OrderId, long UserId = 0)
        {
            if (UserId == 0)
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var dbContext = new CRMDbContext())
            {
                var AccountId = AccountService.GetAccountIdByOrderId(OrderId);
                var Account = (from d in dbContext.XOX_T_ACCNT
                               where d.ROW_ID == AccountId
                               select d).First();
                var Order = (from d in dbContext.XOX_T_ORDER
                             where d.ROW_ID == OrderId
                             select d).First();

                #region CRM Asset Processing
                //Get active assets aand change Asset status to Inactive
                var OldAsset = (from d in dbContext.XOX_T_ASSET
                                where d.CUST_ID == AccountId && d.STATUS_CD == XOXConstants.STATUS_ACTIVE_CD
                                select d).FirstOrDefault();

                if (OldAsset != null)
                {
                    OldAsset.STATUS_CD = XOXConstants.STATUS_INACTIVE_CD;
                    dbContext.SaveChanges();
                }
                #endregion

                #region Update to MPP
                //process to mpp
                var MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : ('6' + Account.MSISDN);

                var result = EAIService.PlanUpdate(new PlanUpdate()
                {
                    User = new User()
                    {
                        Source = "CRM",
                        UserId = (int)UserId
                    },

                    MSISDN = MSISDN,
                    CreditLimit = productManager.GetCreditLimit(Order.PLAN).ToString(),
                    Prime = productManager.GetPrime(Order.PLAN),
                    Plan = Order.PLAN
                });
                var r = JObject.Parse(result);

                string response = "";
                string message = "";
                try
                {
                    response = r["Result"]["status"].ToString();
                    message = r["Result"]["message"].ToString();
                }
                catch { }
                #endregion

                #region CRM Update order status
                if (response.ToLower() == "true")
                {
                    //if success
                    var EAIResult = EAIService.GetSubscriberProfile(new GetSubscriberProfile()
                    {
                        MSISDN = MSISDN,
                        User = new User()
                        {
                            UserId = (int)UserId
                        }
                    });
                    var rGetSubscriberProfile = JObject.Parse(EAIResult);

                    GetSubscriberProfileResponse responseGetSubscriberProfile = null;
                    try
                    {
                        responseGetSubscriberProfile = rGetSubscriberProfile["Result"].ToObject<GetSubscriberProfileResponse>();
                    }
                    catch
                    {
                        throw new Exception(rGetSubscriberProfile["Result"].ToString());
                    }

                    //add asset for changed plan order
                    AssetService.ConvertAssetFromPlan(AccountService.Get(Account.ROW_ID), responseGetSubscriberProfile);
                                        
                    //set status for changed plan
                    Order.ORDER_STATUS = XOXConstants.STATUS_COMPLETE_CD;
                    OrderActivityService.AddActivity(new OrderActivityVO()
                    {
                        ACT_DESC = message + "Successfully changed Plan to " + Order.PLAN,
                        DUE_DATE = DateTime.Now, ///
                        CREATED_BY = UserId,
                        ACT_REMARKS = "",
                        REJECTED_REASON = "",
                        ASSIGNEE = UserService.UserGet((int)UserId).Username,
                        ORDER_ID = OrderId,
                        ORDER_STATUS = XOXConstants.STATUS_COMPLETE_CD,
                    });

                    dbContext.SaveChanges();
                }
                else
                {
                    //set status for changed plan
                    Order.ORDER_STATUS = XOXConstants.STATUS_INCOMPLETE_CD;
                    dbContext.SaveChanges();

                    try
                    {
                        OrderActivityService.AddActivity(new OrderActivityVO()
                        {
                            ACT_DESC = r["Result"].ToString(),
                            DUE_DATE = DateTime.Now, ///
                            CREATED_BY = UserId,
                            ACT_REMARKS = "",
                            REJECTED_REASON = "",
                            ASSIGNEE = UserService.UserGet((int)UserId).Username,
                            ORDER_ID = OrderId,
                            ORDER_STATUS = XOXConstants.STATUS_INCOMPLETE_CD,
                        });

                        throw new Exception(r["Result"].ToString());
                    }
                    catch
                    {
                        OrderActivityService.AddActivity(new OrderActivityVO()
                        {
                            ACT_DESC = XOXExceptions.ORDER_01_FAILED_CHANGE_PLAN,
                            DUE_DATE = DateTime.Now,
                            CREATED_BY = UserId,
                            ACT_REMARKS = "",
                            REJECTED_REASON = "",
                            ASSIGNEE = UserService.UserGet((int)UserId).Username,
                            ORDER_ID = OrderId,
                            ORDER_STATUS = XOXConstants.STATUS_INCOMPLETE_CD,
                        });

                        throw new Exception("Failed syncing to MPP");
                    }
                }
                #endregion
            }
        }

        public bool GetPrintedBillingByOrder(long OrderId)
        {
            using (var dbContext = new CRMDbContext())
            {
                var OrderItem = from d in dbContext.XOX_T_ORDER_ITEM
                                join e in dbContext.XOX_T_PROD_ITEM on d.PROD_ID equals e.ROW_ID
                                join f in dbContext.XOX_T_PROD on e.PROD_ID equals f.ROW_ID
                                where d.ORDER_ID == OrderId
                                && f.EXT_PROD_NAME == XOXConstants.PRODUCT_PRINTED_BILLING
                                && d.STATUS_CD == XOXConstants.STATUS_ACTIVE_CD
                                select d;

                if (OrderItem.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
