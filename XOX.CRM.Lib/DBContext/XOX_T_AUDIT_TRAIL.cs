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
    
    public partial class XOX_T_AUDIT_TRAIL
    {
        public long ROW_ID { get; set; }
        public Nullable<long> CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public Nullable<long> LAST_UPD_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPD { get; set; }
        public Nullable<long> AUDIT_ROW_ID { get; set; }
        public string ACTION_CD { get; set; }
        public string OLD_VAL { get; set; }
        public string NEW_VAL { get; set; }
        public string MODULE_NAME { get; set; }
        public string SCREEN_NAME { get; set; }
        public string FIELD_NAME { get; set; }
    }
}
