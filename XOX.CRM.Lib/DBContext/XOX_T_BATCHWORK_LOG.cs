//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace XOX.CRM.Lib.DBContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class XOX_T_BATCHWORK_LOG
    {
        public long ROW_ID { get; set; }
        public Nullable<long> CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public Nullable<long> BATCHWORK_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<long> RUN_SEQUENCE { get; set; }
        public string JOB_STATUS { get; set; }
        public string FILE_NAME { get; set; }
    }
}
