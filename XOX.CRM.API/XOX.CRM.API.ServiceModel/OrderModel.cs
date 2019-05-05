using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.API.ServiceModel.Types;

namespace XOX.CRM.API.ServiceModel
{
    [Route("/account/order/create", "POST")]
    [Route("/account/{AccountId}/order/create", "POST")]
    public class OrderCreate : IReturn<LongResponse>
    {
        public long AccountId { get; set; }
        public string CustomerRepId { get; set; }
        public string Assignee { get; set; }
        public string OrderSource { get; set; }
        public string OrderType { get; set; }
        public string Plan { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public string SubmitDate { get; set; }
        public int OrderStatus { get; set; }
        public bool FlgReceivedItemised { get; set; }
        public string MSISDN { get; set; }
        public string SubmitBy { get; set; }
        public bool FlgForeigner { get; set; }
    }

    [Route("/order/files/add", "POST")]
    [Route("/order/{OrderId}/files/add", "POST")]
    public class OrderAddFiles : IReturn<BoolResponse>
    {
        public int OrderId { get; set; }
        public List<File> Files { get; set; }
    }

    [Route("/order/files/re-add", "POST")]
    public class OrderReAddFiles : IReturn<BoolResponse>
    {
        public int IntegrationId { get; set; }
        public List<File> Files { get; set; }
    }

    [Route("/order/supplementary/add", "POST")]
    public class OrderSupplementaryAdd : IReturn<BoolResponse>
    {
        public string MSISDN { get; set; }
        public long SuppAccountId { get; set; }
        public List<File> Files { get; set; }
        public long AccountId { get; set; }
        public string Donor { get; set; }
        public long IntegrationId { get; set; }
    }

    [Route("/order/resubmit", "POST")]
    public class OrderResubmit : IReturn<BoolResponse>
    {
        public AccountAdd AccountAdd { get; set; }
        public OrderCreate OrderCreate { get; set; }
        public OrderAddFiles OrderAddFiles { get; set; }
        public long OrderId { get; set; }
        public string SubmitBy { get; set; }
    }

    [Route("/order/activate/mnpack", "POST")]
    public class ActivateMNPOrder
    {
        public string portId { get; set; }
        public string portReqFormId { get; set; }
    }

    [Route("/order/activate/mnpstatus", "POST")]
    public class ActivateMNPOrderStatus
    {
        public string portId { get; set; }
        public string statusMsg { get; set; }
        public string rejectCode { get; set; }
    }

}
