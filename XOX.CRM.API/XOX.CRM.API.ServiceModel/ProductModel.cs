using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.API.ServiceModel.Types;

namespace XOX.CRM.API.ServiceModel
{
    [Route("/product/get/all")]
    public class ProductGetAll : IReturn<ProductListResponse>
    {
    }
    [Route("/plan/get/all")]
    public class PlanGetAll : IReturn<PlanListResponse>
    {
    }

    public class ProductListResponse : Response
    {
        public List<Product> Result { get; set; }
    }

    public class PlanListResponse : Response
    {
        public List<Plan> Result { get; set; }
    }

}
