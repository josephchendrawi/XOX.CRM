using CRM;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.API.ServiceModel;
using XOX.CRM.API.ServiceModel.Types;
using XOX.CRM.Lib;

namespace XOX.CRM.API.ServiceInterface
{
    public class OrderServices : Service
    {
        private readonly IOrderService OrderService;
        private readonly IAccountService AccountService;
        private readonly IProductService ProductService;

        public OrderServices(IOrderService OrderService, IAccountService AccountService, IProductService ProductService)
        {
            this.OrderService = OrderService;
            this.AccountService = AccountService;
            this.ProductService = ProductService;
        }

        public object Post(OrderCreate request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            var OrderVO = Mapper.Order.Map(request, UserId);
            
            var ProductItems = ProductService.GetProductItemByPlan(request.Plan);

            var selectedProds = ProductItems.Where(d => d.VAS_FLG == false).ToList();
            if (request.FlgReceivedItemised == true)
            {
                selectedProds.Add(ProductItems.Where(d => d.EXT_PROD_NAME == "Printed Billing").First());
            }
            if (request.FlgForeigner == true)
            {
                selectedProds.Add(ProductItems.Where(d => d.EXT_PROD_NAME == "Foreigner Deposit").First());
            }

            List<long> _selectedProducts = new List<long>();
            foreach (var v in selectedProds)
            {
                _selectedProducts.Add(v.ROW_ID);
            }

            var result = OrderService.CreateNewOrder(OrderVO, _selectedProducts, request.MSISDN, request.SubmitBy);

            AccountService.EditPrintedBillingFlg(request.AccountId, request.FlgReceivedItemised);

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new LongResponse { Result = result, Key = APIKey };
        }
        
        public object Post(OrderAddFiles request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            List<string> filespath = new List<string>();
            foreach (var v in request.Files)
            {
                if (String.IsNullOrEmpty(v.Base64) == false)
                {
                    var filepath = Helper.FileUpload(v.Base64, v.FileName);
                    if (filepath != null)
                        filespath.Add(filepath);
                }
                else if (String.IsNullOrEmpty(v.FileURL) == false)
                {
                    var filepath = Helper.FileUploadURL(v.FileURL, v.FileName);
                    if (filepath != null)
                        filespath.Add(filepath);
                }
            }

            var result = OrderService.AddDocuments(filespath, request.OrderId, UserId);

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new BoolResponse { Result = result, Key = APIKey };
        }

        public object Post(OrderReAddFiles request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            List<string> filespath = new List<string>();
            foreach (var v in request.Files)
            {
                if (String.IsNullOrEmpty(v.Base64) == false)
                {
                    var filepath = Helper.FileUpload(v.Base64, v.FileName);
                    if (filepath != null)
                        filespath.Add(filepath);
                }
                else if (String.IsNullOrEmpty(v.FileURL) == false)
                {
                    var filepath = Helper.FileUploadURL(v.FileURL + ".jpg", v.FileName);
                    if (filepath != null)
                        filespath.Add(filepath);
                }
            }

            var AccountId = AccountService.GetAccountIdByIntegrationId(request.IntegrationId);
            var OrderId = OrderService.GetOrderIdByAccountId(AccountId);

            var result = OrderService.CheckAndAddDocument(filespath, OrderId, UserId);

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new BoolResponse { Result = result, Key = APIKey };
        }

