using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Constants;

namespace CRM.Controllers
{
    public class SettingController : BaseController
    {
        private readonly IUserService UserService;
        private readonly ICommonService CommonService;

        public SettingController()
        {
            this.UserService = new UserService();
            this.CommonService = new CommonService();
        }

        [UserAuthorize("GROUP", "VIEW")]
        public ActionResult UserGrouping()
        {
            return View();
        }

        [UserAuthorize("GROUP", "VIEW")]
        public ActionResult ListUserGroup(DataTableParam param)
        {
            try
            {
                var model = new UserGroupListData();

                var list = UserService.UserGroupGetAll();

                List<UserGroupModel> UserGroupList = UserGroupMapper.Map(list);

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<UserGroupModel, string> orderingFunction = (c => sortColumnIndex == 1 ? c.GroupCode :
                                                                    sortColumnIndex == 2 ? c.Description :
                                                                    c.Id.ToString());

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        UserGroupList = UserGroupList.OrderBy(orderingFunction).ToList();
                    }
                    else
                    {
                        UserGroupList = UserGroupList.OrderByDescending(orderingFunction).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = UserGroupList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.GroupCode.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = UserGroupList.Count;
                model.iTotalRecords = UserGroupList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("GROUP", "ADD")]
        public ActionResult UserGroupAdd()
        {
            UserGroupModel model = new UserGroupModel();
            return View(model);
        }

        [UserAuthorize("GROUP", "ADD")]
        [HttpPost]
        public ActionResult UserGroupAdd(UserGroupModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vo = UserGroupMapper.ReMap(Model);

                    var result = UserService.UserGroupAdd(vo);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("UserGrouping", "Setting");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }
            
            return View(Model);
        }

        [UserAuthorize("GROUP", "EDIT")]
        public ActionResult UserGroupEdit(int ID)
        {
            UserGroupModel model = new UserGroupModel();

            var result = UserService.UserGroupGet(ID);

            model = UserGroupMapper.Map(result);

            return View(model);
        }

        [UserAuthorize("GROUP", "EDIT")]
        [HttpPost]
        public ActionResult UserGroupEdit(UserGroupModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vo = UserGroupMapper.ReMap(Model);

                    var result = UserService.UserGroupEdit(vo);

                    if (result == true)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("UserGrouping", "Setting");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(Model);
        }

        [UserAuthorize("GROUP", "VIEW")]
        public ActionResult UserGroupView(int ID)
        {
            UserGroupModel model = new UserGroupModel();

            var result = UserService.UserGroupGet(ID);

            model = UserGroupMapper.Map(result);

            return View(model);
        }

        [UserAuthorize("MODULE", "VIEW")]
        public ActionResult ModuleMaster()
        {
            return View();
        }

