using CRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using XOX.CRM.Lib;

namespace CRM.Controllers
{
    public class ServiceRequestController : BaseController
    {
        private readonly IAccountService AccountService;
        private readonly IServiceRequestService ServiceRequestService;

        public ServiceRequestController()
        {
            this.AccountService = new AccountService();
            this.ServiceRequestService = new ServiceRequestService();
        }

        [UserAuthorize("SERVICE", "VIEW")]
        public ActionResult Accounts()
        {
            return View();
        }

        [UserAuthorize("SERVICE", "VIEW")]
        public ActionResult Requests(long AccountId = 0)
        {
            if (AccountId != 0)
            {
                ViewBag.AccountId = AccountId;
            }
            return View();
        }

        [UserAuthorize("SERVICE", "VIEW")]
        public ActionResult ListRequest(DataTableParam param, long AccountId = 0)
        {
            try
            {
                var model = new ServiceRequestVMListData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string orderingFunction = sortColumnIndex == 1 ? "AccountName" :
                                            sortColumnIndex == 2 ? "MSISDN" :
                                            sortColumnIndex == 3 ? "Category" :
                                            sortColumnIndex == 4 ? "CreatedDate" :
                                            sortColumnIndex == 5 ? "Status" :
                                            sortColumnIndex == 6 ? "Priority" :
                                            sortColumnIndex == 7 ? "Assignee" :
                                            "ServiceRequestId";

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                var qFilter = new ServiceRequestVO()
                {
                    ///
                };
                var List = ServiceRequestService.GetAllServiceRequests(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, qFilter, AccountId);

                List<ServiceRequestVM> ServiceRequestList = new List<ServiceRequestVM>();

                foreach (var v in List)
                {
                    ServiceRequestVM VM = new ServiceRequestVM();
                    VM.ServiceRequestId = v.ServiceRequestId;
                    VM.Category = Helper.GetLookupNameByVal(v.Category, "ServiceRequestCategory");
                    VM.DueDate = v.DueDate.ToString("dd MMM yyyy hh:mm");
                    VM.Status = Helper.GetDescription((ServiceRequestStatus)v.Status);
                    VM.Priority = Helper.GetDescription((Priority)v.Priority);
                    VM.Assignee = v.Assignee;
                    VM.CreatedDate = v.Created == null ? "" : v.Created.Value.ToString("dd MMM yyyy hh:mm");
                    VM.MSISDN = v.MSISDN;
                    VM.AccountName = v.AccountName;

                    ServiceRequestList.Add(VM);
                }

                model.aaData = ServiceRequestList;
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

        [UserAuthorize("SERVICE", "VIEW")]
        public ActionResult View(int ID)
        {
            ServiceRequestModel model = new ServiceRequestModel();
            
            var result = ServiceRequestService.GetServiceRequest(ID);

            model.ServiceRequestId = result.ServiceRequestId;
            model.Category = result.Category;
            model.Assignee = result.Assignee;
            model.DueDate = result.DueDate;
            model.Status = result.Status;
            model.Priority = result.Priority;
            model.Description = result.Description;
            model.Resolution = result.Resolution;

            model.MSISDN = result.MSISDN;
            model.OldLimit = result.OldLimit;
            model.NewLimit = result.NewLimit;

            model.SimMSISDN = result.SimMSISDN;
            model.NewSIMNumber = result.NewSIMNumber;
            model.OldSIMNumber = result.OldSIMNumber;

            model.AccountId = (long)result.AccountId;

            model.ServiceRequestNumber = result.ServiceRequestNumber;

            model.NewItemisedBilling = result.NewItemisedBilling;
            
            model.Attachments = new List<ServiceRequestAttachment>();

            var att = ServiceRequestService.GetAllServiceRequestAttachments(ID);

            foreach (var v in att)
            {
                var type = v.Path.Substring(v.Path.LastIndexOf(".") + 1);
                if (type == "jpg" || type == "jpeg" || type == "png" || type == "gif")
                    type = "IMAGE";

                model.Attachments.Add(new ServiceRequestAttachment()
                {
                    AttachmentId = v.AttachmentId,
                    ServiceRequestId = result.ServiceRequestId,
                    Path = v.Path,
                    FullPath = ConfigurationManager.AppSettings["UploadPath"] + v.Path,
                    Type = type
                });
            }

            if (model.Category == "UpdateProfile")
            {
                model.SubscriberProfile = new SubscriberProfileM();
                if (result.Status == (int)ServiceRequestStatus.Closed)
                {
                    model.SubscriberProfileBefore = new SubscriberProfileM();

                    model.SubscriberProfileBefore = (SubscriberProfileM)new JavaScriptSerializer().Deserialize(result.OldProfile, typeof(SubscriberProfileM));
                    model.SubscriberProfile = (SubscriberProfileM)new JavaScriptSerializer().Deserialize(result.NewProfie, typeof(SubscriberProfileM));
                }
                else
                {
                    var Account = AccountService.Get(model.AccountId);
                    if (Account.PersonalInfo.BirthDate != DateTime.MinValue)
                    {
                        model.SubscriberProfile.birthDate = Account.PersonalInfo.BirthDate;
                    }
                    else
                    {
                        model.SubscriberProfile.birthDate = DateTime.Today;
                    }
                    model.SubscriberProfile.emailAddress = Account.PersonalInfo.Email;
                    model.SubscriberProfile.ic = Account.PersonalInfo.IdentityNo;
                    model.SubscriberProfile.name = Account.PersonalInfo.FullName;
                    model.SubscriberProfile.preferredLanguage = ((Language)Account.PersonalInfo.PreferredLanguage).ToString();
                    model.SubscriberProfile.salutation = ((Salutation)Account.PersonalInfo.Salutation).ToString();
                    model.SubscriberProfile.postalAddress = Account.AddressInfo.AddressLine1;
                    model.SubscriberProfile.postalAddressL2 = Account.AddressInfo.AddressLine2;
                    model.SubscriberProfile.city = Account.AddressInfo.City;
                    model.SubscriberProfile.postalCode = Account.AddressInfo.Postcode;
                    model.SubscriberProfile.state = Account.AddressInfo.State;
                }
            }
            else if (model.Category == "UpdateDeposit")
            {
                if (result.Status == (int)ServiceRequestStatus.Closed)
                {
                    decimal OldDeposit = 0;
                    decimal.TryParse(result.OldProfile, out OldDeposit);
                    decimal NewDeposit = 0;
                    decimal.TryParse(result.NewProfie, out NewDeposit);

                    model.OldDeposit = OldDeposit;
                    model.NewDeposit = NewDeposit;
                }
                else
                {
                    var Deposit = new ProductService().GetDepositByAccountId(model.AccountId);
                    model.OldDeposit = Deposit;
                    model.NewDeposit = Deposit;
                }
            }

            return View(model);
        }

        [UserAuthorize("SERVICE", "VIEW")]
        public ActionResult ListActivity(DataTableParam param, long ServiceRequestId)
        {
            try
            {
                var model = new ServiceRequestActivityData();

                var result = ServiceRequestService.GetAllServiceRequestActivity(ServiceRequestId);

                List<ServiceRequestActivity> List = new List<ServiceRequestActivity>();

                foreach (var v in result)
                {
                    ServiceRequestActivity VM = new ServiceRequestActivity();
                    VM.ActivityId = v.ActivityId;
                    VM.FieldStaff = v.FieldStaff;
                    VM.VisitDateTimeText = v.VisitDateTime.ToString("dd MMM yyyy hh:mm");
                    VM.Notes = v.Notes;
                    VM.Status = v.Status;

                    List.Add(VM);
                }


                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<ServiceRequestActivity, string> orderingFunction = (c => sortColumnIndex == 1 ? c.VisitDateTimeText :
                                                                    sortColumnIndex == 2 ? c.FieldStaff :
                                                                    sortColumnIndex == 3 ? c.Status :
                                                                    sortColumnIndex == 4 ? c.Notes :
                                                                    c.ActivityId.ToString());

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        List = List.OrderBy(orderingFunction).ToList();
                    }
                    else
                    {
                        List = List.OrderByDescending(orderingFunction).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = List.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.FieldStaff.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = List.Count;
                model.iTotalRecords = List.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("SERVICE", "VIEW")]
        public ActionResult ListNote(DataTableParam param, long ServiceRequestId)
        {
            try
            {
                var model = new ServiceRequestNoteData();

                var result = ServiceRequestService.GetAllServiceRequestNote(ServiceRequestId);

                List<ServiceRequestNote> List = new List<ServiceRequestNote>();

                foreach (var v in result)
                {
                    ServiceRequestNote VM = new ServiceRequestNote();
                    VM.NoteId = v.NoteId;
                    VM.Note = v.Note;

                    List.Add(VM);
                }


                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<ServiceRequestNote, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Note :
                                                                    c.NoteId.ToString());

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        List = List.OrderBy(orderingFunction).ToList();
                    }
                    else
                    {
                        List = List.OrderByDescending(orderingFunction).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = List.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.Note.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = List.Count;
                model.iTotalRecords = List.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("SERVICE", "EDIT")]
        public ActionResult Edit(int ID)
        {
            ServiceRequestModel model = new ServiceRequestModel();

            var result = ServiceRequestService.GetServiceRequest(ID);

            model.ServiceRequestId = result.ServiceRequestId;
            model.Category = result.Category;
            model.Assignee = result.Assignee;
            model.DueDate = result.DueDate;
            model.Status = result.Status;
            model.Priority = result.Priority;
            model.Description = result.Description;
            model.Resolution = result.Resolution;

            model.MSISDN = result.MSISDN;
            model.OldLimit = result.OldLimit;
            model.NewLimit = result.NewLimit;

            model.SimMSISDN = result.SimMSISDN;
            model.NewSIMNumber = result.NewSIMNumber;
            model.OldSIMNumber = result.OldSIMNumber;

            model.Attachments = new List<ServiceRequestAttachment>();

            var att = ServiceRequestService.GetAllServiceRequestAttachments(ID);

            foreach (var v in att)
            {
                var type = v.Path.Substring(v.Path.LastIndexOf(".") + 1);
                if (type == "jpg" || type == "jpeg" || type == "png" || type == "gif")
                    type = "IMAGE";

                model.Attachments.Add(new ServiceRequestAttachment()
                {
                    AttachmentId = v.AttachmentId,
                    ServiceRequestId = result.ServiceRequestId,
                    Path = v.Path,
                    FullPath = ConfigurationManager.AppSettings["UploadPath"] + v.Path,
                    Type = type
                });
            }

            model.Notes = new List<ServiceRequestNote>();

            var note = ServiceRequestService.GetAllServiceRequestNote(ID);

            foreach (var v in note)
            {
                model.Notes.Add(new ServiceRequestNote()
                {
                    NoteId = v.NoteId,
                    Note = v.Note
                });
            }

            return View(model);
        }

        [UserAuthorize("SERVICE", "EDIT")]
        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "Attachments, Notes")]ServiceRequestModel Model, HttpPostedFileBase[] files, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = ServiceRequestService.Edit(new ServiceRequestVO()
                    {
                        ServiceRequestId = Model.ServiceRequestId,
                        Assignee = Session["UserEmail"].ToString(),
                        Category = Model.Category,
                        Description = Model.Description,
                        DueDate = Model.DueDate,
                        Priority = Model.Priority,
                        Resolution = Model.Resolution,
                        Status = Model.Status,

                        MSISDN = Model.MSISDN,
                        OldLimit = Model.OldLimit,
                        NewLimit = Model.NewLimit,

                        SimMSISDN = Model.SimMSISDN,
                        NewSIMNumber = Model.NewSIMNumber,
                        OldSIMNumber = Model.OldSIMNumber,
                    });

                    if (result == true)
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
                        foreach (var f in filespath)
                        {
                            ServiceRequestService.AddAttachment(f, Model.ServiceRequestId);
                        }
                        //remove files
                        var removedIDs = form["removedIDs"];
                        foreach (var v in removedIDs.Split('|'))
                        {
                            if (v != "")
                            {
                                ServiceRequestService.RemoveAttachment(long.Parse(v));
                            }
                        }

                        //add notes
                        foreach (var v in form["note-add"].Split('|'))
                        {
                            if (v != "")
                            {
                                ServiceRequestService.AddNote(v, Model.ServiceRequestId);
                            }
                        }
                        //remove notes
                        foreach (var v in form["note-remove"].Split('|'))
                        {
                            if (v != "")
                            {
                                ServiceRequestService.RemoveNote(long.Parse(v));
                            }
                        }

                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("View", "ServiceRequest", new { ID = Model.ServiceRequestId });
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(Model);
        }

