using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface IAccountService
    {
        List<AccountVO> GetAll();
        List<AccountVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", string filterBy = "", string filterQuery = "");
        List<AccountVO> GetAllBySearch(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", string Salutation = "", string FullName = "", string Email = "", string IdentityType = "", string IdentityNo = "", string MSISDN = "", string Status = "", string AccountType = "1");
        AccountVO Get(long id);
        bool Edit(AccountVO vo, long UserId = 0, bool flg2EAI = true);
        long Add(AccountVO vo, long UserId = 0, long IntegrationId = 0, bool? Supplementary = false);
        long BillAccAdd(AccountVO vo, long ParAccountId, long UserId = 0);
        bool UpdateStatus(AccountActivityVO Activity, string Status, string SubscriberJoinDate = "", DateTime? DeleteDate = null);
        
        long AddActivity(AccountActivityVO VO);
        List<AccountActivityVO> GetAccountActivities(long accntId);

        long GetIDByMSISDN(string MSISDN, string AccountName);
        List<AccountVO> GetAllSupplementaryLine(long AccountId);
        long GetParentAccountId(long SupplineAccountId);

        long GetAccountIdByOrderId(long OrderId);

        string MakePayment(long AccountId, Payment payment, bool Manual = false, long UserId = 0);
        void CreatePayment(long AccountId, Payment payment, long UserId = 0);
        List<PaymentVO> GetAllPayment(long AccountId);
        List<PaymentResponseList> GetPayment(long AccountId, long UserId = 0);
        GetSubscriberProfileResponse GetSubscriberProfile(long AccountId, long UserId = 0);
        long GetAccountIdByIntegrationId(long IntegrationId);
        string AddSubcriberProfile(long AccountId, long OrderId, int UserId = 0);
        string migrateToPostpaid(long AccountId, long OrderId, int UserId = 0);

        long CheckIntegrationId(long IntegrationId, string AccountTypeCode = "1");
        bool SavePaymentRecord(PaymentRecordVO PaymentRecord, long UserId = 0);
        bool CheckPaymentSufficient(long AccountId, long OrderId);

        void ClearPaymentRecord(long OrderId);

        void AddCUG(long AccountId, string CUGNo, long UserId = 0);
        void RemoveCUG(long AccountId, string CUGNo, long UserId = 0);
        List<string> GetCUG(long AccountId, long UserId = 0);
        List<PaymentVO> GetAllPaymentBySearch(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", PaymentVO Parameter = null);

        void EditPrintedBillingFlg(long AccountId, bool PrintedBillingFlg = false);
        void ChangeAdvancePaymentFlagResponse(long AccountId, bool Response);
        bool? GetAdvancePaymentFlagResponse(long AccountId);
        void MakeAdvancePayment(long AccountId);

        List<AccountPaymentCardDetail> GetAllPaymentCardDetails(string CardType = "", int WithinMonths = 0);

        int GetTerminatedAccountCount(DateTime? From = null, DateTime? To = null);
    }
}
