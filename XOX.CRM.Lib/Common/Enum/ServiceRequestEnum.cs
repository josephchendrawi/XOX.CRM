using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM
{
    public enum ServiceRequestStatus
    {
        [Description("Open")]
        Open = 1,
        [Description("Escalated")]
        Escalated = 2,
        [Description("Closed")]
        Closed = 3,
        [Description("Cancelled")]
        Cancelled = 4,
    }
    public enum ServiceRequestResolution
    {
        [Description("Open")]
        Open = 1,
        [Description("Closed")]
        Closed = 2,
    }
}
