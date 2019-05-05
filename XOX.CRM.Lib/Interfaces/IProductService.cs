using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface IProductService
    {
        List<ProductVO> GetAllProducts();
        List<string> GetAllPrincipalPlan();
        List<ProductVO> GetProductItemByPlan(string Plan);
        decimal GetDeposit(long OrderId);
        decimal GetDepositByAccountId(long AccountId);
        decimal GetAdvancePayment(long OrderId);
        decimal GetAdvancePaymentByAccountId(long AccountId);

        decimal GetRequiredAdvancePayment(long OrderId);
        decimal GetRequiredDeposit(long OrderId);

        decimal GetPrime(string Plan);
        decimal GetCreditLimit(string Plan);

        decimal GetinitFreeOnNetCalls(string Plan);
        decimal GetinitFreeOffNetCalls(string Plan);
        decimal GetinitFreeOnNetSms(string Plan);
        decimal GetinitFreeOffNetSms(string Plan);
        decimal GetinitFreeData(string Plan);

        decimal GetinitFnFData(string Plan);
        decimal GetinitFnFOnNetCalls(string Plan);
        decimal GetinitFnFOffNetCalls(string Plan);
        decimal GetinitFnFOnNetSms(string Plan);
        decimal GetinitFnFOffNetSms(string Plan);

        string GetDataPack(string Plan);

        bool GetPrintedBilling(string Plan);
        decimal GetDeposit(string Plan);
    }
}
