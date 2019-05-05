using CRM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XOX.CRM.Lib;

namespace CRM.Controllers
{
    public class MobileNumController : BaseController
    {
        private readonly IMobileNumService MobileNumService;

        public MobileNumController()
        {
            this.MobileNumService = new MobileNumService();
        }

        [UserAuthorize("MOBILENUM", "VIEW")]
        public ActionResult List()
        {
            return View();
        }

        [UserAuthorize("MOBILENUM", "VIEW")]
        public ActionResult ListMobileNum(DataTableParam param)
        {
            try
            {
                var model = new MobileNumListData();

                var MNList = MobileNumService.GetAll();

                List<MobileNumModel> MobileNumList = new List<MobileNumModel>();
                MobileNumList = MobileNumMapper.Map(MNList);

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<MobileNumModel, string> orderingFunction = (c => sortColumnIndex == 1 ? c.MSISDN :
                                                                    sortColumnIndex == 2 ? c.Price.ToString() :
                                                                    sortColumnIndex == 3 ? c.BatchNum :
                                                                    sortColumnIndex == 4 ? c.CreatedDate :
                                                                    c.CreatedDate);

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        MobileNumList = MobileNumList.OrderBy(orderingFunction).ToList();
                    }
                    else
                    {
                        MobileNumList = MobileNumList.OrderByDescending(orderingFunction).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = MobileNumList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.MSISDN.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = MobileNumList.Count;
                model.iTotalRecords = MobileNumList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("MOBILENUM", "ADD")]
        public ActionResult Add()
        {
            TempData["MNList"] = TempData["MNList"];

            return View();
        }

        [UserAuthorize("MOBILENUM", "ADD")]
        [HttpPost]
        public ActionResult Add(MobileNumModel m)
        {
            var MNList = TempData["MNList"] as List<MobileNumModel>;
            TempData["MNList"] = null;

            if (MNList != null)
            {
                foreach (var v in MNList)
                {
                    MobileNumService.Add(MobileNumMapper.ReMap(v));
                }

                TempData["MNList"] = null;
                TempData["Message"] = "Successfully done.";
                return RedirectToAction("List", "MobileNum");
            }
            else
            {
                TempData["Message"] = "No record found.";
                return View();
            }
        }

        [UserAuthorize("MOBILENUM", "ADD")]
        [HttpPost]
        public ActionResult BatchFileUpload(HttpPostedFileBase file)
        {
            TempData["MNList"] = null;
            if (file.ContentLength > 0)
            {
                try
                {
                    using (StreamReader CsvReader = new StreamReader(file.InputStream))
                    {
                        var MNList = new List<MobileNumModel>();

                        string inputLine = "";
                        while ((inputLine = CsvReader.ReadLine()) != null)
                        {
                            var r = inputLine.Split(','); //array 0 = BatchNumber, arr 1 = MSISDN, arr 2 = Price
                            MNList.Add(new MobileNumModel() { MSISDN = r[1], Price = decimal.Parse(r[2]), BatchNum = r[0] });
                        }

                        CsvReader.Close();
                        TempData["MNList"] = MNList;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }
            else
            {
                TempData["Message"] = "Please upload CSV file.";
            }

            return RedirectToAction("Add","MobileNum");
        }

        [UserAuthorize("MOBILENUM", "VIEW")]
        public ActionResult ListUploadedMobileNum(DataTableParam param)
        {
            try
            {
                var model = new MobileNumListData();

                var MNList = TempData["MNList"];
                TempData["MNList"] = TempData["MNList"];

                List<MobileNumModel> MobileNumList = new List<MobileNumModel>();
                if(MNList != null){
                    MobileNumList = MNList as List<MobileNumModel>;
                }

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<MobileNumModel, string> orderingFunction = (c => sortColumnIndex == 1 ? c.MSISDN :
                                                                    sortColumnIndex == 2 ? c.Price.ToString() :
                                                                    sortColumnIndex == 3 ? c.BatchNum :
                                                                    c.MSISDN);

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        MobileNumList = MobileNumList.OrderBy(orderingFunction).ToList();
                    }
                    else
                    {
                        MobileNumList = MobileNumList.OrderByDescending(orderingFunction).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = MobileNumList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.MSISDN.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = MobileNumList.Count;
                model.iTotalRecords = MobileNumList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("MOBILENUM", "EDIT")]
        public ActionResult Edit(int ID)
        {
            MobileNumModel model = new MobileNumModel();

            var result = MobileNumService.Get(ID);
            model = MobileNumMapper.Map(result);

            return View(model);
        }

        [UserAuthorize("MOBILENUM", "EDIT")]
        [HttpPost]
        public ActionResult Edit(MobileNumModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vo = MobileNumMapper.ReMap(Model);
                    var result = MobileNumService.Edit(vo);

                    if (result == true)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("List", "MobileNum");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(Model);
        }

        [UserAuthorize("MOBILENUM", "VIEW")]
        public ActionResult BatchList()
        {
            return View();
        }

        [UserAuthorize("MOBILENUM", "VIEW")]
        public ActionResult ListBatch(DataTableParam param)
        {
            try
            {
                var model = new BatchListData();

                var BatchList = new List<BatchModel>();

                var MNList = MobileNumService.GetAll();
                var List = MNList.GroupBy(m => m.BatchNum).Select(g => new { BatchNum = g.Key, NumberCount = g.Count(), PriceMin = g.Min(p => p.Price), PriceMax = g.Max(p => p.Price) });
                foreach (var v in List)
                {
                    BatchList.Add(new BatchModel() {
                        BatchNum = v.BatchNum,
                        NumberCount = v.NumberCount,
                        PriceMax = v.PriceMax,
                        PriceMin = v.PriceMin
                    });
                }


                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<BatchModel, string> orderingFunction = (c => sortColumnIndex == 1 ? c.BatchNum :
                                                                    sortColumnIndex == 2 ? c.PriceMin.ToString() :
                                                                    sortColumnIndex == 3 ? c.PriceMax.ToString() :
                                                                    sortColumnIndex == 4 ? c.NumberCount.ToString() :
                                                                    c.BatchNum);

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        BatchList = BatchList.OrderBy(orderingFunction).ToList();
                    }
                    else
                    {
                        BatchList = BatchList.OrderByDescending(orderingFunction).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = BatchList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.BatchNum.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = BatchList.Count;
                model.iTotalRecords = BatchList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("MOBILENUM", "ADD")]
        public ActionResult Assign(string BatchNum)
        {
            BatchModel model = new BatchModel();
            model.BatchNum = BatchNum;
            model.Accounts = new List<AssignedAccount>();

            var result = MobileNumService.GetBatchAssignedUser(BatchNum);
            foreach (var v in result)
            {
                model.Accounts.Add(new AssignedAccount() {
                    AccountId = v.AccountId,
                    Assigned = true,
                    Name = v.PersonalInfo.FullName,
                    MSISDN = v.PersonalInfo.MSISDNNumber
                });
            }

            return View(model);
        }

        [UserAuthorize("MOBILENUM", "EDIT")]
        [HttpPost]
        public ActionResult Assign(BatchModel Model, FormCollection form)
        {
            try
            {
                var AssignList = form["AssignList"];

                MobileNumService.BatchAssignUser(Model.BatchNum, AssignList.Split(',').Select(long.Parse).ToList());

                TempData["Message"] = "Successfully done.";                
            }
            catch (Exception ex)
            {
                if (ex.Message == "Sequence contains no elements")
                    TempData["ErrMessage"] = "Dealer not found.";
                else
                    TempData["Message"] = ex.Message;
            }

            return RedirectToAction("Assign", "MobileNum", new { BatchNum = Model.BatchNum });
        }

        //[UserAuthorize("MOBILENUM", "DELETE")]
        //[HttpPost]
        //public ActionResult MultiAssign(BatchModel Model)
        //{
        //    try
        //    {
        //        foreach (var v in Model.Accounts)
        //        {
        //            if(v.Assigned == false)
        //                MobileNumService.BatchUnAssignUser(Model.BatchNum, v.AccountId);
        //        }

        //        TempData["Message"] = "Successfully done.";
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Message"] = ex.Message;
        //    }

        //    return RedirectToAction("Assign", "MobileNum", new { BatchNum = Model.BatchNum });
        //}

	}
}