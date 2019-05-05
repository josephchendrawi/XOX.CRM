using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Enum;

namespace CRM.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IAccountService AccountService;
        private readonly IOrderService OrderService;
        private readonly ICommonService CommonService;
        private readonly IBillPaymentService BillPaymentService;

        public HomeController()
        {
            this.AccountService = new AccountService();
            this.OrderService = new  OrderService();
            this.CommonService = new  CommonService();
            this.BillPaymentService = new  BillPaymentService();
        }

        public ActionResult Index()
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
                                            sortColumnIndex == 2 ? "Status" :
                                            "OrderId";

                var sortDirection = Request["sSortDir_0"]; // asc or desc


                //filtering : need to pass skip/take and search string to query 
                var filterBy = "Status";
                var filterQuery = param.sSearch;

                //if (string.IsNullOrEmpty(param.sSearch) == false)
                //{
                //    var type = param.sSearch[0];
                //    var query = param.sSearch.Substring(1);

                //    filterBy = type == '1' ? "OrderNum" :
                //                type == '2' ? "MSISDN" :
                //                type == '3' ? "FullName" :
                //                type == '4' ? "RegistrationDate" :
                //                type == '5' ? "Category" :
                //                type == '6' ? "Status" :
                //                "";
                //    filterQuery = query;
                //}

                string UserEmail = (string)Session["UserEmail"];

                //var List = OrderService.GetAll(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, filterBy, filterQuery, UserEmail);
                var List = OrderService.GetAll(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, filterBy, filterQuery, "");

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

        public JsonResult GetOverallUsage()
        {
            int ActiveSubscriberCount = 0;
            var ActiveSubscriberList = AccountService.GetAllBySearch(0, 1, ref ActiveSubscriberCount, "", "", "0", "", "", "0", "", "", (AccountStatus.Active).ToString(), "");

            decimal PaymentCollectedSum = new PaymentCollectedService().GetAmountSum(DateTime.Now.Month, DateTime.Now.Year);

            //int BillPaymentCount = 0;
            //DateTime FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //DateTime ToDate = FromDate.AddMonths(1).AddDays(-1);
            //var BillPaymentList = BillPaymentService.GetAll(0, int.MaxValue, ref BillPaymentCount, "", "", new BillPaymentVO() { SubmissionDate = DateTime.Now.ToString("MMyyyy") });
            
            int TerminatedSubscriberinThisMonthCount = 0;
            var From = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var To = From.AddMonths(1).AddDays(-1);
            TerminatedSubscriberinThisMonthCount = AccountService.GetTerminatedAccountCount(From, To);

            OverallStatus OverallStatus = new OverallStatus();
            OverallStatus.ActiveSubscriberCount = ActiveSubscriberCount;
            //OverallStatus.BillPaymentSum = BillPaymentList.Sum(m => m.AmountDue) ?? 0;
            OverallStatus.TerminatedSubscriberinThisMonthCount = TerminatedSubscriberinThisMonthCount;
            OverallStatus.PaymentCollectedSum = PaymentCollectedSum;

            return Json(OverallStatus, JsonRequestBehavior.AllowGet);
        }
    }
}