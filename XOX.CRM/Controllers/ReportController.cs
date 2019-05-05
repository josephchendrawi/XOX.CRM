using CRM.Models;
using ExportImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.Common.VO.Audit;
using XOX.CRM.Lib.Interfaces;
using XOX.CRM.Lib.Services;

namespace CRM.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IAccountService AccountService;
        private readonly IAssetService AssetService;
        private readonly IBillPaymentService BillPaymentService;
        private readonly IUserService UserService;
        private readonly IAuditService AuditService;
        private readonly IRefundService RefundService;

        public ReportController()
        {
            this.AccountService = new AccountService();
            this.AssetService = new AssetService();
            this.BillPaymentService = new BillPaymentService();
            this.UserService = new UserService();
            if (Thread.CurrentPrincipal.Identity.Name != null && Thread.CurrentPrincipal.Identity.Name != "")
            {
                this.AuditService = new AuditService(long.Parse(Thread.CurrentPrincipal.Identity.Name), XOXConstants.AUDIT_MODULE_ACCOUNT, XOXConstants.AUDIT_ACTION_UPDATE);
            }
            this.RefundService = new RefundService();
        }

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult PaymentSearch()
        {
            ReportPaymentModel model = new ReportPaymentModel();
            return View();
        }

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult PaymentResult(ReportPaymentModel model)
        {
            return View(model);
        }

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult ListSearchPayment(DataTableParam param, string PaymentType = "", string PaymentMethod = "", string From = "", string To = "")
        {
            try
            {
                var model = new PaymentData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string orderingFunction = sortColumnIndex == 1 ? "Name" :
                                            sortColumnIndex == 2 ? "MSISDN" :
                                            sortColumnIndex == 3 ? "Date" :
                                            sortColumnIndex == 4 ? "PaymentType" :
                                            sortColumnIndex == 5 ? "PaymentMethod" :
                                            sortColumnIndex == 6 ? "Amount" :
                                            sortColumnIndex == 7 ? "Reference" :
                                            "Date";

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                var List = AccountService.GetAllPaymentBySearch(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, new PaymentVO() { PaymentType = PaymentType, PaymentMethod = PaymentMethod, From = From, To = To });

                List<ReportPaymentModel> PaymentList = new List<ReportPaymentModel>();

                long count = param.iDisplayStart + 1;
                foreach (var v in List)
                {
                    ReportPaymentModel VM = new ReportPaymentModel();
                    VM.AccountId = v.AccountId;
                    VM.Amount = v.Amount;
                    VM.Created = v.Created;
                    VM.Date = v.Created.Value.ToString("dd MMM yyyy");
                    VM.MSISDN = v.MSISDN;
                    VM.Name = v.Name;
                    VM.PaymentId = v.PaymentId;
                    VM.PaymentMethod = v.PaymentMethod;
                    VM.PaymentType = v.PaymentType;
                    VM.Reference = v.Reference;
                    VM.CreatedBy = v.CreatedBy;

                    PaymentList.Add(VM);
                }

                model.aaData = PaymentList;
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

        [UserAuthorize("REPORT", "VIEW")]
        public FileContentResult DownloadCSVPayment(string PaymentType = "", string PaymentMethod = "", string From = "", string To = "")
        {
            int TotalCount = 0;
            var List = AccountService.GetAllPaymentBySearch(0, -1, ref TotalCount, "Date", "desc", new PaymentVO() { PaymentType = PaymentType, PaymentMethod = PaymentMethod, From = From, To = To });

            List<PaymentCSV> PaymentList = new List<PaymentCSV>();
            foreach (var v in List)
            {
                PaymentCSV VM = new PaymentCSV();
                VM.Amount = v.Amount;
                VM.Date = v.Created.Value.ToString("dd MMM yyyy");
                VM.MSISDN = "'" + v.MSISDN;
                VM.Name = v.Name;
                VM.PaymentMethod = v.PaymentMethod;
                VM.PaymentType = v.PaymentType;
                VM.Reference = v.Reference;
                VM.CreatedBy = v.CreatedBy;

                PaymentList.Add(VM);
            }

            string csv = Helper.GetCSV(PaymentList);

            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "PaymentReport-" + DateTime.Now + ".csv");
        }


        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult Asset()
        {
            return View();
        }

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult ListAsset(DataTableParam param, string MSISDN = "", string Plan = "")
        {
            try
            {
                var model = new AssetData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string orderingFunction = sortColumnIndex == 0 ? "MSISDN" :
                                            sortColumnIndex == 1 ? "SubscriberName" :
                                            sortColumnIndex == 2 ? "Status" :
                                            sortColumnIndex == 3 ? "EffectiveDate" :
                                            sortColumnIndex == 4 ? "Plan" :
                                            sortColumnIndex == 5 ? "PrintedBilling" :
                                            "AssetId";

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                var qFilter = new AssetVO()
                {
                    MSISDN = MSISDN,
                    Plan = Plan
                };

                var List = AssetService.GetAll(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, qFilter);

                List<AssetModel> AssetList = new List<AssetModel>();

                long count = param.iDisplayStart + 1;
                foreach (var v in List)
                {
                    AssetModel VM = new AssetModel();
                    VM.AssetId = v.AssetId;
                    VM.AccountId = v.AccountId;
                    VM.EffectiveDate = v.EffectiveDate.Value.ToString("dd MMM yyyy HH:mm");
                    VM.MSISDN = v.MSISDN;
                    VM.Plan = v.Plan;
                    VM.PrintedBilling = v.PrintedBilling == true ? "Yes" : "No";
                    VM.Status = v.Status;
                    VM.SubscriberName = v.SubscriberName;

                    AssetList.Add(VM);
                }

                model.aaData = AssetList;
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

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult BillPayment()
        {
            ViewBag.AllCreatedDate = BillPaymentService.GetAllCreatedDate();

            return View();
        }

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult ListBillPayment(DataTableParam param, string Date = "")
        {
            try
            {
                var model = new BillPaymentData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string orderingFunction = sortColumnIndex == 0 ? "MSISDN" :
                                            sortColumnIndex == 1 ? "SubscriberName" :
                                            sortColumnIndex == 2 ? "CreditCardNo" :
                                            sortColumnIndex == 3 ? "CompanyName" :
                                            sortColumnIndex == 4 ? "AmountDue" :
                                            sortColumnIndex == 5 ? "CreditCardNo" :
                                            sortColumnIndex == 6 ? "CreditCardCVV" :
                                            sortColumnIndex == 7 ? "CCExpiry" :
                                            sortColumnIndex == 8 ? "CardIssuerBank" :
                                            "BillPaymentId";

                var sortDirection = Request["sSortDir_0"]; // asc or desc
        
                var qFilter = new BillPaymentVO();
                try
                {
                    DateTime OnDate = DateTime.ParseExact(Date, "dd MMM yyyy", null);
                    qFilter.ToDate = OnDate;
                    qFilter.FromDate = OnDate;
                }
                catch { }
                var List = BillPaymentService.GetAll(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, qFilter);

                List<BillPaymentModel> BillPaymentList = new List<BillPaymentModel>();

                long count = param.iDisplayStart + 1;
                foreach (var v in List)
                {
                    BillPaymentModel VM = new BillPaymentModel();
                    VM.BillPaymentId = v.BillPaymentId;
                    VM.AmountDue = v.AmountDue;
                    VM.BillPaymentId = v.BillPaymentId;
                    VM.CCExpiry = v.CCExpiryYear.ToString().PadLeft(4, '0').Substring(2) + "" + v.CCExpiryMonth.ToString().PadLeft(2, '0');
                    VM.CompanyName = v.CompanyName;
                    VM.CreditCardCVV = v.CreditCardCVV;
                    VM.CreditCardNo = v.CreditCardNo;
                    VM.MSISDN = v.MSISDN[0] == '6' ? v.MSISDN : '6' + v.MSISDN;
                    VM.SubmissionDate = v.SubmissionDate;
                    VM.SubscriberName = v.SubscriberName;
                    VM.CardIssuerBank = v.CardIssuerBank;

                    BillPaymentList.Add(VM);
                }

                model.aaData = BillPaymentList;
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

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult AjaxGetBatchWorkStatistic(string BatchDate = "")
        {
            if (!string.IsNullOrWhiteSpace(BatchDate))
            {
                try
                {
                    DateTime Batch_Date = DateTime.ParseExact(BatchDate, "dd MMM yyyy", null);

                    var result = BillPaymentService.GetErrorAndProcessedCountInBatchWorkLog(Batch_Date);
                    return Json("Total " + result.TotalCount + " record(s) (" + result.SuccessCount + " processed, " + result.ErrorCount + " failed)", JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(e.Message, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult GetBillPaymentExcel(string FromDate, string ToDate)
        {
            DateTime? fromDateTime = DateTime.ParseExact(FromDate, "dd MMM yyyy", null);
            DateTime? toDateTime = DateTime.ParseExact(ToDate, "dd MMM yyyy", null);

            int TotalCount = 0;
            var qFilter = new BillPaymentVO()
            {
                FromDate = fromDateTime,
                ToDate = toDateTime
            };
            var List = new List<BillPaymentReport>();
            foreach (var v in BillPaymentService.GetAll(0, int.MaxValue, ref TotalCount, "", "asc", qFilter))
            {
                List.Add(new BillPaymentReport()
                {
                    SubmissionDate = v.SubmissionDate,
                    CreditCardNumber = v.CreditCardNo,
                    MSISDN = v.MSISDN[0] == '6' ? v.MSISDN : '6' + v.MSISDN,
                    SubscriberName = v.SubscriberName,
                    CompanyName = "XOX MOBILE SDN BHD",
                    Amount = v.AmountDue.ToString(),
                    CVV = "",
                    CreditCardExpiry = v.CCExpiryYear.ToString().PadLeft(4, '0').Substring(2) + "" + v.CCExpiryMonth.ToString().PadLeft(2, '0'),
                });
            }

            var data = ExportFactory.ExportData(List, ExportToFormat.Excel2007);

            return File(data, "xlsx", "Bill_Payment_Report_" + DateTime.Now.ToString("dd_MMM_yyyy") + ".xlsx");
        }

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult GetAssetExcel(string MSISDN, string Plan)
        {
            int TotalCount = 0;
            var qFilter = new AssetVO()
            {
                MSISDN = MSISDN,
                Plan = Plan
            };

            var List = new List<AssetReportModel>();

            foreach (var v in AssetService.GetAll(0, int.MaxValue, ref TotalCount, "", "asc", qFilter))
            {
                AssetReportModel VM = new AssetReportModel();
                VM.MSISDN = v.MSISDN;
                VM.SubscriberName = v.SubscriberName;
                VM.Status = v.Status;
                VM.ActivationDate = v.EffectiveDate.Value.ToString("dd MMM yyyy HH:mm");
                VM.Plan = v.Plan;
                VM.PrintedBilling = v.PrintedBilling == true ? "Yes" : "No";

                List.Add(VM);
            }

            var data = ExportFactory.ExportData(List, ExportToFormat.Excel2007);

            return File(data, "xlsx", "Asset_Report_" + DateTime.Now.ToString("dd_MMM_yyyy") + ".xlsx");
        }


        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult AuditTrail()
        {
            var ListUser = UserService.UserGetAll().Select(m => m.Username);
            var ListModule = AuditService.GetAllModule();

            ViewBag.ListUser = ListUser;
            ViewBag.ListModule = ListModule;

            return View();
        }

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult ListAuditTrail(DataTableParam param, string CreatedBy = "", string Module = "")
        {
            try
            {
                var model = new AuditTrailData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string orderingFunction = sortColumnIndex == 0 ? "CreatedBy" :
                                            sortColumnIndex == 1 ? "Created" :
                                            sortColumnIndex == 2 ? "Action" :
                                            sortColumnIndex == 3 ? "OldValue" :
                                            sortColumnIndex == 4 ? "NewValue" :
                                            sortColumnIndex == 5 ? "Module" :
                                            sortColumnIndex == 6 ? "Field" :
                                            "AuditTrailId";

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                
                long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
                var qFilter = new AuditTrailVO()
                {
                    CreatedByUserName = CreatedBy,
                    MODULE_NAME = Module,
                };

                var List = AuditService.GetAll(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, qFilter);

                List<AuditTrailModel> AuditTrailList = new List<AuditTrailModel>();

                long count = param.iDisplayStart + 1;
                foreach (var v in List)
                {
                    AuditTrailModel VM = new AuditTrailModel();
                    VM.AuditTrailId = v.ROW_ID;
                    VM.Created = v.CREATED.Value.ToString("dd MMM yyyy");
                    VM.CreatedBy = v.CreatedByUserName;
                    VM.Action = v.ACTION_CD;
                    VM.OldValue = v.OLD_VAL;
                    VM.NewValue = v.NEW_VAL;
                    VM.Module = v.MODULE_NAME;
                    VM.Field = v.FIELD_NAME;

                    AuditTrailList.Add(VM);
                }

                model.aaData = AuditTrailList;
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

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult PaymentCardDetails()
        {
            return View();
        }
        
        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult GetPaymentCardDetailsExcel(string CardType, int WithinMonths = 0)
        {
            var List = AccountService.GetAllPaymentCardDetails(CardType, WithinMonths);

            var data = ExportFactory.ExportData(List, ExportToFormat.Excel2007);

            AuditService.AddAuditLog(new AuditLogVO()
            {
                EVENT_TYPE = "Download Account Card Info"
            });

            return File(data, "xlsx", "Payment_Card_Report_" + DateTime.Now.ToString("dd_MMM_yyyy") + ".xlsx");
        }

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult Refund()
        {
            return View();
        }


        [UserAuthorize("REPORT", "ADD")]
        [HttpPost]
        public ActionResult UploadRefundReport(HttpPostedFileBase[] files)
        {
            try
            {
                if (files[0] != null && files.Count() > 0)
                {
                    List<string> FailedData = new List<string>();
                    foreach (var file in files)
                    {
                        StreamReader csvreader = new StreamReader(file.InputStream);

                        while (!csvreader.EndOfStream)
                        {
                            var line = csvreader.ReadLine();
                            var values = line.Split(',');

                            AddRefundVO AddRefundVO = null;
                            try
                            {
                                AddRefundVO = new AddRefundVO()
                                {
                                    MSISDN = values[0].Trim(),
                                    Name = values[1].Trim(),
                                    AdvancePayment = string.IsNullOrWhiteSpace(values[2]) == true ? 0 : decimal.Parse(values[2]),
                                    Deposit = string.IsNullOrWhiteSpace(values[3]) == true ? 0 : decimal.Parse(values[3]),
                                    BillPayment = string.IsNullOrWhiteSpace(values[4]) == true ? 0 : decimal.Parse(values[4]),
                                    Usage = string.IsNullOrWhiteSpace(values[5]) == true ? 0 : decimal.Parse(values[5]),
                                    RefundDate = DateTime.ParseExact(values[6].Trim(), "yyyy/MM/dd", null),
                                };

                                var result = RefundService.Add(AddRefundVO, long.Parse(Thread.CurrentPrincipal.Identity.Name));

                                if (result == 0)
                                {
                                    throw new Exception();
                                }
                            }
                            catch
                            {
                                FailedData.Add(line.Replace(",", " , "));
                            }
                        }
                    }

                    if (FailedData.Count() > 0)
                    {
                        TempData["FailedData"] = FailedData;
                    }

                    TempData["Message"] = "Successfully done.";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Refund", "Report");
        }

        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult ListRefund(DataTableParam param, string From, string To)
        {
            try
            {
                var model = new RefundListVMData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string accountingFunction = sortColumnIndex == 1 ? "Name" :
                                            sortColumnIndex == 2 ? "MSISDN" :
                                            sortColumnIndex == 3 ? "PlanType" :
                                            sortColumnIndex == 4 ? "TerminationDate" :
                                            sortColumnIndex == 5 ? "Deposit" :
                                            sortColumnIndex == 6 ? "AdvancePayment" :
                                            sortColumnIndex == 7 ? "BillPayment" :
                                            sortColumnIndex == 8 ? "Usage" :
                                            sortColumnIndex == 9 ? "TotalRefund" :
                                            sortColumnIndex == 10 ? "RefundDate" :
                                            "AccountId";

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                //filtering
                DateTime? FromDate = null;
                DateTime? ToDate = null;
                try { FromDate = DateTime.ParseExact(From, "dd MMM yyyy", null); }
                catch { }
                try { ToDate = DateTime.ParseExact(To, "dd MMM yyyy", null); }
                catch { }

                var List = RefundService.GetAll(param.iDisplayStart, param.iDisplayLength, ref TotalCount, accountingFunction, sortDirection, FromDate, ToDate);

                List<RefundModel> AccountList = new List<RefundModel>();

                int count = param.iDisplayStart + 1;
                foreach (var v in List)
                {
                    RefundModel VM = new RefundModel();
                    VM.AccountId = v.ACCNT_ID;
                    VM.AdvancePayment = v.ADVANCE_PAYMENT;
                    VM.BillPayment = v.BILL_PAYMENT;
                    VM.Deposit = v.DEPOSIT;
                    VM.MSISDN = v.MSISDN;
                    VM.Name = v.NAME;
                    VM.PlanType = v.PLAN;
                    VM.RefundDate = v.REFUND_DATE == null ? "-" : v.REFUND_DATE.Value.ToString("dd MMM yyyy");
                    VM.RefundId = v.ROW_ID;
                    VM.TerminationDate = v.TERMINATION_DATE == null ? "-" : v.TERMINATION_DATE.Value.ToString("dd MMM yyyy");
                    VM.TotalRefund = v.TOTAL_REFUND;
                    VM.Usage = v.USAGE;

                    AccountList.Add(VM);
                }

                model.aaData = AccountList;
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


        [UserAuthorize("REPORT", "VIEW")]
        public ActionResult GetRefundExcel(string From, string To)
        {
            int TotalCount = 0;

            //filtering
            DateTime? FromDate = null;
            DateTime? ToDate = null;
            try { FromDate = DateTime.ParseExact(From, "dd MMM yyyy", null); }
            catch { }
            try { ToDate = DateTime.ParseExact(To, "dd MMM yyyy", null); }
            catch { }

            var List = RefundService.GetAll(0, int.MaxValue, ref TotalCount, "", "asc", FromDate, ToDate);

            var ExcelData = new List<RefundExcel>();

            foreach (var v in List)
            {
                RefundExcel VM = new RefundExcel();
                VM.AdvancePayment = v.ADVANCE_PAYMENT;
                VM.BillPayment = v.BILL_PAYMENT;
                VM.Deposit = v.DEPOSIT;
                VM.MSISDN = v.MSISDN;
                VM.Name = v.NAME;
                VM.Plan = v.PLAN;
                VM.RefundDate = v.REFUND_DATE == null ? "-" : v.REFUND_DATE.Value.ToString("dd MMM yyyy");
                VM.TerminationDate = v.TERMINATION_DATE == null ? "-" : v.TERMINATION_DATE.Value.ToString("dd MMM yyyy");
                VM.TotalRefund = v.TOTAL_REFUND;
                VM.Usage = v.USAGE;

                ExcelData.Add(VM);
            }

            var data = ExportFactory.ExportData(ExcelData, ExportToFormat.Excel2007);

            return File(data, "xlsx", "Refund_Report_" + DateTime.Now.ToString("dd_MMM_yyyy") + ".xlsx");
        }

	}
}