        [UserAuthorize("MODULE", "VIEW")]
        public ActionResult ListModule(DataTableParam param)
        {
            try
            {
                var model = new ModuleListData();

                var list = UserService.ModuleGetAll();

                List<ModuleModel> ModuleList = ModuleMapper.Map(list);

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<ModuleModel, string> orderingFunction = (c => sortColumnIndex == 1 ? c.ModuleCode :
                                                                    sortColumnIndex == 2 ? c.ModuleName :
                                                                    c.Id.ToString());

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        ModuleList = ModuleList.OrderBy(orderingFunction).ToList();
                    }
                    else
                    {
                        ModuleList = ModuleList.OrderByDescending(orderingFunction).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = ModuleList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.ModuleCode.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = ModuleList.Count;
                model.iTotalRecords = ModuleList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("MODULE", "ADD")]
        public ActionResult ModuleAdd()
        {
            ModuleModel model = new ModuleModel();
            return View(model);
        }

        [UserAuthorize("MODULE", "ADD")]
        [HttpPost]
        public ActionResult ModuleAdd(ModuleModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vo = ModuleMapper.ReMap(Model);

                    var result = UserService.ModuleAdd(vo);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("ModuleMaster", "Setting");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(Model);
        }

        [UserAuthorize("MODULE", "EDIT")]
        public ActionResult ModuleEdit(int ID)
        {
            ModuleModel model = new ModuleModel();

            var result = UserService.ModuleGet(ID);

            model = ModuleMapper.Map(result);

            return View(model);
        }

        [UserAuthorize("MODULE", "EDIT")]
        [HttpPost]
        public ActionResult ModuleEdit(ModuleModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vo = ModuleMapper.ReMap(Model);

                    var result = UserService.ModuleEdit(vo);

                    if (result == true)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("ModuleMaster", "Setting");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(Model);
        }

        [UserAuthorize("MODULE", "VIEW")]
        public ActionResult ModuleView(int ID)
        {
            ModuleModel model = new ModuleModel();

            var result = UserService.ModuleGet(ID);

            model = ModuleMapper.Map(result);

            return View(model);
        }

        [UserAuthorize("USERACCESS", "VIEW")]
        public ActionResult AccessControl()
        {
            GroupAccessVM model = new GroupAccessVM();

            var listU = UserService.UserGroupGetAll();
            model.UserGroups = UserGroupMapper.Map(listU);

            var listM = UserService.ModuleGetAll();
            model.Modules = ModuleMapper.Map(listM);

            return View(model);
        }

        [UserAuthorize("USERACCESS", "VIEW")]
        public JsonResult AjaxGetUserGroupAccess(int UserGroupId)
        {
            var List = GroupAccessMapper.Map(UserService.GroupAccessGet(UserGroupId));

            return Json(List, JsonRequestBehavior.AllowGet);
        }

        [UserAuthorize("USERACCESS", "EDIT")]
        [HttpPost]
        public ActionResult AccessControl(FormCollection form)
        {
            long UserGroupId = long.Parse(form["UserGroupId"]);

            var listM = UserService.ModuleGetAll();
            foreach (var v in listM)
            {
                long id = v.ModuleId;
                v.IsViewable = form[id + "-IsViewable"] == null ? false : true;
                v.IsAddable = form[id + "-IsAddable"] == null ? false : true;
                v.IsEditable = form[id + "-IsEditable"] == null ? false : true;
                v.IsDeleteable = form[id + "-IsDeleteable"] == null ? false : true;
                v.IsApprovable = form[id + "-IsApprovable"] == null ? false : true;
                v.IsRejectable = form[id + "-IsRejectable"] == null ? false : true;
            }

            try
            {
                var result = UserService.GroupAccessEdit(UserGroupId, listM);

                TempData["Message"] = "Successfully done.";
                return RedirectToAction("AccessControl", "Setting");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("AccessControl", "Setting");
            }
        }

        [UserAuthorize("USER", "VIEW")]
        public ActionResult UserMaster()
        {
            return View();
        }

        [UserAuthorize("USER", "VIEW")]
        public ActionResult ListUser(DataTableParam param, long UserGroupId = 0)
        {
            try
            {
                var model = new UserListData();

                var list = UserService.UserGetAll(UserGroupId);

                List<UserModel> UserList = UserMapper.Map(list);

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<UserModel, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Username :
                                                                    sortColumnIndex == 2 ? c.StaffName :
                                                                    c.Id.ToString());

                var sortDirection = Request["sSortDir_0"]; // asc or desc

                if (!string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "asc")
                    {
                        UserList = UserList.OrderBy(orderingFunction).ToList();
                    }
                    else
                    {
                        UserList = UserList.OrderByDescending(orderingFunction).ToList();
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = UserList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.Username.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = UserList.Count;
                model.iTotalRecords = UserList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("USER", "ADD")]
        public ActionResult UserAdd()
        {
            UserModel model = new UserModel();

            var list = UserService.UserGroupGetAll();
            ViewBag.UserGroups = UserGroupMapper.Map(list);
            return View(model);
        }

        [UserAuthorize("USER", "ADD")]
        [HttpPost]
        public ActionResult UserAdd(UserModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vo = UserMapper.ReMap(Model);

                    var result = UserService.UserAdd(vo);

                    if (result != 0)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("UserMaster", "Setting");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            var list = UserService.UserGroupGetAll();
            ViewBag.UserGroups = UserGroupMapper.Map(list);
            return View(Model);
        }

        [UserAuthorize("USER", "EDIT")]
        public ActionResult UserEdit(int ID)
        {
            UserModel model = new UserModel();

            var result = UserService.UserGet(ID);

            model = UserMapper.Map(result);

            var list = UserService.UserGroupGetAll();
            ViewBag.UserGroups = UserGroupMapper.Map(list);
            return View(model);
        }

        [UserAuthorize("USER", "EDIT")]
        [HttpPost]
        public ActionResult UserEdit(UserModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vo = UserMapper.ReMap(Model);

                    var result = UserService.UserEdit(vo);

                    if (result == true)
                    {
                        TempData["Message"] = "Successfully done.";
                        return RedirectToAction("UserMaster", "Setting");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            var list = UserService.UserGroupGetAll();
            ViewBag.UserGroups = UserGroupMapper.Map(list);
            return View(Model);
        }

        [UserAuthorize("USER", "VIEW")]
        public ActionResult UserView(int ID)
        {
            UserModel model = new UserModel();

            var result = UserService.UserGet(ID);

            model = UserMapper.Map(result);

            return View(model);
        }

        [UserAuthorize("LOOKUP", "VIEW")]
        public ActionResult LookupMaster()
        {
            return View();
        }

        [UserAuthorize("LOOKUP", "VIEW")]
        public ActionResult ListLookupType(DataTableParam param)
        {
            try
            {
                var model = new LookupListData();

                var list = CommonService.GetLookupTypes();

                var LookupList = new List<LookupModel>();
                foreach (var v in list)
                {
                    LookupList.Add(new LookupModel() { Type = v });
                }

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                if (sortColumnIndex != 0)
                {
                    Func<LookupModel, string> orderingFunction = (c => c.Type);

                    var sortDirection = Request["sSortDir_0"]; // asc or desc

                    if (!string.IsNullOrEmpty(sortDirection))
                    {
                        if (sortDirection == "asc")
                        {
                            LookupList = LookupList.OrderBy(orderingFunction).ToList();
                        }
                        else
                        {
                            LookupList = LookupList.OrderByDescending(orderingFunction).ToList();
                        }
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = LookupList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.Type.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = LookupList.Count;
                model.iTotalRecords = LookupList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("LOOKUP", "VIEW")]
        public ActionResult LookupView(string Type)
        {
            LookupModel model = new LookupModel();
            model.Type = Type;

            return View(model);
        }

        [UserAuthorize("LOOKUP", "EDIT")]
        public ActionResult LookupSequence(string Type)
        {
            var list = new LookupVO() { LookupKey = Type };
            CommonService.GetLookupValues(list);
            
            var LookupList = new List<LookupModel>();
            foreach (var v in list.KeyValues)
            {
                LookupList.Add(new LookupModel() { Name = v.Key, Value = v.Value });
            }

            LookupSequenceVM model = new LookupSequenceVM();
            model.Type = Type;
            model.List = LookupList;

            return View(model);
        }

        [UserAuthorize("LOOKUP", "EDIT")]
        [HttpPost]
        public ActionResult UpdateSequence(FormCollection form)
        {
            var Type = form["Type"];
            var NewOrder = form["NewOrder"];

            try
            {
                foreach (var v in NewOrder.Split(new string[] { "|#*|" }, StringSplitOptions.None))
                {
                    var KeyValue = v.Split(new string[] { "$#*$" }, StringSplitOptions.None);

                    CommonService.UpdateSequence(Type, KeyValue[1], int.Parse(KeyValue[0]));
                }

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("LookupView", "Setting", new { Type = Type });
        }

        [UserAuthorize("LOOKUP", "VIEW")]
        public ActionResult ListLookup(DataTableParam param, string Type)
        {
            try
            {
                var model = new LookupListData();

                var list = new LookupVO() { LookupKey = Type };
                CommonService.GetLookupValues(list);

                var LookupList = new List<LookupModel>();
                foreach (var v in list.KeyValues)
                {
                    LookupList.Add(new LookupModel() { Name = v.Key, Value = v.Value });
                }

                //sorting properties : need to pass respective column to sort in query 
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                if (sortColumnIndex != 0)
                {
                    Func<LookupModel, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name :
                                                                        sortColumnIndex == 2 ? c.Value :
                                                                        c.Name);

                    var sortDirection = Request["sSortDir_0"]; // asc or desc

                    if (!string.IsNullOrEmpty(sortDirection))
                    {
                        if (sortDirection == "asc")
                        {
                            LookupList = LookupList.OrderBy(orderingFunction).ToList();
                        }
                        else
                        {
                            LookupList = LookupList.OrderByDescending(orderingFunction).ToList();
                        }
                    }
                }

                //filtering : need to pass skip/take and search string to query 
                if (string.IsNullOrEmpty(param.sSearch)) { param.sSearch = ""; }
                var filteredlist = LookupList.Skip(param.iDisplayStart).Take(param.iDisplayLength).
                                            Where(m => m.Name.ToLower().Contains(param.sSearch.ToLower())).ToList();


                model.aaData = filteredlist;
                model.iTotalDisplayRecords = LookupList.Count;
                model.iTotalRecords = LookupList.Count;
                model.sEcho = param.sEcho;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [UserAuthorize("LOOKUP", "ADD")]
        public ActionResult LookupAdd(string Type)
        {
            LookupModel model = new LookupModel();
            model.Type = Type;

            return View(model);
        }

        [UserAuthorize("LOOKUP", "ADD")]
        [HttpPost]
        public ActionResult LookupAdd(LookupModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    LookupVO Lookup = new LookupVO();
                    Lookup.CreatedBy = long.Parse(HttpContext.User.Identity.Name);
                    Lookup.LookupKey = Model.Type;
                    Lookup.KeyValues = new List<KeyValuePair<string, string>>();
                    Lookup.KeyValues.Add(new KeyValuePair<string, string>(Model.Name, Model.Value));
                    CommonService.AddLookupValues(Lookup);

                    TempData["Message"] = "Successfully done.";
                    return RedirectToAction("LookupView", "Setting", new { Type = Model.Type });                    
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(Model);
        }

        [UserAuthorize("LOOKUP", "EDIT")]
        public ActionResult LookupEdit(string Type, string Name, string Value)
        {
            LookupModel model = new LookupModel();
            model.Type = Type;
            model.Name = Name;
            model.Value = Value;

            return View(model);
        }

        [UserAuthorize("LOOKUP", "EDIT")]
        [HttpPost]
        public ActionResult LookupEdit(LookupModel Model, FormCollection form)
        {
            var curLookupKey = form["curLookupKey"];
            var curLookupValue = form["curLookupValue"];

            try
            {
                LookupVO Lookup = new LookupVO();
                Lookup.CreatedBy = long.Parse(HttpContext.User.Identity.Name);
                Lookup.LookupKey = Model.Type;
                Lookup.NewKeyValues = new List<KeyValuePair<string, string>>();
                Lookup.NewKeyValues.Add(new KeyValuePair<string, string>(Model.Name, Model.Value));
                Lookup.KeyValues = new List<KeyValuePair<string, string>>();
                Lookup.KeyValues.Add(new KeyValuePair<string, string>(curLookupKey, curLookupValue));
                CommonService.EditLookupValues(Lookup);

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction("LookupView", "Setting", new { Type = Model.Type });
        }

	}
}