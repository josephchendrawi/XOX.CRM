using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib
{
    public class AssetService : IAssetService
    {
        private IProductService productService;

        public void AddAsset(OrderItemVO item, long UserId = 1)
        {
            if (UserId == 0)
            {
                UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var DBContext = new CRMDbContext())
            {
                XOX_T_ASSET asset = new XOX_T_ASSET();
                asset.ASSET_NUM = "temp";
                asset.BILL_ID = item.BILL_ID;
                asset.CREATED = DateTime.Now;
                asset.CREATED_BY = UserId;
                asset.CUST_ID = item.CUST_ID;
                asset.INSTALL_DT = item.INSTALL_DT;
                asset.INTEGRATION_ID = item.INTEGRATION_ID;
                asset.PROD_ID = item.PROD_ID;
                asset.QTY = item.QTY;
                asset.SERVICE_NUM = item.SERVICE_NUM;
                asset.STATUS_CD = item.STATUS_CD;
                asset.SVC_AC_ID = item.SVC_AC_ID;

                DBContext.XOX_T_ASSET.Add(asset);
                DBContext.SaveChanges();

                asset.ASSET_NUM = "A" + asset.ROW_ID.ToString().PadLeft(8, '0');
                asset.ROOT_ASSET_ID = asset.ROW_ID;
                DBContext.SaveChanges();
            }
        }

        // Build asset profile based on MPP response instead of order item
        public bool ConvertAssetFromPlan(AccountVO account, GetSubscriberProfileResponse mppResponse)
        {
            var additionalInformation = mppResponse.additionalInformation.ToDictionary(v => v.name, v => v.value);
            var additionalInfo = mppResponse.additionalInfo.ToDictionary(v => v.name, v => v.value);
            var parameter = mppResponse.parameter.ToDictionary(v => v.name, v => v.value);

            productService = new ProductService();
            var ProductItems = productService.GetProductItemByPlan(additionalInformation["planInfo"]);
            var packages = ProductItems.Where(d => d.PAR_ITEM_ID == 0).ToList();

            OrderItemVO orderItem = new OrderItemVO();
            orderItem.CUST_ID = account.AccountId;
            orderItem.PROD_ID = packages.First().ROW_ID;
            orderItem.QTY = 1;
            orderItem.STATUS_CD = XOXConstants.STATUS_ACTIVE_CD;
            orderItem.INSTALL_DT = DateTime.Parse(mppResponse.Customer.effective);
            orderItem.SERVICE_NUM = additionalInfo["MSISDN"];
            if (this.CheckDuplicateAsset(account.AccountId, orderItem.PROD_ID.Value) == false)
            {
                this.AddAsset(orderItem);
            }

            bool receivedBilling = parameter["PrintedBill"] == "No" ? false : true;
            if (receivedBilling) 
            {
                var selectedProducts = ProductItems.Where(d => d.EXT_PROD_NAME == XOXConstants.PRODUCT_PRINTED_BILLING).First();
                orderItem.PROD_ID = selectedProducts.ROW_ID;
                if (this.CheckDuplicateAsset(account.AccountId, orderItem.PROD_ID.Value) == false)
                {
                    this.AddAsset(orderItem);
                }
            }

            return false;
        }

        // Build asset profile based on MPP response instead of order item
        public bool ConvertAssetFromPlan(AccountVO account, OrderVO order)
        {
            productService = new ProductService();
            var ProductItems = productService.GetProductItemByPlan(order.PLAN);
            var packages = ProductItems.Where(d => d.PAR_ITEM_ID == 0).ToList();

            OrderItemVO orderItem = new OrderItemVO();
            orderItem.CUST_ID = account.AccountId;
            orderItem.PROD_ID = packages.First().ROW_ID;
            orderItem.QTY = 1;
            orderItem.STATUS_CD = XOXConstants.STATUS_ACTIVE_CD;
            orderItem.INSTALL_DT = account.RegistrationDate;
            orderItem.SERVICE_NUM = account.PersonalInfo.MSISDNNumber;
            if (this.CheckDuplicateAsset(account.AccountId, orderItem.PROD_ID.Value) == false)
            {
                this.AddAsset(orderItem);
            }

            bool receivedBilling = productService.GetPrintedBilling(order.PLAN);
            if (receivedBilling)
            {
                var selectedProducts = ProductItems.Where(d => d.EXT_PROD_NAME == XOXConstants.PRODUCT_PRINTED_BILLING).First();
                orderItem.PROD_ID = selectedProducts.ROW_ID;
                if (this.CheckDuplicateAsset(account.AccountId, orderItem.PROD_ID.Value) == false)
                {
                    this.AddAsset(orderItem);
                }
            }

            return false;
        }

        public List<AssetVO> GetAsset(long accountId)
        {
            List<AssetVO> subscriberAssets = new List<AssetVO>();
            using (var dbContext = new CRMDbContext())
            {
                var assets = from x in dbContext.XOX_T_ASSET
                             where x.CUST_ID == accountId
                             select x;
                foreach (var asset in assets)
                {
                    AssetVO assetVo = new AssetVO();
                    
                }
            }
            return subscriberAssets;
        }
        
        public void ManuallyRetrieveOrderAsset()
        {
            using (var dbContext = new CRMDbContext())
            {
                var readpath = @"D:\noasset.csv";
                var reader = new System.IO.StreamReader(System.IO.File.OpenRead(readpath));

                reader.ReadLine(); //ignore header line
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    try
                    {
                        var Result = line.Split(',');

                        var Intgrt = Result[0];
                        var Plan = Result[1];


                        var Account = (from d in dbContext.XOX_T_ACCNT
                                       where d.INTEGRATION_ID == Intgrt
                                       select d).First();
                        
                        //add asset
                        productService = new ProductService();
                        var ProductItems = productService.GetProductItemByPlan(Plan);
                        var packages = ProductItems.Where(d => d.PAR_ITEM_ID == 0).ToList();

                        OrderItemVO orderItem = new OrderItemVO();
                        orderItem.CUST_ID = Account.ROW_ID;
                        orderItem.PROD_ID = packages.First().ROW_ID;
                        orderItem.QTY = 1;
                        orderItem.STATUS_CD = XOXConstants.STATUS_ACTIVE_CD;
                        orderItem.INSTALL_DT = DateTime.Now;
                        orderItem.SERVICE_NUM = Account.MSISDN;
                        this.AddAsset(orderItem);

                        bool receivedBilling = false;
                        if (receivedBilling)
                        {
                            var selectedProducts = ProductItems.Where(d => d.EXT_PROD_NAME == XOXConstants.PRODUCT_PRINTED_BILLING).First();
                            orderItem.PROD_ID = selectedProducts.ROW_ID;
                            this.AddAsset(orderItem);
                        }

                        line += " - Success";
                    }
                    catch (Exception e)
                    {
                        line += " - Error : " + e.Message;
                    }

                    var writepath = @"D:\Scheduler\result.txt";
                    using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter(writepath, true))
                    {
                        file.WriteLine(line);
                    }
                }
            }
        }

        public List<AssetVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", AssetVO qFilter = null)
        {
            List<AssetVO> List = new List<AssetVO>();
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_ASSET
                             join e in dbContext.XOX_T_PROD_ITEM on d.PROD_ID equals e.ROW_ID
                             join f in
                                 (
                                   from g in dbContext.XOX_T_ASSET
                                   join h in dbContext.XOX_T_PROD_ITEM on g.PROD_ID equals h.ROW_ID
                                   where g.STATUS_CD == XOXConstants.STATUS_ACTIVE_CD && h.PROD_ID == 30 //30 = PrintedBilling PROD_ID
                                   select g
                                  ) on d.CUST_ID equals f.CUST_ID into ff
                             from subf in ff.DefaultIfEmpty()
                             join g in dbContext.XOX_T_PROD on e.PROD_ID equals g.ROW_ID
                             join h in dbContext.XOX_T_ACCNT on d.CUST_ID equals h.ROW_ID
                             where e.PAR_ITEM_ID == null
                             //&& d.STATUS_CD == XOXConstants.STATUS_ACTIVE_CD
                             select new
                             {
                                 ASSET = d,
                                 ACCNT = h,
                                 PROD = g,
                                 PrintedBilling = (subf == null ? false : true)
                             };

                //filtering
                if (qFilter != null)
                {
                    if (qFilter.MSISDN != null && qFilter.MSISDN != "")
                    {
                        result = result.Where(m => m.ACCNT.MSISDN.Contains(qFilter.MSISDN));
                    }
                    if (qFilter.Plan != null && qFilter.Plan != "")
                    {
                        result = result.Where(m => m.PROD.EXT_PROD_NAME.ToLower().Contains(qFilter.Plan.ToLower()));
                    }
                }

                TotalCount = result.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "Status")
                        result = result.OrderBy(m => m.ASSET.STATUS_CD);
                    else if (orderBy == "EffectiveDate")
                        result = result.OrderBy(m => m.ASSET.INSTALL_DT);
                    else if (orderBy == "Plan")
                        result = result.OrderBy(m => m.PROD.EXT_PROD_NAME);
                    else if (orderBy == "PrintedBilling")
                        result = result.OrderBy(m => m.PrintedBilling);
                    else if (orderBy == "SubscriberName")
                        result = result.OrderBy(m => m.ACCNT.NAME);
                    else if (orderBy == "MSISDN")
                        result = result.OrderBy(m => m.ACCNT.MSISDN);
                    else
                        result = result.OrderBy(m => m.ASSET.ROW_ID);
                }
                else
                {
                    if (orderBy == "Status")
                        result = result.OrderByDescending(m => m.ASSET.STATUS_CD);
                    else if (orderBy == "EffectiveDate")
                        result = result.OrderByDescending(m => m.ASSET.INSTALL_DT);
                    else if (orderBy == "Plan")
                        result = result.OrderByDescending(m => m.PROD.EXT_PROD_NAME);
                    else if (orderBy == "PrintedBilling")
                        result = result.OrderByDescending(m => m.PrintedBilling);
                    else if (orderBy == "SubscriberName")
                        result = result.OrderByDescending(m => m.ACCNT.NAME);
                    else if (orderBy == "MSISDN")
                        result = result.OrderByDescending(m => m.ACCNT.MSISDN);
                    else
                        result = result.OrderByDescending(m => m.ASSET.ROW_ID);
                }

                if (length >= 0)
                    result = result.Skip(startIdx).Take(length);

                foreach (var v in result)
                {
                    List.Add(new AssetVO()
                    {
                        AccountId = v.ACCNT.ROW_ID,
                        AssetId = v.ASSET.ROW_ID,
                        EffectiveDate = v.ASSET.INSTALL_DT,
                        MSISDN = v.ACCNT.MSISDN,
                        Plan = v.PROD.EXT_PROD_NAME,
                        PrintedBilling = v.PrintedBilling,
                        Status = v.ASSET.STATUS_CD,
                        SubscriberName = v.ACCNT.NAME
                    });
                }
            }

            return List;
        }

        private bool CheckDuplicateAsset(long custId, long prodId)
        {
            bool result = false;
            using (var dbContext = new CRMDbContext())
            {
                var assets = from x in dbContext.XOX_T_ASSET
                             where x.CUST_ID == custId && x.PROD_ID == prodId && x.STATUS_CD == XOXConstants.STATUS_ACTIVE_CD
                             select x;
                result = assets.Count() > 0 ? true : false;
            }
            return result;
        }
    }
}
