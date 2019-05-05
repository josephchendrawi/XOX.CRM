using CRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.Common.Enum;

namespace CRM.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService OrderService;
        private readonly IOrderActivityService OrderActivityService;
        private readonly ICommonService CommonService;
        private readonly IAccountService AccountService;
        private readonly IProductService ProductService;

        public OrderController()
        {
            this.OrderService =  new OrderService();
            this.OrderActivityService = new  OrderActivityService();
            this.CommonService = new  CommonService();
            this.AccountService =  new AccountService();
            this.ProductService = new ProductService();
        }

        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult List()
        {
            List<string> orderstatuslist = new List<string>();
            var lookup = new LookupVO() { LookupKey = "Order Status" };
            CommonService.GetLookupValues(lookup);
            foreach (var v in lookup.KeyValues)
            {
                orderstatuslist.Add(v.Key);
            }

            ViewBag.OrderStatusList = lookup.KeyValues;

            return View();
        }

        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult ListOrder(DataTableParam param)
        {
            try
            {
                var model = new OrderListVMData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string orderingFunction = sortColumnIndex == 1 ? "OrderNum" :
                                            sortColumnIndex == 2 ? "MSISDN" :
                                            sortColumnIndex == 3 ? "FullName" :
                                            sortColumnIndex == 4 ? "SubmissionDate" :
                                            sortColumnIndex == 5 ? "Category" :
                                            sortColumnIndex == 6 ? "Status" :
                                            "OrderId";

                var sortDirection = Request["sSortDir_0"]; // asc or desc


                //filtering : need to pass skip/take and search string to query 
                var filterBy = "";
                var filterQuery = "";

                if (string.IsNullOrEmpty(param.sSearch) == false)
                {
                    var type = param.sSearch[0];
                    var query = param.sSearch.Substring(1);

                    filterBy = type == '1' ? "OrderNum" :
                                type == '2' ? "MSISDN" :
                                type == '3' ? "FullName" :
                                type == '4' ? "SubmissionDate" :
                                type == '5' ? "Category" :
                                type == '6' ? "Status" :
                                "";
                    filterQuery = query;
                }

                var List = OrderService.GetAll(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, filterBy, filterQuery);

                List<OrderListVM> OrderList = new List<OrderListVM>();

                long count = param.iDisplayStart + 1;
                foreach (var v in List)
                {
                    OrderListVM VM = new OrderListVM();
                    VM.OrderId = v.OrderId;
                    VM.OrderNum = v.OrderNum;
                    VM.MSISDN = v.MSISDN == null ? "" : v.MSISDN;
                    VM.FullName = v.Account.PersonalInfo.FullName;
                    VM.SubmissionDate = v.SubmissionDate == null ? "-" : v.SubmissionDate.Value.ToString("dd MMM yyyy");
                    VM.Category = v.Category == null ? "" : v.Category;
                    VM.Status = v.OrderStatus;
                    VM.Idx = count++;

                    OrderList.Add(VM);
                }

                model.aaData = OrderList;
                model.iTotalDisplayRecords = TotalCount;
                model.iTotalRecords = TotalCount;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult View(int ID, string ReturnURL)
        {
            OrderModel model = new OrderModel();

            var result = OrderService.Get(ID);

            model = OrderMapper.Map(result);

            model.ChangePlanEffectiveDate = result.ChangePlanEffectiveDate;

            model.Documents = new List<Document>();
            foreach (var v in OrderService.GetAllAttachments(ID))
            {
                var type = v.Path.Substring(v.Path.LastIndexOf(".") + 1);
                if (type.ToLower() == "jpg" || type.ToLower() == "jpeg" || type.ToLower() == "png" || type.ToLower() == "gif")
                    type = "IMAGE";

                model.Documents.Add(new Document()
                {
                    DocumentId = v.DocumentId,
                    Path = v.Path,
                    FullPath = ConfigurationManager.AppSettings["UploadPath"] + v.Path,
                    Type = type
                });
            }

            return View(model);
        }

        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult ListActivity(DataTableParam param, long ID = 0)
        {
            try
            {
                var model = new ActivityData();

                var List = OrderActivityService.GetActivities(ID);

                List<Activity> ActivityList = new List<Activity>();

                foreach (var v in List)
                {
                    Activity VM = new Activity();
                    VM.OrderId = v.ORDER_ID;
                    VM.Description = v.ACT_DESC;
                    VM.DueDate = ((DateTime)v.DUE_DATE).ToString("yyyy MM dd");
                    VM.Assignee = v.ASSIGNEE;
                    VM.Remarks = v.ACT_REMARKS;
                    VM.CreatedDateTime = ((DateTime)v.CREATED).ToString("yyyyMMddHHmmss");
                    VM.CreatedDateTimeText = ((DateTime)v.CREATED).ToString("dd MMM yyyy HH:mm:ss");
                    VM.CreatedBy = v.CREATED_BY == null ? "" : UserService.GetName((long)v.CREATED_BY);
                    VM.Status = v.ORDER_STATUS;

                    ActivityList.Add(VM);
                }


                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<Activity, string> orderingFunction = (c => sortColumnIndex == 0 ? c.CreatedDateTime :
                                                                    sortColumnIndex == 1 ? c.CreatedBy :
                                                                    sortColumnIndex == 2 ? c.CreatedDateTime :
                                                                    sortColumnIndex == 3 ? c.Assignee :
                                                                    sortColumnIndex == 4 ? c.Remarks :
                                                                    sortColumnIndex == 5 ? c.Status :
                                                                    c.CreatedDateTime);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                if (sortColumnIndex == 0)
                {
                    sortDirection = "desc";
                }

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        ActivityList = ActivityList.OrderBy(orderingFunction).ToList();
                    }
                    else
                    {
                        ActivityList = ActivityList.OrderByDescending(orderingFunction).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = ActivityList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.Description.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = ActivityList.Count;
                model.iTotalRecords = ActivityList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("ORDER", "EDIT")]
        [HttpPost]
        public ActionResult DoActivity(FormCollection form)
        {
            var OrderId = long.Parse(form["OrderId"]);
            var OrderStatus = form["OrderStatus"];

            if (String.IsNullOrEmpty(form["Assignee"]) == false)
            {
                try
                {
                    OrderActivityVO Activity = new OrderActivityVO();
                    Activity.ORDER_ID = OrderId;
                    Activity.ASSIGNEE = form["Assignee"];
                    Activity.ACT_REMARKS = form["Remarks"];
                    Activity.REJECTED_REASON = form["Reason"];
                    var result = OrderService.UpdateStatus(Activity, OrderStatus);

                    if (result != false)
                    {
                        TempData["Message"] = "Successfully done.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }
            else
            {
                TempData["Message"] = "An error occurred, Assignee is empty. Please try to re-login and try again.";
            }

            //if (form["ReturnURL"] != null && form["ReturnURL"] != "")
            //{
            //    return Redirect(form["ReturnURL"]);
            //}
            //else
            //{
                return RedirectToAction("View", "Order", new { ID = OrderId, ReturnURL = form["ReturnURL"] });
            //}
        }

        [UserAuthorize("ORDER", "ADD")]
        public ActionResult Add()
        {
            OrderVO model = new OrderVO();

            return View(model);
        }

        [UserAuthorize("ORDER", "ADD")]
        [HttpPost]
        public ActionResult Add(OrderVO Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<long> _selectedProducts = new List<long>();
                    for (int i = 1; i <= 12; i++)
                    {
                        _selectedProducts.Add(i);
                    }
                    _selectedProducts.Add(48);
                    var result = OrderService.CreateNewOrder(Model, _selectedProducts, "", "");

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("Add", "Order");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(Model);
        }
        

        [UserAuthorize("ORDER", "EDIT")]
        public ActionResult Process(long ID, string ReturnURL)
        {
            try
            {
                if (Session["UserEmail"] != null && String.IsNullOrEmpty(Session["UserEmail"].ToString()) == false)
                {
                    OrderActivityVO Activity = new OrderActivityVO();
                    Activity.ORDER_ID = ID;
                    if (Session["UserEmail"] != null)
                    {
                        Activity.ASSIGNEE = Session["UserEmail"].ToString();
                    }
                    Activity.ACT_REMARKS = "";
                    Activity.REJECTED_REASON = "";
                    var result = OrderService.UpdateStatus(Activity, "Process");

                    if (result != false)
                    {
                        TempData["Message"] = "Successfully done.";
                    }
                }
                else
                {
                    TempData["Message"] = "An error occurred, Session is not found. Please try to re-login and try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            if (ReturnURL != null && ReturnURL != "")
            {
                return Redirect(ReturnURL);
            }
            else
            {
                return RedirectToAction("View", "Order", new { ID = ID });
            }
        }

        [UserAuthorize("ASSIGN", "VIEW")]
        public ActionResult Assign()
        {
            List<AssigneeOrder> AssigneeOrders = new List<AssigneeOrder>();

            foreach (var v in Helper.GetAssigneeList())
            {
                List<OrderStatusCount> OrderStatusCount = new List<Models.OrderStatusCount>();

                foreach (var y in Helper.GetLookupList("Order Status"))
                {
                    OrderStatusCount.Add(new OrderStatusCount()
                    {
                        OrderStatus = y.Value,
                        Count = OrderService.GetAllCount("Status", y.Value, v.Text)
                    });
                }

                AssigneeOrders.Add(new AssigneeOrder()
                {
                    AssigneeId = long.Parse(v.Value),
                    Assignee = v.Text,
                    OrderStatusCount = OrderStatusCount
                });
            }

            ViewBag.AssigneeOrders = AssigneeOrders;            

            return View();
        }
        
        [UserAuthorize("ASSIGN", "EDIT")]
        [HttpPost]
        public ActionResult Assign(FormCollection form)
        {
            int Length = int.Parse(form["OrderCount"]);
            string FromAssignee = Helper.GetAssigneeList().First().Text; // form["FromAssignee"];
            string ToAssignee = form["ToAssignee"];
            string OrderStatus = form["OrderStatus"];

            var Method = form["Method"];
            if (Method == "1")
            {
                var List = OrderService.GetAllOrderId(FromAssignee, Length, OrderStatus);

                foreach (var v in List)
                {
                    OrderActivityVO Activity = new OrderActivityVO();
                    Activity.ORDER_ID = v.OrderId;
                    Activity.ASSIGNEE = ToAssignee;
                    Activity.ACT_REMARKS = "";
                    Activity.REJECTED_REASON = "";
                    var result = OrderService.UpdateStatus(Activity, OrderStatus);
                }

                TempData["Message"] = "Successfully done. " + List.Count + " Order(s) has been assigned to " + ToAssignee + ".";
            }
            else
            {
                foreach (var y in Helper.GetAssigneeList().Skip(1))
                {
                    var List = OrderService.GetAllOrderId(y.Text);
                    foreach (var v in List)
                    {
                        OrderActivityVO Activity = new OrderActivityVO();
                        Activity.ORDER_ID = v.OrderId;
                        Activity.ASSIGNEE = Helper.GetAssigneeList().First().Text;
                        Activity.ACT_REMARKS = "";
                        Activity.REJECTED_REASON = "";
                        var result = OrderService.UpdateStatus(Activity, v.OrderStatus);
                    }
                }

                TempData["Message"] = "Successfully done.";
            }

            return RedirectToAction("Assign", "Order");
        }



        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult Search()
        {
            OrderListVM model = new OrderListVM();
            return View();
        }

        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult Result(OrderListVM model)
        {
            model.OrderType = "Registration";
            return View(model);
        }

        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult ListSearchOrder(DataTableParam param, string MSISDN = "", string FullName = "", string From = "", string To = "", string Category = "", string Status = "", string OrderNum = "", string OrderType = "", long AccountId = 0)
        {
            try
            {
                var model = new OrderListVMData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string orderingFunction = sortColumnIndex == 1 ? "OrderNum" :
                                            sortColumnIndex == 2 ? "MSISDN" :
                                            sortColumnIndex == 3 ? "FullName" :
                                            sortColumnIndex == 4 ? "SubmissionDate" :
                                            sortColumnIndex == 5 ? "Category" :
                                            sortColumnIndex == 6 ? "Status" :
                                            "OrderId";

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                var List = OrderService.GetAllBySearch(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, MSISDN, FullName, From, To, Category, Status, OrderNum, OrderType, AccountId);

                List<OrderListVM> OrderList = new List<OrderListVM>();

                long count = param.iDisplayStart + 1;
                foreach (var v in List)
                {
                    OrderListVM VM = new OrderListVM();
                    VM.OrderId = v.OrderId;
                    VM.OrderNum = v.OrderNum;
                    VM.MSISDN = v.MSISDN == null ? "" : v.MSISDN;
                    VM.FullName = v.Account.PersonalInfo.FullName;
                    VM.SubmissionDate = v.SubmissionDate == null ? "-" : v.SubmissionDate.Value.ToString("dd MMM yyyy");
                    VM.Category = v.Category == null ? "" : v.Category;
                    VM.Status = v.OrderStatus;
                    VM.CRPId = v.CRPId;
                    VM.Idx = count++;

                    OrderList.Add(VM);
                }

                model.aaData = OrderList;
                model.iTotalDisplayRecords = TotalCount;
                model.iTotalRecords = TotalCount;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        [UserAuthorize("ORDER", "EDIT")]
        [HttpPost]
        public ActionResult UploadDocument(HttpPostedFileBase[] files, FormCollection form)
        {
            var OrderId = long.Parse(form["OrderId"]);

            try
            {
                //upload files
                List<string> filespath = new List<string>();
                if (files != null && files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        if (file != null)
                            filespath.Add(Helper.UploadingFile(file));
                    }
                }
                if (filespath.Count() > 0)
                    OrderService.AddFiles(filespath, OrderId);

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            if (form["ReturnURL"] != null && form["ReturnURL"] != "")
            {
                return RedirectToAction("View", "Order", new { ID = OrderId, ReturnURL = form["ReturnURL"] });
            }
            else
            {
                return RedirectToAction("View", "Order", new { ID = OrderId });
            }
        }

        [UserAuthorize("ORDER", "EDIT")]
        [HttpPost]
        public ActionResult DoActivation(FormCollection form)
        {
            var OrderId = long.Parse(form["OrderId"]);

            if (String.IsNullOrEmpty(form["Assignee"]) == false)
            {
                try
                {
                    OrderActivityVO Activity = new OrderActivityVO();
                    Activity.ORDER_ID = OrderId;
                    Activity.ASSIGNEE = form["Assignee"];
                    Activity.ACT_REMARKS = "";
                    Activity.REJECTED_REASON = "";
                    var result = OrderService.ActivateOrderRequest(Activity);

                    if (result == true)
                    {
                        TempData["Message"] = "Successfully done.";   
                    }
                    else
                    {
                        TempData["Message"] = "Failed to activate.";   
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }
            else
            {
                TempData["Message"] = "An error occurred, Assignee is empty. Please try to re-login and try again.";
            }

            return RedirectToAction("View", "Order", new { ID = OrderId, ReturnURL = form["ReturnURL"] });
        }


        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult ListRejectedOrder(DataTableParam param, long ResubmittedOrder)
        {
            try
            {
                var model = new OrderListVMData();

                var List = OrderService.GetRejectedOrder(ResubmittedOrder);

                List<OrderListVM> OrderListVM = new List<OrderListVM>();

                foreach (var v in List)
                {
                    OrderListVM VM = new OrderListVM();
                    VM.OrderId = v.ROW_ID;
                    VM.OrderNum = v.ORDER_NUM;

                    OrderListVM.Add(VM);
                }
                
                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        OrderListVM = OrderListVM.OrderBy(m => m.OrderNum).ToList();
                    }
                    else
                    {
                        OrderListVM = OrderListVM.OrderByDescending(m => m.OrderNum).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = OrderListVM.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.OrderNum.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = OrderListVM.Count;
                model.iTotalRecords = OrderListVM.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //[UserAuthorize("ORDER", "EDIT")]
        //[HttpPost]
        //public ActionResult DoPendingActivation(FormCollection form)
        //{
        //    var OrderId = long.Parse(form["OrderId"]);
        //    var Amount = decimal.Parse(form["amount"]);
        //    var Reference = form["reference"].ToString();

        //    if (String.IsNullOrEmpty(form["Assignee"]) == false)
        //    {
        //        try
        //        {
        //            OrderActivityVO Activity = new OrderActivityVO();
        //            Activity.ORDER_ID = OrderId;
        //            Activity.ASSIGNEE = form["Assignee"];
        //            Activity.ACT_REMARKS = "";
        //            Activity.REJECTED_REASON = "";
        //            var result = OrderService.ActivateOrderRequest(Activity);

        //            if (result == true)
        //            {
        //                TempData["Message"] = "Successfully done.";
        //            }
        //            else
        //            {
        //                TempData["Message"] = "Failed to activate.";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["Message"] = ex.Message;
        //        }
        //    }
        //    else
        //    {
        //        TempData["Message"] = "An error occurred, Assignee is empty. Please try to re-login and try again.";
        //    }

        //    return RedirectToAction("View", "Order", new { ID = OrderId, ReturnURL = form["ReturnURL"] });
        //}


        [UserAuthorize("ORDER", "EDIT")]
        [HttpPost]
        public ActionResult SavePaymentRecord(FormCollection form)
        {
            var OrderId = long.Parse(form["OrderId"]);
            var Deposit = decimal.Parse(form["deposit"]);
            var AdvancePayment = decimal.Parse(form["advancepayment"]);
            var ForeignDeposit = decimal.Parse(string.IsNullOrWhiteSpace(form["foreigndeposit"]) == true ? "0" : form["foreigndeposit"]);
            var Reference = form["reference"];

            try
            {
                PaymentRecordVO vo = new PaymentRecordVO();
                vo.AdvancePayment = AdvancePayment;
                vo.Deposit = Deposit;
                vo.ForeignDeposit = ForeignDeposit;
                vo.Reference = Reference;
                vo.OrderId = OrderId;
                vo.AccountId = AccountService.GetAccountIdByOrderId(OrderId);
                var result = AccountService.SavePaymentRecord(vo);

                foreach (var v in Helper.GetSuppLines(vo.AccountId))
                {
                    PaymentRecordVO vo2 = new PaymentRecordVO();
                    vo2.AdvancePayment = decimal.Parse(form["advancepayment-" + v.AccountId]);
                    vo2.Deposit = decimal.Parse(form["deposit-" + v.AccountId]);
                    vo2.ForeignDeposit = 0;
                    vo2.Reference = "Supplementary Line - " + v.MSISDN;
                    vo2.OrderId = OrderId;
                    vo2.AccountId = v.AccountId;
                    AccountService.SavePaymentRecord(vo2);
                }

                if (result != false)
                {
                    TempData["Message"] = "Successfully done.";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("View", "Order", new { ID = OrderId, ReturnURL = form["ReturnURL"] });
        }


        [UserAuthorize("ORDER", "DELETE")]
        [HttpPost]
        public ActionResult DoClearPaymentRecord(FormCollection form)
        {
            var OrderId = long.Parse(form["OrderId"]);

            try
            {
                AccountService.ClearPaymentRecord(OrderId);
                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("View", "Order", new { ID = OrderId, ReturnURL = form["ReturnURL"] });
        }

        [UserAuthorize("ORDER", "EDIT")]
        public ActionResult ChangePlan(int AccountId)
        {
            ChangePlanVM model = new ChangePlanVM();
            model.AccountId = AccountId;

            try
            {
                var subProfile = AccountService.GetSubscriberProfile(AccountId);

                var additionalInformation = subProfile.additionalInformation.ToDictionary(v => v.name, v => v.value);
                model.CurrentPlan = additionalInformation["planInfo"];

                try
                {
                    var billCycleDay = additionalInformation["billCycleDay"];
                    if (DateTime.Now.Day > int.Parse(billCycleDay))
                    {
                        DateTime NextCycleBillingDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, int.Parse(billCycleDay)).AddMonths(1).AddDays(1);
                        model.NextCycleBillingDate = NextCycleBillingDate.ToString("dd MMM yyyy");
                    }
                    else
                    {
                        DateTime NextCycleBillingDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, int.Parse(billCycleDay)).AddDays(1);
                        model.NextCycleBillingDate = NextCycleBillingDate.ToString("dd MMM yyyy");
                    }
                }
                catch (Exception)
                {
                    model.NextCycleBillingDate = "N/A";
                }

                List<SelectListItem> SelectList = new List<SelectListItem>();
                var products = ProductService.GetAllProducts();
                var parents = products.Where(d => d.Is_Package == true && d.PRD_TYPE == "Principal" && d.EXT_PROD_NAME != model.CurrentPlan);
                foreach (var v in parents)
                {
                    SelectList.Add(new SelectListItem(){
                        Text = v.EXT_PROD_NAME,
                        Value = v.EXT_PROD_NAME
                    });
                }
                ViewBag.Plans = SelectList;
            }
            catch (Exception e)
            {
                TempData["Message"] = e.Message;
            }

            return View(model);
        }

        [UserAuthorize("ORDER", "EDIT")]
        [HttpPost]
        public ActionResult ChangePlan(ChangePlanVM model, FormCollection form)
        {
            try
            {
                var subProfile = AccountService.GetSubscriberProfile(model.AccountId);

                var additionalInformation = subProfile.additionalInformation.ToDictionary(v => v.name, v => v.value);
                var billCycleDay = additionalInformation["billCycleDay"];
                DateTime NextCycleBillingDate;
                if (DateTime.Now.Day > int.Parse(billCycleDay))
                {
                    NextCycleBillingDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, int.Parse(billCycleDay)).AddMonths(1).AddDays(1);
                }
                else
                {
                    NextCycleBillingDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, int.Parse(billCycleDay)).AddDays(1);
                }

                var result = OrderService.ChangePlanRequest(model.AccountId, model.NewPlan, NextCycleBillingDate);
                TempData["Message"] = "Successfully done.";

                return RedirectToAction("View", "Order", new { ID = result });
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("ChangePlan", "Order", new { AccountId = model.AccountId });
        }

        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult ChangePlanSearch()
        {
            OrderListVM model = new OrderListVM();
            return View();
        }

        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult ChangePlanResult(OrderListVM model)
        {
            model.OrderType = "ChangePlan";
            return View(model);
        }


        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult TerminationSearch()
        {
            OrderListVM model = new OrderListVM();
            return View();
        }

        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult TerminationResult(OrderListVM model)
        {
            model.OrderType = "Termination";
            return View(model);
        }

        [UserAuthorize("ORDER", "EDIT")]
        public ActionResult MakeAdvancePayment(long AccountId, string URL)
        {
            try
            {
                AccountService.MakeAdvancePayment(AccountId);

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return Redirect(URL);
        }



        public ActionResult ManuallyAddAsset(BatchModel Model)
        {
            try
            {
                AssetService AssetService = new AssetService();
                AssetService.ManuallyRetrieveOrderAsset();
            }
            catch(Exception e)
            {
                return RedirectToAction(e.Message, "Order");
            }

            return null;
        }

        [UserAuthorize("ORDER", "EDIT")]
        public ActionResult Terminate(int AccountId)
        {
            TerminateVM model = new TerminateVM();
            model.AccountId = AccountId;

            try
            {
                var subProfile = AccountService.GetSubscriberProfile(AccountId);
                var counter = subProfile.counter.ToDictionary(v => v.name, v => v.value);
                var parameter = subProfile.parameter.ToDictionary(v => v.name, v => v.value);

                model.DepositAmount = ProductService.GetDepositByAccountId(AccountId);
                model.OutstandingAmount = decimal.Parse(counter["AmountDue"]);

                var SettlementAmount = (decimal.Parse(parameter["Prime"]) + decimal.Parse(counter["UnbilledAmount"]));
                if(parameter["PrintedBill"] == "Yes"){
                    SettlementAmount += 3;
                }
                SettlementAmount = (SettlementAmount * (decimal)1.06) + decimal.Parse(counter["AmountDue"]);

                model.SettlementAmount = SettlementAmount;
                model.SubscriptionActivationDate = DateTime.Parse(subProfile.Customer.effective);

                var ContractPeriod = int.Parse(parameter["ContractPeriod"]);
                model.SubscriptionContract = model.SubscriptionActivationDate.AddMonths(ContractPeriod);
            }
            catch (Exception e)
            {
                TempData["Message"] = e.Message;
            }

            return View(model);
        }

        [UserAuthorize("ORDER", "EDIT")]
        [HttpPost]
        public ActionResult Terminate(TerminateVM model, FormCollection form)
        {
            try
            {
                var subProfile = AccountService.GetSubscriberProfile(model.AccountId);
                var counter = subProfile.counter.ToDictionary(v => v.name, v => v.value);
                var parameter = subProfile.parameter.ToDictionary(v => v.name, v => v.value);

                var SettlementAmount = (decimal.Parse(parameter["Prime"]) + decimal.Parse(counter["UnbilledAmount"]));
                if (parameter["PrintedBill"] == "Yes")
                {
                    SettlementAmount += 3;
                }
                SettlementAmount = (SettlementAmount * (decimal)1.06) + decimal.Parse(counter["AmountDue"]);

                //get Account
                var Account = AccountService.Get(model.AccountId);

                //store settlement amount
                Payment payment = new Payment()
                {
                    Amount = SettlementAmount.ToString(),
                    MSISDN = Account.PersonalInfo.MSISDNNumber,
                    Reference =  "Termination Settlement",
                    PaymentMethod = XOXConstants.PAYMENT_METHOD_CASH,
                    PaymentType = XOXConstants.PAYMENT_TYPE_SETTLEMENT,
                    CardIssuerBank = "",
                    CardType = "",
                    CardNumber = "",
                    CardExpiryMonth = 0,
                    CardExpiryYear = 0
                };
                AccountService.CreatePayment(model.AccountId, payment);

                //create terminate order

                //call terminate api

            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return View(model);
        }

        //[UserAuthorize("ORDER", "EDIT")]
        //[HttpPost]
        //public ActionResult ChangePlan(ChangePlanVM model, FormCollection form)
        //{
        //    try
        //    {
        //        var subProfile = AccountService.GetSubscriberProfile(model.AccountId);

        //        var additionalInformation = subProfile.additionalInformation.ToDictionary(v => v.name, v => v.value);
        //        var billCycleDay = additionalInformation["billCycleDay"];
        //        DateTime NextCycleBillingDate;
        //        if (DateTime.Now.Day > int.Parse(billCycleDay))
        //        {
        //            NextCycleBillingDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, int.Parse(billCycleDay)).AddDays(1);
        //        }
        //        else
        //        {
        //            NextCycleBillingDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, int.Parse(billCycleDay)).AddDays(1);
        //        }

        //        var result = OrderService.ChangePlanRequest(model.AccountId, model.NewPlan, NextCycleBillingDate);
        //        TempData["Message"] = "Successfully done.";

        //        return RedirectToAction("View", "Order", new { ID = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Message"] = ex.Message;
        //    }

        //    return RedirectToAction("ChangePlan", "Order", new { AccountId = model.AccountId });
        //}

	}
}