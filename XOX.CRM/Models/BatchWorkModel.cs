using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class BatchWork
    {
        public long BatchWorkId { get; set; }
        [Display(Name = "Start At")]
        public DateTime? StartAt { get; set; }
        [Display(Name = "End At")]
        public DateTime? EndAt { get; set; }
        [Display(Name = "Job")]
        public string JobName { get; set; }
        [Display(Name = "Type")]
        public string JobType { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "Run Sequence")]
        public int RunSequence { get; set; }
        public long Frequency { get; set; }
        [Display(Name = "Send Notif")]
        public bool SendEmailNotifFlag { get; set; }
        public int Status { get; set; }

        public string FrequencyType { get; set; }
        public string StatusText { get; set; }
        public string StartAtText { get; set; }
        public string EndAtText { get; set; }
        public string Created { get; set; }

        public List<string> AllJobType { get; set; }
        public List<string> AllLogFilename { get; set; }
    }
    public class BatchWorkListData : DataTableModel
    {
        public List<BatchWork> aaData;
    }
    
    public class BatchWorkLog
    {
        public string Created { get; set; }
        public BatchWork BatchWork { get; set; }
        public string Description { get; set; }
        [Display(Name = "At Sequence")]
        public long RunSequence { get; set; }
        [Display(Name = "Job Status")]
        public string JobStatus { get; set; }
        public string Filename { get; set; }
    }
    public class BatchWorkLogListData : DataTableModel
    {
        public List<BatchWorkLog> aaData;
    }
    public class BatchWorkEmail
    {
        public long BatchWorkEmailId { get; set; }
        public long BatchWorkId { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
    public class BatchWorkEmailListData : DataTableModel
    {
        public List<BatchWorkEmail> aaData;
    }

}