        [UserAuthorize("SERVICE", "ADD")]
        public ActionResult Add(long AccountId)
        {
            ServiceRequestModel model = new ServiceRequestModel();
            model.Attachments = new List<ServiceRequestAttachment>();
            model.DueDate = DateTime.Now;

            var Account = AccountService.Get(AccountId);

            model.MSISDN = Account.PersonalInfo.MSISDNNumber;
            model.OldLimit = Account.PersonalInfo.CreditLimit;
            model.NewLimit = 0;

            model.SimMSISDN = "";
            model.OldSIMNumber = "";
            model.NewSIMNumber = "";

            model.Resolution = (int)ServiceRequestResolution.Open;

            model.AccountId = AccountId;

            return View(model);
        }

        [UserAuthorize("SERVICE", "ADD")]
        [HttpPost]
        public ActionResult Add([Bind(Exclude = "Attachments")]ServiceRequestModel Model, HttpPostedFileBase[] files, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = ServiceRequestService.Add(new ServiceRequestVO()
                    {
                        Assignee = Session["UserEmail"].ToString(),
                        Category = Model.Category,
                        Description = Model.Description,
                        DueDate = Model.DueDate,
                        Priority = Model.Priority,
                        Resolution = Model.Resolution,
                        Status = Model.Status,

                        MSISDN = Model.MSISDN,
                        OldLimit = Model.OldLimit,
                        NewLimit = Model.NewLimit,

                        SimMSISDN = Model.SimMSISDN,
                        NewSIMNumber = Model.NewSIMNumber,
                        OldSIMNumber = Model.OldSIMNumber,
                    }, Model.AccountId);

                    if (result != 0)
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
                        foreach (var f in filespath)
                        {
                            ServiceRequestService.AddAttachment(f, result);
                        }

                        //add notes
                        foreach (var v in form["note-add"].Split('|'))
                        {
                            if (v != "")
                            {
                                ServiceRequestService.AddNote(v, result);
                            }
                        }

                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("View", "ServiceRequest", new { ID = result });
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(Model);
        }

