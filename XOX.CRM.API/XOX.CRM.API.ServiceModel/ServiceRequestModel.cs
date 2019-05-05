using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.API.ServiceModel.Types;

namespace XOX.CRM.API.ServiceModel
{
    [Route("/service_request/add")]
    public class ServiceRequestAdd : IReturn<ServiceRequestAddResponse>
    {
        public long IntegrationId { get; set; }
        [ApiAllowableValues("Category", typeof(Category))]
        public string Category { get; set; }
        [ApiAllowableValues("Priority", "1 = Low", "2 = Medium", "3 = High")]
        public int Priority { get; set; }
        public string Description { get; set; }
    }

    public class ServiceRequestAddResponse
    {
        public long Result { get; set; }
    }

    public enum Category
    {
        UpdateProfile,
        UpdateCreditLimit,
        SimReplacement,
        ManageItemisedBilling,
        UpdateDeposit,
    }
    
}
