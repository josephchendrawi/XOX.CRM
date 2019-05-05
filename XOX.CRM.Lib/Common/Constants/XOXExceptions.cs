using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib.Common.Constants
{
    public class XOXExceptions
    {
        // Unable to find MSISDN
        public const string ACCNT01_MSISDN_NOT_FOUND = "MSISDN not found.";

        // Duplicate account (by Name and MSISDN)
        public const string ACCNT02_DUPLICATE_ACCOUNTS = "More than one account found.";

        public const string ORDER_01_FAILED_CHANGE_PLAN = "Change plan failed.";
    }
}
