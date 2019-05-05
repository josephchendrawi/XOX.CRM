using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class ServiceRequestVO
    {
        public long ServiceRequestId { get; set; }
        public string Category { get; set; }
        public string Assignee { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
        public int Resolution { get; set; }
        public string Description { get; set; }
        public string MSISDN { get; set; }
        public decimal? OldLimit { get; set; }
        public decimal? NewLimit { get; set; }
        public string SimMSISDN { get; set; }
        public string OldSIMNumber { get; set; }
        public string NewSIMNumber { get; set; }
        public long? AccountId { get; set; }
        public string ServiceRequestNumber { get; set; }
        public SubscriberProfile SubscriberProfile { get; set; }
        public string OldProfile { get; set; }
        public string NewProfie { get; set; }
        public DateTime? Created { get; set; }
        public string AccountName { get; set; }
        public bool NewItemisedBilling { get; set; }
        public decimal? NewDeposit { get; set; }
    }

    public class ServiceRequestAttachmentVO
    {
        public long AttachmentId { get; set; }
        public string Path { get; set; }
    }

    public class ServiceRequestNoteVO
    {
        public long NoteId { get; set; }
        public string Note { get; set; }
    }

    public class ServiceRequestActivityVO
    {
        public long ActivityId { get; set; }
        public DateTime VisitDateTime { get; set; }
        public string FieldStaff { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }

}