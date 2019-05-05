using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.Common.VO.Audit;
using XOX.CRM.Lib.Services;

namespace CRM.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService UserService;

        public UserController()
        {
            this.UserService = new UserService();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string usergroup = "";
                    long result = UserService.UserLogin(model.Email, model.Password, ref usergroup);
                    if (result != 0)
                    {
                        //generate RecordKey
                        Random rnd = new Random();
                        long RecordKey = rnd.Next(1, int.MaxValue);

                        //record into audit_log
                        var AuditService = new AuditService(result, "", "");
                        AuditService.AddAuditLog(new AuditLogVO()
                        {
                            EVENT_TYPE = XOXConstants.AUDIT_LOG_EVENT_TYPE_LOGIN,
                            LOG_DETAILS = Request.UserHostAddress,
                            RECORD_KEY = RecordKey
                        });

                        Session["UserEmail"] = model.Email;
                        Session["UserID"] = result;
                        Session["UserGroup"] = usergroup;
                        Session["RecordKey"] = RecordKey;

                        FormsAuthentication.SetAuthCookie(result.ToString(), model.RememberMe);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        TempData["Message"] = "Invalid Email or Password.";
                }
                catch (Exception ex)
                {
                    TempData["Message"] = ex.Message;
                }
            }

            return View(model);
        }
        
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "User");
        }

        [UserAuthorize("USER", "EDIT")]
        public ActionResult Unlock(long Id)
        {
            try
            {
                UserService.UserUnlock(Id);

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("UserMaster", "Setting");
        }

        [UserAuthorize("USER", "EDIT")]
        public ActionResult Lock(long Id)
        {
            try
            {
                UserService.UserLock(Id);

                TempData["Message"] = "Successfully done.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("UserMaster", "Setting");
        }

	}
}