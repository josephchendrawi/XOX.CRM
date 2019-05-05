using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class BatchWorkVO
    {
        public long BatchWorkId { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string JobName { get; set; }
        public string JobType { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public int Status { get; set; }
        public DateTime? Created { get; set; }
        public int RunSequence { get; set; }
        public long Frequency { get; set; }
        public bool SendEmailNotifFlag { get; set; }
    }

    public class BatchWorkLogVO
    {
        public DateTime? Created { get; set; }
        public BatchWorkVO BatchWork { get; set; }
        public string Description { get; set; }
        public long RunSequence { get; set; }
        public string JobStatus { get; set; }

        public long BatchWorkId { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public string JobType { get; set; }
        public string FileName { get; set; }
    }

    public class BatchWorkEmailVO
    {
        public long BatchWorkEmailId { get; set; }
        public long BatchWorkId { get; set; }
        public string Email { get; set; }
    }

    public class BatchWorkLogStatistic
    {
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public int TotalCount { get; set; }
    }

}
