using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Services;

namespace CRM.Controllers
{
    public class BatchWorkController : BaseController
    {
        public BatchWorkService BatchWorkService = new BatchWorkService();

        [UserAuthorize("BATCHWORK", "VIEW")]
        public ActionResult List()
        {
            BatchWork Model = new BatchWork();
            Model.AllJobType = BatchWorkService.GetAllJobType();
            Model.AllLogFilename = BatchWorkService.GetAllLogFilename();

            return View(Model);
        }

        [UserAuthorize("BATCHWORK", "VIEW")]
        public ActionResult ListBatchWork(DataTableParam param)
        {
            try
            {
                var model = new BatchWorkListData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string orderingFunction = sortColumnIndex == 1 ? "JobName" :
                                            sortColumnIndex == 2 ? "JobType" :
                                            sortColumnIndex == 3 ? "Created" :
                                            sortColumnIndex == 4 ? "Description" :
                                            sortColumnIndex == 5 ? "Remarks" :
                                            sortColumnIndex == 6 ? "StartAt" :
                                            sortColumnIndex == 7 ? "RunSequence" :
                                            sortColumnIndex == 8 ? "Status" :
                                            sortColumnIndex == 9 ? "SendEmailNotifFlag" :
                                            "Created";

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                //filtering : need to pass skip/take and search string to query 
                var filterBy = "";
                var filterQuery = "";

                if (string.IsNullOrEmpty(param.sSearch) == false)
                {
                    var query = param.sSearch.Substring(1);

                    filterBy = "JobName";
                    filterQuery = query;
                }

                var List = BatchWorkService.GetAll(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection);

                List<BatchWork> BatchWorkList = new List<BatchWork>();

                foreach (var v in List)
                {
                    BatchWork VM = new BatchWork();
                    VM.BatchWorkId = v.BatchWorkId;
                    VM.Created = v.Created == null ? "-" : v.Created.Value.ToString("dd MMM yyyy HH:mm");
                    VM.Description = v.Description;
                    VM.EndAtText = v.EndAt == null ? "-" : v.EndAt.Value.ToString("dd MMM yyyy HH:mm");
                    VM.JobName = v.JobName;
                    VM.JobType = v.JobType;
                    VM.Remarks = v.Remarks;
                    VM.StartAtText = v.StartAt == null ? "-" : v.StartAt.Value.ToString("dd MMM yyyy HH:mm");
                    VM.Status = v.Status;
                    VM.StatusText = ((BatchWorkStatus)v.Status).ToString();
                    VM.RunSequence = v.RunSequence;
                    VM.SendEmailNotifFlag = v.SendEmailNotifFlag;

                    var FrequencyType = "";
                    VM.Frequency = MinuteConvert(v.Frequency, ref FrequencyType);
                    VM.FrequencyType = FrequencyType;

                    BatchWorkList.Add(VM);
                }

                model.aaData = BatchWorkList;
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

        [UserAuthorize("BATCHWORK", "ADD")]
        public ActionResult Add()
        {
            BatchWork Model = new BatchWork();
            Model.StartAt = DateTime.Now;
            return View(Model);
        }
        
        [UserAuthorize("BATCHWORK", "ADD")]
        [HttpPost]
        public ActionResult Add(BatchWork Model, FormCollection form)
        {
            try
            {
                BatchWorkService.Add(new XOX.CRM.Lib.BatchWorkVO()
                {
                    Description = Model.Description,
                    JobName = Model.JobName,
                    JobType = Model.JobType,
                    Remarks = Model.Remarks,
                    StartAt = Model.StartAt,
                    Frequency = Model.Frequency,
                    SendEmailNotifFlag = Model.SendEmailNotifFlag
                });

                TempData["Message"] = "Successfully done.";
                return RedirectToAction("List", "BatchWork");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return View(Model);
        }

        [UserAuthorize("BATCHWORK", "EDIT")]
        public ActionResult Edit(long Id)
        {
            var v = BatchWorkService.Get(Id);

            BatchWork VM = new BatchWork();
            VM.BatchWorkId = v.BatchWorkId;
            VM.Description = v.Description;
            VM.JobName = v.JobName;
            VM.JobType = v.JobType;
            VM.Remarks = v.Remarks;
            VM.Status = v.Status;
            VM.StartAt = v.StartAt == null ? DateTime.Now : v.StartAt;
            VM.RunSequence = v.RunSequence;
            VM.SendEmailNotifFlag = v.SendEmailNotifFlag;

            var FrequencyType = "";
            VM.Frequency = MinuteConvert(v.Frequency, ref FrequencyType);
            VM.FrequencyType = FrequencyType;

            return View(VM);
        }

        [UserAuthorize("BATCHWORK", "EDIT")]
        [HttpPost]
        public ActionResult Edit(BatchWork Model, FormCollection form)
        {
            try
            {
                long FrequencyInMinute = 0;
                if (Model.FrequencyType == "hourly")
                {
                    FrequencyInMinute = Model.Frequency * 60;
                }
                else if (Model.FrequencyType == "daily")
                {
                    FrequencyInMinute = Model.Frequency * 60 * 24;
                }
                else if (Model.FrequencyType == "weekly")
                {
                    FrequencyInMinute = Model.Frequency * 60 * 24 * 7;
                }
                else if (Model.FrequencyType == "monthly")
                {
                    FrequencyInMinute = Model.Frequency * 60 * 24 * 30;
                }

                var result = BatchWorkService.Edit(new XOX.CRM.Lib.BatchWorkVO()
                {
                    BatchWorkId = Model.BatchWorkId,
                    Description = Model.Description,
                    JobName = Model.JobName,
                    JobType = Model.JobType,
                    Remarks = Model.Remarks,
                    StartAt = Model.StartAt,
                    Frequency = FrequencyInMinute,
                    //Status = Model.Status,
                    SendEmailNotifFlag = Model.SendEmailNotifFlag
                });

                if (result == true)
                {
                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("List", "BatchWork");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return View(Model);
        }
        
        public long MinuteConvert(long Minute, ref string TypeName)
        {
            var hour = Minute / 60;
            if (hour % 24 == 0)
            {
                var day = hour / 24;
                if (day % 24 == 0)
                {
                    var month = day / 24;
                    TypeName = "monthly";
                    return month;
                }
                else if (day % 7 == 0)
                {
                    var week = day / 7;
                    TypeName = "weekly";
                    return week;
                }
                else
                {
                    TypeName = "daily";
                    return day;
                }
            }
            else
            {
                TypeName = "hourly";
                return hour;
            }
        }

        [UserAuthorize("BATCHWORK", "EDIT")]
        public ActionResult Start(long Id)
        {
            try
            {
                BatchWorkService.ChangeStatus(new XOX.CRM.Lib.BatchWorkVO()
                {
                    BatchWorkId = Id,
                    Status = (int)BatchWorkStatus.Waiting
                });

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("List", "BatchWork");
        }

        [UserAuthorize("BATCHWORK", "EDIT")]
        public ActionResult Stop(long Id)
        {
            try
            {
                BatchWorkService.ChangeStatus(new XOX.CRM.Lib.BatchWorkVO()
                {
                    BatchWorkId = Id,
                    Status = (int)BatchWorkStatus.Stopped
                });

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("List", "BatchWork");
        }

        [UserAuthorize("BATCHWORK", "VIEW")]
        public ActionResult Log(string JobStatus, string JobType, string Filename)
        {
            ViewBag.JobStatus = JobStatus;
            ViewBag.JobType = JobType;
            ViewBag.Filename = Filename;
            return View();
        }

        [UserAuthorize("BATCHWORK", "VIEW")]
        public ActionResult ListBatchWorkLog(DataTableParam param, string JobStatus = "", string JobType = "", string FileName = "")
        {
            try
            {
                var model = new BatchWorkLogListData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string orderingFunction = sortColumnIndex == 0 ? "Created" :
                                            sortColumnIndex == 1 ? "JobName" :
                                            sortColumnIndex == 2 ? "JobType" :
                                            sortColumnIndex == 3 ? "RunSequence" :
                                            sortColumnIndex == 4 ? "Description" :
                                            sortColumnIndex == 5 ? "FileName" :
                                            sortColumnIndex == 6 ? "JobStatus" :
                                            "Created";

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                BatchWorkLogVO filter = new BatchWorkLogVO()
                {
                    BatchWorkId = 0,
                    From = "",
                    To = "",
                    Description = "",
                    RunSequence = 0,
                    JobStatus = JobStatus,
                    JobType = JobType,
                    FileName = FileName,
                };
                var List = BatchWorkService.GetAllLog(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, filter);

                List<BatchWorkLog> BatchWorkLogList = new List<BatchWorkLog>();

                foreach (var v in List)
                {
                    BatchWorkLog VM = new BatchWorkLog();
                    VM.BatchWork = new BatchWork()
                    {
                        BatchWorkId = v.BatchWork.BatchWorkId,
                        JobName = v.BatchWork.JobName,
                        JobType = v.BatchWork.JobType
                    };
                    VM.Created = v.Created == null ? "-" : v.Created.Value.ToString("dd MMM yyyy HH:mm:ss");
                    VM.Description = v.Description;
                    VM.RunSequence = v.RunSequence;
                    VM.JobStatus = v.JobStatus;
                    VM.Filename = v.FileName;
                    BatchWorkLogList.Add(VM);
                }

                model.aaData = BatchWorkLogList;
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

        [UserAuthorize("BATCHWORK", "VIEW")]
        public ActionResult EmailList(long BatchWorkId)
        {
            ViewBag.BatchWorkId = BatchWorkId;
            return View();
        }

        [UserAuthorize("BATCHWORK", "VIEW")]
        public ActionResult ListBatchWorkEmail(DataTableParam param, long BatchWorkId = 0)
        {
            try
            {
                var model = new BatchWorkEmailListData();

                int TotalCount = 0;

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string orderingFunction = sortColumnIndex == 1 ? "Email" :
                                            "Created";

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                var List = BatchWorkService.GetAllBatchWorkEmail(param.iDisplayStart, param.iDisplayLength, ref TotalCount, orderingFunction, sortDirection, BatchWorkId);

                List<BatchWorkEmail> BatchWorkEmailList = new List<BatchWorkEmail>();

                foreach (var v in List)
                {
                    BatchWorkEmail VM = new BatchWorkEmail();
                    VM.BatchWorkEmailId = v.BatchWorkEmailId;
                    VM.BatchWorkId = v.BatchWorkId;
                    VM.Email = v.Email;

                    BatchWorkEmailList.Add(VM);
                }

                model.aaData = BatchWorkEmailList;
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

        [UserAuthorize("BATCHWORK", "ADD")]
        public ActionResult AddEmail(long BatchWorkId)
        {
            BatchWorkEmail Model = new BatchWorkEmail();
            Model.BatchWorkId = BatchWorkId;
            return View(Model);
        }

        [UserAuthorize("BATCHWORK", "ADD")]
        [HttpPost]
        public ActionResult AddEmail(BatchWorkEmailVO Model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    BatchWorkService.AddBatchWorkEmail(new XOX.CRM.Lib.BatchWorkEmailVO()
                    {
                        BatchWorkId = Model.BatchWorkId,
                        Email = Model.Email,
                    });

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("EmailList", "BatchWork", new { BatchWorkId = Model.BatchWorkId });
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(Model);
        }

        [UserAuthorize("BATCHWORK", "DELETE")]
        public ActionResult DeleteEmail(long BatchWorkId, long BatchWorkEmailId)
        {
            BatchWorkService.DeleteBatchWorkEmail(BatchWorkEmailId);

            return RedirectToAction("EmailList", "BatchWork", new { BatchWorkId = BatchWorkId });
        }

	}
}