        public object Post(OrderSupplementaryAdd request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            long OrderId = 0;
            if (request.SuppAccountId != -1)
            {
                var AccountId = AccountService.GetParentAccountId(request.SuppAccountId);
                var Account = AccountService.Get(AccountId);

                if (Account.PersonalInfo.CustomerStatus != (int)AccountStatus.Active)
                {
                    OrderId = OrderService.UpdateOrderSupplementary(request.MSISDN, request.SuppAccountId);
                    if (OrderId == 0)
                    {
                        throw new Exception("SuppAccountId not found.");
                    }
                }
                //if Account Status is already Active then create new "Supplementary Registration" order
                else
                {
                    //create new "Supplementary Registration" order 
                    var OrderVOPrincipal = OrderService.GetOrderVO(OrderService.GetOrderIdByAccountId(AccountId));

                    OrderVO OrderVO = new OrderVO();
                    OrderVO.CustomerAccountId = request.SuppAccountId;
                    OrderVO.ORDER_TYPE = "Supplementary Registration";
                    OrderVO.CREATED_BY = UserId;
                    OrderVO.PLAN = "Supp 18";
                    OrderVO.REMARKS = request.Donor;
                    OrderVO.ORDER_SUBMIT_DT = DateTime.Now; ///
                    OrderVO.ORDER_STATUS = "New";
                    OrderVO.CUST_REP_ID = OrderVOPrincipal.ROW_ID.ToString();
                    OrderVO.CATEGORY = string.IsNullOrWhiteSpace(request.Donor) == true ? "NEW" : "MNP";
                    OrderVO.ORDER_SOURCE = "CRP";

                    var selectedProds = ProductService.GetProductItemByPlan(OrderVO.PLAN);
                    List<long> _selectedProducts = new List<long>();
                    foreach (var v in selectedProds)
                    {
                        _selectedProducts.Add(v.ROW_ID);
                    }

                    OrderId = OrderService.CreateNewOrder(OrderVO, _selectedProducts, request.MSISDN, "CRP"); ///
                }
            }
            else if (request.SuppAccountId == -1 && request.AccountId != 0)
            {
                //check if order == supplementary registration
                long SuppAccountId = AccountService.CheckIntegrationId(request.IntegrationId, "3");

                if (OrderService.GetOrderIdByAccountId(SuppAccountId, "Supplementary Registration") == 0)
                {
                    var ParentOrderId = OrderService.GetOrderIdByAccountId(request.AccountId);
                    if (ParentOrderId == 0)
                    {
                        throw new Exception("ParentOrderId not found.");
                    }

                    var ParentOrder = OrderService.Get(ParentOrderId);
                    
                    //if parent account status != active, then just edit the parent order info by parent order id
                    if (ParentOrder.OrderStatus != "Active")
                    {
                        OrderId = ParentOrderId;
                    }
                    else
                    {
                        var SuppAccount = AccountService.Get(SuppAccountId);
                        //if parent account status == active and supp account status != active, then create new supp line order
                        if (ParentOrder.OrderStatus == "Active" && SuppAccount.PersonalInfo.CustomerStatus != (int)AccountStatus.Active)
                        {
                            var ParentAccountId = AccountService.GetParentAccountId(SuppAccountId);
                            //create new "Supplementary Registration" order 
                            var OrderVOPrincipal = OrderService.GetOrderVO(OrderService.GetOrderIdByAccountId(ParentAccountId));

                            OrderVO OrderVO = new OrderVO();
                            OrderVO.CustomerAccountId = SuppAccountId;
                            OrderVO.ORDER_TYPE = "Supplementary Registration";
                            OrderVO.CREATED_BY = UserId;
                            OrderVO.PLAN = "Supp 18";
                            OrderVO.REMARKS = request.Donor;
                            OrderVO.ORDER_SUBMIT_DT = DateTime.Now; ///
                            OrderVO.ORDER_STATUS = "New";
                            OrderVO.CUST_REP_ID = OrderVOPrincipal.ROW_ID.ToString();
                            OrderVO.CATEGORY = string.IsNullOrWhiteSpace(request.Donor) == true ? "NEW" : "MNP";
                            OrderVO.ORDER_SOURCE = "CRP";

                            var selectedProds = ProductService.GetProductItemByPlan(OrderVO.PLAN);
                            List<long> _selectedProducts = new List<long>();
                            foreach (var v in selectedProds)
                            {
                                _selectedProducts.Add(v.ROW_ID);
                            }

                            OrderId = OrderService.CreateNewOrder(OrderVO, _selectedProducts, request.MSISDN, "CRP"); ///
                        }
                        //if parent account status == active and supp account status != active, then nothing happens.
                        else if (ParentOrder.OrderStatus == "Active" && SuppAccount.PersonalInfo.CustomerStatus == (int)AccountStatus.Active)
                        {
                            OrderId = 0; //nothing happens
                        }
                    }
                }
                else
                {
                    long SuppOrderId = OrderService.GetOrderIdByAccountId(SuppAccountId, "Supplementary Registration");
                    OrderId = SuppOrderId;

                    var SuppOrderVO = OrderService.GetOrderVO(SuppOrderId);
                    //if supp order status is not Active, then create new supp order
                    if (SuppOrderVO.ORDER_STATUS != "Active")
                    {
                        var OldSuppOrderVO = OrderService.GetOrderVO(SuppOrderId);

                        OrderVO NewSuppOrderVO = new OrderVO();
                        NewSuppOrderVO.CustomerAccountId = SuppAccountId;
                        NewSuppOrderVO.ORDER_TYPE = "Supplementary Registration";
                        NewSuppOrderVO.CREATED_BY = UserId;
                        NewSuppOrderVO.PLAN = "Supp 18";
                        NewSuppOrderVO.REMARKS = request.Donor;
                        NewSuppOrderVO.ORDER_SUBMIT_DT = DateTime.Now; ///
                        NewSuppOrderVO.ORDER_STATUS = "Resubmitted";
                        NewSuppOrderVO.CUST_REP_ID = OldSuppOrderVO.ROW_ID.ToString();
                        NewSuppOrderVO.CATEGORY = string.IsNullOrWhiteSpace(request.Donor) == true ? "NEW" : "MNP";
                        NewSuppOrderVO.ORDER_SOURCE = "CRP";

                        var selectedProds = ProductService.GetProductItemByPlan(NewSuppOrderVO.PLAN);
                        List<long> _selectedProducts = new List<long>();
                        foreach (var v in selectedProds)
                        {
                            _selectedProducts.Add(v.ROW_ID);
                        }

                        OrderId = OrderService.CreateNewOrder(NewSuppOrderVO, _selectedProducts, request.MSISDN, "CRP", true); ///
                    }
                    else
                    {
                        OrderId = 0; //nothing happens
                    }
                }
            }

            if (OrderId != 0)
            {
                if (request.Files != null && request.Files.Count() > 0)
                {
                    List<string> filespath = new List<string>();
                    foreach (var v in request.Files)
                    {
                        if (String.IsNullOrEmpty(v.Base64) == false)
                        {
                            var filepath = Helper.FileUpload(v.Base64, v.FileName);
                            if (filepath != null)
                                filespath.Add(filepath);
                        }
                        else if (String.IsNullOrEmpty(v.FileURL) == false)
                        {
                            var filepath = Helper.FileUploadURL(v.FileURL, v.FileName);
                            if (filepath != null)
                                filespath.Add(filepath);
                        }
                    }

                    var result = OrderService.AddDocuments(filespath, OrderId, UserId);
                }
            }

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new BoolResponse { Result = true, Key = APIKey };
        }

