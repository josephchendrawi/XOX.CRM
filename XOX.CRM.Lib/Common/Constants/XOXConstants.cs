using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib.Common.Constants
{
    public class XOXConstants
    {
        public static string DRIVE_D_ATT_PATH = @"D:\CRM\Attachments\";

        public static string Active = "1";
        public static string Locked = "2";
        public static string FirstTimeAPIKey = "MXwxMjMxMjN8";
        public static string XOX_BILL_COMPANY_NAME = "XOX MOBILE SDN BHD";

        public static string PRODUCT_LEVEL_MOLI = "MOLI";
        public static string PRODUCT_LEVEL_OLI = "OLI";

        public static string STATUS_ACTIVE_CD = "Active";
        public static string STATUS_TERMINATE_CD = "Terminated";
        public static string STATUS_PROSPECT_CD = "Prospect";
        public static string STATUS_INACTIVE_CD = "Inactive";
        public static string STATUS_WITHDRAW_CD = "Withdraw";
        public static string STATUS_COMPLETE_CD = "Completed";
        public static string STATUS_INCOMPLETE_CD = "Incomplete";

        public static string ACTION_TYPE_ADD = "ADD";
        public static string ACTION_TYPE_DELETE = "DELETE";

        public static int CREDIT_LIMIT = 2;
        public static int FOREIGNER_DEPOSIT = 500;
        public static decimal GST = 1.06m;

        public const string SYSTEM_USER = "abc@abc.com";
        public const string SYSTEM_NAME = "CRM";

        public static string STATUS_MSG_ACTIVE = "NP Activated";
        public static string STATUS_MSG_REJECT = "NPR Reject";

        public static string PAYMENT_TYPE_BILLING = "Billing";
        public static string PAYMENT_TYPE_DEPOSIT = "Deposit";
        public static string PAYMENT_TYPE_ADVANCE_PAYMENT = "Advance Payment";
        public static string PAYMENT_TYPE_FOREIGN_DEPOSIT = "Foreign Deposit";
        public static string PAYMENT_TYPE_REDEMPTION = "Redemption";
        public static string PAYMENT_TYPE_SETTLEMENT = "Settlement";

        public static string PAYMENT_METHOD_XPOINT = "XPoint";
        public static string PAYMENT_METHOD_CASH = "Cash";
        public static string PAYMENT_METHOD_CREDIT_CARD = "Credit Card";

        public const string AUDIT_ACTION_UPDATE = "Update";
        public const string AUDIT_MODULE_ACCOUNT = "XOX_T_ACCNT";
        public const string AUDIT_MODULE_ADDRESS = "XOX_T_ADDR";
        public const string AUDIT_LOG_EVENT_TYPE_LOGIN = "Login";

        public static string OFFER_CODE_PREPAID = "XoX Prepaid Offer";
        public static string OFFER_CODE_POSTPAID = "Consumer Postpaid Offer";
        public static string OFFER_CODE_LIFESTYLE = "Lifestyle Offer";
        public static string OFFER_CODE_HYBRID = "XoX Hybrid Offer";

        public const string ORDER_TYPE_NEW = "New Registration";
        public const string ORDER_TYPE_TERMINATE = "Termination";
        public const string ORDER_TYPE_SUPPLEMENTARY = "Supplementary Registration";
        public const string ORDER_TYPE_CHANGE_PLAN = "Change Plan";
        public const string ORDER_TYPE_UPGRADE_PLAN = "Upgrade Plan";
        public const string ORDER_TYPE_DOWNGRADE_PLAN = "Downgrade Plan";

        public const string ORDER_STATUS_NEW = "New";
        public const string ORDER_STATUS_REJECTED = "Rejected";
        public const string ORDER_STATUS_RESUBMITTED = "Resubmitted";
        public const string ORDER_STATUS_TERMINATED = "Terminated";

        public const string PRODUCT_PRINTED_BILLING = "Printed Billing";

        public const string ORDER_SOURCE_CRP = "CRP";
    }
}
