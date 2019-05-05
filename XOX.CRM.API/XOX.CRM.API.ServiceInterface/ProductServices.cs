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
    public class ProductServices : Service
    {
        private readonly IProductService ProductService;

        public ProductServices(IProductService ProductService)
        {
            this.ProductService = ProductService;
        }

        public object Any(ProductGetAll request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            var products = ProductService.GetAllProducts();

            var result = Mapper.Product.Map(products);

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new ProductListResponse { Result = result, Key = APIKey };
        }

        public object Any(PlanGetAll request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            var PrincipalPlans = ProductService.GetAllPrincipalPlan();

            var result = new List<Plan>();

            foreach (var Plan in PrincipalPlans)
            {
                result.Add(new Plan()
                {
                    AutoDebit = "Yes",
                    ContractPeriod = 24,
                    CreditLimit = ProductService.GetCreditLimit(Plan),
                    DataPack = ProductService.GetDataPack(Plan),
                    Deposit = ProductService.GetDeposit(Plan),
                    InitFnFData = ProductService.GetinitFnFData(Plan),
                    InitFnFOnNetCalls = ProductService.GetinitFnFOnNetCalls(Plan),
                    InitFnFOffNetCalls = ProductService.GetinitFnFOffNetCalls(Plan),
                    InitFnFOnNetSms = ProductService.GetinitFnFOnNetSms(Plan),
                    InitFnFOffNetSms = ProductService.GetinitFnFOffNetSms(Plan),
                    InitFreeData = ProductService.GetinitFreeData(Plan),
                    InitFreeOnNetCalls = ProductService.GetinitFreeOnNetCalls(Plan),
                    InitFreeOffNetCalls = ProductService.GetinitFreeOffNetCalls(Plan),
                    InitFreeOnNetSms = ProductService.GetinitFreeOnNetSms(Plan),
                    InitFreeOffNetSms = ProductService.GetinitFreeOffNetSms(Plan),
                    Prime = ProductService.GetPrime(Plan),
                    planInfo = Plan,
                    PrintedBill = ProductService.GetPrintedBilling(Plan) == true ? "Yes" : "No",
                    SignUpChannel = "CRP",
                });
            }

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new PlanListResponse { Result = result, Key = APIKey };
        }

    }
}
