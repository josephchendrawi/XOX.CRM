using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface IServiceRequestService
    {
        List<ServiceRequestVO> GetAllServiceRequests(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", ServiceRequestVO qFilter = null, long AccountId = 0);
        ServiceRequestVO GetServiceRequest(long ServiceRequestId);
        List<ServiceRequestAttachmentVO> GetAllServiceRequestAttachments(long ServiceRequestId);
        List<ServiceRequestNoteVO> GetAllServiceRequestNote(long ServiceRequestId);
        List<ServiceRequestActivityVO> GetAllServiceRequestActivity(long ServiceRequestId);
        long Add(ServiceRequestVO vo, long AccountId);
        void AddAttachment(String Path, long ServiceRequestId);
        void AddNote(String Note, long ServiceRequestId);
        void AddActivity(ServiceRequestActivityVO Act, long ServiceRequestId);
        bool Edit(ServiceRequestVO vo);
        bool RemoveAttachment(long AttachmentId);
        bool RemoveNote(long NoteId);
        void SubmitCreditLimitRequest(ServiceRequestVO vo, long AccountId);
        void SubmitSIMNumberRequest(ServiceRequestVO vo, long AccountId);
        void SubmitUpdateSubcriberProfile(ServiceRequestVO serviceRequestVO, long AccountId, int UserId = 0);
        void CancelRequest(ServiceRequestVO vo);
        void SubmitManageItemisedBillingRequest(ServiceRequestVO vo, long AccountId);
        void SubmitUpdateDeposit(ServiceRequestVO vo, long AccountId);
    }
}
