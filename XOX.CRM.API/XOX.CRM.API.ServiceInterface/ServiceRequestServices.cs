using CRM;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.API.ServiceModel;
using XOX.CRM.API.ServiceModel.Types;
using XOX.CRM.Lib;

namespace XOX.CRM.API.ServiceInterface
{
    public class ServiceRequestServices : Service
    {
        private readonly IServiceRequestService ServiceRequestService = new ServiceRequestService();

        public object Any(ServiceRequestAdd request)
        {
            var AccountId = new AccountService().GetAccountIdByIntegrationId(request.IntegrationId);

            if (AccountId != 0)
            {
                var result = ServiceRequestService.Add(new ServiceRequestVO()
                {
                    Assignee = "CRP",
                    Category = request.Category,
                    Description = request.Description,
                    Priority = (int)request.Priority,
                    Status = (int)ServiceRequestStatus.Open,
                    DueDate = DateTime.Now,
                    Resolution = (int)ServiceRequestResolution.Open,
                }, AccountId);

                return new ServiceRequestAddResponse { Result = result };
            }
            else
            {
                throw new Exception("Integration ID not found");
            }
        }

    }
}