        [UserAuthorize("SERVICE", "EDIT")]
        [HttpPost]
        public ActionResult Submit(FormCollection form)
        {
            long ServiceRequestId = long.Parse(form["ServiceRequestId"]);
            try
            {
                var Category = form["Category"];
                var AccountId = long.Parse(form["AccountId"]);
                var MSISDN = form["MSISDN"];

                if (Category == "UpdateCreditLimit")
                {
                    var subProfile = Helper.GetSubsProfile(AccountId);
                    var parameter = subProfile.parameter.ToDictionary(v => v.name, v => v.value);
                    var Prime = decimal.Parse(parameter["Prime"]);

                    var NewLimit = decimal.Parse(form["NewLimit"]);

                    var GroupCap = Helper.GetLookupValByName(Session["UserGroup"].ToString(), "Credit Limit Max Value");
                    var MaxValue = (2 * Prime) + decimal.Parse(GroupCap == "" || GroupCap == "-" ? "0" : GroupCap);
                    if (NewLimit > MaxValue)
                    {
                        TempData["Message"] = "New Limit value is bigger than " + MaxValue;
                    }
                    else
                    {
                        ServiceRequestService.SubmitCreditLimitRequest(new ServiceRequestVO() { ServiceRequestId = ServiceRequestId, NewLimit = NewLimit, MSISDN = MSISDN }, AccountId);

                        TempData["Message"] = "Successfully done.";
                    }
                }
                else if (Category == "SimReplacement")
                {
                    var OldSIMNumber = form["OldSIMNumber"];
                    var NewSIMNumber = form["NewSIMNumber"];

                    ServiceRequestService.SubmitSIMNumberRequest(new ServiceRequestVO() { ServiceRequestId = ServiceRequestId, OldSIMNumber = OldSIMNumber, NewSIMNumber = NewSIMNumber, MSISDN = MSISDN }, AccountId);

                    TempData["Message"] = "Successfully done.";
                }
                else if (Category == "UpdateProfile")
                {
                    DateTime? birthDate = null;
                    if (string.IsNullOrWhiteSpace(form["SubscriberProfile.birthDate"]) == false)
                    {
                        birthDate = DateTime.ParseExact(form["SubscriberProfile.birthDate"], "dd MMM yyyy", null);
                    }
                    SubscriberProfile SubscriberProfile = new XOX.CRM.Lib.SubscriberProfile()
                    {
                        birthDate = birthDate,
                        city = string.IsNullOrWhiteSpace(form["SubscriberProfile.birthDate"]) ? null : form["SubscriberProfile.city"],
                        emailAddress = string.IsNullOrWhiteSpace(form["SubscriberProfile.emailAddress"]) ? null : form["SubscriberProfile.emailAddress"],
                        ic = string.IsNullOrWhiteSpace(form["SubscriberProfile.ic"]) ? null : form["SubscriberProfile.ic"],
                        name = string.IsNullOrWhiteSpace(form["SubscriberProfile.name"]) ? null : form["SubscriberProfile.name"],
                        postalAddress = string.IsNullOrWhiteSpace(form["SubscriberProfile.postalAddress"]) ? null : form["SubscriberProfile.postalAddress"],
                        postalAddressL2 = string.IsNullOrWhiteSpace(form["SubscriberProfile.postalAddressL2"]) ? null : form["SubscriberProfile.postalAddressL2"],
                        postalCode = string.IsNullOrWhiteSpace(form["SubscriberProfile.postalCode"]) ? null : form["SubscriberProfile.postalCode"],
                        preferredLanguage = string.IsNullOrWhiteSpace(form["SubscriberProfile.preferredLanguage"]) ? null : form["SubscriberProfile.preferredLanguage"],
                        salutation = string.IsNullOrWhiteSpace(form["SubscriberProfile.salutation"]) ? null : form["SubscriberProfile.salutation"],
                        state = string.IsNullOrWhiteSpace(form["SubscriberProfile.state"]) ? null : form["SubscriberProfile.state"],                    
                    };

                    ServiceRequestService.SubmitUpdateSubcriberProfile(new ServiceRequestVO() { ServiceRequestId = ServiceRequestId, SubscriberProfile = SubscriberProfile }, AccountId);

                    TempData["Message"] = "Successfully done.";
                }
                else if (Category == "ManageItemisedBilling")
                {
                    var NewItemisedBilling = bool.Parse(form["NewItemisedBilling"]);

                    ServiceRequestService.SubmitManageItemisedBillingRequest(new ServiceRequestVO() { ServiceRequestId = ServiceRequestId, NewItemisedBilling = NewItemisedBilling, MSISDN = MSISDN }, AccountId);

                    TempData["Message"] = "Successfully done.";
                }
                else if (Category == "UpdateDeposit")
                {
                    var NewDeposit = decimal.Parse(form["NewDeposit"]);

                    ServiceRequestService.SubmitUpdateDeposit(new ServiceRequestVO() { ServiceRequestId = ServiceRequestId, NewDeposit = NewDeposit, MSISDN = MSISDN }, AccountId);

                    TempData["Message"] = "Successfully done.";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("View", "ServiceRequest", new { ID = ServiceRequestId });
        }


        [UserAuthorize("SERVICE", "EDIT")]
        public ActionResult Cancel(long ServiceRequestId)
        {
            try
            {
                ServiceRequestService.CancelRequest(new ServiceRequestVO() { ServiceRequestId = ServiceRequestId, Status = (int)ServiceRequestStatus.Cancelled });

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("View", "ServiceRequest", new { ID = ServiceRequestId });
        }


        [UserAuthorize("SERVICE", "EDIT")]
        [HttpPost]
        public ActionResult EditAttachment(HttpPostedFileBase[] files, FormCollection form)
        {
            long ServiceRequestId = long.Parse(form["ServiceRequestId"]);
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
                foreach (var f in filespath)
                {
                    ServiceRequestService.AddAttachment(f, ServiceRequestId);
                }
                //remove files
                var removedIDs = form["removedIDs"];
                foreach (var v in removedIDs.Split('|'))
                {
                    if (v != "")
                    {
                        ServiceRequestService.RemoveAttachment(long.Parse(v));
                    }
                }
                                        

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("View", "ServiceRequest", new { ID = ServiceRequestId });
        }

	}
} 