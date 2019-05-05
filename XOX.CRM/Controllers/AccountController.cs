using CRM.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Constants;

namespace CRM.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService AccountService;
        private readonly IAccountAttachmentService AccountAttachmentService;
        private readonly ICommonService CommonService;

        public AccountController()
        {
            this.AccountService = new  AccountService();
            this.AccountAttachmentService = new  AccountAttachmentService();
            this.CommonService = new CommonService();
        }

        [UserAuthorize("ACCOUNT", "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize("ACCOUNT", "VIEW")]
        public ActionResult ListAccount(DataTableParam param)
        {
            try
            {
                var model = new AccountListVMData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string orderingFunction = sortColumnIndex == 1 ? "Salutation" :
                                            sortColumnIndex == 2 ? "FullName" :
                                            sortColumnIndex == 3 ? "Email" :
                                            sortColumnIndex == 4 ? "IdentityType" :
                                            sortColumnIndex == 5 ? "IdentityNumber" :
                                            "OrderId";

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                //filtering : need to pass skip/take and search string to query 
                var filterBy = "";
                var filterQuery = "";

                if (string.IsNullOrEmpty(param.sSearch) == false)
                {
                    var query = param.sSearch.Substring(1);

                    filterBy = "FullName";
                    filterQuery = query;
                }

                var List = AccountService.GetAll(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, filterBy, filterQuery);

                List<AccountListVM> AccountList = new List<AccountListVM>();

                foreach (var v in List)
                {
                    AccountListVM VM = new AccountListVM();
                    VM.AccountId = v.AccountId;
                    VM.Email = v.PersonalInfo.Email;
                    VM.FullName = v.PersonalInfo.FullName;
                    VM.IdentityNo = v.PersonalInfo.IdentityNo;
                    VM.IdentityType = Helper.GetDescription((IdentityType)v.PersonalInfo.IdentityType);
                    VM.Salutation = Helper.GetDescription((Salutation)v.PersonalInfo.Salutation);

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

        [UserAuthorize("ACCOUNT", "VIEW")]
        public ActionResult View(long ID, string ErrMsg)
        {
            AccountModel model = new AccountModel();

            var result = AccountService.Get(ID);

            model = AccountMapper.Map(result);
            model.AccountUsage = new AccountUsage();

            try
            {
                var subProfile = AccountService.GetSubscriberProfile(ID);

                var additionalInfo = subProfile.additionalInfo.ToDictionary(v => v.name, v => v.value);
                model.AccountUsage.previousBalance = additionalInfo["previousBalance"];

                var parameter = subProfile.parameter.ToDictionary(v => v.name, v => v.value);
                model.AccountUsage.ContractPeriod = parameter["ContractPeriod"] + " months";
                model.AccountUsage.DataAddOn = parameter["DataAddOn"];
                model.AccountUsage.DataSMS = parameter["DataSMS"];
                model.AccountUsage.Deposit = parameter["Deposit"];
                model.AccountUsage.SignUpChannel = parameter["SignUpChannel"];
                model.AccountUsage.OriginalBillingDate = parameter["OriginalBillingDate"];
                model.PersonalInfo.CreditLimit = decimal.Parse(parameter["CreditLimit"]);

                var counter = subProfile.counter.ToDictionary(v => v.name, v => v.value);
                model.AccountUsage.AmountDue = counter["AmountDue"];
                model.AccountUsage.UnbilledAmount = counter["UnbilledAmount"];
                model.AccountUsage.MonthlyPayments = counter["MonthlyPayments"];
                model.AccountUsage.RemainingDeposit = counter["RemainingDeposit"];
                model.AccountUsage.DataExpiration = counter["DataExpiration"];
                
                model.AccountUsage.effectiveBillingDate = DateTime.Parse(subProfile.Customer.effective).ToString("dd MMM yyyy (HH:mm)");
                model.AccountUsage.DataExpiration = (new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(double.Parse(model.AccountUsage.DataExpiration)).ToLocalTime()).ToString("dd MMM yyyy (HH:mm)");
                model.AccountUsage.OriginalBillingDate = DateTime.ParseExact(model.AccountUsage.OriginalBillingDate.Replace("MYT", "+08:00"), "ddd MMM dd HH:mm:ss zzz yyyy", null).ToString("dd MMM yyyy (HH:mm)");
                
                var additionalInformation = subProfile.additionalInformation.ToDictionary(v => v.name, v => v.value);
                model.Plan = additionalInformation["planInfo"];

                try
                {
                    var billCycleDay = additionalInformation["billCycleDay"];
                    if (DateTime.Now.Day > int.Parse(billCycleDay))
                    {
                        DateTime NextCycleBillingDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, int.Parse(billCycleDay));
                        model.AccountUsage.NextCycleBillingDate = NextCycleBillingDate.ToString("dd MMM yyyy");
                    }
                    else
                    {
                        DateTime NextCycleBillingDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, int.Parse(billCycleDay));
                        model.AccountUsage.NextCycleBillingDate = NextCycleBillingDate.ToString("dd MMM yyyy");
                    }
                }
                catch (Exception)
                {
                    model.AccountUsage.NextCycleBillingDate = "N/A";
                }
            }
            catch(Exception e)
            {
                TempData["Message"] = e.Message;
                if (ErrMsg != null && ErrMsg != "")
                {
                    TempData["Message"] = ErrMsg + "... " + TempData["Message"];
                }
            }

            return View(model);
        }

        [UserAuthorize("ACCOUNT", "VIEW")]
        public FileStreamResult GetFile(string filepath)
        {
            string serverPath = XOXConstants.DRIVE_D_ATT_PATH;
            string fileName = serverPath + @"\" + filepath;
            FileInfo info = new FileInfo(fileName);
            if (!info.Exists)
            {
                //
            }

            filepath = filepath.Replace("\\", "-");
            return File(info.OpenRead(), System.Net.Mime.MediaTypeNames.Application.Octet, filepath);
        }

        [UserAuthorize("ACCOUNT", "EDIT")]
        public ActionResult Edit(long ID, string ReturnURL)
        {
            AccountModel model = new AccountModel();

            var result = AccountService.Get(ID);

            model = AccountMapper.Map(result);

            return View(model);
        }

        [UserAuthorize("ACCOUNT", "EDIT")]
        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "Files")]AccountModel Model, HttpPostedFileBase[] files, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var ErrMessage = "";
                    if (Model.BankingInfo == null)
                    {
                        Model.BankingInfo = new BankingInfo();
                    }
                    var vo = AccountMapper.ReMap(Model);
                    bool result = false;
                    try
                    {
                        result = AccountService.Edit(vo);
                    }
                    catch (Exception e)
                    {
                        if (e.Message.Contains("Successfully edited."))
                        {
                            ErrMessage = e.Message;
                            result = true;
                        }
                        else
                        {
                            throw e;
                        }
                    }

                    if (result == true)
                    {
                        //upload files
                        List<string> filespath = new List<string>();
                        if (files != null && files.Count() > 0)
                        {
                            foreach (var file in files)
                            {
                                if(file != null)
                                    filespath.Add(Helper.UploadingFile2DriveD(file, "account-" + Model.AccountId));
                            }
                        }
                        if(filespath.Count() > 0)
                            AccountAttachmentService.AddFiles(filespath, Model.AccountId);

                        //remove files
                        var removedIDs = form["removedIDs"];
                        foreach (var v in removedIDs.Split('|'))
                        {
                            if (v != "")
                            {
                                AccountAttachmentService.RemoveFile(long.Parse(v));
                            }
                        }

                        TempData["Message"] = "Successfully done.";

                        if (form["ReturnURL"] != null && form["ReturnURL"] != "")
                        {
                            if (form["ReturnURL"].Contains("?"))
                            {
                                return Redirect(form["ReturnURL"] + "&ErrMsg=" + ErrMessage);
                            }
                            else
                            {
                                return Redirect(form["ReturnURL"] + "?ErrMsg=" + ErrMessage);
                            }
                        }
                        else
                        {
                            return RedirectToAction("View", "Account", new { ID = Model.AccountId, ErrMsg = ErrMessage });
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }
            else
            {
                TempData["Message"] = string.Join(" ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
            }

            return View(Model);
        }

        [UserAuthorize("ACCOUNT", "EDIT")]
        [HttpPost]
        public ActionResult UpdateStatus(FormCollection form)
        {
            var AccountId = long.Parse(form["AccountId"]);

            try
            {
                AccountActivityVO Activity = new AccountActivityVO();
                Activity.ACCNT_ID = AccountId;
                Activity.REASON = "";
                if (Session["UserEmail"] != null)
                {
                    Activity.ASSIGNEE = Session["UserEmail"].ToString();
                }
                var result = AccountService.UpdateStatus(Activity, AccountStatus.Terminated.ToString());

                if (result != false)
                {
                    TempData["Message"] = "Successfully done.";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("View", "Account", new { ID = AccountId });
        }

        [UserAuthorize("ACCOUNT", "ADD")]
        public ActionResult Registration()
        {
            AccountModel model = new AccountModel();

            model.PersonalInfo = new Models.PersonalInfo() { BirthDate = DateTime.Now };
            model.BankingInfo = new Models.BankingInfo();
            model.AddressInfo = new Models.AddressInfo();

            return View(model);
        }

        [UserAuthorize("ACCOUNT", "ADD")]
        [HttpPost]
        public ActionResult Registration([Bind(Exclude = "Files")]AccountModel Model, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vo = AccountMapper.ReMap(Model);

                    var result = AccountService.Add(vo);

                    if (result != 0)
                    {
                        List<string> filespath = new List<string>();
                        if (files[0] != null && files.Count() > 0)
                        {
                            foreach (var file in files)
                            {
                                filespath.Add(Helper.UploadingFile2DriveD(file, "account-" + result));
                            }
                        }
                        AccountAttachmentService.AddFiles(filespath, result);

                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("View", "Account", new { ID = result });
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(Model);
        }

        [UserAuthorize("ACCOUNT", "VIEW")]
        public ActionResult ListAccountActivity(DataTableParam param, long AccountId = 0)
        {
            try
            {
                var model = new AccountActivityListData();

                var List = AccountService.GetAccountActivities(AccountId);

                List<AccountActivity> AccountActivityList = new List<AccountActivity>();
                foreach (var v in List)
                {
                    AccountActivity VM = new AccountActivity();
                    VM.AccountActivityId = v.ROW_ID;
                    VM.AccountId = v.ACCNT_ID;
                    VM.Assignee = v.ASSIGNEE;
                    VM.Description = v.ACT_DESC;
                    VM.Reason = v.REASON;
                    VM.Status = v.STATUS;
                    VM.CreatedDateTime = ((DateTime)v.CREATED).ToString("yyyy MM dd hh:mm");
                    VM.CreatedDateTimeText = ((DateTime)v.CREATED).ToString("dd MMM yyyy hh:mm");

                    AccountActivityList.Add(VM);
                }

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                if (sortColumnIndex != 0)
                {
                    Func<AccountActivity, string> orderingFunction = (c => sortColumnIndex == 0 ? c.Description :
                                                                        sortColumnIndex == 1 ? c.Reason :
                                                                        sortColumnIndex == 2 ? c.Assignee :
                                                                        sortColumnIndex == 3 ? c.CreatedDateTime :
                                                                        c.AccountActivityId.ToString());

                    var sortDirection = Request["sSortDir_0"]; // asc or desc

                    if (!string.IsNullOrEmpty(sortDirection))
                    {
                        if (sortDirection == "asc")
                        {
                            AccountActivityList = AccountActivityList.OrderBy(orderingFunction).ToList();
                        }
                        else
                        {
                            AccountActivityList = AccountActivityList.OrderByDescending(orderingFunction).ToList();
                        }
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = AccountActivityList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.Description.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = AccountActivityList.Count;
                model.iTotalRecords = AccountActivityList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("CUG", "VIEW")]
        public ActionResult ListCUG(DataTableParam param, long AccountId = 0)
        {
            try
            {
                var model = new CUGListData();

                List<CUGModel> CUGList = new List<CUGModel>();

                try
                {
                    var List = AccountService.GetCUG(AccountId);

                    foreach (var v in List)
                    {
                        CUGModel VM = new CUGModel();
                        VM.CUGNumber = v;

                        CUGList.Add(VM);
                    }
                }
                catch { }


                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<CUGModel, string> orderingFunction = (c => sortColumnIndex == 1 ? c.CUGNumber :
                                                                    c.CUGNumber);

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        CUGList = CUGList.OrderBy(orderingFunction).ToList();
                    }
                    else
                    {
                        CUGList = CUGList.OrderByDescending(orderingFunction).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = CUGList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.CUGNumber.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = CUGList.Count;
                model.iTotalRecords = CUGList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("CUG", "EDIT")]
        [HttpPost]
        public ActionResult AddCUG(FormCollection form)
        {
            string NewCUGNumber = form["NewCUGNumber"];
            long AccountId = long.Parse(form["AccountId"]);

            try
            {
                AccountService.AddCUG(AccountId, NewCUGNumber);

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("View", "Account", new { ID = AccountId, ErrMsg = TempData["Message"].ToString() });
        }

        [UserAuthorize("CUG", "EDIT")]
        public ActionResult DeleteCUG(string CUGNo, long AccountId)
        {
            try
            {
                AccountService.RemoveCUG(AccountId, CUGNo);

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("View", "Account", new { ID = AccountId, ErrMsg = TempData["Message"].ToString() });
        }


        [UserAuthorize("ACCOUNT", "VIEW")]
        public ActionResult ListSupplementaryLine(DataTableParam param, long ID)
        {
            try
            {
                var model = new AccountListVMData();

                var List = AccountService.GetAllSupplementaryLine(ID);

                List<AccountListVM> AccountList = new List<AccountListVM>();

                foreach (var v in List)
                {
                    AccountListVM VM = new AccountListVM();
                    VM.AccountId = v.AccountId;
                    VM.Email = v.PersonalInfo.Email;
                    VM.FullName = v.PersonalInfo.FullName;
                    VM.IdentityNo = v.PersonalInfo.IdentityNo;
                    VM.IdentityType = Helper.GetDescription((IdentityType)v.PersonalInfo.IdentityType);
                    VM.Salutation = Helper.GetDescription((Salutation)v.PersonalInfo.Salutation);
                    VM.MSISDN = v.PersonalInfo.MSISDNNumber;

                    AccountList.Add(VM);
                }


                //sorting properties : need to pass respective column to sort in query
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<AccountListVM, string> orderingFunction = (c => sortColumnIndex == 0 ? c.Salutation :
                                                                    sortColumnIndex == 1 ? c.FullName :
                                                                    sortColumnIndex == 4 ? c.MSISDN :
                                                                    c.AccountId.ToString());

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        AccountList = AccountList.OrderBy(orderingFunction).ToList();
                    }
                    else
                    {
                        AccountList = AccountList.OrderByDescending(orderingFunction).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = AccountList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.FullName.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = AccountList.Count;
                model.iTotalRecords = AccountList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("ACCOUNT", "EDIT")] ///
        public ActionResult CreatePayment(int AccountId)
        {
            var Account = AccountService.Get(AccountId);

            PaymentModel model = new PaymentModel();
            model.MSISDN = Account.PersonalInfo.MSISDNNumber;
            model.AccountId = AccountId;
            model.CardIssuerBank = Account.BankingInfo.CardIssuerBank;
            model.CardNumber = Account.BankingInfo.CreditCardNo;
            model.CardExpiryMonth = Account.BankingInfo.CardExpiryMonth;
            model.CardExpiryYear = Account.BankingInfo.CardExpiryYear;
            return View(model);
        }

        [UserAuthorize("ACCOUNT", "EDIT")]
        [HttpPost]
        public ActionResult CreatePayment(PaymentModel model, FormCollection form)
        {
            try
            {
                if (model.PaymentMethod == "Credit Card" && (string.IsNullOrWhiteSpace(model.CardNumber) || string.IsNullOrWhiteSpace(model.CardIssuerBank)))
                {
                    TempData["Message"] = "Card Number and Card Issuer Bank should be filled";
                    return View();
                }

                if (model.Amount > decimal.Parse(CommonService.GetLookupValueByName("Payment Limit", "Payment Limit")))
                {
                    TempData["Message"] = "Amount cannot be more than " + CommonService.GetLookupValueByName("Payment Limit", "Payment Limit");
                    return View();
                }

                var result = AccountService.MakePayment(model.AccountId, new Payment()
                {
                    Amount = model.Amount.ToString(),
                    MSISDN = model.MSISDN,
                    Reference = model.Reference,
                    PaymentMethod = model.PaymentMethod,
                    PaymentType = XOXConstants.PAYMENT_TYPE_BILLING,
                    CardIssuerBank = model.CardIssuerBank,
                    CardType = model.CardType,
                    CardNumber = model.CardNumber,
                    CardExpiryMonth = model.CardExpiryMonth,
                    CardExpiryYear = model.CardExpiryYear
                },true);

                try
                {
                    if (bool.Parse(result) == true)
                    {
                        TempData["Message"] = "Successfully done.";
                    }
                }
                catch
                {
                    TempData["Message"] = result;
                    return View(model);
                }
                return RedirectToAction("View", "Account", new { ID = model.AccountId });
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return View(model);
        }

        [UserAuthorize("ACCOUNT", "VIEW")]
        public ActionResult ListAccountPayment(DataTableParam param, long AccountId = 0)
        {
            try
            {
                var model = new AccountPaymentVMData();

                var List = AccountService.GetPayment(AccountId);

                List<AccountPaymentVM> AccountPaymentList = new List<AccountPaymentVM>();
                for (int i = 0; i < List.Count(); i++)
                {
                    if (List[i].PaymentList != null)
                    {
                        foreach (var v in List[i].PaymentList)
                        {
                            AccountPaymentVM VM = new AccountPaymentVM();
                            VM.amount = v.amount;
                            VM.buyerID = v.buyerID;
                            VM.description = v.description;
                            VM.operationID = v.operationID;
                            VM.paymentDate = v.paymentDate;
                            VM.paymentID = v.paymentID;
                            VM.paymentInstrumentID = v.paymentInstrumentID;
                            VM.paymentMethod = v.paymentMethod;
                            VM.payType = v.payType;
                            VM.status = v.status;
                            VM.vendorID = v.vendorID;

                            AccountPaymentList.Add(VM);
                        }
                    }
                }

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                if (sortColumnIndex != 0)
                {
                    Func<AccountPaymentVM, string> orderingFunction = (c => sortColumnIndex == 0 ? c.amount :
                                                                        sortColumnIndex == 1 ? c.buyerID :
                                                                        sortColumnIndex == 2 ? c.description :
                                                                        sortColumnIndex == 3 ? c.operationID :
                                                                        sortColumnIndex == 4 ? c.paymentDate :
                                                                        sortColumnIndex == 5 ? c.paymentID :
                                                                        sortColumnIndex == 6 ? c.paymentInstrumentID :
                                                                        sortColumnIndex == 7 ? c.paymentMethod :
                                                                        sortColumnIndex == 8 ? c.payType :
                                                                        sortColumnIndex == 9 ? c.status :
                                                                        sortColumnIndex == 10 ? c.vendorID :
                                                                        c.paymentID);

                    var sortDirection = Request["sSortDir_0"]; // asc or desc

                    if (!string.IsNullOrEmpty(sortDirection))
                    {
                        if (sortDirection == "asc")
                        {
                            AccountPaymentList = AccountPaymentList.OrderBy(orderingFunction).ToList();
                        }
                        else
                        {
                            AccountPaymentList = AccountPaymentList.OrderByDescending(orderingFunction).ToList();
                        }
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = AccountPaymentList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.description.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = AccountPaymentList.Count;
                model.iTotalRecords = AccountPaymentList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /*[UserAuthorize("ACCOUNT", "VIEW")]
        public FileStreamResult GetPaymentPDF(long AccountId)
        {
            var List = AccountService.GetPayment(AccountId);

            MemoryStream stream = new MemoryStream();

            //Start of PDF work using iTextSharp PDF library
            iTextSharp.text.Document pdf = new iTextSharp.text.Document();
            PdfWriter writer = PdfWriter.GetInstance(pdf, stream);
            pdf.Open();
            pdf.Add(new Phrase("test"));
            pdf.Close();
            //End of PDF work using iTextSharp PDF library

            //Where the download magic happens
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Log.pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(stream.GetBuffer(), 0, stream.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }*/


        [UserAuthorize("ACCOUNT", "VIEW")]
        public ActionResult Search()
        {
            AccountListVM model = new AccountListVM();
            model.AccountType = "1";
            return View(model);
        }

        [UserAuthorize("ACCOUNT", "VIEW")]
        public ActionResult Result(AccountListVM model)
        {
            return View(model);
        }

        [UserAuthorize("ORDER", "VIEW")]
        public ActionResult ListSearchAccount(DataTableParam param, string Salutation = "0", string FullName = "", string Email = "", string IdentityType = "0", string IdentityNo = "", string MSISDN = "", string Status = "", string AccountType = "")
        {
            try
            {
                var model = new AccountListVMData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string accountingFunction = sortColumnIndex == 1 ? "Salutation" :
                                            sortColumnIndex == 2 ? "FullName" :
                                            sortColumnIndex == 3 ? "MSISDN" :
                                            sortColumnIndex == 4 ? "IdentityType" :
                                            sortColumnIndex == 5 ? "IdentityNo" :
                                            sortColumnIndex == 6 ? "Status" :
                                            "AccountId";

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                Salutation = Salutation == "" ? "0" : Salutation;
                IdentityType = IdentityType == "" ? "0" : IdentityType;
                Status = Status == "" ? "0" : Status;
                var List = AccountService.GetAllBySearch(param.iDisplayStart, param.iDisplayLength, ref TotalCount, accountingFunction, sortDirection, ((Salutation)int.Parse(Salutation)).ToString(), FullName, Email, ((IdentityType)int.Parse(IdentityType)).ToString(), IdentityNo, MSISDN, ((AccountStatus)int.Parse(Status)).ToString(), AccountType);

                List<AccountListVM> AccountList = new List<AccountListVM>();

                int count = param.iDisplayStart + 1;
                foreach (var v in List)
                {
                    AccountListVM VM = new AccountListVM();
                    VM.AccountId = v.AccountId;
                    VM.MSISDN = v.PersonalInfo.MSISDNNumber;
                    VM.FullName = v.PersonalInfo.FullName;
                    VM.IdentityNo = v.PersonalInfo.IdentityNo;
                    VM.IdentityType = Helper.GetDescription((IdentityType)v.PersonalInfo.IdentityType);
                    VM.Salutation = Helper.GetDescription((Salutation)v.PersonalInfo.Salutation);
                    VM.Status = Helper.GetDescription((AccountStatus)v.PersonalInfo.CustomerStatus);
                    VM.Idx = count++;

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

	}
}