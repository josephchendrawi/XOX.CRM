using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace XOX.CRM.Lib.WebServices
{
    public class EAIService
    {
        public static string RejectOrder(OrderReject request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "order/reject" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            ResultObj R = (ResultObj)new JavaScriptSerializer().Deserialize(result, typeof(ResultObj));
            return R.Result;
        }
        public static string EditAccount(AccountEdit request)
        {
            request.Data.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "account/edit" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            ResultObj R = (ResultObj)new JavaScriptSerializer().Deserialize(result, typeof(ResultObj));
            return R.Result;
        }
        public static string AccountUpdateStatus(AccountUpdateStatus request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "account/update-status" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            ResultObj R = (ResultObj)new JavaScriptSerializer().Deserialize(result, typeof(ResultObj));
            return R.Result;
        }
        public static string OrderUpdateStatus(OrderUpdateStatus request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "order/update-status" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            ResultObj R = (ResultObj)new JavaScriptSerializer().Deserialize(result, typeof(ResultObj));
            return R.Result;
        }
        public static string AssignMSISDN(AssignMSISDN request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "account/assign-msisdn" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            ResultObj R = (ResultObj)new JavaScriptSerializer().Deserialize(result, typeof(ResultObj));
            return R.Result;
        }
        public static string CRPAddPayment(CRPAddPayment request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "account/add-payment" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            ResultObj R = (ResultObj)new JavaScriptSerializer().Deserialize(result, typeof(ResultObj));
            return R.Result;
        }

        //////////

        public static string CreatePayment(Payment request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "payment/add" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string GetPayment(GetPayment request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "payment/list" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string GetSubscriberProfile(GetSubscriberProfile request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "subscriber-profile" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string AddSubscriberProfile(AddSubscriberProfile request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "subscriber-profile/add" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }
        public static string AddSubscriberProfileMNP(AddSubscriberProfileMNP request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "subscriber-profile/add/mnp" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }
        public static string AddSubscriberProfileMNPSubs(AddSubscriberProfileMNPSubs request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "subscriber-profile/add/mnp-subs" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string AddSubscriberProfileCOBP(AddSubscriberProfile request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "subscriber-profile/add/cobp" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string CreditLimitUpdate(CreditLimitUpdate request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "credit-limit/update" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string SimNumberUpdate(SimNumberUpdate request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "sim-number/update" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string PlanUpdateRequest(PlanUpdateRequest request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "plan/update_request" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string PlanUpdate(PlanUpdate request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "plan/update" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string CheckOneXDealer(CheckOneXDealer request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "subscriber-profile/checkonex" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string AddCUG(AddCUG request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "cug/add" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string RemoveCUG(RemoveCUG request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "cug/remove" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string GetCUG(GetCUG request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "cug/get" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string SendSMS(SendSMS request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "send-sms" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string SendSMSWithShortCode(SendSMSWithShortCode request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "send-sms/shortcode" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string UpdateSubscriberProfile(UpdateSubscriberProfile request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "subscriber-profile/update" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string ItemisedBillingUpdate(ItemisedBillingUpdate request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "itemised-billing/update" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

        public static string DepositUpdate(DepositUpdate request)
        {
            request.User.Source = "CRM";
            string EAI_HOST = ConfigurationManager.AppSettings["EAI_HOST"];
            string URLAuth = EAI_HOST + "deposit/update" + "?format=json";
            string JSONParameter = new JavaScriptSerializer().Serialize(request);

            WebClientEx webClient = new WebClientEx();
            webClient.Headers["Content-Type"] = "application/json; charset=utf-8";
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.Timeout = 900000;

            string result = webClient.UploadString(URLAuth, "POST", JSONParameter);
            webClient.Dispose();

            return result;
        }

    }
    
    public class WebClientEx : WebClient
    {
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            request.Timeout = Timeout;
            return request;
        }
    }

    #region TestingPurpose
    //public class EAIService
    //{
    //    public static string RejectOrder(OrderReject request)
    //    {
    //        return "True";
    //    }
    //    public static string EditAccount(AccountEdit request)
    //    {
    //        return "True";
    //    }
    //    public static string AccountUpdateStatus(AccountUpdateStatus request)
    //    {
    //        return "True";
    //    }
    //    public static string OrderUpdateStatus(OrderUpdateStatus request)
    //    {
    //        return "True";
    //    }

    //    //////////

    //    public static string CreatePayment(Payment request)
    //    {
    //        var result = "{\"Result\":{\"status\":\"true\"}}";
    //        return result;
    //    }

    //    public static string GetPayment(GetPayment request)
    //    {
    //        var result = "{\"Result\":{\"result\":true,\"message\":\"\"}}";
    //        return result;
    //    }

    //    public static string GetSubscriberProfile(GetSubscriberProfile request)
    //    {
    //        var result = "{\"Result\":{\"customer\":{\"offerCode\":\"Consumer Postpaid Offer\",\"effective\":\"2015-12-28T09:46:27Z\"}}}";

    //        return result;
    //    }

    //    public static string AddSubscriberProfile(AddSubscriberProfile request)
    //    {
    //        var result = "{\"Result\":{\"result\":true,\"message\":\"\"}}";

    //        return result;
    //    }
    //    public static string AddSubscriberProfileMNP(AddSubscriberProfileMNP request)
    //    {
    //        var result = "{\"Result\":\"success\"}";

    //        return result;
    //    }

    //    public static string AddSubscriberProfileCOBP(AddSubscriberProfile request)
    //    {
    //        var result = "{\"Result\":{\"result\":true,\"message\":\"61.2\"}}";

    //        return result;
    //    }

    //    public static string CreditLimitUpdate(CreditLimitUpdate request)
    //    {
    //        var result = "{\"Result\":{\"result\":true,\"message\":\"\"}}";

    //        return result;
    //    }

    //}
    #endregion
}
