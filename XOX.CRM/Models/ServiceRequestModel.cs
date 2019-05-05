using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class ServiceRequestModel
    {
        public long ServiceRequestId { get; set; }
        public long AccountId { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Assignee { get; set; }
        public int Priority { get; set; }
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
        public int Resolution { get; set; }
        public string Description { get; set; }

        [Display(Name = "MSISDN")]
        public string MSISDN { get; set; }
        [Display(Name = "Old Limit")]
        public decimal? OldLimit { get; set; }
        [Display(Name = "New Limit")]
        [Required]
        public decimal? NewLimit { get; set; }
        [Display(Name = "MSISDN")]
        public string SimMSISDN { get; set; }
        [Display(Name = "Old SIM Number")]
        public string OldSIMNumber { get; set; }
        [Display(Name = "New SIM Number")]
        public string NewSIMNumber { get; set; }

        public List<ServiceRequestAttachment> Attachments { get; set; }
        public List<ServiceRequestNote> Notes { get; set; }

        [Display(Name = "Service Request Number")]
        public string ServiceRequestNumber { get; set; }


        public SubscriberProfileM SubscriberProfile { get; set; }
        public SubscriberProfileM SubscriberProfileBefore { get; set; }

        [Display(Name = "Itemised Billing")]
        public bool NewItemisedBilling { get; set; }

        public decimal OldDeposit { get; set; }
        [Required]
        [Display(Name = "New Deposit")]
        public decimal NewDeposit { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }
    }
    public class ServiceRequestVM
    {
        public long ServiceRequestId { get; set; }
        public string Category { get; set; }
        public string Assignee { get; set; }
        public string Priority { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string MSISDN { get; set; }
        public string AccountName { get; set; }
    }
    public class ServiceRequestVMListData : DataTableModel
    {
        public List<ServiceRequestVM> aaData;
    }

    public class ServiceRequestAttachment
    {
        public long AttachmentId { get; set; }
        public long ServiceRequestId { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string Type { get; set; }
    }

    public class ServiceRequestNote
    {
        public long NoteId { get; set; }
        public string Note { get; set; }
    }
    public class ServiceRequestNoteData : DataTableModel
    {
        public List<ServiceRequestNote> aaData;
    }
    
    public class ServiceRequestActivity
    {
        public long ActivityId { get; set; }
        [Display(Name = "Created")]
        public DateTime VisitDateTime { get; set; }
        [Display(Name = "Created By")]
        public string FieldStaff { get; set; }
        public string Status { get; set; }
        [Display(Name = "Description")]
        public string Notes { get; set; }
        public string VisitDateTimeText { get; set; }
    }
    public class ServiceRequestActivityData : DataTableModel
    {
        public List<ServiceRequestActivity> aaData;
    }


    public class SubscriberProfileM
    {
        [Display(Name = "Salutation")]
        public string salutation { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; }
        [Display(Name = "Date of Birth")]
        public DateTime? birthDate { get; set; }
        [Display(Name = "IC Number")]
        public string ic { get; set; }
        [Display(Name = "Preferred Language")]
        public string preferredLanguage { get; set; }
        [Display(Name = "Address Line 1")]
        public string postalAddress { get; set; }
        [Display(Name = "Address Line 2")]
        public string postalAddressL2 { get; set; }
        [Display(Name = "City")]
        public string city { get; set; }
        [Display(Name = "Postal Code")]
        public string postalCode { get; set; }
        [Display(Name = "State")]
        public string state { get; set; }
        [Display(Name = "Email")]
        public string emailAddress { get; set; }
    }

}