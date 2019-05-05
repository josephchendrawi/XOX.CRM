using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib.Common.Enum
{
    public enum OrderStatus
    {
        [Description("New")]
        New = 1,
        [Description("Pending Verification")]
        PendingVerification = 2,
        [Description("Verifying - Call 1")]
        VerifyingCall1 = 3,
        [Description("Verifying - Call 2")]
        VerifyingCall2 = 4,
        [Description("Verifying - Call 3")]
        VerifyingCall3 = 5,
        [Description("Verified")]
        Verified = 6,
        [Description("Pending Approval")]
        PendingApproval = 7,
        [Description("Pending Activation")]
        PendingActivation = 8,
        [Description("Active")]
        Active = 9,
        [Description("Rejected")]
        Rejected = 10
    }

}
