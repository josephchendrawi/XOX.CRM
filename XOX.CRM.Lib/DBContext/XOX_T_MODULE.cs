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
    
    public partial class XOX_T_MODULE
    {
        public long ROW_ID { get; set; }
        public Nullable<long> CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public Nullable<long> LAST_UPD_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPD { get; set; }
        public string MODULE_CD { get; set; }
        public string MODULE_NM { get; set; }
        public Nullable<int> IS_VIEWABLE { get; set; }
        public Nullable<int> IS_ADDABLE { get; set; }
        public Nullable<int> IS_EDITABLE { get; set; }
        public Nullable<int> IS_DELETABLE { get; set; }
        public Nullable<int> IS_APPROVABLE { get; set; }
        public Nullable<int> IS_REJECTABLE { get; set; }
    }
}
