using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.Interfaces;
using XOX.CRM.Lib.Services;

namespace CRM.Controllers
{
    public class BaseController : Controller
    {
        protected override void ExecuteCore()
        {
            if (Request.Path.ToLower().Contains("/user/login"))
            {
                if (Request.IsAuthenticated && Session["UserEmail"] != null)
                {
                    View("Index").ExecuteResult(ControllerContext);
                }
                else
                {
                    base.ExecuteCore();
                }
            }
            else
            {
                if (Request.IsAuthenticated && UserService.IsAuthenticated() && Session["UserEmail"] != null)
                {
                    //check last login has same ip address with current ip
                    IAuditService AuditService = new AuditService(long.Parse(Thread.CurrentPrincipal.Identity.Name), "", "");
                    var LastRecordKey = AuditService.GetLatestAuditLog(XOXConstants.AUDIT_LOG_EVENT_TYPE_LOGIN, long.Parse(Thread.CurrentPrincipal.Identity.Name)).RECORD_KEY;
                    if (Session["RecordKey"] != null && (long)Session["RecordKey"] != LastRecordKey)
                    {
                        FormsAuthentication.SignOut();
                        Session.Abandon();
                        View("Unauthenticated").ExecuteResult(ControllerContext);
                    }
                    else
                    {
                        base.ExecuteCore();
                    }
                }
                else
                {
                    View("Unauthenticated").ExecuteResult(ControllerContext);
                }
            }
        }

        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }

        public class UserAuthorizeAttribute : AuthorizeAttribute
        {
            private readonly string Module;
            private readonly string Access;

            public UserAuthorizeAttribute(params string[] parameter)
            {
                this.Module = parameter[0];
                this.Access = parameter[1];
            }
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                bool authorize = UserService.IsAuthorized(Module, Access);

                return authorize;
            }
            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                filterContext.Result = new ViewResult { ViewName = "Unauthorized" };
            }
        }  

    }
}