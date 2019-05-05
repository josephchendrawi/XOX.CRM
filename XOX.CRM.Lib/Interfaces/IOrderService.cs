using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib;

namespace XOX.CRM.Lib
{
    public interface IOrderService
    {
        long CreateNewOrder(OrderVO _newOrder, List<long> products, string MSISDN, string SubmitBy, bool flgResubmitted = false, bool flgTermination = false);
        OrderDetailVO Get(long OrderId);
        List<OrderDetailVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy, string orderDirection, string filterBy, string filterQuery, string UserName = "");
        List<OrderDocumentVO> GetAllAttachments(long OrderId);
        bool UpdateStatus(OrderActivityVO Activity, string Status, string offerFrom = "");
        bool AddDocuments(List<String> filespath, long OrderId, long UserId = 0);
        bool CheckAndAddDocument(List<String> filespath, long OrderId, long UserId = 0);
        long UpdateOrderSupplementary(string MSISDN, long SuppAccountId);
        long GetAllCount(string filterBy = "", string filterQuery = "", string UserName = "");
        bool ResubmitOrder(long OrderId, OrderVO OrderVO, long UserId);
        List<OrderDetailVO> GetAllOrderId(string UserName = "", int Length = -1, string FilterStatus = "");
        bool ResetDocuments(long OrderId);
        List<OrderDetailVO> GetAllBySearch(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", string MSISDN = "", string FullName = "", string From = "", string To = "", string Category = "", string Status = "", string OrderNum = "", string OrderType = "", long AccountId = 0);
        long GetOrderIdByAccountId(long AccountId, string OrderType = "New Registration");
        bool AddFiles(List<String> filespath, long OrderId, long UserId = 0);
        bool Edit(long OrderId, OrderDetailVO NewOrder, long UserId = 0);
        bool ActivateOrder(OrderActivityVO OrderActivity, int UserId = 0, string offerFrom = "Postpaid Offer");
        bool ActivateOrderRequest(OrderActivityVO OrderActivity);
        string ActivateMNPOrder(string PortReqFormId, string portId);
        string ActivateMNPOrderSuppLine(string PortReqFormId, string portId);
        string ActivateMNPOrderStatus(string portId, string statusMsg, string rejectCode);
        string ActivateMNPSuppLineOrderStatus(string portId, string statusMsg, string rejectCode);
        string FailActivateMNPOrder(string PortReqFormId, string MSISDN, string Remarks);
        List<OrderVO> GetRejectedOrder(long ResubmittedOrderId);
        long ChangePlanRequest(long AccountId, string NewPlan, DateTime RequestDate, long UserId = 0);
        void ChangePlan(long OrderId, long UserId = 0);
        OrderVO GetOrderVO(long OrderId);
        string ActivateMNPOrderRequest(string PortReqFormId, string portId);
        string ActivateMNPOrderStatusRequest(string portId, string statusMsg, string rejectCode);

        bool GetPrintedBillingByOrder(long OrderId);
    }

    public interface IOrderActivityService
    {
        long AddActivity(OrderActivityVO _newOrderAct);
        List<OrderActivityVO> GetActivities(long orderId);
    }
}
