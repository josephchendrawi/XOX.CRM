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
    
    public partial class XOX_T_ORDER
    {
        public long ROW_ID { get; set; }
        public Nullable<long> CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public Nullable<long> LAST_UPD_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPD { get; set; }
        public string ORDER_NUM { get; set; }
        public string ORDER_TYPE { get; set; }
        public string ORDER_STATUS { get; set; }
        public Nullable<System.DateTime> ORDER_SUBMIT_DT { get; set; }
        public Nullable<System.DateTime> PREF_INSTALL_DT { get; set; }
        public string ASSIGNEE { get; set; }
        public string ORDER_SOURCE { get; set; }
        public string CAMPAIGN_CD { get; set; }
        public string ORDER_SUBMITTED_BY { get; set; }
        public string CUST_REP_ID { get; set; }
        public Nullable<long> VERIFICATION_ID { get; set; }
        public string MSISDN { get; set; }
        public string CATEGORY { get; set; }
        public string PLAN { get; set; }
        public string REMARKS { get; set; }
        public string REJECT_REASONS { get; set; }
        public string PORT_REQ_FORM { get; set; }
        public string PORT_ID { get; set; }
    }
}