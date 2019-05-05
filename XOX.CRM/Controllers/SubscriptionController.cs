using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Controllers
{
    public class SubscriptionController : BaseController
    {
        public ActionResult CreditLimit()
        {
            return View();
        }

        public ActionResult Payment()
        {
            return View();
        }

	}
}