        public object Post(OrderResubmit request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            //account
            var AccountVO = Mapper.Account.Map(request.AccountAdd.PersonalInfo, request.AccountAdd.BankingInfo, request.AccountAdd.AddressInfo, request.AccountAdd.BillingAddressInfo, request.AccountAdd.SIMSerialNumber);
            AccountVO.AccountId = AccountService.GetAccountIdByIntegrationId(request.AccountAdd.IntegrationId);
            if (AccountVO.AccountId == 0)
            {
                AccountVO.AccountId = AccountService.GetAccountIdByOrderId(request.OrderId);
            }
            AccountService.Edit(AccountVO, UserId, false);

            //order
            /*
            OrderActivityVO Activity = new OrderActivityVO();
            Activity.ORDER_ID = request.OrderId;
            Activity.ASSIGNEE = "abc@abc.com";
            Activity.ACT_REMARKS = "";
            Activity.REJECTED_REASON = "";
            Activity.CREATED_BY = 1;
            Activity.LAST_UPD_BY = 1;
            var result = OrderService.UpdateStatus(Activity, "Resubmitted");

            OrderService.Edit(request.OrderId, new OrderDetailVO() { Category = request.OrderCreate.Category, Remarks = request.OrderCreate.Remarks, SubscriptionPlan = request.OrderCreate.Plan }, UserId);
            */
            request.OrderCreate.AccountId = AccountVO.AccountId;
            var OrderVO = Mapper.Order.Map(request.OrderCreate, UserId);

            var ProductItems = ProductService.GetProductItemByPlan(request.OrderCreate.Plan);

            var selectedProds = ProductItems.Where(d => d.VAS_FLG == false).ToList();
            if (request.OrderCreate.FlgReceivedItemised == true)
            {
                selectedProds.Add(ProductItems.Where(d => d.EXT_PROD_NAME == "Printed Billing").First());
            }
            if (request.OrderCreate.FlgForeigner == true)
            {
                selectedProds.Add(ProductItems.Where(d => d.EXT_PROD_NAME == "Foreigner Deposit").First());
            }

            List<long> _selectedProducts = new List<long>();
            foreach (var v in selectedProds)
            {
                _selectedProducts.Add(v.ROW_ID);
            }
            
            long OrderId = OrderService.CreateNewOrder(OrderVO, _selectedProducts, AccountVO.PersonalInfo.MSISDNNumber, request.SubmitBy, true); //ResubmittedOrderId

            //order_att
            List<string> filespath = new List<string>();
            foreach (var v in request.OrderAddFiles.Files)
            {
                if (String.IsNullOrEmpty(v.Base64) == false)
                {
                    var filepath = Helper.FileUpload(v.Base64, v.FileName);
                    if (filepath != null)
                        filespath.Add(filepath);
                }
                else if (String.IsNullOrEmpty(v.FileURL) == false)
                {
                    var filepath = Helper.FileUploadURL(v.FileURL, v.FileName);
                    if (filepath != null)
                        filespath.Add(filepath);
                }
            }

            ////
            /*
            var result2 = OrderService.ResetDocuments(request.OrderId);
            if (result2 == true)
            {
                var result3 = OrderService.AddDocuments(filespath, request.OrderId, UserId);
            }
            */
            OrderService.AddDocuments(filespath, OrderId, UserId);
            
            AccountService.EditPrintedBillingFlg(AccountService.GetAccountIdByOrderId(OrderId), request.OrderCreate.FlgReceivedItemised);

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new BoolResponse { Result = true, Key = APIKey };
        }

        public object Post(ActivateMNPOrder request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            var result = "";
            bool success = false;
            try
            {
                result = OrderService.ActivateMNPOrderRequest(request.portReqFormId, request.portId);
                if (result == "Successfully Activated.")
                {
                    success = true;
                }
            }
            catch (Exception e)
            {
                result = e.Message + e.Source;
            }

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new ResultResponse { Result = result, Success = success, Key = APIKey };
        }

        public object Post(ActivateMNPOrderStatus request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            var result = "";
            bool success = false;
            try
            {
                result = OrderService.ActivateMNPOrderStatusRequest(request.portId, request.statusMsg, request.rejectCode);
                if (result == "Successfully done.")
                {
                    success = true;
                }
            }
            catch(Exception e)
            {
                result = e.Message + e.Source;
            }

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new ResultResponse { Result = result, Success = success, Key = APIKey };
        }

    }
}
