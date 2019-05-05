using CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib
{
    public class ProductService : IProductService
    {
        public List<ProductVO> GetAllProducts()
        {
            List<ProductVO> allProducts = new List<ProductVO>();
            using (var dbContext = new CRMDbContext())
            {
                var productList = from x in dbContext.XOX_T_PROD_ITEM
                                  join y in dbContext.XOX_T_PROD on x.PROD_ID equals y.ROW_ID
                                  select new
                                  {
                                      PROD_ITEM_ID = x.PROD_ITEM_ID,
                                      ROW_ID = x.ROW_ID,
                                      PAR_ITEM_ID = x.PAR_ITEM_ID,
                                      ROOT_ITEM_ID = x.ROOT_ITEM_ID,
                                      PRD_DESC = y.PRD_DESC,
                                      VAS_FLG = y.VAS_FLG,
                                      PRD_LVL = y.PRD_LVL,
                                      PRD_PRICE = y.PRD_PRICE,
                                      PRD_CATEGORY = y.PRD_CATEGORY,
                                      PRD_TYPE = y.PRD_TYPE,
                                      PRD_PRICE_TYPE = y.PRD_PRICE_TYPE,
                                      QUOTA = y.QUOTA,
                                      EXT_PROD_NAME = y.EXT_PROD_NAME,
                                      GST_CD = y.GST_CD,
                                      GST_PT = y.GST_PT,
                                      DISPLAY_FLG = y.DISPLAY_FLG
                                  };
                foreach (var p in productList)
                {
                    ProductVO tempProduct = new ProductVO();
                    tempProduct.ROW_ID = p.ROW_ID;
                    tempProduct.PROD_ITEM_ID = p.PROD_ITEM_ID;
                    tempProduct.PAR_ITEM_ID = p.PAR_ITEM_ID ?? 0;
                    tempProduct.PRD_DESC = p.PRD_DESC;
                    tempProduct.VAS_FLG = p.VAS_FLG == "Y" ? true : false; //Boolean.Parse(p.VAS_FLG);
                    tempProduct.PRD_LVL = p.PRD_LVL;
                    tempProduct.PRD_PRICE = p.PRD_PRICE;
                    tempProduct.PRD_CATEGORY = p.PRD_CATEGORY;
                    tempProduct.PRD_TYPE = p.PRD_TYPE;
                    tempProduct.PRD_PRICE_TYPE = p.PRD_PRICE_TYPE;
                    tempProduct.QUOTA = p.QUOTA;
                    tempProduct.ROOT_ITEM_ID = p.ROOT_ITEM_ID ?? 0;
                    tempProduct.EXT_PROD_NAME = p.EXT_PROD_NAME;
                    tempProduct.GST_CD = p.GST_CD == null ? "" : p.GST_CD;
                    tempProduct.GST_PT = p.GST_PT == null ? 0 : (int)p.GST_PT;
                    tempProduct.DISPLAY_FLG = p.DISPLAY_FLG == null ? "N" : p.DISPLAY_FLG;

                    // Additional processing for product display
                    tempProduct.Is_Package = p.PAR_ITEM_ID == null ? true : false;
                    tempProduct.Parent_Package_ID = tempProduct.ROOT_ITEM_ID;
                    allProducts.Add(tempProduct);
                }
            }
            return allProducts;
        }

        public List<string> GetAllPrincipalPlan()
        {
            List<ProductVO> allProducts = new List<ProductVO>();
            using (var dbContext = new CRMDbContext())
            {
                var productList = from x in dbContext.XOX_T_PROD_ITEM
                                  join y in dbContext.XOX_T_PROD on x.PROD_ID equals y.ROW_ID
                                  select new
                                  {
                                      PROD_ITEM_ID = x.PROD_ITEM_ID,
                                      ROW_ID = x.ROW_ID,
                                      PAR_ITEM_ID = x.PAR_ITEM_ID,
                                      ROOT_ITEM_ID = x.ROOT_ITEM_ID,
                                      PRD_DESC = y.PRD_DESC,
                                      VAS_FLG = y.VAS_FLG,
                                      PRD_LVL = y.PRD_LVL,
                                      PRD_PRICE = y.PRD_PRICE,
                                      PRD_CATEGORY = y.PRD_CATEGORY,
                                      PRD_TYPE = y.PRD_TYPE,
                                      PRD_PRICE_TYPE = y.PRD_PRICE_TYPE,
                                      QUOTA = y.QUOTA,
                                      EXT_PROD_NAME = y.EXT_PROD_NAME,
                                      GST_CD = y.GST_CD,
                                      GST_PT = y.GST_PT,
                                      DISPLAY_FLG = y.DISPLAY_FLG
                                  };

                var result = productList.Where(m => m.PRD_TYPE.ToLower() == "principal" || m.PRD_TYPE.ToLower() == "supplementary").Select(m => m.EXT_PROD_NAME);

                return result.ToList();
            }
        }

        public List<ProductVO> GetProductItemByPlan(string Plan)
        {
            List<ProductVO> result = new List<ProductVO>();
            using (var dbContext = new CRMDbContext())
            {
                var ParentProduct = (from d in dbContext.XOX_T_PROD
                                     join e in dbContext.XOX_T_PROD_ITEM on d.ROW_ID equals e.PROD_ID
                                     where d.EXT_PROD_NAME == Plan
                                     select new
                                     {
                                         PROD_ITEM_ROW_ID = e.ROW_ID,
                                         EXT_PROD_NAME = d.EXT_PROD_NAME,
                                         VAS_FLG = d.VAS_FLG,
                                         PAR_ITEM_ID = e.PAR_ITEM_ID
                                     }).First();

                ProductVO parent_item = new ProductVO();
                parent_item.ROW_ID = ParentProduct.PROD_ITEM_ROW_ID;
                parent_item.EXT_PROD_NAME = ParentProduct.EXT_PROD_NAME;
                parent_item.VAS_FLG = ParentProduct.VAS_FLG == "Y" ? true : false;
                parent_item.PAR_ITEM_ID = 0;
                result.Add(parent_item);

                var ProductItems = from d in dbContext.XOX_T_PROD
                                   join e in dbContext.XOX_T_PROD_ITEM on d.ROW_ID equals e.PROD_ID
                                   where e.ROOT_ITEM_ID == ParentProduct.PROD_ITEM_ROW_ID && e.PAR_ITEM_ID == ParentProduct.PROD_ITEM_ROW_ID
                                   select new
                                   {
                                       PROD_ITEM_ROW_ID = e.ROW_ID,
                                       EXT_PROD_NAME = d.EXT_PROD_NAME,
                                       VAS_FLG = d.VAS_FLG,
                                       PAR_ITEM_ID = e.PAR_ITEM_ID,
                                       PRD_PRICE = d.PRD_PRICE,
                                   };

                foreach (var v in ProductItems)
                {
                    ProductVO item = new ProductVO();
                    item.ROW_ID = v.PROD_ITEM_ROW_ID;
                    item.EXT_PROD_NAME = v.EXT_PROD_NAME;
                    item.VAS_FLG = v.VAS_FLG == "Y" ? true : false;
                    item.PAR_ITEM_ID = v.PAR_ITEM_ID == null ? 0 : (long)v.PAR_ITEM_ID;
                    item.PRD_PRICE = v.PRD_PRICE;

                    result.Add(item);
                }
            }

            return result;            
        }

        public decimal GetPayment(long OrderId)
        {
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_ORDER
                             where d.ROW_ID == OrderId
                             select d.PLAN;

                if (result.Count() > 0)
                {
                    decimal Total = 0;

                    var plan = result.First();
                    if (plan.ToLower().Contains("b39"))
                    {
                        return 39;
                    }
                    else if (plan.ToLower().Contains("b59"))
                    {
                        return 59;
                    }
                    else if (plan.ToLower().Contains("b89"))
                    {
                        return 89;
                    }
                    else if (plan.ToLower().Contains("b149"))
                    {
                        return 149;
                    }
                    else if (plan.ToLower().Contains("b10"))
                    {
                        return 10;
                    }
                    else if (plan.Contains("58"))
                    {
                        Total += 53.00m;
                    }
                    else if (plan.Contains("108"))
                    {
                        Total += 106.00m;
                    }
                    else if (plan.Contains("158"))
                    {
                        Total += 159.00m;
                    }
                    else if (plan.Contains("18"))
                    {
                        Total += 19.08m;
                    }
                    else if (plan.Contains("150"))
                    {
                        Total += 159.00m;
                    }

                    return Total;
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal GetDeposit(long OrderId)
        {
            using (var dbContext = new CRMDbContext())
            {
                IAccountService AccountService = new AccountService();
                var AccountId = AccountService.GetAccountIdByOrderId(OrderId);
                var result = from d in dbContext.XOX_T_ACCNT_PAYMENT
                             where d.ACCNT_ID == AccountId
                             && d.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_DEPOSIT
                             select d;

                if (result.Count() > 0)
                {
                    return result.First().AMOUNT == null ? 0 : (decimal)result.First().AMOUNT;
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal GetDepositByAccountId(long AccountId)
        {
            using (var dbContext = new CRMDbContext())
            {
                IAccountService AccountService = new AccountService();
                var result = from d in dbContext.XOX_T_ACCNT_PAYMENT
                             where d.ACCNT_ID == AccountId
                             && d.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_DEPOSIT
                             select d;

                if (result.Count() > 0)
                {
                    return result.First().AMOUNT == null ? 0 : (decimal)result.First().AMOUNT;
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal GetSupplinePrice(long AccountId)
        {
            using (var dbContext = new CRMDbContext())
            {
                var SupplementaryLineType = ((int)AccountType.SupplementaryLine).ToString();
                var result = from d in dbContext.XOX_T_ACCNT
                             where d.PAR_ACCNT_ID == AccountId && d.ACCNT_TYPE_CD == SupplementaryLineType
                             select d;

                if (result.Count() > 0)
                {
                    return (18 * result.Count());
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal GetRequiredAdvancePayment(long OrderId)
        {
            IAccountService AccountService = new AccountService();
            decimal Amount = GetPayment(OrderId) + (GetSupplinePrice(AccountService.GetAccountIdByOrderId(OrderId)) * XOXConstants.GST);

            return Amount;
        }

        public decimal GetAdvancePayment(long OrderId)
        {
            IAccountService AccountService = new AccountService();
            using (var DBContext = new CRMDbContext())
            {
                var AccountId = AccountService.GetAccountIdByOrderId(OrderId);
                var ett = from d in DBContext.XOX_T_ACCNT_PAYMENT
                          where d.ACCNT_ID == AccountId
                          && d.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_ADVANCE_PAYMENT
                          select d;

                if (ett.Count() > 0)
                {
                    var v = ett.First();
                    return (v.AMOUNT == null ? 0 : (decimal)v.AMOUNT);
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal GetAdvancePaymentByAccountId(long AccountId)
        {
            IAccountService AccountService = new AccountService();
            using (var DBContext = new CRMDbContext())
            {
                var ett = from d in DBContext.XOX_T_ACCNT_PAYMENT
                          where d.ACCNT_ID == AccountId
                          && d.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_ADVANCE_PAYMENT
                          select d;

                if (ett.Count() > 0)
                {
                    var v = ett.First();
                    return (v.AMOUNT == null ? 0 : (decimal)v.AMOUNT);
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal GetPrime(string Plan)
        {
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_PROD
                             where d.EXT_PROD_NAME == Plan + " Charges"
                             select d.PRD_PRICE;

                if (result.Count() > 0)
                {
                    return result.First().Value;
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal GetRequiredDeposit(long OrderId)
        {
            using (var dbContext = new CRMDbContext())
            {
                var Order = (from d in dbContext.XOX_T_ORDER
                             where d.ROW_ID == OrderId
                             select d).First();

                var result = from d in dbContext.XOX_T_PROD
                             where d.EXT_PROD_NAME == Order.PLAN + " Charges"
                             select d.PRD_PRICE;

                if (result.Count() > 0)
                {
                    IAccountService AccountService = new AccountService();
                    var SuppLineDeposit = 0;
                    SuppLineDeposit = AccountService.GetAllSupplementaryLine(AccountService.GetAccountIdByOrderId(OrderId)).Count() * 18; ////
                    
                    return result.First().Value + SuppLineDeposit;
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal GetCreditLimit(string Plan)
        {
            if (Plan.ToLower().Contains("b39"))
            {
                return 60;
            }
            else if (Plan.ToLower().Contains("b59"))
            {
                return 90;
            }
            else if (Plan.ToLower().Contains("b89"))
            {
                return 135;
            }
            else if (Plan.ToLower().Contains("b149"))
            {
                return 225;
            }
            else if (Plan.ToLower().Contains("b10"))
            {
                return 15;
            }
            else
            {
                using (var DBContext = new CRMDbContext())
                {
                    var EXT_PROD_NAME = Plan + " Charges";
                    var result3 = from d in DBContext.XOX_T_PROD
                                  where d.EXT_PROD_NAME == EXT_PROD_NAME
                                  select d;

                    if (result3.Count() > 0)
                    {
                        var product = result3.First();
                        return (decimal)product.PRD_PRICE * XOXConstants.CREDIT_LIMIT;
                    }
                    else
                    {
                        throw new Exception(EXT_PROD_NAME + "- Price not found.");
                    }
                }
            }
        }

        //free
        public decimal GetinitFreeOnNetCalls(string Plan)
        {
            return 0;
        }

        public decimal GetinitFreeOffNetCalls(string Plan)
        {
            if (Plan.ToLower().Contains("b39"))
            {
                return 3000;
            }
            else if (Plan.ToLower().Contains("b59"))
            {
                return 6000;
            }
            else if (Plan.ToLower().Contains("b89"))
            {
                return 12000;
            }
            else if (Plan.ToLower().Contains("b149"))
            {
                return 18000;
            }
            else if (Plan.ToLower().Contains("b10"))
            {
                return 1200;
            }
            else if (Plan.ToLower().Contains("lightning"))
            {
                return 0;
            }
            else if (Plan.ToLower().Contains("premium"))
            {
                return 120000;
            }
            else
            {
                return Plan.Split(' ')[1] == "18" ? 6000 : (Plan.Split(' ')[1] == "58" ? 9000 : (Plan.Split(' ')[1] == "108" ? 15000 : 24000));
            }
        }

        public decimal GetinitFreeOnNetSms(string Plan)
        {
            return 0;
        }

        public decimal GetinitFreeOffNetSms(string Plan)
        {
            if (Plan.ToLower().Contains("b39"))
            {
                return 50;
            }
            else if (Plan.ToLower().Contains("b59"))
            {
                return 100;
            }
            else if (Plan.ToLower().Contains("b89"))
            {
                return 200;
            }
            else if (Plan.ToLower().Contains("b149"))
            {
                return 300;
            }
            else if (Plan.ToLower().Contains("b10"))
            {
                return 0;
            }
            else if (Plan.ToLower().Contains("lightning"))
            {
                return 0;
            }
            else if (Plan.ToLower().Contains("premium"))
            {
                return 2000;
            }
            else
            {
                return Plan.Split(' ')[1] == "18" ? 0 : (Plan.Split(' ')[1] == "58" ? 100 : (Plan.Split(' ')[1] == "108" ? 150 : 300));
            }
        }

        public decimal GetinitFreeData(string Plan)
        {
            return 0;
        }

        public string GetDataPack(string Plan)
        {
            if (Plan.ToLower() == "premium 150")
            {
                return "PREMIER150";
            }
            else
            {
                return Plan.ToUpper().Replace(" ", "").Replace("4G", "").Replace("SPEED", "");
            }
        }

        //fnf
        public decimal GetinitFnFData(string Plan)
        {
            return 0;
        }
        public decimal GetinitFnFOnNetCalls(string Plan)
        {
            return 0;
        }
        public decimal GetinitFnFOffNetCalls(string Plan)
        {
            if (Plan.ToLower().Contains("b39") || Plan.ToLower().Contains("b59") || Plan.ToLower().Contains("b89") || Plan.ToLower().Contains("b149") || Plan.ToLower().Contains("b10"))
            {
                return 0;
            }
            else if (Plan.ToLower().Contains("lightning"))
            {
                return 0;
            }
            else if (Plan.ToLower().Contains("supp"))
            {
                return 0;
            }
            else if (Plan.ToLower().Contains("premium"))
            {
                return 120000;
            }
            else
            {
                return Plan.Split(' ')[1] == "158" ? 7200 : 3600;
            }
        }
        public decimal GetinitFnFOnNetSms(string Plan)
        {
            return 0;
        }
        public decimal GetinitFnFOffNetSms(string Plan)
        {
            if (Plan.ToLower().Contains("premium"))
            {
                return 2000;
            }
            else
            {
                return 0;
            }
        }

        public bool GetPrintedBilling(string Plan)
        {
            using (var dbContext = new CRMDbContext())
            {
                var ProductItems = GetProductItemByPlan(Plan);

                return ProductItems.Where(m => m.EXT_PROD_NAME == XOXConstants.PRODUCT_PRINTED_BILLING).Count() > 0;
            }
        }

        public decimal GetDeposit(string Plan)
        {
            using (var dbContext = new CRMDbContext())
            {
                var ProductItems = GetProductItemByPlan(Plan);

                return ProductItems.Where(m => m.EXT_PROD_NAME.ToLower().Contains("deposit") && !m.EXT_PROD_NAME.ToLower().Contains("foreigner")).FirstOrDefault().PRD_PRICE ?? 0;
            }
        }

    }
}
